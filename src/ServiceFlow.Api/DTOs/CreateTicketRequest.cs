using ServiceFlow.Core.Enums;

namespace ServiceFlow.Api.DTOs;

public class CreateTicketRequest
{
    public string Title { get; set; } = "";
    public string Description { get; set; } = "";
    public string CustomerId { get; set; } = "";
    public TicketPriority Priority { get; set; } 
    public decimal EstimatedCostAmount { get; set; }
}