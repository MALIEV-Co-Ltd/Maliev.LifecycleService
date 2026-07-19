using System.ComponentModel.DataAnnotations;
using Maliev.LifecycleService.Application.Validators;
using Xunit;

namespace Maliev.LifecycleService.Tests.Unit.Validators;

public class RecordExitInterviewValidatorTests
{
    private readonly RecordExitInterviewValidator _validator = new();

    [Fact]
    public void Validate_ValidInput_ReturnsNoErrors()
    {
        // Arrange
        var employeeId = Guid.NewGuid();
        var interviewDate = DateTime.UtcNow;

        // Act
        var results = _validator.Validate(employeeId, interviewDate).ToList();

        // Assert
        Assert.Empty(results);
    }

    [Fact]
    public void Validate_EmptyEmployeeId_ReturnsError()
    {
        // Arrange
        var employeeId = Guid.Empty;
        var interviewDate = DateTime.UtcNow;

        // Act
        var results = _validator.Validate(employeeId, interviewDate).ToList();

        // Assert
        Assert.Single(results);
        Assert.Contains("EmployeeId is required", results[0].ErrorMessage);
    }

    [Fact]
    public void Validate_DefaultInterviewDate_ReturnsError()
    {
        // Arrange
        var employeeId = Guid.NewGuid();
        var interviewDate = default(DateTime);

        // Act
        var results = _validator.Validate(employeeId, interviewDate).ToList();

        // Assert
        Assert.Single(results);
        Assert.Contains("InterviewDate is required", results[0].ErrorMessage);
    }

    [Fact]
    public void Validate_BothInvalid_ReturnsTwoErrors()
    {
        // Arrange
        var employeeId = Guid.Empty;
        var interviewDate = default(DateTime);

        // Act
        var results = _validator.Validate(employeeId, interviewDate).ToList();

        // Assert
        Assert.Equal(2, results.Count);
    }
}
