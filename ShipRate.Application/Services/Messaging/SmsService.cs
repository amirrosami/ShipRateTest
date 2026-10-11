using Microsoft.Extensions.Logging;
using ShipRate.Application.Interfaces.Messaging;
using ShipRate.Application.Services.SMS;

namespace ShipRate.Application.Services.Messaging
{
    public class SmsService : ISmsService
    {
        private readonly ILogger<SmsService> _logger;
        private readonly ISmsQueueService _smsQueue;

        public SmsService(ILogger<SmsService> logger, ISmsQueueService smsQueue = null)
        {
            _logger = logger;
            _smsQueue = smsQueue;
        }
        public async Task<bool> SendOtpSmsAsync(string mobileNumber, string code, CancellationToken cancellationToken = default)
        {
            _logger.LogInformation("Sending 2FA OTP {Code} to {Mobile}", code, mobileNumber);




            // TODO: فراخوانی API پنل پیامک (مانند ارسال با پترن)


            await _smsQueue.EnqueueSingleAsync(
        mobile: mobileNumber,
        body: $"کد ورود شما:{code}",
        tag: "OTP"
    );



            await Task.CompletedTask;
            return true;
        }
    }
}



//saeitarabar:saeitarabarWebservice