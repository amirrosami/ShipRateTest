using Microsoft.Extensions.Logging;
using ShipRate.Application.DTOs.ApiIr;
using ShipRate.Application.Interfaces.ExternalServices;
using ShipRate.Application.Interfaces.Logs;
using System.Diagnostics;
using System.Net.Http.Json;
using System.Text.Json;

namespace ShipRate.Application.Services.External
{
    public sealed class ApiIrService : IApiIrService
    {
        private readonly HttpClient _httpClient;
        private readonly IApiInquiryLogService _apiInquiryLogService;
        private readonly ILogger<ApiIrService> _logger;

        private static readonly JsonSerializerOptions JsonOptions = new()
        {
            PropertyNameCaseInsensitive = true
        };

        public ApiIrService(
            HttpClient httpClient,
            IApiInquiryLogService apiInquiryLogService,
            ILogger<ApiIrService> logger)
        {
            _httpClient = httpClient;
            _apiInquiryLogService = apiInquiryLogService;
            _logger = logger;
        }

        public Task<PersonInfoResponse> PersonInfoAsync(
            PersonInfoRequest request,
            CancellationToken ct = default)
        {
            return ExecuteInquiryAsync<PersonInfoRequest, PersonInfoResponse>(
                serviceName: "PersonInfo",
                endpoint: "api/sw1/PersonInfo",
                request: request,
                getApiSuccess: x => x.Success,
                getApiCode: x => x.Code,
                getApiMessage: x => x.Message,
                hasData: x => x.Data is not null,
                ct: ct);
        }

        public Task<CompanyInfoResponse> CompanyInfoAsync(
            CompanyInfoRequest request,
            CancellationToken ct = default)
        {
            return ExecuteInquiryAsync<CompanyInfoRequest, CompanyInfoResponse>(
                serviceName: "CompanyInfo",
                endpoint: "api/sw1/CompanyInfo",
                request: request,
                getApiSuccess: x => x.Success,
                getApiCode: x => x.Code,
                getApiMessage: x => x.Message,
                hasData: x => x.Data is not null,
                ct: ct);
        }

        public Task<ChequeInfoResponse> ChequeInfoAsync(
            ChequeInfoRequest request,
            CancellationToken ct = default)
        {
            return ExecuteInquiryAsync<ChequeInfoRequest, ChequeInfoResponse>(
                serviceName: "UnpaidChequePro",
                endpoint: "api/sw1/UnpaidChequePro",
                request: request,
                getApiSuccess: x => x.Success,
                getApiCode: x => x.Code,
                getApiMessage: x => x.Message,
                hasData: x => x.Data is not null,
                ct: ct);
        }

