using Armoire.Common;
using System;
using System.Collections.Generic;
using System.Text;

namespace Armoire.Entities
{
    public class AuthenticationAttempt : Entity
    {
        public virtual string Username { get; set; }
        public virtual DateTime OccurredAt { get; set; }
        public virtual bool WasSuccessful { get; set; }
        public virtual string ClientIP { get; set; }
    }
}
