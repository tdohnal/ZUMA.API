using MassTransit;
using ZUMA.CommunicationService;
using ZUMA.CommunicationService.Consumers;
using ZUMA.SharedKernel.Configuration;

var builder = Host.CreateApplicationBuilder(args);

builder.Services.ConfigureServices(builder.Configuration);


builder.Services.AddHealthChecks();

builder.Services.AddMassTransit(x =>
{
    x.AddConsumers(typeof(EmailConsumer).Assembly);

    x.UsingRabbitMq((context, cfg) =>
    {
        var rabbitHost = Environment.GetEnvironmentVariable("RABBITMQ_HOST") ?? "rabbitmq";

        cfg.Host(rabbitHost, "/", h =>
        {
            h.Username("zuma_admin");
            h.Password("moje_tajne_heslo_123");
        });

        cfg.ConfigureEndpoints(context);
        cfg.UseMessageRetry(r => r.Interval(3, TimeSpan.FromSeconds(5)));
    });
});

builder.Services.AddHostedService<TcpHealthCheckListener>();

var host = builder.Build();

#region EF Migration

using (var scope = host.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    try
    {
        var context = services.GetRequiredService<CommunicationDbContext>();
        context.Database.Migrate();
    }
    catch (Exception ex)
    {
        var logger = services.GetRequiredService<ILogger<Program>>();
        logger.LogError("Migration Failed", ex);
    }
}

#endregion

host.Run();