using Microsoft.AspNetCore.Mvc.Rendering;
using System.ComponentModel.DataAnnotations;

namespace ShipRate.Application.ViewModels
{
    public class PaymentDocumentSearchVm
    {
        [Display(Name = "Document Number")]
        public string? DocumentNumber { get; set; }

        [Display(Name = "Person Name")]
        public string? PersonName { get; set; }
        public Guid? PersonId { get; set; }

        [Display(Name = "Tracking Number")]
        public string? TrackingNumber { get; set; }

        [Display(Name = "From Date")]
        public string? FromDateShamsi { get; set; }

        [Display(Name = "To Date")]
        public string? ToDateShamsi { get; set; }

        [Display(Name = "Company")]
        public int? CompanyId { get; set; }

        [Display(Name = "Payment Method")]
        public int? PaymentMethodId { get; set; }

        [Display(Name = "Cheque Purpose")]
        public int? ChequePurposeId { get; set; }

        [Display(Name = "Currency")]
        public int? CurrencyId { get; set; }

        [Display(Name = "Finance Direction")]
        public int? FinanceDirectionId { get; set; }

        public bool OnlyHasRemaining { get; set; } = false;


        public int Page { get; set; } = 1;

        public int PageSize { get; set; } = 10;

        public List<SelectListItem> Companies { get; set; } = new();
        public List<SelectListItem> Currencies { get; set; } = new();
        public List<SelectListItem> FinanceDirections { get; set; } = new();
        public List<SelectListItem> PaymentMethods { get; set; } = new();
        public List<SelectListItem> BankAccounts { get; set; } = new();
        public List<SelectListItem> ChequeTypes { get; set; } = new();
        public List<SelectListItem> Branches { get; set; } = new();
        public List<SelectListItem> ChequePurposes { get; set; } = new();
    }
    public class PaymentDocumentListItemVm
    {
        public Guid Id { get; set; }

        public long SystemId { get; set; }
        public bool HasVaultTransaction { get; set; }

        public string? DocumentNumber { get; set; }

        public string? DocumentDateShamsi { get; set; }

        public DateTime? DocumentDateGregorian { get; set; }

        public string? PersonName { get; set; }

        public string? CompanyName { get; set; }

        public string? CurrencyTitle { get; set; }

        public string? FinanceDirectionTitle { get; set; }

        public string? PaymentMethodTitle { get; set; }

        public decimal Amount { get; set; }

        public decimal AllocatedAmount { get; set; }

        public decimal UnAllocatedAmount { get; set; }

        public bool IsFullyAllocated;

        public string? TrackingNumber { get; set; }

        public bool HasCheque { get; set; }

        public DateTime CreatedAt { get; set; }
        public string? CreateUserName { get; set; }
        public DateTime? UpdatedAt { get; set; }
        public string? UpdateUserName { get; set; }
    }
    public class ChequeFormVm
    {
        public Guid? Id { get; set; }

        [Display(Name = "Cheque Number")]
        [StringLength(100)]
        public string? ChequeNumber { get; set; }

        [Display(Name = "Cheque Date")]
        [StringLength(10)]
        public string? ChequeDateShamsi { get; set; }

        public DateTime? ChequeDateGregorian { get; set; }

        [Display(Name = "Cheque Type")]
        public int? ChequeTypeId { get; set; }

        [Display(Name = "Cheque Purpose")]
        public int? ChequePurposeId { get; set; }


        [Display(Name = "Bank")]
        public Guid? BankId { get; set; }

        [Display(Name = "Bank Account")]
        public Guid? BankAccountId { get; set; }

        [Display(Name = "Bank Account No")]
        public string? BankAccountNo { get; set; }

        [Display(Name = "IBAN")]
        public string? IBAN { get; set; }

        [Display(Name = "Sayad No")]
        public string? SayadNo { get; set; }



        [Display(Name = "Account Holder Name")]
        [StringLength(200)]
        public string? AccountHolderName { get; set; }

        [Display(Name = "Description")]
        [StringLength(1000)]
        public string? Description { get; set; }
    }
    public class PaymentDocumentCreateEditVm : IValidatableObject
    {
        public Guid Id { get; set; }

        [Display(Name = "Document Number")]
        [Required(ErrorMessage = "Document number is required.")]
        [StringLength(100)]
        public string DocumentNumber { get; set; } = string.Empty;

        [Display(Name = "Document Date")]
        [Required(ErrorMessage = "Document date is required.")]
        [StringLength(10)]
        public string DocumentDateShamsi { get; set; } = string.Empty;

        public DateTime? DocumentDateGregorian { get; set; }

        [Display(Name = "Company")]
        [Required(ErrorMessage = "Company is required.")]
        public int? CompanyId { get; set; }


        [Display(Name = "Branch")]
        public Guid? BranchId { get; set; }



        [Display(Name = "Person")]
        [Required(ErrorMessage = "Person is required.")]
        public Guid? PersonId { get; set; }

        [Display(Name = "Person Name")]
        public string? PersonName { get; set; }

        [Display(Name = "Currency")]
        [Required(ErrorMessage = "Currency is required.")]
        public int? CurrencyId { get; set; }

        [Display(Name = "Finance Direction")]
        [Required(ErrorMessage = "Finance direction is required.")]
        public int? FinanceDirectionId { get; set; }

        [Display(Name = "Payment Method")]
        [Required(ErrorMessage = "Payment method is required.")]
        public int? PaymentMethodId { get; set; }

