using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using ShipRate.Application.DTOs;
using ShipRate.Domain.Entities.Messaging;
using ShipRate.Infrastructure.Data;

namespace ShipRate.Application.Services.SMS
{
    public class SmsOutboxProcessor : BackgroundService
    {
        private readonly IServiceScopeFactory _scopeFactory;
        private readonly ILogger<SmsOutboxProcessor> _logger;

        public SmsOutboxProcessor(IServiceScopeFactory scopeFactory, ILogger<SmsOutboxProcessor> logger)
        {
            _scopeFactory = scopeFactory;
            _logger = logger;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                try
                {
                    using var scope = _scopeFactory.CreateScope();
                    var db = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
                    var gateway = scope.ServiceProvider.GetRequiredService<ISmsGatewayService>();

                    // ۱. پردازش صف ارسال پیامک‌ها
                    await ProcessPendingOutboxAsync(db, gateway, stoppingToken);

                    // ۲. استعلام وضعیت تحویل پیام‌های ارسال شده
                    await CheckDeliveryStatusesAsync(db, gateway, stoppingToken);

                    // ۳. دریافت پیام‌های ورودی (MO)
                    await PollIncomingMessagesAsync(db, gateway, stoppingToken);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Unhandled error in SmsOutboxProcessor iteration.");
                }

                await Task.Delay(TimeSpan.FromSeconds(20), stoppingToken);
            }
        }

        private async Task ProcessPendingOutboxAsync(ApplicationDbContext db, ISmsGatewayService gateway, CancellationToken ct)
        {
            var now = DateTime.UtcNow;
            var pendingRecipients = await db.Set<SmsRecipient>()
                .Include(r => r.SmsMessage)
                .Where(r => r.Status == SmsDeliveryStatus.Queued
                         && r.RetryCount < 3
                         && (r.SmsMessage!.ScheduledForUtc == null || r.SmsMessage.ScheduledForUtc <= now))
                .Take(250)
                .ToListAsync(ct);

            if (!pendingRecipients.Any()) return;

            // گروه‌بندی بر اساس فرستنده و متن برای ارسال بهینه
            var groups = pendingRecipients.GroupBy(r => new { r.SmsMessage!.Sender, r.SmsMessage.Body });

            foreach (var group in groups)
            {
                var items = group.Select(r => new SmsSendItemRequest
                {
                    Recipient = r.Mobile,
                    CustomerId = r.CustomerTrackingId
                }).ToList();

                try
                {
                    var results = await gateway.SendBatchWithSrcBodyAsync(group.Key.Sender, group.Key.Body, items, ct);

                    foreach (var res in results)
                    {
                        var recipient = group.FirstOrDefault(r => r.CustomerTrackingId == res.CustomerId);
                        if (recipient == null) continue;

                        if (res.IsSuccess)
                        {
                            recipient.GatewayServerId = res.ServerId;
                            recipient.Status = SmsDeliveryStatus.SentSuccessfully;
                            recipient.SentAtUtc = DateTime.UtcNow;
                        }
                        else
                        {
                            recipient.ErrorCode = res.ErrorCode;
                            recipient.ErrorDescription = SmsErrorCatalog.GetDescription(res.ErrorCode);
                            recipient.Status = SmsErrorCatalog.IsPermanentFailure(res.ErrorCode)
                                ? SmsDeliveryStatus.Failed
                                : SmsDeliveryStatus.Queued;
                            recipient.RetryCount++;
                        }
                    }
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Failed to send batch for group: {Sender}", group.Key.Sender);
                    foreach (var r in group) { r.RetryCount++; }
                }

                await db.SaveChangesAsync(ct);
            }
        }

        private async Task CheckDeliveryStatusesAsync(ApplicationDbContext db, ISmsGatewayService gateway, CancellationToken ct)
        {
            var cutoff = DateTime.UtcNow.AddMinutes(-2);
            var checkList = await db.Set<SmsRecipient>()
                .Where(r => r.Status == SmsDeliveryStatus.SentSuccessfully
                         && r.GatewayServerId != null
                         && (r.LastStatusCheckUtc == null || r.LastStatusCheckUtc < cutoff))
                .Take(250)
                .ToListAsync(ct);

            if (!checkList.Any()) return;

            var serverIds = checkList.Select(r => r.GatewayServerId!).ToList();
            var reports = await gateway.GetStatusesAsync(serverIds, ct);

            foreach (var report in reports)
            {
                var recipient = checkList.FirstOrDefault(r => r.GatewayServerId == report.ServerId);
                if (recipient == null) continue;

                recipient.LastStatusCheckUtc = DateTime.UtcNow;

                if (report.Status == "Send Successfully")
                {
                    recipient.Status = (report.TotalDeliveredParts >= report.TotalParts && report.TotalParts > 0)
                        ? SmsDeliveryStatus.Delivered
                        : SmsDeliveryStatus.SentSuccessfully;
                }
                else if (report.Status == "Failed")
                {
                    recipient.Status = SmsDeliveryStatus.Failed;
                }
                else if (report.Status == "Pending In Queue")
                {
                    recipient.Status = SmsDeliveryStatus.PendingInQueue;
                }
            }

            await db.SaveChangesAsync(ct);
        }

        private async Task PollIncomingMessagesAsync(ApplicationDbContext db, ISmsGatewayService gateway, CancellationToken ct)
        {
            var cursors = await db.Set<SmsMoCursor>().ToListAsync(ct);

            foreach (var cursor in cursors)
            {
                var messages = await gateway.GetIncomingAsync(cursor.LineNumber, cursor.LastProcessedId, ct);
                if (!messages.Any()) continue;

                foreach (var msg in messages)
                {
                    if (!await db.Set<SmsIncomingMessage>().AnyAsync(m => m.Id == msg.Id, ct))
                    {
                        db.Set<SmsIncomingMessage>().Add(new SmsIncomingMessage
                        {
                            Id = msg.Id,
                            LineNumber = msg.To,
                            FromMobile = msg.From,
                            Body = msg.Body,
                            ReceivedAtUtc = DateTime.TryParse(msg.Date, out var dt) ? dt : DateTime.UtcNow,
                            IsProcessed = false
                        });
                    }
                    cursor.LastProcessedId = Math.Max(cursor.LastProcessedId, msg.Id);
                }

                await db.SaveChangesAsync(ct);
            }
        }
    }
}
