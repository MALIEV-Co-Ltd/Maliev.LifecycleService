using System.ComponentModel.DataAnnotations;
using Maliev.LifecycleService.Application.Validators;
using Xunit;

namespace Maliev.LifecycleService.Tests.Unit.Validators;

public class StartOffboardingValidatorTests
{
    private readonly StartOffboardingValidator _validator = new();

    [Fact]
    public void Validate_ValidInput_ReturnsNoErrors()
    {
        // Arrange
        var employeeId = Guid.NewGuid();
        var terminationDate = DateTime.UtcNow.AddDays(14);
        var terminationReason = "Resignation";

        // Act
        var results = _validator.Validate(employeeId, terminationDate, terminationReason).ToList();

        // Assert
        Assert.Empty(results);
    }

    [Fact]
    public void Validate_EmptyEmployeeId_ReturnsError()
    {
        // Arrange
        var employeeId = Guid.Empty;
        var terminationDate = DateTime.UtcNow.AddDays(14);
        var terminationReason = "Resignation";

        // Act
        var results = _validator.Validate(employeeId, terminationDate, terminationReason).ToList();

        // Assert
        Assert.Single(results);
        Assert.Contains("EmployeeId is required", results[0].ErrorMessage);
    }

    [Fact]
    public void Validate_DefaultTerminationDate_ReturnsError()
    {
        // Arrange
        var employeeId = Guid.NewGuid();
        var terminationDate = default(DateTime);
        var terminationReason = "Resignation";

        // Act
        var results = _validator.Validate(employeeId, terminationDate, terminationReason).ToList();

        // Assert
        Assert.Single(results);
        Assert.Contains("TerminationDate is required", results[0].ErrorMessage);
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("   ")]
    public void Validate_InvalidTerminationReason_ReturnsError(string? terminationReason)
    {
        // Arrange
        var employeeId = Guid.NewGuid();
        var terminationDate = DateTime.UtcNow.AddDays(14);

        // Act
        var results = _validator.Validate(employeeId, terminationDate, terminationReason!).ToList();

        // Assert
        Assert.Single(results);
        Assert.Contains("TerminationReason is required", results[0].ErrorMessage);
    }

    [Fact]
    public void Validate_AllInvalid_ReturnsThreeErrors()
    {
        // Arrange
        var employeeId = Guid.Empty;
        var terminationDate = default(DateTime);
        var terminationReason = "";

        // Act
        var results = _validator.Validate(employeeId, terminationDate, terminationReason).ToList();

        // Assert
        Assert.Equal(3, results.Count);
    }
}
