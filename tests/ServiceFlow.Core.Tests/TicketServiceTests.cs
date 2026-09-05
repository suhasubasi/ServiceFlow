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
        Assert.Single(service.GetAll());
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
        Assert.NotNull(found);
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
        Assert.Single(service.GetByStatus(TicketStatus.InProgress));

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
        Assert.Empty(service.GetAll());
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

    [Fact]
    public void GetByCustomerId_ShouldReturnOnlyMatchingTickets()
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
            "customer-1",
            TicketPriority.Critical,
            new Money(1000m)
        );


        service.Add(ticket1);
        service.Add(ticket2);
        service.Add(ticket3);

        // Act
        var results = service.GetByCustomerId("customer-1");

        // Assert 
        Assert.Equal(2, results.Count);
    }

    [Fact]
    public void GetByCustomerId_ShouldReturnEmptyList_WhenNoMatch()
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
        var results = service.GetByCustomerId("customer-999");

        // Assert
        Assert.Empty(results);
    }

    [Fact]
    public void GetByPriority_ShouldReturnOnlyMatchingTickets()
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
            "Replace monitor",
            "Monitor is broken",
            "customer-3",
            TicketPriority.High,
            new Money(800m)
        );
        service.Add(ticket1);
        service.Add(ticket2);
        service.Add(ticket3);
        // Act
        var results = service.GetByPriority(TicketPriority.High);
        // Assert
        Assert.Equal(2, results.Count);
    }
    [Fact]
    public void GetByPriority_ShouldReturnEmptyList_WhenNoMatch()
    {
        // Arrange
        var service = new TicketService();
        var ticket = new ServiceTicket(
            "Fix printer",
            "Printer not working",
            "customer-1",
            TicketPriority.Low,
            new Money(500m)
        );
        service.Add(ticket);
        // Act
        var results = service.GetByPriority(TicketPriority.Critical);
        // Assert
        Assert.Empty(results);
    }

        [Fact]
    public void GetByEmployeeId_ShouldReturnOnlyMatchingTickets()
    {
        // Arrange
        var service = new TicketService();
        var employee1 = new Employee("Paul Walker", "paul@serviceflow.se", "Field Service");
        var employee2 = new Employee("Michelle Rodriguez", "michelle@serviceflow.se", "IT Support");

        var ticket1 = new ServiceTicket("Fix printer", "Desc", "cust-1", TicketPriority.Low, new Money(100m));
        var ticket2 = new ServiceTicket("Fix server", "Desc", "cust-2", TicketPriority.High, new Money(500m));
        var ticket3 = new ServiceTicket("Setup PC", "Desc", "cust-3", TicketPriority.Medium, new Money(250m));

        ticket1.Assign(employee1);
        ticket2.Assign(employee2);
        ticket3.Assign(employee1);

        service.Add(ticket1);
        service.Add(ticket2);
        service.Add(ticket3);

        // Act
        var results = service.GetByEmployeeId(employee1.Id);

        // Assert
        Assert.Equal(2, results.Count);
    }

    [Fact]
    public void GetByEmployeeId_ShouldReturnEmptyList_WhenNoMatch()
    {
        // Arrange
        var service = new TicketService();
        var ticket = new ServiceTicket("Fix printer", "Desc", "cust-1", TicketPriority.Low, new Money(100m));
        service.Add(ticket);

        // Act
        var results = service.GetByEmployeeId(Guid.NewGuid());

        // Assert
        Assert.Empty(results);
    }


}
