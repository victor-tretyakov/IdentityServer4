using System.Security.Claims;

namespace IdentityServer4.Validation;

/// <summary>
/// Represents the result of a backchannel authentication request.
/// </summary>
public class BackchannelAuthenticationUserValidationResult
{
    /// <summary>
    /// Indicates if this represents an error.
    /// </summary>
    public bool IsError => !string.IsNullOrWhiteSpace(Error);

    /// <summary>
    /// Gets or sets the error.
    /// </summary>
    public string? Error { get; set; }

    /// <summary>
    /// Gets or sets the error description.
    /// </summary>
    public string? ErrorDescription { get; set; }

    /// <summary>
    /// Gets or sets the subject based upon the provided hint.
    /// </summary>
    public ClaimsPrincipal? Subject { get; set; }
}