namespace IdentityServer4.Models;

/// <summary>
/// Models the data to update a refresh token.
/// </summary>
public class RefreshTokenUpdateRequest
{
    /// <summary>
    /// The handle of the refresh token.
    /// </summary>
    public string Handle { get; set; } = default!;

    /// <summary>
    /// The client.
    /// </summary>
    public Client Client { get; set; } = default!;

    /// <summary>
    /// The refresh token to update.
    /// </summary>
    public RefreshToken RefreshToken { get; set; } = default!;

    /// <summary>
    /// Flag to indicate that the refreth token was modified, and requires an update to the database.
    /// </summary>
    public bool MustUpdate { get; set; }
}