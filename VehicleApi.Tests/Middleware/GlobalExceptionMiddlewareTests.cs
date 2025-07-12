using System.Text.Json;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using NSubstitute;
using VehicleApi.Middleware;

namespace VehicleApi.Tests.Middleware;

public class GlobalExceptionMiddlewareTests
{
    [Fact]
    public async Task InvokeAsync_NoException_PassesThrough()
    {
        var context = new DefaultHttpContext();
        var logger = Substitute.For<ILogger<GlobalExceptionMiddleware>>();
        var called = false;
        var middleware = new GlobalExceptionMiddleware(_ => { called = true; return Task.CompletedTask; }, logger);

        await middleware.InvokeAsync(context);
        Assert.True(called);
        Assert.Equal(200, context.Response.StatusCode);
    }

    [Fact]
    public async Task InvokeAsync_ExceptionThrown_ReturnsProblemDetails()
    {
        var context = new DefaultHttpContext();
        context.TraceIdentifier = "test-trace-id";
        context.Request.Path = "/test-path";
        var logger = Substitute.For<ILogger<GlobalExceptionMiddleware>>();
        var middleware = new GlobalExceptionMiddleware(_ => throw new Exception("fail"), logger);

        var responseStream = new MemoryStream();
        context.Response.Body = responseStream;

        await middleware.InvokeAsync(context);
        responseStream.Position = 0;
        var json = await new StreamReader(responseStream).ReadToEndAsync();
        var problem = JsonSerializer.Deserialize<ProblemDetails>(json, new JsonSerializerOptions { PropertyNamingPolicy = JsonNamingPolicy.CamelCase });

        Assert.Equal(500, context.Response.StatusCode);
        Assert.Equal("application/problem+json", context.Response.ContentType);
        Assert.Equal("An unexpected error occurred.", problem?.Title);
        Assert.Equal("/test-path", problem?.Instance);
        Assert.Equal(500, problem?.Status);
        Assert.Contains("test-trace-id", problem?.Detail);
    }
}
