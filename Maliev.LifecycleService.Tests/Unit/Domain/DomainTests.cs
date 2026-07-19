using Maliev.LifecycleService.Domain.Commands;
using Maliev.LifecycleService.Domain.Exceptions;
using Xunit;

namespace Maliev.LifecycleService.Tests.Unit.Domain;

public class DomainTests
{
    [Fact]
    public void ChecklistAlreadyStartedException_ShouldSetProperties()
    {
        var employeeId = Guid.NewGuid();
        var ex = new ChecklistAlreadyStartedException(employeeId);
        Assert.Equal(employeeId, ex.EmployeeId);
        Assert.Contains(employeeId.ToString(), ex.Message);
    }

    [Fact]
    public void ChecklistNotFoundException_ShouldSetProperties()
    {
        var employeeId = Guid.NewGuid();
        var ex = new ChecklistNotFoundException(employeeId, "Onboarding");
        Assert.Equal(employeeId, ex.EmployeeId);
        Assert.Equal("Onboarding", ex.ChecklistType);
        Assert.Contains(employeeId.ToString(), ex.Message);
    }

    [Fact]
    public void ItemAlreadyCompletedException_ShouldSetProperties()
    {
        var itemId = Guid.NewGuid();
        var ex = new ItemAlreadyCompletedException(itemId);
        Assert.Equal(itemId, ex.ItemId);
        Assert.Contains(itemId.ToString(), ex.Message);
    }

    [Fact]
    public void RevokeAccessCommand_ShouldSetProperties()
    {
        var employeeId = Guid.NewGuid();
        var correlationId = Guid.NewGuid();
        var command = new RevokeAccessCommand(employeeId, correlationId);
        Assert.Equal(employeeId, command.EmployeeId);
        Assert.Equal(correlationId, command.CorrelationId);
    }
}
