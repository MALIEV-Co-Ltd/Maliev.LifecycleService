using Maliev.LifecycleService.Application.Commands.Templates;
using Maliev.LifecycleService.Application.Commands.Templates.Handlers;
using Maliev.LifecycleService.Application.Interfaces;
using Maliev.LifecycleService.Domain.Entities;
using Maliev.LifecycleService.Domain.Enums;
using Moq;
using Xunit;

namespace Maliev.LifecycleService.Tests.Unit.Handlers;

public class CreateTemplateCommandHandlerTests
{
    private readonly Mock<ITemplateRepository> _repositoryMock;
    private readonly Mock<ITemplateCacheService> _cacheMock;
    private readonly Mock<IAuditLogService> _auditLogServiceMock;
    private readonly CreateTemplateCommandHandler _handler;

    public CreateTemplateCommandHandlerTests()
    {
        _repositoryMock = new Mock<ITemplateRepository>();
        _cacheMock = new Mock<ITemplateCacheService>();
        _auditLogServiceMock = new Mock<IAuditLogService>();
        _handler = new CreateTemplateCommandHandler(_repositoryMock.Object, _cacheMock.Object, _auditLogServiceMock.Object);
    }

    [Fact]
    public async Task HandleAsync_ValidCommand_CreatesTemplate()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var departmentId = Guid.NewGuid();

        var command = new CreateTemplateCommand(
            "Engineering Onboarding",
            "Onboarding for engineering team",
            departmentId,
            new List<CreateTemplateItemDto>
            {
                new CreateTemplateItemDto("Setup laptop", "Configure development environment", ItemCategory.ITSetup, "IT", 0, 1),
                new CreateTemplateItemDto("Security training", "Complete security awareness training", ItemCategory.Training, "Security", 1, 2)
            },
            userId
        );

        _repositoryMock.Setup(r => r.AddAsync(It.IsAny<OnboardingTemplate>(), It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);

        // Act
        var result = await _handler.HandleAsync(command);

        // Assert
        Assert.NotEqual(Guid.Empty, result);

        _repositoryMock.Verify(r => r.AddAsync(
            It.Is<OnboardingTemplate>(t =>
                t.Name == "Engineering Onboarding" &&
                t.Description == "Onboarding for engineering team" &&
                t.DepartmentId == departmentId &&
                t.Items.Count == 2
            ),
            It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task HandleAsync_EmptyItems_CreatesTemplateWithNoItems()
    {
        // Arrange
        var userId = Guid.NewGuid();

        var command = new CreateTemplateCommand(
            "Minimal Template",
            "Template with no items",
            null,
            new List<CreateTemplateItemDto>(),
            userId
        );

        _repositoryMock.Setup(r => r.AddAsync(It.IsAny<OnboardingTemplate>(), It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);

        // Act
        var result = await _handler.HandleAsync(command);

        // Assert
        Assert.NotEqual(Guid.Empty, result);

        _repositoryMock.Verify(r => r.AddAsync(
            It.Is<OnboardingTemplate>(t => t.Items.Count == 0),
            It.IsAny<CancellationToken>()), Times.Once);
    }
}

public class DeleteTemplateCommandHandlerTests
{
    private readonly Mock<ITemplateRepository> _repositoryMock;
    private readonly Mock<ITemplateCacheService> _cacheMock;
    private readonly Mock<IAuditLogService> _auditLogServiceMock;
    private readonly DeleteTemplateCommandHandler _handler;

    public DeleteTemplateCommandHandlerTests()
    {
        _repositoryMock = new Mock<ITemplateRepository>();
        _cacheMock = new Mock<ITemplateCacheService>();
        _auditLogServiceMock = new Mock<IAuditLogService>();
        _handler = new DeleteTemplateCommandHandler(_repositoryMock.Object, _cacheMock.Object, _auditLogServiceMock.Object);
    }

    [Fact]
    public async Task HandleAsync_TemplateExists_SoftDeletesAndInvalidatesCache()
    {
        // Arrange
        var templateId = Guid.NewGuid();
        var departmentId = Guid.NewGuid();
        var userId = Guid.NewGuid();

        var template = new OnboardingTemplate
        {
            Id = templateId,
            Name = "Template to delete",
            DepartmentId = departmentId,
            IsActive = true
        };

        var command = new DeleteTemplateCommand(templateId, userId);

        _repositoryMock.Setup(r => r.GetByIdWithItemsAsync(templateId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(template);

        // Act
        await _handler.HandleAsync(command);

        // Assert
        Assert.False(template.IsActive);
        _repositoryMock.Verify(r => r.UpdateAsync(template, It.IsAny<CancellationToken>()), Times.Once);
        _cacheMock.Verify(c => c.RemoveAsync(departmentId, It.IsAny<CancellationToken>()), Times.Once);
        _auditLogServiceMock.Verify(a => a.LogAsync("OnboardingTemplate", templateId, "SoftDeleted", userId, null, null, It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task HandleAsync_TemplateNotFound_DoesNothing()
    {
        // Arrange
        var templateId = Guid.NewGuid();
        var command = new DeleteTemplateCommand(templateId, Guid.NewGuid());

        _repositoryMock.Setup(r => r.GetByIdWithItemsAsync(templateId, It.IsAny<CancellationToken>()))
            .ReturnsAsync((OnboardingTemplate?)null);

        // Act
        await _handler.HandleAsync(command);

        // Assert
        _repositoryMock.Verify(r => r.UpdateAsync(It.IsAny<OnboardingTemplate>(), It.IsAny<CancellationToken>()), Times.Never);
        _cacheMock.Verify(c => c.RemoveAsync(It.IsAny<Guid?>(), It.IsAny<CancellationToken>()), Times.Never);
    }
}
