using Microsoft.AspNetCore.Antiforgery;
using Microsoft.AspNetCore.Mvc;
using PagePlay.Site.Application.Accounts.Login;
using PagePlay.Site.Infrastructure.Application;

namespace PagePlay.Site.Pages.Signin;

public static class SigninEndpoint
{
    public static void MapSigninRoutes(this IEndpointRouteBuilder endpoints)
    {
        var component = new SigninComponent();

        endpoints.MapGet("/signin", (IAntiforgery antiforgery, HttpContext context) =>
        {
            var tokens = antiforgery.GetAndStoreTokens(context);
            var bodyContent = component.RenderPage(tokens.RequestToken!);
            var fullPage = WrapInLayout(bodyContent, "Sign In");
            return Results.Content(fullPage, "text/html");
        });

        endpoints.MapPost("/api/signin", async (
            [FromForm] string email,
            [FromForm] string password,
            IHttpClientFactory httpClientFactory) =>
        {
            var client = httpClientFactory.CreateClient("ApiClient");

            var request = new { email, password };
            var response = await client.PostAsJsonAsync("/api/account/login", request);

            if (!response.IsSuccessStatusCode)
            {
                return Results.Content(
                    component.RenderError("Login failed. Please check your credentials."),
                    "text/html");
            }

            var result = await response.Content.ReadFromJsonAsync<ApiResult>();

            if (result?.Success == false)
            {
                var errorMessage = result.Errors?.FirstOrDefault()?.Message ?? "An error occurred";
                return Results.Content(
                    component.RenderError(errorMessage),
                    "text/html");
            }

            return Results.Content(
                component.RenderSuccess(result?.Model?.Token ?? "No token received"),
                "text/html");
        });
    }

    private class ApiResult
    {
        public bool Success { get; set; }
        public LoginResponse? Model { get; set; }
        public List<ErrorEntry>? Errors { get; set; }
    }

    private class ErrorEntry
    {
        public string? Property { get; set; }
        public string? Message { get; set; }
    }

    // language=html
    private static string WrapInLayout(string bodyContent, string title) =>
    $$"""
    <!DOCTYPE html>
    <html lang="en">
    <head>
        <meta charset="utf-8" />
        <meta name="viewport" content="width=device-width, initial-scale=1.0" />
        <title>{{title}} - PagePlay</title>
        <script src="https://unpkg.com/htmx.org@1.9.10"></script>
        <script src="https://unpkg.com/idiomorph@0.3.0/dist/idiomorph-ext.min.js"></script>
        <link rel="stylesheet" href="/css/site.css" />
    </head>
    <body>
        <main>
            {{bodyContent}}
        </main>
    </body>
    </html>
    """;
}
