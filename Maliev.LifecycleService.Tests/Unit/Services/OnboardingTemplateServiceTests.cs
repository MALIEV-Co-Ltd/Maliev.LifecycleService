using Maliev.LifecycleService.Application.Interfaces;
using Maliev.LifecycleService.Application.Services;
using Maliev.LifecycleService.Domain.Entities;
using Moq;
using Xunit;

namespace Maliev.LifecycleService.Tests.Unit.Services;

public class OnboardingTemplateServiceTests
{
    private readonly Mock<ITemplateRepository> _repositoryMock;
    private readonly Mock<ITemplateCacheService> _cacheMock;
    private readonly Mock<ILifecycleMetrics> _metricsMock;
    private readonly OnboardingTemplateService _service;

    public OnboardingTemplateServiceTests()
    {
        _repositoryMock = new Mock<ITemplateRepository>();
        _cacheMock = new Mock<ITemplateCacheService>();
        _metricsMock = new Mock<ILifecycleMetrics>();
        _service = new OnboardingTemplateService(_repositoryMock.Object, _cacheMock.Object, _metricsMock.Object);
    }

    [Fact]
    public async Task GetTemplateForDepartmentAsync_WhenCached_ShouldReturnCached()
    {
        // Arrange
        var departmentId = Guid.NewGuid();
        var template = new OnboardingTemplate { DepartmentId = departmentId };
        _cacheMock.Setup(c => c.GetAsync(departmentId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(template);

        // Act
        var result = await _service.GetTemplateForDepartmentAsync(departmentId);

        // Assert
        Assert.Equal(template, result);
        _metricsMock.Verify(m => m.RecordTemplateCacheAccess(true), Times.Once);
        _repositoryMock.Verify(r => r.GetByDepartmentIdAsync(It.IsAny<Guid?>(), It.IsAny<CancellationToken>()), Times.Never);
    }

    [Fact]
    public async Task GetTemplateForDepartmentAsync_WhenNotCached_ShouldLoadFromDbAndCache()
    {
        // Arrange
        var departmentId = Guid.NewGuid();
        var template = new OnboardingTemplate { DepartmentId = departmentId };
        _cacheMock.Setup(c => c.GetAsync(departmentId, It.IsAny<CancellationToken>()))
            .ReturnsAsync((OnboardingTemplate?)null);
        _repositoryMock.Setup(r => r.GetByDepartmentIdAsync(departmentId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(template);

        // Act
        var result = await _service.GetTemplateForDepartmentAsync(departmentId);

        // Assert
        Assert.Equal(template, result);
        _metricsMock.Verify(m => m.RecordTemplateCacheAccess(false), Times.Once);
        _cacheMock.Verify(c => c.SetAsync(departmentId, template, It.IsAny<CancellationToken>()), Times.Once);
    }
}
