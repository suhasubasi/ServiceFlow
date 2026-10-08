using System.Net;
using System.Net.Http.Json;
using Microsoft.Extensions.DependencyInjection;
using ServiceFlow.Infrastructure.Persistence;

namespace ServiceFlow.Api.Tests;

public class AuthEndpointTests : IClassFixture<ServiceFlowApiFactory>
{
    private readonly HttpClient _client;
    private readonly ServiceFlowApiFactory _factory;
    public AuthEndpointTests(ServiceFlowApiFactory factory)
    {
        _factory = factory;
        _client = factory.CreateClient();
    }

    private record LoginResponse(string Token);

    [Fact]
    public async Task Login_WithValidCredentials_ShouldReturn200AndToken()
    {
        var request = new
        {
            username = "admin",
            password = "admin123"
        };

        var response = await _client.PostAsJsonAsync("/api/auth/login", request);

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        var data = await response.Content.ReadFromJsonAsync<LoginResponse>();
        Assert.NotNull(data);
        Assert.False(string.IsNullOrWhiteSpace(data.Token));
    }

    [Fact]
    public async Task Login_WithUnknownUsername_ShouldReturn401Unauthorized()
    {
        var request = new
        {
            username = "nobody",
            password = "admin123"
        };
        
        var response = await _client.PostAsJsonAsync("/api/auth/login", request);
        
        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
    }

    [Fact]
    public async Task Login_WithInvalidPassword_ShouldReturn401Unauthorized()
    {
        var request = new
        {
            username = "admin",
            password = "wrong-password"
        };

        var response = await _client.PostAsJsonAsync("/api/auth/login", request);

        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
    }

    [Fact]
    public async Task Login_WithEmptyCredentials_ShouldReturn400BadRequest()
    {
        var request = new
        {
            username = "",
            password = ""
        };

        var response = await _client.PostAsJsonAsync("/api/auth/login", request);

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }
    
    [Fact]
    public void SeededAdmin_ShouldStoreHashedPassword()
    {
        using var scope = _factory.Services.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<ServiceFlowDbContext>();
        
        var admin = db.Users.Single(u => u.Username == "admin");
        
        Assert.NotEqual("admin123", admin.PasswordHash);
        Assert.False(string.IsNullOrWhiteSpace(admin.PasswordHash));
    }
}

