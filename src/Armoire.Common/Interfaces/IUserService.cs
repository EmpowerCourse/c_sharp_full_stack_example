using System;
using System.Collections.Generic;
using System.Text;

namespace Armoire.Common
{
    public interface IUserService
    {
        AuthenticationResultDto AttemptAuthentication(string Username, string Password, System.Net.IPAddress clientIP);
        UserDto Get(int userId);
        IList<UserDto> Find(UserSearchCriteria criteria);
        PaginatedList<UserDto> GetPaginatedList(UserSearchCriteria criteria, int page);
        UserDto Add(UserDto dto, string initialPassword, int? byUserId = null);
        void ResetAndSendPassword(int userId);
        void SetPassword(int userId, string oldPassword, string newPassword);
        void UpdateProfile(UserDto dto);
        void Update(UserDto dto, int byUserId);
        void ToggleUserActive(int userId, int byUserId);
        bool UserExists(string username);
        UserDto GetUserByUserName(string username);
    }
}
