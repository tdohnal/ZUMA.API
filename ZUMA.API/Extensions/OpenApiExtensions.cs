using Microsoft.OpenApi;
using System.Reflection;

namespace ZUMA.API.Extensions;

public static class OpenApiExtensions
{
    public static IServiceCollection AddZumaOpenApi(this IServiceCollection services)
    {
        string deploymentVersion = Assembly.GetEntryAssembly()?.GetName().Version?.ToString() ?? "unknown";

        services.AddOpenApi(options =>
        {
            options.AddOperationTransformer<SecurityRequirementsTransformer>();

            options.AddDocumentTransformer((document, context, cancellationToken) =>
            {
                document.Info.Title = "ZUMA API";
                document.Info.Version = "v1";
                document.Info.Description = $"Build version `{deploymentVersion}`";
                document.Info.Contact = new OpenApiContact { Name = "ZUMA Team", Email = "tomas.dohnal46@seznam.cz" };

                var securityScheme = new OpenApiSecurityScheme
                {
                    Type = SecuritySchemeType.Http,
                    Name = "Authorization",
                    Scheme = "Bearer",
                    BearerFormat = "JWT",
                    In = ParameterLocation.Header,
                    Description = "Vložte pouze JWT token."
                };

                document.Components ??= new OpenApiComponents();

                document.Components.SecuritySchemes ??= new Dictionary<string, IOpenApiSecurityScheme>();

                document.Components.SecuritySchemes.Add("Bearer", securityScheme);

                return Task.CompletedTask;
            });
        });

        return services;
    }
}