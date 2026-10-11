using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using MimeKit;
using ShipRate.Application.Interfaces;
using System.Net;
using System.Net.Mail;
using MailKitSmtpClient = MailKit.Net.Smtp.SmtpClient; // جلوگیری از تداخل نام SmtpClient

namespace ShipRate.Application.Services
{
    public class EmailService : IEmailService
    {
        private readonly IConfiguration _configuration;
        private readonly IWebHostEnvironment _env;

        public EmailService(IConfiguration configuration, IWebHostEnvironment env)
        {
            _configuration = configuration;
            _env = env;
        }
        public async Task SendEmailAsync(string to, string subject, string body, bool isHtml = true)
        {
            await SendEmailAsync(new List<string> { to }, subject, body, isHtml);
        }
        public async Task SendEmailAsync(List<string> toList, string subject, string body, bool isHtml = true)
        {
            var smtpHost = _configuration["EmailSettings:SmtpHost"];
            var smtpPort = int.Parse(_configuration["EmailSettings:SmtpPort"]);
            var smtpUser = _configuration["EmailSettings:SmtpUser"];
            var smtpPass = _configuration["EmailSettings:SmtpPass"];
            var fromEmail = _configuration["EmailSettings:FromEmail"];
            var fromName = _configuration["EmailSettings:FromName"];

            var fakeEnabled = bool.Parse(_configuration["EmailSettings:FakeEnabled"]);
            var fakeToEmailAddress = _configuration["EmailSettings:Fake_To_EmailAddress"];


            using var client = new SmtpClient(smtpHost, smtpPort)
            {
                Credentials = new NetworkCredential(smtpUser, smtpPass),
                EnableSsl = false
            };

            var message = new MailMessage
            {
                From = new MailAddress(fromEmail, fromName),
                Subject = subject,
                Body = body,
                IsBodyHtml = isHtml,
            };

            foreach (var to in toList.Where(e => !string.IsNullOrWhiteSpace(e)))
            {
                if (fakeEnabled)
                    message.To.Add(fakeToEmailAddress);
                else
                    message.To.Add(to);
            }
            await client.SendMailAsync(message);
        }
        #region New Methods (MailKit)
        public async Task SendEmailViaMailKitAsync(string to, string subject, string body, bool isHtml = true, string? replyTo = null, string? companyLogoContentId = null)
        {
            await SendEmailViaMailKitAsync(new List<string> { to }, subject, body, isHtml, replyTo, companyLogoContentId);
        }

