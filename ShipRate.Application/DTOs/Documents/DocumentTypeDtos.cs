using System.ComponentModel.DataAnnotations;

namespace ShipRate.Application.DTOs.Documents
{
    public class DocumentTypeInputDto
    {
        public int? Id { get; set; }

        [Required(ErrorMessage = "Document Type Category is required.")]
        public int DocumentTypeCategoryId { get; set; }

        [Required(ErrorMessage = "Document Type Name is required.")]
        [StringLength(100, ErrorMessage = "Name cannot exceed 100 characters.")]
        public string DocumentTypeName { get; set; } = string.Empty;


        [StringLength(100, ErrorMessage = "Local Name cannot exceed 100 characters.")]
        public string? DocumentTypeLocalName { get; set; }

        public bool SystemDefined { get; set; }
        public bool HasExpiryDate { get; set; }

        [Range(0, 365, ErrorMessage = "Alarm days must be between 0 and 365.")]
        public int AlarmDaysBeforeExpire { get; set; }
    }

    // DTO برای نمایش در جدول لیست
    public class DocumentTypeListDto
    {
        public int Id { get; set; }
        public string DocumentTypeName { get; set; } = string.Empty;
        public string? DocumentTypeLocalName { get; set; }
        public string CategoryName { get; set; } = string.Empty;
        public bool SystemDefined { get; set; }
        public bool HasExpiryDate { get; set; }
        public int AlarmDaysBeforeExpire { get; set; }

        public DateTime? CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
        public string? CreateUserName { get; set; }
        public string? UpdateUserName { get; set; }
    }




    public class SystemObjectTypeDto
    {
        public int Id { get; set; }
        public string SystemObjectTypeName { get; set; } = string.Empty;
        public string SystemObjectTypeLocalName { get; set; } = string.Empty;
    }

    public class DocumentTypeObjectTypeDto
    {
        public int Id { get; set; }
        public int DocumentTypeId { get; set; }
        public string DocumentTypeName { get; set; } = string.Empty;
        public string DocumentTypeLocalName { get; set; } = string.Empty;
        public int ObjectTypeId { get; set; }
        public string ObjectTypeName { get; set; } = string.Empty;
        public bool SystemDefined { get; set; }
    }

    public class DocumentTypeObjectTypeInputDto
    {
        public int? Id { get; set; }

        [Required(ErrorMessage = "System Object is required.")]
        public int ObjectTypeId { get; set; }

        [Required(ErrorMessage = "Document Type is required.")]
        public int DocumentTypeId { get; set; }

        public bool SystemDefined { get; set; }
    }




}
