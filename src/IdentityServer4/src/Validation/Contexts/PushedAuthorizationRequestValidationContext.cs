using IdentityServer4.Models;
using System.Collections.Specialized;

namespace IdentityServer4.Validation;

/// <summary>
/// Context needed to validate a pushed authorization request.
/// </summary>
public class PushedAuthorizationRequestValidationContext
{
    /// <summary>
    /// Initializes an instance of the <see cref="PushedAuthorizationRequestValidationContext"/> class.
    /// </summary>
    /// <param name="requestParameters">The raw parameters that were passed to the PAR endpoint.</param>
    /// <param name="client">The client that made the request.</param>
    public PushedAuthorizationRequestValidationContext(NameValueCollection requestParameters, Client client)
    {
        RequestParameters = requestParameters;
        Client = client;
    }
    /// <summary>
    /// The request form parameters
    /// </summary>
    public NameValueCollection RequestParameters { get; set; }

    /// <summary>
    /// The validation result of client authentication
    /// </summary>
    public Client Client { get; set; }
}