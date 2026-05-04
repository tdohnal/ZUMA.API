using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.OpenApi;
using Microsoft.OpenApi;

public class SecurityRequirementsTransformer : IOpenApiOperationTransformer
{
    public Task TransformAsync(OpenApiOperation operation, OpenApiOperationTransformerContext context, CancellationToken cancellationToken)
    {
        var hasAuthorize = context.Description.ActionDescriptor.EndpointMetadata
            .Any(m => m is IAuthorizeData);

        var allowAnonymous = context.Description.ActionDescriptor.EndpointMetadata
            .Any(m => m is IAllowAnonymous);

        if (hasAuthorize && !allowAnonymous)
        {
            operation.Responses.TryAdd("401", new OpenApiResponse { Description = "Unauthorized" });
            operation.Responses.TryAdd("403", new OpenApiResponse { Description = "Forbidden" });
        }

        return Task.CompletedTask;
    }
}