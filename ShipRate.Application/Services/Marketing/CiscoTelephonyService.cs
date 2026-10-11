using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using ShipRate.Application.Interfaces;
using ShipRate.Application.Interfaces.Marketing;
using ShipRate.Application.ViewModels;
using ShipRate.Domain.Entities.Marketing;
using ShipRate.Infrastructure.Data;
using System.Text;

namespace ShipRate.Application.Services.Marketing
{

    public class CiscoTelephonyService : ICiscoTelephonyService
    {
        private readonly HttpClient _httpClient;
        private readonly IConfiguration _config;
        private readonly ILogger<CiscoTelephonyService> _logger;

        public CiscoTelephonyService(HttpClient httpClient, IConfiguration config, ILogger<CiscoTelephonyService> logger)
        {
            _httpClient = httpClient;
            _config = config;
            _logger = logger;
        }

        public async Task<bool> DialToCiscoPhoneAsync(string phoneIp, string destinationNumber)
        {
            try
            {
                // استاندارد Cisco IP Phone Execute XML
                string cleanNumber = destinationNumber.Trim().Replace(" ", "").Replace("-", "");
                string xmlPayload = $"XML=<CiscoIPPhoneExecute><ExecuteItem Priority=\"0\" URL=\"Dial:{cleanNumber}\"/></CiscoIPPhoneExecute>";

                var request = new HttpRequestMessage(HttpMethod.Post, $"http://{phoneIp}/CGI/Execute")
                {
                    Content = new StringContent(xmlPayload, Encoding.UTF8, "application/x-www-form-urlencoded")
                };

                // نام کاربری و پسورد پیش‌فرض احراز هویت سرویس سیسکو
                string user = _config["Cisco:AuthUser"] ?? "cisco";
                string pass = _config["Cisco:AuthPassword"] ?? "cisco";
                var authBytes = Encoding.ASCII.GetBytes($"{user}:{pass}");
                request.Headers.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Basic", Convert.ToBase64String(authBytes));

                var response = await _httpClient.SendAsync(request);
                return response.IsSuccessStatusCode;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "خطا در برقراری ارتباط با IP تلفن سیسکو: {PhoneIp}", phoneIp);
                return false;
            }
        }
    }


    public class MarketingCallService : IMarketingCallService
    {
        private readonly ApplicationDbContext _context;
        private readonly ICiscoTelephonyService _ciscoService;
        private readonly ICurrentUserService _currentUserService;
        public MarketingCallService(ApplicationDbContext context, ICiscoTelephonyService ciscoService, ICurrentUserService currentUserService)
        {
            _context = context;
            _ciscoService = ciscoService;
            _currentUserService = currentUserService;
        }
        public async Task<(bool Success, Guid CallLogId, string Message)> StartCallAsync(
            QuickCallInitiateDto dto, string userId, string userExt, string phoneIp)
        {
            // ۱. ثبت رکورد اولیه تماس در وضعیت در انتظار (Pending)
            var log = new MarketingCallLog
            {
                CustomerName = dto.CustomerName,
                PhoneNumber = dto.PhoneNumber,
                Subject = dto.Subject,
                InquiryReference = dto.InquiryReference,
                CallStartTime = DateTime.Now,
                CallerUserId = userId,
                CallerExtension = userExt,
                PhoneIpAddress = phoneIp,
                Outcome = CallOutcome.Pending,
                CreatedAt = DateTime.Now,
                CreateUserId = _currentUserService.UserId
            };
            _context.MarketingCallLogs.Add(log);
            await _context.SaveChangesAsync();

            // ۲. ارسال دستور شماره‌گیری به تلفن سیسکو
            bool dialed = await _ciscoService.DialToCiscoPhoneAsync(phoneIp, dto.PhoneNumber);

            if (!dialed)
            {
                return (false, log.Id, "تلفن سیسکو پاسخ نداد. لطفاً ارتباط شبکه و IP تلفن را بررسی کنید.");
            }

            return (true, log.Id, "تماس با موفقیت روی تلفن سیسکو شماره‌گیری شد.");
        }
        public async Task<bool> SaveCallResultAsync(SaveCallResultDto dto)
        {
            var log = await _context.MarketingCallLogs.FindAsync(dto.CallLogId);
            if (log == null) return false;

            log.DurationSeconds = dto.DurationSeconds;
            log.CallEndTime = log.CallStartTime.AddSeconds(dto.DurationSeconds);
            log.Outcome = dto.Outcome;
            log.Notes = dto.Notes;
            log.FollowUpDate = dto.FollowUpDate;

            await _context.SaveChangesAsync();
            return true;
        }
        public async Task<List<MarketingCallLog>> GetRecentCallsAsync(int count = 10)
        {
            return await _context.MarketingCallLogs
                .OrderByDescending(x => x.CallStartTime)
                .Take(count)
                .ToListAsync();
        }
    }
}