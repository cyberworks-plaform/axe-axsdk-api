using Microsoft.AspNetCore.Http;

namespace AXAPIWrapper.Middleware
{
public class RateLimitErrorResponse
{
    public int State { get; set; } = StatusCodes.Status423Locked;
    public string Message { get; set; } = "Server is busy";
    public RateLimitServerInfo Description { get; set; } = new RateLimitServerInfo();
}

}
