using Armoire.Common;
using Microsoft.AspNetCore.Html;
using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.Encodings.Web;
using System.Threading.Tasks;

namespace Armoire.Infrastructure
{
    public class AjaxPager : IHtmlContent
    {
        private readonly IPagination _pagination;
        private readonly ViewContext _viewContext;

        private string _paginationFormat = "Showing {0} - {1} of {2} ";
        private string _paginationSingleFormat = "Showing {0} of {1} ";
        private string _paginationFirst = "first";
        private string _paginationPrev = "prev";
        private string _paginationNext = "next";
        private string _paginationLast = "last";
        private string _pageQueryName = "page";
        private Func<int, string> _urlBuilder;

        /// <summary>
        /// Creates a new instance of the Pager class.
        /// </summary>
        /// <param name="pagination">The IPagination datasource</param>
        /// <param name="context">The view context</param>
        public AjaxPager(IPagination pagination, ViewContext context)
        {
            _pagination = pagination;
            _viewContext = context;

            //_urlBuilder = CreateDefaultUrl;
        }

        protected ViewContext ViewContext
        {
            get { return _viewContext; }
        }


        /// <summary>
        /// Specifies the query string parameter to use when generating pager links. The default is 'page'
        /// </summary>
        public AjaxPager QueryParam(string queryStringParam)
        {
            _pageQueryName = queryStringParam;
            return this;
        }
        /// <summary>
        /// Specifies the format to use when rendering a pagination containing a single page. 
        /// The default is 'Showing {0} of {1}' (eg 'Showing 1 of 3')
        /// </summary>
        public AjaxPager SingleFormat(string format)
        {
            _paginationSingleFormat = format;
            return this;
        }

        /// <summary>
        /// Specifies the format to use when rendering a pagination containing multiple pages. 
        /// The default is 'Showing {0} - {1} of {2}' (eg 'Showing 1 to 3 of 6')
        /// </summary>
        public AjaxPager Format(string format)
        {
            _paginationFormat = format;
            return this;
        }

        /// <summary>
        /// Text for the 'first' link.
        /// </summary>
        public AjaxPager First(string first)
        {
            _paginationFirst = first;
            return this;
        }

        /// <summary>
        /// Text for the 'prev' link
        /// </summary>
        public AjaxPager Previous(string previous)
        {
            _paginationPrev = previous;
            return this;
        }

        /// <summary>
        /// Text for the 'next' link
        /// </summary>
        public AjaxPager Next(string next)
        {
            _paginationNext = next;
            return this;
        }

        /// <summary>
        /// Text for the 'last' link
        /// </summary>
        public AjaxPager Last(string last)
        {
            _paginationLast = last;
            return this;
        }

        /// <summary>
        /// Uses a lambda expression to generate the URL for the page links.
        /// </summary>
        /// <param name="urlBuilder">Lambda expression for generating the URL used in the page links</param>
        public AjaxPager Link(Func<int, string> urlBuilder)
        {
            _urlBuilder = urlBuilder;
            return this;
        }

        // For backwards compatibility with WebFormViewEngine
        public override string ToString()
        {
            return ToHtmlString();
        }

        public string ToHtmlString()
        {
            if (_pagination.TotalItems == 0) return String.Empty;
            var builder = new StringBuilder();
            builder.Append("<div class='pagination' data-currentPage='" + _pagination.PageNumber + "'>");
            RenderLeftSideOfPager(builder);
            if (_pagination.TotalPages > 1)
            {
                RenderRightSideOfPager(builder);
            }
            builder.Append(@"</div>");
            return builder.ToString();
        }

        protected virtual void RenderLeftSideOfPager(StringBuilder builder)
        {
            builder.Append("<span class='paginationLeft'>");

            //Special case handling where the page only contains 1 item (ie it's a details-view rather than a grid)
            if (_pagination.PageSize == 1)
            {
                RenderNumberOfItemsWhenThereIsOnlyOneItemPerPage(builder);
            }
            else
            {
                RenderNumberOfItemsWhenThereAreMultipleItemsPerPage(builder);
            }
            builder.Append("</span>");
        }

        protected virtual void RenderRightSideOfPager(StringBuilder builder)
        {
            builder.Append("<span class='paginationRight'>");

            //If we're on page 1 then there's no need to render a link to the first page. 
            if (_pagination.PageNumber == 1)
            {
                builder.Append(_paginationFirst);
            }
            else
            {
                builder.Append(createPageLink(1, _paginationFirst));
            }

            builder.Append(" | ");

            //If we're on page 2 or later, then render a link to the previous page. 
            //If we're on the first page, then there is no need to render a link to the previous page. 
            if (_pagination.HasPreviousPage)
            {
                builder.Append(createPageLink(_pagination.PageNumber - 1, _paginationPrev));
            }
            else
            {
                builder.Append(_paginationPrev);
            }

            builder.Append(" | ");

            //Only render a link to the next page if there is another page after the current page.
            if (_pagination.HasNextPage)
            {
                builder.Append(createPageLink(_pagination.PageNumber + 1, _paginationNext));
            }
            else
            {
                builder.Append(_paginationNext);
            }

            builder.Append(" | ");

            int lastPage = _pagination.TotalPages;

            //Only render a link to the last page if we're not on the last page already.
            if (_pagination.PageNumber < lastPage)
            {
                builder.Append(createPageLink(lastPage, _paginationLast));
            }
            else
            {
                builder.Append(_paginationLast);
            }

            builder.Append("</span>");
        }


        protected virtual void RenderNumberOfItemsWhenThereIsOnlyOneItemPerPage(StringBuilder builder)
        {
            builder.AppendFormat(_paginationSingleFormat, _pagination.FirstItem, _pagination.TotalItems);
        }

        protected virtual void RenderNumberOfItemsWhenThereAreMultipleItemsPerPage(StringBuilder builder)
        {
            builder.AppendFormat(_paginationFormat, _pagination.FirstItem, _pagination.LastItem, _pagination.TotalItems);
        }

        private static string createPageLink(int pageNumber, string text)
        {
            //var builder = new TagBuilder("a");
            //builder.SetInnerText(text);
            //builder.MergeAttribute("href", "#");
            //builder.MergeAttribute("class", "pager-link");
            //builder.MergeAttribute("data-page", pageNumber.ToString());
            //return builder.ToString(TagRenderMode.Normal);
            return String.Format("<a href='#' class='pager-link' data-page='{0}'>{1}</a>", pageNumber, text);
        }

        public void WriteTo(TextWriter writer, HtmlEncoder encoder)
        {
            writer.Write(ToHtmlString());
        }
    }
}
