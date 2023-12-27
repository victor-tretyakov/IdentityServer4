using System.Collections.Specialized;
using System.Threading.Tasks;

namespace IdentityServer4.Validation;

/// <summary>
/// Interface for the backchannel authentication request validator
/// </summary>
public interface IBackchannelAuthenticationRequestValidator
{
    /// <summary>
    /// Validates the request.
    /// </summary>
    /// <param name="parameters">The parameters.</param>
    /// <param name="clientValidationResult">The client validation result.</param>
    /// <returns></returns>
    Task<BackchannelAuthenticationRequestValidationResult> ValidateRequestAsync(NameValueCollection parameters, ClientSecretValidationResult clientValidationResult);
}