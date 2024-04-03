using IdentityModel;
using IdentityServer4;
using IdentityServerHost.Configuration;
using IdentityServerHost.Extensions;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using System.Linq;
using System.Security.Cryptography.X509Certificates;

namespace IdentityServerHost;

internal static class IdentityServerExtensions
{
    internal static WebApplicationBuilder ConfigureIdentityServer(this WebApplicationBuilder builder)
    {
        var identityServer = builder.Services.AddIdentityServer(options =>
        {
            options.Events.RaiseSuccessEvents = true;
            options.Events.RaiseFailureEvents = true;
            options.Events.RaiseErrorEvents = true;
            options.Events.RaiseInformationEvents = true;

            options.EmitScopesAsSpaceDelimitedStringInJwt = true;
            options.Endpoints.EnableJwtRequestUri = true;

            options.ServerSideSessions.UserDisplayNameClaimType = JwtClaimTypes.Name;

            options.UserInteraction.CreateAccountUrl = "/Account/Create";

            options.Endpoints.EnablePushedAuthorizationEndpoint = true;
            options.PushedAuthorization.AllowUnregisteredPushedRedirectUris = true;
        })
            .AddServerSideSessions()
            .AddInMemoryClients(Clients.Get().ToList())
            .AddInMemoryIdentityResources(Resources.IdentityResources)
            .AddInMemoryApiScopes(Resources.ApiScopes)
            .AddInMemoryApiResources(Resources.ApiResources)
            //.AddStaticSigningCredential()
            .AddExtensionGrantValidator<ExtensionGrantValidator>()
            .AddExtensionGrantValidator<NoSubjectExtensionGrantValidator>()
            .AddJwtBearerClientAuthentication()
            .AddAppAuthRedirectUriValidator()
            .AddTestUsers(TestUsers.Users)
            .AddProfileService<HostProfileService>()
            .AddCustomTokenRequestValidator<ParameterizedScopeTokenRequestValidator>()
            .AddScopeParser<ParameterizedScopeParser>()
            .AddMutualTlsSecretValidators()
            .AddInMemoryOidcProviders(new[]
            {
                new IdentityServer4.Models.OidcProvider
                {
                    Scheme = "dynamicprovider-idsvr",
                    DisplayName = "IdentityServer (via Dynamic Providers)",
                    Authority = "https://demo.identityserver4.com",
                    ClientId = "login",
                    ResponseType = "id_token",
                    Scope = "openid profile"
                }
            });


        builder.Services.AddDistributedMemoryCache();

        return builder;
    }

    private static IIdentityServerBuilder AddStaticSigningCredential(this IIdentityServerBuilder builder)
    {
        // create random RS256 key
        //builder.AddDeveloperSigningCredential();

        // use an RSA-based certificate with RS256
        using var rsaCert = new X509Certificate2("./testkeys/identityserver.test.rsa.p12", "changeit");
        builder.AddSigningCredential(rsaCert, "RS256");

        // ...and PS256
        builder.AddSigningCredential(rsaCert, "PS256");

        // or manually extract ECDSA key from certificate (directly using the certificate is not support by Microsoft right now)
        using var ecCert = new X509Certificate2("./testkeys/identityserver.test.ecdsa.p12", "changeit");
        var key = new ECDsaSecurityKey(ecCert.GetECDsaPrivateKey())
        {
            KeyId = CryptoRandom.CreateUniqueId(16, CryptoRandom.OutputFormat.Hex)
        };

        return builder.AddSigningCredential(
            key,
            IdentityServerConstants.ECDsaSigningAlgorithm.ES256);
    }
}