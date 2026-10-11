using Microsoft.AspNetCore.Mvc.Rendering;
using ShipRate.Application.DTOs;
using System.ComponentModel.DataAnnotations;

namespace ShipRate.Application.ViewModels
{
    public class TIRCarnetSearchVm
    {
        public string? TIRCarnetNumber { get; set; }
        public int? IssuingCountryId { get; set; }
        public int? IssuingAssociationId { get; set; }
        public int? StatusId { get; set; }
        public DateTime? IssueDateFrom { get; set; }
        public DateTime? IssueDateTo { get; set; }
        public DateTime? ExpiryDateFrom { get; set; }
        public DateTime? ExpiryDateTo { get; set; }
        public bool? IsActive { get; set; }

        public int PageNumber { get; set; } = 1;
        public int PageSize { get; set; } = 10;

        public string? SortColumn { get; set; } = "IssueDate";
        public string? SortDirection { get; set; } = "desc";

        public List<SelectListItem> Countries { get; set; } = new();
        public List<SelectListItem> Associations { get; set; } = new();
        public List<SelectListItem> Statuses { get; set; } = new();
        public List<SelectListItem> PageSizeOptions { get; set; } = new()
    {
        new SelectListItem { Value = "10", Text = "10" },
        new SelectListItem { Value = "20", Text = "20" },
        new SelectListItem { Value = "50", Text = "50" },
        new SelectListItem { Value = "100", Text = "100" }
    };
    }

    public class TIRCarnetListItemVm
    {
        public Guid Id { get; set; }
        public string TIRCarnetNumber { get; set; } = string.Empty;

        public int IssuingCountryId { get; set; }
        public string IssuingCountryName { get; set; } = string.Empty;

        public int IssuingAssociationId { get; set; }
        public string IssuingAssociationName { get; set; } = string.Empty;

        public DateTime? BuyDate { get; set; }
        public DateTime IssueDate { get; set; }
        public DateTime ExpiryDate { get; set; }
        public DateTime? ClosedDate { get; set; }

        public int VoletsCount { get; set; }
        public string? GuaranteeChainName { get; set; } = string.Empty;
        public decimal GuaranteeAmount { get; set; }

        public int CurrencyId { get; set; }
        public string CurrencyName { get; set; } = string.Empty;

        public DateTime? ReceivedFromDriverAt { get; set; }
        public DateTime? SubmittedToChamberAt { get; set; }
        public DateTime? SettledAt { get; set; }

        public int StatusId { get; set; }
        public string StatusTitle { get; set; } = string.Empty;

        public string? Remarks { get; set; }
        public bool IsActive { get; set; }

        public int AssignmentsCount { get; set; }
        public int UsagesCount { get; set; }
    }

    public class TIRCarnetCreateEditVm
    {
        public Guid? Id { get; set; }

        [Required]
        [StringLength(20)]
        [Display(Name = "TIR Carnet Number")]
        public string TIRCarnetNumber { get; set; } = string.Empty;

        [Required]
        [Display(Name = "Issuing Country")]
        public int IssuingCountryId { get; set; }

        [Display(Name = "Issuing Association")]
        public int? IssuingAssociationId { get; set; }

        [Display(Name = "Buy Date")]
        public DateTime? BuyDate { get; set; }

        [Required]
        [Display(Name = "Issue Date")]
        public DateTime IssueDate { get; set; } = DateTime.Today;

        [Required]
        [Display(Name = "Expiry Date")]
        public DateTime ExpiryDate { get; set; } = DateTime.Today;

        [Display(Name = "Closed Date")]
        public DateTime? ClosedDate { get; set; }

        [Required]
        [Range(1, 999)]
        [Display(Name = "Volets Count")]
        public int VoletsCount { get; set; }

        [Display(Name = "Guarantee Chain Name")]
        public string? GuaranteeChainName { get; set; } = string.Empty;

        [Required]
        [Range(0, int.MaxValue)]
        [Display(Name = "Guarantee Amount")]
        public decimal GuaranteeAmount { get; set; }

        [Required]
        [Display(Name = "Currency")]
        public int CurrencyId { get; set; }

        [Display(Name = "Received From Driver At")]
        public DateTime? ReceivedFromDriverAt { get; set; }

        [Display(Name = "Submitted To Chamber At")]
        public DateTime? SubmittedToChamberAt { get; set; }

        [Display(Name = "Settled At")]
        public DateTime? SettledAt { get; set; }

        [Required]
        [Display(Name = "Status")]
        public int StatusId { get; set; } = -1;

        [StringLength(200)]
        [Display(Name = "Remarks")]
        public string? Remarks { get; set; }

        [Display(Name = "Is Active")]
        public bool IsActive { get; set; } = true;

        public List<SelectListItem> Countries { get; set; } = new();
        public List<SelectListItem> Associations { get; set; } = new();
        public List<SelectListItem> Currencies { get; set; } = new();
        public List<SelectListItem> Statuses { get; set; } = new();
    }

