using System;
using System.Collections.Generic;
using System.Text;

namespace Armoire.Common
{
    public interface ISimpleDto
    {
        int Id { get; set; }
        string Name { get; set; }
    }
}