        [Display(Name = "Amount")]
        [Required(ErrorMessage = "Amount is required.")]
        [Range(0.01, double.MaxValue, ErrorMessage = "Amount must be greater than zero.")]
        public decimal Amount { get; set; }

        [Display(Name = "Tracking Number")]
        [StringLength(100)]
        public string? TrackingNumber { get; set; }

        [Display(Name = "Description")]
        [StringLength(1000)]
        public string? Description { get; set; }

        public ChequeFormVm Cheque { get; set; } = new();

        public List<SelectListItem> Companies { get; set; } = new();
        public List<SelectListItem> Currencies { get; set; } = new();
        public List<SelectListItem> FinanceDirections { get; set; } = new();
        public List<SelectListItem> PaymentMethods { get; set; } = new();
        public List<SelectListItem> Banks { get; set; } = new();
        public List<SelectListItem> BankAccounts { get; set; } = new();
        public List<SelectListItem> ChequeTypes { get; set; } = new();
        public List<SelectListItem> Branches { get; set; } = new();
        public List<SelectListItem> ChequePurposes { get; set; } = new();


        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            if (PaymentMethodId == 3)
            {
                if (string.IsNullOrWhiteSpace(Cheque.ChequeNumber))
                {
                    yield return new ValidationResult(
                        "Cheque number is required for cheque payment method.",
                        new[] { nameof(Cheque.ChequeNumber) });
                }

                if (string.IsNullOrWhiteSpace(Cheque.ChequeDateShamsi))
                {
                    yield return new ValidationResult(
                        "Cheque date is required for cheque payment method.",
                        new[] { nameof(Cheque.ChequeDateShamsi) });
                }

                if (!Cheque.BankId.HasValue)
                {
                    yield return new ValidationResult(
                        "Bank is required for cheque payment method.",
                        new[] { nameof(Cheque.BankId) });
                }

                //if (!Cheque.BankAccountId.HasValue)
                //{
                //    yield return new ValidationResult(
                //        "Bank account is required for cheque payment method.",
                //        new[] { nameof(Cheque.BankAccountId) });
                //}


                if (!Cheque.ChequeTypeId.HasValue)
                {
                    yield return new ValidationResult(
                        "Cheque type is required for cheque payment method.",
                        new[] { nameof(Cheque.ChequeTypeId) });
                }
            }
        }
    }
    public class PaymentDocumentDetailsVm
    {
        public Guid Id { get; set; }

        public string? DocumentNumber { get; set; }

        public string? DocumentDateShamsi { get; set; }

        public DateTime? DocumentDateGregorian { get; set; }

        public string? CompanyName { get; set; }

        public string? PersonName { get; set; }

        public string? CurrencyTitle { get; set; }

        public string? FinanceDirectionTitle { get; set; }

        public string? PaymentMethodTitle { get; set; }

        public decimal Amount { get; set; }

        public string? TrackingNumber { get; set; }

        public string? Description { get; set; }

        public ChequeDetailsVm? Cheque { get; set; }
    }

    public class ChequeDetailsVm
    {
        public Guid Id { get; set; }

        public string? ChequeNumber { get; set; }

        public string? ChequeDateShamsi { get; set; }

        public DateTime? ChequeDateGregorian { get; set; }

        public string? ChequeTypeTitle { get; set; }

        public string? BankName { get; set; }

        public string? BankAccountNumber { get; set; }

        public string? AccountHolderName { get; set; }

        public string? Description { get; set; }
    }
    public class PersonLookupVm
    {
        public Guid Id { get; set; }

        public string FullName { get; set; } = string.Empty;

        public string? NationalCode { get; set; }
    }

    public class DateConversionVm
    {
        public string? ShamsiDate { get; set; }
        public string? GregorianDisplay { get; set; }
        public string? GregorianValue { get; set; }
        public bool IsValid { get; set; }
        public string? ErrorMessage { get; set; }
        public string FieldName { get; set; } = string.Empty;
    }

    //public class AvailablePaymentDocumentDto
    //{
    //    public Guid PaymentDocumentId { get; set; }
    //    public string DocumentNo { get; set; } = string.Empty;
    //    public DateTime DocumentDate { get; set; }
    //    public string? DocumentDateShamsi { get; set; }
    //    public string PaymentMethodName { get; set; } = string.Empty;

    //    public decimal TotalAmount { get; set; }
    //    public decimal AllocatedAmount { get; set; }
    //    public decimal UnallocatedAmount { get; set; }

    //    public int CurrencyId { get; set; }
    //    public string CurrencyCode { get; set; } = string.Empty;
    //}

    public class PaymentDocumentUsagePanelVm
    {
        public Guid PaymentDocumentId { get; set; }

        public string PaymentDocumentNumber { get; set; } = string.Empty;

        public decimal Amount { get; set; }

        public decimal AllocatedAmount { get; set; }

        public decimal RemainingAmount { get; set; }

        public string CurrencyTitle { get; set; } = string.Empty;

        public List<PaymentDocumentUsageItemVm> Items { get; set; } = [];
    }

    public class PaymentDocumentUsageItemVm
    {
        public int RowNumber { get; set; }

        public string UsageType { get; set; } = string.Empty;

        public string ReferenceNumber { get; set; } = string.Empty;

        public string? ReferenceDate { get; set; }

        public string? PartyName { get; set; }

        public decimal AllocatedAmount { get; set; }

        public string CurrencyTitle { get; set; } = string.Empty;

        public string? Description { get; set; }
    }






}
