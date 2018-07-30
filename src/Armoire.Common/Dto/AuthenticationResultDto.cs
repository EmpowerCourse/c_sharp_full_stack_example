using System;
using System.Collections.Generic;
using System.Text;

namespace Armoire.Common
{
    public class AuthenticationResultDto
    {
        public string ErrorMessage { get; set; }
        public bool Success
        {
            get
            {
                return String.IsNullOrEmpty(ErrorMessage);
            }
        }
        public UserDto User { get; set; }
    }
}
