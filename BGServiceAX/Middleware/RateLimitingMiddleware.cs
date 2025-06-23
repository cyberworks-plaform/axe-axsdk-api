using Microsoft.Extensions.Options;
using Microsoft.Extensions.FileSystemGlobbing;
using AXAPIWrapper.Middleware;
using Microsoft.AspNetCore.Http;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Threading;
using System;
using System.Linq;
using System.Text.RegularExpressions;
using Serilog;
using System.Text.Json;
using System.Text;
using MongoDB.Driver.Core.Servers;
using AutoMapper.Configuration;
using Microsoft.Extensions.Configuration;
using IConfiguration = Microsoft.Extensions.Configuration.IConfiguration;


public class RateLimitingMiddleware
{
    private readonly RequestDelegate _next;
    private static int _activeRequests = 0;
    private readonly IOptionsMonitor<RateLimitOptions> _options;
    private readonly IConfiguration _configuration;
    public RateLimitingMiddleware(RequestDelegate next, IOptionsMonitor<RateLimitOptions> options,IConfiguration configuration)
    {
        _next = next;
        _options = options;
        _configuration = configuration;
    }

    public async Task Invoke(HttpContext context)
    {
        var path = context.Request.Path.ToString().ToLower();
        var config = _options.CurrentValue;

        // process any request if no limit
        if (config.MaxConcurrentRequests <= 0)
        {
            await _next(context);
            return;
        }

        // process request if it matchs ExcludePaths pattern
        if (!IsPathMatched(path, config.IncludePaths, config.ExcludePaths))
        {
            await _next(context);
            return;
        }

        // increate number of active request
        int current = Interlocked.Increment(ref _activeRequests);

        // return response with not OK code because it execed MaxConcurrentRequests 
        if (current > config.MaxConcurrentRequests)
        {
            Interlocked.Decrement(ref _activeRequests);

            context.Response.StatusCode = StatusCodes.Status423Locked;
            context.Response.ContentType = "application/json";

            var clientIp = context.Connection.RemoteIpAddress?.ToString() ?? "Unknown";
            var serverIp = context.Connection.LocalIpAddress?.ToString() ?? "Unknown";
            var serviceName = $"{_configuration["ServiceCode"] ?? "Unknown"}@{Environment.MachineName}";

            var info = new RateLimitServerInfo
            {
                Timestamp = DateTime.UtcNow,
                RequestPath = context.Request.Path,
                Method = context.Request.Method,
                ClientIp = clientIp,
                ServerIp = serverIp,
                ServiceName = serviceName,
                ActiveRequests = _activeRequests,
                RateLimit = config.MaxConcurrentRequests
            };

            var result = new RateLimitErrorResponse
            {
                State = StatusCodes.Status423Locked,
                Message = "Server is busy",
                Description = info
            };

            context.Response.StatusCode = StatusCodes.Status423Locked;
            context.Response.ContentType = "application/json";

            var json = JsonSerializer.Serialize(result, new JsonSerializerOptions
            {
                WriteIndented = true
            });

            await context.Response.WriteAsync(json);
            return;
        }

        // process request if it not exced MaxConcurrentRequests
        try
        {
            await _next(context);
        }
        finally
        {
            Interlocked.Decrement(ref _activeRequests);
        }
    }
    private bool IsPathMatched(string path, List<string> includePatterns, List<string> excludePatterns)
    {
        path = NormalizePath(path.ToLowerInvariant());

        // Nếu khớp exclude → bỏ qua
        foreach (var pattern in excludePatterns)
        {
            if (PathMatches(path, NormalizePattern(pattern.ToLowerInvariant())))
                return false;
        }

        // Nếu khớp include → áp dụng rate limit
        foreach (var pattern in includePatterns)
        {
            if (PathMatches(path, NormalizePattern(pattern.ToLowerInvariant())))
                return true;
        }

        // Không khớp gì → bỏ qua
        return false;
    }

    private bool PathMatches(string path, string pattern)
    {
        if (pattern.EndsWith("/*"))
        {
            var prefix = pattern.Substring(0, pattern.Length - 1); // giữ dấu /
            return path.StartsWith(prefix);
        }
        else
        {
            return path.Equals(pattern, StringComparison.OrdinalIgnoreCase);
        }
    }

    private string NormalizePath(string path)
    {
        return path.StartsWith("/") ? path : "/" + path;
    }

    private string NormalizePattern(string pattern)
    {
        return pattern.StartsWith("/") ? pattern : "/" + pattern;
    }


}
