using Microsoft.AspNetCore.Mvc;
using PagePlay.Site.Application.Accounts.Login;
using PagePlay.Site.Infrastructure.Application;

namespace PagePlay.Site.Pages.Signin;

public static class SigninEndpoint
{
    public static void MapSigninRoutes(this IEndpointRouteBuilder endpoints)
    {
        var component = new SigninComponent();

        endpoints.MapGet("/signin", () =>
            Results.Content(component.RenderPage(), "text/html"));

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
        })
        .DisableAntiforgery();
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
