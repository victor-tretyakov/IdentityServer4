using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using System;
using Xunit;

namespace IdentityServer4.EntityFramework.Tests.DI;

public class DITests
{
    [Fact]
    public void AddConfigurationStore_on_empty_builder_should_not_throw()
    {
        var services = new ServiceCollection();
        services.AddIdentityServerBuilder()
            .AddConfigurationStore(options => options.ConfigureDbContext = b => b.UseInMemoryDatabase(Guid.NewGuid().ToString()));
    }
}