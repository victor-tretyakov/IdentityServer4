using IdentityServer4.Models;
using IdentityServer4.Storage.Stores;
using System.Collections.Concurrent;
using System.Threading.Tasks;

namespace IdentityServer4.Stores;

/// <summary>
/// In-memory implementation of the pushed authorization request store
/// </summary>
public class InMemoryPushedAuthorizationRequestStore : IPushedAuthorizationRequestStore
{
    private readonly ConcurrentDictionary<string, PushedAuthorizationRequest> _repository = new();

    /// <inheritdoc/>
    public Task StoreAsync(PushedAuthorizationRequest pushedAuthorizationRequest)
    {
        _repository[pushedAuthorizationRequest.ReferenceValueHash] = pushedAuthorizationRequest;

        return Task.CompletedTask;
    }

    /// <inheritdoc/>
    public Task<PushedAuthorizationRequest?> GetByHashAsync(string referenceValueHash)
    {
        _repository.TryGetValue(referenceValueHash, out var request);

        return Task.FromResult(request);
    }

    /// <inheritdoc/>
    public Task ConsumeByHashAsync(string referenceValueHash)
    {
        _repository.TryRemove(referenceValueHash, out _);
        return Task.CompletedTask;
    }
}