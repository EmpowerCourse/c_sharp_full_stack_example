using Armoire.Common;
using System;
using System.Collections.Generic;
using System.Text;

namespace Armoire.Entities
{
    public class Patron: Entity
    {
        public virtual string Username { get; set; }
        public virtual string Password { get; set; }
        public virtual string FirstName { get; set; }
        public virtual string LastName { get; set; }
        public virtual string Email { get; set; }
        public virtual string Address { get; set; }
        public virtual string City { get; set; }
        public virtual string State { get; set; }
        public virtual string Zip { get; set; }
        public virtual string Phone { get; set; }
        public virtual int AccountType { get; set; }
        public virtual byte[] Image { get; set; }
        public virtual string AccountStatus { get; set; }
        public virtual DateTime DateCreated { get; set; }
        public virtual DateTime LastUpdated { get; set; }
    }
}
