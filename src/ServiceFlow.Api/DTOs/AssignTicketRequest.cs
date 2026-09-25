using System.ComponentModel.DataAnnotations;

namespace ServiceFlow.Api.DTOs;

public class AssignTicketRequest
{
    [Required]
    public Guid EmployeeId { get; set; }
}