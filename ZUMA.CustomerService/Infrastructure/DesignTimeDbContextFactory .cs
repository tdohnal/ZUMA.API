using ZUMA.BussinessLogic.Infrastructure.Contexts.Customer;

public class CustomerDbContextFactory : BaseDesignTimeDbContextFactory<CustomerDbContext>
{
    protected override string ConnectionStringName => "DbConnection";
}