using Armoire.Common;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Linq;
using System.Security.Claims;

namespace Armoire.Infrastructure
{
    public static class ControllerExtensions
    {
        public static UserDto GetCurrentUser(this Controller c, IUserService userService)
        {
            if (!c.User.Identity.IsAuthenticated) return null;
            return userService.Get(
                Convert.ToInt32(
                    c.User.FindFirstValue(
                        AppConstants.CLAIM_TYPE_USER_ID)));
        }

        public static string GetUserClaim(this Controller c, string claimType)
        {
            if (!c.User.Identity.IsAuthenticated) return null;
            return c.User.FindFirstValue(claimType);
        }

        public static string[] GetUserClaimList(this Controller c, string claimType)
        {
            if (!c.User.Identity.IsAuthenticated) return null;
            return c.User.FindAll(claimType).ToList().Select(x => x.Value).ToArray();
        }

        public static JsonResult SuccessResult(this Controller c, string message = null)
        {
            return new JsonResult(new { Success = true, Message = message ?? String.Empty })
            {
                ContentType = AppConstants.MimeTypes.JSON
                // JsonRequestBehavior = JsonRequestBehavior.AllowGet
            };
        }

        public static JsonResult FailureResult(this Controller c, string message)
        {
            return new JsonResult(new { Success = false, Message = message })
            {
                ContentType = AppConstants.MimeTypes.JSON
                // JsonRequestBehavior = JsonRequestBehavior.AllowGet
            };
        }
    }
}
