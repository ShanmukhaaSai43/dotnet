using EmployeeManagement.API.Middleware;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;

namespace EmployeeManagement.UnitTests.DiAndMiddlewareTests;

public class RequestLoggingMiddlewareTests
{
    [Fact]
    public async Task InvokeAsync_ShouldAttachCorrelationIdAndExecuteNext()
    {
        // Arrange
        var context = new DefaultHttpContext();
        bool nextCalled = false;
        RequestDelegate next = (ctx) =>
        {
            nextCalled = true;
            ctx.Response.StatusCode = 200;
            return Task.CompletedTask;
        };

        var logger = new NullLogger<RequestLoggingMiddleware>();
        var middleware = new RequestLoggingMiddleware(next, logger);

        // Act
        await middleware.InvokeAsync(context);

        // Assert
        Assert.True(nextCalled);
        Assert.True(context.Items.ContainsKey(RequestLoggingMiddleware.CorrelationIdHeader));
        Assert.NotNull(context.Items[RequestLoggingMiddleware.CorrelationIdHeader]);
    }

    [Fact]
    public async Task InvokeAsync_PropagatesExistingCorrelationIdIfPresent()
    {
        // Arrange
        var customId = "my-custom-trace-12345";
        var context = new DefaultHttpContext();
        context.Request.Headers[RequestLoggingMiddleware.CorrelationIdHeader] = customId;

        RequestDelegate next = (ctx) => Task.CompletedTask;
        var logger = new NullLogger<RequestLoggingMiddleware>();
        var middleware = new RequestLoggingMiddleware(next, logger);

        // Act
        await middleware.InvokeAsync(context);

        // Assert
        Assert.Equal(customId, context.Items[RequestLoggingMiddleware.CorrelationIdHeader]?.ToString());
    }
}
