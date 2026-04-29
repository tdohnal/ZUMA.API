using Microsoft.Extensions.Diagnostics.HealthChecks;
using System.Net.Sockets;

namespace ZUMA.CustomerService;

public class TcpHealthCheckListener : BackgroundService
{
    private readonly HealthCheckService _healthCheckService;
    private readonly int _port = 8082;

    public TcpHealthCheckListener(HealthCheckService healthCheckService)
    {
        _healthCheckService = healthCheckService;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        TcpListener listener = new(System.Net.IPAddress.Any, _port);
        listener.Start();

        while (!stoppingToken.IsCancellationRequested)
        {
            using TcpClient client = await listener.AcceptTcpClientAsync(stoppingToken);
            HealthReport report = await _healthCheckService.CheckHealthAsync(stoppingToken);

            if (report.Status == HealthStatus.Healthy)
            {
                client.Close();
            }
        }
    }
}
