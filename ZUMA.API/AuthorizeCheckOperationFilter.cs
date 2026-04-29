using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.OpenApi;
using Microsoft.OpenApi;

public class SecurityRequirementsTransformer : IOpenApiOperationTransformer
{
    public Task TransformAsync(OpenApiOperation operation, OpenApiOperationTransformerContext context, CancellationToken cancellationToken)
    {
        // Kontrola, zda endpoint vyžaduje autorizaci
        var hasAuthorize = context.Description.ActionDescriptor.EndpointMetadata
            .Any(m => m is IAuthorizeData);

        // Kontrola, zda není povolen anonymní přístup (přebíjí Authorize)
        var allowAnonymous = context.Description.ActionDescriptor.EndpointMetadata
            .Any(m => m is IAllowAnonymous);

        if (hasAuthorize && !allowAnonymous)
        {
            // Přidání chybových kódů
            operation.Responses.TryAdd("401", new OpenApiResponse { Description = "Unauthorized" });
            operation.Responses.TryAdd("403", new OpenApiResponse { Description = "Forbidden" });

            //// Definice bezpečnostního schématu
            //var scheme = new OpenApiSecurityScheme
            //{
            //    Reference = new OpenApiReference
            //    {
            //        Type = ReferenceType.SecurityScheme,
            //        Id = JwtBearerDefaults.AuthenticationScheme // Obvykle "Bearer"
            //    }
            //};

            //operation.Security =
            //[
            //    new() { [scheme] = Array.Empty<string>() }
            //];
        }

        return Task.CompletedTask;
    }
}