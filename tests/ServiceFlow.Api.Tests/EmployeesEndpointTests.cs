using System.Net;
using System.Net.Http.Json;

namespace ServiceFlow.Api.Tests;

public class EmployeesEndpointTests : IClassFixture<ServiceFlowApiFactory>
{
    private readonly HttpClient _client;

    public EmployeesEndpointTests(ServiceFlowApiFactory factory)
    {
        _client = factory.CreateClient();
    }

    private record EmployeeResponse(Guid Id, string FullName, string Email, string Department);

    [Fact]
    public async Task GetAllEmployees_ShouldReturn200()
    {
        var response = await _client.GetAsync("/api/employees");

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
    }

    [Fact]
    public async Task GetEmployeeById_WithUnknownId_ShouldReturn404()
    {
        var response = await _client.GetAsync($"/api/employees/{Guid.NewGuid()}");

        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    [Fact]
    public async Task CreateEmployee_WithoutToken_ShouldReturn401Unauthorized()
    {
        var request = new
        {
            fullName = "Unauthenticated Employee",
            email = "unauth@example.com",
            department = "Support"
        };

        var response = await _client.PostAsJsonAsync("/api/employees", request);

        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
    }

    [Fact]
    public async Task CreateEmployee_WithValidData_ShouldReturn201()
    {
        await AuthHelper.AuthenticateAsync(_client);

        var request = new
        {
            fullName = "Erik Svensson",
            email = $"erik-{Guid.NewGuid()}@example.com",
            department = "IT Support"
        };

        var response = await _client.PostAsJsonAsync("/api/employees", request);

        Assert.Equal(HttpStatusCode.Created, response.StatusCode);

        var employee = await response.Content.ReadFromJsonAsync<EmployeeResponse>();
        Assert.NotNull(employee);
        Assert.NotEqual(Guid.Empty, employee.Id);
        Assert.Equal(request.fullName, employee.FullName);
    }

    [Fact]
    public async Task CreateEmployee_WithoutName_ShouldReturn400()
    {
        await AuthHelper.AuthenticateAsync(_client);

        var request = new
        {
            fullName = "",
            email = "noname@example.com",
            department = "IT Support"
        };

        var response = await _client.PostAsJsonAsync("/api/employees", request);

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }

    [Fact]
    public async Task CreateEmployee_WithInvalidEmail_ShouldReturn400()
    {
        await AuthHelper.AuthenticateAsync(_client);

        var request = new
        {
            fullName = "Lars Larsson",
            email = "not-a-valid-email",
            department = "IT Support"
        };

        var response = await _client.PostAsJsonAsync("/api/employees", request);

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }
    
    [Fact]
    public async Task CreateEmployee_AsTechnician_ShouldReturn403Forbidden()
    {
        await AuthHelper.AuthenticateAsync(_client, "tech", "tech123");

        var request = new
        {
            fullName = "Erik Svensson",
            email = $"erik-{Guid.NewGuid()}@example.com",
            department = "IT Support"
        };

        var response = await _client.PostAsJsonAsync("/api/employees", request);

        Assert.Equal(HttpStatusCode.Forbidden, response.StatusCode);
    }
}

