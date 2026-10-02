using System.Net;
using System.Net.Http.Json;

namespace ServiceFlow.Api.Tests;

public class CustomersEndpointTests : IClassFixture<ServiceFlowApiFactory>
{
    private readonly HttpClient _client;

    public CustomersEndpointTests(ServiceFlowApiFactory factory)
    {
        _client = factory.CreateClient();
    }

    [Fact]
    public async Task CreateCustomer_WithValidData_ShouldReturn201()
    {
        var request = new
        {
            fullName = "Test Customer",
            email = $"test-{Guid.NewGuid()}@example.com",
            phoneNumber = "070 123 45 67",
            companyName = "Test Company"
        };

        var response = await _client.PostAsJsonAsync("/api/customers", request);
        
        Assert.Equal(HttpStatusCode.Created, response.StatusCode);
    }

    [Fact]
    public async Task CreateCustomer_WithoutName_Returns400()
    {
        var request = new
        {
            fullName = "",
            email = "no-name@example.com"
        };

        var response = await _client.PostAsJsonAsync("/api/customers", request);
        
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }
}