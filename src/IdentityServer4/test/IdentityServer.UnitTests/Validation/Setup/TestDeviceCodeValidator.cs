using IdentityServer4.Models;
using IdentityServer4.Validation;
using System.Threading.Tasks;

namespace IdentityServer.UnitTests.Validation.Setup;

public class TestDeviceCodeValidator : IDeviceCodeValidator
{
    private readonly bool shouldError;

    public DeviceCode DeviceCodeResult { get; set; } = new DeviceCode();

    public TestDeviceCodeValidator(bool shouldError = false)
    {
        this.shouldError = shouldError;
    }

    public Task ValidateAsync(DeviceCodeValidationContext context)
    {
        if (shouldError) context.Result = new TokenRequestValidationResult(context.Request, "error");
        else context.Result = new TokenRequestValidationResult(context.Request);

        context.Request.DeviceCode = DeviceCodeResult;

        return Task.CompletedTask;
    }
}