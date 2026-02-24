using Maliev.LifecycleService.Application.Commands.Onboarding;
using Maliev.LifecycleService.Application.Commands.Onboarding.Handlers;
using Maliev.LifecycleService.Application.Interfaces;
using Maliev.LifecycleService.Domain.Entities;
using Maliev.LifecycleService.Domain.Enums;
using Maliev.MessagingContracts.Generated;
using Maliev.MessagingContracts.Contracts.Lifecycle;
using Moq;
using Xunit;

namespace Maliev.LifecycleService.Tests.Unit.Handlers;

public class StartOnboardingCommandHandlerTests
{
    private readonly Mock<IOnboardingRepository> _onboardingRepositoryMock;
    private readonly Mock<ITemplateRepository> _templateRepositoryMock;
    private readonly Mock<IAuditLogService> _auditLogServiceMock;
    private readonly Mock<IEventPublisher> _eventPublisherMock;
    private readonly Mock<ILifecycleMetrics> _metricsMock;
    private readonly StartOnboardingCommandHandler _handler;

    public StartOnboardingCommandHandlerTests()
    {
        _onboardingRepositoryMock = new Mock<IOnboardingRepository>();
        _templateRepositoryMock = new Mock<ITemplateRepository>();
        _auditLogServiceMock = new Mock<IAuditLogService>();
        _eventPublisherMock = new Mock<IEventPublisher>();
        _metricsMock = new Mock<ILifecycleMetrics>();

        _handler = new StartOnboardingCommandHandler(
            _onboardingRepositoryMock.Object,
            _templateRepositoryMock.Object,
            _auditLogServiceMock.Object,
            _eventPublisherMock.Object,
            _metricsMock.Object
        );
    }

