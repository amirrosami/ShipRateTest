namespace ShipRate.Application.Interfaces.Messaging
{
    public interface ISmsService
    {
        Task<bool> SendOtpSmsAsync(string mobileNumber, string code, CancellationToken cancellationToken = default);
    }
}
