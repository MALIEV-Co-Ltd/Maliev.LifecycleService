using Maliev.LifecycleService.Application.Interfaces;

namespace Maliev.LifecycleService.Application.Commands.Onboarding.Handlers;

/// <summary>
/// Handles the reassignment of an onboarding item.
/// </summary>
public class ReassignOnboardingItemCommandHandler
{
    private readonly IOnboardingRepository _repository;
    private readonly IAuditLogService _auditLogService;

    /// <summary>
    /// Initializes a new instance of the <see cref="ReassignOnboardingItemCommandHandler"/> class.
    /// </summary>
    /// <param name="repository">The onboarding repository.</param>
    /// <param name="auditLogService">The audit logging service.</param>
    public ReassignOnboardingItemCommandHandler(IOnboardingRepository repository, IAuditLogService auditLogService)
    {
        _repository = repository;
        _auditLogService = auditLogService;
    }

    /// <summary>
    /// Handles the reassignment of an onboarding item.
    /// </summary>
    /// <param name="command">The reassign onboarding item command.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>A task representing the asynchronous operation.</returns>
    /// <exception cref="Exception">Thrown when the onboarding item is not found.</exception>
    public async Task HandleAsync(ReassignOnboardingItemCommand command, CancellationToken cancellationToken = default)
    {
        var item = await _repository.GetItemByIdAsync(command.ItemId, cancellationToken);
        if (item == null)
        {
            throw new Exception("Onboarding item not found");
        }

        var beforeState = new { item.AssignedTo };

        item.AssignedTo = command.AssignedTo;

        await _repository.UpdateAsync(item.Checklist, cancellationToken);

        await _auditLogService.LogAsync(
            "OnboardingItem",
            item.Id,
            "Reassigned",
            command.UserId,
            beforeState,
            new { item.AssignedTo },
            cancellationToken);
    }
}
