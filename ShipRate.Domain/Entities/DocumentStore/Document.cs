using ShipRate.Domain.Entities.Base;
using ShipRate.Domain.Entities.BaseInfo.Generals;
using ShipRate.Domain.Entities.Security;
using ShipRate.Domain.Entities.Settings;

namespace ShipRate.Domain.Entities.DocumentStore
{
    public class Document : BusinessEntity
    {
        public int CompanyId { get; set; }
        public int DocumentTypeId { get; set; }
        public string DocumentTitle { get; set; } = "";
        public string DocumentNo { get; set; } = "";
        public DateTime DocumentDate { get; set; }
        public string DocumentDescription { get; set; } = "";
        public int ObjectTypeId { get; set; }
        public Guid ObjectId { get; set; }
        public DateTime? ExpiryDate { get; set; }
        public bool IsLast { get; set; }

        //Relations 
        public SystemObjectType? ObjectType { get; set; }
        public DocumentType? DocumentType { get; set; }
        public Company Company { get; set; }
        public ApplicationUser CreateUser { get; set; }
        public ApplicationUser EditUser { get; set; }
        public ICollection<DocumentAttachment> Attachments { get; set; } = new List<DocumentAttachment>();
        public ICollection<DocumentEmail> Emails { get; set; } = new List<DocumentEmail>();
    }

    public class DocumentAttachment : BusinessEntity
    {
        public Guid DocumentId { get; set; }
        public string DocumentAttachmentFileName { get; set; } = string.Empty;
        public string DocumentAttachmentFileType { get; set; } = string.Empty;
        public string DocumentAttachmentFileExtension { get; set; } = string.Empty;
        public decimal DocumentAttachmentFileSize { get; set; }
        public byte[]? DocumentAttachmentFileData { get; set; }
        public string DocumentAttachmentFilePath { get; set; } = string.Empty;
        //Relations
        public Document? Document { get; set; }
    }

    public class DocumentEmail : BusinessEntity
    {
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

        //Relation
        public Document Document { get; set; }
        public EmailDirection EmailDirection { get; set; }

    }
    public class DocumentType : SystemEntity
    {
        public int DocumentTypeCategoryId { get; set; }
        public string DocumentTypeName { get; set; } = string.Empty;
        public string? DocumentTypeLocalName { get; set; }
        public bool SystemDefined { get; set; }
        public bool HasExpiryDate { get; set; }
        public int AlarmDaysBeforeExpire { get; set; }

        public DateTime? CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
        public DateTime? DeletedAt { get; set; }

        public Guid? CreateUserId { get; set; }
        public Guid? UpdateUserId { get; set; }
        public Guid? DeleteUserId { get; set; }

        //Relations
        public DocumentTypeCategory DocumentTypeCategory { get; set; }
        public ApplicationUser CreateUser { get; set; }
        public ApplicationUser UpdateUser { get; set; }
        public ApplicationUser DeleteUser { get; set; }

    }
    public class DocumentTypeCategory : SystemEntity
    {
        public string DocumentTypeCategoryName { get; set; } = string.Empty;
        public bool SystemDefined { get; set; }
    }
    public class DocumentTypeObjectType : SystemEntity
    {
        public int? DocumentTypeId { get; set; }
        public int? ObjectTypeId { get; set; }
        public bool SystemDefined { get; set; }
        //Relations
        public DocumentType DocumentType { get; set; }
        public SystemObjectType ObjectType { get; set; }
    }
}