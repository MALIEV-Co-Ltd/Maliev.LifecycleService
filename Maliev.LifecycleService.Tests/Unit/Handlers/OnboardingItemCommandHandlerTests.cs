using Maliev.LifecycleService.Application.Commands.Onboarding;
using Maliev.LifecycleService.Application.Commands.Onboarding.Handlers;
using Maliev.LifecycleService.Application.Interfaces;
using Maliev.LifecycleService.Domain.Entities;
using Maliev.LifecycleService.Domain.Enums;
using Maliev.LifecycleService.Domain.Exceptions;
using Maliev.MessagingContracts;
using Maliev.MessagingContracts.Contracts.Lifecycle;
using Moq;
using Xunit;

namespace Maliev.LifecycleService.Tests.Unit.Handlers;

public class CompleteOnboardingItemCommandHandlerTests
{
    private readonly Mock<IOnboardingRepository> _repositoryMock;
    private readonly Mock<IAuditLogService> _auditLogServiceMock;
    private readonly Mock<IEventPublisher> _eventPublisherMock;
    private readonly Mock<ILifecycleMetrics> _metricsMock;
    private readonly CompleteOnboardingItemCommandHandler _handler;

    public CompleteOnboardingItemCommandHandlerTests()
    {
        _repositoryMock = new Mock<IOnboardingRepository>();
        _auditLogServiceMock = new Mock<IAuditLogService>();
        _eventPublisherMock = new Mock<IEventPublisher>();
        _metricsMock = new Mock<ILifecycleMetrics>();
        _handler = new CompleteOnboardingItemCommandHandler(
            _repositoryMock.Object,
            _auditLogServiceMock.Object,
            _eventPublisherMock.Object,
            _metricsMock.Object
        );
    }

