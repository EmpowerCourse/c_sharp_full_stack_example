using System;
using System.Collections.Generic;
using System.Text;

namespace Armoire.Common
{
    public class PaginatedList<T>
    {
        public int PageNo { get; set; }
        public IList<T> Items { get; set; }
        public int TotalCount { get; set; }
    }
}
