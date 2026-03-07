using Maliev.LifecycleService.Application.Interfaces;
using Maliev.LifecycleService.Application.Services;
using Maliev.LifecycleService.Domain.Entities;
using Moq;
using Xunit;
using OffboardingStatus = Maliev.LifecycleService.Domain.Enums.OffboardingStatus;

namespace Maliev.LifecycleService.Tests.Unit.Services;

public class AuditLogServiceTests
{
    private readonly Mock<IAuditLogRepository> _repositoryMock;
    private readonly AuditLogService _service;

    public AuditLogServiceTests()
    {
        _repositoryMock = new Mock<IAuditLogRepository>();
        _service = new AuditLogService(_repositoryMock.Object);
    }

    [Fact]
    public async Task LogAsync_WithBothStates_LogsWithBothStates()
    {
        // Arrange
        var entityType = "OnboardingChecklist";
        var entityId = Guid.NewGuid();
        var action = "Created";
        var userId = Guid.NewGuid();
        var beforeState = new { Status = "NotStarted" };
        var afterState = new { Status = "InProgress" };

        _repositoryMock.Setup(r => r.AddAsync(It.IsAny<AuditLog>(), It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);

        // Act
        await _service.LogAsync(entityType, entityId, action, userId, beforeState, afterState);

        // Assert
        _repositoryMock.Verify(r => r.AddAsync(
            It.Is<AuditLog>(log =>
                log.EntityType == entityType &&
                log.EntityId == entityId &&
                log.Action == action &&
                log.UserId == userId &&
                log.BeforeState != null &&
                log.AfterState != null
            ),
            It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task LogAsync_WithNoStates_LogsWithNullStates()
    {
        // Arrange
        var entityType = "OnboardingChecklist";
        var entityId = Guid.NewGuid();
        var action = "Created";
        var userId = Guid.NewGuid();

        _repositoryMock.Setup(r => r.AddAsync(It.IsAny<AuditLog>(), It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);

        // Act
        await _service.LogAsync(entityType, entityId, action, userId);

        // Assert
        _repositoryMock.Verify(r => r.AddAsync(
            It.Is<AuditLog>(log =>
                log.BeforeState == null &&
                log.AfterState == null
            ),
            It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task GetLogAsync_ReturnsLog()
    {
        // Arrange
        var logId = Guid.NewGuid();
        var expectedLog = new AuditLog { Id = logId, EntityType = "Test" };

        _repositoryMock.Setup(r => r.GetByIdAsync(logId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(expectedLog);

        // Act
        var result = await _service.GetLogAsync(logId);

        // Assert
        Assert.Equal(expectedLog, result);
    }

    [Fact]
    public async Task GetLogsForEntityAsync_ReturnsLogs()
    {
        // Arrange
        var entityType = "OnboardingChecklist";
        var entityId = Guid.NewGuid();
        var expectedLogs = new List<AuditLog>
        {
            new AuditLog { Id = Guid.NewGuid(), EntityType = entityType, EntityId = entityId },
            new AuditLog { Id = Guid.NewGuid(), EntityType = entityType, EntityId = entityId }
        };

        _repositoryMock.Setup(r => r.GetByEntityAsync(entityType, entityId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(expectedLogs);

        // Act
        var result = await _service.GetLogsForEntityAsync(entityType, entityId);

        // Assert
        Assert.Equal(2, result.Count());
    }
}

public class PaycheckBlockingServiceTests
{
    [Fact]
    public void CalculatePaycheckBlocked_AllBlockerTasksCompleted_ReturnsFalse()
    {
        // Arrange
        var checklist = new OffboardingChecklist
        {
            Status = OffboardingStatus.InProgress,
            Tasks = new List<OffboardingTask>
            {
                new OffboardingTask { IsPaycheckBlocker = true, IsCompleted = true },
                new OffboardingTask { IsPaycheckBlocker = true, IsCompleted = true }
            }
        };

        // Act
        var service = new PaycheckBlockingService();
        var result = service.CalculatePaycheckBlocked(checklist);

        // Assert
        Assert.False(result);
    }

    [Fact]
    public void CalculatePaycheckBlocked_IncompleteBlockerTask_ReturnsTrue()
    {
        // Arrange
        var checklist = new OffboardingChecklist
        {
            Status = OffboardingStatus.InProgress,
            Tasks = new List<OffboardingTask>
            {
                new OffboardingTask { IsPaycheckBlocker = true, IsCompleted = true },
                new OffboardingTask 
                { 
                    IsPaycheckBlocker = true,
                    IsCompleted = false
                }
            }
        };

        // Act
        var service = new PaycheckBlockingService();
        var result = service.CalculatePaycheckBlocked(checklist);

        // Assert
        Assert.True(result);
    }

    [Fact]
    public void CalculatePaycheckBlocked_NoBlockerTasks_ReturnsFalse()
    {
        // Arrange
        var checklist = new OffboardingChecklist
        {
            Status = OffboardingStatus.InProgress,
            Tasks = new List<OffboardingTask>
            {
                new OffboardingTask { IsPaycheckBlocker = false, IsCompleted = false }
            }
        };

        // Act
        var service = new PaycheckBlockingService();
        var result = service.CalculatePaycheckBlocked(checklist);

        // Assert
        Assert.False(result);
    }

    [Fact]
    public void CalculatePaycheckBlocked_EmptyTasks_ReturnsFalse()
    {
        // Arrange
        var checklist = new OffboardingChecklist
        {
            Status = OffboardingStatus.InProgress,
            Tasks = new List<OffboardingTask>()
        };

        // Act
        var service = new PaycheckBlockingService();
        var result = service.CalculatePaycheckBlocked(checklist);

        // Assert
        Assert.False(result);
    }
}
