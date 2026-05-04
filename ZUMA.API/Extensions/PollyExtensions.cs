using Polly;
using Polly.Extensions.Http;

namespace ZUMA.API.Extensions;

public static class PollyExtensions
{
    public static IServiceCollection AddZumaPolly(this IServiceCollection services)
    {
        var circuitBreakerPolicy = HttpPolicyExtensions
            .HandleTransientHttpError()
            .CircuitBreakerAsync(5, TimeSpan.FromSeconds(30));

        var retryPolicy = HttpPolicyExtensions
            .HandleTransientHttpError()
            .WaitAndRetryAsync(3, retryAttempt =>
                TimeSpan.FromSeconds(Math.Pow(2, retryAttempt)) // 2, 4, 8
                + TimeSpan.FromMilliseconds(new Random().Next(0, 100))); // Jitter

        services.AddHttpClient("ZumaExternalClient")
            .AddPolicyHandler(retryPolicy)
            .AddPolicyHandler(circuitBreakerPolicy);

        return services;

    }
}
