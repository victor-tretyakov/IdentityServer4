using IdentityServer4.Models;
using System.Threading.Tasks;

namespace IdentityServer4.Validation;

public interface IJwtRequestValidator
{
    Task<JwtRequestValidationResult> ValidateAsync(Client client, string jwtTokenString);
}
