// Copyright (c) Brock Allen & Dominick Baier. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.


using IdentityServer4;
using IdentityServer4.Models;
using IdentityServerHost.Data;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Serilog;
using Serilog.Events;

namespace IdentityServerHost;

internal static class HostingExtensions
{
    internal static WebApplication ConfigureServices(this WebApplicationBuilder builder)
    {
        builder.Services.AddControllersWithViews();

        builder.Services.AddDbContext<ApplicationDbContext>(options =>
            options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

        builder.Services.AddIdentity<ApplicationUser, IdentityRole>()
            .AddEntityFrameworkStores<ApplicationDbContext>()
            .AddDefaultTokenProviders();

        builder.ConfigureIdentityServer();
        builder.AddExternalIdentityProviders();

        return builder.Build();
    }

    private static void AddExternalIdentityProviders(this WebApplicationBuilder builder)
    {
        // configures the OpenIdConnect handlers to persist the state parameter into the server-side IDistributedCache.
        builder.Services.AddAuthentication()
            .AddOpenIdConnect("Google", "Google", options =>
            {
                options.SignInScheme = IdentityServerConstants.ExternalCookieAuthenticationScheme;
                options.ForwardSignOut = IdentityServerConstants.DefaultCookieAuthenticationScheme;

                options.Authority = "https://accounts.google.com/";
                options.ClientId = "708996912208-9m4dkjb5hscn7cjrn5u0r4tbgkbj1fko.apps.googleusercontent.com";

                options.CallbackPath = "/signin-google";
                options.Scope.Add("email");
            });
    }


    internal static WebApplication ConfigurePipeline(this WebApplication app)
    {
        app.UseSerilogRequestLogging(
            options => options.GetLevel = (httpContext, elapsed, ex) => LogEventLevel.Debug);

        app.UseDeveloperExceptionPage();
        app.UseStaticFiles();

        app.UseRouting();
        app.UseIdentityServer();
        app.UseAuthorization();

        // UI
        app.MapDefaultControllerRoute()
            .RequireAuthorization();

        return app;
    }
}