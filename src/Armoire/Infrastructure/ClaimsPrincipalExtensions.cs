using Armoire.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace Armoire
{
    public static class ClaimsPrincipalExtensions
    {
        private static string ADMIN_ROLE_ID = ((int)TypeOfUserRole.Administrator).ToString();
        public static bool IsAdministrator(this ClaimsPrincipal cp)
        {
            if (!cp.Identity.IsAuthenticated) return false;
            // return cp.FindFirstValue(ClaimTypes.Role).Split(',').Select(x => Convert.ToInt32(x)).Contains((int)TypeOfUserRole.Administrator);
            return cp.FindAll(ClaimTypes.Role).Any(x => x.Value == ADMIN_ROLE_ID);
        }

        public static int Id(this ClaimsPrincipal cp)
        {
            Claim claimValue = cp.FindFirst(AppConstants.CLAIM_TYPE_USER_ID);
            if (claimValue == null) return 0;
            return Convert.ToInt32(claimValue.Value);
        }
    }
}
