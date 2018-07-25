using Armoire.Common;
using Ninject;
using Ninject.Modules;
using System;
using System.Collections.Generic;
using System.Text;

namespace Armoire.Automapper
{
    public class AutoMapperModule : NinjectModule
    {
        public override void Load()
        {
            //ninject will fill in any configured mappings
            var mapper = Kernel.Get<AutomapperMapping>();
            Bind<IAutomapperMapping>().ToConstant(mapper).InSingletonScope();
        }
    }
}
