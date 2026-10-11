using Microsoft.AspNetCore.Mvc.Rendering;
using System.ComponentModel.DataAnnotations;

namespace ShipRate.Application.DTOs
{

    public class VaultFilterDto
    {
        public int? CompanyId { get; set; }
        public Guid? BranchId { get; set; }
        public bool? IsActive { get; set; }
        public string? SearchTerm { get; set; }
        public int PageNumber { get; set; } = 1;
        public int PageSize { get; set; } = 15;
    }

    public class VaultItemRowDto
    {
        public Guid Id { get; set; }
        public string VaultName { get; set; } = string.Empty;
        public int CompanyId { get; set; }
        public string? CompanyName { get; set; }
        public Guid BranchId { get; set; }
        public string? BranchName { get; set; }
        public Guid? KeeperUserId { get; set; }
        public string? KeeperUserName { get; set; }
        public string? Remarks { get; set; }
        public bool IsActive { get; set; }
        public int TotalCurrenciesCount { get; set; }
        public int ActiveCustodyItemsCount { get; set; }
    }

    public class VaultFormVm
    {
        public Guid? Id { get; set; }
        [Required] public string VaultName { get; set; } = string.Empty;
        [Required] public int CompanyId { get; set; }
        [Required] public Guid BranchId { get; set; }
        public Guid? KeeperUserId { get; set; }
        public string? Remarks { get; set; }
        public bool IsActive { get; set; } = true;

        // Dropdown Data
        public List<SelectListItem> Companies { get; set; } = new();
        public List<SelectListItem> Branches { get; set; } = new();
        public List<SelectListItem> Keepers { get; set; } = new();
    }




    public class VaultDto
    {
        public Guid Id { get; set; }
        public string VaultName { get; set; } = null!;
        public string? Remarks { get; set; }
        public bool IsActive { get; set; }
        public string? KeeperUserName { get; set; }
        public string? BranchName { get; set; }


        public int CompanyId { get; set; }
        public Guid BranchId { get; set; }
        public Guid KeeperUserId { get; set; }

        public List<VaultAssetDto> Assets { get; set; } = new();
    }

    public class VaultUpsertDto
    {
        public Guid? Id { get; set; }
        [Required] public string VaultName { get; set; } = string.Empty;
        public int CompanyId { get; set; }
        public Guid BranchId { get; set; }
        public Guid? KeeperUserId { get; set; }
        public string? Remarks { get; set; }
        public bool IsActive { get; set; } = true;


        public List<SelectListItem> CompanyList { get; set; } = new();
        public List<SelectListItem> BranchList { get; set; } = new();
        public List<SelectListItem> KeeperUserList { get; set; } = new();

    }

    public class VaultAssetDto
    {
        public Guid Id { get; set; }
        public Guid VaultId { get; set; }
        public int CurrencyId { get; set; }
        public string CurrencyCode { get; set; } = string.Empty;
        public decimal Amount { get; set; }
        public DateTime LastUpdated { get; set; }
    }

    public class VaultMovementFilterDto
    {
        public Guid? VaultId { get; set; }
        public int? CurrencyId { get; set; }
        public int? InOutId { get; set; }
        public int? SourceTypeId { get; set; }
        public DateTime? FromDate { get; set; }
        public DateTime? ToDate { get; set; }
        public string? SearchTerm { get; set; }
        public int PageNumber { get; set; } = 1;
        public int PageSize { get; set; } = 15;
    }

    public class VaultMovementDto
    {
        public Guid Id { get; set; }
        public Guid VaultId { get; set; }
        public string VaultName { get; set; } = string.Empty;
        public int CurrencyId { get; set; }
        public string CurrencyCode { get; set; } = string.Empty;
        public int InOutId { get; set; }
        public string InOutTitle { get; set; } = string.Empty;
        public decimal Amount { get; set; }
        public string? ReferenceNo { get; set; }
        public string? ReferenceNumber { get; set; }
        public string SourceTypeName { get; set; } = string.Empty;
        public string? DepositorOrReceiver { get; set; }
        public string? Remarks { get; set; }
        public string? ExecutedByUserName { get; set; }
        public DateTime? MovementDate { get; set; }
    }
    public class PaymentDocumentLookupDto
    {
        public Guid Id { get; set; }
        public string? DocumentNo { get; set; }
        public string? PayerName { get; set; }
        public DateTime? DocumentDate { get; set; }
        public decimal Amount { get; set; }
        public int CurrencyId { get; set; }
        public string? CurrencyCode { get; set; }
    }


    public class CreateVaultMovementDto
    {
        [Required] public Guid VaultId { get; set; }
        [Required] public int CurrencyId { get; set; }
        [Required] public int InOutId { get; set; } // 1: In, 2: Out
        [Range(0.01, double.MaxValue)] public decimal Amount { get; set; }
        [Required] public int VaultMovementSourceTypeId { get; set; }
        public string? ReferenceNo { get; set; }
        public string? ReferenceNumber { get; set; }
        public string? DepositorOrReceiver { get; set; }
        public string? Remarks { get; set; }

        public Guid? PaymentDocumentId { get; set; }


    }

    public class CashPaymentRequestFilterDto
    {
        public Guid? VaultId { get; set; }
        public int? StatusId { get; set; }
        public string? SearchTerm { get; set; }
        public int PageNumber { get; set; } = 1;
        public int PageSize { get; set; } = 15;
    }

