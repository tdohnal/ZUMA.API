using Microsoft.AspNetCore.Mvc;

namespace ZUMA.API.REST.Base;

[ApiController]
[Produces("application/json")]
[Consumes("application/json")]
[Route("api/v{version:apiVersion}/[controller]")]
public class ZumaBaseController
{
}
