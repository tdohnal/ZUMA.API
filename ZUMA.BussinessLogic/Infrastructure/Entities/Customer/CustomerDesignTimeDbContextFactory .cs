using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using ZUMA.BussinessLogic.Infrastructure.Contexts.Customer;

namespace ZUMA.BussinessLogic.Infrastructure.Entities.Customer;

public class DesignTimeDbContextFactory : IDesignTimeDbContextFactory<CustomerDbContext>
{
    public CustomerDbContext CreateDbContext(string[] args)
    {
        var optionsBuilder = new DbContextOptionsBuilder<CustomerDbContext>();

        var connectionString = "Server=localhost,1433;Database=zuma-test;User Id=sa;Password=P@ssw0rd123!;TrustServerCertificate=True;";
        optionsBuilder.UseSqlServer(connectionString);

        return new CustomerDbContext(optionsBuilder.Options);
    }
}
