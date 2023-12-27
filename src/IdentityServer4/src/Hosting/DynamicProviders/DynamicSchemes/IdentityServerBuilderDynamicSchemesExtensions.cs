using IdentityServer4.Models;
using Microsoft.Extensions.DependencyInjection;
using System.Collections.Generic;

namespace IdentityServer4.Hosting.DynamicProviders;

/// <summary>
/// Add extension methods for configuring generic dynamic providers.
/// </summary>
public static class IdentityServerBuilderDynamicSchemesExtensions
{
    /// <summary>
    /// Adds the in memory identity provider store.
    /// </summary>
    /// <param name="builder">The builder.</param>
    /// <param name="providers"></param>
    /// <returns></returns>
    public static IIdentityServerBuilder AddInMemoryIdentityProviders(
        this IIdentityServerBuilder builder, IEnumerable<IdentityProvider> providers)
    {
        builder.Services.AddSingleton(providers);
        builder.AddIdentityProviderStore<InMemoryIdentityProviderStore>();

        return builder;
    }
}