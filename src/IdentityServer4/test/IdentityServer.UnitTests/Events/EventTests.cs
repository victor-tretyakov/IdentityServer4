using FluentAssertions;
using IdentityServer4.Events;
using System;
using Xunit;

namespace IdentityServer.UnitTests.Events;

public class EventTests
{
    [Fact]
    public void UnhandledExceptionEventCanCallToString()
    {
        try
        {
            throw new InvalidOperationException("Boom");
        }
        catch (Exception ex)
        {
            var unhandledExceptionEvent = new UnhandledExceptionEvent(ex);

            var s = unhandledExceptionEvent.ToString();

            s.Should().NotBeNullOrEmpty();
        }
    }
}