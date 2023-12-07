// Copyright (c) Brock Allen & Dominick Baier. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.


using IdentityServer4.EntityFramework.Entities;
using IdentityServer4.Models;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;

namespace IdentityServer4.EntityFramework.Mappers;

/// <summary>
/// Extension methods to map to/from entity/model for clients.
/// </summary>
public static class ClientMappers
{
    /// <summary>
    /// Maps an entity to a model.
    /// </summary>
    /// <param name="entity">The entity.</param>
    /// <returns></returns>
    public static Models.Client ToModel(this Entities.Client entity)
    {
        return new Models.Client
        {
            Enabled = entity.Enabled,
            ClientId = entity.ClientId,
            ProtocolType = entity.ProtocolType,
            ClientSecrets = entity.ClientSecrets?.Select(s => new Models.Secret
            {
                Value = s.Value,
                Type = s.Type,
                Description = s.Description,
                Expiration = s.Expiration
            }).ToList() ?? new List<Models.Secret>(),
            RequireClientSecret = entity.RequireClientSecret,
            ClientName = entity.ClientName,
            Description = entity.Description,
            ClientUri = entity.ClientUri,
            LogoUri = entity.LogoUri,
            RequireConsent = entity.RequireConsent,
            AllowRememberConsent = entity.AllowRememberConsent,
            AlwaysIncludeUserClaimsInIdToken = entity.AlwaysIncludeUserClaimsInIdToken,
            AllowedGrantTypes = entity.AllowedGrantTypes?.Select(t => t.GrantType).ToList() ?? new List<string>(),
            RequirePkce = entity.RequirePkce,
            AllowPlainTextPkce = entity.AllowPlainTextPkce,
            RequireRequestObject = entity.RequireRequestObject,
            AllowAccessTokensViaBrowser = entity.AllowAccessTokensViaBrowser,
            RedirectUris = entity.RedirectUris?.Select(uri => uri.RedirectUri).ToList() ?? new List<string>(),
            PostLogoutRedirectUris = entity.PostLogoutRedirectUris?.Select(uri => uri.PostLogoutRedirectUri).ToList() ?? new List<string>(),
            FrontChannelLogoutUri = entity.FrontChannelLogoutUri,
            FrontChannelLogoutSessionRequired = entity.FrontChannelLogoutSessionRequired,
            BackChannelLogoutUri = entity.BackChannelLogoutUri,
            BackChannelLogoutSessionRequired = entity.BackChannelLogoutSessionRequired,
            AllowOfflineAccess = entity.AllowOfflineAccess,
            AllowedScopes = entity.AllowedScopes?.Select(s => s.Scope).ToList() ?? new List<string>(),
            IdentityTokenLifetime = entity.IdentityTokenLifetime,
            AllowedIdentityTokenSigningAlgorithms = AllowedSigningAlgorithmsConverter.Convert(entity.AllowedIdentityTokenSigningAlgorithms),
            AccessTokenLifetime = entity.AccessTokenLifetime,
            AuthorizationCodeLifetime = entity.AuthorizationCodeLifetime,
            ConsentLifetime = entity.ConsentLifetime,
            AbsoluteRefreshTokenLifetime = entity.AbsoluteRefreshTokenLifetime,
            SlidingRefreshTokenLifetime = entity.SlidingRefreshTokenLifetime,
            RefreshTokenUsage = (TokenUsage) entity.RefreshTokenUsage,
            UpdateAccessTokenClaimsOnRefresh = entity.UpdateAccessTokenClaimsOnRefresh,
            RefreshTokenExpiration = (TokenExpiration) entity.RefreshTokenExpiration,
            AccessTokenType = (AccessTokenType) entity.AccessTokenType,
            EnableLocalLogin = entity.EnableLocalLogin,
            IdentityProviderRestrictions = entity.IdentityProviderRestrictions?.Select(r => r.Provider).ToList() ?? new List<string>(),
            IncludeJwtId = entity.IncludeJwtId,
            Claims = entity.Claims?.Select(c => new Models.ClientClaim
            {
                Type = c.Type,
                Value = c.Value,
                ValueType = ClaimValueTypes.String
            }).ToList() ?? new List<Models.ClientClaim>(),
            AlwaysSendClientClaims = entity.AlwaysSendClientClaims,
            ClientClaimsPrefix = entity.ClientClaimsPrefix,
            PairWiseSubjectSalt = entity.PairWiseSubjectSalt,
            AllowedCorsOrigins = entity.AllowedCorsOrigins?.Select(o => o.Origin).ToList() ?? new List<string>(),
            Properties = entity.Properties?.ToDictionary(p => p.Key, p => p.Value) ?? new Dictionary<string, string>(),
            UserSsoLifetime = entity.UserSsoLifetime,
            UserCodeType = entity.UserCodeType,
            DeviceCodeLifetime = entity.DeviceCodeLifetime,
        };
    }

