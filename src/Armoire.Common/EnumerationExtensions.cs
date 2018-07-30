using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;

namespace Armoire.Common
{
    public static class EnumerationExtensions
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
        public static IList<SimpleDto> ToSimpleDtoList(this System.Enum value,
                int[] excludeEntries = null)
        {
            var values = value.GetType().GetEnumValues();
            return (from System.Enum x in values
                    where (excludeEntries == null || !excludeEntries.Contains((int)(object)x))
                    select x.ToSimpleDto()).ToList();
        }
        public static SimpleDto ToSimpleDto(this System.Enum value)
        {
            var attribute = value.GetType().GetField(value.ToString()).GetCustomAttributes(typeof(DescriptionAttribute), false).SingleOrDefault() as DescriptionAttribute;
            var description = attribute == null ? value.ToString() : attribute.Description;
            return new SimpleDto()
            {
                Id = Convert.ToInt32(value),
                Name = description
            };
        }

        public static string Description(this System.Enum value)
        {
            var attribute = value.GetType().GetField(value.ToString()).GetCustomAttributes(typeof(DescriptionAttribute), false).SingleOrDefault() as DescriptionAttribute;
            return attribute == null ? value.ToString() : attribute.Description;
        }
    }
}
