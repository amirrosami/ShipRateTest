using System.Security.Claims;

namespace ShipRate.WebApp.Middlewares
{



    public class ServiceAuthorizationMiddleware
    {
        private readonly RequestDelegate _next;

        public ServiceAuthorizationMiddleware(RequestDelegate next)
        {
            _next = next;
        }
        public async Task InvokeAsync(HttpContext context)
        {
            // get service name from endpoint
            // 
            var endpoint = context.GetEndpoint();
            var user = context.User.Claims.Where(x => x.Type == ClaimTypes.Name).FirstOrDefault()?.Value;

            // query to database to check that user have access to this service or not ? 
            await _next(context);
        }
    }
}
