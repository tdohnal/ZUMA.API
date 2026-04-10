using MassTransit;
using Microsoft.EntityFrameworkCore;
using ZUMA.CommunicationService;
using ZUMA.CommunicationService.Application.Consumers;
using ZUMA.CommunicationService.Infrastructure.Configuration;

var builder = Host.CreateApplicationBuilder(args);

builder.Services.AddInfrastructure(builder.Configuration);

builder.Services.AddHealthChecks()
    .AddNpgSql(builder.Configuration.GetConnectionString("DbConnection")!, name: "Customer DB");

builder.Services.AddMassTransit(x =>
{
    x.AddConsumers(typeof(EmailConsumer).Assembly);

    x.UsingRabbitMq((context, cfg) =>
    {
        var rabbitHost = Environment.GetEnvironmentVariable("RABBITMQ_HOST") ?? "rabbitmq";

        cfg.Host(rabbitHost, "/", h =>
        {
            h.Username(builder.Configuration["RabbitMQ__Username"] ?? "guest");
            h.Password(builder.Configuration["RabbitMQ__PASS"] ?? "guest");
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
        logger.LogError(ex, "An error occurred while migrating the database.");
        throw;
    }
}

#endregion

host.Run();