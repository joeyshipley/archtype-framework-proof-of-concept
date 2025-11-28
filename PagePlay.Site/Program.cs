using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using PagePlay.Site.Application.Accounts.Domain.Models;
using PagePlay.Site.Infrastructure.Core.Application;
using PagePlay.Site.Infrastructure.Data;
using PagePlay.Site.Infrastructure.Dependencies;
using PagePlay.Site.Infrastructure.Web.Routing;
using PagePlay.Site.Infrastructure.Security;
using PagePlay.Site.Infrastructure.Web.Middleware;
using Scalar.AspNetCore;

var builder = WebApplication.CreateBuilder(args);

DependencyResolver.Bind(builder.Services);

// builder.Services.AddScoped<ILoginPageHtmx, LoginPage>();
// builder.Services.AddScoped<IPageDataLoader<LoginPageData>, LoginPageDataLoader>();

builder.Services.AddRazorPages();

builder.Services.AddOpenApi();
builder.Services.AddAuthorization();

// Configure antiforgery for HTMX requests
builder.Services.AddAntiforgery(options =>
{
    options.HeaderName = "X-XSRF-TOKEN"; // HTMX will send this header
});
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

        // Add cookie support for HTMX/browser requests
        options.Events = new JwtBearerEvents
        {
            OnMessageReceived = context =>
            {
                // First check Authorization header (for API calls)
                if (string.IsNullOrEmpty(context.Token))
                {
                    // Fall back to cookie (for HTMX/browser requests)
                    context.Token = context.Request.Cookies["auth_token"];
                }
                return Task.CompletedTask;
            }
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
app.UseMiddleware<RateLimitingMiddleware>();
app.UseMiddleware<SecurityHeadersMiddleware>();
app.UseStaticFiles();
app.UseRouting();
app.UseAuthentication();
app.UseAuthorization();
app.UseAntiforgery();
app.MapRazorPages();
app.MapEndpoints();


// TODO: delete this after updating login for new patterns.
// app.MapLoginRoutes();

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
