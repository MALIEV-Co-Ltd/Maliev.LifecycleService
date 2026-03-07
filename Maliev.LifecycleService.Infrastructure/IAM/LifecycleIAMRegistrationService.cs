using Maliev.Aspire.ServiceDefaults.IAM;
using Maliev.LifecycleService.Domain.Authorization;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace Maliev.LifecycleService.Infrastructure.IAM;

/// <summary>
/// Service for registering Lifecycle Service permissions and roles with the IAM service.
/// </summary>
public class LifecycleIAMRegistrationService : IAMRegistrationService
{
    /// <summary>
    /// Initializes a new instance of the <see cref="LifecycleIAMRegistrationService"/> class.
    /// </summary>
    /// <param name="configuration">The application configuration.</param>
    /// <param name="logger">The logger.</param>
    public LifecycleIAMRegistrationService(
        IConfiguration configuration,
        ILogger<LifecycleIAMRegistrationService> logger)
        : base(configuration, logger, "lifecycle")
    {
    }

    /// <inheritdoc/>
    protected override IEnumerable<PermissionRegistration> GetPermissions()
    {
        return LifecyclePermissions.All.Select(p => new PermissionRegistration
        {
            PermissionId = p.Key,
            Description = p.Value
        });
    }

    /// <inheritdoc/>
    protected override IEnumerable<RoleRegistration> GetPredefinedRoles()
    {
        return new List<RoleRegistration>
        {
            new RoleRegistration
            {
                RoleId = "roles.lifecycle.manager",
                Description = "Full access to manage onboarding and offboarding workflows",
                PermissionIds = new List<string>
                {
                    LifecyclePermissions.OnboardingsRead,
                    LifecyclePermissions.OnboardingsCreate,
                    LifecyclePermissions.OnboardingsUpdate,
                    LifecyclePermissions.OffboardingsRead,
                    LifecyclePermissions.OffboardingsCreate,
                    LifecyclePermissions.OffboardingsUpdate,
                    LifecyclePermissions.ExitInterviewsRead,
                    LifecyclePermissions.ExitInterviewsCreate
                }
            },
            new RoleRegistration
            {
                RoleId = "roles.lifecycle.admin",
                Description = "Administrative access to lifecycle service settings and templates",
                PermissionIds = new List<string>
                {
                    LifecyclePermissions.OnboardingsRead,
                    LifecyclePermissions.OnboardingsCreate,
                    LifecyclePermissions.OnboardingsUpdate,
                    LifecyclePermissions.OffboardingsRead,
                    LifecyclePermissions.OffboardingsCreate,
                    LifecyclePermissions.OffboardingsUpdate,
                    LifecyclePermissions.ExitInterviewsRead,
                    LifecyclePermissions.ExitInterviewsCreate,
                    LifecyclePermissions.TemplatesRead,
                    LifecyclePermissions.TemplatesCreate,
                    LifecyclePermissions.TemplatesUpdate,
                    LifecyclePermissions.TemplatesDelete
                }
            }
        };
    }
}

