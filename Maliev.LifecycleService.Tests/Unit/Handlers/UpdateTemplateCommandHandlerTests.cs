using Maliev.LifecycleService.Application.Commands.Templates;
using Maliev.LifecycleService.Application.Commands.Templates.Handlers;
using Maliev.LifecycleService.Application.Interfaces;
using Maliev.LifecycleService.Domain.Entities;
using Maliev.LifecycleService.Domain.Enums;
using Moq;
using Xunit;

namespace Maliev.LifecycleService.Tests.Unit.Handlers;

public class UpdateTemplateCommandHandlerTests
{
    private readonly Mock<ITemplateRepository> _repositoryMock;
    private readonly Mock<ITemplateCacheService> _cacheMock;
    private readonly Mock<IAuditLogService> _auditLogServiceMock;
    private readonly UpdateTemplateCommandHandler _handler;

    public UpdateTemplateCommandHandlerTests()
    {
        _repositoryMock = new Mock<ITemplateRepository>();
        _cacheMock = new Mock<ITemplateCacheService>();
        _auditLogServiceMock = new Mock<IAuditLogService>();
        _handler = new UpdateTemplateCommandHandler(_repositoryMock.Object, _cacheMock.Object, _auditLogServiceMock.Object);
    }

    [Fact]
    public async Task HandleAsync_WhenTemplateExists_ShouldUpdateAndInvalidateCache()
    {
        // Arrange
        var templateId = Guid.NewGuid();
        var departmentId = Guid.NewGuid();
        var existingTemplate = new OnboardingTemplate
        {
            Id = templateId,
            Name = "Old Name",
            DepartmentId = departmentId,
            Items = new List<OnboardingTemplateItem>()
        };

        var command = new UpdateTemplateCommand(
            templateId,
            "New Name",
            "New Description",
            departmentId,
            true,
            new List<UpdateTemplateItemDto>
            {
                new UpdateTemplateItemDto(null, "New Item", "Desc", ItemCategory.ITSetup, "Admin", 1, 1)
            },
            Guid.NewGuid()
        );

        _repositoryMock.Setup(r => r.GetByIdWithItemsAsync(templateId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(existingTemplate);

        // Act
        await _handler.HandleAsync(command);

        // Assert
        Assert.Equal("New Name", existingTemplate.Name);
        Assert.Single(existingTemplate.Items);
        _repositoryMock.Verify(r => r.UpdateAsync(existingTemplate, It.IsAny<CancellationToken>()), Times.Once);
        _cacheMock.Verify(c => c.RemoveAsync(departmentId, It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task HandleAsync_WhenTemplateDoesNotExist_ShouldThrow()
    {
        // Arrange
        var templateId = Guid.NewGuid();
        var command = new UpdateTemplateCommand(templateId, "Name", null, null, true, new List<UpdateTemplateItemDto>(), Guid.NewGuid());
        _repositoryMock.Setup(r => r.GetByIdWithItemsAsync(templateId, It.IsAny<CancellationToken>()))
            .ReturnsAsync((OnboardingTemplate?)null);

        // Act & Assert
        await Assert.ThrowsAsync<Exception>(() => _handler.HandleAsync(command));
    }
}
