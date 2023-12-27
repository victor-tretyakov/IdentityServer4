using IdentityServer4.Validation;
using System.Threading.Tasks;

namespace IdentityServer4.ResponseHandling;

/// <summary>
/// Service that generates response models for the pushed authorization endpoint. This service encapsulates the behavior that
/// is needed to create a response model from a validated request. 
/// </summary>
public interface IPushedAuthorizationResponseGenerator
{
    /// <summary>
    /// Asynchronously creates a response model from a validated pushed authorization request.
    /// </summary>
    /// <param name="request">The validated pushed authorization request.</param>
    /// <returns>A task that contains response model indicating either success or failure.</returns>
    Task<PushedAuthorizationResponse> CreateResponseAsync(ValidatedPushedAuthorizationRequest request);
}