using ShipRate.Application.ViewModels;
using ShipRate.Domain.Entities.Marketing;

namespace ShipRate.Application.Interfaces.Marketing
{
    public interface ICiscoTelephonyService
    {
        Task<bool> DialToCiscoPhoneAsync(string phoneIp, string destinationNumber);
    }
    public interface IMarketingCallService
    {
        Task<(bool Success, Guid CallLogId, string Message)> StartCallAsync(QuickCallInitiateDto dto, string userId, string userExt, string phoneIp);
        Task<bool> SaveCallResultAsync(SaveCallResultDto dto);
        Task<List<MarketingCallLog>> GetRecentCallsAsync(int count = 10);
    }
}
