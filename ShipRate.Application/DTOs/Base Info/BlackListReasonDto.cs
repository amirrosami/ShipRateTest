using System.ComponentModel.DataAnnotations;

namespace ShipRate.Application.DTOs.Base_Info
{
    public class BlackListReasonDto
    {
        public int Id { get; set; }
        public string BlackListReasonName { get; set; } = string.Empty;
        public bool SystemDefined { get; set; } = true;
        public bool IsActive { get; set; } = true;
        public bool IsDeleted { get; set; } = true;

    }
}
public class BlackListReasonCreateDto
{
    public int Id { get; set; }
    [Required(ErrorMessage = "Balck List reason Name is required.")]
    [StringLength(200, ErrorMessage = "Name cannot exceed 100 characters.")]
    public string BlackListReasonName { get; set; } = string.Empty;
    public bool SystemDefined { get; set; } = false;
    public bool IsActive { get; set; } = true;
}

public class BlackListReasonUpdateDto
{
    public int Id { get; set; }

    [Required(ErrorMessage = "Balck List reason Name is required.")]
    [StringLength(200, ErrorMessage = "Name cannot exceed 100 characters.")]
    public string BlackListReasonName { get; set; } = string.Empty;

    public bool SystemDefined { get; set; } = false;
    public bool IsActive { get; set; } = true;
    public int? pageNumber { get; set; }
}