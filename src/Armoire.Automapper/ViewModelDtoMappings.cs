using AutoMapper;
using System;
using System.Collections.Generic;
using System.Text;
using Armoire.Common;
using Armoire.Entities;

namespace Armoire.Automapper
{
    public class ViewModelDtoMappings : IAutoMapperMappingCollection
    {
        public IMapperConfigurationExpression AddMappingsToAutoMapper(IMapperConfigurationExpression cfg)
        {
            cfg.CreateMap<RegisterVM, UserDto>();
            //cfg.CreateMap<UserDto, UserVM>();
            return cfg;
        }
    }
}
