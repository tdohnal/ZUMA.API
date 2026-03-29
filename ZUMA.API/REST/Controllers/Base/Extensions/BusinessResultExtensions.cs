using Microsoft.AspNetCore.Mvc;

public static class BusinessResultExtensions
{
    public static IActionResult ToCreated<TS, TF>(this BusinessResult<TS, TF> result)
        where TS : class where TF : class
        => result.IsSuccess
            ? new ObjectResult(result.SuccessData) { StatusCode = 201 }
            : new UnprocessableEntityObjectResult(result.FailureData);

    public static IActionResult ToOk<TS, TF>(this BusinessResult<TS, TF> result)
        where TS : class where TF : class
        => result.IsSuccess
            ? new OkObjectResult(result.SuccessData)
            : new BadRequestObjectResult(result.FailureData);

    public static IActionResult ToOk<TS, TF, TResponse>(
    this BusinessResult<TS, TF> result,
    Func<TS, TResponse> mapper)
    where TS : class where TF : class
    {
        return result.IsSuccess
            ? new OkObjectResult(mapper(result.SuccessData!))
            : new NotFoundObjectResult(result.FailureData);
    }

    public static IActionResult ToNoContent<TS, TF>(this BusinessResult<TS, TF> result)
        where TS : class where TF : class
        => result.IsSuccess
            ? new NoContentResult()
            : new UnprocessableEntityObjectResult(result.FailureData);
}