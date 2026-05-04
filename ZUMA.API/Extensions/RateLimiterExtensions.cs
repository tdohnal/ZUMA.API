using Microsoft.AspNetCore.RateLimiting;
using System.Threading.RateLimiting;

namespace ZUMA.API.Extensions;

public static class RateLimiterExtensions
{
    public static IServiceCollection AddZumaRateLimiter(this IServiceCollection services)
    {
        return services.AddRateLimiter(options =>
        {
            options.RejectionStatusCode = StatusCodes.Status429TooManyRequests;

            options.GlobalLimiter = PartitionedRateLimiter.Create<HttpContext, string>(httpContext =>
                RateLimitPartition.GetFixedWindowLimiter(
                    partitionKey: httpContext.Connection.RemoteIpAddress?.ToString() ?? "unknown",
                    factory: partition => new FixedWindowRateLimiterOptions
                    {
                        AutoReplenishment = true,
                        PermitLimit = 100,
                        Window = TimeSpan.FromMinutes(1)
                    }));

            options.AddFixedWindowLimiter("auth-limit", opt =>
            {
                opt.Window = TimeSpan.FromSeconds(30);
                opt.PermitLimit = 1;
                opt.QueueLimit = 0;
            });
        });
    }
}
