namespace ServiceFlow.Core.Entities;

public class Customer
{
    public Guid Id {get; set; } = Guid.NewGuid();
    public string FullName {get; set;}
    public string Email {get; set;}
    public string PhoneNumber {get; set;}
    public string CompanyName {get; set;}
    public DateTime CreatedAt {get; set;} = DateTime.Now;

    public Customer(
        string fullName,
        string email,
        string phoneNumber = "",
        string companyName = "")
    {
        FullName = fullName;
        Email = email;
        PhoneNumber = phoneNumber;
        CompanyName = companyName;
        
    }

}