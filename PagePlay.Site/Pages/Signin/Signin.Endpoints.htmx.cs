using Microsoft.AspNetCore.Antiforgery;
using Microsoft.AspNetCore.Mvc;
using PagePlay.Site.Application.Accounts.Login;
using PagePlay.Site.Pages.Shared;

namespace PagePlay.Site.Pages.Signin;

public static class SigninEndpoints
{
    public static void MapSigninRoutes(this IEndpointRouteBuilder endpoints)
    {
        var page = new SigninPage();

        endpoints.MapGet("/signin", (IAntiforgery antiforgery, HttpContext context) =>
        {
            var tokens = antiforgery.GetAndStoreTokens(context);
            var bodyContent = page.RenderPage(tokens.RequestToken!);
            var fullPage = Layout.Render(bodyContent, "Sign In", tokens.RequestToken!);
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
                    page.RenderError("Login failed. Please check your credentials."),
                    "text/html");
            }

            var result = await response.Content.ReadFromJsonAsync<ApiResult>();

            if (result?.Success == false)
            {
                var errorMessage = result.Errors?.FirstOrDefault()?.Message ?? "An error occurred";
                return Results.Content(
                    page.RenderError(errorMessage),
                    "text/html");
            }

            return Results.Content(
                page.RenderSuccess(result?.Model?.Token ?? "No token received"),
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
}
