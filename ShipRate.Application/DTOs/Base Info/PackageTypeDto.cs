using System.ComponentModel.DataAnnotations;

namespace ShipRate.Application.DTOs.Base_Info
{
    public class PackageTypeDto
    {
        public int Id { get; set; }
        public int Sequence { get; set; }
        public string PackageTypeCode { get; set; } = string.Empty;
        public string PackageTypeName { get; set; } = string.Empty;
        public string PackageTypeDescription { get; set; } = string.Empty;
        public string NumericCode { get; set; } = string.Empty;
        public string ENSCode { get; set; } = string.Empty;
        public bool IsActive { get; set; }
        public bool IsDeleted { get; set; }
    }
}
public class PackageTypeCreateDto
{
    public int Id { get; set; }
    [Required(ErrorMessage = "Sequence is required.")]
    [Range(1, int.MaxValue, ErrorMessage = "Sequence must be greater than 0.")]
    public int Sequence { get; set; }

    [Required(ErrorMessage = "Package Type Code is required.")]
    [StringLength(5, ErrorMessage = "Code cannot exceed 5 characters.")]
    public string PackageTypeCode { get; set; } = string.Empty;

    [Required(ErrorMessage = "Package Type Name is required.")]
    [StringLength(200, ErrorMessage = "Name cannot exceed 200 characters.")]
    public string PackageTypeName { get; set; } = string.Empty;

    [Required(ErrorMessage = "Description is required.")]
    [StringLength(250, ErrorMessage = "Description cannot exceed 250 characters.")]
    public string PackageTypeDescription { get; set; } = string.Empty;

    [Required(ErrorMessage = "Numeric Code is required.")]
    [StringLength(100, ErrorMessage = "Numeric Code cannot exceed 100 characters.")]
    public string NumericCode { get; set; } = string.Empty;

    [Required(ErrorMessage = "ENS Code is required.")]
    [StringLength(5, ErrorMessage = "ENS Code cannot exceed 5 characters.")]
    public string ENSCode { get; set; } = string.Empty;

    public bool IsActive { get; set; } 
}

public class PackageTypeUpdateDto
{
    public int Id { get; set; }

    [Required(ErrorMessage = "Sequence is required.")]
    [Range(1, int.MaxValue, ErrorMessage = "Sequence must be greater than 0.")]
    public int Sequence { get; set; }

    [Required(ErrorMessage = "Package Type Code is required.")]
    [StringLength(5, ErrorMessage = "Code cannot exceed 5 characters.")]
    public string PackageTypeCode { get; set; } = string.Empty;

    [Required(ErrorMessage = "Package Type Name is required.")]
    [StringLength(200, ErrorMessage = "Name cannot exceed 200 characters.")]
    public string PackageTypeName { get; set; } = string.Empty;

    [Required(ErrorMessage = "Description is required.")]
    [StringLength(250, ErrorMessage = "Description cannot exceed 250 characters.")]
    public string PackageTypeDescription { get; set; } = string.Empty;

    [Required(ErrorMessage = "Numeric Code is required.")]
    [StringLength(100, ErrorMessage = "Numeric Code cannot exceed 100 characters.")]
    public string NumericCode { get; set; } = string.Empty;

    [Required(ErrorMessage = "ENS Code is required.")]
    [StringLength(5, ErrorMessage = "ENS Code cannot exceed 5 characters.")]
    public string ENSCode { get; set; } = string.Empty;

    public bool IsActive { get; set; }
    public int? pageNumber { get; set; }
}