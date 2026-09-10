using Microsoft.EntityFrameworkCore;
using ServiceFlow.Core.Entities;
using ServiceFlow.Core.Interfaces;
using ServiceFlow.Infrastructure.Persistence;

namespace ServiceFlow.Infrastructure.Services;

public class CustomerService : ICustomerService
{
    private readonly ServiceFlowDbContext _context;

    // Inject our PostgreSQL DbContext
    public CustomerService(ServiceFlowDbContext context)
    {
        _context = context;
    }

    // CREATE: Save a new customer to PostgreSQL
    public async Task AddAsync(Customer customer)
    {
        _context.Customers.Add(customer);
        await _context.SaveChangesAsync();
    }

    // READ: Get all customers from PostgreSQL
    public async Task<List<Customer>> GetAllAsync()
    {
        return await _context.Customers.ToListAsync();
    }

    // READ: Find customer by ID 
    public async Task<Customer?> GetByIdAsync(Guid id)
    {
        return await _context.Customers.FirstOrDefaultAsync(c => c.Id == id);
    }

    // READ: Find customer by Email
    public async Task<Customer?> GetByEmailAsync(string email)
    {
        return await _context.Customers.FirstOrDefaultAsync(c => c.Email == email);
    }
}