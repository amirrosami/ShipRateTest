using System.ComponentModel.DataAnnotations;

namespace ShipRate.Application.DTOs
{
    public class CorrespondenceDto
    {
        public Guid Id { get; set; }
        public int CompanyId { get; set; }
        public string? CompanyName { get; set; }
        public int InOutId { get; set; }
        public string? InOutName { get; set; }
        public Guid? SenderReceiverId { get; set; }
        public string? SenderReceiverName { get; set; }
        public string? SenderReceiverTitle { get; set; }
        public string CorrespondenceNo { get; set; } = string.Empty;
        public DateTime CorrespondenceDate { get; set; }
        public int? SubjectId { get; set; }
        public string? SubjectName { get; set; }
        public string SubjectFreeText { get; set; } = string.Empty;
        public string Keywords { get; set; } = string.Empty;
        public bool IsActive { get; set; }
    }

    public class CorrespondenceFilterDto
    {
        public int? SubjectId { get; set; }
        public string? CorrespondenceNo { get; set; }
        public DateTime? DateFrom { get; set; }
        public DateTime? DateTo { get; set; }
        public string? Keywords { get; set; }
    }

    public class CorrespondenceCreateDto
    {
        [Required]
        public int CompanyId { get; set; }
        [Required]
        public int InOutId { get; set; }

        public Guid? SenderReceiverId { get; set; }

        [MaxLength(150)]
        public string? SenderReceiverTitle { get; set; }

        [Required]
        [MaxLength(35)]
        public string CorrespondenceNo { get; set; } = string.Empty;

        [Required]
        public DateTime CorrespondenceDate { get; set; } = DateTime.Now;

        public int? SubjectId { get; set; }

        [Required]
        [MaxLength(150)]
        public string SubjectFreeText { get; set; } = string.Empty;

        public int? SystemObjectTypeId { get; set; }
        public Guid? ObjectId { get; set; }

        [MaxLength(200)]
        public string Keywords { get; set; } = string.Empty;
    }
}
