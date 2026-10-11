using ShipRate.Domain.Entities.Security;

namespace ShipRate.Application.Security
{
    public interface IOtpService
    {
        // SMS OTP
        Task<string> GenerateAndSaveOtpAsync(Guid userId, TwoFactorMethodEnum method, int expiryMinutes = 2, CancellationToken cancellationToken = default);
        Task<OtpVerifyResult> VerifyOtpAsync(Guid userId, string rawCode, TwoFactorMethodEnum method, CancellationToken cancellationToken = default);
        Task<(bool CanResend, int RemainingSeconds)> CanResendOtpAsync(Guid userId, TwoFactorMethodEnum method, int coolDownSeconds = 120, CancellationToken cancellationToken = default);

        // Google Authenticator
        Task<GoogleAuthenticatorSetupResult> GetGoogleAuthenticatorSetupAsync(Guid userId, string? accountName = null, CancellationToken cancellationToken = default);
        Task<OtpVerifyResult> VerifyGoogleAuthenticatorCodeAsync(Guid userId, string rawCode, CancellationToken cancellationToken = default);
    }
    public record OtpVerifyResult(bool IsSuccess, string? ErrorMessage = null, bool IsLockedOut = false);
    public sealed record GoogleAuthenticatorSetupResult(string SharedKey, string AuthenticatorUri);
}