    /// <summary>
    /// Maps a model to an entity.
    /// </summary>
    /// <param name="model">The model.</param>
    /// <returns></returns>
    public static Entities.Client ToEntity(this Models.Client model)
    {
        return new Entities.Client
        {
            Enabled = model.Enabled,
            ClientId = model.ClientId,
            ProtocolType = model.ProtocolType,
            ClientSecrets = model.ClientSecrets?.Select(s => new ClientSecret
            {
                Value = s.Value,
                Type = s.Type,
                Description = s.Description,
                Expiration = s.Expiration
            }).ToList() ?? new List<ClientSecret>(),
            RequireClientSecret = model.RequireClientSecret,
            ClientName = model.ClientName,
            Description = model.Description,
            ClientUri = model.ClientUri,
            LogoUri = model.LogoUri,
            RequireConsent = model.RequireConsent,
            AllowRememberConsent = model.AllowRememberConsent,
            AlwaysIncludeUserClaimsInIdToken = model.AlwaysIncludeUserClaimsInIdToken,
            AllowedGrantTypes = model.AllowedGrantTypes?.Select(t => new ClientGrantType
            {
                GrantType = t
            }).ToList() ?? new List<ClientGrantType>(),
            RequirePkce = model.RequirePkce,
            AllowPlainTextPkce = model.AllowPlainTextPkce,
            RequireRequestObject = model.RequireRequestObject,
            AllowAccessTokensViaBrowser = model.AllowAccessTokensViaBrowser,
            RedirectUris = model.RedirectUris?.Select(uri => new ClientRedirectUri
            {
                RedirectUri = uri
            }).ToList() ?? new List<ClientRedirectUri>(),
            PostLogoutRedirectUris = model.PostLogoutRedirectUris?.Select(uri => new ClientPostLogoutRedirectUri
            {
                PostLogoutRedirectUri = uri
            }).ToList() ?? new List<ClientPostLogoutRedirectUri>(),
            FrontChannelLogoutUri = model.FrontChannelLogoutUri,
            FrontChannelLogoutSessionRequired = model.FrontChannelLogoutSessionRequired,
            BackChannelLogoutUri = model.BackChannelLogoutUri,
            BackChannelLogoutSessionRequired = model.BackChannelLogoutSessionRequired,
            AllowOfflineAccess = model.AllowOfflineAccess,
            AllowedScopes = model.AllowedScopes?.Select(s => new ClientScope
            {
                Scope = s
            }).ToList() ?? new List<ClientScope>(),
            IdentityTokenLifetime = model.IdentityTokenLifetime,
            AllowedIdentityTokenSigningAlgorithms = AllowedSigningAlgorithmsConverter.Convert(model.AllowedIdentityTokenSigningAlgorithms),
            AccessTokenLifetime = model.AccessTokenLifetime,
            AuthorizationCodeLifetime = model.AuthorizationCodeLifetime,
            ConsentLifetime = model.ConsentLifetime,
            AbsoluteRefreshTokenLifetime = model.AbsoluteRefreshTokenLifetime,
            SlidingRefreshTokenLifetime = model.SlidingRefreshTokenLifetime,
            RefreshTokenUsage = (int) model.RefreshTokenUsage,
            UpdateAccessTokenClaimsOnRefresh = model.UpdateAccessTokenClaimsOnRefresh,
            RefreshTokenExpiration = (int) model.RefreshTokenExpiration,
            AccessTokenType = (int) model.AccessTokenType,
            EnableLocalLogin = model.EnableLocalLogin,
            IdentityProviderRestrictions = model.IdentityProviderRestrictions?.Select(r => new ClientIdPRestriction
            {
                Provider = r
            }).ToList() ?? new List<ClientIdPRestriction>(),
            IncludeJwtId = model.IncludeJwtId,
            Claims = model.Claims?.Select(c => new Entities.ClientClaim
            {
                Type = c.Type,
                Value = c.Value,
            }).ToList() ?? new List<Entities.ClientClaim>(),
            AlwaysSendClientClaims = model.AlwaysSendClientClaims,
            ClientClaimsPrefix = model.ClientClaimsPrefix,
            PairWiseSubjectSalt = model.PairWiseSubjectSalt,
            AllowedCorsOrigins = model.AllowedCorsOrigins?.Select(o => new ClientCorsOrigin
            {
                Origin = o
            }).ToList() ?? new List<ClientCorsOrigin>(),
            Properties = model.Properties?.Select(pair => new ClientProperty
            {
                Key = pair.Key,
                Value = pair.Value,
            }).ToList() ?? new List<ClientProperty>(),
            UserSsoLifetime = model.UserSsoLifetime,
            UserCodeType = model.UserCodeType,
            DeviceCodeLifetime = model.DeviceCodeLifetime,
        };
    }
}