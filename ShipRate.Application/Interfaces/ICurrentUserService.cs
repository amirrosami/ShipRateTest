namespace ShipRate.Application.Interfaces
{
    public interface ICurrentUserService
    {
        Guid UserId { get; }
        string UserName { get; }
        Task<string> EmailAddress();
        bool IsAuthenticated { get; }
        bool IsInRole(string role);
        Task<string?> GetUserFullNameAsync();
        Task<Guid?> GetPersonIdAsync();
        Task<int> NumberOfCompaniesAsync();
        Task<int> CurrentSelectedCompanyIdAsync();
        bool IsSalesUser();
        bool IsAdmin();
        bool IsManager();
        bool IsSalesManager();
        bool IsOnlySalesUser();
        bool IsOnlyFinanceExpert();
    }
}