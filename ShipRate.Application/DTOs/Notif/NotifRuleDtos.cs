using Microsoft.AspNetCore.Mvc.Rendering;
using System.ComponentModel.DataAnnotations;

namespace ShipRate.Application.DTOs.Notif
{
    public class NotificationRuleGetListInput
    {
        public int PageNumber { get; set; } = 1;
        public int PageSize { get; set; } = 10;

        public string? ActivityType { get; set; }
        public int? EntityTypeId { get; set; }
        public int? RecipientModeId { get; set; }
        public bool? IsActive { get; set; }
    }


    public class NotificationRuleIndexDto
    {
        public Guid Id { get; set; }
        public string ActivityType { get; set; } = null!;
        public string EntityTypeName { get; set; } = null!;
        public string RecipientModeName { get; set; } = null!;
        public bool ShouldNotify { get; set; }
        public bool IsActive { get; set; }
        public DateTime CreatedAt { get; set; }
    }
    public class NotificationRuleCreateDto
    {
        [Required]
        [MaxLength(50)]
        public string ActivityType { get; set; } = null!;

        [Required]
        public int EntityTypeId { get; set; }

        public bool ShouldNotify { get; set; } = true;

        [Required]
        public int RecipientModeId { get; set; }

        [Required]
        [MaxLength(500)]
        public string TitleTemplate { get; set; } = null!;

        [Required]
        [MaxLength(500)]
        public string MessageTemplate { get; set; } = null!;

        [MaxLength(500)]
        public string? TargetUrlTemplate { get; set; }

        public bool IsActive { get; set; } = true;

        public List<Guid> SelectedUserIds { get; set; } = new();
        public List<int> SelectedUserTypeIds { get; set; } = new();


        public int SpecificUsersModeId { get; set; }
        public int UserTypesModeId { get; set; }



        public List<SelectListItem> EntityTypes { get; set; } = new();
        public List<SelectListItem> RecipientModes { get; set; } = new();
        public List<SelectListItem> Users { get; set; } = new();
        public List<SelectListItem> UserTypes { get; set; } = new();
    }
    public class NotificationRuleEditDto
    {
        [Required]
        public Guid Id { get; set; }

        [Required]
        [MaxLength(50)]
        public string ActivityType { get; set; } = null!;

        [Required]
        public int EntityTypeId { get; set; }

        public bool ShouldNotify { get; set; }

        [Required]
        public int RecipientModeId { get; set; }

        [Required]
        [MaxLength(500)]
        public string TitleTemplate { get; set; } = null!;

        [Required]
        [MaxLength(500)]
        public string MessageTemplate { get; set; } = null!;

        [MaxLength(500)]
        public string? TargetUrlTemplate { get; set; }

        public bool IsActive { get; set; }

        public int SpecificUsersModeId { get; set; }
        public int UserTypesModeId { get; set; }


        public List<Guid> SelectedUserIds { get; set; } = new();
        public List<int> SelectedUserTypeIds { get; set; } = new();

        public List<SelectListItem> EntityTypes { get; set; } = new();
        public List<SelectListItem> RecipientModes { get; set; } = new();
        public List<SelectListItem> Users { get; set; } = new();
        public List<SelectListItem> UserTypes { get; set; } = new();
    }

    public class NotificationRuleDetailsDto
    {
        public Guid Id { get; set; }
        public string ActivityType { get; set; } = null!;
        public int EntityTypeId { get; set; }
        public string EntityTypeName { get; set; } = null!;
        public bool ShouldNotify { get; set; }
        public int RecipientModeId { get; set; }
        public string RecipientModeName { get; set; } = null!;
        public string TitleTemplate { get; set; } = null!;
        public string MessageTemplate { get; set; } = null!;
        public string? TargetUrlTemplate { get; set; }
        public bool IsActive { get; set; }

        public List<string> Users { get; set; } = new();
        public List<string> UserTypes { get; set; } = new();

        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
    }

    public class NotificationRuleDeleteDto
    {
        public Guid Id { get; set; }
        public string ActivityType { get; set; } = null!;
        public string EntityTypeName { get; set; } = null!;
        public string RecipientModeName { get; set; } = null!;
    }
}
