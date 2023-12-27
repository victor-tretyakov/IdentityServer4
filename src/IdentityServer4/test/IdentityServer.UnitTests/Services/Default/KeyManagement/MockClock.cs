using Microsoft.AspNetCore.Authentication;
using System;

namespace IdentityServer.UnitTests.Services.Default.KeyManagement;

class MockClock : ISystemClock
{
    public MockClock()
    {
    }

    public MockClock(DateTime now)
    {
        UtcNow = now;
    }

    public DateTimeOffset UtcNow { get; set; }
}