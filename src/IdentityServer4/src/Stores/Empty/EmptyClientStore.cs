using IdentityServer4.Models;
using System.Threading.Tasks;

namespace IdentityServer4.Stores.Empty;

internal class EmptyClientStore : IClientStore
{
    public Task<Client> FindClientByIdAsync(string clientId)
    {
        return Task.FromResult<Client>(null);
    }
}