using IdentityServer4.Models;
using IdentityServer4.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace IdentityServerHost.Pages.Ciba;

[SecurityHeaders]
[Authorize]
public class AllModel : PageModel
{
    public IEnumerable<BackchannelUserLoginRequest> Logins { get; set; } = default!;

    private readonly IBackchannelAuthenticationInteractionService _backchannelAuthenticationInteraction;

    public AllModel(IBackchannelAuthenticationInteractionService backchannelAuthenticationInteractionService)
    {
        _backchannelAuthenticationInteraction = backchannelAuthenticationInteractionService;
    }

    public async Task OnGet()
    {
        Logins = await _backchannelAuthenticationInteraction.GetPendingLoginRequestsForCurrentUserAsync();
    }
}