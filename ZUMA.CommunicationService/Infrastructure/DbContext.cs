using Microsoft.EntityFrameworkCore;
using ZUMA.SharedKernel.Entities.Customer;

public class CommunicationDbContext : Microsoft.EntityFrameworkCore.DbContext
{
    public CommunicationDbContext(DbContextOptions<CommunicationDbContext> options)
          : base(options)
    {
    }

    public DbSet<EmailEntity> Emails { get; set; } = null!;


    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        base.OnConfiguring(optionsBuilder);
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(DbContext).Assembly);

        base.OnModelCreating(modelBuilder);
    }
}