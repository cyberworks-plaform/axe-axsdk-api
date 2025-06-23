using System;

namespace AXAPIWrapper.Middleware
{
    public class RateLimitServerInfo
    {
        public DateTime Timestamp { get; set; }
        public string RequestPath { get; set; } = "";
        public string Method { get; set; } = "";
        public string ClientIp { get; set; } = "";
        public string ServerIp { get; set; } = "";
        public string ServiceName { get; set; } = "";
        public int ActiveRequests { get; set; }
        public int RateLimit { get; set; }
    }

}
