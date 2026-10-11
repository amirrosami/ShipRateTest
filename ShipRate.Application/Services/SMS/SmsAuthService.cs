using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using ShipRate.Application.DTOs;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;

namespace ShipRate.Application.Services.SMS
{
    public interface ISmsAuthService
    {
        Task<string> GetAccessTokenAsync(bool forceRefresh = false, CancellationToken ct = default);
    }

    public class SmsAuthService : ISmsAuthService
    {
        private readonly HttpClient _authClient;
        private readonly SmsGatewayOptions _options;
        private readonly IMemoryCache _cache;
        private readonly ILogger<SmsAuthService> _logger;

        private static readonly SemaphoreSlim Lock = new(1, 1);
        private const string CacheKey = "SMS_AUTH_TOKEN_PAIR";

        public SmsAuthService(
            HttpClient authClient,
            IOptions<SmsGatewayOptions> options,
            IMemoryCache cache,
            ILogger<SmsAuthService> logger)
        {
            _authClient = authClient;
            _options = options.Value;
            _cache = cache;
            _logger = logger;
        }

        public async Task<string> GetAccessTokenAsync(bool forceRefresh = false, CancellationToken ct = default)
        {
            CachedSmsToken? token = null;

            if (!forceRefresh && _cache.TryGetValue<CachedSmsToken>(CacheKey, out token) && !token!.IsExpired)
            {
                return token.AccessToken;
            }

            await Lock.WaitAsync(ct);
            try
            {
                // بررسی مجدد پس از عبور از قفل
                if (!forceRefresh && _cache.TryGetValue<CachedSmsToken>(CacheKey, out token) && !token!.IsExpired)
                {
                    return token.AccessToken;
                }

                // تلاش برای تمدید از طریق Refresh Token
                if (token != null && !string.IsNullOrWhiteSpace(token.RefreshToken))
                {
                    try
                    {
                        return await RefreshTokenAsync(token.RefreshToken, ct);
                    }
                    catch (Exception ex)
                    {
                        _logger.LogWarning(ex, "SMS Refresh token failed. Falling back to full password login.");
                    }
                }

                // لاگین کامل اولیه
                return await LoginAsync(ct);
            }
            finally
            {
                Lock.Release();
            }
        }

        private async Task<string> LoginAsync(CancellationToken ct)
        {
            // ۱. ساخت Query Parameters دقیقا مثل Postman
            var queryParams = new Dictionary<string, string?>
            {
                ["systemName"] = _options.SystemName,
                ["username"] = _options.Username,
                ["password"] = _options.Password,
                ["scope"] = "webservice",
                ["grant_type"] = "password"
            };

            var queryString = BuildQueryString(queryParams);
            // توجه: مسیر بدون اسلش ابتدایی تا با BaseAddress کلاینت تداخل نکند
            var endpoint = $"auth/oauth/token?{queryString}";

            var request = new HttpRequestMessage(HttpMethod.Post, endpoint);
            ApplyHeadersAndAuth(request);

            // ۲. خالی کردن Body تا Content-Length صفر شود
            request.Content = new StringContent(string.Empty, Encoding.UTF8, "application/json");
            request.Content.Headers.ContentLength = 0;

            using var response = await _authClient.SendAsync(request, ct);
            var body = await response.Content.ReadAsStringAsync(ct);

            if (!response.IsSuccessStatusCode)
            {
                _logger.LogError("SMS OAuth login failed. StatusCode={Code}, Body={Body}", response.StatusCode, body);
                throw new InvalidOperationException($"SMS Login failed with code {response.StatusCode}: {body}");
            }

            var result = JsonSerializer.Deserialize<SmsTokenResponse>(body, new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            })!;

            StoreInCache(result);
            return result.AccessToken!;
        }

        private async Task<string> RefreshTokenAsync(string refreshToken, CancellationToken ct)
        {
            var queryParams = new Dictionary<string, string?>
            {
                ["refresh_token"] = refreshToken,
                ["scope"] = "webservice",
                ["grant_type"] = "refresh_token"
            };

            var queryString = BuildQueryString(queryParams);
            var endpoint = $"auth/oauth/token?{queryString}";

            var request = new HttpRequestMessage(HttpMethod.Post, endpoint);
            ApplyHeadersAndAuth(request);

            request.Content = new StringContent(string.Empty, Encoding.UTF8, "application/json");
            request.Content.Headers.ContentLength = 0;

            using var response = await _authClient.SendAsync(request, ct);
            var body = await response.Content.ReadAsStringAsync(ct);

            if (!response.IsSuccessStatusCode)
            {
                _logger.LogError("SMS Refresh token failed. StatusCode={Code}, Body={Body}", response.StatusCode, body);
                throw new InvalidOperationException($"Refresh token failed with status: {response.StatusCode}");
            }

            var result = JsonSerializer.Deserialize<SmsTokenResponse>(body, new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            })!;

            StoreInCache(result);
            return result.AccessToken!;
        }

        private void ApplyHeadersAndAuth(HttpRequestMessage request)
        {
            // ۱. Basic Auth
            request.Headers.Authorization = null;
            var rawCreds = $"{_options.ClientId?.Trim()}:{_options.ClientSecret?.Trim()}";
            var base64Creds = Convert.ToBase64String(Encoding.UTF8.GetBytes(rawCreds));
            request.Headers.Authorization = new AuthenticationHeaderValue("Basic", base64Creds);

            // ۲. هدرهای مشابه مرورگر و Postman برای جلوگیری از بلاک شدن توسط WAF/Nginx
            request.Headers.TryAddWithoutValidation("User-Agent", "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/127.0.0.0 Safari/537.36");
            request.Headers.TryAddWithoutValidation("accept", "*/*");
            request.Headers.TryAddWithoutValidation("Accept-Language", "en,fa;q=0.9,la;q=0.8");
            request.Headers.TryAddWithoutValidation("sec-ch-ua", "\"Not)A;Brand\";v=\"99\", \"Google Chrome\";v=\"127\", \"Chromium\";v=\"127\"");
            request.Headers.TryAddWithoutValidation("sec-ch-ua-mobile", "?0");
            request.Headers.TryAddWithoutValidation("sec-ch-ua-platform", "\"Windows\"");
            request.Headers.TryAddWithoutValidation("Sec-Fetch-Dest", "empty");
            request.Headers.TryAddWithoutValidation("Sec-Fetch-Mode", "cors");
            request.Headers.TryAddWithoutValidation("Sec-Fetch-Site", "cross-site");
        }

        private static string BuildQueryString(Dictionary<string, string?> parameters)
        {
            return string.Join("&", parameters
                .Where(kv => !string.IsNullOrWhiteSpace(kv.Value))
                .Select(kv => $"{Uri.EscapeDataString(kv.Key)}={Uri.EscapeDataString(kv.Value!)}"));
        }

        private void StoreInCache(SmsTokenResponse result)
        {
            var seconds = result.ExpiresIn > 0 ? result.ExpiresIn : 3600;
            // 60 ثانیه بافر قبل از اکسپایر شدن واقعی
            var safeSeconds = Math.Max(seconds - 60, 60);

            var pair = new CachedSmsToken
            {
                AccessToken = result.AccessToken!,
                RefreshToken = result.RefreshToken ?? string.Empty,
                ExpiryUtc = DateTime.UtcNow.AddSeconds(safeSeconds)
            };

            _cache.Set(CacheKey, pair, TimeSpan.FromSeconds(safeSeconds));
        }
    }
}
