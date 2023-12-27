using IdentityServer4.Models;
using IdentityServer4.Services;
using System.Threading.Tasks;

namespace IdentityServer.UnitTests.Common;

internal class MockTokenCreationService : ITokenCreationService
{
    public string TokenResult { get; set; }
    public Token Token { get; set; }

    public Task<string> CreateTokenAsync(Token token)
    {
        Token = token;
        return Task.FromResult(TokenResult);
    }
}