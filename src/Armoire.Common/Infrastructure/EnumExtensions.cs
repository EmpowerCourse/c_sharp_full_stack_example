using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;

namespace Armoire.Common
{
    public static class EnumExtensions
    {
        public static IList<SimpleDto> ToSimpleDtoList(this Enum value,
                int[] excludeEntries = null)
        {
            var values = value.GetType().GetEnumValues();
            return (from System.Enum x in values
                    where (excludeEntries == null || !excludeEntries.Contains((int)(object)x))
                    select x.ToSimpleDto()).ToList();
        }
        public static SimpleDto ToSimpleDto(this Enum value)
        {
            var attribute = value.GetType().GetField(value.ToString()).GetCustomAttributes(typeof(DescriptionAttribute), false).SingleOrDefault() as DescriptionAttribute;
            var description = attribute == null ? value.ToString() : attribute.Description;
            return new SimpleDto()
            {
                Id = Convert.ToInt32(value),
                Name = description
            };
        }

        public static string Description(this Enum value)
        {
            var attribute = value.GetType().GetField(value.ToString()).GetCustomAttributes(typeof(DescriptionAttribute), false).SingleOrDefault() as DescriptionAttribute;
            return attribute == null ? value.ToString() : attribute.Description;
        }

        public static string Descriptor<T>(this T obj, string descriptorCategory)
        {
            Type to = typeof(T);
            return Descriptor(obj, to, descriptorCategory);
        }

        public static string Descriptor(this Object obj, Type to, string descriptorCategory)
        {
            IList<DescriptorAttribute> attributes = null;
            if (to.BaseType == typeof(Enum))
            {
                attributes = to.GetMember(obj.ToString())
                    .First()
                    .GetCustomAttributes(typeof(DescriptorAttribute), false)
                    .Select(x => (DescriptorAttribute)x)
                    .ToList();
            }
            if (attributes == null || !attributes.Any())
            {
                attributes = to.GetCustomAttributes(typeof(DescriptorAttribute), false)
                    .Select(x => (DescriptorAttribute)x)
                    .ToList();
            }
            return attributes.Where(x => x.DescriptorCategory == descriptorCategory)
                    .Select(x => x.Descriptor)
                    .FirstOrDefault();
        }
    }
}
