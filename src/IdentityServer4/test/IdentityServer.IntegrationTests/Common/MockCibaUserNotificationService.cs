using IdentityServer4.Models;
using IdentityServer4.Services;
using System.Threading.Tasks;

namespace IdentityServer.IntegrationTests.Common;

internal class MockCibaUserNotificationService : IBackchannelAuthenticationUserNotificationService
{
    public BackchannelUserLoginRequest LoginRequest { get; set; }

    public Task SendLoginRequestAsync(BackchannelUserLoginRequest request)
    {
        LoginRequest = request;
        return Task.CompletedTask;
    }
}