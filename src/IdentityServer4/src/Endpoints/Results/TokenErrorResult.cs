// Copyright (c) Brock Allen & Dominick Baier. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.


using IdentityServer4.Extensions;
using IdentityServer4.Hosting;
using IdentityServer4.ResponseHandling;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace IdentityServer4.Endpoints.Results;

/// <summary>
/// Models a token error result
/// </summary>
public class TokenErrorResult : EndpointResult<TokenErrorResult>
{
    /// <summary>
    /// The response
    /// </summary>
    public TokenErrorResponse Response { get; }

    /// <summary>
    /// Ctor
    /// </summary>
    /// <param name="error"></param>
    /// <exception cref="ArgumentNullException"></exception>
    public TokenErrorResult(TokenErrorResponse error)
    {
        if (error.Error.IsMissing()) throw new ArgumentNullException(nameof(error.Error), "Error must be set");

        Response = error;
    }
}

internal class TokenErrorHttpWriter : IHttpResponseWriter<TokenErrorResult>
{
    public async Task WriteHttpResponse(TokenErrorResult result, HttpContext context)
    {
        context.Response.StatusCode = 400;
        context.Response.SetNoCache();

        var dto = new ResultDto
        {
            error = result.Response.Error,
            error_description = result.Response.ErrorDescription,

            custom = result.Response.Custom
        };

        await context.Response.WriteJsonAsync(dto);
    }

    internal class ResultDto
    {
        public string error { get; set; }
        public string error_description { get; set; }

        [JsonExtensionData]
        public Dictionary<string, object> custom { get; set; }
    }
}