using System;

namespace IdentityServer4;

class DefaultClock : IClock
{
    public DateTimeOffset UtcNow => throw new NotImplementedException();
}