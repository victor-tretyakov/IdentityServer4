using Microsoft.AspNetCore.Authentication;
using System;

namespace IdentityServer.IntegrationTests.Common;

internal class MockClock : ISystemClock
{
    public DateTimeOffset UtcNow { get; set; } = DateTimeOffset.UtcNow;
}