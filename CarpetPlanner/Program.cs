using System.Net;
using CarpetPlanner.Models;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Microsoft.AspNetCore.HttpOverrides;
using Microsoft.EntityFrameworkCore;
using Microsoft.Identity.Web;
using Microsoft.Identity.Web.UI;

var builder = WebApplication.CreateBuilder(args);

builder.Services.Configure<ForwardedHeadersOptions>(options =>
{
    options.ForwardedHeaders = ForwardedHeaders.XForwardedFor | ForwardedHeaders.XForwardedProto;
    options.ForwardLimit = 2;

    var knownProxy = builder.Configuration["KnownProxy"];
    if (!string.IsNullOrEmpty(knownProxy))
    {
        options.KnownProxies.Add(IPAddress.Parse(knownProxy));
        Console.WriteLine($"Adding proxy:{knownProxy}");
    }
});

builder.Services.Configure<CookiePolicyOptions>(options =>
{
    options.MinimumSameSitePolicy = SameSiteMode.Lax;
});

// Add services to the container.
builder.Services.AddDbContext<CarpetDataContext>(options => options.UseNpgsql(builder.Configuration["PsqlConnectionString"]));

builder.Services.AddMicrosoftIdentityWebAppAuthentication(builder.Configuration, Constants.AzureAdB2C);

// Without this you'll get following error:
// IDW10303: Issuer: 'https://carpetplanner.b2clogin.com/7663a1b0-dea2-4b60-98d8-9828d143d30d/v2.0/', does not match any of the valid issuers provided for this application.
builder.Services.Configure<OpenIdConnectOptions>(OpenIdConnectDefaults.AuthenticationScheme, options =>
{
    options.TokenValidationParameters.ValidIssuer = "https://carpetplanner.b2clogin.com/7663a1b0-dea2-4b60-98d8-9828d143d30d/v2.0/";
});

builder.Services.AddControllersWithViews();
builder.Services.AddRazorPages()
    .AddMicrosoftIdentityUI();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    // Https enforcing in production is done using nginx
    app.UseHttpsRedirection();
}
else
{
    app.UseExceptionHandler("/Home/Error");
    app.UseForwardedHeaders();
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseStaticFiles();

app.UseCookiePolicy();

app.UseRouting();
app.UseAuthentication();
app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");
app.MapRazorPages();

app.Run();
