using Maliev.LifecycleService.Application.Commands.Offboarding;
using Maliev.LifecycleService.Application.Commands.Offboarding.Handlers;
using Maliev.LifecycleService.Application.Interfaces;
using Maliev.LifecycleService.Application.Services;
using Maliev.LifecycleService.Domain.Entities;
using Maliev.LifecycleService.Domain.Enums;
using Maliev.MessagingContracts;
using Maliev.MessagingContracts.Contracts.Lifecycle;
using Moq;
using Xunit;

namespace Maliev.LifecycleService.Tests.Unit.Handlers;

public class StartOffboardingCommandHandlerTests
{
    private readonly Mock<IOffboardingRepository> _repositoryMock;
    private readonly Mock<IAuditLogService> _auditLogServiceMock;
    private readonly Mock<IEventPublisher> _eventPublisherMock;
    private readonly Mock<ILifecycleMetrics> _metricsMock;
    private readonly StartOffboardingCommandHandler _handler;

    public StartOffboardingCommandHandlerTests()
    {
        _repositoryMock = new Mock<IOffboardingRepository>();
        _auditLogServiceMock = new Mock<IAuditLogService>();
        _eventPublisherMock = new Mock<IEventPublisher>();
        _metricsMock = new Mock<ILifecycleMetrics>();
        _handler = new StartOffboardingCommandHandler(
            _repositoryMock.Object,
            _auditLogServiceMock.Object,
            _eventPublisherMock.Object,
            _metricsMock.Object
        );
    }

