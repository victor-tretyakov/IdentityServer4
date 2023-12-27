using IdentityServer4.Validation;
using System.Threading.Tasks;

namespace IdentityServer.IntegrationTests.Common;

internal class MockCibaUserValidator : IBackchannelAuthenticationUserValidator
{
    public BackchannelAuthenticationUserValidationResult Result { get; set; } = new BackchannelAuthenticationUserValidationResult();
    public BackchannelAuthenticationUserValidatorContext UserValidatorContext { get; set; }

    public Task<BackchannelAuthenticationUserValidationResult> ValidateRequestAsync(BackchannelAuthenticationUserValidatorContext userValidatorContext)
    {
        UserValidatorContext = userValidatorContext;
        return Task.FromResult(Result);
    }
}