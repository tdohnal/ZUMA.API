using Microsoft.AspNetCore.Mvc;

namespace ZUMA.API.REST.Controllers.Base;

[ApiController]
[Produces("application/json")]
[Consumes("application/json")]
[Route("api/v1/[controller]")]
public class BaseController : ControllerBase
{
    protected IActionResult ProcessResult<TS, TF>(
         BusinessResult<TS, TF> result,
         Func<TS, IActionResult>? onSuccess = null,
         Func<TF, IActionResult>? onFailure = null)
         where TS : class
         where TF : class
    {
        if (result.IsSuccess)
        {
            return onSuccess != null
                ? onSuccess(result.SuccessData!)
                : Ok(result.SuccessData);
        }

        return onFailure != null
            ? onFailure(result.FailureData!)
            : BadRequest(result.FailureData);
    }
}


