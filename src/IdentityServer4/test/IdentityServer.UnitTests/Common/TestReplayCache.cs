using IdentityServer4.Services;
using Microsoft.AspNetCore.Authentication;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace IdentityServer.UnitTests.Common;

public class TestReplayCache : IReplayCache
{
    private readonly ISystemClock _clock;
    Dictionary<string, DateTimeOffset> _values = new Dictionary<string, DateTimeOffset>();

    public TestReplayCache(ISystemClock clock)
    {
        _clock = clock;
    }

    public Task AddAsync(string purpose, string handle, DateTimeOffset expiration)
    {
        _values[purpose + handle] = expiration;
        return Task.CompletedTask;
    }

    public Task<bool> ExistsAsync(string purpose, string handle)
    {
        if (_values.TryGetValue(purpose + handle, out var expiration))
        {
            return Task.FromResult(_clock.UtcNow <= expiration);
        }
        return Task.FromResult(false);
    }
}