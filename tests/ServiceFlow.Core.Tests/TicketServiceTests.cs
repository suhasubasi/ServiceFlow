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

    [Fact]
    public void GetByStatus_ShouldReturnOnlyMatchingTickets()
    {
        // Arrange
        var service = new TicketService();


        var ticket1 = new ServiceTicket(
            "Fix printer",
            "Printer not working",
            "customer-1",
            TicketPriority.High,
            new Money(500m)
        );

        var ticket2 = new ServiceTicket(
            "Install software",
            "Need Visual Studio",
            "customer-2",
            TicketPriority.Low,
            new Money(200m)
        );

        var ticket3 = new ServiceTicket(
            "Fix network",
            "Wi-Fi is down",
            "customer-3",
            TicketPriority.Critical,
            new Money(1000m)
        );


        service.Add(ticket1);
        service.Add(ticket2);
        service.Add(ticket3);

        // Act - assign ticket1 so it becomes InProgress
        ticket1.Assign("Anna");

        // Assert 
        Assert.Equal(2, service.GetByStatus(TicketStatus.Open).Count);
        Assert.Equal(1, service.GetByStatus(TicketStatus.InProgress).Count);

    }

    [Fact]
    public void Remove_ShouldRemoveTicket_WhenFound()
    {
        // Arrange
        var service = new TicketService();
        var ticket = new ServiceTicket(
            "Fix printer",
            "Printer not working",
            "customer-1",
            TicketPriority.High,
            new Money(500m)
        );

        service.Add(ticket);

        // Act
        bool removed = service.Remove(ticket.Id);

        // Assert 
        Assert.True(removed);
        Assert.Equal(0, service.GetAll().Count);
    }

    [Fact]
    public void Remove_ShouldReturnFalse_WhenNotFound()
    {
        // Arrange 
        var service = new TicketService();

        // Act 
        bool removed = service.Remove(Guid.NewGuid());

        // Assert 
        Assert.False(removed);

    }
}
