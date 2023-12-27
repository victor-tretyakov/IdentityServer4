namespace IdentityServer4.Stores;

/// <summary>
/// Marker interface to indicate if server side sessions enabled in DI.
/// </summary>
public interface IServerSideSessionsMarker { }

/// <summary>
/// Nop implementation for IServerSideSessionsMarker.
/// </summary>
public class NopIServerSideSessionsMarker : IServerSideSessionsMarker { }