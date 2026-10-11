using ShipRate.Application.DTOs;

namespace ShipRate.Application.Interfaces.Messaging
{
    public interface ISmsManagementService
    {
        Task<SmsStatsDto> GetSmsStatsAsync();
        Task<List<SmsOutboxItemDto>> GetPagedOutboxAsync(SmsOutboxFilterDto filter);
        Task<List<SmsIncomingItemDto>> GetPagedIncomingAsync(SmsIncomingFilterDto filter);
        Task<(bool Success, string Message)> SendManualSmsAsync(SendManualSmsDto dto);
        Task<(bool Success, string Message)> RetryFailedSmsAsync(Guid recipientId);
    }
}
