using ZUMA.BussinessLogic.Infrastructure.Factories;

public class UserDbContextFactory : BaseDesignTimeDbContextFactory<RegistrationDbContext>
{
    protected override string ConnectionStringName => "DbConnection";
}