using ServiceFlow.Core.Entities;

namespace ServiceFlow.Core.Tests;

public class EmployeeTests
{
    [Fact]
    public void NewEmployee_ShouldHaveCorrectValues()
    {
        // Arrange & Act 
        var employee = new Employee("Dwayne Johnson",
        "dwayne@serviceflow.se", "IT Support");

        // Assert
        Assert.Equal("Dwayne Johnson", employee.FullName);
        Assert.Equal("dwayne@serviceflow.se", employee.Email);
        Assert.Equal("IT Support", employee.Department);
    }

    [Fact]
    public void NewEmployee_ShouldHaveDefaultDepartment_WhenNotProvided()
    {
        // Arrange & Act
        var employee = new Employee("Vin Diesel", "vin@serviceflow.se");

        // Assert 
        Assert.Equal("", employee.Department);
    }
}