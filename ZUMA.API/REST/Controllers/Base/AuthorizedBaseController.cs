using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ZUMA.API.REST.Controllers.Base;

[ApiController]
[Authorize]
[Produces("application/json")]
[Consumes("application/json")]
[Route("api/v1/[controller]")]
public class AuthorizedBaseController : ControllerBase
{
    protected HttpMethod CurrentHttpMethod => new(HttpContext.Request.Method);

    protected Guid AuthorizedUserId => GetUserIdOrThrow();

    private Guid GetUserIdOrThrow()
    {
        string? claimValue = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;

        if (string.IsNullOrEmpty(claimValue) || !Guid.TryParse(claimValue, out Guid userGuid))
        {
            throw new UnauthorizedAccessException("User ID is missing or invalid in the JWT token.");
        }

        return userGuid;
    }
}