        private async Task<TResponse> ExecuteInquiryAsync<TRequest, TResponse>(
            string serviceName,
            string endpoint,
            TRequest request,
            Func<TResponse, bool> getApiSuccess,
            Func<TResponse, int> getApiCode,
            Func<TResponse, string?> getApiMessage,
            Func<TResponse, bool> hasData,
            CancellationToken ct)
            where TRequest : class
            where TResponse : class
        {
            /*
             * ابتدا لاگ Pending ثبت می‌شود.
             * اگر این عملیات ناموفق باشد، API.ir فراخوانی نمی‌شود؛
             * بنابراین هیچ درخواست هزینه‌بری بدون رکورد Audit نخواهیم داشت.
             */
            var log = await _apiInquiryLogService.StartAsync(
                serviceName: serviceName,
                endpoint: endpoint,
                requestPayload: request,
                ct: ct);

            var stopwatch = Stopwatch.StartNew();
            var logCompleted = false;
            int? httpStatusCode = null;
            string? responseJson = null;

            try
            {
                using var response = await _httpClient.PostAsJsonAsync(
                    endpoint,
                    request,
                    ct);

                httpStatusCode = (int)response.StatusCode;

                responseJson = await response.Content.ReadAsStringAsync(ct);

                if (!response.IsSuccessStatusCode)
                {
                    await CompleteLogAsync(
                        logId: log.Id,
                        httpStatusCode: httpStatusCode,
                        isSucceeded: false,
                        apiSuccess: null,
                        apiCode: null,
                        responseJson: responseJson,
                        errorMessage: $"API.ir failed with HTTP status code {httpStatusCode}.",
                        durationMilliseconds: stopwatch.ElapsedMilliseconds);

                    logCompleted = true;

                    throw new InvalidOperationException(
                        $"API.ir failed: {httpStatusCode} - {responseJson}");
                }

                var result = JsonSerializer.Deserialize<TResponse>(
                    responseJson,
                    JsonOptions);
                if (result is null)
                {
                    await CompleteLogAsync(
                        logId: log.Id,
                        httpStatusCode: httpStatusCode,
                        isSucceeded: false,
                        apiSuccess: null,
                        apiCode: null,
                        responseJson: responseJson,
                        errorMessage: "Empty or invalid response from API.ir.",
                        durationMilliseconds: stopwatch.ElapsedMilliseconds);

                    logCompleted = true;

                    throw new InvalidOperationException(
                        "Empty or invalid response from API.ir.");
                }
                var isSucceeded = hasData(result);
                await CompleteLogAsync(
                    logId: log.Id,
                    httpStatusCode: httpStatusCode,
                    isSucceeded: isSucceeded,
                    apiSuccess: getApiSuccess(result),
                    apiCode: getApiCode(result),
                    responseJson: responseJson,
                    errorMessage: isSucceeded ? null : getApiMessage(result),
                    durationMilliseconds: stopwatch.ElapsedMilliseconds);

                logCompleted = true;

                return result;
            }
            catch (OperationCanceledException) when (ct.IsCancellationRequested)
            {
                if (!logCompleted)
                {
                    await CompleteLogAsync(
                        logId: log.Id,
                        httpStatusCode: httpStatusCode,
                        isSucceeded: false,
                        apiSuccess: null,
                        apiCode: null,
                        responseJson: responseJson,
                        errorMessage: "Request was cancelled by caller.",
                        durationMilliseconds: stopwatch.ElapsedMilliseconds);

                    logCompleted = true;
                }

                throw;
            }
            catch (Exception ex)
            {
                if (!logCompleted)
                {
                    await CompleteLogAsync(
                        logId: log.Id,
                        httpStatusCode: httpStatusCode,
                        isSucceeded: false,
                        apiSuccess: null,
                        apiCode: null,
                        responseJson: responseJson,
                        errorMessage: ex.Message,
                        durationMilliseconds: stopwatch.ElapsedMilliseconds);

                    logCompleted = true;
                }

                _logger.LogError(
                    ex,
                    "API.ir inquiry failed. ServiceName: {ServiceName}, Endpoint: {Endpoint}, CorrelationId: {CorrelationId}",
                    serviceName,
                    endpoint,
                    log.CorrelationId);

                throw;
            }
        }

        private Task CompleteLogAsync(
            Guid logId,
            int? httpStatusCode,
            bool isSucceeded,
            bool? apiSuccess,
            int? apiCode,
            string? responseJson,
            string? errorMessage,
            long durationMilliseconds)
        {
            /*
             * از CancellationToken.None استفاده می‌کنیم تا حتی وقتی کاربر صفحه را بست
             * یا Request اصلی Cancel شد، وضعیت نهایی Audit Log ثبت شود.
             */
            return _apiInquiryLogService.CompleteAsync(
                logId: logId,
                httpStatusCode: httpStatusCode,
                isSucceeded: isSucceeded,
                apiSuccess: apiSuccess,
                apiCode: apiCode,
                responsePayload: responseJson,
                errorMessage: errorMessage,
                durationMilliseconds: durationMilliseconds,
                ct: CancellationToken.None);
        }
    }
}
