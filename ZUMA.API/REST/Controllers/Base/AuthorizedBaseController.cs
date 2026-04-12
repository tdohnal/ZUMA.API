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
    protected Guid AuthorizedUserId => GetUserIdOrThrow();

    private Guid GetUserIdOrThrow()
    {
        var claimValue = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;

        if (string.IsNullOrEmpty(claimValue) || !Guid.TryParse(claimValue, out var userGuid))
        {
            throw new UnauthorizedAccessException("User ID is missing or invalid in the JWT token.");
        }

        return userGuid;
    }
}
