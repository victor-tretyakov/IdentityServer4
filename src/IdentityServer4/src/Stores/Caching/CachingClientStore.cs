// Copyright (c) Brock Allen & Dominick Baier. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.


using IdentityServer4.Configuration;
using IdentityServer4.Models;
using IdentityServer4.Services;
using System.Threading.Tasks;

namespace IdentityServer4.Stores;

/// <summary>
/// Cache decorator for IClientStore
/// </summary>
/// <typeparam name="T"></typeparam>
/// <seealso cref="IdentityServer.Stores.IClientStore" />
public class CachingClientStore<T> : IClientStore
    where T : IClientStore
{
    private readonly IdentityServerOptions _options;
    private readonly ICache<Client> _cache;
    private readonly IClientStore _inner;

    /// <summary>
    /// Initializes a new instance of the <see cref="CachingClientStore{T}"/> class.
    /// </summary>
    /// <param name="options">The options.</param>
    /// <param name="inner">The inner.</param>
    /// <param name="cache">The cache.</param>
    public CachingClientStore(IdentityServerOptions options, T inner, ICache<Client> cache)
    {
        _options = options;
        _inner = inner;
        _cache = cache;
    }

    /// <summary>
    /// Finds a client by id
    /// </summary>
    /// <param name="clientId">The client id</param>
    /// <returns>
    /// The client
    /// </returns>
    public async Task<Client> FindClientByIdAsync(string clientId)
    {
        var client = await _cache.GetOrAddAsync(clientId,
            _options.Caching.ClientStoreExpiration,
            async () => await _inner.FindClientByIdAsync(clientId));

        return client;
    }
}