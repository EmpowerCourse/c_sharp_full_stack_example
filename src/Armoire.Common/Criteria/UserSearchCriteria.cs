using System;
using System.Collections.Generic;
using System.Text;

namespace Armoire.Common
{
    public class UserSearchCriteria
    {
        public int? Id { get; set; }
        public string Name { get; set; }
        public string Username { get; set; }
        public string Email { get; set; }
        public bool ActiveOnly { get; set; }
        public IEnumerable<int> IdList { get; set; }
        public TypeOfUserRole? MemberOfRole { get; set; }
    }
}
