namespace IdentityServer4.Stores.Serialization;

/// <summary>
/// Options for how persisted grants are persisted.
/// </summary>
public class PersistentGrantOptions
{
    /// <summary>
    /// Data protect the persisted grants "data" column.
    /// </summary>
    public bool DataProtectData { get; set; } = true;

    /// <summary>
    /// Delete one time only refresh tokens when they are used to obtain a new
    /// token. If false, one time only refresh tokens will instead be marked as
    /// Consumed.
    /// </summary>
    public bool DeleteOneTimeOnlyRefreshTokensOnUse { get; set; } = true;
}