    [Fact]
    public async Task HandleAsync_ValidCommand_CreatesOnboardingChecklist()
    {
        // Arrange
        var employeeId = Guid.NewGuid();
        var templateId = Guid.NewGuid();
        var userId = Guid.NewGuid();

        var template = new OnboardingTemplate
        {
            Id = templateId,
            Name = "Standard Onboarding",
            IsActive = true,
            Items = new List<OnboardingTemplateItem>
            {
                new OnboardingTemplateItem
                {
                    Id = Guid.NewGuid(),
                    Title = "Setup workspace",
                    Description = "Configure desk and equipment",
                    Category = ItemCategory.ITSetup,
                    DaysDue = 0,
                    SortOrder = 1
                },
                new OnboardingTemplateItem
                {
                    Id = Guid.NewGuid(),
                    Title = "HR orientation",
                    Description = "Complete HR paperwork",
                    Category = ItemCategory.Documentation,
                    DaysDue = 1,
                    SortOrder = 2
                }
            }
        };

        var command = new StartOnboardingCommand(employeeId, DateTime.UtcNow.AddDays(7), templateId, userId);

        _onboardingRepositoryMock.Setup(x => x.GetByEmployeeIdAsync(employeeId, It.IsAny<CancellationToken>()))
            .ReturnsAsync((OnboardingChecklist?)null);

        _templateRepositoryMock.Setup(x => x.GetByIdWithItemsAsync(templateId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(template);

        // Act
        var result = await _handler.HandleAsync(command);

        // Assert
        Assert.NotEqual(Guid.Empty, result);

        _onboardingRepositoryMock.Verify(x => x.AddAsync(
            It.Is<OnboardingChecklist>(c =>
                c.EmployeeId == employeeId &&
                c.Items.Count == 2 &&
                c.TotalItems == 2
            ),
            It.IsAny<CancellationToken>()), Times.Once);

        _metricsMock.Verify(x => x.RecordOnboardingStarted(), Times.Once);

        _eventPublisherMock.Verify(x => x.PublishAsync(
            It.IsAny<OnboardingStartedEvent>(),
            It.IsAny<CancellationToken>()), Times.Once);

        _auditLogServiceMock.Verify(x => x.LogAsync(
            "OnboardingChecklist",
            It.IsAny<Guid>(),
            "ManuallyStarted",
            userId,
            null,
            It.IsAny<OnboardingChecklist>(),
            It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task HandleAsync_OnboardingAlreadyStarted_ThrowsException()
    {
        // Arrange
        var employeeId = Guid.NewGuid();
        var userId = Guid.NewGuid();
        var command = new StartOnboardingCommand(employeeId, DateTime.UtcNow.AddDays(7), null, userId);

        var existingChecklist = new OnboardingChecklist
        {
            Id = Guid.NewGuid(),
            EmployeeId = employeeId,
            StartDate = DateTime.UtcNow,
            Status = OnboardingStatus.InProgress
        };

        _onboardingRepositoryMock.Setup(x => x.GetByEmployeeIdAsync(employeeId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(existingChecklist);

        // Act & Assert
        var exception = await Assert.ThrowsAsync<Exception>(() => _handler.HandleAsync(command));
        Assert.Contains("already started", exception.Message);

        _onboardingRepositoryMock.Verify(x => x.AddAsync(It.IsAny<OnboardingChecklist>(), It.IsAny<CancellationToken>()), Times.Never);
    }

    [Fact]
    public async Task HandleAsync_NoTemplateFound_ThrowsException()
    {
        // Arrange
        var employeeId = Guid.NewGuid();
        var templateId = Guid.NewGuid();
        var userId = Guid.NewGuid();
        var command = new StartOnboardingCommand(employeeId, DateTime.UtcNow.AddDays(7), templateId, userId);

        _onboardingRepositoryMock.Setup(x => x.GetByEmployeeIdAsync(employeeId, It.IsAny<CancellationToken>()))
            .ReturnsAsync((OnboardingChecklist?)null);

        _templateRepositoryMock.Setup(x => x.GetByIdWithItemsAsync(templateId, It.IsAny<CancellationToken>()))
            .ReturnsAsync((OnboardingTemplate?)null);

        // Act & Assert
        var exception = await Assert.ThrowsAsync<Exception>(() => _handler.HandleAsync(command));
        Assert.Contains("No onboarding template found", exception.Message);

        _onboardingRepositoryMock.Verify(x => x.AddAsync(It.IsAny<OnboardingChecklist>(), It.IsAny<CancellationToken>()), Times.Never);
    }

    [Fact]
    public async Task HandleAsync_NoTemplateSpecified_UsesDefaultTemplate()
    {
        // Arrange
        var employeeId = Guid.NewGuid();
        var userId = Guid.NewGuid();
        var command = new StartOnboardingCommand(employeeId, DateTime.UtcNow.AddDays(7), null, userId);

        var defaultTemplate = new OnboardingTemplate
        {
            Id = Guid.NewGuid(),
            Name = "Default Template",
            IsActive = true,
            Items = new List<OnboardingTemplateItem>()
        };

        _onboardingRepositoryMock.Setup(x => x.GetByEmployeeIdAsync(employeeId, It.IsAny<CancellationToken>()))
            .ReturnsAsync((OnboardingChecklist?)null);

        _templateRepositoryMock.Setup(x => x.GetByDepartmentIdAsync(null, It.IsAny<CancellationToken>()))
            .ReturnsAsync(defaultTemplate);

        // Act
        var result = await _handler.HandleAsync(command);

        // Assert
        Assert.NotEqual(Guid.Empty, result);

        _templateRepositoryMock.Verify(x => x.GetByDepartmentIdAsync(null, It.IsAny<CancellationToken>()), Times.Once);
        _onboardingRepositoryMock.Verify(x => x.AddAsync(It.IsAny<OnboardingChecklist>(), It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task HandleAsync_TemplateWithMultipleItems_CreatesItemsInCorrectOrder()
    {
        // Arrange
        var employeeId = Guid.NewGuid();
        var templateId = Guid.NewGuid();
        var userId = Guid.NewGuid();

        var template = new OnboardingTemplate
        {
            Id = templateId,
            Name = "Multi-Item Template",
            IsActive = true,
            Items = new List<OnboardingTemplateItem>
            {
                new OnboardingTemplateItem { Title = "Third", SortOrder = 3, Category = ItemCategory.Training, DaysDue = 2 },
                new OnboardingTemplateItem { Title = "First", SortOrder = 1, Category = ItemCategory.ITSetup, DaysDue = 0 },
                new OnboardingTemplateItem { Title = "Second", SortOrder = 2, Category = ItemCategory.Documentation, DaysDue = 1 }
            }
        };

        var command = new StartOnboardingCommand(employeeId, DateTime.UtcNow.AddDays(7), templateId, userId);

        _onboardingRepositoryMock.Setup(x => x.GetByEmployeeIdAsync(employeeId, It.IsAny<CancellationToken>()))
            .ReturnsAsync((OnboardingChecklist?)null);

        _templateRepositoryMock.Setup(x => x.GetByIdWithItemsAsync(templateId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(template);

        OnboardingChecklist? capturedChecklist = null;
        _onboardingRepositoryMock.Setup(x => x.AddAsync(It.IsAny<OnboardingChecklist>(), It.IsAny<CancellationToken>()))
            .Callback<OnboardingChecklist, CancellationToken>((c, _) => capturedChecklist = c)
            .Returns(Task.CompletedTask);

        // Act
        await _handler.HandleAsync(command);

        // Assert
        Assert.NotNull(capturedChecklist);
        Assert.Equal(3, capturedChecklist.Items.Count);

        var itemsList = capturedChecklist.Items.ToList();
        Assert.Equal("First", itemsList[0].Title);
        Assert.Equal("Second", itemsList[1].Title);
        Assert.Equal("Third", itemsList[2].Title);
    }
}
