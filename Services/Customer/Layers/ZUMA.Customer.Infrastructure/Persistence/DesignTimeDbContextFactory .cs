using ZUMA.Communication.Infrastructure.Persistence;

namespace ZUMA.Customer.Infrastructure.Persistance;

public class CustomerDbContextFactory : BaseDesignTimeDbContextFactory<CustomerDbContext>
{
    protected override string ConnectionStringName => "DbConnection";
}