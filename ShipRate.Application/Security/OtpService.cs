using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.EntityFrameworkCore;
using ShipRate.Domain.Entities.Security;
using ShipRate.Infrastructure.Data;
using System.Security.Cryptography;
using System.Text;

namespace ShipRate.Application.Security
{
    public class OtpService : IOtpService
    {

        private readonly UserManager<ApplicationUser> _userManager;
        private readonly ApplicationDbContext _db;
        private const int MaxAllowedAttempts = 5;

        public OtpService(ApplicationDbContext db, UserManager<ApplicationUser> userManager)
        {
            _db = db;
            _userManager = userManager;
        }

        public async Task<string> GenerateAndSaveOtpAsync(
            Guid userId,
            TwoFactorMethodEnum method,
            int expiryMinutes = 2,
            CancellationToken cancellationToken = default)
        {
            // 1. تولید کد 5 رقمی تصادفی امن (از 10000 تا 99999)
            var rawCode = RandomNumberGenerator.GetInt32(10000, 100000).ToString();
            var codeHash = HashCode(rawCode);
            // 2. ابطال/حذف نرم کدهای قبلی مصرف‌نشده این کاربر برای این روش
            var previousActiveCodes = await _db.UserOtpCodes
                .Where(x => x.UserId == userId && x.Method == method && !x.IsUsed && !x.IsDeleted)
                .ToListAsync(cancellationToken);

            foreach (var oldCode in previousActiveCodes)
            {
                oldCode.IsDeleted = true;
            }

            // 3. ثبت کد جدید
            var now = DateTime.Now;
            var newOtp = new UserOtpCode
            {
                Id = Guid.NewGuid(), // Guid v7
                UserId = userId,
                CodeHash = codeHash,
                Method = method,
                CreatedAt = now,
                ExpiresAt = now.AddMinutes(expiryMinutes),
                IsUsed = false,
                AttemptCount = 0,
                IsDeleted = false
            };

            _db.UserOtpCodes.Add(newOtp);
            await _db.SaveChangesAsync(cancellationToken);

            return rawCode; // کد خام فقط برای ارسال پیامک برگشت داده می‌شود
        }

        public async Task<OtpVerifyResult> VerifyOtpAsync(
            Guid userId,
            string rawCode,
            TwoFactorMethodEnum method,
            CancellationToken cancellationToken = default)
        {
            var now = DateTime.Now;

            var otpRecord = await _db.UserOtpCodes
                .Where(x => x.UserId == userId && x.Method == method && !x.IsUsed && !x.IsDeleted)
                .OrderByDescending(x => x.CreatedAt)
                .FirstOrDefaultAsync(cancellationToken);

            if (otpRecord == null)
                return new OtpVerifyResult(false, "Security code was not found or has expired.");

            // Check expiration
            if (now > otpRecord.ExpiresAt)
            {
                otpRecord.IsDeleted = true;
                await _db.SaveChangesAsync(cancellationToken);
                return new OtpVerifyResult(false, "Security code has expired. Please request a new code.");
            }

            // Check attempt limit (Brute-force protection)
            if (otpRecord.AttemptCount >= MaxAllowedAttempts)
            {
                otpRecord.IsDeleted = true;
                await _db.SaveChangesAsync(cancellationToken);
                return new OtpVerifyResult(false, "Too many attempts. The code has been invalidated.", IsLockedOut: true);
            }

            otpRecord.AttemptCount++;
            otpRecord.LastAttemptAt = now;

            var inputHash = HashCode(rawCode.Trim());
            if (otpRecord.CodeHash != inputHash)
            {
                await _db.SaveChangesAsync(cancellationToken);
                var remainingAttempts = MaxAllowedAttempts - otpRecord.AttemptCount;
                return new OtpVerifyResult(false, $"Invalid code entered. ({remainingAttempts} attempts remaining)");
            }

            // موفقیت‌آمیز: علامت‌گذاری به عنوان استفاده‌شده
            otpRecord.IsUsed = true;
            otpRecord.UsedAt = now;
            await _db.SaveChangesAsync(cancellationToken);

            return new OtpVerifyResult(true);
        }

