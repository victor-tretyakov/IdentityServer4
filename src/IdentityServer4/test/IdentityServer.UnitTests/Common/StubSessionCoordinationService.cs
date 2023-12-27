using IdentityServer4.Models;
using IdentityServer4.Services;
using System.Threading.Tasks;

namespace IdentityServer.UnitTests.Common;

internal class StubSessionCoordinationService : ISessionCoordinationService
{
    public Task ProcessExpirationAsync(UserSession session)
    {
        return Task.CompletedTask;
    }

    public Task ProcessLogoutAsync(UserSession session)
    {
        return Task.CompletedTask;
    }

    public Task<bool> ValidateSessionAsync(SessionValidationRequest request)
    {
        return Task.FromResult(true);
    }
}