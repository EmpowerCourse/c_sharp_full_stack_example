using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;

namespace Armoire.Common
{
    public static class EnumerableExtensions
    {
        public static IPagination<T> AsPagination<T>(this IEnumerable<T> source, int pageNumber, int pageSize)
        {
            if (pageNumber < 1) throw new ArgumentOutOfRangeException("pageNumber", "The page number should be greater than or equal to 1.");
            return new LazyPagination<T>(source.AsQueryable(), pageNumber, pageSize);
        }
        public static IList<T> InsertItemAtFrontOfList<T>(this IList<T> list, T item)
        {
            list.Insert(0, item);
            return list;
        }
    }
}
