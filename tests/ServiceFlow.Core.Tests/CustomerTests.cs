using ServiceFlow.Core.Entities;

namespace ServiceFlow.Core.Tests;

public class CustomerTests
{
    [Fact]
    public void NewCustomer_ShouldHaveCorrectValues_WhenAllParametersProvided()
    {
        // Arrange & Act
        var customer = new Customer(
            "Jason Statham",
            "jason@example.com",
            "0701234567",
            "Hollywood AB"
        );

        // Assert 
        Assert.Equal("Jason Statham", customer.FullName);
        Assert.Equal("jason@example.com", customer.Email);
        Assert.Equal("0701234567", customer.PhoneNumber);
        Assert.Equal("Hollywood AB", customer.CompanyName);
        Assert.NotEqual(Guid.Empty, customer.Id);
    }

    [Fact]
    public void NewCustomer_ShouldHaveEmptyDefaults_WhenOptionalParametersOmitted()
    {
        // Arrange & Act
        var customer = new Customer("Tom Holland","tom@example.com");

        // Assert 
        Assert.Equal("Tom Holland", customer.FullName);
        Assert.Equal("tom@example.com", customer.Email);
        Assert.Equal("", customer.PhoneNumber);
        Assert.Equal("", customer.CompanyName);
    }
}