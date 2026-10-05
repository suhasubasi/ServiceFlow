using System.Net.Http.Headers;
using System.Net.Http.Json;

namespace ServiceFlow.Api.Tests;

public static class AuthHelper
{
    private record TokenResponse(string Token);

    public static async Task AuthenticateAsync(HttpClient client)
    {
        var loginResponse = await client.PostAsJsonAsync("/api/auth/login", new
        {
            username = "admin",
            password = "admin123"
        });
        loginResponse.EnsureSuccessStatusCode();
        
        var result = await loginResponse.Content.ReadFromJsonAsync<TokenResponse>();
        client.DefaultRequestHeaders.Authorization =
            new AuthenticationHeaderValue("Bearer", result!.Token);

    }

}