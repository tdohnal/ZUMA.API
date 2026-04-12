using DotNetEnv;
using MassTransit;
using Microsoft.EntityFrameworkCore;
using Serilog;
using ZUMA.CustomerService;
using ZUMA.CustomerService.Application.Consumers;
using ZUMA.CustomerService.Infrastructure.Configuration;
using ZUMA.CustomerService.Infrastructure.Persistence;

var builder = Host.CreateApplicationBuilder(args);

Env.TraversePath().Load();
builder.Configuration.AddEnvironmentVariables();

#region Serilog

Log.Logger = new LoggerConfiguration()
    .MinimumLevel.Information() // Pro vývoj v Workeru doporučuji Debug
    .Enrich.FromLogContext()
    .Enrich.WithProperty("Application", "ZUMA.CustomerService") // Aby ses v Seque vyznal
    .WriteTo.Seq(builder.Configuration["SERILOG:SEQ:URL"] ?? throw new Exception("key [Serilog:WriteTo:0:Args:serverUrl] IS NOT CONFIGURED!"))
    .CreateLogger();

builder.Services.AddSerilog();

#endregion

// 1. Infrastruktura
builder.Services.AddInfrastructure(builder.Configuration);

// 2. Health Checks (Nezapomeň nainstalovat balíček AspNetCore.HealthChecks.NpgSql)
builder.Services.AddHealthChecks()
    .AddNpgSql(builder.Configuration.GetConnectionString("DbConnection")!, name: "Customer DB");

// 3. MassTransit - RabbitMQ dynamicky
builder.Services.AddMassTransit(x =>
{
    x.AddConsumers(typeof(RegistrationCreateConsumer).Assembly);

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
        var context = services.GetRequiredService<CustomerDbContext>();
        context.Database.Migrate();
        Console.WriteLine("[Migration] Success: Customer Database is up to date.");
    }
    catch (Exception ex)
    {
        var logger = services.GetRequiredService<ILogger<Program>>();
        logger.LogError(ex, "Migration Failed");
        throw;
    }
}
#endregion

host.Run();