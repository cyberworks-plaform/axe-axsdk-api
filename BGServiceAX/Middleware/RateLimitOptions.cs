using System.Collections.Generic;

namespace AXAPIWrapper.Middleware
{
    public class RateLimitOptions
    {
        public int MaxConcurrentRequests { get; set; } = 0;
        public List<string> IncludePaths { get; set; } = new List<string>();
        public List<string> ExcludePaths { get; set; } = new List<string>();
    }

}
