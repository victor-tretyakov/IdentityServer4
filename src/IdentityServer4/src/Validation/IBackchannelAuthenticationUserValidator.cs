using System.Threading.Tasks;

namespace IdentityServer4.Validation;

/// <summary>
/// Interface for the backchannel authentication user validation
/// </summary>
public interface IBackchannelAuthenticationUserValidator
{
    /// <summary>
    /// Validates the user.
    /// </summary>
    /// <param name="userValidatorContext"></param>
    /// <returns></returns>
    Task<BackchannelAuthenticationUserValidationResult> ValidateRequestAsync(BackchannelAuthenticationUserValidatorContext userValidatorContext);
}