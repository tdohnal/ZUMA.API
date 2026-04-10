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
    public DbSet<ControlElementsItemEntity> ElementItems { get; set; } = null!;
    public DbSet<ControlElementsAcessEntity> ElementAccesses { get; set; } = null!;

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        // Automaticky načte konfigurace z oddělených tříd (viz níže)
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(CustomerDbContext).Assembly);

        // Původní vazba pro registraci
        modelBuilder.Entity<RegistrationEntity>(entity =>
        {
            entity.HasOne(x => x.User)
                  .WithOne()
                  .HasForeignKey<RegistrationEntity>(x => x.UserId);
        });

        base.OnModelCreating(modelBuilder);
    }
}