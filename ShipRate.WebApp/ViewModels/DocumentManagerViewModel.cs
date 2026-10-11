namespace ShipRate.WebApp.ViewModels
{
    public class DocumentManagerViewModel
    {
        public Guid ObjectId { get; set; }
        public int ObjectTypeId { get; set; }
        public List<DocumentListItem> Documents { get; set; } = new();
        public bool ShowSubset { get; set; }
    }

    public class DocumentListItem
    {
        public Guid Id { get; set; }
        public string DocumentType { get; set; } = "";
        public string DocumentTitle { get; set; } = "";
        public string DocumentNo { get; set; } = "";
        public DateTime DocumentDate { get; set; }
        public string? Description { get; set; }
        public int AttachmentCount { get; set; }
        public int EmailCount { get; set; }
        public bool IsLast { get; set; }
        public string? CreateUserName { get; set; }
        public string? LastEditUserName { get; set; }
        public DateTime? CreateDateTime { get; set; }
        public DateTime? LastEditDateTime { get; set; }
        public string? ObjectTypeName { get; set; }

    }


    public class DocumentFormViewModel
    {
        public Guid? Id { get; set; }
        public Guid ObjectId { get; set; }
        public int ObjectTypeId { get; set; }
        public int DocumentTypeId { get; set; }
        public string DocumentTitle { get; set; } = "";
        public string DocumentNo { get; set; } = "";
        public bool IsLast { get; set; }
        public DateTime DocumentDate { get; set; } = DateTime.Now;
        public string? DocumentDescription { get; set; }
        public List<LookupItem> DocumentTypes { get; set; } = new();
    }

    public class AttachmentListItem
    {
        public Guid Id { get; set; }
        public string FileName { get; set; } = "";
        public string FileType { get; set; } = "";
        public string FileExtension { get; set; } = "";
        public decimal FileSize { get; set; }
        public DateTime CreatedAt { get; set; }
    }

    public class EmailListItem
    {
        public Guid Id { get; set; }
        public string Subject { get; set; } = "";
        public string From { get; set; } = "";
        public string To { get; set; } = "";
        public DateTime SendDate { get; set; }
        public string Direction { get; set; } = "";
    }

    public class EmailFormViewModel
    {
        public Guid? Id { get; set; }
        public Guid DocumentId { get; set; }
        public string Subject { get; set; } = "";
        public string Body { get; set; } = "";
        public string To { get; set; } = "";
        public string CC { get; set; } = "";
        public string BCC { get; set; } = "";
        public int EmailDirectionId { get; set; }
        public List<LookupItem> EmailDirections { get; set; } = new();
    }
    public class LookupItem
    {
        public int Id { get; set; }
        public string Title { get; set; } = "";
    }
}
