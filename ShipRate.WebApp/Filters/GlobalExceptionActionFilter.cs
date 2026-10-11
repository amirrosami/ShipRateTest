
namespace ShipRate.WebApp.Filters
{
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.AspNetCore.Mvc.Filters;
    using ShipRate.Common.Exceptions;

    public class GlobalExceptionActionFilter : IAsyncActionFilter
    {
        public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
        {
            var executedContext = await next();

            var exception = executedContext.Exception;
            if (exception == null)
                return;

            var http = context.HttpContext;
            var errorMessage = "an error ocuured! contact admin";
            var isHtmx = http.Request.Headers["HX-Request"] == "true";

            if (exception is GeneralAppException)
            {
                errorMessage = exception.Message;
            }

            if (isHtmx)
            {
                executedContext.Result = new ContentResult
                {
                    StatusCode = 400,
                    Content = errorMessage,
                    ContentType = "text/plain"
                };

                executedContext.ExceptionHandled = true;
                return;
            }

            // Non‑HTMX request
            //var controller = context.Controller as Controller;

            //if (controller != null)
            //{

            executedContext.Result = new RedirectToActionResult(
            actionName: "Index",
            controllerName: "Error",
            routeValues: new { errorMessage, });
            executedContext.ExceptionHandled = true;
        }

    }
}
