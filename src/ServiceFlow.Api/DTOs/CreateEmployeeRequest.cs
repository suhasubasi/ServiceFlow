namespace ServiceFlow.Api.DTOs;

public class CreateEmployeeRequest
{
    public string FullName { get; set; } = "";
    public string Email { get; set; } = "";
    public string Department { get; set; } = "";
}
