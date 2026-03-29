using Quartz;
using ZUMA.DataCleaner.Jobs;
using ZUMA.DataCleaner.Services;

namespace ZUMA.DataCleaner.Configuration;

public static class DIContainer
{
    public static void ConfigureServices(this IServiceCollection services)
    {
        services.AddScoped<DataCleanerService<UserEntity>>();
        services.AddScoped<DataCleanerService<RegistrationEntity>>();
        services.AddScoped<DataCleanerService<EmailEntity>>();

        services.AddScoped<DataCleanerService<EmailEntity>>();
        services.AddScoped<IJob, EmailCleanerJob>();
        services.AddScoped<IJob, UserCleanerJob>();
        services.AddScoped<IJob, RegistrationCleanerJob>();
    }
}

