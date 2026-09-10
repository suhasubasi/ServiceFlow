using ServiceFlow.Core.Entities;
using ServiceFlow.Core.Enums;

namespace ServiceFlow.Core.Interfaces;

public interface ITicketService
{
    Task AddAsync(ServiceTicket ticket);
    Task<List<ServiceTicket>> GetAllAsync();
    Task<ServiceTicket?> GetByIdAsync(Guid id);
    Task<List<ServiceTicket>> GetByStatusAsync(TicketStatus status);
    Task<List<ServiceTicket>> GetByCustomerIdAsync(string customerId);
    Task<List<ServiceTicket>> GetByPriorityAsync(TicketPriority priority);
    Task<List<ServiceTicket>> GetByEmployeeIdAsync(Guid employeeId);
    Task UpdateAsync();
    Task<bool> RemoveAsync(Guid id);
}