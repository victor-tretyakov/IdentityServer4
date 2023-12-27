using IdentityServer4.Extensions;
using Microsoft.AspNetCore.DataProtection;
using System.Collections.Generic;
using System.Collections.Specialized;

namespace IdentityServer4.Services;

/// <summary>
/// Default implementation of <see cref="IPushedAuthorizationSerializer"/>.
/// </summary>
public class PushedAuthorizationSerializer : IPushedAuthorizationSerializer
{
    private readonly IDataProtector _dataProtector;

    /// <summary>
    /// Initializes a new instance of the <see cref="PushedAuthorizationSerializer"/>. 
    /// </summary>
    /// <param name="dataProtectionProvider">The data protection provider.</param>
    public PushedAuthorizationSerializer(IDataProtectionProvider dataProtectionProvider)
    {
        _dataProtector = dataProtectionProvider.CreateProtector("PAR");
    }

    /// <inheritdoc />
    public NameValueCollection Deserialize(string raw)
    {
        var unprotected = _dataProtector.Unprotect(raw);
        return ObjectSerializer
            .FromString<Dictionary<string, string[]>>(unprotected)
            .FromFullDictionary();
    }

    /// <inheritdoc />
    public string Serialize(NameValueCollection raw)
    {
        // Serialize
        var serialized = ObjectSerializer.ToString(raw.ToFullDictionary());

        // Data Protect
        var protectedData = _dataProtector.Protect(serialized);
        return protectedData;
    }
}