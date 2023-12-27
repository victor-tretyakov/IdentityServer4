using IdentityServer4.Services;
using System.Threading.Tasks;

namespace IdentityServer.UnitTests.Validation.Setup;

internal class TestIssuerNameService : IIssuerNameService
{
    private readonly string _value;

    public TestIssuerNameService(string value = null)
    {
        _value = value ?? "https://identityserver";
    }

    public Task<string> GetCurrentAsync()
    {
        return Task.FromResult(_value);
    }
}