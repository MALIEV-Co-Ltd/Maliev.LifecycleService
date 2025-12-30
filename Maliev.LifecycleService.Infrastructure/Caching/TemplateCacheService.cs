using System.Text.Json;
using Maliev.LifecycleService.Application.Interfaces;
using Maliev.LifecycleService.Domain.Entities;
using Microsoft.Extensions.Caching.Distributed;

namespace Maliev.LifecycleService.Infrastructure.Caching;

/// <summary>
/// Implementation of the template cache service using distributed cache.
/// </summary>
public class TemplateCacheService : ITemplateCacheService
{
    private readonly IDistributedCache _cache;
    private static readonly TimeSpan DefaultExpiration = TimeSpan.FromHours(1);

    /// <summary>
    /// Initializes a new instance of the <see cref="TemplateCacheService"/> class.
    /// </summary>
    /// <param name="cache">The distributed cache.</param>
    public TemplateCacheService(IDistributedCache cache)
    {
        _cache = cache;
    }

    /// <inheritdoc/>
    public async Task<OnboardingTemplate?> GetAsync(Guid? departmentId, CancellationToken cancellationToken = default)
    {
        var cacheKey = GetCacheKey(departmentId);
        var cachedData = await _cache.GetStringAsync(cacheKey, cancellationToken);

        if (string.IsNullOrEmpty(cachedData))
        {
            return null;
        }

        return JsonSerializer.Deserialize<OnboardingTemplate>(cachedData);
    }

    /// <inheritdoc/>
    public async Task SetAsync(Guid? departmentId, OnboardingTemplate template, CancellationToken cancellationToken = default)
    {
        var cacheKey = GetCacheKey(departmentId);
        var serializedData = JsonSerializer.Serialize(template);

        await _cache.SetStringAsync(cacheKey, serializedData, new DistributedCacheEntryOptions
        {
            AbsoluteExpirationRelativeToNow = DefaultExpiration
        }, cancellationToken);
    }

    /// <inheritdoc/>
    public async Task RemoveAsync(Guid? departmentId, CancellationToken cancellationToken = default)
    {
        var cacheKey = GetCacheKey(departmentId);
        await _cache.RemoveAsync(cacheKey, cancellationToken);
    }

    private static string GetCacheKey(Guid? departmentId) => $"lifecycle:template:{departmentId?.ToString() ?? "default"}";
}