    public class CashPaymentRequestDto
    {
        public Guid Id { get; set; }
        public string RequestNo { get; set; } = string.Empty;
        public Guid VaultId { get; set; }
        public string VaultName { get; set; } = string.Empty;
        public string? PayeeName { get; set; }
        public decimal Amount { get; set; }
        public string CurrencyCode { get; set; } = string.Empty;
        public string Reason { get; set; } = string.Empty;
        public int StatusId { get; set; }
        public string StatusName { get; set; } = string.Empty;
        public string? CreatedByName { get; set; }
        public DateTime CreatedDate { get; set; }
        public string? InitialConfirmedByName { get; set; }
        public string? FinalConfirmedByName { get; set; }
        public string? PaidByName { get; set; }
        public DateTime? PaidDate { get; set; }
        public string? RejectReason { get; set; }
    }

    public class CreateCashPaymentRequestDto
    {
        [Required] public Guid VaultId { get; set; }
        public Guid? PartyId { get; set; }
        public string? PayeeName { get; set; }
        [Range(0.01, double.MaxValue)] public decimal Amount { get; set; }
        [Required] public int CurrencyId { get; set; }
        [Required] public string Reason { get; set; } = string.Empty;
    }

    public class VaultItemFilterDto
    {
        public Guid? VaultId { get; set; }
        public int? CategoryId { get; set; }
        public int? StatusId { get; set; }
        public string? SearchTerm { get; set; }
        public bool OnlyExpiringSoon { get; set; }
        public int PageNumber { get; set; } = 1;
        public int PageSize { get; set; } = 15;
    }

    public class VaultItemDto
    {
        public Guid Id { get; set; }
        public Guid VaultId { get; set; }
        public string VaultName { get; set; } = string.Empty;
        public int VaultItemCategoryId { get; set; }
        public string CategoryName { get; set; } = string.Empty;
        public string Title { get; set; } = string.Empty;
        public string? IdentifierNo { get; set; }
        public string? OwnerOrBeneficiary { get; set; }
        public decimal? NominalValue { get; set; }
        public string? CurrencyCode { get; set; }
        public DateTime? IssueDate { get; set; }
        public DateTime? ExpiryDate { get; set; }
        public string? ShelfLocation { get; set; }
        public int VaultItemStatusId { get; set; }
        public string StatusName { get; set; } = string.Empty;
        public string? CurrentHolderName { get; set; }
        public DateTime? CheckedOutDate { get; set; }
        public DateTime? ExpectedReturnDate { get; set; }
        public string? AttachmentPath { get; set; }
    }

    public class VaultItemUpsertDto
    {
        public Guid? Id { get; set; }
        [Required] public Guid VaultId { get; set; }
        [Required] public int VaultItemCategoryId { get; set; }
        [Required] public string Title { get; set; } = string.Empty;
        public string? IdentifierNo { get; set; }
        public string? OwnerOrBeneficiary { get; set; }
        public decimal? NominalValue { get; set; }
        public int? CurrencyId { get; set; }
        public DateTime? IssueDate { get; set; }
        public DateTime? ExpiryDate { get; set; }
        public string? ShelfLocation { get; set; }
        public string? Description { get; set; }
    }

    public class VaultCustodyActionDto
    {
        [Required] public Guid VaultItemId { get; set; }
        [Required] public int ActionTypeId { get; set; }
        public Guid? HandedToUserId { get; set; }
        public string? HandedToExternalName { get; set; }
        [Required] public string Reason { get; set; } = string.Empty;
        public DateTime? ExpectedReturnDate { get; set; }
    }

    public class LookupItemDto
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string LocalName { get; set; } = string.Empty;
        public string EntityType { get; set; } = string.Empty;
    }

    public class LookupUpsertDto
    {
        public int Id { get; set; }
        [Required] public string Name { get; set; } = string.Empty;
        [Required] public string LocalName { get; set; } = string.Empty;
        [Required] public string EntityType { get; set; } = string.Empty;
    }

    public class VaultCockpitVm
    {
        public List<VaultDto> Vaults { get; set; } = new();
        public Guid SelectedVaultId { get; set; }
        public PagedResultDto<VaultMovementDto> RecentMovements { get; set; } = new();
        public PagedResultDto<CashPaymentRequestDto> PendingPaymentRequests { get; set; } = new();
        public PagedResultDto<VaultItemDto> VaultItems { get; set; } = new();
    }
    public class VaultItemCustodyLogDto
    {
        public Guid Id { get; set; }
        public int VaultCustodyActionTypeId { get; set; }
        public string ActionTypeName { get; set; } = string.Empty;
        public DateTime ActionDate { get; set; }
        public string ExecutedByName { get; set; } = string.Empty;
        public string ReceiverName { get; set; } = string.Empty;
        public string Reason { get; set; } = string.Empty;
        public DateTime? ExpectedReturnDate { get; set; }
        public string? HandoverReceiptScanPath { get; set; }
    }

    public class VaultItemCustodyHistoryViewModel
    {
        public Guid VaultItemId { get; set; }
        public string ItemTitle { get; set; } = string.Empty;
        public string? TrackingNo { get; set; }
        public string? CurrentCustodianName { get; set; }
        public List<VaultItemCustodyLogDto> Logs { get; set; } = new();
    }


}
