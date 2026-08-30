using ServiceFlow.Core.Entities;
using ServiceFlow.Core.Services;

namespace ServiceFlow.Core.Tests;

public class EmployeeServiceTests
{
    [Fact]
    public void Add_ShouldAddEmployeeToList()
    {
        // Arrange
        var service = new EmployeeService();
        var employee = new Employee("Paul Walker", "paul@serviceflow.se", "Field Service");

        // Act
        service.Add(employee);

        // Assert
        Assert.Single(service.GetAll());
    }

    [Fact]
    public void GetAll_ShouldReturnAllEmployees()
    {
        // Arrange
        var service = new EmployeeService();
        var employee1 = new Employee("Paul Walker", "paul@serviceflow.se", "Field Service");
        var employee2 = new Employee("Michelle Rodriguez", "michelle@serviceflow.se", "IT Support");

        // Act
        service.Add(employee1);
        service.Add(employee2);

        // Assert
        Assert.Equal(2, service.GetAll().Count);
    }

    [Fact]
    public void GetById_ShouldReturnCorrectEmployee_WhenFound()
    {
        // Arrange
        var service = new EmployeeService();
        var employee = new Employee("Paul Walker", "paul@serviceflow.se", "Field Service");
        service.Add(employee);

        // Act
        var found = service.GetById(employee.Id);

        // Assert
        Assert.NotNull(found);
        Assert.Equal("Paul Walker", found.FullName);
    }

    [Fact]
    public void GetById_ShouldReturnNull_WhenNotFound()
    {
        // Arrange
        var service = new EmployeeService();

        // Act
        var found = service.GetById(Guid.NewGuid());

        // Assert
        Assert.Null(found);
    }

    [Fact]
    public void GetByEmail_ShouldReturnCorrectEmployee_WhenFound()
    {
        // Arrange
        var service = new EmployeeService();
        var employee = new Employee("Paul Walker", "paul@serviceflow.se", "Field Service");
        service.Add(employee);

        // Act
        var found = service.GetByEmail("paul@serviceflow.se");

        // Assert
        Assert.NotNull(found);
        Assert.Equal(employee.Id, found.Id);
    }

    [Fact]
    public void GetByEmail_ShouldReturnNull_WhenNotFound()
    {
        // Arrange
        var service = new EmployeeService();

        // Act
        var found = service.GetByEmail("unknown@serviceflow.se");

        // Assert
        Assert.Null(found);
    }
}