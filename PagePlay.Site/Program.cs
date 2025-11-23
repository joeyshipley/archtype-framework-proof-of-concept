using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using PagePlay.Site.Application.Accounts.Domain.Models;
using PagePlay.Site.Infrastructure.Application;
using PagePlay.Site.Infrastructure.Database;
using PagePlay.Site.Infrastructure.Dependencies;
using PagePlay.Site.Infrastructure.Routing;
using PagePlay.Site.Infrastructure.Security;
using PagePlay.Site.Pages.Login;
using PagePlay.Site.Pages.Signin;
using PagePlay.Site.Pages.Signin2;
using Scalar.AspNetCore;

var builder = WebApplication.CreateBuilder(args);

DependencyResolver.Bind(builder.Services);

builder.Services.AddScoped<ILoginPageHtmx, LoginPage>();
builder.Services.AddScoped<IPageDataLoader<LoginPageData>, LoginPageDataLoader>();

builder.Services.AddRazorPages();

builder.Services.AddOpenApi();
builder.Services.AddAuthorization();
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        var jwtSettings = builder.Configuration.GetSection("Security:Jwt").Get<JwtSettings>();
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = jwtSettings.Issuer,
            ValidAudience = jwtSettings.Audience,
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSettings.SecretKey))
        };
    });

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.MapScalarApiReference();
    app.UseDeveloperExceptionPage();
}
else
{
    app.UseExceptionHandler("/error");
}

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseRouting();
app.UseAuthentication();
app.UseAuthorization();
app.UseAntiforgery();
app.MapRazorPages();
app.MapSigninRoutes();
app.MapSignin2Routes();
app.MapLoginRoutes();
app.MapEndpoints();

// TODO: clean this up.
// Warm up services to avoid cold start penalty on first request
await Task.Run(async () =>
{
    // Warm up BCrypt password hasher
    var passwordHasher = app.Services.GetRequiredService<IPasswordHasher>();
    _ = passwordHasher.VerifyPassword("warmup", "$2a$12$dummy.hash.for.warmup.only...................");

    // Warm up EF Core query compilation and database connection pool
    using var scope = app.Services.CreateScope();
    var dbContextFactory = scope.ServiceProvider.GetRequiredService<IDbContextFactory<AppDbContext>>();
    await using var dbContext = await dbContextFactory.CreateDbContextAsync();
    _ = await dbContext.Set<User>()
        .Where(u => u.Email == "warmup@example.com")
        .AsNoTracking()
        .FirstOrDefaultAsync();

    // TODO: For later - HttpClient warmup would need a background service since server isn't listening yet at this point
});

app.Run();
