using System.Diagnostics.Metrics;
using Maliev.LifecycleService.Application.Interfaces;

namespace Maliev.LifecycleService.Infrastructure.Metrics;

/// <summary>
/// Implementation of the lifecycle metrics service using .NET Metrics API.
/// </summary>
public class LifecycleMetrics : ILifecycleMetrics
{
    private readonly Counter<long> _onboardingStartedCounter;
    private readonly Counter<long> _onboardingCompletedCounter;
    private readonly Counter<long> _offboardingStartedCounter;
    private readonly Counter<long> _offboardingCompletedCounter;
    private readonly Counter<long> _tasksOverdueCounter;
    private readonly Counter<long> _exitInterviewsRecordedCounter;

    private readonly Histogram<double> _onboardingDurationHistogram;
    private readonly Histogram<double> _offboardingDurationHistogram;

    private readonly Counter<long> _templateCacheHitCounter;
    private readonly Counter<long> _templateCacheMissCounter;

    /// <summary>
    /// Initializes a new instance of the <see cref="LifecycleMetrics"/> class.
    /// </summary>
    /// <param name="meterFactory">The meter factory.</param>
    public LifecycleMetrics(IMeterFactory meterFactory)
    {
        var meter = meterFactory.Create("maliev.lifecycle");

        _onboardingStartedCounter = meter.CreateCounter<long>("lifecycle.onboarding.started.total");
        _onboardingCompletedCounter = meter.CreateCounter<long>("lifecycle.onboarding.completed.total");
        _offboardingStartedCounter = meter.CreateCounter<long>("lifecycle.offboarding.started.total");
        _offboardingCompletedCounter = meter.CreateCounter<long>("lifecycle.offboarding.completed.total");
        _tasksOverdueCounter = meter.CreateCounter<long>("lifecycle.tasks.overdue.total");
        _exitInterviewsRecordedCounter = meter.CreateCounter<long>("lifecycle.exit_interviews.recorded.total");

        _onboardingDurationHistogram = meter.CreateHistogram<double>("lifecycle.onboarding.duration_days", "days");
        _offboardingDurationHistogram = meter.CreateHistogram<double>("lifecycle.offboarding.duration_days", "days");

        _templateCacheHitCounter = meter.CreateCounter<long>("lifecycle.template_cache.hits.total");
        _templateCacheMissCounter = meter.CreateCounter<long>("lifecycle.template_cache.misses.total");
    }

    /// <inheritdoc/>
    public void RecordOnboardingStarted() => _onboardingStartedCounter.Add(1);
    /// <inheritdoc/>
    public void RecordOnboardingCompleted() => _onboardingCompletedCounter.Add(1);
    /// <inheritdoc/>
    public void RecordOffboardingStarted() => _offboardingStartedCounter.Add(1);
    /// <inheritdoc/>
    public void RecordOffboardingCompleted() => _offboardingCompletedCounter.Add(1);
    /// <inheritdoc/>
    public void RecordTaskOverdue() => _tasksOverdueCounter.Add(1);
    /// <inheritdoc/>
    public void RecordExitInterviewRecorded() => _exitInterviewsRecordedCounter.Add(1);

    /// <inheritdoc/>
    public void RecordOnboardingDuration(double days) => _onboardingDurationHistogram.Record(days);
    /// <inheritdoc/>
    public void RecordOffboardingDuration(double days) => _offboardingDurationHistogram.Record(days);

    /// <inheritdoc/>
    public void RecordTemplateCacheAccess(bool hit)
    {
        if (hit) _templateCacheHitCounter.Add(1);
        else _templateCacheMissCounter.Add(1);
    }
}
