using IdentityServer4.Configuration;
using IdentityServer4.Models;
using IdentityServer4.Stores;
using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.Caching.Distributed;
using System;
using System.Threading.Tasks;

namespace IdentityServer4.Services;

/// <summary>
/// Implementation of IBackchannelAuthenticationThrottlingService that uses the IDistributedCache.
/// </summary>
public class DistributedBackchannelAuthenticationThrottlingService : IBackchannelAuthenticationThrottlingService
{
    private readonly IDistributedCache _cache;
    private readonly IClientStore _clientStore;
    private readonly ISystemClock _clock;
    private readonly IdentityServerOptions _options;

    private const string KeyPrefix = "backchannel_";

    /// <summary>
    /// Ctor
    /// </summary>
    public DistributedBackchannelAuthenticationThrottlingService(
        IDistributedCache cache,
        IClientStore clientStore,
        ISystemClock clock,
        IdentityServerOptions options)
    {
        _cache = cache;
        _clientStore = clientStore;
        _clock = clock;
        _options = options;
    }

    /// <inheritdoc/>
    public async Task<bool> ShouldSlowDown(string requestId, BackChannelAuthenticationRequest details)
    {
        if (requestId == null)
        {
            throw new ArgumentNullException(nameof(requestId));
        }

        var key = KeyPrefix + requestId;
        var options = new DistributedCacheEntryOptions { AbsoluteExpiration = _clock.UtcNow.AddSeconds(details.Lifetime) };

        var lastSeenAsString = await _cache.GetStringAsync(key);

        // record new
        if (lastSeenAsString == null)
        {
            await _cache.SetStringAsync(key, _clock.UtcNow.ToString("O"), options);
            return false;
        }

        // check interval
        if (DateTime.TryParse(lastSeenAsString, out var lastSeen))
        {
            lastSeen = lastSeen.ToUniversalTime();

            var client = await _clientStore.FindEnabledClientByIdAsync(details.ClientId);
            var interval = client?.PollingInterval ?? _options.Ciba.DefaultPollingInterval;
            if (_clock.UtcNow.UtcDateTime < lastSeen.AddSeconds(interval))
            {
                await _cache.SetStringAsync(key, _clock.UtcNow.ToString("O"), options);
                return true;
            }
        }

        // store current and continue
        await _cache.SetStringAsync(key, _clock.UtcNow.ToString("O"), options);

        return false;
    }
}