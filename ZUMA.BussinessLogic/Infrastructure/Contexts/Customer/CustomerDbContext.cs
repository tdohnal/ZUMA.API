using Microsoft.EntityFrameworkCore;
using ZUMA.BussinessLogic.Entities.Customer;

namespace ZUMA.BussinessLogic.Infrastructure.Contexts.Customer;

public class CustomerDbContext : DbContext
{
    public CustomerDbContext(DbContextOptions<CustomerDbContext> options)
        : base(options)
    {
    }

    public DbSet<UserEntity> Users { get; set; } = null!;
    public DbSet<RegistrationEntity> Registrations { get; set; } = null!;
    public DbSet<EmailEntity> Emails { get; set; } = null!;

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

        modelBuilder.Entity<EmailEntity>(entity =>
        {
            entity.HasKey(x => x.InternalId);

            // Definice vztahu: Jeden email má jednoho příjemce (User)
            entity.HasOne(x => x.Recipient)
                  .WithMany() // Pokud uživatel nemá v sobě kolekci List<EmailEntity> Emails
                  .HasForeignKey(x => x.RecipientId)
                  .OnDelete(DeleteBehavior.Cascade); // Pokud smažeš uživatele, smažou se i jeho záznamy o emailech
        });

        base.OnModelCreating(modelBuilder);
    }
}
