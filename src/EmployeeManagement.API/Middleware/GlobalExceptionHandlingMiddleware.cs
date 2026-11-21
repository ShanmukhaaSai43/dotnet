using System.Net;
using System.Text.Json;
using EmployeeManagement.Application.Common.Exceptions;
using Microsoft.AspNetCore.Mvc;

namespace EmployeeManagement.API.Middleware;

/// <summary>
/// Centralized Global Exception Handling Middleware.
/// Intercepts unhandled exceptions across the pipeline and formats them
/// as standardized RFC 7807 ProblemDetails / ValidationProblemDetails responses.
/// </summary>
public class GlobalExceptionHandlingMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<GlobalExceptionHandlingMiddleware> _logger;
    private readonly IHostEnvironment _env;

    private static readonly JsonSerializerOptions _jsonOptions = new()
    {
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
        WriteIndented = true
    };

    public GlobalExceptionHandlingMiddleware(
        RequestDelegate next,
        ILogger<GlobalExceptionHandlingMiddleware> logger,
        IHostEnvironment env)
    {
        _next = next;
        _logger = logger;
        _env = env;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await _next(context);
        }
        catch (Exception ex)
        {
            await HandleExceptionAsync(context, ex);
        }
    }

    private async Task HandleExceptionAsync(HttpContext context, Exception exception)
    {
        var correlationId = context.Items[RequestLoggingMiddleware.CorrelationIdHeader]?.ToString()
            ?? Guid.NewGuid().ToString("N");

        int statusCode;
        string title;
        string detail;
        IDictionary<string, string[]>? errors = null;

        switch (exception)
        {
            case ValidationException valEx:
                statusCode = StatusCodes.Status400BadRequest;
                title = valEx.Title;
                detail = valEx.Message;
                errors = valEx.Errors;
                _logger.LogWarning(
                    "[{CorrelationId}] Validation failure on {Path}: {Message}. Errors: {ErrorCount}",
                    correlationId, context.Request.Path, valEx.Message, errors.Count);
                break;

            case NotFoundException notFoundEx:
                statusCode = StatusCodes.Status404NotFound;
                title = notFoundEx.Title;
                detail = notFoundEx.Message;
                _logger.LogWarning(
                    "[{CorrelationId}] Resource not found on {Path}: {Message}",
                    correlationId, context.Request.Path, notFoundEx.Message);
                break;

            case ConflictException conflictEx:
                statusCode = StatusCodes.Status409Conflict;
                title = conflictEx.Title;
                detail = conflictEx.Message;
                _logger.LogWarning(
                    "[{CorrelationId}] Conflict encountered on {Path}: {Message}",
                    correlationId, context.Request.Path, conflictEx.Message);
                break;

            case BusinessRuleException ruleEx:
                statusCode = StatusCodes.Status422UnprocessableEntity;
                title = ruleEx.Title;
                detail = ruleEx.Message;
                _logger.LogWarning(
                    "[{CorrelationId}] Business rule violation on {Path}: {Message}",
                    correlationId, context.Request.Path, ruleEx.Message);
                break;

            default:
                statusCode = StatusCodes.Status500InternalServerError;
                title = "An unexpected server error occurred";
                detail = _env.IsDevelopment()
                    ? $"{exception.Message} ({exception.GetType().Name})"
                    : "An error occurred while processing your request. Please contact support with the correlation ID.";

                _logger.LogError(
                    exception,
                    "[{CorrelationId}] Unhandled server exception on {Path}: {Message}",
                    correlationId, context.Request.Path, exception.Message);
                break;
        }

        context.Response.Clear();
        context.Response.StatusCode = statusCode;
        context.Response.ContentType = "application/problem+json";

        object problemResponse;

        if (errors != null && errors.Count > 0)
        {
            var validationProblem = new ValidationProblemDetails(errors)
            {
                Status = statusCode,
                Title = title,
                Detail = detail,
                Instance = context.Request.Path
            };
            validationProblem.Extensions["correlationId"] = correlationId;
            validationProblem.Extensions["timestamp"] = DateTime.UtcNow;
            problemResponse = validationProblem;
        }
        else
        {
            var problem = new ProblemDetails
            {
                Status = statusCode,
                Title = title,
                Detail = detail,
                Instance = context.Request.Path
            };
            problem.Extensions["correlationId"] = correlationId;
            problem.Extensions["timestamp"] = DateTime.UtcNow;

            if (_env.IsDevelopment() && statusCode == StatusCodes.Status500InternalServerError)
            {
                problem.Extensions["stackTrace"] = exception.StackTrace;
            }

            problemResponse = problem;
        }

        var json = JsonSerializer.Serialize(problemResponse, problemResponse.GetType(), _jsonOptions);
        await context.Response.WriteAsync(json);
    }
}

public static class GlobalExceptionHandlingMiddlewareExtensions
{
    public static IApplicationBuilder UseGlobalExceptionHandler(this IApplicationBuilder app)
    {
        return app.UseMiddleware<GlobalExceptionHandlingMiddleware>();
    }
}
