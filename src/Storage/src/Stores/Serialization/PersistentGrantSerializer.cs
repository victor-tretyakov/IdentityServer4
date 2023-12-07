// Copyright (c) Brock Allen & Dominick Baier. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.


using Microsoft.AspNetCore.DataProtection;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace IdentityServer4.Stores.Serialization;

/// <summary>
/// JSON-based persisted grant serializer
/// </summary>
/// <seealso cref="IPersistentGrantSerializer" />
public class PersistentGrantSerializer : IPersistentGrantSerializer
{
    private static readonly JsonSerializerOptions Settings;

    private readonly PersistentGrantOptions _options;
    private readonly IDataProtector _provider;

    static PersistentGrantSerializer()
    {
        Settings = new JsonSerializerOptions
        {
            IgnoreReadOnlyFields = true,
            IgnoreReadOnlyProperties = true,
            DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull,
        };

        Settings.Converters.Add(new ClaimConverter());
        Settings.Converters.Add(new ClaimsPrincipalConverter());
    }

    /// <summary>
    /// Ctor.
    /// </summary>
    /// <param name="options"></param>
    /// <param name="dataProtectionProvider"></param>
    public PersistentGrantSerializer(PersistentGrantOptions options = null, IDataProtectionProvider dataProtectionProvider = null)
    {
        _options = options;
        _provider = dataProtectionProvider?.CreateProtector(nameof(PersistentGrantSerializer));
    }

    bool ShouldDataProtect => _options?.DataProtectData == true && _provider != null;

    /// <summary>
    /// Serializes the specified value.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="value">The value.</param>
    /// <returns></returns>
    public string Serialize<T>(T value)
    {
        return JsonSerializer.Serialize(value, Settings);
    }

    /// <summary>
    /// Deserializes the specified string.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="json">The json.</param>
    /// <returns></returns>
    public T Deserialize<T>(string json)
    {
        return JsonSerializer.Deserialize<T>(json, Settings);
    }
}