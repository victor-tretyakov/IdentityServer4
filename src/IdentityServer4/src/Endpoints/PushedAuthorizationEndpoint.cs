using IdentityModel;
using IdentityServer4.Configuration;
using IdentityServer4.Endpoints.Results;
using IdentityServer4.Hosting;
using IdentityServer4.Logging.Models;
using IdentityServer4.ResponseHandling;
using IdentityServer4.Validation;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using System.Collections.Specialized;
using System.Net;
using System.Threading.Tasks;
using System;
using IdentityServer4.Extensions;

namespace IdentityServer4.Endpoints;

internal class PushedAuthorizationEndpoint : IEndpointHandler
{
    private readonly IClientSecretValidator _clientValidator;
    private readonly IPushedAuthorizationRequestValidator _parValidator;
    private readonly IPushedAuthorizationResponseGenerator _responseGenerator;
    private readonly IdentityServerOptions _options;
    private readonly ILogger<PushedAuthorizationEndpoint> _logger;

    public PushedAuthorizationEndpoint(
        IClientSecretValidator clientValidator,
        IPushedAuthorizationRequestValidator parValidator,
        IPushedAuthorizationResponseGenerator responseGenerator,
        IdentityServerOptions options,
        ILogger<PushedAuthorizationEndpoint> logger
        )
    {
        _clientValidator = clientValidator;
        _parValidator = parValidator;
        _responseGenerator = responseGenerator;
        _options = options;
        _logger = logger;
    }

    public async Task<IEndpointResult> ProcessAsync(HttpContext context)
    {
        _logger.LogDebug("Start pushed authorization request");

        NameValueCollection values;
        IFormCollection form;
        if (HttpMethods.IsPost(context.Request.Method))
        {
            form = await context.Request.ReadFormAsync();
            values = form.AsNameValueCollection();
        }
        else
        {
            return new StatusCodeResult(HttpStatusCode.MethodNotAllowed);
        }

        // Authenticate Client
        var client = await _clientValidator.ValidateAsync(context);
        if (client.IsError)
        {
            return CreateErrorResult(
                logMessage: "Client secret validation failed",
                request: null,
                client.Error,
                client.ErrorDescription);
        }

        // Perform validations specific to PAR, as well as validation of the pushed parameters
        var parValidationResult = await _parValidator.ValidateAsync(new PushedAuthorizationRequestValidationContext(values, client.Client));
        if (parValidationResult.IsError)
        {
            return CreateErrorResult(
                logMessage: "Pushed authorization validation failed",
                request: parValidationResult.ValidatedRequest,
                parValidationResult.Error,
                parValidationResult.ErrorDescription);
        }

        var response = await _responseGenerator.CreateResponseAsync(parValidationResult.ValidatedRequest);

        switch (response)
        {
            case PushedAuthorizationSuccess success:
                return new PushedAuthorizationResult(success);
            case PushedAuthorizationFailure fail:
                return new PushedAuthorizationErrorResult(fail);
            default:
                throw new Exception("Unexpected pushed authorization response. The result of the pushed authorization response generator should be either a PushedAuthorizationSuccess or PushedAuthorizationFailure.");
        }
    }

    private PushedAuthorizationErrorResult CreateErrorResult(
        string logMessage,
        ValidatedPushedAuthorizationRequest request = null,
        string error = OidcConstants.AuthorizeErrors.ServerError,
        string errorDescription = null,
        bool logError = true)
    {
        if (logError)
        {
            _logger.LogError(logMessage);
        }

        if (request != null)
        {
            var details = new AuthorizeRequestValidationLog(request, _options.Logging.PushedAuthorizationSensitiveValuesFilter);
            _logger.LogInformation("{@validationDetails}", details);
        }

        return new PushedAuthorizationErrorResult(new PushedAuthorizationFailure
        {
            Error = error,
            ErrorDescription = errorDescription,
        });
    }
}