        public async Task SendEmailViaMailKitAsync(List<string> toList, string subject, string body, bool isHtml = true, string? replyTo = null, string? companyLogoContentId = null)
        {
            await SendEmailViaMailKitAdvancedAsync(
                toList: toList,
                subject: subject,
                body: body,
                isHtml: isHtml,
                replyToEmail: replyTo,
                companyLogoContentId: companyLogoContentId
                );
        }
        public async Task SendEmailViaMailKitAdvancedAsync(
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
            string? companyLogoContentId = null
            )
        {
            if (toList == null || !toList.Any(e => !string.IsNullOrWhiteSpace(e)))
                throw new ArgumentException("At least one valid recipient is required.", nameof(toList));

            var smtpHost = _configuration["EmailSettings:SmtpHost"];
            var smtpPort = int.Parse(_configuration["EmailSettings:SmtpPort"] ?? "587");
            var smtpUser = _configuration["EmailSettings:SmtpUser"];
            var smtpPass = _configuration["EmailSettings:SmtpPass"];
            var defaultFromEmail = _configuration["EmailSettings:FromEmail"];
            var defaultFromName = _configuration["EmailSettings:FromName"];

            var fakeEnabled = bool.Parse(_configuration["EmailSettings:FakeEnabled"]);
            var fakeToEmailAddress = _configuration["EmailSettings:Fake_To_EmailAddress"];


            // در صورت وجود در appsettings بررسی می‌شود، در غیر این صورت پیش‌فرض false است
            bool.TryParse(_configuration["EmailSettings:UseSsl"], out var useSsl);

            var message = new MimeMessage();

            // 1. تنظیم فیلد From (ایمیل عمومی یا سفارشی)
            var finalFromEmail = !string.IsNullOrWhiteSpace(customFromEmail) ? customFromEmail : defaultFromEmail;
            var finalFromName = !string.IsNullOrWhiteSpace(customFromName) ? customFromName : defaultFromName;
            message.From.Add(new MailboxAddress(finalFromName, finalFromEmail));

            // 2. تنظیم فیلد Reply-To (ایمیل خصوصی که جواب‌ها به آن برود)
            if (!string.IsNullOrWhiteSpace(replyToEmail))
            {
                var rName = !string.IsNullOrWhiteSpace(replyToName) ? replyToName : replyToEmail;
                message.ReplyTo.Add(new MailboxAddress(rName, replyToEmail));
            }

            // 3. افزودن To
            foreach (var to in toList.Where(e => !string.IsNullOrWhiteSpace(e)))
            {
                if (fakeEnabled)
                    message.To.Add(MailboxAddress.Parse(fakeToEmailAddress.Trim()));
                else
                    message.To.Add(MailboxAddress.Parse(to.Trim()));
            }

            // 4. افزودن CC اختیاری
            if (ccList != null)
            {
                foreach (var cc in ccList.Where(e => !string.IsNullOrWhiteSpace(e)))
                {
                    message.Cc.Add(MailboxAddress.Parse(cc.Trim()));
                }
            }

            // 5. افزودن BCC اختیاری
            if (bccList != null)
            {
                foreach (var bcc in bccList.Where(e => !string.IsNullOrWhiteSpace(e)))
                {
                    message.Bcc.Add(MailboxAddress.Parse(bcc.Trim()));
                }
            }

            message.Subject = subject;

            // 6. ساخت بدنه ایمیل
            var bodyBuilder = new BodyBuilder();
            if (isHtml)
            {
                bodyBuilder.HtmlBody = body;
                if (!string.IsNullOrWhiteSpace(companyLogoContentId))
                {
                    var logoPath = Path.Combine(
                        _env.WebRootPath,
                        "logos",
                        $"logo-{1}.png");

                    if (File.Exists(logoPath))
                    {
                        var logo = bodyBuilder.LinkedResources.Add(logoPath);
                        logo.ContentId = companyLogoContentId;
                        logo.ContentDisposition = new ContentDisposition(ContentDisposition.Inline);
                    }
                }

            }
            else
            {
                bodyBuilder.TextBody = body;
            }
            message.Body = bodyBuilder.ToMessageBody();
            using var client = new MailKitSmtpClient();
            // برای محیط‌های لوکال/تستی اگر سرور SmarterMail گواهی SSL Self-Signed دارد
            // client.ServerCertificateValidationCallback = (s, c, h, e) => true;
            var secureSocketOptions = useSsl
                ? MailKit.Security.SecureSocketOptions.SslOnConnect
                : MailKit.Security.SecureSocketOptions.Auto;

            await client.ConnectAsync(smtpHost, smtpPort, secureSocketOptions, cancellationToken);
            if (!string.IsNullOrWhiteSpace(smtpUser))
            {
                await client.AuthenticateAsync(smtpUser, smtpPass, cancellationToken);
            }
            await client.SendAsync(message, cancellationToken);
            await client.DisconnectAsync(true, cancellationToken);
        }
        #endregion

        public async Task SendEmailWithAttachmentAsync(
            string to,
            string subject,
            string body,
            byte[] attachmentBytes,
            string attachmentFileName,
            CancellationToken ct = default)
        {
            var smtpHost = _configuration["EmailSettings:SmtpHost"];
            var smtpPort = int.Parse(_configuration["EmailSettings:SmtpPort"] ?? "587");
            var smtpUser = _configuration["EmailSettings:SmtpUser"];
            var smtpPass = _configuration["EmailSettings:SmtpPass"];
            var defaultFromEmail = _configuration["EmailSettings:FromEmail"];
            var defaultFromName = _configuration["EmailSettings:FromName"];
            bool.TryParse(_configuration["EmailSettings:UseSsl"], out var useSsl);

            var fakeEnabled = bool.Parse(_configuration["EmailSettings:FakeEnabled"]);
            var fakeToEmailAddress = _configuration["EmailSettings:Fake_To_EmailAddress"];




            var message = new MimeMessage();
            message.From.Add(new MailboxAddress(defaultFromName, defaultFromEmail));

            if (fakeEnabled)
            {
                message.To.Add(MailboxAddress.Parse(fakeToEmailAddress.Trim()));
            }
            else
            {
                message.To.Add(MailboxAddress.Parse(to.Trim()));
            }

            message.Subject = subject;

            var bodyBuilder = new BodyBuilder { HtmlBody = body };
            if (attachmentBytes != null && attachmentBytes.Length > 0)
            {
                bodyBuilder.Attachments.Add(attachmentFileName, attachmentBytes, ContentType.Parse("application/pdf"));
            }

            message.Body = bodyBuilder.ToMessageBody();

            using var client = new MailKitSmtpClient();
            var secureSocketOptions = useSsl
                ? MailKit.Security.SecureSocketOptions.SslOnConnect
                : MailKit.Security.SecureSocketOptions.Auto;

            await client.ConnectAsync(smtpHost, smtpPort, secureSocketOptions, ct);
            if (!string.IsNullOrWhiteSpace(smtpUser))
            {
                await client.AuthenticateAsync(smtpUser, smtpPass, ct);
            }
            await client.SendAsync(message, ct);
            await client.DisconnectAsync(true, ct);
        }




    }
}
