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
using Serilog;

// Configure Serilog early in the application startup
// This allows us to capture startup errors and logging from the very beginning
Log.Logger = new LoggerConfiguration()
    .ReadFrom.Configuration(new ConfigurationBuilder()
        .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
        .AddJsonFile($"appsettings.{Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") ?? "Production"}.json", optional: true, reloadOnChange: true)
        .AddEnvironmentVariables()
        .Build())
    .CreateLogger();

try
{
    Log.Information("Starting PagePlay application");

    var builder = WebApplication.CreateBuilder(args);

    // Replace default logging with Serilog
    builder.Host.UseSerilog();

DependencyResolver.Bind(builder.Services);
builder.Services.AddResponseCompressionMiddleware();
builder.ConfigureRequestSizeLimits();

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
app.UseResponseCompressionMiddleware();
app.UseMiddleware<RequestLoggingMiddleware>();
app.UseMiddleware<RateLimitingMiddleware>();
app.UseMiddleware<SecurityHeadersMiddleware>();
app.UseStaticFiles();
app.UseRouting();
app.UseAuthentication();
app.UseAuthorization();
app.UseAntiforgery();
app.MapRazorPages();
app.MapEndpoints();

// Warm up services to avoid cold start penalty on first request
await app.Services.WarmupAsync();

Log.Information("PagePlay application started successfully");
    app.Run();
}
catch (Exception ex)
{
    Log.Fatal(ex, "Application startup failed");
    throw;
}
finally
{
    Log.CloseAndFlush();
}
