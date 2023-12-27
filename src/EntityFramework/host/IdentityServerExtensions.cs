using IdentityModel;
using IdentityServerHost.Extensions;
using Microsoft.AspNetCore.Builder;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;

namespace IdentityServerHost;

internal static class IdentityServerExtensions
{
    internal static WebApplicationBuilder ConfigureIdentityServer(this WebApplicationBuilder builder)
    {
        var connectionString = builder.Configuration.GetConnectionString("db");

        builder.Services.AddIdentityServer(options =>
        {
            options.Authentication.CoordinateClientLifetimesWithUserSession = true;
            options.ServerSideSessions.UserDisplayNameClaimType = JwtClaimTypes.Name;
            options.ServerSideSessions.RemoveExpiredSessions = true;
            options.ServerSideSessions.RemoveExpiredSessionsFrequency = TimeSpan.FromSeconds(10);
            options.ServerSideSessions.ExpiredSessionsTriggerBackchannelLogout = true;
            options.Endpoints.EnablePushedAuthorizationEndpoint = true;
        })
            .AddTestUsers(TestUsers.Users)
            // this adds the config data from DB (clients, resources, CORS)
            .AddConfigurationStore(options =>
            {
                options.ConfigureDbContext = builder => builder.UseSqlServer(connectionString);
            })
            // this adds the operational data from DB (codes, tokens, consents)
            .AddOperationalStore(options =>
            {
                options.ConfigureDbContext = builder => builder.UseSqlServer(connectionString);

                // this enables automatic token cleanup. this is optional.
                options.EnableTokenCleanup = true;
                options.RemoveConsumedTokens = true;
                options.TokenCleanupInterval = 10; // interval in seconds
            })
            .AddAppAuthRedirectUriValidator()
            .AddServerSideSessions()
            .AddScopeParser<ParameterizedScopeParser>()
            // this is something you will want in production to reduce load on and requests to the DB
            //.AddConfigurationStoreCache()
            ;

        return builder;
    }
}