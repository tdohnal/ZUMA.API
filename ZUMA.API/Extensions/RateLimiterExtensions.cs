using Microsoft.AspNetCore.RateLimiting;

namespace ZUMA.API.Extensions;

public static class RateLimiterExtensions
{
    public static IServiceCollection AddZumaRateLimiter(this IServiceCollection services)
    {
        return services.AddRateLimiter(options =>
        {
            options.RejectionStatusCode = StatusCodes.Status429TooManyRequests;

            options.AddFixedWindowLimiter("auth-limit", opt =>
            {
                opt.Window = TimeSpan.FromMinutes(2);
                opt.PermitLimit = 3;
                opt.QueueLimit = 0;
            });
        });
    }
}
