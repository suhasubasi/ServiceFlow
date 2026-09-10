using ServiceFlow.Core.Entities;

namespace ServiceFlow.Core.Interfaces;

public interface ICustomerService
{
    Task AddAsync(Customer customer);
    Task<List<Customer>> GetAllAsync();
    Task<Customer?> GetByIdAsync(Guid id);
    Task<Customer?> GetByEmailAsync(string email);
}