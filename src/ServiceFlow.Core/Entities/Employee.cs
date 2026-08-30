namespace ServiceFlow.Core.Entities;

public class Employee
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public string FullName { get; set; }
    public string Email { get; set; }
    public string Department { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.Now;

    public Employee(
        string fullName,
        string email,
        string department="")
    {
        FullName = fullName;
        Email = email;
        Department = department;
    } 
}