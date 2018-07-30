using Armoire.Common;
using System;
using System.Collections.Generic;
using System.Text;

namespace Armoire.Entities
{
    public class UserRole : Entity
    {
        public virtual int UserId { get; set; }
        public virtual TypeOfUserRole TypeOfUserRole { get; set; }
    }
}
