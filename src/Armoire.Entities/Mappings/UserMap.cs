using System;
using System.Collections.Generic;
using System.Text;
using FluentNHibernate.Mapping;

namespace Armoire.Entities.Mappings
{
    public class UserMap : ClassMap<User>
    {
        public UserMap()
        {
            Table("Users");
            Id(x => x.Id);
            Map(x => x.FirstName);
            Map(x => x.LastName);
            Map(x => x.Username);
            Map(x => x.Email);
            Map(x => x.Phone).Nullable();
            Map(x => x.PasswordHash);
            Map(x => x.Salt);
            Map(x => x.DeactivatedAt).Nullable();
            Map(x => x.CreatedAt);
            Map(x => x.LastUpdatedAt);
            HasMany(x => x.RoleList).KeyColumn("UserId").Cascade.None().Inverse();
        }
    }
}
