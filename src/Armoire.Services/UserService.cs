using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using Armoire.Common;
using Armoire.Entities;
using NHibernate;
using NHibernate.Linq;

namespace Armoire.Services
{
    public class UserService: IUserService
    {
        private readonly ISession _nhSession;
        private readonly IAutomapperMapping _mapper;
        private readonly ICipherService _cipherService;
        private readonly IRepository<User> _userRepository;
        private readonly IUnitOfWork _unitOfWork;
        private readonly INotificationService _notificationService;
        private readonly ISettingsService _settingService;

        public UserService(ISession nhSession, IAutomapperMapping mapper, ICipherService cipherService, IRepository<User> userRepository, IUnitOfWork unitOfWork,
            INotificationService notificationService, ISettingsService settingService)
        {
            _nhSession = nhSession;
            _mapper = mapper;
            _cipherService = cipherService;
            _userRepository = userRepository;
            _unitOfWork = unitOfWork;
            _notificationService = notificationService;
            _settingService = settingService;
        }

        public AuthenticationResultDto AttemptAuthentication(string Username, string Password, System.Net.IPAddress clientIP)
        {
            AuthenticationResultDto result = new AuthenticationResultDto();
            bool success = false;
            var user = _userRepository.GetQuery().Where(x => x.Username == Username).FirstOrDefault();
            if (user == null)
            {
                result.ErrorMessage = "Invalid username";
            }
            else
            {
                if (user.DeactivatedAt == null)
                {
                    success = _cipherService.SHA256HashMatches(Password, user.Salt, user.PasswordHash);
                    if (!success)
                    {
                        result.ErrorMessage = "Invalid password";
                    }
                }
                else
                {
                    result.ErrorMessage = "This user account is inactive.  Contact an administrator.";
                }
            }
            _nhSession.Save(new AuthenticationAttempt()
            {
                OccurredAt = DateTime.UtcNow,
                Username = Username,
                WasSuccessful = success,
                ClientIP = clientIP.ToString()
            });
            result.User = _mapper.Map<User, UserDto>(user);
            return result;
        }

        public UserDto Get(int userId)
        {
            return _mapper.Map<User, UserDto>(
                _userRepository.Get(userId));
        }

        private IQueryable<User> resolveQuery(UserSearchCriteria criteria)
        {
            var query = _userRepository.GetQuery();
            if (criteria.ActiveOnly)
                query = query.Where(x => x.DeactivatedAt == null);
            if (criteria.IdList != null && criteria.IdList.Any())
            {
                int[] IdArray = criteria.IdList.ToArray();
                query = query.Where(x => IdArray.Contains(x.Id));
            }
            if (!String.IsNullOrWhiteSpace(criteria.Name))
                query = query.Where(x => (x.FirstName + " " + x.LastName).Contains(criteria.Name) || x.Username.Contains(criteria.Name));
            if (!String.IsNullOrWhiteSpace(criteria.Username))
                query = query.Where(x => x.Username == criteria.Username);
            if (!String.IsNullOrWhiteSpace(criteria.Email))
                query = query.Where(x => x.Email == criteria.Email);
            if (criteria.MemberOfRole.HasValue)
                query = query.Where(x => x.RoleList.Any(y => y.TypeOfUserRole == criteria.MemberOfRole.Value));
            return query;
        }

        public IList<UserDto> Find(UserSearchCriteria criteria)
        {
            return _mapper.Map<IList<User>, IList<UserDto>>(
                resolveQuery(criteria).ToList());
        }

        private void validateUserModel(User user, IList<TypeOfUserRole> proposedRoleList)
        {
            HashSet<string> validationErrors = new HashSet<string>();
            if (String.IsNullOrWhiteSpace(user.Username))
                validationErrors.Add("Username is required");
            if (String.IsNullOrWhiteSpace(user.FirstName)
                || String.IsNullOrWhiteSpace(user.LastName))
                validationErrors.Add("Both first and last name are required");
            //if (!_notificationService.EmailAddressIsValid(user.Email))
            //    validationErrors.Add(AppConstants.ERR_INVALID_EMAIL);
            if (String.IsNullOrWhiteSpace(user.PasswordHash)
                || String.IsNullOrWhiteSpace(user.Salt))
                validationErrors.Add("The proposed password value is not valid");
            if (proposedRoleList == null || proposedRoleList.Count == 0)
                validationErrors.Add("The provided user has an invalid associated list of roles");
            if (validationErrors.Any())
                throw new ApplicationException(String.Join(",", validationErrors));
        }

