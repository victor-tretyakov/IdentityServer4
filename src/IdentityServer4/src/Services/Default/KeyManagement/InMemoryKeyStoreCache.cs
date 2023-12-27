using System.Collections.Generic;
using System.Threading.Tasks;
using System;
using Microsoft.AspNetCore.Authentication;

namespace IdentityServer4.Services.KeyManagement;

/// <summary>
/// In-memory implementation of ISigningKeyStoreCache based on static variables. This expects to be used as a singleton.
/// </summary>
internal class InMemoryKeyStoreCache : ISigningKeyStoreCache
{
    private readonly ISystemClock _clock;

    private readonly object _lock = new();

    private DateTime _expires = DateTime.MinValue;
    private IEnumerable<KeyContainer> _cache;

    /// <summary>
    /// Constructor for InMemoryKeyStoreCache.
    /// </summary>
    /// <param name="clock"></param>
    public InMemoryKeyStoreCache(ISystemClock clock)
    {
        _clock = clock;
    }

    /// <summary>
    /// Returns cached keys.
    /// </summary>
    /// <returns></returns>
    public Task<IEnumerable<KeyContainer>> GetKeysAsync()
    {
        DateTime expires;
        IEnumerable<KeyContainer> keys;

        lock (_lock)
        {
            expires = _expires;
            keys = _cache;
        }

        if (keys != null && expires >= _clock.UtcNow.UtcDateTime)
        {
            return Task.FromResult(keys);
        }

        return Task.FromResult<IEnumerable<KeyContainer>>(null);
    }

    /// <summary>
    /// Caches keys for duration.
    /// </summary>
    /// <param name="keys"></param>
    /// <param name="duration"></param>
    /// <returns></returns>
    public Task StoreKeysAsync(IEnumerable<KeyContainer> keys, TimeSpan duration)
    {
        lock (_lock)
        {
            _expires = _clock.UtcNow.UtcDateTime.Add(duration);
            _cache = keys;
        }

        return Task.CompletedTask;
    }
}