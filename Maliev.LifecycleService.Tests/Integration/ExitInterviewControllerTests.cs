using System.Net;
using Maliev.LifecycleService.Application.DTOs;
using Maliev.LifecycleService.Domain.Entities;
using Maliev.LifecycleService.Domain.Enums;
using Maliev.LifecycleService.Infrastructure.Data;
using Microsoft.Extensions.DependencyInjection;
using Xunit;

namespace Maliev.LifecycleService.Tests.Integration;

[Collection("IntegrationTests")]
public class ExitInterviewControllerTests : BaseIntegrationTest
{
    public ExitInterviewControllerTests(TestUtilities.LifecycleTestWebApplicationFactory factory) : base(factory)
    {
        // Ensure test user has Admin role and permission for these tests
        var claims = new[] {
            new System.Security.Claims.Claim("permissions", Maliev.LifecycleService.Domain.Authorization.LifecyclePermissions.Admin),
            new System.Security.Claims.Claim("permissions", Maliev.LifecycleService.Domain.Authorization.LifecyclePermissions.Manage),
            new System.Security.Claims.Claim(System.Security.Claims.ClaimTypes.Role, "Admin")
        };
        var token = factory.CreateTestToken("test-user", claims);
        Client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);
    }

    [Fact]
    public async Task RecordExitInterview_ValidRequest_ReturnsCreated()
    {
        // Arrange
        var employeeId = Guid.NewGuid();

        // Create offboarding checklist first (required for exit interview)
        using (var scope = ServiceProvider.CreateScope())
        {
            var context = scope.ServiceProvider.GetRequiredService<LifecycleDbContext>();
            await context.Database.EnsureCreatedAsync();

            var checklist = new OffboardingChecklist
            {
                Id = Guid.NewGuid(),
                EmployeeId = employeeId,
                TerminationDate = DateTime.UtcNow.AddDays(14),
                TerminationReason = "Resignation",
                EligibleForRehire = true,
                Status = Domain.Enums.OffboardingStatus.InProgress,
                Tasks = new List<OffboardingTask>()
            };
            context.OffboardingChecklists.Add(checklist);
            await context.SaveChangesAsync();
        }

        var request = new RecordExitInterviewRequest
        {
            ConductedBy = Guid.NewGuid(),
            InterviewDate = DateTime.UtcNow,
            ReasonForLeaving = "Better opportunity",
            FeedbackOnManager = "Supportive and helpful",
            FeedbackOnTeam = "Great collaboration",
            FeedbackOnCompany = "Good culture, could improve benefits",
            ImprovementSuggestions = "More flexible work hours",
            WouldRecommendCompany = true
        };

        // Act
        var response = await Client.PostAsJsonSnakeCaseAsync($"/lifecycle/v1/employees/{employeeId}/exit-interview", request);

        // Assert
        Assert.Equal(HttpStatusCode.Created, response.StatusCode);
        var result = await response.Content.ReadFromJsonSnakeCaseAsync<ExitInterviewCreatedResponse>();
        Assert.NotNull(result);
        Assert.NotEqual(Guid.Empty, result.Id);
    }

    [Fact]
    public async Task GetExitInterview_ExistingInterview_ReturnsOk()
    {
        // Arrange
        var employeeId = Guid.NewGuid();
        Guid offboardingChecklistId;

        using (var scope = ServiceProvider.CreateScope())
        {
            var context = scope.ServiceProvider.GetRequiredService<LifecycleDbContext>();
            await context.Database.EnsureCreatedAsync();

            // Create offboarding checklist first
            var checklist = new OffboardingChecklist
            {
                Id = Guid.NewGuid(),
                EmployeeId = employeeId,
                TerminationDate = DateTime.UtcNow.AddDays(14),
                TerminationReason = "Resignation",
                EligibleForRehire = true,
                Status = Domain.Enums.OffboardingStatus.InProgress,
                Tasks = new List<OffboardingTask>()
            };
            context.OffboardingChecklists.Add(checklist);
            await context.SaveChangesAsync();
            offboardingChecklistId = checklist.Id;

            var interview = new ExitInterview
            {
                Id = Guid.NewGuid(),
                OffboardingChecklistId = offboardingChecklistId,
                ConductedBy = Guid.NewGuid(),
                InterviewDate = DateTime.UtcNow,
                ReasonForLeaving = "Better opportunity",
                FeedbackOnManager = "Good manager",
                FeedbackOnTeam = "Great team",
                FeedbackOnCompany = "Positive experience",
                ImprovementSuggestions = "More training",
                WouldRecommendCompany = true,
                CreatedDate = DateTime.UtcNow
            };
            context.ExitInterviews.Add(interview);
            await context.SaveChangesAsync();
        }

        // Act
        var response = await Client.GetAsync($"/lifecycle/v1/employees/{employeeId}/exit-interview");

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        var result = await response.Content.ReadFromJsonSnakeCaseAsync<ExitInterviewDto>();
        Assert.NotNull(result);
        Assert.Equal(offboardingChecklistId, result.OffboardingChecklistId);
    }

    [Fact]
    public async Task GetExitInterview_NonExistingInterview_ReturnsNotFound()
    {
        // Arrange
        var employeeId = Guid.NewGuid();

        // Act
        var response = await Client.GetAsync($"/lifecycle/v1/employees/{employeeId}/exit-interview");

        // Assert
        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    [Fact]
    public async Task RecordExitInterview_MinimalData_ReturnsCreated()
    {
        // Arrange
        var employeeId = Guid.NewGuid();

        // Create offboarding checklist first (required for exit interview)
        using (var scope = ServiceProvider.CreateScope())
        {
            var context = scope.ServiceProvider.GetRequiredService<LifecycleDbContext>();
            await context.Database.EnsureCreatedAsync();

            var checklist = new OffboardingChecklist
            {
                Id = Guid.NewGuid(),
                EmployeeId = employeeId,
                TerminationDate = DateTime.UtcNow.AddDays(14),
                TerminationReason = "Resignation",
                EligibleForRehire = true,
                Status = Domain.Enums.OffboardingStatus.InProgress,
                Tasks = new List<OffboardingTask>()
            };
            context.OffboardingChecklists.Add(checklist);
            await context.SaveChangesAsync();
        }

        var request = new RecordExitInterviewRequest
        {
            ConductedBy = Guid.NewGuid(),
            InterviewDate = DateTime.UtcNow,
            ReasonForLeaving = null,
            FeedbackOnManager = null,
            FeedbackOnTeam = null,
            FeedbackOnCompany = null,
            ImprovementSuggestions = null,
            WouldRecommendCompany = false
        };

        // Act
        var response = await Client.PostAsJsonSnakeCaseAsync($"/lifecycle/v1/employees/{employeeId}/exit-interview", request);

        // Assert
        Assert.Equal(HttpStatusCode.Created, response.StatusCode);
    }
}

public record ExitInterviewCreatedResponse(Guid Id);
public record RecordExitInterviewRequest
{
    public Guid ConductedBy { get; init; }
    public DateTime InterviewDate { get; init; }
    public string? ReasonForLeaving { get; init; }
    public string? FeedbackOnManager { get; init; }
    public string? FeedbackOnTeam { get; init; }
    public string? FeedbackOnCompany { get; init; }
    public string? ImprovementSuggestions { get; init; }
    public bool WouldRecommendCompany { get; init; }
}
