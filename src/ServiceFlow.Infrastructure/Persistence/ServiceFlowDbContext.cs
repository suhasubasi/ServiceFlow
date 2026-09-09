using Microsoft.EntityFrameworkCore;
using ServiceFlow.Core.Entities;
using ServiceFlow.Core.ValueObjects;

namespace ServiceFlow.Infrastructure.Persistence;

public class ServiceFlowDbContext : DbContext
{
    // Ensures PosgreSQL accepts local DateTime without strict UTC conversion errors
    static ServiceFlowDbContext()
    {
        AppContext.SetSwitch("Npgsql.EnableLegacyTimestampBehavior", true);
    }

    public ServiceFlowDbContext(DbContextOptions<ServiceFlowDbContext>options) : base(options)
    {
    }

    public DbSet<Customer> Customers => Set<Customer>();
    public DbSet<Employee> Employees => Set<Employee>();
    public DbSet<ServiceTicket> Tickets => Set<ServiceTicket>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // Configure Customer table
        modelBuilder.Entity<Customer>(entity =>
        {
            entity.HasKey(c => c.Id);
            entity.Property(c => c.FullName).IsRequired().HasMaxLength(150);
            entity.Property(c => c.Email).IsRequired().HasMaxLength(150);
            entity.Property(c => c.PhoneNumber).HasMaxLength(50);
            entity.Property(c => c.CompanyName).HasMaxLength(150);
        });

        // Configure Employee table
        modelBuilder.Entity<Employee>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.FullName).IsRequired().HasMaxLength(150);
            entity.Property(e => e.Email).IsRequired().HasMaxLength(150);
            entity.Property(e => e.Department).HasMaxLength(100);
        });
        
        // Configure ServiceTicket table
        modelBuilder.Entity<ServiceTicket>(entity =>
        {
            entity.HasKey(t => t.Id);
            entity.Property(t => t.Title).IsRequired().HasMaxLength(200);
            entity.Property(t => t.Description).IsRequired().HasMaxLength(2000);
            entity.Property(t => t.CustomerId).IsRequired();

            
            // Money is a DDD Value Object (a record struct with Amount + Currency, but no separate Id).
            // ComplexProperty tells EF Core: "Don't create a separate table! Flatten these two values
            // directly into the Tickets table as normal columns."
            entity.ComplexProperty(t => t.EstimatedCost, money =>
            {
                // Store the numeric cost: up to 18 digits with 2 decimal places for cents...
                money.Property(m => m.Amount)
                    .HasColumnName("EstimatedCost_Amount")
                    .HasPrecision(18, 2);

                // Store the currency code: max 10 characters (Example: "SEK", "EUR", "USD")
                money.Property(m => m.Currency)
                    .HasColumnName("EstimatedCost_Currency")
                    .HasMaxLength(10);
            });
        });
    }
}