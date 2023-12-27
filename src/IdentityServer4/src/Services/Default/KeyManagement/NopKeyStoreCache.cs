using System.Collections.Generic;
using System.Threading.Tasks;
using System;

namespace IdentityServer4.Services.KeyManagement;

/// <summary>
/// Nop implementation of ISigningKeyStoreCache that does not cache keys.
/// </summary>
internal class NopKeyStoreCache : ISigningKeyStoreCache
{
    /// <summary>
    /// Returns null.
    /// </summary>
    /// <returns></returns>
    public Task<IEnumerable<KeyContainer>> GetKeysAsync()
    {
        return Task.FromResult<IEnumerable<KeyContainer>>(null);
    }

    /// <summary>
    /// Does not cache keys.
    /// </summary>
    /// <param name="keys"></param>
    /// <param name="duration"></param>
    /// <returns></returns>
    public Task StoreKeysAsync(IEnumerable<KeyContainer> keys, TimeSpan duration)
    {
        return Task.CompletedTask;
    }
}