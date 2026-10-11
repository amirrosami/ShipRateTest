using Microsoft.AspNetCore.Mvc.Rendering;
using System.ComponentModel.DataAnnotations;

namespace ShipRate.Application.DTOs.Finance
{
    public class TreasuryDocumentSearchDto
    {
        public string? DocumentNo { get; set; }
        public DateTime? FromDocumentDate { get; set; }
        public DateTime? ToDocumentDate { get; set; }
        public string? PartyName { get; set; }
        public int? TreasuryDocumentStatusId { get; set; }

        public int PageNumber { get; set; } = 1;
        public int PageSize { get; set; } = 10;
    }
    public class TreasuryDocumentListItemDto
    {
        public Guid Id { get; set; }

        public string? DocumentNo { get; set; }
        public DateTime DocumentDate { get; set; }

        public int CompanyId { get; set; }
        public string CompanyName { get; set; } = string.Empty;

        public Guid? PartyId { get; set; }
        public string PartyName { get; set; } = string.Empty;

        public int TreasuryDocumentTypeId { get; set; }
        public string TreasuryDocumentTypeName { get; set; } = string.Empty;

        public int TreasuryDocumentStatusId { get; set; }
        public string TreasuryDocumentStatusName { get; set; } = string.Empty;

        public decimal TotalAmount { get; set; }

        public int CurrencyId { get; set; }
        public string CurrencyCode { get; set; } = string.Empty;
    }

    public class TreasuryDocumentFormDto
    {
        public Guid? Id { get; set; }

        public int CompanyId { get; set; }
        public string? DocumentNo { get; set; }

        public int TreasuryDocumentTypeId { get; set; }
        public DateTime DocumentDate { get; set; } = DateTime.Today;

        public Guid? PartyId { get; set; }
        public string? PartyName { get; set; }

        public int CurrencyId { get; set; }
        public string? Description { get; set; }

        public int TreasuryDocumentStatusId { get; set; }

        public List<TreasuryDocumentPaymentSelectorDto> PaymentItems { get; set; } = new();
    }
    public class TreasuryDocumentPaymentSelectorDto
    {
        public Guid PaymentDocumentId { get; set; }

        public string DocumentNo { get; set; } = string.Empty;
        public DateTime DocumentDate { get; set; }

        public decimal Amount { get; set; }

        public int CurrencyId { get; set; }
        public string CurrencyCode { get; set; } = string.Empty;

        public string PaymentMethodName { get; set; } = string.Empty;

        public bool IsSelected { get; set; }

        public bool IsCurrentDocumentItem { get; set; }
    }
    public class TreasuryDocumentSaveDto
    {
        public Guid? Id { get; set; }

        [Required]
        public int CompanyId { get; set; }

        public string? DocumentNo { get; set; }

        [Required]
        public int TreasuryDocumentTypeId { get; set; }

        [Required]
        public DateTime DocumentDate { get; set; }

        [Required]
        public Guid? PartyId { get; set; }

        [Required]
        public int CurrencyId { get; set; }

        public string? Description { get; set; }

        [Required]
        public int TreasuryDocumentStatusId { get; set; }

        public List<Guid> SelectedPaymentDocumentIds { get; set; } = new();
    }

    public class PersonLookupDto
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = string.Empty;
    }
    public class TreasuryDocumentIndexVm
    {
        public TreasuryDocumentSearchDto Search { get; set; } = new();

        public PagedResultDto<TreasuryDocumentListItemDto> Result { get; set; } = default!;

        public List<SelectListItem> Statuses { get; set; } = new();
    }
    public class TreasuryDocumentFormVm
    {
        public TreasuryDocumentFormDto Form { get; set; } = new();

        public List<SelectListItem> Companies { get; set; } = new();
        public List<SelectListItem> Currencies { get; set; } = new();
        public List<SelectListItem> Types { get; set; } = new();
        public List<SelectListItem> Statuses { get; set; } = new();
    }

}
