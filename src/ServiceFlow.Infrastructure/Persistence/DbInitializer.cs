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

        // 1. Create initial Swedish demo customer & technician
        var customer = new Customer(
            fullName: "Anna Lindberg",
            email: "anna.lindberg@volvoit.se",
            phoneNumber: "+46 70 123 4567",
            companyName: "Volvo IT"
        );

        var technician = new Employee(
            fullName: "Mikael Blom",
            email: "mikael.blom@serviceflow.se",
            department: "Fältservice"
        );

        context.Customers.Add(customer);
        context.Employees.Add(technician);
        context.SaveChanges();

        // 2. Create realistic Swedish demo tickets in different states
        var ticket1 = new ServiceTicket(
            title: "Skärmbyte Lenovo ThinkPad",
            description: "Sprucken skärm efter fall. Kräver ny 15.6 FHD IPS panel.",
            customerId: customer.Id,
            priority: TicketPriority.High,
            estimatedCost: new Money(3400, "SEK")
        );
        ticket1.Assign(technician); // Status: InProgress

        var ticket2 = new ServiceTicket(
            title: "Fiberanslutning nere",
            description: "Huvudkontorets switch svarar inte på ping sedan kl 08:00. Ingen internetaccess i byggnad B.",
            customerId: customer.Id,
            priority: TicketPriority.Critical,
            estimatedCost: new Money(1850, "SEK")
        ); // Status: Open

        var ticket3 = new ServiceTicket(
            title: "Byte av nätaggregat",
            description: "Server PSU 2 varnar för spänningsfall. Ny 750W redundant enhet monterad och testad.",
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

