using Armoire.Common;
using System;
using System.Collections.Generic;
using System.Text;

namespace Armoire.Entities
{
    public class User: Entity
    {
        public virtual string Username { get; set; }
        public virtual string PasswordHash { get; set; }
        public virtual string FirstName { get; set; }
        public virtual string LastName { get; set; }
        public virtual string Email { get; set; }
        public virtual string Phone { get; set; }
        public virtual string Salt { get; set; }
        public virtual DateTime? DeactivatedAt { get; set; }
        public virtual DateTime CreatedAt { get; set; }
        public virtual DateTime LastUpdatedAt { get; set; }
        public virtual ICollection<UserRole> RoleList { get; set; }
    }
}
