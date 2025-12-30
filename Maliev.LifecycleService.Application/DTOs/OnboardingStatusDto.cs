using Maliev.LifecycleService.Domain.Enums;

namespace Maliev.LifecycleService.Application.DTOs;

/// <summary>
/// Data transfer object representing the status of an onboarding process.
/// </summary>
/// <param name="Id">The unique identifier of the onboarding checklist.</param>
/// <param name="EmployeeId">The identifier of the employee being onboarded.</param>
/// <param name="Status">The current status of the onboarding process.</param>
/// <param name="StartDate">The scheduled start date for the employee.</param>
/// <param name="CompletedDate">The date when the onboarding process was completed.</param>
/// <param name="TotalItems">The total number of items in the onboarding checklist.</param>
/// <param name="CompletedItems">The number of completed items in the onboarding checklist.</param>
/// <param name="Items">The collection of items associated with this onboarding process.</param>
public record OnboardingStatusDto(
    Guid Id,
    Guid EmployeeId,
    OnboardingStatus Status,
    DateTime StartDate,
    DateTime? CompletedDate,
    int TotalItems,
    int CompletedItems,
    List<OnboardingItemDto> Items);