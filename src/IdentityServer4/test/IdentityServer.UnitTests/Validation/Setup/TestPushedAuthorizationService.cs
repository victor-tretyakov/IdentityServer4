using IdentityServer4.Services;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace IdentityServer.UnitTests.Validation.Setup;

/// <summary>
/// Test implementation of the pushed authorization service. Always returns a setup
/// pushed authorization request.
/// </summary>
internal class TestPushedAuthorizationService : IPushedAuthorizationService
{
    Dictionary<string, DeserializedPushedAuthorizationRequest> pushedRequests = new();


    public Task ConsumeAsync(string referenceValue)
    {
        pushedRequests.Remove(referenceValue);
        return Task.CompletedTask;
    }

    public Task<DeserializedPushedAuthorizationRequest> GetPushedAuthorizationRequestAsync(string referenceValue)
    {
        pushedRequests.TryGetValue(referenceValue, out var par);
        return Task.FromResult(par);
    }

    public Task StoreAsync(DeserializedPushedAuthorizationRequest request)
    {
        pushedRequests[request.ReferenceValue] = request;
        return Task.CompletedTask;
    }
}