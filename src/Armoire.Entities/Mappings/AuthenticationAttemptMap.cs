using System;
using System.Collections.Generic;
using System.Text;
using FluentNHibernate.Mapping;

namespace Armoire.Entities.Mappings
{
    public class AuthenticationAttemptMap : ClassMap<AuthenticationAttempt>
    {
        public AuthenticationAttemptMap()
        {
            Table("AuthenticationAttempt");
            Id(x => x.Id);
            Map(x => x.OccurredAt);
            Map(x => x.Username);
            Map(x => x.WasSuccessful);
            Map(x => x.ClientIP);
        }
    }
}
