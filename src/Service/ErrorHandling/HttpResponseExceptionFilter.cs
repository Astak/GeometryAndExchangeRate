using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace GeometryAndExchangeRate.Service.ErrorHandling;

public class HttpResponseExceptionFilter : IActionFilter, IOrderedFilter {
    public int Order => int.MaxValue - 10;
    public void OnActionExecuting(ActionExecutingContext context) {}
    public void OnActionExecuted(ActionExecutedContext context) {
        if(context.Exception is UserFriendlyException ex) {
            context.Result = new JsonResult(ex.ErrorData) {
                StatusCode = StatusCodes.Status500InternalServerError
            };
            context.ExceptionHandled = true;
        }
    }
}