using Microsoft.OpenApi.Models;
using System.Reflection;

namespace ZUMA.API.Extensions;

public static class SwaggerExtensions
{
    public static IServiceCollection AddZumaSwagger(this IServiceCollection services)
    {
        string deploymentVersion = Assembly.GetEntryAssembly()?.GetName().Version?.ToString() ?? "unknown";

        return services.AddSwaggerGen(options =>
        {
            OpenApiInfo apiInfo = new()
            {
                Title = "ZUMA API",
                Version = "v1",
                Contact = new OpenApiContact { Name = "ZUMA Team", Email = "tomas.dohnal46@seznam.cz" },
                License = new OpenApiLicense { Name = "MIT" },
                Description = $"Build version `{deploymentVersion}`"
            };

            options.SwaggerDoc("v1", apiInfo);

            options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
            {
                Name = "Authorization",
                Type = SecuritySchemeType.Http,
                Scheme = "Bearer",
                BearerFormat = "JWT",
                In = ParameterLocation.Header,
                Description = "Paste only JWT token."
            });

            options.OperationFilter<AuthorizeCheckOperationFilter>();

            string xmlFile = Path.Combine(AppContext.BaseDirectory, "ZUMA.API.xml");
            if (File.Exists(xmlFile)) options.IncludeXmlComments(xmlFile);
        });
    }
}
