using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.IdentityModel.Tokens;
using Moq;
using Maliev.LifecycleService.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using Testcontainers.PostgreSql;
using Testcontainers.RabbitMq;
using Testcontainers.Redis;
using MassTransit;

namespace Maliev.LifecycleService.Tests.TestUtilities;

public class LifecycleTestWebApplicationFactory : WebApplicationFactory<Program>, IAsyncLifetime
{
    private readonly PostgreSqlContainer _postgresContainer =
#pragma warning disable CS0618
        new PostgreSqlBuilder().WithImage("postgres:18-alpine")
        .Build();

    private readonly RedisContainer _redisContainer = new RedisBuilder().WithImage("redis:7-alpine")
        .Build();

    private readonly RabbitMqContainer _rabbitmqContainer = new RabbitMqBuilder().WithImage("rabbitmq:4-management-alpine")
        .Build();
#pragma warning restore CS0618

    private readonly RSA _testRsa = RSA.Create(2048);

    public string CreateTestToken(string userId = "test-user", IEnumerable<Claim>? claims = null)
    {
        var allClaims = new List<Claim>
        {
            new(JwtRegisteredClaimNames.Sub, userId),
            new(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
        };

        if (claims != null)
        {
            allClaims.AddRange(claims);
        }

        var key = new RsaSecurityKey(_testRsa);
        var creds = new SigningCredentials(key, SecurityAlgorithms.RsaSha256);

        var token = new JwtSecurityToken(
            issuer: "test-issuer",
            audience: "test-audience",
            claims: allClaims,
            expires: DateTime.UtcNow.AddHours(1),
            signingCredentials: creds
        );

        return new JwtSecurityTokenHandler().WriteToken(token);
    }

    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.UseEnvironment("Testing");
        builder.UseSetting("CORS:AllowedOrigins:0", "http://localhost:3000");
        builder.UseSetting("Features:FailOpenOnIAMError", "true");
        builder.UseSetting("IAM:RegistrationDelaySeconds", "0");

        builder.ConfigureAppConfiguration((_, configuration) =>
        {
            configuration.AddInMemoryCollection(new Dictionary<string, string?>
            {
                ["ServiceAuthentication:ClientId"] = "service-lifecycle-service",
                ["ServiceAuthentication:ClientSecret"] = "lifecycle-integration-secret-with-at-least-32-bytes",
                ["Services:AuthService:BaseUrl"] = "https://auth.test",
                ["Services:IAMService:BaseUrl"] = "https://iam.test",
                ["Jwt:PublicKey"] = Convert.ToBase64String(
                    System.Text.Encoding.UTF8.GetBytes(_testRsa.ExportSubjectPublicKeyInfoPem())),
                ["Jwt:Issuer"] = "https://api.maliev.com",
                ["Jwt:Audience"] = "https://api.maliev.com"
            });
        });

        // Set environment variables for connection strings (read early in configuration pipeline)
        Environment.SetEnvironmentVariable("ConnectionStrings__LifecycleDbContext", _postgresContainer.GetConnectionString());
        Environment.SetEnvironmentVariable("ConnectionStrings__redis", _redisContainer.GetConnectionString());
        Environment.SetEnvironmentVariable("ConnectionStrings__rabbitmq", _rabbitmqContainer.GetConnectionString());

        builder.ConfigureTestServices(services =>
        {
            services.RemoveAll<Maliev.Aspire.ServiceDefaults.IAM.IIamServiceClient>();
            var iamClient = new Moq.Mock<Maliev.Aspire.ServiceDefaults.IAM.IIamServiceClient>();
            iamClient
                .Setup(client => client.CheckPermissionAsync(
                    Moq.It.IsAny<string>(),
                    Moq.It.IsAny<string>(),
                    Moq.It.IsAny<string>(),
                    Moq.It.IsAny<CancellationToken>()))
                .ReturnsAsync(false);
            services.AddScoped(_ => iamClient.Object);

            services.PostConfigureAll<JwtBearerOptions>(options =>
            {
                options.MapInboundClaims = false;
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = true,
                    ValidIssuer = "test-issuer",
                    ValidateAudience = true,
                    ValidAudience = "test-audience",
                    ValidateLifetime = true,
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new RsaSecurityKey(_testRsa)
                };
            });

            // Ensure MassTransit waits until started for tests to avoid race conditions
            services.Configure<MassTransitHostOptions>(options =>
            {
                options.WaitUntilStarted = true;
                options.StartTimeout = TimeSpan.FromSeconds(30);
            });

            services.AddMassTransitTestHarness();
        });
    }

    public async Task InitializeAsync()
    {
        await Task.WhenAll(
            _postgresContainer.StartAsync(),
            _redisContainer.StartAsync(),
            _rabbitmqContainer.StartAsync()
        );
    }

    public new async Task DisposeAsync()
    {
        await Task.WhenAll(
            _postgresContainer.DisposeAsync().AsTask(),
            _redisContainer.DisposeAsync().AsTask(),
            _rabbitmqContainer.DisposeAsync().AsTask()
        );
        _testRsa.Dispose();
    }
}




