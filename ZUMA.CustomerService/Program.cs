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
        var rabbitHost = Environment.GetEnvironmentVariable("RABBITMQ:HOST");

        if (string.IsNullOrWhiteSpace(rabbitHost))
            throw new Exception("RABBITMQ__HOST IS NULL");

        cfg.Host(rabbitHost, "/", h =>
        {
            var username = builder.Configuration["RabbitMQ:USERNAME"];
            var password = builder.Configuration["RabbitMQ:PASSWORD"];

            if (string.IsNullOrWhiteSpace(username))
                throw new Exception("RabbitMQ__USERNAME IS NULL");

            if (string.IsNullOrWhiteSpace(password))
                throw new Exception("RabbitMQ__PASSWORD IS NULL");

            h.Username(username);
            h.Password(password);
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