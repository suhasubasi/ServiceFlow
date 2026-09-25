using System.ComponentModel.DataAnnotations;
using ServiceFlow.Core.Enums;

namespace ServiceFlow.Api.DTOs;

public class CreateTicketRequest
{
    [Required, MaxLength(200)]
    public string Title { get; set; } = "";
    [Required, MaxLength(2000)]
    public string Description { get; set; } = "";
    [Required]
    public Guid CustomerId { get; set; }
    [EnumDataType(typeof(TicketPriority))]
    public TicketPriority Priority { get; set; } 
    [Range(0, 1_000_000)]
    public decimal EstimatedCostAmount { get; set; }
}