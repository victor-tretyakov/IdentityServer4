using FluentAssertions;
using IdentityServer4.EntityFramework.DbContexts;
using IdentityServer4.EntityFramework.Entities;
using IdentityServer4.EntityFramework.Interfaces;
using IdentityServer4.EntityFramework.Options;
using IdentityServer4.EntityFramework.Stores;
using IdentityServer4.Stores;
using IdentityServer4.Test;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Xunit;

namespace IdentityServer4.EntityFramework.IntegrationTests.TokenCleanup;

public class TokenCleanupTests : IntegrationTest<TokenCleanupTests, PersistedGrantDbContext, OperationalStoreOptions>
{
    public TokenCleanupTests(DatabaseProviderFixture<PersistedGrantDbContext> fixture) : base(fixture)
    {
        foreach (var options in TestDatabaseProviders.SelectMany(x => x.Select(y => (DbContextOptions<PersistedGrantDbContext>) y)).ToList())
        {
            using var context = new PersistedGrantDbContext(options);
            context.Database.EnsureCreated();
        }
    }

    [Theory, MemberData(nameof(TestDatabaseProviders))]
    public async Task CleanupGrantsAsync_WhenExpiredGrantsExist_ExpectExpiredGrantsRemoved(DbContextOptions<PersistedGrantDbContext> options)
    {
        var expiredGrant = new PersistedGrant
        {
            Key = Guid.NewGuid().ToString(),
            ClientId = "app1",
            Type = "reference",
            SubjectId = "123",
            Expiration = DateTime.UtcNow.AddDays(-3),
            Data = "{!}"
        };

        using (var context = new PersistedGrantDbContext(options))
        {
            context.PersistedGrants.Add(expiredGrant);
            context.SaveChanges();
        }

        await CreateSut(options).CleanupGrantsAsync();

        using (var context = new PersistedGrantDbContext(options))
        {
            context.PersistedGrants.FirstOrDefault(x => x.Key == expiredGrant.Key).Should().BeNull();
        }
    }

    [Theory, MemberData(nameof(TestDatabaseProviders))]
    public async Task CleanupGrantsAsync_WhenValidGrantsExist_ExpectValidGrantsInDb(DbContextOptions<PersistedGrantDbContext> options)
    {
        var validGrant = new PersistedGrant
        {
            Key = Guid.NewGuid().ToString(),
            ClientId = "app1",
            Type = "reference",
            SubjectId = "123",
            Expiration = DateTime.UtcNow.AddDays(3),
            Data = "{!}"
        };

        using (var context = new PersistedGrantDbContext(options))
        {
            context.PersistedGrants.Add(validGrant);
            context.SaveChanges();
        }

        await CreateSut(options).CleanupGrantsAsync();

        using (var context = new PersistedGrantDbContext(options))
        {
            context.PersistedGrants.FirstOrDefault(x => x.Key == validGrant.Key).Should().NotBeNull();
        }
    }

    [Theory, MemberData(nameof(TestDatabaseProviders))]
    public async Task CleanupGrantsAsync_WhenExpiredDeviceGrantsExist_ExpectExpiredDeviceGrantsRemoved(DbContextOptions<PersistedGrantDbContext> options)
    {
        var expiredGrant = new DeviceFlowCodes
        {
            DeviceCode = Guid.NewGuid().ToString(),
            UserCode = Guid.NewGuid().ToString(),
            ClientId = "app1",
            SubjectId = "123",
            CreationTime = DateTime.UtcNow.AddDays(-4),
            Expiration = DateTime.UtcNow.AddDays(-3),
            Data = "{!}"
        };

        using (var context = new PersistedGrantDbContext(options))
        {
            context.DeviceFlowCodes.Add(expiredGrant);
            context.SaveChanges();
        }

        await CreateSut(options).CleanupGrantsAsync();

        using (var context = new PersistedGrantDbContext(options))
        {
            context.DeviceFlowCodes.FirstOrDefault(x => x.DeviceCode == expiredGrant.DeviceCode).Should().BeNull();
        }
    }

    [Theory, MemberData(nameof(TestDatabaseProviders))]
    public async Task CleanupGrantsAsync_WhenValidDeviceGrantsExist_ExpectValidDeviceGrantsInDb(DbContextOptions<PersistedGrantDbContext> options)
    {
        var validGrant = new DeviceFlowCodes
        {
            DeviceCode = Guid.NewGuid().ToString(),
            UserCode = "2468",
            ClientId = "app1",
            SubjectId = "123",
            CreationTime = DateTime.UtcNow.AddDays(-4),
            Expiration = DateTime.UtcNow.AddDays(3),
            Data = "{!}"
        };

        using (var context = new PersistedGrantDbContext(options))
        {
            context.DeviceFlowCodes.Add(validGrant);
            context.SaveChanges();
        }

        await CreateSut(options).CleanupGrantsAsync();

        using (var context = new PersistedGrantDbContext(options))
        {
            context.DeviceFlowCodes.FirstOrDefault(x => x.DeviceCode == validGrant.DeviceCode).Should().NotBeNull();
        }
    }


