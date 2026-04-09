using ZUMA.Communication.Infrastructure.Persistence;

public class CommunicationDbContextFactory : BaseDesignTimeDbContextFactory<CommunicationDbContext>
{
    protected override string ConnectionStringName => "DbConnection";
}