        public async Task<(bool CanResend, int RemainingSeconds)> CanResendOtpAsync(
            Guid userId,
            TwoFactorMethodEnum method,
            int coolDownSeconds = 120,
            CancellationToken cancellationToken = default)
        {
            var lastOtp = await _db.UserOtpCodes
                .Where(x => x.UserId == userId && x.Method == method && !x.IsDeleted)
                .OrderByDescending(x => x.CreatedAt)
                .Select(x => x.CreatedAt)
                .FirstOrDefaultAsync(cancellationToken);

            if (lastOtp == default)
                return (true, 0);

            var elapsedSeconds = (DateTime.Now - lastOtp).TotalSeconds;
            if (elapsedSeconds < coolDownSeconds)
            {
                return (false, (int)Math.Ceiling(coolDownSeconds - elapsedSeconds));
            }

            return (true, 0);
        }
        private static string HashCode(string code)
        {
            // استفاده از SHA256 با Salt سیستمی
            const string salt = "ShipRate_2FA_Salt_#2026";
            var bytes = SHA256.HashData(Encoding.UTF8.GetBytes(code + salt));
            return Convert.ToHexString(bytes);
        }

        public async Task<GoogleAuthenticatorSetupResult>
            GetGoogleAuthenticatorSetupAsync(
                Guid userId,
                string? accountName = null,
                CancellationToken cancellationToken = default)
        {
            var user = await FindUserAsync(userId, cancellationToken);

            if (user is null)
            {
                throw new InvalidOperationException(
                    "User was not found or is inactive.");
            }

            var sharedKey = await _userManager.GetAuthenticatorKeyAsync(user);

            if (string.IsNullOrWhiteSpace(sharedKey))
            {
                var resetResult =
                    await _userManager.ResetAuthenticatorKeyAsync(user);

                if (!resetResult.Succeeded)
                {
                    var errors = string.Join(
                        "; ",
                        resetResult.Errors.Select(x => x.Description));

                    throw new InvalidOperationException(
                        $"Unable to create authenticator key. {errors}");
                }

                sharedKey = await _userManager.GetAuthenticatorKeyAsync(user);
            }

            if (string.IsNullOrWhiteSpace(sharedKey))
            {
                throw new InvalidOperationException(
                    "Authenticator key could not be generated.");
            }

            accountName = string.IsNullOrWhiteSpace(accountName)
                ? user.Email ?? user.UserName ?? userId.ToString()
                : accountName.Trim();

            const string issuer = "ShipRate";

            var label = $"{issuer}:{accountName}";

            var query = QueryHelpers.AddQueryString(
                string.Empty,
                new Dictionary<string, string?>
                {
                    ["secret"] = sharedKey,
                    ["issuer"] = issuer
                });

            var authenticatorUri =
                $"otpauth://totp/{Uri.EscapeDataString(label)}{query}";

            return new GoogleAuthenticatorSetupResult(
                SharedKey: sharedKey,
                AuthenticatorUri: authenticatorUri);
        }


        public async Task<OtpVerifyResult>
            VerifyGoogleAuthenticatorCodeAsync(
                Guid userId,
                string rawCode,
                CancellationToken cancellationToken = default)
        {
            if (string.IsNullOrWhiteSpace(rawCode))
            {
                return new OtpVerifyResult(
                    IsSuccess: false,
                    ErrorMessage: "Authenticator code is required.");
            }

            var code = rawCode.Trim();

            if (code.Length != 6 || !code.All(char.IsDigit))
            {
                return new OtpVerifyResult(
                    IsSuccess: false,
                    ErrorMessage: "Authenticator code must be 6 digits.");
            }

            var user = await FindUserAsync(userId, cancellationToken);

            if (user is null)
            {
                return new OtpVerifyResult(
                    IsSuccess: false,
                    ErrorMessage: "User was not found or is inactive.");
            }

            var provider =
                _userManager.Options.Tokens.AuthenticatorTokenProvider;

            var isValid =
                await _userManager.VerifyTwoFactorTokenAsync(
                    user,
                    provider,
                    code);

            if (!isValid)
            {
                return new OtpVerifyResult(
                    IsSuccess: false,
                    ErrorMessage: "Invalid authenticator code.");
            }

            return new OtpVerifyResult(IsSuccess: true);
        }




        private async Task<ApplicationUser?> FindUserAsync(
            Guid userId,
            CancellationToken cancellationToken)
        {
            return await _userManager.Users
                .FirstOrDefaultAsync(
                    x => x.Id == userId &&
                         !x.IsDeleted &&
                         x.IsActive,
                    cancellationToken);
        }


    }
}
