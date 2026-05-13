using DotNetEnv;
using Microsoft.EntityFrameworkCore;
using Serilog;
using ZUMA.CustomerService;
using ZUMA.CustomerService.Infrastructure.Configuration;
using ZUMA.CustomerService.Infrastructure.Persistence;
using ZUMA.SharedKernel.Infrastructure.Extensions;

HostApplicationBuilder builder = Host.CreateApplicationBuilder(args);

Env.TraversePath().Load();
builder.Configuration.AddEnvironmentVariables();

#region Serilog

builder.SetZumaLoggerConfigurationSerilog();
builder.Services.AddSerilog();

#endregion

#region OpenTelemetry

var otlpEndpoint = builder.Configuration["OTEL_EXPORTER_OTLP_ENDPOINT"];
builder.Services.AddZumaTelemetry("ZUMA.API", otlpEndpoint);

#endregion

#region Cache

builder.Services.AddStackExchangeRedisCache(options =>
{
    options.Configuration = builder.Configuration["REDIS_CONNECTION"];
    options.InstanceName = "ZUMA_";
});

#endregion

// 1. Infrastruktura
builder.Services.AddInfrastructure(builder.Configuration);

// 2. Health Checks (Nezapomeň nainstalovat balíček AspNetCore.HealthChecks.NpgSql)
builder.Services.AddHealthChecks()
    .AddNpgSql(builder.Configuration.GetConnectionString("DbConnection")!, name: "Customer DB");


builder.Services.AddHostedService<TcpHealthCheckListener>();

IHost host = builder.Build();

#region EF Migration
using (IServiceScope scope = host.Services.CreateScope())
{
    IServiceProvider services = scope.ServiceProvider;
    try
    {
        CustomerDbContext context = services.GetRequiredService<CustomerDbContext>();
        context.Database.Migrate();
        Console.WriteLine("[Migration] Success: Customer Database is up to date.");
    }
    catch (Exception ex)
    {
        ILogger<Program> logger = services.GetRequiredService<ILogger<Program>>();
        logger.LogError(ex, "Migration Failed");
        throw;
    }
}
#endregion

host.Run();

[System.Diagnostics.CodeAnalysis.ExcludeFromCodeCoverage]
public partial class Program { }