using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using ZUMA.BusinessLogic.Configuration;
using ZUMA.BussinessLogic.Infrastructure.Contexts.Customer;

namespace ZUMA.Unittests.Fixtures
{
    [TestFixture]
    public class DatabaseFixture
    {
        protected IServiceProvider ServiceProvider = null!;
        protected IServiceScope ServiceScope = null!;
        protected CustomerDbContext DbContext = null!;

        [SetUp]
        public virtual void SetupDatabase()
        {
            // Vytvoř configuration
            var configuration = new ConfigurationBuilder()
                .AddJsonFile("appsettings.json", optional: true)
                .AddInMemoryCollection(new Dictionary<string, string>
                {
                    { "ConnectionStrings:CustomerDb", "Server=(local);Database=zuma-Customer;User Id=sa;Password=P@ssw0rd123!;" }
                })
                .Build();

            // Zaregistruj všechny služby z DIContainer
            var services = new ServiceCollection();
            DIContainer.ConfigureServices(services, configuration);

            // Nahraď CustomerDbContext na in-memory DB pro testování
            services.AddScoped(sp =>
            {
                var options = new DbContextOptionsBuilder<CustomerDbContext>()
                    .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                    .EnableSensitiveDataLogging()
                    .Options;

                return new CustomerDbContext(options);
            });

            // Vytvoř DI container
            ServiceProvider = services.BuildServiceProvider();
            ServiceScope = ServiceProvider.CreateScope();
            DbContext = ServiceScope.ServiceProvider.GetRequiredService<CustomerDbContext>();

            // Vytvoř tabulky
            DbContext.Database.EnsureCreated();
        }

        [TearDown]
        public virtual void CleanupDatabase()
        {
            //DbContext?.Database.EnsureDeleted();
            //DbContext?.Dispose();
            //ServiceScope?.Dispose();
            //(ServiceProvider as IDisposable)?.Dispose();
        }

        protected async Task SeedDatabaseAsync(Func<CustomerDbContext, Task> seeder)
        {
            await seeder(DbContext);
            await DbContext.SaveChangesAsync();
        }

        protected T GetService<T>() where T : notnull
        {
            return ServiceScope.ServiceProvider.GetRequiredService<T>();
        }
    }
}