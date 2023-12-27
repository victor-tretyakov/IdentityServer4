// Copyright (c) Brock Allen & Dominick Baier. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.


using IdentityServer4.Infrastructure;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Microsoft.Extensions.Options;
using System;
using System.Linq;

namespace IdentityServer4.Configuration;

internal class ConfigureOpenIdConnectOptions : IPostConfigureOptions<OpenIdConnectOptions>
{
    private string[] _schemes;
    private readonly IServiceProvider _serviceProvider;

    public ConfigureOpenIdConnectOptions(string[] schemes, IServiceProvider serviceProvider)
    {
        _schemes = schemes ?? throw new ArgumentNullException(nameof(schemes));
        _serviceProvider = serviceProvider ?? throw new ArgumentNullException(nameof(serviceProvider));
    }

    public void PostConfigure(string name, OpenIdConnectOptions options)
    {
        // no schemes means configure them all
        if (_schemes.Length == 0 || _schemes.Contains(name))
        {
            options.StateDataFormat = new DistributedCacheStateDataFormatter(_serviceProvider, name);
        }
    }
}