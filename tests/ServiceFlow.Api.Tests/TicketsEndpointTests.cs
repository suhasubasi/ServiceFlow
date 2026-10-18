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

    // Small type to read a paged response
    private record PagedResponse(List<IdResponse> Items, int TotalCount, int Page, int PageSize, int TotalPages);
    
    // Small type to read a comment
    private record CommentResponse(Guid Id, string Text, string Author);

    // Creates a customer and a ticket with the given title
    private async Task<Guid> CreateTicketAsync(string title)
    {
        var customerResponse = await _client.PostAsJsonAsync("/api/customers", new
        {
            fullName = "Search Test Customer",
            email = $"search-{Guid.NewGuid()}@example.com",
            phoneNumber = "070 111 22 33",
            companyName = "Search Test Company"
        });
        var customer = await customerResponse.Content.ReadFromJsonAsync<IdResponse>();

        var response = await _client.PostAsJsonAsync("/api/tickets", new
        {
            title,
            description = "Created by a search test",
            customerId = customer!.Id,
            priority = TicketPriority.Low,
            estimatedCostAmount = 100
        });
        response.EnsureSuccessStatusCode();

        var ticket = await response.Content.ReadFromJsonAsync<IdResponse>();
        return ticket!.Id;
    }

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
        await AuthHelper.AuthenticateAsync(_client);

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
        await AuthHelper.AuthenticateAsync(_client);

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

    [Fact]
    public async Task DeleteTicket_AsTechnician_ShouldReturn403Forbidden()
    {
        await AuthHelper.AuthenticateAsync(_client, "tech", "tech123");

        var response = await _client.DeleteAsync($"/api/tickets/{Guid.NewGuid()}");

        Assert.Equal(HttpStatusCode.Forbidden, response.StatusCode);
    }

    [Fact]
    public async Task DeleteTicket_AsAdmin_WithUnknownId_ShouldReturn404()
    {
        await AuthHelper.AuthenticateAsync(_client);

        var response = await _client.DeleteAsync($"/api/tickets/{Guid.NewGuid()}");

        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    [Fact]
    public async Task SearchTickets_ByTitle_ShouldReturnOnlyMatches()
    {
        await AuthHelper.AuthenticateAsync(_client);
        var word = $"zebra{Guid.NewGuid():N}";
        await CreateTicketAsync($"{word} printer");
        await CreateTicketAsync("Something else");

        var result = await _client.GetFromJsonAsync<PagedResponse>(
            $"/api/tickets/search?query={word.ToUpper()}");

        Assert.Equal(1, result!.TotalCount);
        Assert.Single(result.Items);
    }

    [Fact]
    public async Task SearchTickets_WithPageSize_ShouldSplitIntoPages()
    {
        await AuthHelper.AuthenticateAsync(_client);
        var word = $"page{Guid.NewGuid():N}";
        for (var i = 0; i < 3; i++)
        {
            await CreateTicketAsync($"{word} ticket {i}");
        }

        var page2 = await _client.GetFromJsonAsync<PagedResponse>(
            $"/api/tickets/search?query={word}&page=2&pageSize=2");

        Assert.Equal(3, page2!.TotalCount);
        Assert.Equal(2, page2.TotalPages);
        Assert.Single(page2.Items);
    }

    [Fact]
    public async Task GetComments_WithUnknownTicket_ShouldReturn404()
    {
        var response = await _client.GetAsync($"/api/tickets/{Guid.NewGuid()}/comments");

        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    [Fact]
    public async Task AddComment_WithoutToken_ShouldReturn401()
    {
        var response = await _client.PostAsJsonAsync(
            $"/api/tickets/{Guid.NewGuid()}/comments", new { text = "Hello" });

        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
    }

    [Fact]
    public async Task AddComment_WithEmptyText_ShouldReturn400()
    {
        await AuthHelper.AuthenticateAsync(_client);
        var ticketId = await CreateTicketAsync("Comment empty test");

        var response = await _client.PostAsJsonAsync(
            $"/api/tickets/{ticketId}/comments", new { text = " "});

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }

    [Fact]
    public async Task AddComment_AsTechnician_ShouldReturn201WithAuthor()
    {
        await AuthHelper.AuthenticateAsync(_client);
        var ticketId = await CreateTicketAsync("Comment author test");
        await AuthHelper.AuthenticateAsync(_client, "tech", "tech123");

        var response = await _client.PostAsJsonAsync(
            $"/api/tickets/{ticketId}/comments", new { text = "Replaced the cable" });
        var comment = await response.Content.ReadFromJsonAsync<CommentResponse>();

        Assert.Equal(HttpStatusCode.Created, response.StatusCode);
        Assert.Equal("tech", comment!.Author);       
    }

    [Fact]
    public async Task GetComments_ShouldReturnOldestFirst()
    {
        await AuthHelper.AuthenticateAsync(_client);
        var ticketId = await CreateTicketAsync("Comment order test");
        await _client.PostAsJsonAsync($"/api/tickets/{ticketId}/comments", new { text = "First" });
        await _client.PostAsJsonAsync($"/api/tickets/{ticketId}/comments", new { text = "Second" });

        var comments = await _client.GetFromJsonAsync<List<CommentResponse>>(
            $"/api/tickets/{ticketId}/comments");

        Assert.Equal(2, comments!.Count);
        Assert.Equal("First", comments[0].Text);
        Assert.Equal("Second", comments[1].Text); 
    }
}