namespace ShipRate.Application.Interfaces
{
    public interface IRazorViewRenderService
    {
        Task<string> RenderToStringAsync<TModel>(string viewName, TModel model);
    }
}