    [Fact]
    public async Task HandleAsync_ValidItem_CompletesItemAndUpdatesMetrics()
    {
        // Arrange
        var itemId = Guid.NewGuid();
        var userId = Guid.NewGuid();
        var notes = "Completed successfully";

        var checklist = new OnboardingChecklist
        {
            Id = Guid.NewGuid(),
            EmployeeId = Guid.NewGuid(),
            TotalItems = 5,
            CompletedItems = 2,
            Status = OnboardingStatus.InProgress,
            Items = new List<OnboardingItem>
            {
                new OnboardingItem
                {
                    Id = itemId,
                    Title = "Task to complete",
                    IsCompleted = false,
                    OnboardingChecklistId = Guid.NewGuid(),
                    Checklist = null!
                }
            }
        };

        var item = checklist.Items.First();
        item.Checklist = checklist;

        var command = new CompleteOnboardingItemCommand(itemId, userId, notes);

        _repositoryMock.Setup(r => r.GetItemByIdAsync(itemId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(item);

        _repositoryMock.Setup(r => r.UpdateAsync(checklist, It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);

        // Act
        await _handler.HandleAsync(command);

        // Assert
        Assert.True(item.IsCompleted);
        Assert.Equal(notes, item.Notes);
        Assert.NotNull(item.CompletedDate);
        Assert.Equal(3, checklist.CompletedItems);
        
        _eventPublisherMock.Verify(e => e.PublishAsync(
            It.IsAny<LifecycleTaskCompletedEvent>(),
            It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task HandleAsync_AllItemsComplete_UpdatesChecklistStatusAndPublishesEvent()
    {
        // Arrange
        var itemId = Guid.NewGuid();
        var userId = Guid.NewGuid();

        var checklist = new OnboardingChecklist
        {
            Id = Guid.NewGuid(),
            EmployeeId = Guid.NewGuid(),
            TotalItems = 2,
            CompletedItems = 1,
            Status = OnboardingStatus.InProgress,
            CreatedDate = DateTime.UtcNow.AddDays(-5),
            Items = new List<OnboardingItem>
            {
                new OnboardingItem { Id = itemId, IsCompleted = false, Checklist = null! },
                new OnboardingItem { Id = Guid.NewGuid(), IsCompleted = true, Checklist = null! }
            }
        };

        var item = checklist.Items.First();
        item.Checklist = checklist;

        var command = new CompleteOnboardingItemCommand(itemId, userId, null);

        _repositoryMock.Setup(r => r.GetItemByIdAsync(itemId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(item);

        _repositoryMock.Setup(r => r.UpdateAsync(checklist, It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);

        // Act
        await _handler.HandleAsync(command);

        // Assert
        Assert.Equal(OnboardingStatus.Completed, checklist.Status);
        Assert.NotNull(checklist.CompletedDate);
        
        _metricsMock.Verify(m => m.RecordOnboardingCompleted(), Times.Once);
        _metricsMock.Verify(m => m.RecordOnboardingDuration(It.IsAny<double>()), Times.Once);
        
        _eventPublisherMock.Verify(e => e.PublishAsync(
            It.IsAny<OnboardingCompletedEvent>(),
            It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task HandleAsync_ItemNotFound_ThrowsException()
    {
        // Arrange
        var itemId = Guid.NewGuid();
        var command = new CompleteOnboardingItemCommand(itemId, Guid.NewGuid(), null);

        _repositoryMock.Setup(r => r.GetItemByIdAsync(itemId, It.IsAny<CancellationToken>()))
            .ReturnsAsync((OnboardingItem?)null);

        // Act & Assert
        await Assert.ThrowsAsync<Exception>(() => _handler.HandleAsync(command));
    }

    [Fact]
    public async Task HandleAsync_AlreadyCompleted_ThrowsItemAlreadyCompletedException()
    {
        // Arrange
        var itemId = Guid.NewGuid();
        var userId = Guid.NewGuid();

        var item = new OnboardingItem 
        { 
            Id = itemId, 
            IsCompleted = true,
            Checklist = new OnboardingChecklist()
        };

        var command = new CompleteOnboardingItemCommand(itemId, userId, null);

        _repositoryMock.Setup(r => r.GetItemByIdAsync(itemId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(item);

        // Act & Assert
        await Assert.ThrowsAsync<ItemAlreadyCompletedException>(() => _handler.HandleAsync(command));
    }

    [Fact]
    public async Task HandleAsync_NotStartedStatus_TransitionsToInProgress()
    {
        // Arrange
        var itemId = Guid.NewGuid();
        var userId = Guid.NewGuid();

        var checklist = new OnboardingChecklist
        {
            Id = Guid.NewGuid(),
            Status = OnboardingStatus.NotStarted,
            TotalItems = 2,
            CompletedItems = 0,
            Items = new List<OnboardingItem>
            {
                new OnboardingItem { Id = itemId, IsCompleted = false, Checklist = null! }
            }
        };

        var item = checklist.Items.First();
        item.Checklist = checklist;

        var command = new CompleteOnboardingItemCommand(itemId, userId, null);

        _repositoryMock.Setup(r => r.GetItemByIdAsync(itemId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(item);

        _repositoryMock.Setup(r => r.UpdateAsync(checklist, It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);

        // Act
        await _handler.HandleAsync(command);

        // Assert
        Assert.Equal(OnboardingStatus.InProgress, checklist.Status);
    }
}

public class ReassignOnboardingItemCommandHandlerTests
{
    private readonly Mock<IOnboardingRepository> _repositoryMock;
    private readonly Mock<IAuditLogService> _auditLogServiceMock;
    private readonly ReassignOnboardingItemCommandHandler _handler;

    public ReassignOnboardingItemCommandHandlerTests()
    {
        _repositoryMock = new Mock<IOnboardingRepository>();
        _auditLogServiceMock = new Mock<IAuditLogService>();
        _handler = new ReassignOnboardingItemCommandHandler(_repositoryMock.Object, _auditLogServiceMock.Object);
    }

    [Fact]
    public async Task HandleAsync_ValidItem_ReassignsItem()
    {
        // Arrange
        var itemId = Guid.NewGuid();
        var newAssignee = Guid.NewGuid();
        var userId = Guid.NewGuid();

        var checklist = new OnboardingChecklist
        {
            Id = Guid.NewGuid(),
            Items = new List<OnboardingItem>
            {
                new OnboardingItem
                {
                    Id = itemId,
                    Title = "Task to reassign",
                    AssignedTo = Guid.NewGuid(),
                    Checklist = null!
                }
            }
        };

        var item = checklist.Items.First();
        item.Checklist = checklist;

        var command = new ReassignOnboardingItemCommand(itemId, newAssignee, userId);

        _repositoryMock.Setup(r => r.GetItemByIdAsync(itemId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(item);

        _repositoryMock.Setup(r => r.UpdateAsync(checklist, It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);

        // Act
        await _handler.HandleAsync(command);

        // Assert
        Assert.Equal(newAssignee, item.AssignedTo);
        _repositoryMock.Verify(r => r.UpdateAsync(checklist, It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task HandleAsync_ItemNotFound_ThrowsException()
    {
        // Arrange
        var itemId = Guid.NewGuid();
        var command = new ReassignOnboardingItemCommand(itemId, Guid.NewGuid(), Guid.NewGuid());

        _repositoryMock.Setup(r => r.GetItemByIdAsync(itemId, It.IsAny<CancellationToken>()))
            .ReturnsAsync((OnboardingItem?)null);

        // Act & Assert
        await Assert.ThrowsAsync<Exception>(() => _handler.HandleAsync(command));
    }
}
