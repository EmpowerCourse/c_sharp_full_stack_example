using Armoire.Common;
using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Armoire.Infrastructure
{
    public static class HtmlHelperExtensions
    {
        public static AjaxPager AjaxPager(this IHtmlHelper helper, IPagination pagination)
        {
            return new AjaxPager(pagination, helper.ViewContext);
        }
    }
}
