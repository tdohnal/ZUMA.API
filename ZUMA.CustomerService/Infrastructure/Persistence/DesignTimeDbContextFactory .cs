using System.Diagnostics.CodeAnalysis;

namespace ZUMA.CustomerService.Infrastructure.Persistence;

[ExcludeFromCodeCoverage]
public class CustomerDbContextFactory : BaseDesignTimeDbContextFactory<CustomerDbContext>
{
    protected override string ConnectionStringName => "DbConnection";
}