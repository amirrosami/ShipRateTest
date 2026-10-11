using Microsoft.AspNetCore.Mvc.Rendering;
using System.ComponentModel.DataAnnotations;

namespace ShipRate.Application.ViewModels
{
    public class JobPartyFilterVm
    {
        public Guid JobId { get; set; }
    }


    public class JobPartyListItemVm
    {
        public Guid Id { get; set; }
        public int SequenceNo { get; set; }

        public Guid PartyId { get; set; }
        public string PartyName { get; set; } = string.Empty;

        public int RoleTypeId { get; set; }
        public string RoleTypeName { get; set; } = string.Empty;

        public bool IsPrimaryRole { get; set; }

        public string? ContactName { get; set; }
        public string? ContactEmail { get; set; }
        public string? ContactPhone { get; set; }

        public bool IsActive { get; set; }
    }
    public class JobPartyFormVm
    {
        public Guid? Id { get; set; }

        [Required]
        public Guid JobId { get; set; }

        [Display(Name = "Sequence No")]
        [Required]
        public int SequenceNo { get; set; }

        [Display(Name = "Party")]
        [Required]
        public Guid? PartyId { get; set; }

        public string? PartyName { get; set; }

        [Display(Name = "Role Type")]
        [Required]
        public int? RoleTypeId { get; set; }

        [Display(Name = "Is Primary Role")]
        public bool IsPrimaryRole { get; set; }

        [Display(Name = "Contact Name")]
        [MaxLength(50)]
        public string? ContactName { get; set; }

        [Display(Name = "Contact Email")]
        [EmailAddress]
        [MaxLength(35)]
        public string? ContactEmail { get; set; }

        [Display(Name = "Contact Phone")]
        [MaxLength(35)]
        public string? ContactPhone { get; set; }

        [Display(Name = "Is Active")]
        public bool IsActive { get; set; } = true;

        public List<SelectListItem> Parties { get; set; } = new();
        public List<SelectListItem> RoleTypes { get; set; } = new();
    }

    public class JobPartyDetailsVm
    {
        public Guid Id { get; set; }
        public Guid JobId { get; set; }
        public int SequenceNo { get; set; }

        public Guid PartyId { get; set; }
        public string PartyName { get; set; } = string.Empty;

        public int RoleTypeId { get; set; }
        public string RoleTypeName { get; set; } = string.Empty;

        public bool IsPrimaryRole { get; set; }

        public string? ContactName { get; set; }
        public string? ContactEmail { get; set; }
        public string? ContactPhone { get; set; }

        public bool IsActive { get; set; }

        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
    }

    public class JobPartyDeleteVm
    {
        public Guid Id { get; set; }
        public Guid JobId { get; set; }
        public string PartyName { get; set; } = string.Empty;
        public string RoleTypeName { get; set; } = string.Empty;
        public int SequenceNo { get; set; }
    }

}
