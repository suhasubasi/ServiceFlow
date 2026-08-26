using ServiceFlow.Core.Entities;
using ServiceFlow.Core.Enums;
using ServiceFlow.Core.Services;
using ServiceFlow.Core.ValueObjects;

namespace ServiceFlow.Core.Tests;

public class TicketServiceTests
{
    [Fact]
    public void Add_ShouldAddTicketToList()
    {
        // Arrange
        var service = new TicketService();
        var ticket = new ServiceTicket(
            "Fix printer",
            "Printer not working",
            "customer-1",
            TicketPriority.High,
            new Money(500m));

        // Act
        service.Add(ticket);

        // Assert
        Assert.Equal(1, service.GetAll().Count);
    }

    [Fact]
    public void GetAll_ShouldReturnAllTickets()
    {
        // Arrange
        var service = new TicketService();
        var ticket1 = new ServiceTicket(
            "Fix printer",
            "Printer not working",
            "customer-1",
            TicketPriority.High,
            new Money(500m));

        var ticket2 = new ServiceTicket(
            "Install software",
            "Need Visual Studio",
            "customer-2",
            TicketPriority.Low,
            new Money(200m));

        // Act
        service.Add(ticket1);
        service.Add(ticket2);

        // Assert
        Assert.Equal(2, service.GetAll().Count);
    }

    [Fact]
    public void GetById_ShouldReturnCorrectTicket()
    {
        // Arrange
        var service = new TicketService();
        var ticket = new ServiceTicket(
            "Fix printer",
            "Printer not working",
            "customer-1",
            TicketPriority.High,
            new Money(500m));

        service.Add(ticket);

        // Act
        var found = service.GetById(ticket.Id);

        // Assert
        Assert.Equal("Fix printer", found.Title);
    }

    [Fact]
    public void GetById_ShouldReturnNull_WhenNotFound()
    {
        // Arrange
        var service = new TicketService();

        // Act
        var found = service.GetById(Guid.NewGuid());

        // Assert
        Assert.Null(found);
    }
}
