using ServiceFlow.Core.Entities;

namespace ServiceFlow.Core.Services;

public class EmployeeService
{
    private List<Employee> employees = new List<Employee>();

    public void Add(Employee employee)
    {
        employees.Add(employee);
    }

    public List<Employee> GetAll()
    {
        return employees;
    }

    public Employee? GetById(Guid id)
    {
        for (int i = 0; i < employees.Count; i++)
        {
            if (employees[i].Id == id)
            {
                return employees[i];
            }
        }

        return null;
    }

    public Employee? GetByEmail(string email)
    {
        for (int i = 0; i < employees.Count; i++)
        {
            if (employees[i].Email == email)
            {
                return employees[i];
            }
        }
        
        return null;
    }
}