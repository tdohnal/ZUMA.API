using MassTransit;
using Microsoft.EntityFrameworkCore;
using Serilog;
using ZUMA.CommunicationService;
using ZUMA.CommunicationService.Application.Consumers;
using ZUMA.CommunicationService.Infrastructure.Configuration;

var builder = Host.CreateApplicationBuilder(args);

#region Serilog

Log.Logger = new LoggerConfiguration()
    .MinimumLevel.Information()
    .Enrich.FromLogContext()
    .Enrich.WithProperty("Application", "ZUMA.CommunicationService")
    .WriteTo.Seq(builder.Configuration["Serilog:WriteTo:0:Args:serverUrl"] ?? throw new Exception("key [Serilog:WriteTo:0:Args:serverUrl] IS NOT CONFIGURED!"))
    .CreateLogger();

#endregion

builder.Services.AddInfrastructure(builder.Configuration);

builder.Services.AddHealthChecks()
    .AddNpgSql(builder.Configuration.GetConnectionString("DbConnection")!, name: "Customer DB");

builder.Services.AddMassTransit(x =>
{
    x.AddConsumers(typeof(EmailConsumer).Assembly);

    x.UsingRabbitMq((context, cfg) =>
    {
        var rabbitHost = builder.Configuration["RABBITMQ:HOST"];
        var username = builder.Configuration["RABBITMQ:USERNAME"];
        var password = builder.Configuration["RABBITMQ:PASSWORD"];

        Console.WriteLine($"DEBUG: Připojuji se k Rabbitu: {rabbitHost} jako {username}");

        if (string.IsNullOrWhiteSpace(rabbitHost))
            throw new Exception("CHYBA: RABBITMQ__HOST nebyl nalezen v konfiguraci!");

        cfg.Host(rabbitHost, "/", h =>
        {
            h.Username(username ?? "guest");
            h.Password(password ?? "guest");
        });

        cfg.ConfigureEndpoints(context);
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