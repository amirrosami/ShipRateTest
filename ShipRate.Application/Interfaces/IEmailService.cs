namespace ShipRate.Application.Interfaces
{
    public interface IEmailService
    {
        Task SendEmailAsync(string to, string subject, string body, bool isHtml = true);
        Task SendEmailAsync(List<string> toList, string subject, string body, bool isHtml = true);


        Task SendEmailViaMailKitAsync(string to, string subject, string body, bool isHtml = true, string? replyTo = null, string? companyLogoContentId = null);
        Task SendEmailViaMailKitAsync(List<string> toList, string subject, string body, bool isHtml = true, string? replyTo = null, string? companyLogoContentId = null);
        Task SendEmailViaMailKitAdvancedAsync(
            List<string> toList,
            string subject,
            string body,
            bool isHtml = true,
            string? replyToEmail = null,
            string? replyToName = null,
            string? customFromEmail = null,
            string? customFromName = null,
            List<string>? ccList = null,
            List<string>? bccList = null,
            CancellationToken cancellationToken = default,
            string? companyLogoContentId = null);
        Task SendEmailWithAttachmentAsync(string to, string subject, string body, byte[] attachmentBytes, string attachmentFileName, CancellationToken ct = default);
    }


}

