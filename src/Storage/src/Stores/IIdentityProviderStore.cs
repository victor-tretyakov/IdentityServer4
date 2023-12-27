using IdentityServer4.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace IdentityServer4.Stores;

/// <summary>
/// Interface to model storage of identity providers.
/// </summary>
public interface IIdentityProviderStore
{
    /// <summary>
    /// Gets all identity providers name.
    /// </summary>
    Task<IEnumerable<IdentityProviderName>> GetAllSchemeNamesAsync();

    /// <summary>
    /// Gets the identity provider by scheme name.
    /// </summary>
    /// <param name="scheme"></param>
    /// <returns></returns>
    Task<IdentityProvider?> GetBySchemeAsync(string scheme);
}