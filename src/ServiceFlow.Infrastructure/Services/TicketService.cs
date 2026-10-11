using Microsoft.EntityFrameworkCore;
using ServiceFlow.Core.Entities;
using ServiceFlow.Core.Enums;
using ServiceFlow.Core.Interfaces;
using ServiceFlow.Infrastructure.Persistence;
using ServiceFlow.Core.Common;

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

    public async Task<List<ServiceTicket>> GetByCustomerIdAsync(Guid customerId)
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

    public async Task<PagedResult<ServiceTicket>> SearchAsync(string? query, TicketStatus? status, int page, int pageSize)
    {
        var tickets = _context.Tickets.AsQueryable();
        if (!string.IsNullOrWhiteSpace(query))
        {
            var pattern = $"%{query.Trim()}%";
            tickets = tickets.Where(t => 
                EF.Functions.ILike(t.Title, pattern) ||
                EF.Functions.ILike(t.Description, pattern)
            );
        }

        if (status is not null)
        {
            tickets = tickets.Where(t => t.Status == status);
        }

        var totalCount = await tickets.CountAsync();

        var items = await tickets
            .OrderByDescending(t => t.CreatedAt)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();

        return new PagedResult<ServiceTicket>
        {
            Items = items,
            TotalCount = totalCount,
            Page = page,
            Pagesize = pageSize
        };
    }
}