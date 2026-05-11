using System.Diagnostics.CodeAnalysis;

[ExcludeFromCodeCoverage]
public class CommunicationDbContextFactory : BaseDesignTimeDbContextFactory<CommunicationDbContext>
{
    protected override string ConnectionStringName => "DbConnection";
}