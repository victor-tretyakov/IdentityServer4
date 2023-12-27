using IdentityServer4.Services.KeyManagement;
using System.Collections.Generic;
using System.Threading.Tasks;
using System;
using System.Linq;

namespace IdentityServer.UnitTests.Services.Default.KeyManagement;

internal class MockSigningKeyStoreCache : ISigningKeyStoreCache
{
    public List<KeyContainer> Cache { get; set; } = new List<KeyContainer>();

    public bool GetKeysAsyncWasCalled { get; set; }
    public bool StoreKeysAsyncWasCalled { get; set; }
    public TimeSpan StoreKeysAsyncDuration { get; set; }

    public Task<IEnumerable<KeyContainer>> GetKeysAsync()
    {
        GetKeysAsyncWasCalled = true;
        return Task.FromResult(Cache.AsEnumerable());
    }

    public Task StoreKeysAsync(IEnumerable<KeyContainer> keys, TimeSpan duration)
    {
        StoreKeysAsyncWasCalled = true;
        StoreKeysAsyncDuration = duration;

        Cache = keys.ToList();
        return Task.CompletedTask;
    }
}