using Armoire.Common;
using System;
using System.Collections.Generic;
using System.Text;
using FluentNHibernate.Mapping;

namespace Armoire.Entities.Mappings
{
    public class UserRoleMap: ClassMap<UserRole>
    {
        public UserRoleMap()
        {
            Table("UserRole");
            Id(x => x.Id);
            Map(x => x.UserId);
            Map(x => x.TypeOfUserRole).Column("RoleId").CustomType<TypeOfUserRole>();
        }
    }
}
