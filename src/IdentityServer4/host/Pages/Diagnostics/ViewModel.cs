using IdentityModel;
using Microsoft.AspNetCore.Authentication;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Text;

namespace IdentityServerHost.Pages.Diagnostics;

public class ViewModel
{
    public ViewModel(AuthenticateResult result)
    {
        AuthenticateResult = result;

        if (result?.Properties?.Items.TryGetValue("client_list", out var encoded) == true)
        {
            if (encoded != null)
            {
                var bytes = Base64Url.Decode(encoded);
                var value = Encoding.UTF8.GetString(bytes);
                Clients = JsonSerializer.Deserialize<string[]>(value) ?? Enumerable.Empty<string>();
                return;
            }
        }
        Clients = Enumerable.Empty<string>();
    }

    public AuthenticateResult AuthenticateResult { get; }
    public IEnumerable<string> Clients { get; }
}