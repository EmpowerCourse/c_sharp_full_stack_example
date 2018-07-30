using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Armoire.Common
{
    public class UserDto
    {
        public int Id { get; set; }
        public string Username { get; set; }
        public string PasswordHash { get; set; }
        public string Salt { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
        public string Phone { get; set; }

        //
        private IList<TypeOfUserRole> _roles;
        public IList<TypeOfUserRole> Roles
        {
            get { return _roles; }
            set
            {
                if (value != null)
                {
                    _roles = value.ToList();
                }
                else
                {
                    _roles = new List<TypeOfUserRole>();
                }
            }
        }

        [JsonIgnore]
        public string RoleInformation
        {
            get
            {
                return String.Join(",", Roles);
            }
        }

        // mapped/calculated/flattened properties
        public bool IsActive { get; set; }
        public DateTime? SessionLastValidatedAt { get; set; }
        public Guid? SessionTokenValue { get; set; }

        [JsonIgnore]
        public bool IsAdmin
        {
            get
            {
                return Roles != null && Roles.Contains(TypeOfUserRole.Administrator);
            }
        }

        [JsonIgnore]
        public string FullName
        {
            get
            {
                return String.Format("{0} {1}", FirstName, LastName);
            }
        }
    }
}
