using ShipRate.Domain.Entities.Marketing;

namespace ShipRate.Application.ViewModels
{
    public class QuickCallInitiateDto
    {
        public string PhoneNumber { get; set; } = string.Empty;
        public string? CustomerName { get; set; }
        public string? Subject { get; set; }
        public string? InquiryReference { get; set; }
    }

    public class SaveCallResultDto
    {
        public Guid CallLogId { get; set; }
        public int DurationSeconds { get; set; }
        public CallOutcome Outcome { get; set; }
        public string? Notes { get; set; }
        public DateTime? FollowUpDate { get; set; }
    }
}
