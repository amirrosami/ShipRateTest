using System.Net.Http.Headers;

namespace ShipRate.Application.Services.SMS
{
    public class SmsAuthenticationHandler : DelegatingHandler
    {
        private readonly ISmsAuthService _authService;

        public SmsAuthenticationHandler(ISmsAuthService authService)
        {
            _authService = authService;
        }

        protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            var token = await _authService.GetAccessTokenAsync(forceRefresh: false, cancellationToken);
            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);

            var response = await base.SendAsync(request, cancellationToken);

            // اگر توکن به هر دلیلی سمت سرور نامعتبر اعلام شد (401)، توکن تازه دریافت کرده و درخواست را تکرار کن
            if (response.StatusCode == System.Net.HttpStatusCode.Unauthorized)
            {
                response.Dispose();

                token = await _authService.GetAccessTokenAsync(forceRefresh: true, cancellationToken);
                request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);

                return await base.SendAsync(request, cancellationToken);
            }

            return response;
        }
    }
}
