using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using ZUMA.BusinessLogic.Configuration;
using ZUMA.BussinessLogic.Infrastructure.Contexts.Customer;

var host = Host.CreateDefaultBuilder(args)
    .ConfigureAppConfiguration((hostingContext, config) =>
    {
        var env = hostingContext.HostingEnvironment;
        config.AddJsonFile("appsettings.json", optional: false, reloadOnChange: true);
        config.AddEnvironmentVariables();
        config.AddCommandLine(args);
    })
    .ConfigureLogging((context, logging) =>
    {
        logging.ClearProviders();

        logging.AddConfiguration(context.Configuration.GetSection("Logging"));
        logging.AddConsole();
        logging.AddDebug();
    })
    .ConfigureServices((context, services) =>
    {
        DIContainer.ConfigureServices(services, context.Configuration);
    })
    .Build();

using var scope = host.Services.CreateScope();
var logger = scope.ServiceProvider.GetRequiredService<ILogger<Program>>();

try
{
    logger.LogInformation("Starting database migration...");

    var db = scope.ServiceProvider.GetRequiredService<CustomerDbContext>();
    await db.Database.MigrateAsync();

    logger.LogInformation("Database and tables are ready.");
}
catch (Exception ex)
{
    logger.LogError(ex, "Error while initializing database");
    throw;
}
