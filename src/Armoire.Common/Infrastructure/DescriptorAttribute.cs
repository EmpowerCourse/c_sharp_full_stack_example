using System;
using System.Collections.Generic;
using System.Text;

namespace Armoire.Common
{
    [AttributeUsage(AttributeTargets.All, Inherited = false, AllowMultiple = true)]
    public class DescriptorAttribute : Attribute
    {
        public DescriptorAttribute(string descriptorCategory, string descriptor)
        {
            DescriptorCategory = descriptorCategory;
            Descriptor = descriptor;
        }
        public string DescriptorCategory { get; set; }
        public string Descriptor { get; set; }
    }
}
