using System.ComponentModel.DataAnnotations;

namespace ShipRate.Application.DTOs.ContainerActions
{
    public class RegisterContainerActionRequest
    {
        /// <summary>
        /// شناسه یکتای درخواست برای کنترل Idempotency.
        /// در صورت ارسال، به عنوان Reference ذخیره می‌شود یا می‌تواند در بررسی تکراری بودن استفاده شود.
        /// </summary>
        public Guid? RequestId { get; set; }

        [Required]
        public int CompanyId { get; set; }

        [Required]
        public Guid ContainerId { get; set; }

        public Guid? ContainerUseId { get; set; }

        [Required]
        [StringLength(12)]
        public string ContainerNumber { get; set; } = string.Empty;

        [Required]
        public int CntActionTypeId { get; set; }

        public Guid? LocationId { get; set; }

        public Guid? SubLocationId { get; set; }

        public Guid? VoyageId { get; set; }

        public int? VoyageTypeId { get; set; }

        public Guid? LoadListId { get; set; }

        public Guid? ContainerVoyageAllocationId { get; set; }

        [Required]
        public int CntOwnTypeId { get; set; }

        [Required]
        public int FullMTId { get; set; }

        [Required]
        public DateTime ContainerActionDateTime { get; set; }

        [Required]
        public int EventActionSourceId { get; set; }

        [StringLength(30)]
        public string? ContainerActionSourceReference { get; set; }

        /// <summary>
        /// Attributeهای جانبی مربوط به Action.
        /// مثل SealNo, DriverName, TruckNo, EdiMessageId و ...
        /// </summary>
        public Dictionary<string, string>? Attributes { get; set; } = new();
    }
}
