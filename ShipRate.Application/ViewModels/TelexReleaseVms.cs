using Microsoft.AspNetCore.Mvc.Rendering;
using ShipRate.Application.DTOs;
using System.ComponentModel.DataAnnotations;

namespace ShipRate.Application.ViewModels
{
    public class TelexReleaseSearchViewModel
    {
        [Display(Name = "BL No")]
        public string? BLNo { get; set; }

        [Display(Name = "Status")]
        public int? TelexReleaseStatusId { get; set; }

        public int PageNumber { get; set; } = 1;
        public int PageSize { get; set; } = 10;
    }

    public class TelexReleaseListItemViewModel
    {
        public Guid Id { get; set; }
        public string? BLNo { get; set; }
        public string? StatusName { get; set; }
        public string? RequestNo { get; set; }
        public DateTime RequestDate { get; set; }
        public DateTime? ApprovalDate { get; set; }
        public DateTime? CompletedAt { get; set; }
        public string? Remarks { get; set; }
        public bool IsActive { get; set; }
        public string? CreatedByUserName { get; set; }
        public DateTime? CreateDate { get; set; }
        public string? ApproveByUserName { get; set; }
    }

    public class TelexReleaseCreateEditViewModel
    {
        public Guid? Id { get; set; }

        [Required]
        public int CompanyId { get; set; }

        public Guid? ShipmentId { get; set; }

        [Display(Name = "BL")]
        public Guid? BLId { get; set; }

        [Display(Name = "BL No")]
        public string? BLNo { get; set; }

        public Guid? CustomerId { get; set; }

        public string? CustomerName { get; set; }


        public Guid? ShippingLineId { get; set; }

        [Required]
        [Display(Name = "Line/Forwarder Type")]
        public int LineForwarderTypeId { get; set; }

        [Required]
        [Display(Name = "Request Date")]
        public DateTime RequestDate { get; set; } = DateTime.Now;

        [Display(Name = "Request No")]
        public string? RequestNo { get; set; }




        [Required]
        [Display(Name = "Status")]
        public int TelexReleaseStatusId { get; set; }

        public Guid? BLSurrenderId { get; set; }
        public Guid? ApprovedByUserId { get; set; }

        [Display(Name = "Approval Date")]
        public DateTime? ApprovalDate { get; set; }

        [Display(Name = "Completed At")]
        public DateTime? CompletedAt { get; set; }

        [StringLength(200)]
        public string? Remarks { get; set; }

        [Display(Name = "Active")]
        public bool IsActive { get; set; } = true;

        public IEnumerable<SelectListItem> LineForwarderTypes { get; set; } = new List<SelectListItem>();
        public IEnumerable<SelectListItem> Companies { get; set; } = new List<SelectListItem>();
        public IEnumerable<SelectListItem> Carriers { get; set; } = new List<SelectListItem>();
    }

    public class TelexReleaseDetailsViewModel
    {
        public Guid Id { get; set; }
        public int CompanyId { get; set; }
        public Guid? ShipmentId { get; set; }
        public Guid? BLId { get; set; }
        public string? BLNo { get; set; }
        public Guid? CustomerId { get; set; }
        public Guid? ShippingLineId { get; set; }
        public int LineForwarderTypeId { get; set; }
        public string? LineForwarderTypeName { get; set; }
        public DateTime RequestDate { get; set; }
        public int TelexReleaseStatusId { get; set; }
        public string? StatusName { get; set; }
        public Guid? BLSurrenderId { get; set; }
        public Guid? ApprovedByUserId { get; set; }
        public DateTime? ApprovalDate { get; set; }
        public DateTime? CompletedAt { get; set; }
        public string? Remarks { get; set; }
        public bool IsActive { get; set; }
        public bool IsDeleted { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
        public Guid CreateUserId { get; set; }
        public Guid? EditUserId { get; set; }

        public string? CompanyName { get; set; }
        public string? CarrierName { get; set; }
        public string? CustomerName { get; set; }
    }

    public class TelexReleaseIndexViewModel
    {
        public TelexReleaseSearchViewModel Search { get; set; } = new();
        public PagedResultDto<TelexReleaseListItemViewModel> Result { get; set; } = new();
        public IEnumerable<SelectListItem> Statuses { get; set; } = new List<SelectListItem>();
    }

    public class BLLookupViewModel
    {
        public string? SearchTerm { get; set; }
        public List<BLItemLookupViewModel> Items { get; set; } = new();
    }

    public class BLItemLookupViewModel
    {
        public Guid Id { get; set; }
        public string BLNo { get; set; } = string.Empty;
    }
}
