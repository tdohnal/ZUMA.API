using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;
using ZUMA.BussinessLogic.Infrastructure.Contexts.Customer;

public class DesignTimeDbContextFactory : IDesignTimeDbContextFactory<CustomerDbContext>
{
    public CustomerDbContext CreateDbContext(string[] args)
    {
        string basePath = Path.Combine(Directory.GetCurrentDirectory(), "..", "ZUMA.API");

        if (!File.Exists(Path.Combine(basePath, "appsettings.json")))
        {
            basePath = Directory.GetCurrentDirectory();
        }

        IConfigurationRoot configuration = new ConfigurationBuilder()
            .SetBasePath(basePath)
            .AddJsonFile("appsettings.json", optional: false)
            .Build();

        var optionsBuilder = new DbContextOptionsBuilder<CustomerDbContext>();
        var connectionString = configuration.GetConnectionString("CustomerDb");

        if (string.IsNullOrEmpty(connectionString))
        {
            throw new InvalidOperationException("ConnectionString 'CustomerDb' nebyl nalezen!");
        }

        var localConnectionString = connectionString.Replace("zuma-db", "localhost");

        optionsBuilder.UseSqlServer(localConnectionString);

        return new CustomerDbContext(optionsBuilder.Options);
    }
}