        public UserDto Add(UserDto dto, string initialPassword, int? byUserId = null)
        {
            try
            {
                _unitOfWork.Begin();
                var user = new User();
                pushUpdatableUserProfileToModel(user, dto);
                validatePassword(initialPassword);
                // these properties are only set once upon user creation
                user.CreatedAt = DateTime.UtcNow;
                user.Salt = _cipherService.GenerateSalt();
                user.PasswordHash = _cipherService.ComputeSHA256Hash(initialPassword, user.Salt);
                // default to just a viewer
                dto.Roles = new List<TypeOfUserRole>() { TypeOfUserRole.Viewer };
                validateUserModel(user, dto.Roles);
                if (UserExists(dto.Username))
                {
                    throw new ApplicationException($"The username {dto.Username} has already been taken.  Please either register with another username or login if you have already registered.");
                }
                user = _userRepository.Add(user);
                setUserRoles(user, dto.Roles);
                var newUserDto = _mapper.Map<User, UserDto>(user);
                _unitOfWork.Commit();
                // _userRepository.AddSystemLogEntry(byUserId, "Created user " + newUserDto.FullName);
                return newUserDto;
            }
            catch (Exception ex)
            {
                _unitOfWork.Rollback();
                // _logger.Error("Error during user addition", ex);
                throw;
            }
        }

        private static void validatePassword(string v)
        {
            if (String.IsNullOrWhiteSpace(v))
            {
                throw new ApplicationException("A password value is required.");
            }
            if (!Regex.IsMatch(v, AppConstants.PASSWORD_COMPLEXITY_REGEX))
            {
                throw new ApplicationException("The password provided does not meet the minimum complexity requirements (" +
                    AppConstants.PASSWORD_COMPLEXITY_REGEX_EXPLANATION + ")");
            }
            // implement more complexity rules here if so required...
        }

        private static char[] getRandomCharactersOfLength(int charCount)
        {
            char[] discardCharacters = { 'O', '0', 'o', ' ', '\\', '/', ';' };
            StringBuilder sb = new StringBuilder(charCount);
            int charsAdded = 0;
            Random rnd = new Random();
            while (charsAdded < charCount)
            {
                char proposed = (char)rnd.Next(47, 95);
                if (discardCharacters.Contains(proposed)) continue;
                sb.Append(proposed);
                charsAdded++;
            }
            return sb.ToString().ToCharArray();
        }

        private static string generatePasswordOfLength(int charCount)
        {
            Random rnd = new Random();
            const int specialCharDenominator = 5;
            bool hasLower = false;
            char[] result = new char[charCount];
            char[] randomBytes = getRandomCharactersOfLength(charCount);
            char[] specialCharacters = { '%', '_', '-', '$', '#' };
            for (int i = 0; i < charCount; i++)
            {
                if (i > 0 && i % specialCharDenominator == 0)
                {
                    result[i] = specialCharacters[rnd.Next(0, specialCharacters.Length)];
                }
                else
                {
                    if (Char.IsLetter(randomBytes[i]))
                    {
                        if (hasLower)
                        {
                            result[i] = Char.ToUpper(randomBytes[i]);
                        }
                        else
                        {
                            result[i] = Char.ToLower(randomBytes[i]);
                            hasLower = true;
                        }
                    }
                    else
                        result[i] = randomBytes[i];
                }
            }
            return new String(result);
        }

        private string generateRandomPassword(string regExMinRequirements)
        {
            string pwd = String.Empty;
            for (int attempts = 0; attempts < 20; attempts++)
            {
                pwd = generatePasswordOfLength(8 + attempts);
                if (Regex.IsMatch(pwd, regExMinRequirements))
                {
                    break;
                }
            }
            if (pwd.Length == 0)
            {
                throw new Exception("Could not generate a password compatible with {" + regExMinRequirements + "}");
            }
            return pwd;
        }

