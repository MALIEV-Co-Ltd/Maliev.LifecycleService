using System.Net;
using Maliev.LifecycleService.Application.DTOs;
using Maliev.LifecycleService.Domain.Entities;
using Maliev.LifecycleService.Domain.Enums;
using Maliev.LifecycleService.Infrastructure.Data;
using Microsoft.Extensions.DependencyInjection;
using Xunit;

namespace Maliev.LifecycleService.Tests.Integration;

[Collection("IntegrationTests")]
public class OffboardingControllerTests : BaseIntegrationTest
{
    public OffboardingControllerTests(TestUtilities.LifecycleTestWebApplicationFactory factory) : base(factory)
    {
    }

    [Fact]
    public async Task StartOffboarding_ValidRequest_ReturnsCreated()
    {
        // Arrange
        var employeeId = Guid.NewGuid();
        var request = new StartOffboardingRequest
        {
            TerminationDate = DateTime.UtcNow.AddDays(14),
            TerminationReason = "Resignation",
            EligibleForRehire = true
        };

        // Act
        var response = await Client.PostAsJsonSnakeCaseAsync($"/lifecycle/v1/employees/{employeeId}/offboarding/start", request);

        // Assert
        Assert.Equal(HttpStatusCode.Created, response.StatusCode);
        var result = await response.Content.ReadFromJsonSnakeCaseAsync<OffboardingCreatedResponse>();
        Assert.NotNull(result);
        Assert.NotEqual(Guid.Empty, result.Id);
    }

    [Fact]
    public async Task GetStatus_ExistingOffboarding_ReturnsOk()
    {
        // Arrange
        var employeeId = Guid.NewGuid();

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
                Tasks = new List<OffboardingTask>
                {
                    new OffboardingTask
                    {
                        Title = "Collect company equipment",
                        Description = "Laptop, badge, keys",
                        IsCompleted = false
                    }
                }
            };
            context.OffboardingChecklists.Add(checklist);
            await context.SaveChangesAsync();
        }

        // Act
        var response = await Client.GetAsync($"/lifecycle/v1/employees/{employeeId}/offboarding/status");

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        var result = await response.Content.ReadFromJsonSnakeCaseAsync<OffboardingStatusDto>();
        Assert.NotNull(result);
        Assert.Equal(employeeId, result.EmployeeId);
    }

    [Fact]
    public async Task GetStatus_NonExistingOffboarding_ReturnsNotFound()
    {
        // Arrange
        var employeeId = Guid.NewGuid();

        // Act
        var response = await Client.GetAsync($"/lifecycle/v1/employees/{employeeId}/offboarding/status");

        // Assert
        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    [Fact]
    public async Task GetPending_ReturnsOkWithList()
    {
        // Arrange
        using (var scope = ServiceProvider.CreateScope())
        {
            var context = scope.ServiceProvider.GetRequiredService<LifecycleDbContext>();
            await context.Database.EnsureCreatedAsync();

            var checklist = new OffboardingChecklist
            {
                Id = Guid.NewGuid(),
                EmployeeId = Guid.NewGuid(),
                TerminationDate = DateTime.UtcNow.AddDays(14),
                TerminationReason = "Resignation",
                EligibleForRehire = true,
                Status = Domain.Enums.OffboardingStatus.InProgress,
                Tasks = new List<OffboardingTask>()
            };
            context.OffboardingChecklists.Add(checklist);
            await context.SaveChangesAsync();
        }

        // Act
        var response = await Client.GetAsync("/lifecycle/v1/offboarding/pending");

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        var result = await response.Content.ReadFromJsonSnakeCaseAsync<List<OffboardingStatusDto>>();
        Assert.NotNull(result);
        Assert.NotEmpty(result);
    }

    [Fact]
    public async Task CompleteTask_ValidRequest_ReturnsNoContent()
    {
        // Arrange
        Guid taskId;

        using (var scope = ServiceProvider.CreateScope())
        {
            var context = scope.ServiceProvider.GetRequiredService<LifecycleDbContext>();
            await context.Database.EnsureCreatedAsync();

            var checklist = new OffboardingChecklist
            {
                Id = Guid.NewGuid(),
                EmployeeId = Guid.NewGuid(),
                TerminationDate = DateTime.UtcNow.AddDays(14),
                TerminationReason = "Resignation",
                EligibleForRehire = true,
                Status = Domain.Enums.OffboardingStatus.InProgress,
                Tasks = new List<OffboardingTask>
                {
                    new OffboardingTask
                    {
                        Id = Guid.NewGuid(),
                        Title = "Task to complete",
                        Description = "Test task",
                        IsCompleted = false
                    }
                }
            };
            context.OffboardingChecklists.Add(checklist);
            await context.SaveChangesAsync();
            taskId = checklist.Tasks.First().Id;
        }

        var request = new CompleteTaskRequest
        {
            Notes = "Task completed successfully"
        };

        // Act
        var response = await Client.PutAsJsonSnakeCaseAsync($"/lifecycle/v1/offboarding-tasks/{taskId}/complete", request);

        // Assert
        Assert.Equal(HttpStatusCode.NoContent, response.StatusCode);
    }

    [Fact]
    public async Task ReassignTask_ValidRequest_ReturnsNoContent()
    {
        // Arrange
        Guid taskId;

        using (var scope = ServiceProvider.CreateScope())
        {
            var context = scope.ServiceProvider.GetRequiredService<LifecycleDbContext>();
            await context.Database.EnsureCreatedAsync();

            var checklist = new OffboardingChecklist
            {
                Id = Guid.NewGuid(),
                EmployeeId = Guid.NewGuid(),
                TerminationDate = DateTime.UtcNow.AddDays(14),
                TerminationReason = "Resignation",
                EligibleForRehire = true,
                Status = Domain.Enums.OffboardingStatus.InProgress,
                Tasks = new List<OffboardingTask>
                {
                    new OffboardingTask
                    {
                        Id = Guid.NewGuid(),
                        Title = "Task to reassign",
                        Description = "Test task",
                        IsCompleted = false,
                        AssignedTo = Guid.NewGuid()
                    }
                }
            };
            context.OffboardingChecklists.Add(checklist);
            await context.SaveChangesAsync();
            taskId = checklist.Tasks.First().Id;
        }

        var request = new ReassignTaskRequest
        {
            AssignedTo = Guid.NewGuid()
        };

        // Act
        var response = await Client.PutAsJsonSnakeCaseAsync($"/lifecycle/v1/offboarding-tasks/{taskId}/reassign", request);

        // Assert
        Assert.Equal(HttpStatusCode.NoContent, response.StatusCode);
    }
}

public record OffboardingCreatedResponse(Guid Id);
public record StartOffboardingRequest { public DateTime TerminationDate { get; init; } public string TerminationReason { get; init; } = string.Empty; public bool EligibleForRehire { get; init; } }
public record CompleteTaskRequest { public string? Notes { get; init; } }
public record ReassignTaskRequest { public Guid AssignedTo { get; init; } }
