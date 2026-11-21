using System.Diagnostics;

namespace EmployeeManagement.API.Middleware;

/// <summary>
/// Custom Middleware demonstrating the ASP.NET Core Middleware Pipeline.
/// 1. Attaches or propagates a unique correlation ID (X-Correlation-Id)
/// 2. Records execution time and adds X-Response-Time-Ms header
/// 3. Logs incoming request metadata and outgoing status codes using structured logging
/// </summary>
public class RequestLoggingMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<RequestLoggingMiddleware> _logger;
    public const string CorrelationIdHeader = "X-Correlation-Id";
    public const string ResponseTimeHeader = "X-Response-Time-Ms";

    public RequestLoggingMiddleware(RequestDelegate next, ILogger<RequestLoggingMiddleware> logger)
    {
        _next = next;
        _logger = logger;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        var stopwatch = Stopwatch.StartNew();

        // 1. Correlation ID management
        if (!context.Request.Headers.TryGetValue(CorrelationIdHeader, out var correlationId) || string.IsNullOrWhiteSpace(correlationId))
        {
            correlationId = Guid.NewGuid().ToString("N");
        }

        context.Items[CorrelationIdHeader] = correlationId.ToString();

        // Ensure headers are added before response starts streaming
        context.Response.OnStarting(() =>
        {
            stopwatch.Stop();
            context.Response.Headers[CorrelationIdHeader] = correlationId;
            context.Response.Headers[ResponseTimeHeader] = stopwatch.ElapsedMilliseconds.ToString();
            return Task.CompletedTask;
        });

        // 2. Structured Request Logging
        _logger.LogInformation(
            "[{CorrelationId}] Incoming HTTP {Method} {Path}{Query} from {ClientIp}",
            correlationId,
            context.Request.Method,
            context.Request.Path,
            context.Request.QueryString,
            context.Connection.RemoteIpAddress?.ToString() ?? "localhost"
        );

        try
        {
            // Call next middleware in the pipeline
            await _next(context);
        }
        catch (Exception ex)
        {
            stopwatch.Stop();
            _logger.LogError(
                ex,
                "[{CorrelationId}] Unhandled exception during HTTP {Method} {Path} after {ElapsedMs}ms",
                correlationId,
                context.Request.Method,
                context.Request.Path,
                stopwatch.ElapsedMilliseconds
            );
            throw; // Re-throw for downstream exception handlers
        }

        // 3. Structured Response Logging
        _logger.LogInformation(
            "[{CorrelationId}] Completed HTTP {Method} {Path} with Status {StatusCode} in {ElapsedMs}ms",
            correlationId,
            context.Request.Method,
            context.Request.Path,
            context.Response.StatusCode,
            stopwatch.ElapsedMilliseconds
        );
    }
}

public static class RequestLoggingMiddlewareExtensions
{
    public static IApplicationBuilder UseRequestLogging(this IApplicationBuilder app)
    {
        return app.UseMiddleware<RequestLoggingMiddleware>();
    }
}
