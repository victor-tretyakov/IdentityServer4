using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Linq;
using System.Reflection;

namespace IdentityServerHost.Pages.Home;

[AllowAnonymous]
public class Index : PageModel
{
    public Index()
    {
    }

    public string Version
    {
        get => typeof(IdentityServer4.Hosting.IdentityServerMiddleware).Assembly
            .GetCustomAttribute<AssemblyInformationalVersionAttribute>()
            ?.InformationalVersion.Split('+').First()
            ?? "unavailable";
    }
}