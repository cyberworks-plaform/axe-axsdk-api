using AXAPIWrapper.Middleware;
using Microsoft.AspNetCore.Http;
using Serilog;
using System.Text.Json;
using System.Threading.Tasks;
using System;
using Microsoft.Extensions.Configuration;

public class AxDesExceptionMiddleware
{
    private readonly RequestDelegate _next;
    private readonly IConfiguration _configuration;

    public AxDesExceptionMiddleware(RequestDelegate next, IConfiguration configuration)
    {
        _next = next;
        _configuration = configuration;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await _next(context);
        }
        catch (AxDesNullValueException ex)
        {
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
                ActiveRequests = 0,
                RateLimit = 0
            };

            var result = new RateLimitErrorResponse
            {
                State = StatusCodes.Status423Locked,
                Message = ex.Message,
                Description = info
            };

            context.Response.StatusCode = StatusCodes.Status423Locked;
            context.Response.ContentType = "application/json";

            var json = JsonSerializer.Serialize(result, new JsonSerializerOptions { WriteIndented = true });
            Log.Error("AxDesNullValueException: {Json}", json);
            await context.Response.WriteAsync(json);
        }
    }
}
