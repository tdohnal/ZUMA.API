using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace ZUMA.API.Attributes;

public class ValidationFilterAttribute : ActionFilterAttribute
{
    public override void OnActionExecuting(ActionExecutingContext context)
    {
        if (!context.ModelState.IsValid)
        {
            Dictionary<string, string[]> errors = context.ModelState
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