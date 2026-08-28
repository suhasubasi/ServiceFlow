using ServiceFlow.Core.Entities;
using ServiceFlow.Core.Services;

namespace ServiceFlow.Core.Tests;

public class CustomerServiceTests
{
    [Fact]
    public void Add_ShouldAddCustomerToList()
    {
        // Arrange 
        var service = new CustomerService();
        var customer = new Customer("Jason Statham","jason@example.com");

        // Act
        service.Add(customer);

        // Assert
        Assert.Single(service.GetAll());
    }

    [Fact]
    public void GetAll_ShouldReturnAllCustomers()
    {
        // Arrange
        var service = new CustomerService();
        var customer1 = new Customer("Jason Statham", "jason@example.com");
        var customer2 = new Customer("Tom Holland", "tom@example.com");

        // Act 
        service.Add(customer1);
        service.Add(customer2);

        // Assert 
        Assert.Equal(2, service.GetAll().Count);
    }

    [Fact]
    public void GetById_ShouldReturnCorrectCustomer_WhenFound()
    {
        // Arrange
        var service = new CustomerService();
        var customer = new Customer("Jason Statham", "jason@example.com");
        service.Add(customer);

        // Act 
        var found = service.GetById(customer.Id);

        // Assert
        Assert.NotNull(found);
        Assert.Equal("Jason Statham", found.FullName);
    }

    [Fact]
    public void GetById_ShouldReturnNull_WhenNotFound()
    {
        // Arrange
        var service = new CustomerService();

        // Act 
        var found = service.GetById(Guid.NewGuid());

        // Assert
        Assert.Null(found);
    }

    [Fact]
    public void GetByEmail_ShouldReturnCorrectCustomer_WhenFound()
    {
        // Arrange
        var service = new CustomerService();
        var customer = new Customer("Jason Statham", "jason@example.com");
        service.Add(customer);

        // Act
        var found = service.GetByEmail("jason@example.com");

        // Assert
        Assert.NotNull(found);
        Assert.Equal(customer.Id, found.Id);
    }

    [Fact]
    public void GetByEmail_ShouldReturnNull_WhenNotFound()
    {
        // Arrange
        var service = new CustomerService();

        // Act
        var found = service.GetByEmail("unknown@example.com");

        // Assert
        Assert.Null(found);
    }
}
