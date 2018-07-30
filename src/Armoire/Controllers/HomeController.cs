using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Armoire.Models;
using NHibernate;
using Armoire.Common;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using System.Security.Claims;
using Armoire.Infrastructure;

namespace Armoire.Controllers
{
    public class HomeController : Controller
    {
        private IUserService _userService;
        private IAutomapperMapping _mapper;

        public HomeController(IUserService userService, IAutomapperMapping mapper)
        {
            _userService = userService;
            _mapper = mapper;
        }

        public IActionResult Index()
        {
            return View();
        }

        public IActionResult About()
        {
            // light weight user claim lookup that avoids any call to the database
            var currentUserFullName = this.GetUserClaim(AppConstants.CLAIM_TYPE_USER_FULL_NAME);
            var currentUserRoleIdCSV = this.GetUserClaimList(ClaimTypes.Role).Select(x => Convert.ToInt32(x)).ToArray();
            var roleNameList = String.Join(',', TypeOfUserRole.Administrator.ToSimpleDtoList().Where(x => currentUserRoleIdCSV.Contains(x.Id)).Select(x => x.Name));
            ViewData["Message"] = currentUserFullName == null 
                ? "Not authenticated" 
                : currentUserFullName + " is signed in and is associated with the following roles: " + roleNameList;
            return View();
        }

        public IActionResult Contact()
        {
            // provides access to any user field persisted to the db
            var currentUser = this.GetCurrentUser(_userService);
            ViewData["Message"] = currentUser == null ? "Not authenticated" : currentUser.FullName + " is signed in";
            return View();
        }

        public IActionResult Login()
        {
            var vm = TempData.Get<LoginVM>("LoginVM");
            ViewData.Model = vm ?? new LoginVM();
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Login(string Username, string Password)
        {
            AuthenticationResultDto result = _userService.AttemptAuthentication(Username, Password, this.Request.HttpContext.Connection.RemoteIpAddress);
            if (result.Success)
            {
                var claims = new List<Claim> {
                    new Claim(AppConstants.CLAIM_TYPE_USER_ID, result.User.Id.ToString()),
                    new Claim(ClaimTypes.Name, result.User.Username),
                    new Claim(AppConstants.CLAIM_TYPE_USER_FULL_NAME, result.User.FullName)
                };
                // new Claim(ClaimTypes.Role, String.Join(",", result.User.Roles.Select(x => (int)x))),
                foreach(var r in result.User.Roles)
                {
                    claims.Add(
                        new Claim(ClaimTypes.Role, 
                            ((int)r).ToString()));
                }
                var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
                var authProperties = new AuthenticationProperties
                {
                    AllowRefresh = true,
                    ExpiresUtc = DateTimeOffset.UtcNow.AddMinutes(180),
                    IsPersistent = true,
                    IssuedUtc = DateTimeOffset.UtcNow
                };
                await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme,
                    new ClaimsPrincipal(claimsIdentity), authProperties);
                return await Task.Run<RedirectToActionResult>(() =>
                {
                    return RedirectToAction("Index");
                });
            }
            else
            {
                TempData.Put("LoginVM", new LoginVM() {
                    Username = Username,
                    Password = Password,
                    Message = result.ErrorMessage
                });
                return await Task.Run<RedirectToActionResult>(() =>
                {
                    return RedirectToAction("Login");
                });
            }
        }

        public async Task<IActionResult> LogoutAsync()
        {
            await HttpContext.SignOutAsync(
                CookieAuthenticationDefaults.AuthenticationScheme);
            return RedirectToAction("Login");
        }

        
        public IActionResult Register()
        {
            var vm = TempData.Get<RegisterVM>("RegisterVM");
            ViewData.Model = vm ?? new RegisterVM();
            return View();
        }

        [HttpPost]
        public IActionResult Register(RegisterVM viewModel)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    var dto = _mapper.Map<RegisterVM, UserDto>(viewModel);
                    dto = _userService.Add(dto, viewModel.Password);
                    TempData.Put("LoginVM", new LoginVM() { Message = "You've successfully registered, please log in" });
                    return RedirectToAction(nameof(HomeController.Login), "Home");
                }
                else {
                    throw new ApplicationException("Registration could not be completed due to invalid data.");
                }
            }
            catch (ApplicationException aex)
            {
                viewModel.Message = aex.Message;
                TempData.Put("RegisterVM", viewModel);
                return RedirectToAction(nameof(HomeController.Register), "Home");
            }
            catch (Exception ex)
            {
                TempData["Message"] = ex.Message;
                return RedirectToAction(nameof(HomeController.Error), "Home");
            }
        }

        public IActionResult Error()
        {
            return View(new ErrorVM {
                RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier,
                Message = (string)TempData["Message"]
            });
        }
    }
}
