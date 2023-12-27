using IdentityServer4.Models;
using IdentityServer4.Services;
using System.Threading.Tasks;

namespace IdentityServer.UnitTests.Common;

public class MockJwtRequestUriHttpClient : IJwtRequestUriHttpClient
{
    public string Jwt { get; set; }

    public Task<string> GetJwtAsync(string url, Client client)
    {
        return Task.FromResult(Jwt);
    }
}