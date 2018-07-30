using System;
using System.Collections.Generic;
using System.Text;

namespace Armoire.Common
{
    public class SimpleDto : ISimpleDto
    {
        public int Id { get; set; }
        public string Name { get; set; }
    }
}
