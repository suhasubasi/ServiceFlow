using Microsoft.EntityFrameworkCore;
using ServiceFlow.Core.Entities;
using ServiceFlow.Core.Interfaces;
using ServiceFlow.Infrastructure.Persistence;

namespace ServiceFlow.Infrastructure.Services;

public class EmployeeService : IEmployeeService
{
    private readonly ServiceFlowDbContext _context;

    public EmployeeService(ServiceFlowDbContext context)
    {
        _context = context;
    }

    public async Task AddAsync(Employee employee)
    {
        _context.Employees.Add(employee);
        await _context.SaveChangesAsync();
    }

    public async Task<List<Employee>> GetAllAsync()
    {
        return await _context.Employees.ToListAsync();
    }

    public async Task<Employee?> GetByIdAsync(Guid id)
    {
        return await _context.Employees.FirstOrDefaultAsync(e => e.Id == id);
    }
}