using Serilog;
using Serilog.Events;
using System.Reflection;

namespace ZUMA.CustomerService.Infrastructure.Extensions;

public static class SerilogExtensions
{
    public static void AddZumaSerilog(this IHostApplicationBuilder host)
    {
        AssemblyName? assemblyName = Assembly.GetEntryAssembly()?.GetName();
        string? serviceName = assemblyName?.Name;
        string? version = assemblyName?.Version?.ToString();

        Log.Logger = new LoggerConfiguration()
            .MinimumLevel.Information()
            .MinimumLevel.Override("Microsoft", LogEventLevel.Warning)
            .Enrich.FromLogContext()
            .Enrich.WithProperty("ServiceSource", serviceName)
            .Enrich.WithProperty("BuildVersion", version)
            .WriteTo.Console(outputTemplate: "[{Timestamp:HH:mm:ss} {Level:u3}] ({ServiceSource}) {Message:lj}{NewLine}{Exception}")
            .WriteTo.Seq(host.Configuration["SERILOG:SEQ:URL"]
                ?? throw new Exception("Key SERILOG:SEQ:URL not found in configuration!"))
            .CreateLogger();

        host.Services.AddSerilog();
    }
}