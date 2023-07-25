using LabWebApp.Data;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.AspNetCore.Authentication;
using NuGet.Packaging;
using System.Security.Claims;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection") ?? throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(connectionString));
builder.Services.AddDatabaseDeveloperPageExceptionFilter();


builder.Services.AddDefaultIdentity<IdentityUser>(options => options.SignIn.RequireConfirmedAccount = true)
    .AddEntityFrameworkStores<ApplicationDbContext>();
builder.Services.AddControllersWithViews();

var app = builder.Build();

builder.Services.AddAuthentication()
    .AddGitHub(o =>
    {
        o.ClientId = builder.Configuration["Authentication:GitHub:aeb5dc480e80b218afbb"];
        o.ClientSecret = builder.Configuration["Authentication:GitHub:d9d71fb3382889ec6ae533095ee91db06032bb9a"];
        o.CallbackPath = "/signin-github";

        // Grants access to read a user's profile data.
        // https://docs.github.com/en/developers/apps/building-oauth-apps/scopes-for-oauth-apps
        o.Scope.Add("read:user");

        // Optional
        // if you need an access token to call GitHub APIs
        o.Events.OnCreatingTicket += context =>
        {
            if (context.AccessToken is { })
            {
                context.Identity?.AddClaim(new Claim("access_token", context.AccessToken));
            }

            return Task.CompletedTask;
        };
    });

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseMigrationsEndPoint();
}
else
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();
app.UseAuthentication();
app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");
app.MapRazorPages();

app.Run();
