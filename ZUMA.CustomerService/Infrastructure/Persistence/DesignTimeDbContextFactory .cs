namespace ZUMA.CustomerService.Infrastructure.Persistence;

public class CustomerDbContextFactory : BaseDesignTimeDbContextFactory<CustomerDbContext>
{
    protected override string ConnectionStringName => "DbConnection";
}