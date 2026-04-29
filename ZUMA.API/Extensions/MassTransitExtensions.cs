using MassTransit;

namespace ZUMA.API.Extensions;

public static class MassTransitExtensions
{
    public static IServiceCollection AddZumaMassTransit(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddMassTransit(x =>
        {
            x.UsingRabbitMq((context, cfg) =>
            {

                var rabbitHost = configuration["RABBITMQ:HOST"];
                var username = configuration["RABBITMQ:USERNAME"];
                var password = configuration["RABBITMQ:PASSWORD"];

                if (string.IsNullOrWhiteSpace(rabbitHost))
                    throw new NullReferenceException($"{nameof(rabbitHost)} IS NULL");

                if (string.IsNullOrWhiteSpace(username))
                    throw new NullReferenceException($"{nameof(username)} IS NULL");

                if (string.IsNullOrWhiteSpace(password))
                    throw new NullReferenceException($"{nameof(password)} IS NULL");

                cfg.Host(rabbitHost, "/", h =>
                {
                    h.Username(username);
                    h.Password(password);
                });

                cfg.ConfigureEndpoints(context);
            });
        });

        return services;
    }
}
