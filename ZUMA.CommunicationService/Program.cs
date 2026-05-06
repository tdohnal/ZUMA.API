using DotNetEnv;
using Microsoft.EntityFrameworkCore;
using Serilog;
using ZUMA.CommunicationService;
using ZUMA.CommunicationService.Infrastructure.Configuration;
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

builder.Services.AddInfrastructure(builder.Configuration);

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
        CommunicationDbContext context = services.GetRequiredService<CommunicationDbContext>();
        context.Database.Migrate();
    }
    catch (Exception ex)
    {
        ILogger<Program> logger = services.GetRequiredService<ILogger<Program>>();
        logger.LogError(ex, "An error occurred while migrating the database.");
        throw;
    }
}

#endregion

host.Run();