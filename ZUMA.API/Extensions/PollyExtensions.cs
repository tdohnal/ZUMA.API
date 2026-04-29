using Polly;
using Polly.Extensions.Http;

namespace ZUMA.API.Extensions;

public static class PollyExtensions
{
    public static IServiceCollection AddZumaPolly(this IServiceCollection services)
    {
        // 1. Definujeme politiku pro Circuit Breaker (pokud se to 5x po sobě nepovede, na 30s stopneme provoz)
        var circuitBreakerPolicy = HttpPolicyExtensions
            .HandleTransientHttpError()
            .CircuitBreakerAsync(5, TimeSpan.FromSeconds(30));

        // 2. Definujeme Retry s exponenciálním zpětným rázem a Jitterem
        var retryPolicy = HttpPolicyExtensions
            .HandleTransientHttpError()
            .WaitAndRetryAsync(3, retryAttempt =>
                TimeSpan.FromSeconds(Math.Pow(2, retryAttempt)) // 2, 4, 8 sekund
                + TimeSpan.FromMilliseconds(new Random().Next(0, 100))); // Jitter

        // 3. Registrace pojmenovaného klienta s "Policy Wrap"
        services.AddHttpClient("ZumaExternalClient")
            .AddPolicyHandler(retryPolicy)
            .AddPolicyHandler(circuitBreakerPolicy);

        return services;

    }
}
