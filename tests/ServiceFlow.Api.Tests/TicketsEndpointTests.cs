using System.Net;
using System.Net.Http.Json;
using ServiceFlow.Core.Enums;


namespace ServiceFlow.Api.Tests;

public class TicketsEndpointTests : IClassFixture<ServiceFlowApiFactory>
{
    private readonly HttpClient _client;

    public TicketsEndpointTests(ServiceFlowApiFactory factory)
    {
        _client = factory.CreateClient();
    }
    
    // Small type to read the "id" from a JSON response
    private record IdResponse(Guid Id);

    [Fact]
    public async Task GetAllTickets_ShouldReturn200()
    {
        var response = await _client.GetAsync("/api/tickets");
        
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
    }
    
    [Fact]
    public async Task GetTicketById_WithUnknownId_ShouldReturn404()
    {
        var response = await _client.GetAsync($"/api/tickets/{Guid.NewGuid()}");

        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }
    
    [Fact]
    public async Task CreateTicket_WithoutTitle_ShouldReturn400()
    {
        var request = new
        {
            title = "",
            description = "Printer is not working",
            customerId = Guid.NewGuid(),
            priority = TicketPriority.Medium,
            estimatedCostAmount = 500
        };
        
        var response = await _client.PostAsJsonAsync("/api/tickets", request);

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }

    [Fact]
    public async Task CreateTicket_WithValidData_ShouldReturn201()
    {
        // Arrange: create a real customer first
        var customerRequest = new
        {
            fullName = "Ticket Test Customer",
            email = $"ticket-{Guid.NewGuid()}@example.com",
            phoneNumber = "070 111 22 33",
            companyName = "Ticket Test Company"
        };
        var customerResponse = await _client.PostAsJsonAsync("/api/customers", customerRequest);
        var customer = await customerResponse.Content.ReadFromJsonAsync<IdResponse>();

        var ticketRequest = new
        {
            title = "Laptop does not start",
            description = "Black screen after update",
            customerId = customer!.Id,
            priority = TicketPriority.High,
            estimatedCostAmount = 750
        };
        
        // Act 
        var response = await _client.PostAsJsonAsync("/api/tickets", ticketRequest);
        
        // Assert
        Assert.Equal(HttpStatusCode.Created, response.StatusCode);
    }
}