        public void ResetAndSendPassword(int userId)
        {
            bool wasInTransaction = _unitOfWork.IsInTransaction();
            try
            {
                if (!wasInTransaction) _unitOfWork.Begin();
                User user = _userRepository.Get(userId);
                if (user == null) throw new ApplicationException("The specified user could not be found!");
                string newPassword = generateRandomPassword(AppConstants.PASSWORD_COMPLEXITY_REGEX);
                user.PasswordHash = _cipherService.ComputeSHA256Hash(newPassword, user.Salt);
                _userRepository.Update(user);
                _notificationService.SendEmailNow(new NotificationDto()
                {
                    RecipientAddress = user.Email,
                    Body = "<p>Your password has been reset to the following value and is available for immediate logon.  It is strongly recommended that you change your password to something memorable by clicking your name at the top, right once you log in with this one.</p><br/><span style='margin:12px;font-weight:bold;font-size:14pt;'>" + newPassword + "</span>",
                    Subject = AppConstants.APP_NAME + " Password Reset"
                });
                // _logger.Warn("A password reset and reminder was performed for user id " + userId);
                if (!wasInTransaction) _unitOfWork.Commit();
            }
            catch (Exception ex)
            {
                if (!wasInTransaction) _unitOfWork.Rollback();
                // _logger.Error("Error during user ResetAndSendPassword", ex);
                throw;
            }
        }

        public void SetPassword(int userId, string oldPassword, string newPassword)
        {
            // if (userId == AppConstants.SYSTEM_USER_ID) return;
            bool wasInTransaction = _unitOfWork.IsInTransaction();
            try
            {
                if (!wasInTransaction) _unitOfWork.Begin();
                User user = _userRepository.Get(userId);
                if (user == null) throw new ApplicationException("The specified user could not be found!");
                if (_cipherService.SHA256HashMatches(oldPassword, user.Salt, user.PasswordHash))
                {
                    validatePassword(newPassword);
                    user.PasswordHash = _cipherService.ComputeSHA256Hash(newPassword, user.Salt);
                }
                else
                {
                    throw new ApplicationException("The provided, existing password is incorrect");
                }
                _userRepository.Update(user);
                if (!wasInTransaction) _unitOfWork.Commit();
            }
            catch (Exception ex)
            {
                if (!wasInTransaction) _unitOfWork.Rollback();
                // _logger.Error("Error during user profile update", ex);
                throw;
            }
        }

        public void UpdateProfile(UserDto dto)
        {
            // if (dto.Id == AppConstants.SYSTEM_USER_ID) return;
            bool wasInTransaction = _unitOfWork.IsInTransaction();
            try
            {
                if (!wasInTransaction) _unitOfWork.Begin();
                User user = _userRepository.Get(dto.Id);
                if (user != null)
                {
                    pushUpdatableUserProfileToModel(user, dto);
                    validateUserModel(user, dto.Roles);
                    _userRepository.Update(user);
                }
                if (!wasInTransaction) _unitOfWork.Commit();
            }
            catch (Exception ex)
            {
                if (!wasInTransaction) _unitOfWork.Rollback();
                // _logger.Error("Error during user profile update", ex);
                throw;
            }
        }

        public void Update(UserDto dto, int byUserId)
        {
            // if (dto.Id == AppConstants.SYSTEM_USER_ID) return;
            bool wasInTransaction = _unitOfWork.IsInTransaction();
            try
            {
                if (!wasInTransaction) _unitOfWork.Begin();
                User user = _userRepository.Get(dto.Id);
                if (user != null)
                {
                    pushUpdatableUserProfileToModel(user, dto);
                    validateUserModel(user, dto.Roles);
                    _userRepository.Update(user);
                    setUserRoles(user, dto.Roles);
                }
                if (!wasInTransaction) _unitOfWork.Commit();
            }
            catch (Exception ex)
            {
                if (!wasInTransaction) _unitOfWork.Rollback();
                // _logger.Error("Error during user update", ex);
                throw;
            }
        }

