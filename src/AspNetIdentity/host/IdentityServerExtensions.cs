// Copyright (c) Duende Software. All rights reserved.
// See LICENSE in the project root for license information.

using IdentityServer4.Models;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;

namespace IdentityServerHost;

internal static class IdentityServerExtensions
{
    internal static WebApplicationBuilder ConfigureIdentityServer(this WebApplicationBuilder builder)
    {
        builder.Services.AddIdentityServer()
            .AddDeveloperSigningCredential()
            .AddInMemoryIdentityResources(Configuration.Resources.IdentityResources)
            .AddInMemoryApiResources(Configuration.Resources.ApiResources)
            .AddInMemoryApiScopes(Configuration.Resources.ApiScopes)
            .AddInMemoryClients(Configuration.Clients.Get())
            .AddAspNetIdentity<ApplicationUser>();

        return builder;
    }
}