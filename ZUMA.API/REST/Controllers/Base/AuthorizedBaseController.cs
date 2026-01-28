using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ZUMA.API.REST.Controllers.Base;

[ApiController]
[Authorize]
[Produces("application/json")]
[Consumes("application/json")]
[Route("api/v{version:apiVersion}/[controller]")]
public class AuthorizedBaseController : ControllerBase
{
}
