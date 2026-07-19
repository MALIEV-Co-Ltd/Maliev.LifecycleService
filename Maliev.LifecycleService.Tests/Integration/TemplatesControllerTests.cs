using System.Net;
using Maliev.LifecycleService.Application.Commands.Templates;
using Maliev.LifecycleService.Application.DTOs;
using Maliev.LifecycleService.Domain.Entities;
using Maliev.LifecycleService.Domain.Enums;
using Maliev.LifecycleService.Infrastructure.Data;
using Microsoft.Extensions.DependencyInjection;
using Xunit;

namespace Maliev.LifecycleService.Tests.Integration;

[Collection("IntegrationTests")]
public class TemplatesControllerTests : BaseIntegrationTest
{
    public TemplatesControllerTests(TestUtilities.LifecycleTestWebApplicationFactory factory) : base(factory)
    {
    }

    [Fact]
    public async Task CreateTemplate_ValidRequest_ReturnsCreated()
    {
        // Arrange
        var request = new CreateTemplateCommand(
            Name: "Engineering Onboarding",
            Description: "Standard onboarding for engineering team",
            DepartmentId: null,
            Items: new List<CreateTemplateItemDto>
            {
                new CreateTemplateItemDto(
                    Title: "Setup development environment",
                    Description: "Install IDE, tools, and dependencies",
                    Category: ItemCategory.ITSetup,
                    DefaultAssigneeRole: "Engineering Manager",
                    DaysDue: 0,
                    SortOrder: 0
                )
            },
            UserId: Guid.Empty
        );

        // Act
        var response = await Client.PostAsJsonSnakeCaseAsync("/lifecycle/v1/templates", request);

        // Assert
        Assert.Equal(HttpStatusCode.Created, response.StatusCode);
        var result = await response.Content.ReadFromJsonSnakeCaseAsync<TemplateCreatedResponse>();
        Assert.NotNull(result);
        Assert.NotEqual(Guid.Empty, result.Id);
    }

    [Fact]
    public async Task GetTemplate_ExistingTemplate_ReturnsOk()
    {
        // Arrange
        Guid templateId;

        using (var scope = ServiceProvider.CreateScope())
        {
            var context = scope.ServiceProvider.GetRequiredService<LifecycleDbContext>();
            await context.Database.EnsureCreatedAsync();

            var template = new OnboardingTemplate
            {
                Id = Guid.NewGuid(),
                Name = "Sales Onboarding",
                Description = "Standard onboarding for sales team",
                IsActive = true,
                Items = new List<OnboardingTemplateItem>
                {
                    new OnboardingTemplateItem
                    {
                        Title = "CRM training",
                        Description = "Learn Salesforce basics",
                        DaysDue = 1,
                        DefaultAssigneeRole = "Sales Manager"
                    }
                }
            };
            context.OnboardingTemplates.Add(template);
            await context.SaveChangesAsync();
            templateId = template.Id;
        }

        // Act
        var response = await Client.GetAsync($"/lifecycle/v1/templates/{templateId}");

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        var result = await response.Content.ReadFromJsonSnakeCaseAsync<TemplateResponse>();
        Assert.NotNull(result);
        Assert.Equal(templateId, result.Id);
        Assert.Equal("Sales Onboarding", result.Name);
    }

    [Fact]
    public async Task GetTemplate_NonExistingTemplate_ReturnsNotFound()
    {
        // Arrange
        var templateId = Guid.NewGuid();

        // Act
        var response = await Client.GetAsync($"/lifecycle/v1/templates/{templateId}");

        // Assert
        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    [Fact]
    public async Task ListTemplates_ReturnsOkWithList()
    {
        // Arrange
        using (var scope = ServiceProvider.CreateScope())
        {
            var context = scope.ServiceProvider.GetRequiredService<LifecycleDbContext>();
            await context.Database.EnsureCreatedAsync();

            var template = new OnboardingTemplate
            {
                Id = Guid.NewGuid(),
                Name = "Marketing Onboarding",
                Description = "Standard onboarding for marketing team",
                IsActive = true,
                Items = new List<OnboardingTemplateItem>()
            };
            context.OnboardingTemplates.Add(template);
            await context.SaveChangesAsync();
        }

        // Act
        var response = await Client.GetAsync("/lifecycle/v1/templates");

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        var result = await response.Content.ReadFromJsonSnakeCaseAsync<List<TemplateResponse>>();
        Assert.NotNull(result);
        Assert.NotEmpty(result);
    }

    [Fact]
    public async Task UpdateTemplate_ValidRequest_ReturnsNoContent()
    {
        // Arrange
        Guid templateId;

        using (var scope = ServiceProvider.CreateScope())
        {
            var context = scope.ServiceProvider.GetRequiredService<LifecycleDbContext>();
            await context.Database.EnsureCreatedAsync();

            var template = new OnboardingTemplate
            {
                Id = Guid.NewGuid(),
                Name = "Original Template",
                Description = "Original description",
                IsActive = true,
                Items = new List<OnboardingTemplateItem>()
            };
            context.OnboardingTemplates.Add(template);
            await context.SaveChangesAsync();
            templateId = template.Id;
        }

        var request = new UpdateTemplateCommand(
            Id: templateId,
            Name: "Updated Template",
            Description: "Updated description",
            DepartmentId: null,
            IsActive: true,
            Items: new List<UpdateTemplateItemDto>(),
            UserId: Guid.Empty
        );

        // Act
        var response = await Client.PutAsJsonSnakeCaseAsync($"/lifecycle/v1/templates/{templateId}", request);

        // Assert
        Assert.Equal(HttpStatusCode.NoContent, response.StatusCode);
    }

    [Fact]
    public async Task UpdateTemplate_IdMismatch_ReturnsBadRequest()
    {
        // Arrange
        var urlId = Guid.NewGuid();
        var bodyId = Guid.NewGuid();

        var request = new UpdateTemplateCommand(
            Id: bodyId,
            Name: "Test Template",
            Description: "Test",
            DepartmentId: null,
            IsActive: true,
            Items: new List<UpdateTemplateItemDto>(),
            UserId: Guid.Empty
        );

        // Act
        var response = await Client.PutAsJsonSnakeCaseAsync($"/lifecycle/v1/templates/{urlId}", request);

        // Assert
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }

    [Fact]
    public async Task DeleteTemplate_ExistingTemplate_ReturnsNoContent()
    {
        // Arrange
        Guid templateId;

        using (var scope = ServiceProvider.CreateScope())
        {
            var context = scope.ServiceProvider.GetRequiredService<LifecycleDbContext>();
            await context.Database.EnsureCreatedAsync();

            var template = new OnboardingTemplate
            {
                Id = Guid.NewGuid(),
                Name = "Template to Delete",
                Description = "Will be deleted",
                IsActive = true,
                Items = new List<OnboardingTemplateItem>()
            };
            context.OnboardingTemplates.Add(template);
            await context.SaveChangesAsync();
            templateId = template.Id;
        }

        // Act
        var response = await Client.DeleteAsync($"/lifecycle/v1/templates/{templateId}");

        // Assert
        Assert.Equal(HttpStatusCode.NoContent, response.StatusCode);
    }
}

public record TemplateCreatedResponse(Guid Id);
public record TemplateResponse(Guid Id, string Name, string? Description, Guid? DepartmentId, bool IsActive, List<Application.DTOs.TemplateItemDto> Items);
