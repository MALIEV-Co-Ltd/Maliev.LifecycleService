using System.ComponentModel.DataAnnotations;
using Maliev.LifecycleService.Application.Validators;
using Xunit;

namespace Maliev.LifecycleService.Tests.Unit.Validators;

public class StartOnboardingValidatorTests
{
    private readonly StartOnboardingValidator _validator = new();

    [Fact]
    public void Validate_ValidInput_ReturnsNoErrors()
    {
        // Arrange
        var employeeId = Guid.NewGuid();
        var startDate = DateTime.UtcNow.AddDays(7);

        // Act
        var results = _validator.Validate(employeeId, startDate).ToList();

        // Assert
        Assert.Empty(results);
    }

    [Fact]
    public void Validate_EmptyEmployeeId_ReturnsError()
    {
        // Arrange
        var employeeId = Guid.Empty;
        var startDate = DateTime.UtcNow.AddDays(7);

        // Act
        var results = _validator.Validate(employeeId, startDate).ToList();

        // Assert
        Assert.Single(results);
        Assert.Contains("EmployeeId is required", results[0].ErrorMessage);
    }

    [Fact]
    public void Validate_DefaultStartDate_ReturnsError()
    {
        // Arrange
        var employeeId = Guid.NewGuid();
        var startDate = default(DateTime);

        // Act
        var results = _validator.Validate(employeeId, startDate).ToList();

        // Assert
        Assert.Single(results);
        Assert.Contains("StartDate is required", results[0].ErrorMessage);
    }

    [Fact]
    public void Validate_BothInvalid_ReturnsTwoErrors()
    {
        // Arrange
        var employeeId = Guid.Empty;
        var startDate = default(DateTime);

        // Act
        var results = _validator.Validate(employeeId, startDate).ToList();

        // Assert
        Assert.Equal(2, results.Count);
    }
}
