
namespace ShipRate.Application.Interfaces
{
    public interface ICurrentCompanyService
    {
        int? CompanyId { get; }
        Task<int?> GetCompanyIdAsync();
        Task<int> GetRequiredCompanyIdAsync();
        Task<bool> HasAccessAsync(int companyId);
        Task<bool> HasAccessToCurrentCompanyAsync();
        Task SetCurrentCompanyAsync(int companyId);
        Task EnsureCurrentCompanyAsync();
        Task<string?> GetCurrentCompanyNameAsync();
        Task<List<CurrentCompanyItemDto>> GetAccessibleCompaniesAsync();
        void Clear();
    }

    public class CurrentCompanyItemDto
    {
        public int Id { get; set; }

        public string Name { get; set; } = "";
    }

}
