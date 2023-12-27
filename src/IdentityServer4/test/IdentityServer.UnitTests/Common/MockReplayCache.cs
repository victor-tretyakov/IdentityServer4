using IdentityServer4.Services;
using System;
using System.Threading.Tasks;

namespace IdentityServer.UnitTests.Common;

public class MockReplayCache : IReplayCache
{
    public bool Exists { get; set; }

    public Task AddAsync(string purpose, string handle, DateTimeOffset expiration)
    {
        return Task.CompletedTask;
    }

    public Task<bool> ExistsAsync(string purpose, string handle)
    {
        return Task.FromResult(Exists);
    }
}