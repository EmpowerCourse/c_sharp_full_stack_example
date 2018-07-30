using Armoire.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace Armoire.Common
{
    public class UserVM
    {
        // common, managed
        public int Id { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
        public string Username { get; set; }
        public string Phone { get; set; }
        public bool IsActive { get; set; }
        public IList<TypeOfUserRole> Roles { get; set; }

        // viewmodel fields
        public string InitialPassword { get; set; }
        public SelectList AvailableRoles { get; set; }
        public IList<RoleSelection> RoleSelection { get; set; }
        public string RoleInformation
        {
            get
            {
                if (Roles == null) return String.Empty;
                return String.Join(",", Roles);
            }
        }

        public string FullName
        {
            get
            {
                return String.Format("{0} {1}", FirstName, LastName);
            }
        }
    }
}
