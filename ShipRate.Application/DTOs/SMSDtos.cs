using ShipRate.Domain.Entities.Messaging;
using System.Text.Json.Serialization;

namespace ShipRate.Application.DTOs
{
    #region Authentication
    public class SmsTokenResponse
    {
        [JsonPropertyName("access_token")] public string? AccessToken { get; set; }
        [JsonPropertyName("refresh_token")] public string? RefreshToken { get; set; }
        [JsonPropertyName("expires_in")] public int ExpiresIn { get; set; }
        [JsonPropertyName("token_type")] public string? TokenType { get; set; }
    }

    public class CachedSmsToken
    {
        public string AccessToken { get; set; } = string.Empty;
        public string RefreshToken { get; set; } = string.Empty;
        public DateTime ExpiryUtc { get; set; }
        public bool IsExpired => DateTime.UtcNow >= ExpiryUtc.AddMinutes(-5); // 5 دقیقه بافر امنیتی
    }
    #endregion

    #region Sending
    public class SmsSendItemRequest
    {
        [JsonPropertyName("recipient")] public string Recipient { get; set; } = string.Empty;
        [JsonPropertyName("customerId")] public string? CustomerId { get; set; }
        [JsonPropertyName("sendDate")] public string? SendDate { get; set; }
    }

    public class SmsSendMultipleWithSrcBodyRequest
    {
        [JsonPropertyName("sender")] public string Sender { get; set; } = string.Empty;
        [JsonPropertyName("body")] public string Body { get; set; } = string.Empty;
        [JsonPropertyName("sendDate")] public string? SendDate { get; set; }
        [JsonPropertyName("smsItems")] public List<SmsSendItemRequest> SmsItems { get; set; } = new();
    }

    public class SmsSendApiResponse
    {
        [JsonPropertyName("customerId")] public string? CustomerId { get; set; }
        [JsonPropertyName("mobile")] public string? Mobile { get; set; }
        [JsonPropertyName("serverId")] public string? ServerId { get; set; }
        [JsonPropertyName("errorCode")] public string? ErrorCode { get; set; }
        [JsonPropertyName("description")] public string? Description { get; set; }

        [JsonIgnore] public bool IsSuccess => !string.IsNullOrWhiteSpace(ServerId);
    }
    #endregion

    #region Status & MO
    public class SmsStatusApiResponse
    {
        [JsonPropertyName("customerId")] public string? CustomerId { get; set; }
        [JsonPropertyName("serverId")] public string? ServerId { get; set; }
        [JsonPropertyName("totalParts")] public long TotalParts { get; set; }
        [JsonPropertyName("totalDeliveredParts")] public long TotalDeliveredParts { get; set; }
        [JsonPropertyName("status")] public string? Status { get; set; }
    }

    public class SmsMoApiResponse
    {
        [JsonPropertyName("id")] public long Id { get; set; }
        [JsonPropertyName("body")] public string Body { get; set; } = string.Empty;
        [JsonPropertyName("from")] public string From { get; set; } = string.Empty;
        [JsonPropertyName("to")] public string To { get; set; } = string.Empty;
        [JsonPropertyName("date")] public string? Date { get; set; }
    }
    #endregion}


    public static class SmsErrorCatalog
    {
        private static readonly Dictionary<string, string> Errors = new(StringComparer.OrdinalIgnoreCase)
        {
            ["E1"] = "نام کاربری و یا کلمه عبور وارد نشده است",
            ["E2"] = "ورود تمامی پارامترها الزامیست",
            ["E3"] = "پارامتر ورودی باید از نوع رشته باشد",
            ["E4"] = "دسترسی غیر مجاز به سیستم",
            ["E5"] = "نام کاربری و یا کلمه عبور صحیح نیست",
            ["E6"] = "اعتبار برای ارسال کافی نیست",
            ["E7"] = "پیام ورودی جدید خوانده نشده‌ای وجود ندارد",
            ["E8"] = "شناسه وارد شده جهت دریافت وضعیت نامعتبر است",
            ["E43"] = "شماره گیرنده نامعتبر است",
            ["E65"] = "مقداردهی شناسه پیام الزامیست",
            ["E66"] = "مقداردهی شماره اختصاصی مورد نظر الزامیست",
            ["E107"] = "تعداد گیرندگان بیشتر از ۲۵۰ گیرنده است",
            ["E132"] = "متن وارد شده شامل کلمات حساس است",
            ["E211"] = "طول متن پیام بیش از حد مجاز است",
            ["E223"] = "شماره اختصاصی وارد شده نامعتبر است",
            ["E303"] = "هیچ شماره گیرنده صحیحی وارد نشده",
            ["E305"] = "شناسه وارد شده جهت دریافت وضعیت وجود ندارد",
            ["E766"] = "متن پیام خالیست",
            ["E800"] = "شماره در لیست سیاه است",
            ["E998"] = "پکیج منقضی شده است",
            ["E999"] = "شماره منقضی شده است",
            ["E1000"] = "خطای کلی رخ داده است",
            ["E1001"] = "زمان ارسال باید بعد از زمان حال باشد",
            ["E1002"] = "فرمت زمان ارسال صحیح نیست"
        };

        public static string GetDescription(string? errorCode)
        {
            if (string.IsNullOrWhiteSpace(errorCode)) return "خطای نامشخص";
            return Errors.TryGetValue(errorCode, out var desc) ? desc : $"خطای پنل با کد ({errorCode})";
        }

        public static bool IsPermanentFailure(string? errorCode) => errorCode switch
        {
            "E6" or "E43" or "E132" or "E211" or "E223" or "E303" or "E766" or "E800" or "E998" or "E999" => true,
            _ => false
        };








    }




    public class SmsDashboardViewModel
    {
        public SmsStatsDto Stats { get; set; } = new();
    }

    public class SmsStatsDto
    {
        public int QueuedCount { get; set; }
        public int SentTodayCount { get; set; }
        public int FailedCount { get; set; }
        public int UnreadIncomingCount { get; set; }
    }
    public class SmsOutboxFilterDto
    {
        public string? SearchTerm { get; set; }
        public SmsDeliveryStatus? Status { get; set; }
        public string? Tag { get; set; }
        public int Page { get; set; } = 1;
        public int PageSize { get; set; } = 15;
    }

    public class SmsOutboxItemDto
    {
        public Guid RecipientId { get; set; }
        public string Mobile { get; set; } = string.Empty;
        public string MessageBody { get; set; } = string.Empty;
        public string? Tag { get; set; }
        public string CustomerTrackingId { get; set; } = string.Empty;
        public string? GatewayServerId { get; set; }
        public SmsDeliveryStatus Status { get; set; }
        public string? ErrorDescription { get; set; }
        public DateTime CreatedAtUtc { get; set; }
        public DateTime? SentAtUtc { get; set; }
        public int RetryCount { get; set; }
    }

    public class SmsIncomingFilterDto
    {
        public string? SearchText { get; set; }
        public int Page { get; set; } = 1;
        public int PageSize { get; set; } = 15;
    }

    public class SmsIncomingItemDto
    {
        public long Id { get; set; }
        public string LineNumber { get; set; } = string.Empty;
        public string FromMobile { get; set; } = string.Empty;
        public string MessageBody { get; set; } = string.Empty;
        public DateTime ReceivedAtUtc { get; set; }
        public bool IsProcessed { get; set; }
    }

    public class SendManualSmsDto
    {
        public string MobileNumber { get; set; } = string.Empty;
        public string MessageBody { get; set; } = string.Empty;
        public string? Tag { get; set; } = "Manual";
        public string? TrackingId { get; set; }
    }
}
