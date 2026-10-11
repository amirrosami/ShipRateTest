using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Abstractions;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Mvc.Razor;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.AspNetCore.Routing;
using ShipRate.Application.Interfaces;

namespace ShipRate.Application.Services
{
    public class RazorViewRenderService : IRazorViewRenderService
    {
        private readonly IRazorViewEngine _viewEngine;
        private readonly ITempDataProvider _tempDataProvider;
        private readonly IServiceProvider _serviceProvider;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public RazorViewRenderService(
            IRazorViewEngine viewEngine,
            ITempDataProvider tempDataProvider,
            IServiceProvider serviceProvider,
            IHttpContextAccessor httpContextAccessor)
        {
            _viewEngine = viewEngine;
            _tempDataProvider = tempDataProvider;
            _serviceProvider = serviceProvider;
            _httpContextAccessor = httpContextAccessor;
        }
        public async Task<string> RenderToStringAsync<TModel>(string viewName, TModel model)
        {
            var httpContext = _httpContextAccessor.HttpContext;

            if (httpContext == null)
            {
                httpContext = new DefaultHttpContext
                {
                    RequestServices = _serviceProvider
                };
            }

            var actionContext = new ActionContext(
                httpContext,
                new RouteData(),
                new ActionDescriptor()
            );

            using var sw = new StringWriter();

            var getViewResult = _viewEngine.GetView(
                executingFilePath: null,
                viewPath: viewName,
                isMainPage: false
            );

            var findViewResult = _viewEngine.FindView(
                actionContext,
                viewName,
                isMainPage: false
            );

            var viewResult = getViewResult.Success
                ? getViewResult
                : findViewResult;

            if (!viewResult.Success)
            {
                var searchedLocations = new List<string>();

                if (getViewResult.SearchedLocations != null)
                    searchedLocations.AddRange(getViewResult.SearchedLocations);

                if (findViewResult.SearchedLocations != null)
                    searchedLocations.AddRange(findViewResult.SearchedLocations);

                throw new InvalidOperationException(
                    $"View '{viewName}' not found. Searched locations:{Environment.NewLine}{string.Join(Environment.NewLine, searchedLocations)}"
                );
            }

            var viewDictionary = new ViewDataDictionary<TModel>(
                new EmptyModelMetadataProvider(),
                new ModelStateDictionary())
            {
                Model = model
            };

            var tempData = new TempDataDictionary(
                actionContext.HttpContext,
                _tempDataProvider
            );

            var viewContext = new ViewContext(
                actionContext,
                viewResult.View,
                viewDictionary,
                tempData,
                sw,
                new HtmlHelperOptions()
            );

            await viewResult.View.RenderAsync(viewContext);

            return sw.ToString();
        }

    }
}
