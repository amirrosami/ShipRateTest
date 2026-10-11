using ShipRate.Domain.Entities.Base;

namespace ShipRate.Domain.Entities.Marketing
{
    public class MarketingCallLog : BusinessEntity
    {
        public string? CustomerName { get; set; }        // نام مشتری / شرکت
        public string PhoneNumber { get; set; } = string.Empty; // شماره تلفن/موبایل مقصد

        public string? Subject { get; set; }            // موضوع تماس (مثلاً استعلام نرخ کانتینری / دموراژ)
        public string? InquiryReference { get; set; }   // شماره استعلام یا پرونده (Quote/Booking No)

        public DateTime CallStartTime { get; set; } = DateTime.Now;
        public DateTime? CallEndTime { get; set; }
        public int DurationSeconds { get; set; }        // مدت زمان مکالمه به ثانیه

        public CallOutcome Outcome { get; set; } = CallOutcome.Pending; // وضعیت/نتیجه تماس
        public string? Notes { get; set; }              // خلاصه مذاکره و توضیحات
        public DateTime? FollowUpDate { get; set; }     // تاریخ پیگیری بعدی در صورت نیاز

        // اطلاعات کارشناس و تلفن سیسکو
        public string CallerUserId { get; set; } = string.Empty;
        public string CallerExtension { get; set; } = string.Empty;
        public string PhoneIpAddress { get; set; } = string.Empty;
    }
    public enum CallOutcome
    {
        Pending = 0,         // در انتظار ثبت نتیجه
        Successful = 1,      // مذاکره موفق / ارسال پیش‌فاکتور
        NeedsFollowUp = 2,   // نیاز به پیگیری مجدد
        NoAnswer = 3,        // عدم پاسخگویی
        Busy = 4,            // اشغال بود
        WrongNumber = 5,     // شماره اشتباه
        Rejected = 6         // عدم تمایل / رد پیشنهاد
    }
}