    public class TIRCarnetDeleteVm
    {
        public Guid Id { get; set; }
        public string TIRCarnetNumber { get; set; } = string.Empty;
    }

    public class TIRAssignmentListItemVm
    {
        public Guid Id { get; set; }
        public Guid TIRCarnetId { get; set; }

        public Guid TruckId { get; set; }
        public string TruckDisplay { get; set; } = string.Empty;

        public DateTime TIRAssignmentAssignDate { get; set; }
        public DateTime? TIRAssignmentReleaseDate { get; set; }

        public string? Remarks { get; set; }
        public bool IsActive { get; set; }
    }

    public class TIRAssignmentCreateEditVm
    {
        public Guid? Id { get; set; }

        [Required]
        public Guid TIRCarnetId { get; set; }

        [Required]
        [Display(Name = "Truck")]
        public Guid TruckId { get; set; }

        [Required]
        [Display(Name = "Assign Date")]
        public DateTime TIRAssignmentAssignDate { get; set; } = DateTime.Now;

        [Display(Name = "Release Date")]
        public DateTime? TIRAssignmentReleaseDate { get; set; }

        [StringLength(200)]
        [Display(Name = "Remarks")]
        public string? Remarks { get; set; }

        [Display(Name = "Is Active")]
        public bool IsActive { get; set; } = true;

        public string TIRCarnetNumber { get; set; } = string.Empty;
        public List<SelectListItem> Trucks { get; set; } = new();
    }


    public class TIRAssignmentDeleteVm
    {
        public Guid Id { get; set; }
        public Guid TIRCarnetId { get; set; }
        public string TruckDisplay { get; set; } = string.Empty;
    }

    public class TIRUsageListItemVm
    {
        public Guid Id { get; set; }

        public Guid TIRAssignmentId { get; set; }
        public Guid TIRCarnetId { get; set; }

        public string TIRCarnetNumber { get; set; } = string.Empty;
        public string TruckDisplay { get; set; } = string.Empty;

        public Guid? BorderId { get; set; }
        public string? BorderName { get; set; }

        public Guid? CustomsOfficeId { get; set; }
        public string? CustomsOfficeName { get; set; }

        public int InOutId { get; set; }
        public string InOutName { get; set; } = string.Empty;

        public string VoletNo { get; set; } = string.Empty;
        public DateTime TIRUsageUsedAt { get; set; }

        public string? Remarks { get; set; }
        public bool IsActive { get; set; }
    }
    public class TIRUsageCreateEditVm
    {
        public Guid? Id { get; set; }

        [Required]
        public Guid TIRAssignmentId { get; set; }

        public Guid TIRCarnetId { get; set; }

        [Display(Name = "Border")]
        public Guid? BorderId { get; set; }

        [Display(Name = "Customs Office")]
        public Guid? CustomsOfficeId { get; set; }

        [Required]
        [Display(Name = "In / Out")]
        public int InOutId { get; set; }

        [Required]
        [StringLength(50)]
        [Display(Name = "Volet No")]
        public string VoletNo { get; set; } = string.Empty;

        [Required]
        [Display(Name = "Used At")]
        public DateTime TIRUsageUsedAt { get; set; } = DateTime.Now;

        [StringLength(200)]
        [Display(Name = "Remarks")]
        public string? Remarks { get; set; }

        [Display(Name = "Is Active")]
        public bool IsActive { get; set; } = true;

        public string TIRCarnetNumber { get; set; } = string.Empty;
        public string TruckDisplay { get; set; } = string.Empty;

        public List<SelectListItem> Borders { get; set; } = new();
        public List<SelectListItem> CustomsOffices { get; set; } = new();
        public List<SelectListItem> InOutItems { get; set; } = new();
    }
    public class TIRUsageDeleteVm
    {
        public Guid Id { get; set; }
        public Guid TIRAssignmentId { get; set; }
        public Guid TIRCarnetId { get; set; }

        public string TIRCarnetNumber { get; set; } = string.Empty;
        public string TruckDisplay { get; set; } = string.Empty;
        public string VoletNo { get; set; } = string.Empty;
    }
    public class TIRCarnetIndexVm
    {
        public TIRCarnetSearchVm Search { get; set; } = new();
        public PagedResultDto<TIRCarnetListItemVm> Result { get; set; } = new();
    }

    public class AssignmentsPanelVm
    {
        public Guid TIRCarnetId { get; set; }
        public string TIRCarnetNumber { get; set; } = string.Empty;
        public List<TIRAssignmentListItemVm> Items { get; set; } = new();
    }

    public class UsagesPanelVm
    {
        public Guid TIRAssignmentId { get; set; }
        public Guid TIRCarnetId { get; set; }

        public string TIRCarnetNumber { get; set; } = string.Empty;
        public string TruckDisplay { get; set; } = string.Empty;

        public List<TIRUsageListItemVm> Items { get; set; } = new();
    }

}
