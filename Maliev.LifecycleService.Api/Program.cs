using Maliev.Aspire.ServiceDefaults.IAM;
using Maliev.LifecycleService.Application.Commands.Onboarding.Handlers;
using Maliev.LifecycleService.Application.Commands.Offboarding.Handlers;
using Maliev.LifecycleService.Application.Commands.Templates.Handlers;
using Maliev.LifecycleService.Application.Commands.ExitInterview.Handlers;
using Maliev.LifecycleService.Application.Interfaces;
using Maliev.LifecycleService.Application.Queries.Onboarding.Handlers;
using Maliev.LifecycleService.Application.Queries.Offboarding.Handlers;
using Maliev.LifecycleService.Application.Queries.Templates.Handlers;
using Maliev.LifecycleService.Application.Queries.ExitInterview.Handlers;
using Maliev.LifecycleService.Application.Services;
using Maliev.LifecycleService.Infrastructure.BackgroundServices;
using Maliev.LifecycleService.Infrastructure.Caching;
using Maliev.LifecycleService.Infrastructure.Consumers;
using Maliev.LifecycleService.Infrastructure.Data;
using Maliev.LifecycleService.Infrastructure.IAM;
using Maliev.LifecycleService.Infrastructure.Messaging;
using Maliev.LifecycleService.Infrastructure.Metrics;
using Maliev.LifecycleService.Infrastructure.Repositories;
using MassTransit;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// --- 1. Secrets & Configuration ---
builder.AddGoogleSecretManagerVolume();

// --- 2. Infrastructure & Observability ---
builder.AddServiceDefaults();
builder.AddStandardMiddleware(options =>
{
    options.EnableRequestLogging = true;
});
builder.AddServiceMeters("lifecycle-service");

// --- 3. Data & Cache ---
builder.AddPostgresDbContext<LifecycleDbContext>(connectionName: "LifecycleDbContext");
builder.AddRedisDistributedCache(instanceName: "lifecycle:");

// --- 4. Messaging ---
builder.AddMassTransitWithRabbitMq(
    configure: x =>
    {
        x.AddConsumer<EmployeeCreatedEventConsumer>();
        x.AddConsumer<EmployeeTerminatedEventConsumer>();
        x.AddConsumer<UndoRevokeAccessConsumer>();
    },
    configureRabbitMq: (context, cfg) =>
    {
        cfg.UseMessageRetry(r =>
        {
            r.Exponential(3, TimeSpan.FromSeconds(2), TimeSpan.FromSeconds(30), TimeSpan.FromSeconds(5));
        });
        cfg.ConfigureEndpoints(context);
    });

// --- 5. Security ---
builder.AddJwtAuthentication();
builder.Services.AddIAMRegistration<LifecycleIAMRegistrationService>();

// --- 6. API Configuration ---
builder.AddDefaultCors();
builder.AddDefaultApiVersioning();
builder.AddStandardRateLimiting();

if (!builder.Environment.IsProduction())
{
    builder.AddStandardOpenApi(
        title: "MALIEV Lifecycle Service API",
        description: "Manages employee onboarding, offboarding, and lifecycle events.");
}

builder.Services.AddControllers()
.AddJsonOptions(options =>
{
    options.JsonSerializerOptions.PropertyNamingPolicy = System.Text.Json.JsonNamingPolicy.SnakeCaseLower;
    options.JsonSerializerOptions.ReferenceHandler = System.Text.Json.Serialization.ReferenceHandler.IgnoreCycles;
});

// --- 7. Application Services ---
builder.Services.AddScoped<IAuditLogRepository, AuditLogRepository>();
builder.Services.AddScoped<IAuditLogService, AuditLogService>();
builder.Services.AddScoped<IEventPublisher, EventPublisher>();
builder.Services.AddSingleton<ILifecycleMetrics, LifecycleMetrics>();

builder.Services.AddScoped<IOnboardingRepository, OnboardingRepository>();
builder.Services.AddScoped<IOffboardingRepository, OffboardingRepository>();
builder.Services.AddScoped<ITemplateRepository, TemplateRepository>();
builder.Services.AddScoped<IExitInterviewRepository, ExitInterviewRepository>();
builder.Services.AddScoped<ITemplateCacheService, TemplateCacheService>();
builder.Services.AddScoped<IOnboardingTemplateService, OnboardingTemplateService>();
builder.Services.AddScoped<IPaycheckBlockingService, PaycheckBlockingService>();
builder.Services.AddScoped<Maliev.LifecycleService.Application.Commands.Handlers.UndoRevokeAccessCommandHandler>();

// Handlers (could be optimized with MediatR if needed, but keeping existing pattern)
builder.Services.AddScoped<GetOnboardingStatusQueryHandler>();
builder.Services.AddScoped<GetPendingOnboardingsQueryHandler>();
builder.Services.AddScoped<StartOnboardingCommandHandler>();
builder.Services.AddScoped<CompleteOnboardingItemCommandHandler>();
builder.Services.AddScoped<ReassignOnboardingItemCommandHandler>();

builder.Services.AddScoped<StartOffboardingCommandHandler>();
builder.Services.AddScoped<CompleteOffboardingTaskCommandHandler>();
builder.Services.AddScoped<ReassignOffboardingTaskCommandHandler>();
builder.Services.AddScoped<GetOffboardingStatusQueryHandler>();
builder.Services.AddScoped<GetPendingOffboardingsQueryHandler>();

builder.Services.AddScoped<CreateTemplateCommandHandler>();
builder.Services.AddScoped<UpdateTemplateCommandHandler>();
builder.Services.AddScoped<DeleteTemplateCommandHandler>();
builder.Services.AddScoped<GetTemplateQueryHandler>();
builder.Services.AddScoped<ListTemplatesQueryHandler>();

builder.Services.AddScoped<RecordExitInterviewCommandHandler>();
builder.Services.AddScoped<GetExitInterviewQueryHandler>();

// Background Services
builder.Services.AddHostedService<AccessRevocationBackgroundService>();
builder.Services.AddHostedService<OnboardingReminderBackgroundService>();

var app = builder.Build();

// --- 8. Database Migrations ---
try
{
    await app.MigrateDatabaseAsync<LifecycleDbContext>();
}
catch (Exception ex)
{
    logger.LogError(ex, "Database migration failed");
}

// --- 9. Middleware Pipeline ---
app.UseStandardMiddleware();
app.UseHttpsRedirection();
app.UseRouting();
app.UseCors();
app.UseAuthentication();
app.UseAuthorization();
app.UseRateLimiter();

// --- 10. Endpoints ---
app.MapControllers();
app.MapDefaultEndpoints(servicePrefix: "lifecycle");
app.MapApiDocumentation(servicePrefix: "lifecycle");

await app.RunAsync();

/// <summary>
/// Partial class for Program to enable integration testing.
/// </summary>
public partial class Program { }