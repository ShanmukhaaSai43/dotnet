using System.Text.Json;
using EmployeeManagement.API.Middleware;
using EmployeeManagement.Application.Common.Exceptions;
using FluentValidation.Results;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging.Abstractions;

namespace EmployeeManagement.UnitTests.ApplicationTests;

public class GlobalExceptionHandlingMiddlewareTests
{
    private readonly NullLogger<GlobalExceptionHandlingMiddleware> _logger = new();
    private readonly FakeHostEnvironment _env = new() { EnvironmentName = Environments.Development };

    [Fact]
    public async Task InvokeAsync_WhenValidationException_Returns400WithValidationProblemDetails()
    {
        // Arrange
        var context = CreateHttpContext();
        var failures = new List<ValidationFailure>
        {
            new("Email", "Invalid email format"),
            new("Name", "Name is required")
        };
        RequestDelegate next = (ctx) => throw new ValidationException(failures);

        var middleware = new GlobalExceptionHandlingMiddleware(next, _logger, _env);

        // Act
        await middleware.InvokeAsync(context);

        // Assert
        Assert.Equal(StatusCodes.Status400BadRequest, context.Response.StatusCode);
        Assert.Equal("application/problem+json", context.Response.ContentType);

        context.Response.Body.Seek(0, SeekOrigin.Begin);
        using var reader = new StreamReader(context.Response.Body);
        var responseBody = await reader.ReadToEndAsync();

        Assert.Contains("Validation Failure", responseBody);
        Assert.Contains("Invalid email format", responseBody);
        Assert.Contains("Name is required", responseBody);
    }

    [Fact]
    public async Task InvokeAsync_WhenNotFoundException_Returns404WithProblemDetails()
    {
        // Arrange
        var context = CreateHttpContext();
        RequestDelegate next = (ctx) => throw new NotFoundException("Employee", 999);

        var middleware = new GlobalExceptionHandlingMiddleware(next, _logger, _env);

        // Act
        await middleware.InvokeAsync(context);

        // Assert
        Assert.Equal(StatusCodes.Status404NotFound, context.Response.StatusCode);
        Assert.Equal("application/problem+json", context.Response.ContentType);

        context.Response.Body.Seek(0, SeekOrigin.Begin);
        using var reader = new StreamReader(context.Response.Body);
        var responseBody = await reader.ReadToEndAsync();

        Assert.Contains("Not Found", responseBody);
        Assert.Contains("999", responseBody);
    }

    [Fact]
    public async Task InvokeAsync_WhenConflictException_Returns409WithProblemDetails()
    {
        // Arrange
        var context = CreateHttpContext();
        RequestDelegate next = (ctx) => throw new ConflictException("Email already exists");

        var middleware = new GlobalExceptionHandlingMiddleware(next, _logger, _env);

        // Act
        await middleware.InvokeAsync(context);

        // Assert
        Assert.Equal(StatusCodes.Status409Conflict, context.Response.StatusCode);
        Assert.Equal("application/problem+json", context.Response.ContentType);

        context.Response.Body.Seek(0, SeekOrigin.Begin);
        using var reader = new StreamReader(context.Response.Body);
        var responseBody = await reader.ReadToEndAsync();

        Assert.Contains("Conflict", responseBody);
        Assert.Contains("Email already exists", responseBody);
    }

    [Fact]
    public async Task InvokeAsync_WhenUnhandledException_Returns500WithProblemDetails()
    {
        // Arrange
        var context = CreateHttpContext();
        RequestDelegate next = (ctx) => throw new InvalidOperationException("Unexpected fatal crash");

        var middleware = new GlobalExceptionHandlingMiddleware(next, _logger, _env);

        // Act
        await middleware.InvokeAsync(context);

        // Assert
        Assert.Equal(StatusCodes.Status500InternalServerError, context.Response.StatusCode);
        Assert.Equal("application/problem+json", context.Response.ContentType);

        context.Response.Body.Seek(0, SeekOrigin.Begin);
        using var reader = new StreamReader(context.Response.Body);
        var responseBody = await reader.ReadToEndAsync();

        Assert.Contains("Unexpected fatal crash", responseBody);
    }

    private static DefaultHttpContext CreateHttpContext()
    {
        var context = new DefaultHttpContext();
        context.Response.Body = new MemoryStream();
        context.Request.Path = "/api/test";
        return context;
    }

    private class FakeHostEnvironment : IHostEnvironment
    {
        public string EnvironmentName { get; set; } = Environments.Development;
        public string ApplicationName { get; set; } = "EmployeeManagement.API";
        public string ContentRootPath { get; set; } = AppContext.BaseDirectory;
        public Microsoft.Extensions.FileProviders.IFileProvider ContentRootFileProvider { get; set; } = null!;
    }
}
