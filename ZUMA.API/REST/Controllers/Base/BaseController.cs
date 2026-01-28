using Microsoft.AspNetCore.Mvc;

namespace ZUMA.API.REST.Controllers.Base;

[ApiController]
[Produces("application/json")]
[Consumes("application/json")]
[Route("api/v{version:apiVersion}/[controller]")]
public class BaseController : ControllerBase
{
}
