using IdentityServer4.Extensions;
using IdentityServer4.Hosting;
using IdentityServer4.ResponseHandling;
using Microsoft.AspNetCore.Http;
using System;
using System.Net;
using System.Threading.Tasks;

namespace IdentityServer4.Endpoints.Results;

/// <summary>
/// Represents a successful result from the pushed authorization endpoint that can be written to the http response.
/// </summary>
public class PushedAuthorizationResult : EndpointResult<PushedAuthorizationResult>
{
    /// <summary>
    /// The successful response model.
    /// </summary>
    public PushedAuthorizationSuccess Response { get; }

    /// <summary>
    /// Initializes a new instance of the <see cref="PushedAuthorizationResult"/> class.
    /// </summary>
    /// <param name="response">The successful response model.</param>
    public PushedAuthorizationResult(PushedAuthorizationSuccess response)
    {
        Response = response ?? throw new ArgumentNullException(nameof(response));
    }
}

internal class PushedAuthorizationHttpWriter : IHttpResponseWriter<PushedAuthorizationResult>
{
    public async Task WriteHttpResponse(PushedAuthorizationResult result, HttpContext context)
    {
        context.Response.SetNoCache();
        context.Response.StatusCode = (int) HttpStatusCode.Created;
        var dto = new ResultDto
        {
            request_uri = result.Response.RequestUri,
            expires_in = result.Response.ExpiresIn
        };
        await context.Response.WriteJsonAsync(dto);
    }

    internal class ResultDto
    {
        public required string request_uri { get; set; }
        public required int expires_in { get; set; }
    }
}