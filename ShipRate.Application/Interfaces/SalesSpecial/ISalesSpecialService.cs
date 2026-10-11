using Microsoft.AspNetCore.Mvc.Rendering;
namespace ShipRate.Application.Interfaces.SalesSpecial
{
    public interface ISalesSpecialService
    {
        Task<List<SelectListItem>> GetSalesUsersAsync();
        Task<int> GetSalesUserNewFileNoAsync(Guid userId);
        Task<string> GetSalesPrefixAsync(Guid userId);
    }
}