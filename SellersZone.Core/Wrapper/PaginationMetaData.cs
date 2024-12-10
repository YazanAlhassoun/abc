using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Extensions;
using Microsoft.AspNetCore.Http.Internal;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Microsoft.Extensions.Primitives;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SellersZone.Core.Wrapper
{
    public class PaginationMetaData
    {
        public PaginationMetaData(int page = 1, int totalCount = 0, int itemPerPage = 30, HttpRequest? requestUrl = null)
        {
            Page = page;
            TotalCount = totalCount;
            TotalPages = (int)Math.Ceiling(totalCount / (double)itemPerPage);
            ItemPerPage = itemPerPage;
            if (requestUrl != null)
            {
                SetPageUrls(requestUrl);
            }
        }

        public int Page { get; set; }
        public int TotalCount { get; set; }
        public int TotalPages { get; set; }

        public int ItemPerPage { get; set; }
        public bool HasPrevious => Page > 1;
        public bool HasNext => Page < TotalPages;

        public string? FirstPageUrl { get; set; }
        public string? LastPageUrl { get; set; }
        public string? NextPageUrl { get; set; }
        public string? PrevPageUrl { get; set; }

        private void SetPageUrls(HttpRequest request)
        {
            var queryParameters = new Dictionary<string, StringValues>(request.Query);

            if (queryParameters.TryGetValue("Page", out StringValues pageValue))
            {
               
                FirstPageUrl = CalculatePageUrl(1);
                PrevPageUrl = HasPrevious ? CalculatePageUrl(int.Parse(pageValue) - 1) : null;
                NextPageUrl = HasNext ? CalculatePageUrl(int.Parse(pageValue) + 1) : null;
                LastPageUrl = (Page != TotalPages) ? CalculatePageUrl(TotalPages) : null;

                string CalculatePageUrl(int page)
                {
                    queryParameters["Page"] = page.ToString();
                    var uriBuilder = new UriBuilder(request.GetDisplayUrl())
                    {
                        Query = QueryString.Create(queryParameters).Value
                    };
                    return uriBuilder.Uri.ToString();
                }
            }
        }

    }
}
