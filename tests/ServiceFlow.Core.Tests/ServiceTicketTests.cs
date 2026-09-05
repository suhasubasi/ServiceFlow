using ServiceFlow.Core.Entities;
using ServiceFlow.Core.Enums;
using ServiceFlow.Core.ValueObjects;

namespace ServiceFlow.Core.Tests;

public class ServiceTicketTests
{
    [Fact]
    public void NewTicket_ShouldHaveCorrectValues()
    {
        // Arrange & Act
        var ticket = new ServiceTicket(
            "Fix printer",
            "Printer on 2nd floor is not working",
            "customer-1",
            TicketPriority.High,
            new Money(500m));

        // Assert
        Assert.Equal("Fix printer", ticket.Title);
        Assert.Equal("customer-1", ticket.CustomerId);
        Assert.Equal(TicketPriority.High, ticket.Priority);
        Assert.Equal(TicketStatus.Open, ticket.Status);
    }

    [Fact]
    public void Assign_ShouldSetEngineerAndChangeStatus()
    {
        // Arrange
        var ticket = new ServiceTicket(
            "Fix printer",
            "Printer on 2nd floor is not working",
            "customer-1",
            TicketPriority.High,
            new Money(500m));

        // Act
        ticket.Assign("Anna");

        // Assert
        Assert.Equal("Anna", ticket.AssignedTo);
        Assert.Equal(TicketStatus.InProgress, ticket.Status);
    }

    [Fact]
    public void Resolve_ShouldChangeStatusToResolved()
    {
        // Arrange
        var ticket = new ServiceTicket(
            "Fix printer",
            "Printer on 2nd floor is not working",
            "customer-1",
            TicketPriority.Medium,
            new Money(300m));

        // Act
        ticket.Resolve();

        // Assert
        Assert.Equal(TicketStatus.Resolved, ticket.Status);
    }

    [Fact]
    public void Close_ShouldChangeStatusToClosed()
    {
        // Arrange
        var ticket = new ServiceTicket(
            "Fix printer",
            "Printer on 2nd floor is not working",
            "customer-1",
            TicketPriority.Low,
            new Money(200m));

        // Act
        ticket.Close();

        // Assert
        Assert.Equal(TicketStatus.Closed, ticket.Status);
    }

    [Fact]
    public void Assign_ShouldNotChangeStatus_WhenTicketIsClosed()
    {
        // Arrange
        var ticket = new ServiceTicket(
            "Fix printer",
            "Printer on 2nd floor is not working",
            "customer-1",
            TicketPriority.High,
            new Money(500m));
        ticket.Close();

        // Act
        ticket.Assign("Anna");

        // Assert
        Assert.Equal(TicketStatus.Closed, ticket.Status);
        Assert.Equal("", ticket.AssignedTo);
    }

    [Fact]
    public void Resolve_ShouldNotChangeStatus_WhenTicketIsClosed()
    {
        // Arrange
        var ticket = new ServiceTicket(
            "Fix printer",
            "Printer on 2nd floor is not working",
            "customer-1",
            TicketPriority.High,
            new Money(500m));
        ticket.Close();

        // Act
        ticket.Resolve();

        // Assert
        Assert.Equal(TicketStatus.Closed, ticket.Status);
    }

    [Fact]
    public void Assign_WithEmployee_ShouldSetEmployeeDetailsAndStatus()
    {
        // Arrange
        var ticket = new ServiceTicket(
            "Fix printer",
            "Printer on 2nd floor is not working",
            "customer-1",
            TicketPriority.High,
            new Money(500m));
        var employee = new Employee("John Wick", "jongwick@example.com", "IT Support");

        // Act
        ticket.Assign(employee);

        // Assert
        Assert.Equal(employee.Id, ticket.AssignedEmployeeId);
        Assert.Equal("John Wick", ticket.AssignedTo);
        Assert.Equal(TicketStatus.InProgress, ticket.Status);
    }
}

