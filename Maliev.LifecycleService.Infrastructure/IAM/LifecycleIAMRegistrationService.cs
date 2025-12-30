using Maliev.Aspire.ServiceDefaults.IAM;
using Maliev.LifecycleService.Domain.Authorization;
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
    /// <param name="httpClientFactory">The HTTP client factory.</param>
    /// <param name="logger">The logger.</param>
    public LifecycleIAMRegistrationService(
        IHttpClientFactory httpClientFactory,
        ILogger<LifecycleIAMRegistrationService> logger)
        : base(httpClientFactory, logger, "LifecycleService")
    {
    }

    /// <inheritdoc/>
    protected override IEnumerable<PermissionRegistration> GetPermissions()
    {
        return new List<PermissionRegistration>
        {
            new PermissionRegistration { PermissionId = LifecyclePermissions.Manage, Description = "Manage onboarding/offboarding workflows" },
            new PermissionRegistration { PermissionId = LifecyclePermissions.Admin, Description = "Manage templates and administrative functions" }
        };
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
                PermissionIds = new List<string> { LifecyclePermissions.Manage }
            },
            new RoleRegistration
            {
                RoleId = "roles.lifecycle.admin",
                Description = "Administrative access to lifecycle service settings and templates",
                PermissionIds = new List<string> { LifecyclePermissions.Manage, LifecyclePermissions.Admin }
            }
        };
    }
}