    [Fact]
    public async Task HandleAsync_ValidCommand_CreatesOffboardingChecklist()
    {
        // Arrange
        var employeeId = Guid.NewGuid();
        var userId = Guid.NewGuid();
        var terminationDate = DateTime.UtcNow.AddDays(14);

        var command = new StartOffboardingCommand(
            employeeId,
            terminationDate,
            "Resignation",
            true,
            userId
        );

        _repositoryMock.Setup(r => r.GetByEmployeeIdAsync(employeeId, It.IsAny<CancellationToken>()))
            .ReturnsAsync((OffboardingChecklist?)null);

        _repositoryMock.Setup(r => r.AddAsync(It.IsAny<OffboardingChecklist>(), It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);

        // Act
        var result = await _handler.HandleAsync(command);

        // Assert
        Assert.NotEqual(Guid.Empty, result);

        _repositoryMock.Verify(r => r.AddAsync(
            It.Is<OffboardingChecklist>(c =>
                c.EmployeeId == employeeId &&
                c.TerminationDate == terminationDate &&
                c.TerminationReason == "Resignation" &&
                c.EligibleForRehire == true
            ),
            It.IsAny<CancellationToken>()), Times.Once);

        _metricsMock.Verify(m => m.RecordOffboardingStarted(), Times.Once);
    }

    [Fact]
    public async Task HandleAsync_OffboardingAlreadyStarted_ThrowsException()
    {
        // Arrange
        var employeeId = Guid.NewGuid();
        var userId = Guid.NewGuid();

        var existingChecklist = new OffboardingChecklist
        {
            Id = Guid.NewGuid(),
            EmployeeId = employeeId,
            Status = OffboardingStatus.InProgress
        };

        var command = new StartOffboardingCommand(
            employeeId,
            DateTime.UtcNow.AddDays(14),
            "Resignation",
            true,
            userId
        );

        _repositoryMock.Setup(r => r.GetByEmployeeIdAsync(employeeId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(existingChecklist);

        // Act & Assert
        await Assert.ThrowsAsync<Exception>(() => _handler.HandleAsync(command));
    }
}

public class CompleteOffboardingTaskCommandHandlerTests
{
    private readonly Mock<IOffboardingRepository> _repositoryMock;
    private readonly Mock<IPaycheckBlockingService> _paycheckBlockingServiceMock;
    private readonly Mock<IAuditLogService> _auditLogServiceMock;
    private readonly Mock<IEventPublisher> _eventPublisherMock;
    private readonly Mock<ILifecycleMetrics> _metricsMock;
    private readonly CompleteOffboardingTaskCommandHandler _handler;

    public CompleteOffboardingTaskCommandHandlerTests()
    {
        _repositoryMock = new Mock<IOffboardingRepository>();
        _paycheckBlockingServiceMock = new Mock<IPaycheckBlockingService>();
        _auditLogServiceMock = new Mock<IAuditLogService>();
        _eventPublisherMock = new Mock<IEventPublisher>();
        _metricsMock = new Mock<ILifecycleMetrics>();
        _handler = new CompleteOffboardingTaskCommandHandler(
            _repositoryMock.Object,
            _paycheckBlockingServiceMock.Object,
            _auditLogServiceMock.Object,
            _eventPublisherMock.Object,
            _metricsMock.Object
        );
    }

    [Fact]
    public async Task HandleAsync_ValidTask_CompletesTask()
    {
        // Arrange
        var taskId = Guid.NewGuid();
        var userId = Guid.NewGuid();
        var notes = "Task completed";

        var checklist = new OffboardingChecklist
        {
            Id = Guid.NewGuid(),
            EmployeeId = Guid.NewGuid(),
            Status = OffboardingStatus.InProgress,
            Tasks = new List<OffboardingTask>
            {
                new OffboardingTask
                {
                    Id = taskId,
                    Title = "Task to complete",
                    IsCompleted = false,
                    OffboardingChecklistId = Guid.NewGuid(),
                    Checklist = null!
                }
            }
        };

        var task = checklist.Tasks.First();
        task.Checklist = checklist;

        var command = new CompleteOffboardingTaskCommand(taskId, userId, notes);

        _repositoryMock.Setup(r => r.GetTaskByIdAsync(taskId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(task);

        _paycheckBlockingServiceMock.Setup(p => p.CalculatePaycheckBlocked(It.IsAny<OffboardingChecklist>()))
            .Returns(false);

        _repositoryMock.Setup(r => r.UpdateAsync(checklist, It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);

        // Act
        await _handler.HandleAsync(command);

        // Assert
        Assert.True(task.IsCompleted);
        Assert.Equal(notes, task.Notes);
        Assert.NotNull(task.CompletedDate);

        _eventPublisherMock.Verify(e => e.PublishAsync(
            It.IsAny<LifecycleTaskCompletedEvent>(),
            It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task HandleAsync_AllTasksComplete_UpdatesChecklistStatusAndPublishesEvents()
    {
        // Arrange
        var taskId = Guid.NewGuid();
        var userId = Guid.NewGuid();

        var checklist = new OffboardingChecklist
        {
            Id = Guid.NewGuid(),
            EmployeeId = Guid.NewGuid(),
            Status = OffboardingStatus.InProgress,
            CreatedDate = DateTime.UtcNow.AddDays(-5),
            Tasks = new List<OffboardingTask>
            {
                new OffboardingTask { Id = taskId, IsCompleted = false, Checklist = null! },
                new OffboardingTask { Id = Guid.NewGuid(), IsCompleted = true, Checklist = null! }
            }
        };

        var task = checklist.Tasks.First();
        task.Checklist = checklist;

        var command = new CompleteOffboardingTaskCommand(taskId, userId, null);

        _repositoryMock.Setup(r => r.GetTaskByIdAsync(taskId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(task);

        _paycheckBlockingServiceMock.Setup(p => p.CalculatePaycheckBlocked(It.IsAny<OffboardingChecklist>()))
            .Returns(false);

        _repositoryMock.Setup(r => r.UpdateAsync(checklist, It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);

        // Act
        await _handler.HandleAsync(command);

        // Assert
        Assert.Equal(OffboardingStatus.Completed, checklist.Status);
        Assert.NotNull(checklist.CompletedDate);

        _metricsMock.Verify(m => m.RecordOffboardingCompleted(), Times.Once);
        
        _eventPublisherMock.Verify(e => e.PublishAsync(
            It.IsAny<OffboardingCompletedEvent>(),
            It.IsAny<CancellationToken>()), Times.Once);
        
        _eventPublisherMock.Verify(e => e.PublishAsync(
            It.IsAny<AccessRevocationRequiredEvent>(),
            It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task HandleAsync_TaskNotFound_ThrowsException()
    {
        // Arrange
        var taskId = Guid.NewGuid();
        var command = new CompleteOffboardingTaskCommand(taskId, Guid.NewGuid(), null);

        _repositoryMock.Setup(r => r.GetTaskByIdAsync(taskId, It.IsAny<CancellationToken>()))
            .ReturnsAsync((OffboardingTask?)null);

        // Act & Assert
        await Assert.ThrowsAsync<Exception>(() => _handler.HandleAsync(command));
    }

    [Fact]
    public async Task HandleAsync_AlreadyCompleted_DoesNotProcessAgain()
    {
        // Arrange
        var taskId = Guid.NewGuid();
        var userId = Guid.NewGuid();

        var checklist = new OffboardingChecklist
        {
            Id = Guid.NewGuid(),
            Tasks = new List<OffboardingTask>
            {
                new OffboardingTask { Id = taskId, IsCompleted = true, Checklist = null! }
            }
        };

        var task = checklist.Tasks.First();
        task.Checklist = checklist;

        var command = new CompleteOffboardingTaskCommand(taskId, userId, null);

        _repositoryMock.Setup(r => r.GetTaskByIdAsync(taskId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(task);

        // Act
        await _handler.HandleAsync(command);

        // Assert - no updates should happen
        _repositoryMock.Verify(r => r.UpdateAsync(It.IsAny<OffboardingChecklist>(), It.IsAny<CancellationToken>()), Times.Never);
    }
}

public class ReassignOffboardingTaskCommandHandlerTests
{
    private readonly Mock<IOffboardingRepository> _repositoryMock;
    private readonly Mock<IAuditLogService> _auditLogServiceMock;
    private readonly ReassignOffboardingTaskCommandHandler _handler;

    public ReassignOffboardingTaskCommandHandlerTests()
    {
        _repositoryMock = new Mock<IOffboardingRepository>();
        _auditLogServiceMock = new Mock<IAuditLogService>();
        _handler = new ReassignOffboardingTaskCommandHandler(_repositoryMock.Object, _auditLogServiceMock.Object);
    }

    [Fact]
    public async Task HandleAsync_ValidTask_ReassignsTask()
    {
        // Arrange
        var taskId = Guid.NewGuid();
        var newAssignee = Guid.NewGuid();
        var userId = Guid.NewGuid();

        var checklist = new OffboardingChecklist
        {
            Id = Guid.NewGuid(),
            Tasks = new List<OffboardingTask>
            {
                new OffboardingTask
                {
                    Id = taskId,
                    Title = "Task to reassign",
                    AssignedTo = Guid.NewGuid(),
                    Checklist = null!
                }
            }
        };

        var task = checklist.Tasks.First();
        task.Checklist = checklist;

        var command = new ReassignOffboardingTaskCommand(taskId, newAssignee, userId);

        _repositoryMock.Setup(r => r.GetTaskByIdAsync(taskId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(task);

        _repositoryMock.Setup(r => r.UpdateAsync(checklist, It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);

        // Act
        await _handler.HandleAsync(command);

        // Assert
        Assert.Equal(newAssignee, task.AssignedTo);
    }
}
