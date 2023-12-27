using IdentityModel;
using IdentityServer4.Configuration.DependencyInjection;
using IdentityServer4.Models;
using IdentityServer4.Validation;
using System.Threading.Tasks;

namespace IdentityServer4.Services.Default;

/// <summary>
/// Decorator on the refresh token service to coordinate refresh token lifetimes and server-side sessions.
/// </summary>
internal class ServerSideSessionRefreshTokenService : IRefreshTokenService
{
    /// <summary>
    /// The inner IRefreshTokenService implementation.
    /// </summary>
    protected readonly IRefreshTokenService Inner;

    /// <summary>
    /// The session coordination service.
    /// </summary>
    protected readonly ISessionCoordinationService SessionCoordinationService;

    /// <summary>
    /// Initializes a new instance of the <see cref="DefaultRefreshTokenService" /> class.
    /// </summary>
    public ServerSideSessionRefreshTokenService(
        Decorator<IRefreshTokenService> inner,
        ISessionCoordinationService sessionCoordinationService)
    {
        Inner = inner.Instance;
        SessionCoordinationService = sessionCoordinationService;
    }

    private static readonly TokenValidationResult TokenValidationError = new()
    {
        IsError = true,
        Error = OidcConstants.TokenErrors.InvalidGrant
    };

    /// <inheritdoc/>
    public virtual async Task<TokenValidationResult> ValidateRefreshTokenAsync(string tokenHandle, Client client)
    {
        var result = await Inner.ValidateRefreshTokenAsync(tokenHandle, client);

        if (!result.IsError)
        {
            var valid = await SessionCoordinationService.ValidateSessionAsync(new SessionValidationRequest
            {
                SubjectId = result.RefreshToken.SubjectId,
                SessionId = result.RefreshToken.SessionId,
                Client = result.Client,
                Type = SessionValidationType.RefreshToken
            });

            if (!valid)
            {
                result = TokenValidationError;
            }
        }

        return result;
    }

    /// <inheritdoc/>
    public Task<string> CreateRefreshTokenAsync(RefreshTokenCreationRequest request)
    {
        return Inner.CreateRefreshTokenAsync(request);
    }

    /// <inheritdoc/>
    public Task<string> UpdateRefreshTokenAsync(RefreshTokenUpdateRequest request)
    {
        return Inner.UpdateRefreshTokenAsync(request);
    }
}