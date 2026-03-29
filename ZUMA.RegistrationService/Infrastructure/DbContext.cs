using Microsoft.EntityFrameworkCore;
using ZUMA.BussinessLogic.Entities.Customer;

public class RegistrationDbContext : Microsoft.EntityFrameworkCore.DbContext
{
    public RegistrationDbContext(DbContextOptions<DbContext> options)
        : base(options)
    {
    }

    public DbSet<RegistrationEntity> Registrations { get; set; } = null!;


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