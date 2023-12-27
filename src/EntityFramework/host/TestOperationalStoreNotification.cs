using IdentityServer4.EntityFramework;
using IdentityServer4.EntityFramework.Entities;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace IdentityServerHost;

public class TestOperationalStoreNotification : IOperationalStoreNotification
{
    public TestOperationalStoreNotification()
    {
        Console.WriteLine("ctor");
    }

    public Task PersistedGrantsRemovedAsync(IEnumerable<PersistedGrant> persistedGrants, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(persistedGrants);
        foreach (var grant in persistedGrants)
        {
            Console.WriteLine("cleaned: " + grant.Type);
        }
        return Task.CompletedTask;
    }

    public Task DeviceCodesRemovedAsync(IEnumerable<DeviceFlowCodes> deviceCodes, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(deviceCodes);
        foreach (var deviceCode in deviceCodes)
        {
            Console.WriteLine("cleaned device code");
        }
        return Task.CompletedTask;
    }

    public Task ServerSideSessionsRemovedAsync(IEnumerable<ServerSideSession> userSessions, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(userSessions);
        foreach (var session in userSessions)
        {
            Console.WriteLine("cleaned user session");
        }
        return Task.CompletedTask;
    }
}