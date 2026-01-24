using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace ZUMA.API.REST.Filters;

/// <summary>
/// Action Filter pro validaci ModelState.
/// Automaticky vrátí 422 Unprocessable Entity pokud je model neplatný.
/// </summary>
public class ValidationFilterAttribute : ActionFilterAttribute
{
    public override void OnActionExecuting(ActionExecutingContext context)
    {
        if (!context.ModelState.IsValid)
        {
            var errors = context.ModelState
                .Where(ms => ms.Value?.Errors.Count > 0)
                .ToDictionary(
                    ms => ms.Key,
                    ms => ms.Value!.Errors.Select(e => e.ErrorMessage).ToArray());

            context.Result = new UnprocessableEntityObjectResult(new
            {
                message = "Validace selhala",
                errors = errors
            });
        }

        base.OnActionExecuting(context);
    }
}