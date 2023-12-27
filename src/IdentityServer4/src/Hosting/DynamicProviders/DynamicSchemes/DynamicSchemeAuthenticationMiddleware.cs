using IdentityServer4.Configuration;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using System.Threading.Tasks;

namespace IdentityServer4.Hosting.DynamicProviders;

internal class DynamicSchemeAuthenticationMiddleware
{
    private readonly RequestDelegate _next;
    private readonly DynamicProviderOptions _options;

    public DynamicSchemeAuthenticationMiddleware(RequestDelegate next, DynamicProviderOptions options)
    {
        _next = next;
        _options = options;
    }

    public async Task Invoke(HttpContext context)
    {
        // this is needed to dynamically load the handler if this load balanced server
        // was not the one that initiated the call out to the provider
        if (context.Request.Path.StartsWithSegments(_options.PathPrefix))
        {
            var startIndex = _options.PathPrefix.ToString().Length;
            if (context.Request.Path.Value.Length > startIndex)
            {
                var scheme = context.Request.Path.Value[(startIndex + 1)..];
                var idx = scheme.IndexOf('/');
                if (idx > 0)
                {
                    // this assumes the path is: /<PathPrefix>/<scheme>/<extra>
                    // e.g.: /federation/my-oidc-provider/signin
                    scheme = scheme[..idx];
                }

                var handlers = context.RequestServices.GetRequiredService<IAuthenticationHandlerProvider>();

                if (await handlers.GetHandlerAsync(context, scheme) is IAuthenticationRequestHandler handler && await handler.HandleRequestAsync())
                {
                    return;
                }
            }
        }

        await _next(context);
    }
}