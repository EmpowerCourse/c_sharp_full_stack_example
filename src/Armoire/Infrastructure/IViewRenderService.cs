using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Armoire.Infrastructure
{
    public interface IViewRenderService
    {
        string RenderToString(string viewName, object model);
    }
}
