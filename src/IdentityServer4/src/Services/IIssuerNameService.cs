using System.Threading.Tasks;

namespace IdentityServer4.Services;

/// <summary>
/// Abstract access to the current issuer name
/// </summary>
public interface IIssuerNameService
{
    /// <summary>
    /// Returns the issuer name for the current request
    /// </summary>
    /// <returns></returns>
    Task<string> GetCurrentAsync();
}