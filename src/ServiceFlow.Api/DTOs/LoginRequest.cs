using System.ComponentModel.DataAnnotations;

namespace ServiceFlow.Api.DTOs;

public class LoginRequest
{
    [Required] public string Username { get; set; } = "";
    [Required] public string Password { get; set; } = "";

}