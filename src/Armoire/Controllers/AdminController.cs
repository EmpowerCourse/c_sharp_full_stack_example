using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Armoire.Common;
using Armoire.Infrastructure;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.ViewEngines;

namespace Armoire.Controllers
{
    [Authorize(Policy = "Administrators")]
    public class AdminController: Controller
    {
        private IUserService _userService;
        private IAutomapperMapping _mapper;
        private ISettingsService _settingService;
        // private IViewRenderService _viewRenderService;

        public AdminController(IUserService userService, ISettingsService settingService, IAutomapperMapping mapper)
        {
            _mapper = mapper;
            _userService = userService;
            _settingService = settingService;
            // _viewRenderService = viewRenderService;
        }

        public IActionResult Users()
        {
            ViewData.Model = new UserListVM()
            {
                RoleList = TypeOfUserRole.Administrator.ToSimpleDtoList()
                            .InsertItemAtFrontOfList(new SimpleDto()
                            {
                                Id = AppConstants.PLACEHOLDER_ROW_ID,
                                Name = "[Any Role]"
                            }),
                UserList = new CustomPagination<UserDto>(new List<UserDto>(), 1, AppConstants.UNPAGED_PAGE_SIZE_IN_ROWS, 0)
            };
            ViewData["sort"] = new GridSortOptions();
            return View();
        }

        public virtual ActionResult NewUser()
        {
            var viewModel = new UserVM();
            populateUserVMLists(viewModel);
            viewModel.RoleSelection = getRolesAvailable(false)
                    .Select(x => new RoleSelection()
                    {
                        Role = (TypeOfUserRole)x.Id,
                        Selected = true // default select all
                    }).OrderBy(x => x.Role.Description()).ToList();
            ViewData.Model = viewModel;
            return PartialView("_EditUser");
        }

        public virtual ActionResult EditUser(int id)
        {
            var dto = _userService.Get(id);
            if (!User.IsAdministrator())
            {
                throw new ApplicationException(AppConstants.ERR_ACCESS_DENIED);
            }
            var viewModel = _mapper.Map<UserDto, UserVM>(dto);
            populateUserVMLists(viewModel);
            ViewData.Model = viewModel;
            return PartialView("_EditUser");
        }

        [HttpPost]
        public virtual ActionResult ToggleUserActive(int id)
        {
            if (id == AppConstants.PLACEHOLDER_ROW_ID) return this.FailureResult("Cannot delete this user");
            if (!User.IsAdministrator()) throw new ApplicationException(AppConstants.ERR_ACCESS_DENIED);
            try
            {
                _userService.ToggleUserActive(id, User.Id());
                return this.SuccessResult();
            }
            catch (Exception ex)
            {
                return this.FailureResult(ex.Message);
            }
        }

        private static IList<SimpleDto> getRolesAvailable(bool allowAnySelection = true)
        {
            IList<SimpleDto> roleFilterList = new List<SimpleDto>()
            {
                TypeOfUserRole.Viewer.ToSimpleDto(),
                TypeOfUserRole.Administrator.ToSimpleDto()
            };
            if (allowAnySelection)
            {
                roleFilterList.InsertItemAtFrontOfList(
                        new SimpleDto()
                        {
                            Id = AppConstants.PLACEHOLDER_ROW_ID,
                            Name = "[Any Role]"
                        });
            }
            return roleFilterList;
        }

        private void populateUserVMLists(UserVM viewModel)
        {
            var rolesAvailable = getRolesAvailable(false);
            viewModel.AvailableRoles = new SelectList(rolesAvailable, "Id", "Name");
            if (viewModel.Roles == null) viewModel.Roles = new List<TypeOfUserRole>();
            viewModel.RoleSelection = rolesAvailable
                    .Select(x => new RoleSelection() { Role = (TypeOfUserRole)x.Id, Selected = viewModel.Roles.Contains((TypeOfUserRole)x.Id) })
                    .OrderBy(x => x.Role.Description()).ToList();
        }

        public virtual ActionResult SaveUser(UserVM vm)
        {
            if (!User.IsAdministrator()) throw new ApplicationException(AppConstants.ERR_ACCESS_DENIED);
            var currentUser = this.GetCurrentUser(_userService);
            var dto = _mapper.Map<UserVM, UserDto>(vm);
            if (vm.RoleSelection == null) vm.RoleSelection = new List<RoleSelection>();
            dto.Roles = vm.RoleSelection.Where(x => x.Selected).Select(x => x.Role).ToList();
            try
            {
                if (vm.Id > 0)
                {
                    _userService.Update(dto, currentUser.Id);
                }
                else
                {
                    dto = _userService.Add(dto, vm.InitialPassword, currentUser.Id);
                }
            }
            catch (Exception ex)
            {
                if (ex.InnerException != null &&
                    ex.InnerException.Message.ToLower().Contains("cannot insert duplicate key"))
                {
                    return this.FailureResult(
                        String.Format(
                            "{0} {1} [{2}] has already been defined and cannot be added a second time.\nPlease find and update the existing user if changes need to be made.",
                                dto.FirstName, dto.LastName, dto.Username)
                    );
                }
                // _logger.Error("error during AdministrationController.SaveUser", ex);
                return this.FailureResult(ex.Message);
            }
            return this.SuccessResult();
        }

        [HttpPost]
        public virtual ActionResult FilterUsers(int? roleId, string name, bool activeOnly, GridSortOptions gridSortOptions, int? page)
        {
            TypeOfUserRole? role = null;
            if (roleId.HasValue && roleId.Value > 0)
            {
                role = (TypeOfUserRole)roleId.Value;
            }
            if (String.IsNullOrWhiteSpace(name)) name = null;
            UserSearchCriteria criteria = new UserSearchCriteria()
            {
                Name = name,
                MemberOfRole = role,
                ActiveOnly = activeOnly
            };
            var viewModel = getPaginatedUserList(page, criteria, gridSortOptions);
            // ViewData["sort"] = gridSortOptions;
            return PartialView("_UserList", viewModel);
        }

        private IPagination<UserDto> getPaginatedUserList(int? page, UserSearchCriteria criteria = null, GridSortOptions gridSortOptions = null)
        {
            // string column = null;
            // var direction = SortDirection.Ascending;
            int pageNo = page.HasValue ? page.Value : 1;
            //if (gridSortOptions != null)
            //{
            //    column = gridSortOptions.Column;
            //    direction = gridSortOptions.Direction;
            //}
            var paginatedList = _userService.GetPaginatedList(criteria, pageNo); // , column, direction
            return new CustomPagination<UserDto>(
                paginatedList.Items, paginatedList.PageNo,
                    _settingService.GetIntValue("DefaultPageSizeInRows"), paginatedList.TotalCount);
        }

    }
}