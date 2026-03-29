using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;

public abstract class BaseDesignTimeDbContextFactory<T> : IDesignTimeDbContextFactory<T>
    where T : DbContext
{
    protected abstract string ConnectionStringName { get; }

    public T CreateDbContext(string[] args)
    {
        // 1. Vezme aktuální složku (tam kde je ten projekt/Context)
        string basePath = Directory.GetCurrentDirectory();

        // 2. Sestavení konfigurace z lokálního souboru dané služby
        IConfigurationRoot configuration = new ConfigurationBuilder()
            .SetBasePath(basePath)
            .AddJsonFile("appsettings.json", optional: false)
            .AddJsonFile("appsettings.Development.json", optional: true)
            .Build();

        var connectionString = configuration.GetConnectionString(ConnectionStringName);

        if (string.IsNullOrEmpty(connectionString))
        {
            // Debug info, aby ses v konzoli hned dozvěděl, kde to hledalo
            throw new InvalidOperationException(
                $"Error: ConnectionString '{ConnectionStringName}' not found in {Path.Combine(basePath, "appsettings.json")}");
        }

        // Hostname transformace (zuma-db -> localhost)
        var localConnectionString = connectionString.Contains("zuma-db")
            ? connectionString.Replace("zuma-db", "localhost")
            : connectionString;

        var optionsBuilder = new DbContextOptionsBuilder<T>();
        optionsBuilder.UseSqlServer(localConnectionString);

        return (T)Activator.CreateInstance(typeof(T), optionsBuilder.Options)!;
    }
}