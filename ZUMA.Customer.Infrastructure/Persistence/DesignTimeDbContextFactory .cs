namespace ZUMA.Customer.Infrastructure.Persistance;

public class CustomerDbContextFactory : BaseDesignTimeDbContextFactory<CustomerDbContext>
{
    protected override string ConnectionStringName => "DbConnection";
}