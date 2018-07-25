using System;
using System.Collections.Generic;
using System.Text;

namespace Armoire.Common
{
    public interface IUserService
    {
        PatronDto Register(PatronDto p);
    }
}
