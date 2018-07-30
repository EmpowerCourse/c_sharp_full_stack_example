using System;
using System.Collections.Generic;
using System.Text;

namespace Armoire.Common
{
    public class UserDtoGridColumnSorterMapper : GridColumnSorterMapper<UserDtoGridColumnSorterMapper>
    {
        protected override void Initialize()
        {
            ColumnsMap = new Dictionary<string, string> {
                {"FirstName", "FirstName"},
                {"LastName","LastName"},
                {"Username", "Username"},
                {"Roles", "RoleInformation"},
                {"Active", "IsActive"}
            };
        }
    }
}
