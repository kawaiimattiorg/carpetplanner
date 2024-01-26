using CarpetPlanner.Models;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Microsoft.AspNetCore.HttpOverrides;
using Microsoft.EntityFrameworkCore;
using Microsoft.Identity.Web;
using Microsoft.Identity.Web.UI;

var builder = WebApplication.CreateBuilder(args);

// ReSharper disable once InconsistentNaming
var b2cSection = builder.Configuration.GetSection("AzureAdB2C");
b2cSection["ClientSecret"] = Environment.GetEnvironmentVariable("CARPETPLANNER_B2C_CLIENTSECRET") ?? throw new Exception("ClientSecret not found");

// Add services to the container.
builder.Services.AddDbContext<CarpetDataContext>(options => options.UseNpgsql(builder.Configuration["PsqlConnectionString"]));

builder.Services.AddAuthentication(OpenIdConnectDefaults.AuthenticationScheme)
    .AddMicrosoftIdentityWebApp(b2cSection);
builder.Services.AddControllersWithViews();
builder.Services.AddRazorPages()
    .AddMicrosoftIdentityUI();

var app = builder.Build();

// Configure the HTTP request pipeline.
app.UseHttpsRedirection();

if (!app.Environment.IsDevelopment())
{
    app.UseForwardedHeaders(new ForwardedHeadersOptions
    {
        ForwardedHeaders = ForwardedHeaders.XForwardedFor | ForwardedHeaders.XForwardedProto
    });

    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseStaticFiles();

app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");
app.MapRazorPages();

app.Run();
