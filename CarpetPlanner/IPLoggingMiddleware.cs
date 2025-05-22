namespace CarpetPlanner;

using Microsoft.AspNetCore.Http;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;

public class IPLoggingMiddleware(RequestDelegate next, ILogger<IPLoggingMiddleware> logger)
{
    private readonly RequestDelegate _next = next;
    private readonly ILogger<IPLoggingMiddleware> _logger = logger;

    public async Task InvokeAsync(HttpContext context)
    {
        var ipAddress = context.Connection.RemoteIpAddress?.ToString();
        _logger.LogInformation("Incoming request from IP: {IPAddress}", ipAddress);

        // Call the next delegate/middleware in the pipeline
        await _next(context);
    }
}