namespace TrailShop.Api.Middleware;

/// <summary>
/// Writes one log line per request to help with support tickets.
/// </summary>
public class RequestLoggingMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<RequestLoggingMiddleware> _logger;

    public RequestLoggingMiddleware(RequestDelegate next, ILogger<RequestLoggingMiddleware> logger)
    {
        _next = next;
        _logger = logger;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        var headers = string.Join("; ", context.Request.Headers.Select(h => $"{h.Key}={h.Value}"));

        _logger.LogInformation(
            "HTTP {Method} {Path}{Query} headers: {Headers}",
            context.Request.Method,
            context.Request.Path,
            context.Request.QueryString,
            headers);

        await _next(context);
    }
}
