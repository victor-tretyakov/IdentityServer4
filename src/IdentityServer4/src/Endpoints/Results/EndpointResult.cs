using IdentityServer4.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Threading.Tasks;

namespace IdentityServer4.Endpoints.Results;

/// <summary>
/// Provides the base implementation of <see cref="IEndpointResult"/> that
/// invokes the corresponding <see cref="IHttpResponseWriter{T}"/> to write the
/// result as an http response.
/// </summary>
/// <typeparam name="T"></typeparam>
public abstract class EndpointResult<T> : IEndpointResult
    where T : class, IEndpointResult
{
    /// <inheritdoc/>
    public async Task ExecuteAsync(HttpContext context)
    {
        var writer = context.RequestServices.GetService<IHttpResponseWriter<T>>();
        if (writer != null)
        {
            var target = this as T ?? throw new Exception($"Type parameter {typeof(T)} must be the class derived from 'EndpointResult<T>'.");
            await writer.WriteHttpResponse(target, context);
        }
        else
        {
            throw new Exception($"No IEndpointResultGenerator<T> registered for IEndpointResult type '{typeof(T)}'.");
        }
    }
}