using System.Threading.Tasks;

namespace IdentityServer4.Validation;

// todo: ciba perhaps make a default IBackchannelAuthenticationUserValidator based on the idtokenhint claims?
// and maybe it calls into the profile service?

/// <summary>
/// Nop implementation of IBackchannelAuthenticationUserValidator.
/// </summary>
public class NopBackchannelAuthenticationUserValidator : IBackchannelAuthenticationUserValidator
{
    /// <inheritdoc/>
    public Task<BackchannelAuthenticationUserValidationResult> ValidateRequestAsync(BackchannelAuthenticationUserValidatorContext userValidatorContext)
    {
        var result = new BackchannelAuthenticationUserValidationResult
        {
            Error = "not implemented"
        };
        return Task.FromResult(result);
    }
}