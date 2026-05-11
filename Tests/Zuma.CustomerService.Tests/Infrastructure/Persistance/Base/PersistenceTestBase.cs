using Microsoft.EntityFrameworkCore;
using ZUMA.CustomerService.Infrastructure.Persistence;

namespace Zuma.CustomerService.Tests.Infrastructure.Persistance.Base;

public abstract class PersistenceTestBase : IDisposable
{
    protected readonly CustomerDbContext Context;

    protected PersistenceTestBase()
    {
        var options = new DbContextOptionsBuilder<CustomerDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;

        Context = new CustomerDbContext(options);
        Context.Database.EnsureCreated();
    }

    public void Dispose()
    {
        Context.Database.EnsureDeleted();
        Context.Dispose();
    }
}
