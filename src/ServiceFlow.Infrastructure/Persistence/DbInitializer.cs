using ServiceFlow.Core.Entities;
using ServiceFlow.Core.Enums;
using ServiceFlow.Core.ValueObjects;

namespace ServiceFlow.Infrastructure.Persistence;

public static class DbInitializer
{
    public static void Seed(ServiceFlowDbContext context)
    {
        // Only seed if the database is completely empty
        if (context.Customers.Any())
        {
            return;
        }

        // 1. Create initial demo customer & technician
        var customer = new Customer(
            fullName: "Anna Lindberg",
            email: "anna.lindberg@nordiclogistics.se",
            phoneNumber: "+46 70 123 4567",
            companyName: "Nordic Logistics"
        );

        var technician = new Employee(
            fullName: "Mikael Blom",
            email: "mikael.blom@serviceflow.se",
            department: "Hardware & Field Service"
        );

        context.Customers.Add(customer);
        context.Employees.Add(technician);
        context.SaveChanges();

        // 2. Create realistic demo tickets in different states
        var ticket1 = new ServiceTicket(
            title: "Broken screen - ThinkPad T14",
            description: "User dropped laptop. Screen is cracked and flickering. Needs replacement 14-inch IPS panel.",
            customerId: customer.Id,
            priority: TicketPriority.High,
            estimatedCost: new Money(3400, "SEK")
        );
        ticket1.Assign(technician); // Status: InProgress

        var ticket2 = new ServiceTicket(
            title: "Office switch offline in Building B",
            description: "Switch in rack 2 stopped responding to ping. No network access for 15 users since 08:30.",
            customerId: customer.Id,
            priority: TicketPriority.Critical,
            estimatedCost: new Money(1850, "SEK")
        ); // Status: Open

        var ticket3 = new ServiceTicket(
            title: "Server power supply failure",
            description: "Redundant PSU 2 in main server threw voltage warning. Replaced with spare 750W unit and tested.",
            customerId: customer.Id,
            priority: TicketPriority.Medium,
            estimatedCost: new Money(2200, "SEK")
        );
        ticket3.Assign(technician);
        ticket3.Resolve(); // Status: Resolved

        context.Tickets.AddRange(ticket1, ticket2, ticket3);
        context.SaveChanges();
    }
}

