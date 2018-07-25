using System;
using System.Collections.Generic;
using System.Text;
using AutoMapper;

namespace Armoire.Common
{
    public interface IAutoMapperMappingCollection
    {
        IMapperConfigurationExpression AddMappingsToAutoMapper(IMapperConfigurationExpression cfg);
    }
}
