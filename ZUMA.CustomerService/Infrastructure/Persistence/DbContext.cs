using Microsoft.EntityFrameworkCore;
using ZUMA.CustomerService.Domain.Entities;

namespace ZUMA.CustomerService.Infrastructure.Persistence;

public class CustomerDbContext : DbContext
{
    public CustomerDbContext(DbContextOptions<CustomerDbContext> options)
        : base(options)
    {
    }

    public DbSet<UserEntity> Users { get; set; } = null!;
    public DbSet<RegistrationEntity> Registrations { get; set; } = null!;

    public DbSet<ControlElementEntity> ControlElements { get; set; } = null!;
    public DbSet<ControlElementsItemEntity> ControlElementsItems { get; set; } = null!;

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(CustomerDbContext).Assembly);


        base.OnModelCreating(modelBuilder);
    }
}