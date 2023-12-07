using IdentityModel;
using IdentityServer4.Extensions;
using IdentityServer4.Logging.Models;
using IdentityServer4.Services;
using Microsoft.Extensions.Logging;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace IdentityServer4.Validation;

internal class RequestObjectValidator : IRequestObjectValidator
{
    private readonly IJwtRequestValidator _jwtRequestValidator;
    private readonly IJwtRequestUriHttpClient _jwtRequestUriHttpClient;
    private readonly IdentityServerOptions _options;
    private readonly ILogger<RequestObjectValidator> _logger;

    public RequestObjectValidator(
        IJwtRequestValidator jwtRequestValidator,
        IJwtRequestUriHttpClient jwtRequestUriHttpClient,
        IdentityServerOptions options,
        ILogger<RequestObjectValidator> logger)
    {
        _jwtRequestValidator = jwtRequestValidator;
        _jwtRequestUriHttpClient = jwtRequestUriHttpClient;
        _options = options;
        _logger = logger;
    }


    public async Task<AuthorizeRequestValidationResult> LoadRequestObjectAsync(ValidatedAuthorizeRequest request)
    {
        var requestObject = request.Raw.Get(OidcConstants.AuthorizeRequest.Request);
        var requestUri = request.Raw.Get(OidcConstants.AuthorizeRequest.RequestUri);

        if (requestObject.IsPresent() && requestUri.IsPresent())
        {
            LogError("Both request and request_uri are present", request);
            return Invalid(request, description: "Only one request parameter is allowed");
        }

        if (_options.Endpoints.EnableJwtRequestUri)
        {
            if (requestUri.IsPresent())
            {
                // 512 is from the spec
                if (requestUri.Length > 512)
                {
                    LogError("request_uri is too long", request);
                    return Invalid(request, error: OidcConstants.AuthorizeErrors.InvalidRequestUri, description: "request_uri is too long");
                }

                var jwt = await _jwtRequestUriHttpClient.GetJwtAsync(requestUri, request.Client);
                if (jwt.IsMissing())
                {
                    LogError("no value returned from request_uri", request);
                    return Invalid(request, error: OidcConstants.AuthorizeErrors.InvalidRequestUri, description: "no value returned from request_uri");
                }

                requestObject = jwt;
            }
        }
        else if (requestUri.IsPresent())
        {
            LogError("request_uri present but config prohibits", request);
            return Invalid(request, error: OidcConstants.AuthorizeErrors.RequestUriNotSupported);
        }

        // check length restrictions
        if (requestObject.IsPresent())
        {
            if (requestObject.Length >= _options.InputLengthRestrictions.Jwt)
            {
                LogError("request value is too long", request);
                return Invalid(request, error: OidcConstants.AuthorizeErrors.InvalidRequestObject, description: "Invalid request value");
            }
        }

        request.RequestObject = requestObject;
        return Valid(request);
    }

    public async Task<AuthorizeRequestValidationResult> ValidateRequestObjectAsync(ValidatedAuthorizeRequest request)
    {
        //////////////////////////////////////////////////////////
        // validate request object
        /////////////////////////////////////////////////////////
        if (request.RequestObject.IsPresent())
        {
            // validate the request JWT for this client
            var jwtRequestValidationResult = await _jwtRequestValidator.ValidateAsync(request.Client, request.RequestObject);
            if (jwtRequestValidationResult.IsError)
            {
                LogError("request JWT validation failure", request);
                return Invalid(request, error: OidcConstants.AuthorizeErrors.InvalidRequestObject, description: "Invalid JWT request");
            }

            if (jwtRequestValidationResult.Payload == null)
            {
                throw new Exception("JwtRequestValidation succeeded but did not return a payload");
            }

            // validate response_type match
            var responseType = request.Raw.Get(OidcConstants.AuthorizeRequest.ResponseType);
            if (responseType != null)
            {
                var payloadResponseType =
                    jwtRequestValidationResult.Payload.SingleOrDefault(c =>
                        c.Type == OidcConstants.AuthorizeRequest.ResponseType)?.Value;

                if (!string.IsNullOrEmpty(payloadResponseType))
                {
                    if (payloadResponseType != responseType)
                    {
                        LogError("response_type in JWT payload does not match response_type in request", request);
                        return Invalid(request, description: "Invalid JWT request");
                    }
                }
            }

            // validate client_id mismatch
            var payloadClientId =
                jwtRequestValidationResult.Payload.SingleOrDefault(c =>
                    c.Type == OidcConstants.AuthorizeRequest.ClientId)?.Value;

            if (!string.IsNullOrEmpty(payloadClientId))
            {
                if (!string.Equals(request.Client.ClientId, payloadClientId, StringComparison.Ordinal))
                {
                    LogError("client_id in JWT payload does not match client_id in request", request);
                    return Invalid(request, description: "Invalid JWT request");
                }
            }
            else
            {
                LogError("client_id is missing in JWT payload", request);
                return Invalid(request, error: OidcConstants.AuthorizeErrors.InvalidRequestObject, description: "Invalid JWT request");
            }

            var ignoreKeys = new[]
            {
                JwtClaimTypes.Issuer,
                JwtClaimTypes.Audience
            };

            /// merge jwt payload values into original request parameters
            // 1. clear the keys in the raw collection for every key found in the request object
            foreach (var claimType in jwtRequestValidationResult.Payload.Select(c => c.Type).Distinct())
            {
                var qsValue = request.Raw.Get(claimType);
                if (qsValue != null)
                {
                    request.Raw.Remove(claimType);
                }
            }

            // 2. copy over the value
            foreach (var claim in jwtRequestValidationResult.Payload)
            {
                request.Raw.Add(claim.Type, claim.Value);
            }

            request.RequestObjectValues = jwtRequestValidationResult.Payload;
        }

        return Valid(request);
    }


    private AuthorizeRequestValidationResult Invalid(ValidatedAuthorizeRequest request, string error = OidcConstants.AuthorizeErrors.InvalidRequest, string? description = null)
    {
        return new AuthorizeRequestValidationResult(request, error, description);
    }

    private AuthorizeRequestValidationResult Valid(ValidatedAuthorizeRequest request)
    {
        return new AuthorizeRequestValidationResult(request);
    }

    private void LogError(string message, ValidatedAuthorizeRequest request)
    {
        var requestDetails = new AuthorizeRequestValidationLog(request, _options.Logging.AuthorizeRequestSensitiveValuesFilter);
        _logger.LogError(message + "\n{@requestDetails}", requestDetails);
    }
}
