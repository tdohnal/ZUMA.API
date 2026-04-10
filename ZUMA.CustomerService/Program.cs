using MassTransit;
using Microsoft.EntityFrameworkCore;
using ZUMA.CustomerService;
using ZUMA.CustomerService.Application.Consumers;
using ZUMA.CustomerService.Infrastructure.Configuration;
using ZUMA.CustomerService.Infrastructure.Persistence;

var builder = Host.CreateApplicationBuilder(args);

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
        var rabbitHost = Environment.GetEnvironmentVariable("RABBITMQ_HOST") ?? "rabbitmq";

        cfg.Host(rabbitHost, "/", h =>
        {
            // Sjednocené s API a Communication
            h.Username(builder.Configuration["RabbitMQ__USERNAME"] ?? "guest");
            h.Password(builder.Configuration["RABBITMQ__PASSWORD"] ?? "guest");
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