namespace IdentityServer4.Models;

/// <summary>
/// Models the reason the user's session was ended.
/// </summary>
public enum LogoutNotificationReason
{
    /// <summary>
    /// The user interactively triggered logout.
    /// </summary>
    UserLogout,
    /// <summary>
    /// The user's session expired due to inactivity.
    /// </summary>
    SessionExpiration,
    /// <summary>
    /// The user's session was explicitly terminated by some other means (e.g. an admin)
    /// </summary>
    Terminated,
}