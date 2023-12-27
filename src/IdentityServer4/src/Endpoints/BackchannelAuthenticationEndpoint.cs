using IdentityModel;
using IdentityServer4.Configuration;
using IdentityServer4.Endpoints.Results;
using IdentityServer4.Events;
using IdentityServer4.Extensions;
using IdentityServer4.Hosting;
using IdentityServer4.ResponseHandling;
using IdentityServer4.Services;
using IdentityServer4.Validation;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using System.IO;
using System.Threading.Tasks;

namespace IdentityServer4.Endpoints;

internal class BackchannelAuthenticationEndpoint : IEndpointHandler
{
    private readonly IClientSecretValidator _clientValidator;
    private readonly IBackchannelAuthenticationRequestValidator _requestValidator;
    private readonly IBackchannelAuthenticationResponseGenerator _responseGenerator;
    private readonly IEventService _events;
    private readonly ILogger<BackchannelAuthenticationEndpoint> _logger;

    public BackchannelAuthenticationEndpoint(
        IClientSecretValidator clientValidator,
        IBackchannelAuthenticationRequestValidator requestValidator,
        IBackchannelAuthenticationResponseGenerator responseGenerator,
        IEventService events,
        ILogger<BackchannelAuthenticationEndpoint> logger)
    {
        _clientValidator = clientValidator;
        _requestValidator = requestValidator;
        _responseGenerator = responseGenerator;
        _events = events;
        _logger = logger;
    }

    public async Task<IEndpointResult> ProcessAsync(HttpContext context)
    {
        _logger.LogTrace("Processing backchannel authentication request.");

        // validate HTTP
        if (!HttpMethods.IsPost(context.Request.Method) || !context.Request.HasApplicationFormContentType())
        {
            _logger.LogWarning("Invalid HTTP request for backchannel authentication endpoint");
            return Error(OidcConstants.BackchannelAuthenticationRequestErrors.InvalidRequest);
        }

        try
        {
            return await ProcessAuthenticationRequestAsync(context);
        }
        catch (InvalidDataException ex)
        {
            _logger.LogWarning(ex, "Invalid HTTP request for backchannel authentication endpoint");
            return Error(OidcConstants.BackchannelAuthenticationRequestErrors.InvalidRequest);
        }
    }

    private async Task<IEndpointResult> ProcessAuthenticationRequestAsync(HttpContext context)
    {
        _logger.LogDebug("Start backchannel authentication request.");

        // validate client
        var clientResult = await _clientValidator.ValidateAsync(context);
        if (clientResult.IsError)
        {
            var error = clientResult.Error ?? OidcConstants.BackchannelAuthenticationRequestErrors.InvalidClient;
            return Error(error);
        }

        // validate request
        var form = (await context.Request.ReadFormAsync()).AsNameValueCollection();
        _logger.LogTrace("Calling into backchannel authentication request validator: {type}", _requestValidator.GetType().FullName);
        var requestResult = await _requestValidator.ValidateRequestAsync(form, clientResult);

        if (requestResult.IsError)
        {
            await _events.RaiseAsync(new BackchannelAuthenticationFailureEvent(requestResult));
            return Error(requestResult.Error, requestResult.ErrorDescription);
        }

        // create response
        _logger.LogTrace("Calling into backchannel authentication request response generator: {type}", _responseGenerator.GetType().FullName);
        var response = await _responseGenerator.ProcessAsync(requestResult);

        await _events.RaiseAsync(new BackchannelAuthenticationSuccessEvent(requestResult));
        LogResponse(response, requestResult);

        // return result
        _logger.LogDebug("Backchannel authentication request success.");
        return new BackchannelAuthenticationResult(response);
    }

    private void LogResponse(BackchannelAuthenticationResponse response, BackchannelAuthenticationRequestValidationResult requestResult)
    {
        _logger.LogTrace("BackchannelAuthenticationResponse: {@response} for subject {subjectId}", response, requestResult.ValidatedRequest.Subject.GetSubjectId());
    }

    BackchannelAuthenticationResult Error(string error, string errorDescription = null)
    {
        return new BackchannelAuthenticationResult(new BackchannelAuthenticationResponse(error, errorDescription));
    }
}