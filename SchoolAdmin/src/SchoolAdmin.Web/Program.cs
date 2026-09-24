using System.Security.Claims;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MudBlazor.Services;
using SchoolAdmin.Data;
using SchoolAdmin.Data.Services;
using SchoolAdmin.Web.Components;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents();
builder.Services.AddMudServices();

builder.Services.AddSchoolDb(
    builder.Configuration["Database:Provider"] ?? "SqlServer",
    builder.Configuration.GetConnectionString("SchoolDb")
        ?? throw new InvalidOperationException("ConnectionStrings:SchoolDb is not set."));
builder.Services.AddSingleton<AuthService>();

builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(options =>
    {
        options.LoginPath = "/login";
        options.AccessDeniedPath = "/access-denied";
        options.ExpireTimeSpan = TimeSpan.FromHours(8);
        options.SlidingExpiration = true;
    });
builder.Services.AddAuthorization();
builder.Services.AddCascadingAuthenticationState();

var app = builder.Build();

// Create/upgrade the database, then add demo data when running as the online demo.
await using (var db = await app.Services.GetRequiredService<IDbContextFactory<SchoolDbContext>>().CreateDbContextAsync())
{
    await DbInitializer.InitializeAsync(db);
    if (app.Configuration.GetValue<bool>("DemoMode"))
    {
        await DemoDataSeeder.SeedAsync(db);
    }
}

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error", createScopeForErrors: true);
}
app.UseStatusCodePagesWithReExecute("/not-found", createScopeForStatusCodePages: true);

app.UseAuthentication();
app.UseAuthorization();
app.UseAntiforgery();

app.MapStaticAssets();
app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode();

// ---- Login / logout (plain HTTP endpoints so the auth cookie can be set) ----

app.MapPost("/account/login", async (HttpContext http, AuthService auth,
    [FromForm] string username, [FromForm] string password, [FromForm] string? returnUrl) =>
{
    var user = await auth.LoginAsync(username, password);
    if (user is null)
    {
        return Results.Redirect($"/login?error=1&returnUrl={Uri.EscapeDataString(returnUrl ?? "/")}");
    }

    var claims = new List<Claim>
    {
        new(ClaimTypes.NameIdentifier, user.Id.ToString()),
        new(ClaimTypes.Name, user.Username),
        new(ClaimTypes.Role, user.Role.ToString()),
        new("FullName", user.FullName),
    };
    if (user.StaffId is int staffId) claims.Add(new("StaffId", staffId.ToString()));
    if (user.StudentId is int studentId) claims.Add(new("StudentId", studentId.ToString()));

    await http.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme,
        new ClaimsPrincipal(new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme)));

    var isLocal = !string.IsNullOrEmpty(returnUrl) && returnUrl.StartsWith('/') && !returnUrl.StartsWith("//");
    return Results.LocalRedirect(isLocal ? returnUrl! : "/");
});

app.MapGet("/account/logout", async (HttpContext http) =>
{
    await http.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
    return Results.Redirect("/login");
});

app.MapGet("/health", () => Results.Ok("ok"));

app.Run();
