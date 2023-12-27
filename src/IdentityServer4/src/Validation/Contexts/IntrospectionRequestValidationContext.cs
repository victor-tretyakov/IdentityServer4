using IdentityServer4.Models;
using System.Collections.Specialized;

namespace IdentityServer4.Validation;

/// <summary>
/// Context for validating an introspection request.
/// </summary>
public class IntrospectionRequestValidationContext
{
    /// <summary>
    /// The request parameters 
    /// </summary>
    public NameValueCollection Parameters { get; set; } = default!;

    /// <summary>
    /// The ApiResource that is making the request
    /// </summary>
    public ApiResource? Api { get; set; }

    /// <summary>
    /// The Client that is making the request
    /// </summary>
    public Client? Client { get; set; }
}