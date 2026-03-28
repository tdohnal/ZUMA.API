using Microsoft.Extensions.Diagnostics.HealthChecks;

namespace ZUMA.EmailService
{
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
            var listener = new System.Net.Sockets.TcpListener(System.Net.IPAddress.Any, _port);
            listener.Start();

            while (!stoppingToken.IsCancellationRequested)
            {
                using var client = await listener.AcceptTcpClientAsync(stoppingToken);
                var report = await _healthCheckService.CheckHealthAsync(stoppingToken);

                if (report.Status == HealthStatus.Healthy)
                {
                    // Pokud jsme Healthy, prostě zavřeme spojení (OK)
                    client.Close();
                }
            }
        }
    }
}
