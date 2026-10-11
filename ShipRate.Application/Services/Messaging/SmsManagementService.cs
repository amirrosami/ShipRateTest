using Microsoft.EntityFrameworkCore;
using ShipRate.Application.DTOs;
using ShipRate.Application.Interfaces.Messaging;
using ShipRate.Application.Services.SMS;
using ShipRate.Domain.Entities.Messaging;
using ShipRate.Infrastructure.Data;

namespace ShipRate.Application.Services.Messaging
{
    public class SmsManagementService : ISmsManagementService
    {
        private readonly ApplicationDbContext _context;
        private readonly ISmsQueueService _smsQueue;

        public SmsManagementService(ApplicationDbContext context, ISmsQueueService smsQueue)
        {
            _context = context;
            _smsQueue = smsQueue;
        }

        public async Task<SmsStatsDto> GetSmsStatsAsync()
        {
            var today = DateTime.UtcNow.Date;

            return new SmsStatsDto
            {
                QueuedCount = await _context.Set<SmsRecipient>()
                    .CountAsync(r => r.Status == SmsDeliveryStatus.Queued ||
                                     r.Status == SmsDeliveryStatus.PendingInQueue),
                SentTodayCount = await _context.Set<SmsRecipient>()
                    .CountAsync(r => r.SentAtUtc >= today),
                FailedCount = await _context.Set<SmsRecipient>()
                    .CountAsync(r => r.Status == SmsDeliveryStatus.Failed),
                UnreadIncomingCount = await _context.Set<SmsIncomingMessage>()
                    .CountAsync(i => !i.IsProcessed)
            };
        }

        public async Task<List<SmsOutboxItemDto>> GetPagedOutboxAsync(SmsOutboxFilterDto filter)
        {
            filter ??= new SmsOutboxFilterDto();

            var query = _context.Set<SmsRecipient>()
                .Include(r => r.SmsMessage)
                .AsQueryable();

            if (!string.IsNullOrWhiteSpace(filter.SearchTerm))
            {
                query = query.Where(r => r.Mobile.Contains(filter.SearchTerm) ||
                                         r.CustomerTrackingId.Contains(filter.SearchTerm) ||
                                         r.SmsMessage!.Body.Contains(filter.SearchTerm));
            }

            if (filter.Status.HasValue)
            {
                query = query.Where(r => r.Status == filter.Status.Value);
            }

            if (!string.IsNullOrWhiteSpace(filter.Tag))
            {
                query = query.Where(r => r.SmsMessage!.Tag == filter.Tag);
            }

            return await query
                .OrderByDescending(r => r.SmsMessage!.CreatedAtUtc)
                .Skip((filter.Page - 1) * filter.PageSize)
                .Take(filter.PageSize)
                .Select(r => new SmsOutboxItemDto
                {
                    RecipientId = r.Id,
                    Mobile = r.Mobile,
                    MessageBody = r.SmsMessage!.Body,
                    Tag = r.SmsMessage.Tag,
                    CustomerTrackingId = r.CustomerTrackingId,
                    GatewayServerId = r.GatewayServerId,
                    Status = r.Status,
                    ErrorDescription = r.ErrorDescription,
                    CreatedAtUtc = r.SmsMessage.CreatedAtUtc,
                    SentAtUtc = r.SentAtUtc,
                    RetryCount = r.RetryCount
                })
                .ToListAsync();
        }

        public async Task<List<SmsIncomingItemDto>> GetPagedIncomingAsync(SmsIncomingFilterDto filter)
        {
            filter ??= new SmsIncomingFilterDto();

            var query = _context.Set<SmsIncomingMessage>().AsQueryable();

            if (!string.IsNullOrWhiteSpace(filter.SearchText))
            {
                query = query.Where(i => i.FromMobile.Contains(filter.SearchText) ||
                                         i.Body.Contains(filter.SearchText));
            }

            return await query
                .OrderByDescending(i => i.ReceivedAtUtc)
                .Skip((filter.Page - 1) * filter.PageSize)
                .Take(filter.PageSize)
                .Select(i => new SmsIncomingItemDto
                {
                    Id = i.Id,
                    LineNumber = i.LineNumber,
                    FromMobile = i.FromMobile,
                    MessageBody = i.Body,
                    ReceivedAtUtc = i.ReceivedAtUtc,
                    IsProcessed = i.IsProcessed
                })
                .ToListAsync();
        }

        public async Task<(bool Success, string Message)> SendManualSmsAsync(SendManualSmsDto dto)
        {
            if (string.IsNullOrWhiteSpace(dto.MobileNumber) || string.IsNullOrWhiteSpace(dto.MessageBody))
            {
                return (false, "Mobile number and message body are required.");
            }
            await _smsQueue.EnqueueSingleAsync(dto.MobileNumber, dto.MessageBody, dto.Tag);
            //var id = Guid.CreateVersion7();
            //var message = new SmsMessage
            //{
            //    Id = id,
            //    Sender = "SystemPanel",
            //    Body = dto.MessageBody,
            //    Tag = string.IsNullOrWhiteSpace(dto.Tag) ? "Manual" : dto.Tag,
            //    CreatedAtUtc = DateTime.UtcNow,
            //    Recipients = new List<SmsRecipient>
            //    {
            //        new SmsRecipient
            //        {
            //            Id = Guid.CreateVersion7(),
            //            SmsMessageId=id,
            //            Mobile = dto.MobileNumber,
            //            CustomerTrackingId = string.IsNullOrWhiteSpace(dto.TrackingId)
            //                ? Guid.NewGuid().ToString("N")
            //                : dto.TrackingId,
            //            Status = SmsDeliveryStatus.Queued,
            //            RetryCount = 0
            //        }
            //    }
            //};

            //_context.Set<SmsMessage>().Add(message);
            //await _context.SaveChangesAsync();






            return (true, "SMS successfully queued in database.");
        }

        public async Task<(bool Success, string Message)> RetryFailedSmsAsync(Guid recipientId)
        {
            var recipient = await _context.Set<SmsRecipient>().FindAsync(recipientId);
            if (recipient == null)
            {
                return (false, "SMS recipient record not found.");
            }

            if (recipient.Status != SmsDeliveryStatus.Failed)
            {
                return (false, "Only failed messages can be retried.");
            }

            recipient.Status = SmsDeliveryStatus.Queued;
            recipient.ErrorDescription = null;
            recipient.ErrorCode = null;
            recipient.RetryCount += 1;

            await _context.SaveChangesAsync();
            return (true, "SMS reset to queue for retry.");
        }
    }
}
