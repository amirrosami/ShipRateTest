using System.ComponentModel.DataAnnotations;

namespace ShipRate.Application.DTOs.Base_Info
{
    public class IndustryDto
    {
        public Guid Id { get; set; }
        public string IndustryName { get; set; } = string.Empty;
        public bool IsActive { get; set; } = true;
        public bool IsDeleted { get; set; } = true;

    }
}
public class IndustryCreateDto
{

    [Required(ErrorMessage = "Industry Name is required.")]
    [StringLength(200, ErrorMessage = "Name cannot exceed 100 characters.")]
    public string IndustryName { get; set; } = string.Empty;

    public bool IsActive { get; set; } = true;
}

public class IndustryUpdateDto
{
    public Guid Id { get; set; }

    [Required(ErrorMessage = "Industry Name is required.")]
    [StringLength(200, ErrorMessage = "Name cannot exceed 100 characters.")]
    public string IndustryName { get; set; } = string.Empty;

    public bool IsActive { get; set; } = true;
    public int? pageNumber { get; set; }
}