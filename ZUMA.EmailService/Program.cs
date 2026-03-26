using MassTransit;
using ZUMA.BusinessLogic.Configuration; // Předpokládám, že tady máš svoje DI rozšíření
using ZUMA.EmailService;

var builder = Host.CreateApplicationBuilder(args);

// Pokud tvoje ConfigureServices registruje IEmailService a DB, nech to tu
builder.Services.ConfigureServices(builder.Configuration);

builder.Services.AddMassTransit(x =>
{
    x.AddConsumer<EmailConsumer>();

    x.UsingRabbitMq((context, cfg) =>
    {
        // DŮLEŽITÉ: V Dockeru musí být "rabbitmq", pro lokální ladění bez Dockeru "localhost"
        var host = Environment.GetEnvironmentVariable("RABBITMQ_HOST") ?? "rabbitmq";

        cfg.Host(host, "/", h =>
        {
            // Tady musí být ty údaje, co jsi nastavil v docker-compose
            h.Username("zuma_admin");
            h.Password("moje_tajne_heslo_123");
        });

        cfg.ReceiveEndpoint("email-service-queue", e =>
        {
            // Toto propojí frontu s tvým Consumerem
            e.ConfigureConsumer<EmailConsumer>(context);
        });
    });
});

var host = builder.Build();
host.Run();