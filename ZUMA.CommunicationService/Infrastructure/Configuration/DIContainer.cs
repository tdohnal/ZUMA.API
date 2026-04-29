using Microsoft.EntityFrameworkCore;
using ZUMA.CommunicationService.Application.Configuration;
using ZUMA.CommunicationService.Application.Consumers;
using ZUMA.CommunicationService.Domain.Interfaces;
using ZUMA.CommunicationService.Infrastructure.ExternalServices;
using ZUMA.CommunicationService.Infrastructure.Repositories;
using ZUMA.SharedKernel.Infrastructure.Extensions;

namespace ZUMA.CommunicationService.Infrastructure.Configuration;

public static class DIContainer
{
    public static void AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddApplication(configuration);

        #region Contexts

        services.AddDbContext<CommunicationDbContext>(options =>
            options.UseNpgsql(
                configuration.GetConnectionString("DbConnection")
                ?? throw new InvalidOperationException("ConnectionString 'DbConnection' not found.")));

        #endregion

        services.Configure<SmtpOptions>(configuration.GetSection("Smtp"));

        #region Email 

        services.AddScoped<IEmailRepository, EmailRepository>();
        services.AddScoped<IEmailClient, SmtpEmailClient>();

        #endregion

        #region MassTransit

        services.AddZumaMassTransitWorker<CommunicationDbContext>(configuration, typeof(EmailConsumer).Assembly);

        #endregion
    }
}
