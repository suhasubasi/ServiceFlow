using System.ComponentModel.DataAnnotations;

namespace ServiceFlow.Api.DTOs;

public class CreateCustomerRequest
{
    [Required, MaxLength(150)]
    public string FullName { get; set; } = "";
    [Required, EmailAddress, MaxLength(150)]
    public string Email { get; set; } = "";
    [MaxLength(50)]
    public string PhoneNumber { get; set; } = "";
    [MaxLength(150)]
    public string CompanyName { get; set; } = "";

}