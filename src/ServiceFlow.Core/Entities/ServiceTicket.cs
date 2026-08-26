using ServiceFlow.Core.Enums;
using ServiceFlow.Core.ValueObjects;

namespace ServiceFlow.Core.Entities;

public class ServiceTicket
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public string Title { get; set; }
    public string Description { get; set; }
    public string CustomerId { get; set; }
    public string AssignedTo { get; set; } = "";
    public TicketPriority Priority { get; set; }
    public TicketStatus Status { get; set; } = TicketStatus.Open;
    public Money EstimatedCost { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.Now;

    public ServiceTicket(
        string title,
        string description,
        string customerId,
        TicketPriority priority,
        Money estimatedCost)
    {
        Title = title;
        Description = description;
        CustomerId = customerId;
        Priority = priority;
        EstimatedCost = estimatedCost;
    }

    public void Assign(string engineerName)
    {
        AssignedTo = engineerName;
        Status = TicketStatus.InProgress;
    }

    public void Resolve()
    {
        Status = TicketStatus.Resolved;
    }

    public void Close()
    {
        Status = TicketStatus.Closed;
    }
}