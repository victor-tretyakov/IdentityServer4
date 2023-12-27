using IdentityServer4.Models;
using IdentityServer4.Test;
using Microsoft.Extensions.Logging;
using System;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace IdentityServerHost.Extensions;

public class HostProfileService : TestUserProfileService
{
    public HostProfileService(TestUserStore users, ILogger<TestUserProfileService> logger) : base(users, logger)
    {
    }

    public override async Task GetProfileDataAsync(ProfileDataRequestContext context)
    {
        ArgumentNullException.ThrowIfNull(context);
        await base.GetProfileDataAsync(context);

        var transaction = context.RequestedResources.ParsedScopes.FirstOrDefault(x => x.ParsedName == "transaction");
        if (transaction?.ParsedParameter != null)
        {
            context.IssuedClaims.Add(new Claim("transaction_id", transaction.ParsedParameter));
        }
    }
}