using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System;
using System.Linq;

namespace BGServiceAX.Attributes
{
    public class FilterSupportTagOnlyAttribute : Attribute, IResourceFilter
    {
        public string[] ListTag { get; set; }
        public FilterSupportTagOnlyAttribute(params string[] args)
        {
            ListTag = args;
        }
        public void OnResourceExecuting(ResourceExecutingContext context)
        {
            var configuration = context.HttpContext.RequestServices.GetService<IConfiguration>();
            var lstSupportTag = configuration.GetSection("SupportedTag").Get<string[]>();
            var intersect = lstSupportTag.Intersect(ListTag);
            if (!(intersect != null && intersect.Count() > 0))
            {
                context.Result = new NotFoundResult();
            }
            
        }

        public void OnResourceExecuted(ResourceExecutedContext context)
        {
        }
    }
}
