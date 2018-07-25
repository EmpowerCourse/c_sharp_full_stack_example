using Armoire.Common;
using AutoMapper;
using System;
using System.Collections.Generic;
using System.Text;

namespace Armoire.Automapper
{
    public class AutomapperMapping: IAutomapperMapping
    {
        public AutomapperMapping(IEnumerable<IAutoMapperMappingCollection> configurations)
        {
            Mapper.Initialize(cfg =>
            {
                foreach (var autoMapperConfiguration in configurations)
                {
                    cfg = autoMapperConfiguration.AddMappingsToAutoMapper(cfg);
                }
            });
        }

        public TKTo Map<TFrom, TKTo>(TFrom from)
        {
            return Mapper.Map<TFrom, TKTo>(from);
        }

        public TKTo Map<TFrom, TKTo>(TFrom from, TKTo to)
        {
            return Mapper.Map<TFrom, TKTo>(from, to);
        }
    }
}
