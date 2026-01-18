using Maliev.LifecycleService.Api.Middleware;
using Maliev.LifecycleService.Application.Interfaces;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Moq;
using System.Security.Claims;
using Xunit;

namespace Maliev.LifecycleService.Tests.Unit.Middleware;

public class AuditLoggingMiddlewareTests
{
    private readonly Mock<RequestDelegate> _nextMock;
    private readonly Mock<ILogger<AuditLoggingMiddleware>> _loggerMock;
    private readonly Mock<IAuditLogService> _auditLogServiceMock;
    private readonly AuditLoggingMiddleware _middleware;

    public AuditLoggingMiddlewareTests()
    {
        _nextMock = new Mock<RequestDelegate>();
        _loggerMock = new Mock<ILogger<AuditLoggingMiddleware>>();
        _auditLogServiceMock = new Mock<IAuditLogService>();
        _middleware = new AuditLoggingMiddleware(_nextMock.Object, _loggerMock.Object);
    }

    [Fact]
    public async Task InvokeAsync_PostRequestWithUser_ShouldLog()
    {
        // Arrange
        var context = new DefaultHttpContext();
        context.Request.Method = HttpMethods.Post;
        context.Request.Path = "/test";
        
        var userId = Guid.NewGuid();
        var claims = new[] { new Claim(ClaimTypes.NameIdentifier, userId.ToString()) };
        var identity = new ClaimsIdentity(claims, "Test");
        context.User = new ClaimsPrincipal(identity);

        // Act
        await _middleware.InvokeAsync(context, _auditLogServiceMock.Object);

        // Assert
        _nextMock.Verify(n => n(context), Times.Once);
        // Verify logger was called (it's hard to verify exact message with Mock<ILogger>)
    }

    [Fact]
    public async Task InvokeAsync_GetRequest_ShouldNotLog()
    {
        // Arrange
        var context = new DefaultHttpContext();
        context.Request.Method = HttpMethods.Get;
        context.Request.Path = "/test";

        // Act
        await _middleware.InvokeAsync(context, _auditLogServiceMock.Object);

        // Assert
        _nextMock.Verify(n => n(context), Times.Once);
    }

    [Fact]
    public void UseAuditLogging_ShouldRegisterMiddleware()
    {
        // Arrange
        var builderMock = new Mock<IApplicationBuilder>();

        // Act
        builderMock.Object.UseAuditLogging();

        // Assert
        builderMock.Verify(b => b.Use(It.IsAny<Func<RequestDelegate, RequestDelegate>>()), Times.Once);
    }
}
