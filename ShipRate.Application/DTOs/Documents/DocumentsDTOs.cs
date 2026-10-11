namespace ShipRate.Application.DTOs.Documents
{
    public class DocumentSearchDto
    {
        public int? CompanyId { get; set; }
        public int? DocumentTypeId { get; set; }
        public int? ObjectTypeId { get; set; }

        public string? DocumentTitle { get; set; }
        public string? DocumentNo { get; set; }

        public DateTime? DocumentDateFrom { get; set; }
        public DateTime? DocumentDateTo { get; set; }

        public DateTime? ExpiryDateFrom { get; set; }
        public DateTime? ExpiryDateTo { get; set; }

        public bool? IsActive { get; set; }

        public int PageNumber { get; set; } = 1;
        public int PageSize { get; set; } = 10;
    }

    public class DocumentListItemDto
    {
        public Guid Id { get; set; }
        public int CompanyId { get; set; }
        public string CompanyName { get; set; } = string.Empty;

        public int DocumentTypeId { get; set; }
        public string DocumentTypeName { get; set; } = string.Empty;

        public int ObjectTypeId { get; set; }
        public string ObjectTypeName { get; set; } = string.Empty;

        public string DocumentTitle { get; set; } = string.Empty;
        public string DocumentNo { get; set; } = string.Empty;
        public DateTime DocumentDate { get; set; }
        public string DocumentDescription { get; set; } = string.Empty;
        public Guid ObjectId { get; set; }
        public string ObjectDisplayText { get; set; } = string.Empty;
        public DateTime? ExpiryDate { get; set; }
        public bool IsActive { get; set; }
        public bool IsLast { get; set; }
    }

    public class DocumentAttachmentDto
    {
        public Guid Id { get; set; }
        public Guid DocumentId { get; set; }
        public string DocumentAttachmentFileName { get; set; } = string.Empty;
        public string DocumentAttachmentFileType { get; set; } = string.Empty;
        public string DocumentAttachmentFileExtension { get; set; } = string.Empty;
        public decimal DocumentAttachmentFileSize { get; set; }
        public string DocumentAttachmentFilePath { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; }
        public bool IsActive { get; set; }
    }

    public class DocumentEmailDto
    {
        public Guid Id { get; set; }
        public Guid DocumentId { get; set; }
        public string DocumentEmailSubject { get; set; } = string.Empty;
        public string DocumentEmailBody { get; set; } = string.Empty;
        public Guid DocumentEmailFromUserId { get; set; }
        public string DocumentEmailFrom { get; set; } = string.Empty;
        public string DocumentEmailTo { get; set; } = string.Empty;
        public string DocumentEmailCC { get; set; } = string.Empty;
        public string DocumentEmailBCC { get; set; } = string.Empty;
        public DateTime DocumentEmailSendDate { get; set; }
        public int EmailDirectionId { get; set; }
        public string EmailDirectionName { get; set; } = string.Empty;
        public bool IsActive { get; set; }
    }
    public class DocumentDetailsSectionDto
    {
        public Guid DocumentId { get; set; }
        public string DocumentTitle { get; set; } = string.Empty;
        public string DocumentNo { get; set; } = string.Empty;
        public DateTime DocumentDate { get; set; }
        public string DocumentDescription { get; set; } = string.Empty;
        public string CompanyName { get; set; } = string.Empty;
        public string DocumentTypeName { get; set; } = string.Empty;
        public string ObjectTypeName { get; set; } = string.Empty;
        public string ObjectDisplayText { get; set; } = string.Empty;
        public DateTime? ExpiryDate { get; set; }
        public bool IsActive { get; set; }
        public bool IsLast { get; set; }

        public List<DocumentAttachmentDto> Attachments { get; set; } = new();
        public List<DocumentEmailDto> Emails { get; set; } = new();
    }

}
