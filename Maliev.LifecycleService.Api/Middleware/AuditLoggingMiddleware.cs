using System.Security.Claims;
using Maliev.LifecycleService.Application.Interfaces;

namespace Maliev.LifecycleService.Api.Middleware;

/// <summary>
/// Middleware for logging auditable state-changing API requests.
/// </summary>
public class AuditLoggingMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<AuditLoggingMiddleware> _logger;

    /// <summary>
    /// Initializes a new instance of the <see cref="AuditLoggingMiddleware"/> class.
    /// </summary>
    /// <param name="next">The next request delegate in the pipeline.</param>
    /// <param name="logger">The logger.</param>
    public AuditLoggingMiddleware(RequestDelegate next, ILogger<AuditLoggingMiddleware> logger)
    {
        _next = next;
        _logger = logger;
    }

    /// <summary>
    /// Invokes the middleware to log auditable requests.
    /// </summary>
    /// <param name="context">The HTTP context.</param>
    /// <param name="auditLogService">The audit logging service.</param>
    /// <returns>A task representing the asynchronous operation.</returns>
    public async Task InvokeAsync(HttpContext context, IAuditLogService auditLogService)
    {
        // We only audit state-changing operations (POST, PUT, DELETE)
        if (context.Request.Method == HttpMethods.Post ||
            context.Request.Method == HttpMethods.Put ||
            context.Request.Method == HttpMethods.Delete)
        {
            var userIdString = context.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (Guid.TryParse(userIdString, out var userId))
            {
                // Capture the request info for potential auditing
                // Note: Detailed entity auditing is usually done in command handlers
                // where we have access to before/after state.
                // This middleware can log the API call itself.
                _logger.LogInformation("Auditable request: {Method} {Path} by user {UserId}",
                    context.Request.Method, context.Request.Path, userId);
            }
        }

        await _next(context);
    }
}

/// <summary>
/// Extension methods for registering audit logging middleware.
/// </summary>
public static class AuditLoggingMiddlewareExtensions
{
    /// <summary>
    /// Adds audit logging middleware to the request pipeline.
    /// </summary>
    /// <param name="builder">The application builder.</param>
    /// <returns>The application builder.</returns>
    public static IApplicationBuilder UseAuditLogging(this IApplicationBuilder builder)
    {
        return builder.UseMiddleware<AuditLoggingMiddleware>();
    }
}