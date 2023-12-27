using IdentityServer4.Services;

namespace IdentityServer.UnitTests.Common;

public class MockServerUrls : IServerUrls
{
    public string Origin { get; set; }
    public string BasePath { get; set; }
}