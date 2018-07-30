using System;
using System.Collections.Generic;
using System.Text;

namespace Armoire.Common
{
    public class UserListVM
    {
        public IList<SimpleDto> RoleList { get; set; }
        public IPagination<UserDto> UserList { get; set; }
    }
}
