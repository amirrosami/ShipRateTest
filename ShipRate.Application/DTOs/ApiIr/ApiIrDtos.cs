using Microsoft.AspNetCore.Mvc.Rendering;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace ShipRate.Application.DTOs.ApiIr
{
    public sealed class ApiIrOptions
    {
        public string BaseUrl { get; set; } = default!;
        public string BearerToken { get; set; } = default!;
    }
    public sealed class PersonInfoRequest
    {
        public string NationalCode { get; set; } = default!;
        public string BirthDate { get; set; } = default!;
    }
    public sealed class PersonInfoResponse
    {
        [JsonPropertyName("data")]
        public PersonInfoData? Data { get; set; }

        [JsonPropertyName("success")]
        public bool Success { get; set; }

        [JsonPropertyName("code")]
        public int Code { get; set; }

        [JsonPropertyName("message")]
        public string? Message { get; set; }
    }

    public sealed class PersonInfoData
    {
        [JsonPropertyName("nationalCode")]
        public string? NationalCode { get; set; }

        [JsonPropertyName("firstName")]
        public string? FirstName { get; set; }

        [JsonPropertyName("lastName")]
        public string? LastName { get; set; }

        [JsonPropertyName("fatherName")]
        public string? FatherName { get; set; }

        [JsonPropertyName("gender")]
        public int? Gender { get; set; }

        [JsonPropertyName("alive")]
        public bool? Alive { get; set; }
    }

    public sealed class PersonInfoLookupVm
    {
        public string NationalCode { get; set; } = string.Empty;
        public string BirthDate { get; set; } = string.Empty;

        public PersonInfoResponse? Result { get; set; }
        public string? ErrorMessage { get; set; }
    }

    public sealed class CompanyInfoRequest
    {
        [JsonPropertyName("nationalID")]
        public string NationalId { get; set; } = string.Empty;
    }
    public sealed class CompanyInfoResponse
    {
        [JsonPropertyName("data")]
        public CompanyInfoData? Data { get; set; }

        [JsonPropertyName("success")]
        public bool Success { get; set; }

        [JsonPropertyName("code")]
        public int Code { get; set; }

        [JsonPropertyName("message")]
        public string? Message { get; set; }
    }
    public sealed class CompanyInfoData
    {
        [JsonPropertyName("companyType")]
        public string? CompanyType { get; set; }

        [JsonPropertyName("name")]
        public string? Name { get; set; }

        /*
         * حتی اگر API مقدار را عددی برگرداند، System.Text.Json
         * با JsonNumberHandling.AllowReadingFromString نیز قابل توسعه است.
         * طبق نمونه فعلی API، این مقدار numeric است.
         */
        [JsonPropertyName("nationalID")]
        public long? NationalId { get; set; }

        [JsonPropertyName("registerNumber")]
        public long? RegisterNumber { get; set; }

        [JsonPropertyName("registerDate")]
        public string? RegisterDate { get; set; }

        [JsonPropertyName("active")]
        public bool? Active { get; set; }

        [JsonPropertyName("address")]
        public string? Address { get; set; }

        [JsonPropertyName("postalCode")]
        public string? PostalCode { get; set; }

        [JsonPropertyName("province")]
        public string? Province { get; set; }

        [JsonPropertyName("city")]
        public string? City { get; set; }

        [JsonPropertyName("endDate")]
        public string? EndDate { get; set; }
    }

    public sealed class CompanyInfoLookupVm
    {
        [Required(ErrorMessage = "شناسه ملی شرکت الزامی است.")]
        [RegularExpression(@"^\d{11}$", ErrorMessage = "شناسه ملی شرکت باید دقیقاً 11 رقم باشد.")]
        public string NationalId { get; set; } = string.Empty;

        public CompanyInfoResponse? Result { get; set; }

        public string? ErrorMessage { get; set; }
    }

    public sealed class ChequeInfoRequest
    {
        [JsonPropertyName("nationalCode")]
        public string NationalCode { get; set; } = string.Empty;
    }



    public sealed class ChequeInfoResponse
    {
        [JsonPropertyName("data")]
        public ChequeInfoData? Data { get; set; }

        [JsonPropertyName("success")]
        public bool Success { get; set; }

        [JsonPropertyName("code")]
        public int Code { get; set; }

        [JsonPropertyName("message")]
        public string? Message { get; set; }
    }

    public sealed class ChequeInfoData
    {
        [JsonPropertyName("nationalCode")]
        public string? NationalCode { get; set; }

        [JsonPropertyName("legalId")]
        public string? LegalId { get; set; }

        [JsonPropertyName("name")]
        public string? Name { get; set; }

        [JsonPropertyName("count")]
        public int Count { get; set; }

        [JsonPropertyName("chequeList")]
        public List<ChequeItem>? ChequeList { get; set; }
    }


    public sealed class ChequeItem
    {
        [JsonPropertyName("accountNumber")]
        public string? AccountNumber { get; set; }

        [JsonPropertyName("amount")]
        public string? Amount { get; set; }

        [JsonPropertyName("bouncedAmount")]
        public string? BouncedAmount { get; set; }

        [JsonPropertyName("bankCode")]
        public string? BankCode { get; set; }

        [JsonPropertyName("branchCode")]
        public string? BranchCode { get; set; }

        [JsonPropertyName("branchDescription")]
        public string? BranchDescription { get; set; }

        [JsonPropertyName("dishonoringBranchName")]
        public string? DishonoringBranchName { get; set; }

        [JsonPropertyName("dishonorReason")]
        public string? DishonorReason { get; set; }

        [JsonPropertyName("branchCodeBounced")]
        public string? BranchCodeBounced { get; set; }

        [JsonPropertyName("chequeDate")]
        public string? ChequeDate { get; set; }

        [JsonPropertyName("backDate")]
        public string? BackDate { get; set; }

        [JsonPropertyName("chequeID")]
        public string? ChequeId { get; set; }

        [JsonPropertyName("chequeNumber")]
        public string? ChequeNumber { get; set; }
    }
    public sealed class ChequeInfoLookupVm
    {
        [Required(ErrorMessage = "کد ملی یا شناسه ملی الزامی است.")]
        [RegularExpression(@"^\d{10,11}$", ErrorMessage = "فرمت کد ملی یا شناسه ملی صحیح نیست.")]
        public string NationalCode { get; set; } = string.Empty;
        public ChequeInfoResponse? Result { get; set; }
        public string? ErrorMessage { get; set; }
    }


    public sealed class ApiInquiryLogFilterDto
    {
        public Guid? UserId { get; set; }

        public string? ServiceName { get; set; }

        public DateTime? FromDateUtc { get; set; }

        public DateTime? ToDateUtc { get; set; }

        public string? SearchTerm { get; set; }

        public int PageNumber { get; set; } = 1;

        public int PageSize { get; set; } = 20;
    }

    public sealed class ApiInquiryLogListItemDto
    {
        public Guid Id { get; set; }

        public Guid? UserId { get; set; }
        public string? UserName { get; set; }

        public string Provider { get; set; } = string.Empty;
        public string ServiceName { get; set; } = string.Empty;
        public string Endpoint { get; set; } = string.Empty;

        public string CorrelationId { get; set; } = string.Empty;

        public int? HttpStatusCode { get; set; }
        public int? ApiCode { get; set; }
        public bool? ApiSuccess { get; set; }

        public bool IsSucceeded { get; set; }
        public bool IsCompleted { get; set; }

        public long? DurationMilliseconds { get; set; }

        public DateTime RequestStartedAtUtc { get; set; }
        public DateTime? ResponseReceivedAtUtc { get; set; }

        public string? ErrorMessage { get; set; }
    }

    public sealed class ApiInquiryLogDetailsDto
    {
        public Guid Id { get; set; }

        public Guid? UserId { get; set; }
        public string? UserName { get; set; }

        public string Provider { get; set; } = string.Empty;
        public string ServiceName { get; set; } = string.Empty;
        public string Endpoint { get; set; } = string.Empty;
        public string CorrelationId { get; set; } = string.Empty;

        public int? HttpStatusCode { get; set; }
        public int? ApiCode { get; set; }
        public bool? ApiSuccess { get; set; }

        public bool IsSucceeded { get; set; }
        public bool IsCompleted { get; set; }

        public long? DurationMilliseconds { get; set; }

        public DateTime RequestStartedAtUtc { get; set; }
        public DateTime? ResponseReceivedAtUtc { get; set; }

        public string? ErrorMessage { get; set; }

        // Decrypted JSON
        public string? RequestPayload { get; set; }
        public string? ResponsePayload { get; set; }

        public string? DecryptionError { get; set; }
    }
    public sealed class ApiInquiryLogIndexVm
    {
        public ApiInquiryLogFilterDto Filter { get; set; } = new();

        public PagedResultDto<ApiInquiryLogListItemDto> Result { get; set; } = new();

        public List<SelectListItem> Users { get; set; } = [];

        public List<SelectListItem> Services { get; set; } =
        [
            new("All Services", ""),
        new("Person Information", "PersonInfo"),
        new("Company Information", "CompanyInfo"),
        new("Unpaid Cheque Pro", "UnpaidChequePro")
        ];

        public int[] PageSizes { get; set; } = [10, 20, 50, 100];
    }
}
