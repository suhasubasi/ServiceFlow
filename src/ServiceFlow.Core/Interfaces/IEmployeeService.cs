using ServiceFlow.Core.Entities;

namespace ServiceFlow.Core.Interfaces;

public interface IEmployeeService
{
    Task AddAsync(Employee employee);
    Task<List<Employee>> GetAllAsync();
    Task<Employee?> GetByIdAsync(Guid id);
}