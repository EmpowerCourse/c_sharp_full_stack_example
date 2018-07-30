using AutoMapper;
using System;
using System.Collections.Generic;
using System.Text;
using Armoire.Common;
using Armoire.Entities;
using System.Linq;

namespace Armoire.Automapper
{
    public class ModelDtoMappings : IAutoMapperMappingCollection
    {
        public IMapperConfigurationExpression AddMappingsToAutoMapper(IMapperConfigurationExpression cfg)
        {
            cfg.CreateMap<User, UserDto>()
                .ForMember(dest => dest.IsActive, opt => opt.MapFrom(src => src.DeactivatedAt == null))
                .ForMember(dest => dest.Roles, opt => opt.MapFrom(src => src.RoleList == null || src.RoleList.Count == 0
                    ? new List<TypeOfUserRole>()
                    : src.RoleList.ToList()
                        .Select(s => s.TypeOfUserRole)
                        .ToList()));
            return cfg;
        }
    }
}
