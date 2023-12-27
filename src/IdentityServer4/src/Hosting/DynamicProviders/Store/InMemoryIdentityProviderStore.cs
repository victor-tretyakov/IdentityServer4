using IdentityServer4.Models;
using IdentityServer4.Stores;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace IdentityServer4.Hosting.DynamicProviders;

class InMemoryIdentityProviderStore : IIdentityProviderStore
{
    private readonly IEnumerable<IdentityProvider> _providers;

    public InMemoryIdentityProviderStore(IEnumerable<IdentityProvider> providers)
    {
        _providers = providers;
    }

    public Task<IEnumerable<IdentityProviderName>> GetAllSchemeNamesAsync()
    {
        var items = _providers.Select(x => new IdentityProviderName
        {
            Enabled = x.Enabled,
            DisplayName = x.DisplayName,
            Scheme = x.Scheme
        });

        return Task.FromResult(items);
    }

    public Task<IdentityProvider> GetBySchemeAsync(string scheme)
    {
        var item = _providers.FirstOrDefault(x => x.Scheme == scheme);
        return Task.FromResult(item);
    }
}