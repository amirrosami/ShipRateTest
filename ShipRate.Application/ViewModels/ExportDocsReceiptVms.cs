using Microsoft.AspNetCore.Mvc.Rendering;
using System.ComponentModel.DataAnnotations;

namespace ShipRate.Application.ViewModels
{
    public class ExportDocsReceiptIndexVm
    {
        public string? ReceiptNo { get; set; }
        public Guid? BookingId { get; set; }
        public Guid? PartyId { get; set; }
        public int? ReceiptDirectionId { get; set; }
        public DateTime? FromDate { get; set; }
        public DateTime? ToDate { get; set; }

        public int PageNumber { get; set; } = 1;
        public int PageSize { get; set; } = 10;
    }

    public class ExportDocsReceiptListItemVm
    {
        public Guid Id { get; set; }
        public string ReceiptNo { get; set; } = string.Empty;
        public DateTime ReceiptDate { get; set; }
        public string PartyName { get; set; } = string.Empty;
        public string BookingNo { get; set; } = string.Empty;
        public string ReceiptDirectionName { get; set; } = string.Empty;
        public string? PersonPersonName { get; set; }
        public string? Remarks { get; set; }
    }
    public class ExportDocsReceiptDocumentRowVm
    {
        public Guid? Id { get; set; }
        public Guid DocumentId { get; set; }
        public string DocumentName { get; set; } = string.Empty;
    }

    public class ExportDocsReceiptContainerRowVm
    {
        public Guid? Id { get; set; }
        public Guid? ContainerId { get; set; }
        public Guid? ContainerUseId { get; set; }
        public string? ContainerNo { get; set; }
        public DateTime? RBSDate { get; set; }
    }
    public class ExportDocsReceiptFormVm
    {
        public Guid? Id { get; set; }

        [Required]
        public int CompanyId { get; set; }

        [Required]
        public int ReceiptDirectionId { get; set; }

        [Required]
        public Guid PartyId { get; set; }

        [Required]
        public Guid BookingId { get; set; }

        [Required]
        [MaxLength(50)]
        public string ReceiptNo { get; set; } = string.Empty;

        [Required]
        public DateTime ReceiptDate { get; set; } = DateTime.Today;

        [MaxLength(200)]
        public string? PersonPersonName { get; set; }

        [MaxLength(1000)]
        public string? Remarks { get; set; }

        public bool IsEditMode => Id.HasValue;
        public bool HeaderSaved => Id.HasValue;

        public List<ExportDocsReceiptDocumentRowVm> Documents { get; set; } = new();
        public List<ExportDocsReceiptContainerRowVm> Containers { get; set; } = new();

        public List<SelectListItem> ReceiptDirections { get; set; } = new();
        public List<SelectListItem> Parties { get; set; } = new();
        public List<SelectListItem> Bookings { get; set; } = new();
        public List<SelectListItem> Companies { get; set; } = new();
    }
    public class ExportDocsReceiptContainerItemVm
    {
        public Guid? Id { get; set; }
        public Guid? ContainerId { get; set; }
        public Guid? ContainerUseId { get; set; }
        public DateTime? RBSDate { get; set; }
    }

    public class AddExportDocsReceiptDocumentVm
    {
        [Required]
        public Guid ExportDocsReceiptId { get; set; }

        [Required]
        public Guid DocumentId { get; set; }

        public List<SelectListItem> Documents { get; set; } = new();
    }

    public class AddExportDocsReceiptContainerVm
    {
        public Guid ExportDocsReceiptId { get; set; }
        public Guid? ContainerId { get; set; }
        public Guid? ContainerUseId { get; set; }
        public DateTime? RBSDate { get; set; }

        public List<SelectListItem> Containers { get; set; } = new();
        public List<SelectListItem> ContainerUses { get; set; } = new();
    }

}
