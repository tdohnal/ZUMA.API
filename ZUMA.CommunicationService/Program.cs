using DotNetEnv;
using MassTransit;
using Microsoft.EntityFrameworkCore;
using ZUMA.CommunicationService;
using ZUMA.CommunicationService.Application.Consumers;
using ZUMA.CommunicationService.Infrastructure.Configuration;
using ZUMA.CommunicationService.Infrastructure.Extensions;

var builder = Host.CreateApplicationBuilder(args);

Env.TraversePath().Load();
builder.Configuration.AddEnvironmentVariables();

#region Serilog

builder.AddZumaSerilog();

#endregion

builder.Services.AddInfrastructure(builder.Configuration);

builder.Services.AddHealthChecks()
    .AddNpgSql(builder.Configuration.GetConnectionString("DbConnection")!, name: "Customer DB");

#region MassTransit

builder.Services.AddMassTransit(x =>
{
    x.AddConsumers(typeof(EmailConsumer).Assembly);

    x.UsingRabbitMq((context, cfg) =>
    {
        var rabbitHost = builder.Configuration["RABBITMQ:HOST"];
        var username = builder.Configuration["RABBITMQ:USERNAME"];
        var password = builder.Configuration["RABBITMQ:PASSWORD"];

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

        cfg.UseMessageRetry(r => r.Exponential(3,
            TimeSpan.FromSeconds(2),
            TimeSpan.FromSeconds(30),
            TimeSpan.FromSeconds(5)));

        cfg.UseCircuitBreaker(cb =>
        {
            cb.TripThreshold = 15;
            cb.ActiveThreshold = 10;
            cb.ResetInterval = TimeSpan.FromMinutes(5);
        });

        cfg.ConfigureEndpoints(context);
    });
});

#endregion

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