        private void setUserRoles(User user, IList<TypeOfUserRole> desiredRoles)
        {
            if (user.RoleList == null)
            {
                user.RoleList = new List<UserRole>();
            }
            if (desiredRoles == null)
            {
                desiredRoles = new List<TypeOfUserRole>();
            }
            var existingRoles = user.RoleList.Select(x => x.TypeOfUserRole).ToArray();
            // find and add any roles that don't already exist
            var newRoles = desiredRoles.Where(d => !existingRoles.Contains(d)).ToArray();
            // find any roles no longer in the desired list
            var oldRoles = existingRoles.Where(e => !desiredRoles.Contains(e)).ToArray();
            if (!newRoles.Any() && !oldRoles.Any()) return;
            foreach (var o in oldRoles)
            {
                var oldRoleModel = user.RoleList.First(x => x.TypeOfUserRole == o);
                user.RoleList.Remove(oldRoleModel);
                _nhSession.Delete(oldRoleModel);
            }
            foreach (var n in newRoles)
            {
                var newRoleModel = new UserRole()
                {
                    TypeOfUserRole = n,
                    UserId = user.Id
                };
                _nhSession.Save(newRoleModel);
                user.RoleList.Add(newRoleModel);
            }
        }

        private static void pushUpdatableUserProfileToModel(User model, UserDto dto)
        {
            model.FirstName = dto.FirstName;
            model.LastName = dto.LastName;
            model.Email = dto.Email ?? String.Empty;
            model.Username = dto.Username;
            model.Phone = dto.Phone ?? String.Empty;
            model.LastUpdatedAt = DateTime.UtcNow;
        }

        public void ToggleUserActive(int userId, int byUserId)
        {
            // if (userId == AppConstants.SYSTEM_USER_ID) return;
            bool wasInTransaction = _unitOfWork.IsInTransaction();
            try
            {
                if (!wasInTransaction) _unitOfWork.Begin();
                User user = _userRepository.Get(userId);
                if (user == null) throw new ApplicationException("The specified user could not be found!");
                // string itemDsc = user.FirstName + " " + user.LastName + " (" + user.Id + ")";
                if (user.DeactivatedAt == null)
                {
                    user.DeactivatedAt = DateTime.UtcNow;
                    // _userRepository.AddSystemLogEntry(byUserId, "deactivated user " + itemDsc);
                }
                else
                {
                    user.DeactivatedAt = null;
                    // _userRepository.AddSystemLogEntry(byUserId, "revived user " + itemDsc);
                }
                _userRepository.Update(user);
                if (!wasInTransaction) _unitOfWork.Commit();
            }
            catch (Exception ex)
            {
                if (!wasInTransaction) _unitOfWork.Rollback();
                // _logger.Error("Error during user toggle active", ex);
                throw;
            }
        }

        public bool UserExists(string username)
        {
            return GetUserByUserName(username) != null;
        }

        public UserDto GetUserByUserName(string username)
        {
            var query = _userRepository.GetQuery()
                .Where(x => x.Username == username);
            return _mapper.Map<User, UserDto>(
                query.FirstOrDefault());
        }

        public PaginatedList<UserDto> GetPaginatedList(UserSearchCriteria criteria, int page)
        {
            IQueryable<User> queryable = resolveQuery(criteria ?? new UserSearchCriteria());
            // var totalCountFuture = queryable.GroupBy(x => x.GroupingConstant).Select(x => x.Count()).ToFutureValue();
            var totalCountFuture = queryable.GroupBy(x => "all").Select(x => x.Count()).ToFutureValue();
            queryable = queryable.OrderBy(x => x.DeactivatedAt)
                    .ThenBy(x => x.LastName).ThenBy(x => x.FirstName);
            int pageSize = _settingService.GetIntValue("DefaultPageSizeInRows");
            int skipVal = pageSize * (page - 1);
            var results = queryable
                .Skip(skipVal)
                .Take(pageSize)
                    // add fetches
                    .FetchMany(x => x.RoleList)
                .ToList();
            return new PaginatedList<UserDto>()
            {
                Items = _mapper.Map<IList<User>, IList<UserDto>>(results),
                PageNo = page,
                TotalCount = totalCountFuture.Value
            };
        }
    }
}
