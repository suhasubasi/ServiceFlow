using Microsoft.EntityFrameworkCore;
using ServiceFlow.Core.Entities;
using ServiceFlow.Core.Enums;
using ServiceFlow.Core.Interfaces;
using ServiceFlow.Infrastructure.Persistence;

namespace ServiceFlow.Infrastructure.Services;

public class TicketService : ITicketService
{
    private readonly ServiceFlowDbContext _context;

    public TicketService(ServiceFlowDbContext context)
    {
        _context = context;
    }

    public async Task AddAsync(ServiceTicket ticket)
    {
        _context.Tickets.Add(ticket);
        await _context.SaveChangesAsync();
    }

    public async Task<List<ServiceTicket>> GetAllAsync()
    {
        return await _context.Tickets.ToListAsync();
    }

    public async Task<ServiceTicket?> GetByIdAsync(Guid id)
    {
        return await _context.Tickets.FirstOrDefaultAsync(t => t.Id == id);
    }

    public async Task<List<ServiceTicket>> GetByStatusAsync(TicketStatus status)
    {
        return await _context.Tickets
            .Where(t => t.Status == status)
            .ToListAsync();
    }

    public async Task<List<ServiceTicket>> GetByCustomerIdAsync(string customerId)
    {
        return await _context.Tickets
            .Where(t => t.CustomerId == customerId)
            .ToListAsync();
    }

    public async Task<List<ServiceTicket>> GetByPriorityAsync(TicketPriority priority)
    {
        return await _context.Tickets
            .Where(t => t.Priority == priority)
            .ToListAsync();
    }

    public async Task<List<ServiceTicket>> GetByEmployeeIdAsync(Guid employeeId)
    {
        return await _context.Tickets
            .Where(t => t.AssignedEmployeeId == employeeId)
            .ToListAsync();
    }

    public async Task UpdateAsync()
    {
        await _context.SaveChangesAsync();
    }

    public async Task<bool> RemoveAsync(Guid id)
    {
        var ticket = await _context.Tickets.FirstOrDefaultAsync(t => t.Id == id);
        if (ticket == null)
        {
            return false;
        }

        _context.Tickets.Remove(ticket);
        await _context.SaveChangesAsync();
        return true;
    }
}