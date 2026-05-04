using Serilog;
using Serilog.Events;
using System.Reflection;

namespace ZUMA.API.Extensions;

public static class SerilogExtensions
{
    public static void AddZumaSerilog(this IHostApplicationBuilder builder)
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
            .WriteTo.Seq(builder.Configuration["SERILOG:SEQ:URL"]
                ?? throw new Exception("KEY SERILOG:SEQ:URL not found!"))
            .CreateLogger();

        builder.Services.AddSerilog();
    }
}