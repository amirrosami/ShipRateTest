using ShipRate.Application.Utilities.Attributes;
using System.ComponentModel.DataAnnotations;

namespace ShipRate.Application.ViewModels
{
    public class RtsDeliverySummaryVm
    {
        public Guid RTSInstructionId { get; set; }
        public Guid BookingId { get; set; }
        public string RTSInstructionNo { get; set; }
        public string ShipperName { get; set; }
        public string DepotName { get; set; }
        public DateTime? ValidDateTo { get; set; }
        public bool IsExpired => ValidDateTo.HasValue && ValidDateTo.Value.Date < DateTime.Today;

        public List<RtsDeliveryItemStatusVm> AllowedContainers { get; set; } = new();
        public List<DeliveredContainerVm> AlreadyDelivered { get; set; } = new();
    }

    public class RtsDeliveryItemStatusVm
    {
        public int ContainerSizeTypeId { get; set; }
        public string ContainerSizeTypeName { get; set; }
        public int AllowedCount { get; set; }
        public int DeliveredCount { get; set; }
        public int RemainingCount => Math.Max(0, AllowedCount - DeliveredCount);
    }

    public class DeliveredContainerVm
    {
        public string ContainerNumber { get; set; }
        public string SealNumber { get; set; }
        public string ContainerSizeTypeName { get; set; }
        public DateTime ActionDateTime { get; set; }
    }

    public class IssueContainerCommand
    {
        [Required]
        public Guid RTSInstructionId { get; set; }

        public Guid? BookingId { get; set; }

        [Required]
        public int ContainerSizeTypeId { get; set; }

        [Required(ErrorMessage = "Container number is required.")]
        [StringLength(11, MinimumLength = 11, ErrorMessage = "Container number must be exactly 11 characters.")]
        [RegularExpression(@"^[A-Za-z]{4}[0-9]{7}$", ErrorMessage = "Container number format must be like ABCD1234567.")]
        [Iso6346Container]
        public string ContainerNumber { get; set; }

        [Required(ErrorMessage = "Seal number is required.")]
        [StringLength(50)]
        public string SealNumber { get; set; }

        [Range(0, 999999, ErrorMessage = "Tare weight must be a positive number.")]
        public decimal? TareWeight { get; set; }

        [Range(0, 999999, ErrorMessage = "Gross weight must be a positive number.")]
        public decimal? GrossWeight { get; set; }

        [StringLength(100)]
        public string DriverName { get; set; }

        public DateTime? ActionDateTime { get; set; }

        [StringLength(50)]
        public string DriverPlate { get; set; }


        public bool IsNewContainer { get; set; }
        public Guid? NewContainerSizeTypeId { get; set; }
        public decimal? NewContainerMaxPayload { get; set; }
        public int? NewContainerTareWeight { get; set; }
    }
}
