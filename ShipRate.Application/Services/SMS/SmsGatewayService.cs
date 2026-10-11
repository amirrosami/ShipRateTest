using Microsoft.Extensions.Logging;
using ShipRate.Application.DTOs;
using System.Text;
using System.Text.Json;

namespace ShipRate.Application.Services.SMS
{
    public interface ISmsGatewayService
    {
        Task<List<SmsSendApiResponse>> SendBatchWithSrcBodyAsync(string sender, string body, List<SmsSendItemRequest> items, CancellationToken ct = default);
        Task<List<SmsStatusApiResponse>> GetStatusesAsync(List<string> serverIds, CancellationToken ct = default);
        Task<List<SmsMoApiResponse>> GetIncomingAsync(string lineNumber, long? lastId = null, CancellationToken ct = default);
    }

    public class SmsGatewayService : ISmsGatewayService
    {
        private readonly HttpClient _httpClient;
        private readonly ILogger<SmsGatewayService> _logger;
        private const int MaxBatchSize = 250; // محدودیت درگاه

        public SmsGatewayService(HttpClient httpClient, ILogger<SmsGatewayService> logger)
        {
            _httpClient = httpClient;
            _logger = logger;
        }

        public async Task<List<SmsSendApiResponse>> SendBatchWithSrcBodyAsync(
            string sender, string body, List<SmsSendItemRequest> items, CancellationToken ct = default)
        {
            var allResults = new List<SmsSendApiResponse>();

            foreach (var chunk in items.Chunk(MaxBatchSize))
            {
                var payload = new SmsSendMultipleWithSrcBodyRequest
                {
                    Sender = sender,
                    Body = body,
                    SmsItems = chunk.ToList()
                };

                var json = JsonSerializer.Serialize(payload);
                using var content = new StringContent(json, Encoding.UTF8, "application/json");

                using var response = await _httpClient.PostAsync("/panel/webservice/sendMultipleWithSrcBody", content, ct);
                var resBody = await response.Content.ReadAsStringAsync(ct);

                if (!response.IsSuccessStatusCode)
                {
                    _logger.LogError("SMS Send failed. HttpCode={Code}, Response={Res}", response.StatusCode, resBody);
                    throw new InvalidOperationException($"SMS Gateway Error: HTTP {response.StatusCode}");
                }

                var results = JsonSerializer.Deserialize<List<SmsSendApiResponse>>(resBody,
                    new JsonSerializerOptions { PropertyNameCaseInsensitive = true }) ?? new();

                allResults.AddRange(results);
            }

            return allResults;
        }

        public async Task<List<SmsStatusApiResponse>> GetStatusesAsync(List<string> serverIds, CancellationToken ct = default)
        {
            var allReports = new List<SmsStatusApiResponse>();

            foreach (var chunk in serverIds.Chunk(MaxBatchSize))
            {
                var query = $"byCustomer=false&ids={string.Join(",", chunk)}";
                using var response = await _httpClient.GetAsync($"/report/webservice/status?{query}", ct);
                var resBody = await response.Content.ReadAsStringAsync(ct);

                if (!response.IsSuccessStatusCode)
                {
                    _logger.LogError("SMS Status report failed. Status={Code}, Response={Res}", response.StatusCode, resBody);
                    continue;
                }

                var reports = JsonSerializer.Deserialize<List<SmsStatusApiResponse>>(resBody,
                    new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

                if (reports != null) allReports.AddRange(reports);
            }

            return allReports;
        }

        public async Task<List<SmsMoApiResponse>> GetIncomingAsync(string lineNumber, long? lastId = null, CancellationToken ct = default)
        {
            var query = $"number={Uri.EscapeDataString(lineNumber)}";
            if (lastId.HasValue && lastId.Value > 0)
            {
                query += $"&id={lastId.Value}";
            }

            using var response = await _httpClient.GetAsync($"/report/webservice/mo?{query}", ct);
            var resBody = await response.Content.ReadAsStringAsync(ct);

            if (!response.IsSuccessStatusCode)
            {
                _logger.LogError("SMS MO fetch failed. Status={Code}, Response={Res}", response.StatusCode, resBody);
                return new List<SmsMoApiResponse>();
            }

            return JsonSerializer.Deserialize<List<SmsMoApiResponse>>(resBody,
                new JsonSerializerOptions { PropertyNameCaseInsensitive = true }) ?? new();
        }
    }
}
