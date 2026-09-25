using System.ComponentModel.DataAnnotations;

namespace ServiceFlow.Api.DTOs;

public class CreateEmployeeRequest
{
    [Required, MaxLength(150)]
    public string FullName { get; set; } = "";
    [Required, EmailAddress, MaxLength(150)]
    public string Email { get; set; } = "";
    [MaxLength(100)]
    public string Department { get; set; } = "";
}
