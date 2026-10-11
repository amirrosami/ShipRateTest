using Microsoft.Extensions.Options;
using ShipRate.Domain.Entities.Messaging;
using ShipRate.Infrastructure.Data;

namespace ShipRate.Application.Services.SMS
{
    public interface ISmsQueueService
    {
        Task<Guid> EnqueueSingleAsync(string mobile, string body, string? tag = null, DateTime? scheduleForUtc = null, CancellationToken ct = default);
        Task<Guid> EnqueueGroupAsync(IEnumerable<string> mobiles, string body, string? tag = null, DateTime? scheduleForUtc = null, CancellationToken ct = default);
    }

    public class SmsQueueService : ISmsQueueService
    {
        private readonly ApplicationDbContext _db;
        private readonly SmsGatewayOptions _options;

        public SmsQueueService(ApplicationDbContext db, IOptions<SmsGatewayOptions> options)
        {
            _db = db;
            _options = options.Value;
        }

        public Task<Guid> EnqueueSingleAsync(string mobile, string body, string? tag = null, DateTime? scheduleForUtc = null, CancellationToken ct = default)
        {
            return EnqueueGroupAsync(new[] { mobile }, body, tag, scheduleForUtc, ct);
        }

        public async Task<Guid> EnqueueGroupAsync(IEnumerable<string> mobiles, string body, string? tag = null, DateTime? scheduleForUtc = null, CancellationToken ct = default)
        {
            var id = Guid.CreateVersion7();
            var message = new SmsMessage
            {
                Id = id,
                CreatedAt = DateTime.UtcNow,
                IsDeleted = false,
                Sender = _options.DefaultSender,
                Body = body,
                Tag = tag,
                ScheduledForUtc = scheduleForUtc,
                Recipients = mobiles.Distinct().Select(m => new SmsRecipient
                {
                    Mobile = m.Trim(),
                    SmsMessageId = id,
                }).ToList()
            };

            _db.Set<SmsMessage>().Add(message);
            await _db.SaveChangesAsync(ct);

            return message.Id;
        }
    }
}
