namespace ShipRate.Application.Interfaces
{
    public interface IRateInquiryEmailService
    {
        Task<string> BuildInquiryEmailHtmlAsync(Guid inquiryId, bool canSend, bool isPreview);
        Task SendInquiryEmailAsync(Guid inquiryId);
        Task<bool> SendInquiryInAppAsync(Guid inquiryId);
        Task<int> SendAllDraftInquiryEmailsAsync(Guid rateRequestId);
        Task<bool> SendSellQuoteInAppAsync(Guid quoteId);
        Task<bool> AcceptSellQuoteInAppAsync(Guid quoteId, string respondComments = "");
        Task<bool> RejectSellQuoteInAppAsync(Guid quoteId, string respondComments = "");
        Task<string> BuildSellQuoteEmailHtmlAsync(Guid quoteId, bool canSendEmail, bool isPreview);
        Task SendSellQuoteEmailAsync(Guid quoteId);
    }
}
