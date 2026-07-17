using System.Net;
using System.Net.Http.Headers;
using System.Security.Cryptography;
using System.Text;
using Maliev.Aspire.ServiceDefaults.IAM;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Http;
using Microsoft.Extensions.Options;

namespace Maliev.LifecycleService.Tests.Unit.Security;

/// <summary>
/// Verifies LifecycleService's centrally issued workload-authentication boundary.
/// </summary>
public sealed class ServiceAuthenticationWiringTests
{
    private const string ExpectedToken = "centrally-issued-lifecycle-token";

    /// <summary>
    /// Startup must opt into AuthService exchange while preserving RabbitMQ IAM registration.
    /// </summary>
    [Fact]
    public void Program_RegistersLifecycleExchangeWithoutLegacySigner()
    {
        var source = ReadRepositoryFile("Maliev.LifecycleService.Api", "Program.cs");

        Assert.Contains("builder.AddAuthServiceTokenExchange(\"LifecycleService\");", source, StringComparison.Ordinal);
        Assert.Contains("builder.AddAuthServiceIAMClient();", source, StringComparison.Ordinal);
        Assert.Contains("AddIAMRegistration<LifecycleIAMRegistrationService>(\"lifecycle\")", source, StringComparison.Ordinal);
        Assert.DoesNotContain("AddIAMServiceClient", source, StringComparison.Ordinal);
        Assert.DoesNotContain("AddAuthenticatedServiceClient", source, StringComparison.Ordinal);
    }

    /// <summary>
    /// The IAM client must attach the centrally issued bearer token.
    /// </summary>
    [Fact]
    public async Task IamClient_UsesAuthServiceBearer()
    {
        var builder = CreateConfiguredBuilder();
        var filter = new TrackingPrimaryHandlerFilter();
        builder.Services.AddSingleton<IHttpMessageHandlerBuilderFilter>(filter);

        builder.AddAuthServiceTokenExchange("LifecycleService");
        builder.Services.AddSingleton<IAuthServiceTokenProvider>(new StubTokenProvider());
        builder.AddAuthServiceIAMClient();

        await using var provider = builder.Services.BuildServiceProvider();
        var factory = provider.GetRequiredService<IHttpClientFactory>();
        using var client = factory.CreateClient("IAMService");
        using var response = await client.GetAsync("/probe", CancellationToken.None);

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        Assert.Equal(
            new AuthenticationHeaderValue("Bearer", ExpectedToken),
            filter.GetCapture("IAMService").Authorization);
        Assert.True(filter.HasAuthServiceHandler("IAMService"));
    }

    /// <summary>
    /// Lifecycle uses its exact process identity and cannot resolve legacy signing services.
    /// </summary>
    [Fact]
    public void LifecycleExchange_RegistersExactIdentityWithoutLegacySigningServices()
    {
        var builder = CreateConfiguredBuilder();
        builder.AddAuthServiceTokenExchange("LifecycleService");
        builder.AddAuthServiceIAMClient();

        using var provider = builder.Services.BuildServiceProvider();

        Assert.Equal("LifecycleService", provider.GetRequiredService<ServiceProcessIdentity>().ServiceName);
        Assert.Null(provider.GetService<IServiceAccountTokenProvider>());
        Assert.Null(provider.GetService<ServiceAccountAuthenticationHandler>());
    }

    /// <summary>
    /// Invalid workload credentials must stop the host rather than permit anonymous fallback.
    /// </summary>
    [Theory]
    [InlineData(null, null)]
    [InlineData("service-lifecycle-service", "short")]
    public async Task AuthServiceExchange_InvalidCredentials_FailsClosedAtHostStartup(
        string? clientId,
        string? clientSecret)
    {
        var builder = CreateConfiguredBuilder(clientId, clientSecret);
        builder.AddAuthServiceTokenExchange("LifecycleService");

        using var host = builder.Build();

        await Assert.ThrowsAsync<OptionsValidationException>(() => host.StartAsync());
    }

    private static HostApplicationBuilder CreateConfiguredBuilder(
        string? clientId = "service-lifecycle-service",
        string? clientSecret = "lifecycle-test-secret-with-at-least-32-bytes")
    {
        var builder = Host.CreateApplicationBuilder(new HostApplicationBuilderSettings
        {
            EnvironmentName = "Testing"
        });

        using var rsa = RSA.Create(2048);
        builder.Configuration["ServiceAuthentication:ClientId"] = clientId;
        builder.Configuration["ServiceAuthentication:ClientSecret"] = clientSecret;
        builder.Configuration["Services:AuthService:BaseUrl"] = "https://auth.test";
        builder.Configuration["Services:IAMService:BaseUrl"] = "https://iam.test";
        builder.Configuration["Jwt:PublicKey"] = Convert.ToBase64String(
            Encoding.UTF8.GetBytes(rsa.ExportSubjectPublicKeyInfoPem()));
        builder.Configuration["Jwt:Issuer"] = "https://api.maliev.com";
        builder.Configuration["Jwt:Audience"] = "https://api.maliev.com";

        return builder;
    }

    private static string ReadRepositoryFile(params string[] segments)
    {
        var path = Path.GetFullPath(Path.Combine(
            AppContext.BaseDirectory,
            "..",
            "..",
            "..",
            "..",
            Path.Combine(segments)));

        Assert.True(File.Exists(path), $"Could not find source file: {path}");
        return File.ReadAllText(path);
    }

    private sealed class StubTokenProvider : IAuthServiceTokenProvider
    {
        public Task<string> GetTokenAsync(CancellationToken cancellationToken = default) =>
            Task.FromResult(ExpectedToken);
    }

    private sealed class AuthorizationCaptureHandler : HttpMessageHandler
    {
        public AuthenticationHeaderValue? Authorization { get; private set; }

        protected override Task<HttpResponseMessage> SendAsync(
            HttpRequestMessage request,
            CancellationToken cancellationToken)
        {
            Authorization = request.Headers.Authorization;
            return Task.FromResult(new HttpResponseMessage(HttpStatusCode.OK));
        }
    }

    private sealed class TrackingPrimaryHandlerFilter : IHttpMessageHandlerBuilderFilter
    {
        private readonly Dictionary<string, AuthorizationCaptureHandler> _captures = new(StringComparer.Ordinal);
        private readonly Dictionary<string, bool> _authHandlers = new(StringComparer.Ordinal);

        public AuthorizationCaptureHandler GetCapture(string clientName) => _captures[clientName];

        public bool HasAuthServiceHandler(string clientName) => _authHandlers[clientName];

        public Action<HttpMessageHandlerBuilder> Configure(Action<HttpMessageHandlerBuilder> next) => builder =>
        {
            next(builder);
            var clientName = builder.Name
                ?? throw new InvalidOperationException("Every HttpClientFactory handler must have a client name.");
            _authHandlers[clientName] = builder.AdditionalHandlers.Any(
                handler => handler is AuthServiceTokenExchangeHandler);

            for (var index = builder.AdditionalHandlers.Count - 1; index >= 0; index--)
            {
                if (builder.AdditionalHandlers[index].GetType().FullName?.Contains(
                        "ServiceDiscovery",
                        StringComparison.Ordinal) == true ||
                    builder.AdditionalHandlers[index].GetType().FullName?.Contains(
                        "ResolvingHttpDelegatingHandler",
                        StringComparison.Ordinal) == true)
                {
                    builder.AdditionalHandlers.RemoveAt(index);
                }
            }

            var capture = new AuthorizationCaptureHandler();
            _captures[clientName] = capture;
            builder.PrimaryHandler = capture;
        };
    }
}
