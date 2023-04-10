using Core.Entities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace Web.Filters;

public class ValidationFilter : ActionFilterAttribute
{
    public void OnActionExecuting(ActionExecutingContext context)
    {
        var param = context.ActionArguments.SingleOrDefault(p => p.Value is BaseEntity);
        if (param.Value == null)
        {
            context.Result = new BadRequestObjectResult("Object is null");
            return;
        }

        if (!context.ModelState.IsValid)
        {
            context.Result = new UnprocessableEntityObjectResult(context.ModelState);
        }
    }
}
