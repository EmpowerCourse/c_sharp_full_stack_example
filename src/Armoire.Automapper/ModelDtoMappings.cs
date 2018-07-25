using AutoMapper;
using System;
using System.Collections.Generic;
using System.Text;
using Armoire.Common;
using Armoire.Entities;

namespace Armoire.Automapper
{
    public class ModelDtoMappings : IAutoMapperMappingCollection
    {
        public IMapperConfigurationExpression AddMappingsToAutoMapper(IMapperConfigurationExpression cfg)
        {
            cfg.CreateMap<Patron, PatronDto>();
            return cfg;
            //Mapper.CreateMap<ManufacturerDealerLot, SimpleDto>()
            //    .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Dealer.Id))
            //    .ForMember(dest => dest.Name, opt => opt.MapFrom(src => src.Dealer.Name))
            //    ;
        }
    }
}
