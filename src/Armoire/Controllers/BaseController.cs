using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Armoire.Common;
using Armoire.Infrastructure;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.ViewEngines;
using System.IO;
using Microsoft.AspNetCore.Mvc.ViewFeatures;

namespace Armoire.Controllers
{
    public class BaseController : Controller
    {
        protected ICompositeViewEngine _viewEngine;

        public BaseController(ICompositeViewEngine viewEngine)
        {
            _viewEngine = viewEngine;
        }

        protected string RenderViewAsString(object model, string viewName = null)
        {
            viewName = viewName ?? ControllerContext.ActionDescriptor.ActionName;
            ViewData.Model = model;
            using (StringWriter sw = new StringWriter())
            {
                IView view = _viewEngine.FindView(ControllerContext, viewName, true).View;
                ViewContext viewContext = new ViewContext(ControllerContext, view, ViewData, TempData, sw, new HtmlHelperOptions());
                view.RenderAsync(viewContext).Wait();
                return sw.GetStringBuilder().ToString();
            }
        }
    }
}
