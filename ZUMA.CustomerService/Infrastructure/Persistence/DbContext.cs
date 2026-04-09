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

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        base.OnConfiguring(optionsBuilder);
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(CustomerDbContext).Assembly);

        modelBuilder.Entity<RegistrationEntity>(entity =>
        {
            entity.HasOne(x => x.User)
                  .WithOne()
                 .HasForeignKey<RegistrationEntity>(x => x.UserId);
        });

        base.OnModelCreating(modelBuilder);
    }
}
