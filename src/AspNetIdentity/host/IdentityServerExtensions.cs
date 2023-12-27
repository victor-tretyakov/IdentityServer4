// Copyright (c) Brock Allen & Dominick Baier. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.


using IdentityServer4.Models;

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