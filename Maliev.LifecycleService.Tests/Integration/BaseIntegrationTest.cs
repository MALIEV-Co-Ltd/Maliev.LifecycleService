using System.Net.Http.Headers;
using Maliev.LifecycleService.Infrastructure.Data;
using Maliev.LifecycleService.Tests.TestUtilities;
using Microsoft.Extensions.DependencyInjection;
using Xunit;

namespace Maliev.LifecycleService.Tests.Integration;

/// <summary>
/// Base class for all integration tests, providing access to shared infrastructure fixtures.
/// </summary>
public abstract class BaseIntegrationTest : IClassFixture<LifecycleTestWebApplicationFactory>
{
    protected readonly LifecycleTestWebApplicationFactory Factory;
    protected readonly HttpClient Client;
    protected readonly IServiceProvider ServiceProvider;

    protected BaseIntegrationTest(LifecycleTestWebApplicationFactory factory)
    {
        Factory = factory;
        Client = factory.CreateClient();

        // Set up authentication
        var claims = new[] {
            new System.Security.Claims.Claim("permissions", Maliev.LifecycleService.Domain.Authorization.LifecyclePermissions.TemplatesRead),
            new System.Security.Claims.Claim("permissions", Maliev.LifecycleService.Domain.Authorization.LifecyclePermissions.OnboardingsRead),
            new System.Security.Claims.Claim("permissions", Maliev.LifecycleService.Domain.Authorization.LifecyclePermissions.OnboardingsCreate),
            new System.Security.Claims.Claim("permissions", Maliev.LifecycleService.Domain.Authorization.LifecyclePermissions.OnboardingsUpdate),
            new System.Security.Claims.Claim("permissions", Maliev.LifecycleService.Domain.Authorization.LifecyclePermissions.OffboardingsRead),
            new System.Security.Claims.Claim("permissions", Maliev.LifecycleService.Domain.Authorization.LifecyclePermissions.OffboardingsCreate),
            new System.Security.Claims.Claim("permissions", Maliev.LifecycleService.Domain.Authorization.LifecyclePermissions.OffboardingsUpdate),
            new System.Security.Claims.Claim("permissions", Maliev.LifecycleService.Domain.Authorization.LifecyclePermissions.ExitInterviewsRead),
            new System.Security.Claims.Claim("permissions", Maliev.LifecycleService.Domain.Authorization.LifecyclePermissions.ExitInterviewsCreate),
            new System.Security.Claims.Claim("permissions", Maliev.LifecycleService.Domain.Authorization.LifecyclePermissions.TemplatesCreate),
            new System.Security.Claims.Claim("permissions", Maliev.LifecycleService.Domain.Authorization.LifecyclePermissions.TemplatesUpdate),
            new System.Security.Claims.Claim("permissions", Maliev.LifecycleService.Domain.Authorization.LifecyclePermissions.TemplatesDelete)
        };
        var token = factory.CreateTestToken("test-user", claims);
        Client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

        ServiceProvider = factory.Services;
    }
}
