using IdentityServer4.Configuration;
using IdentityServer4.Services;
using IdentityServer4.Validation;
using Microsoft.Extensions.Logging;
using System;
using System.Threading.Tasks;

namespace IdentityServer4.ResponseHandling;

/// <inheritdoc />
public class PushedAuthorizationResponseGenerator : IPushedAuthorizationResponseGenerator
{
    private readonly IHandleGenerationService _handleGeneration;
    private readonly IdentityServerOptions _options;
    private readonly IPushedAuthorizationService _pushedAuthorizationService;

    /// <summary>
    /// Initializes a new instance of the <see cref="PushedAuthorizationResponseGenerator"/> class.
    /// </summary>
    /// <param name="handleGeneration">The handle generation service, used for creation of request uri reference values.
    /// </param>
    /// <param name="options">The IdentityServer options</param>
    /// <param name="pushedAuthorizationService">The pushed authorization service</param>
    public PushedAuthorizationResponseGenerator(IHandleGenerationService handleGeneration,
        IdentityServerOptions options,
        IPushedAuthorizationService pushedAuthorizationService)
    {
        _handleGeneration = handleGeneration;
        _options = options;
        _pushedAuthorizationService = pushedAuthorizationService;
    }

    /// <inheritdoc />
    public async Task<PushedAuthorizationResponse> CreateResponseAsync(ValidatedPushedAuthorizationRequest request)
    {
        // Create a reference value
        var referenceValue = await _handleGeneration.GenerateAsync();

        var requestUri = $"{IdentityServerConstants.PushedAuthorizationRequestUri}:{referenceValue}";

        // Calculate the expiration
        var expiration = request.Client.PushedAuthorizationLifetime ?? _options.PushedAuthorization.Lifetime;
        var expiresAt = DateTime.UtcNow.AddSeconds(expiration);

        await _pushedAuthorizationService.StoreAsync(new DeserializedPushedAuthorizationRequest
        {
            ReferenceValue = referenceValue,
            ExpiresAtUtc = expiresAt,
            PushedParameters = request.Raw
        });

        // Return reference and expiration
        return new PushedAuthorizationSuccess
        {
            RequestUri = requestUri,
            ExpiresIn = expiration
        };
    }
}