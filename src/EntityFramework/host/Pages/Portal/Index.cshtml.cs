using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace IdentityServerHost.Pages.Portal;

public class Index : PageModel
{
    private readonly ClientRepository _repository;
    public IEnumerable<ThirdPartyInitiatedLoginLink> Clients { get; private set; } = default!;

    public Index(ClientRepository repository)
    {
        _repository = repository;
    }

    public async Task OnGetAsync()
    {
        Clients = await _repository.GetClientsWithLoginUris();
    }
}