    [Theory, MemberData(nameof(TestDatabaseProviders))]
    public async Task CleanupGrantsAsync_WhenFlagIsOnAndConsumedGrantsExist_ExpectConsumedGrantsRemoved(DbContextOptions<PersistedGrantDbContext> options)
    {
        var consumedGrant = new PersistedGrant
        {
            Expiration = DateTime.UtcNow.AddDays(3), // Token not yet expired
            ConsumedTime = DateTime.UtcNow.AddMinutes(-15), // But was consumed

            Key = Guid.NewGuid().ToString(),
            Type = IdentityServerConstants.PersistedGrantTypes.RefreshToken,
            ClientId = "app1",
            Data = "{!}"
        };

        using (var context = new PersistedGrantDbContext(options))
        {
            context.PersistedGrants.Add(consumedGrant);
            context.SaveChanges();
        }

        await CreateSut(options, removeConsumedTokens: true).CleanupGrantsAsync();

        using (var context = new PersistedGrantDbContext(options))
        {
            context.PersistedGrants.FirstOrDefault(x => x.Id == consumedGrant.Id).Should().BeNull();
        }
    }

    [Theory, MemberData(nameof(TestDatabaseProviders))]
    public async Task CleanupGrantsAsync_WhenFlagIsOffAndConsumedGrantsExist_ExpectConsumedGrantsNotRemoved(DbContextOptions<PersistedGrantDbContext> options)
    {
        var consumedGrant = new PersistedGrant
        {
            Expiration = DateTime.UtcNow.AddDays(3), // Token not yet expired
            ConsumedTime = DateTime.UtcNow.AddMinutes(-15), // But was consumed

            Key = Guid.NewGuid().ToString(),
            Type = IdentityServerConstants.PersistedGrantTypes.RefreshToken,
            ClientId = "app1",
            Data = "{!}"
        };

        using (var context = new PersistedGrantDbContext(options))
        {
            context.PersistedGrants.Add(consumedGrant);
            context.SaveChanges();
        }

        await CreateSut(options, removeConsumedTokens: false).CleanupGrantsAsync();

        using (var context = new PersistedGrantDbContext(options))
        {
            context.PersistedGrants.FirstOrDefault(x => x.Id == consumedGrant.Id).Should().NotBeNull();
        }
    }


    [Theory, MemberData(nameof(TestDatabaseProviders))]
    public async Task CleanupGrantsAsync_WhenFlagIsOnAndConsumedGrantsExistAndDelayIsSet_ExpectConsumedGrantsRemovedRespectsDelay(DbContextOptions<PersistedGrantDbContext> options)
    {
        var delay = 100;

        // This grant was consumed long enough in the past that it should be deleted
        var oldConsumedGrant = new PersistedGrant
        {
            Expiration = DateTime.UtcNow.AddDays(3),                    // Token not yet expired
            ConsumedTime = DateTime.UtcNow.AddSeconds(-(delay + 100)),  // But was consumed MORE than the delay in the past

            Key = Guid.NewGuid().ToString(),
            Type = IdentityServerConstants.PersistedGrantTypes.RefreshToken,
            ClientId = "app1",
            Data = "{!}"
        };

        // This grant was consumed recently enough that it should not be deleted
        var newConsumedGrant = new PersistedGrant
        {
            Expiration = DateTime.UtcNow.AddDays(3),                    // Token not yet expired
            ConsumedTime = DateTime.UtcNow.AddSeconds(-(delay - 100)),  // But was consumed LESS than the delay in the past

            Key = Guid.NewGuid().ToString(),
            Type = IdentityServerConstants.PersistedGrantTypes.RefreshToken,
            ClientId = "app1",
            Data = "{!}"
        };

        using (var context = new PersistedGrantDbContext(options))
        {
            context.PersistedGrants.Add(newConsumedGrant);
            context.PersistedGrants.Add(oldConsumedGrant);
            context.SaveChanges();
        }

        await CreateSut(options, removeConsumedTokens: true, delay).CleanupGrantsAsync();

        using (var context = new PersistedGrantDbContext(options))
        {
            context.PersistedGrants.FirstOrDefault(x => x.Id == newConsumedGrant.Id).Should().NotBeNull();
            context.PersistedGrants.FirstOrDefault(x => x.Id == oldConsumedGrant.Id).Should().BeNull();
        }

    }

    private static TokenCleanupService CreateSut(
        DbContextOptions<PersistedGrantDbContext> dbContextOpts,
        bool removeConsumedTokens,
        int consumedTokenCleanupDelay = 0
    )
    {
        StoreOptions.RemoveConsumedTokens = removeConsumedTokens;
        StoreOptions.ConsumedTokenCleanupDelay = consumedTokenCleanupDelay;
        return CreateSut(dbContextOpts);
    }

    private static TokenCleanupService CreateSut(DbContextOptions<PersistedGrantDbContext> options)
    {
        IServiceCollection services = new ServiceCollection();
        services.AddIdentityServer()
            .AddTestUsers(new List<TestUser>())
            .AddInMemoryClients(new List<Models.Client>())
            .AddInMemoryIdentityResources(new List<Models.IdentityResource>())
            .AddInMemoryApiResources(new List<Models.ApiResource>());

        services.AddScoped<IPersistedGrantDbContext, PersistedGrantDbContext>(_ =>
            new PersistedGrantDbContext(options));
        services.AddTransient<IPersistedGrantStore, PersistedGrantStore>();
        services.AddTransient<IDeviceFlowStore, DeviceFlowStore>();

        services.AddTransient<ITokenCleanupService, TokenCleanupService>();
        services.AddSingleton(StoreOptions);

        return services.BuildServiceProvider().GetRequiredService<ITokenCleanupService>() as TokenCleanupService;
    }
}