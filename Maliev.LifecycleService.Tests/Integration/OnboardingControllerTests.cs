using System.Net;
using Maliev.LifecycleService.Application.DTOs;
using Maliev.LifecycleService.Domain.Entities;
using Maliev.LifecycleService.Domain.Enums;
using Maliev.LifecycleService.Infrastructure.Data;
using Microsoft.Extensions.DependencyInjection;
using Xunit;

namespace Maliev.LifecycleService.Tests.Integration;

public class OnboardingControllerTests : BaseIntegrationTest
{
    public OnboardingControllerTests(TestUtilities.LifecycleTestWebApplicationFactory factory) : base(factory)
    {
    }

    [Fact]
    public async Task StartOnboarding_ValidRequest_ReturnsCreated()
    {
        // Arrange
        var employeeId = Guid.NewGuid();
        Guid templateId;

        using (var scope = ServiceProvider.CreateScope())
        {
            var context = scope.ServiceProvider.GetRequiredService<LifecycleDbContext>();
            await context.Database.EnsureCreatedAsync();

            var template = new OnboardingTemplate
            {
                Id = Guid.NewGuid(),
                Name = "Standard Onboarding",
                Description = "Standard employee onboarding process",
                IsActive = true,
                Items = new List<OnboardingTemplateItem>
                {
                    new OnboardingTemplateItem
                    {
                        Title = "Complete HR paperwork",
                        Description = "Fill out all required forms",
                        DaysDue = 0,
                        DefaultAssigneeRole = "HR"
                    }
                }
            };
            context.OnboardingTemplates.Add(template);
            await context.SaveChangesAsync();
            templateId = template.Id;
        }

        var request = new StartOnboardingRequest
        {
            StartDate = DateTime.UtcNow.AddDays(7),
            TemplateId = templateId
        };

        // Act
        var response = await Client.PostAsJsonSnakeCaseAsync($"/lifecycle/v1/employees/{employeeId}/onboarding/start", request);

        // Assert
        Assert.Equal(HttpStatusCode.Created, response.StatusCode);
        var result = await response.Content.ReadFromJsonSnakeCaseAsync<OnboardingCreatedResponse>();
        Assert.NotNull(result);
        Assert.NotEqual(Guid.Empty, result.Id);
    }

    [Fact]
    public async Task GetStatus_ExistingOnboarding_ReturnsOk()
    {
        // Arrange
        var employeeId = Guid.NewGuid();

        using (var scope = ServiceProvider.CreateScope())
        {
            var context = scope.ServiceProvider.GetRequiredService<LifecycleDbContext>();
            await context.Database.EnsureCreatedAsync();

            var checklist = new OnboardingChecklist
            {
                Id = Guid.NewGuid(),
                EmployeeId = employeeId,
                StartDate = DateTime.UtcNow,
                Status = Domain.Enums.OnboardingStatus.InProgress,
                Items = new List<OnboardingItem>
                {
                    new OnboardingItem
                    {
                        Title = "Setup workspace",
                        Description = "Configure desk and equipment",
                        DaysDue = 1,
                        IsCompleted = false
                    }
                }
            };
            context.OnboardingChecklists.Add(checklist);
            await context.SaveChangesAsync();
        }

        // Act
        var response = await Client.GetAsync($"/lifecycle/v1/employees/{employeeId}/onboarding/status");

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        var result = await response.Content.ReadFromJsonSnakeCaseAsync<OnboardingStatusDto>();
        Assert.NotNull(result);
        Assert.Equal(employeeId, result.EmployeeId);
    }

    [Fact]
    public async Task GetStatus_NonExistingOnboarding_ReturnsNotFound()
    {
        // Arrange
        var employeeId = Guid.NewGuid();

        // Act
        var response = await Client.GetAsync($"/lifecycle/v1/employees/{employeeId}/onboarding/status");

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

            var checklist = new OnboardingChecklist
            {
                Id = Guid.NewGuid(),
                EmployeeId = Guid.NewGuid(),
                StartDate = DateTime.UtcNow,
                Status = Domain.Enums.OnboardingStatus.InProgress,
                Items = new List<OnboardingItem>()
            };
            context.OnboardingChecklists.Add(checklist);
            await context.SaveChangesAsync();
        }

        // Act
        var response = await Client.GetAsync("/lifecycle/v1/onboarding/pending");

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        var result = await response.Content.ReadFromJsonSnakeCaseAsync<List<OnboardingStatusDto>>();
        Assert.NotNull(result);
        Assert.NotEmpty(result);
    }

    [Fact]
    public async Task CompleteItem_ValidRequest_ReturnsNoContent()
    {
        // Arrange
        Guid itemId;

        using (var scope = ServiceProvider.CreateScope())
        {
            var context = scope.ServiceProvider.GetRequiredService<LifecycleDbContext>();
            await context.Database.EnsureCreatedAsync();

            var checklist = new OnboardingChecklist
            {
                Id = Guid.NewGuid(),
                EmployeeId = Guid.NewGuid(),
                StartDate = DateTime.UtcNow,
                Status = Domain.Enums.OnboardingStatus.InProgress,
                Items = new List<OnboardingItem>
                {
                    new OnboardingItem
                    {
                        Id = Guid.NewGuid(),
                        Title = "Task to complete",
                        Description = "Test task",
                        DaysDue = 1,
                        IsCompleted = false
                    }
                }
            };
            context.OnboardingChecklists.Add(checklist);
            await context.SaveChangesAsync();
            itemId = checklist.Items.First().Id;
        }

        var request = new CompleteItemRequest
        {
            Notes = "Task completed successfully"
        };

        // Act
        var response = await Client.PutAsJsonSnakeCaseAsync($"/lifecycle/v1/onboarding-items/{itemId}/complete", request);

        // Assert
        Assert.Equal(HttpStatusCode.NoContent, response.StatusCode);
    }

    [Fact]
    public async Task ReassignItem_ValidRequest_ReturnsNoContent()
    {
        // Arrange
        Guid itemId;

        using (var scope = ServiceProvider.CreateScope())
        {
            var context = scope.ServiceProvider.GetRequiredService<LifecycleDbContext>();
            await context.Database.EnsureCreatedAsync();

            var checklist = new OnboardingChecklist
            {
                Id = Guid.NewGuid(),
                EmployeeId = Guid.NewGuid(),
                StartDate = DateTime.UtcNow,
                Status = Domain.Enums.OnboardingStatus.InProgress,
                Items = new List<OnboardingItem>
                {
                    new OnboardingItem
                    {
                        Id = Guid.NewGuid(),
                        Title = "Task to reassign",
                        Description = "Test task",
                        DaysDue = 1,
                        IsCompleted = false,
                        AssignedTo = Guid.NewGuid()
                    }
                }
            };
            context.OnboardingChecklists.Add(checklist);
            await context.SaveChangesAsync();
            itemId = checklist.Items.First().Id;
        }

        var request = new ReassignItemRequest
        {
            AssignedTo = Guid.NewGuid()
        };

        // Act
        var response = await Client.PutAsJsonSnakeCaseAsync($"/lifecycle/v1/onboarding-items/{itemId}/reassign", request);

        // Assert
        Assert.Equal(HttpStatusCode.NoContent, response.StatusCode);
    }
}

public record OnboardingCreatedResponse(Guid Id);
public record StartOnboardingRequest { public DateTime StartDate { get; init; } public Guid TemplateId { get; init; } }
public record CompleteItemRequest { public string? Notes { get; init; } }
public record ReassignItemRequest { public Guid AssignedTo { get; init; } }
