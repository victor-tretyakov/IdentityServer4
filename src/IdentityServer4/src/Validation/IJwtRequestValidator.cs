using System.Threading.Tasks;

namespace IdentityServer4.Validation;

/// <summary>
/// Interface for request object validator
/// </summary>
public interface IJwtRequestValidator
{
    /// <summary>
    /// Validates a JWT request object
    /// </summary>
    Task<JwtRequestValidationResult> ValidateAsync(JwtRequestValidationContext context);
}