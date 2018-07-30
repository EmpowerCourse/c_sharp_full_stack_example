using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;

namespace Armoire.Common
{
    public interface IPaginatedList<T> : IPagination, IEnumerable<T>, IEnumerable
    {
    }
}
