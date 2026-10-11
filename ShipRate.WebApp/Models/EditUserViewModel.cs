using System.ComponentModel.DataAnnotations;

namespace ShipRate.Web.Models
{
    public class EditUserViewModel
    {
        public Guid Id { get; set; }

        [Required]
        [Display(Name = "User Name")]
        public string UserName { get; set; } = string.Empty;

        [Required]
        [EmailAddress]
        [Display(Name = "Email Address")]
        public string Email { get; set; } = string.Empty;

        [Required]
        [Display(Name = "Full Name")]
        public string FullName { get; set; } = string.Empty;

        [Display(Name = "Active")]
        public bool IsActive { get; set; }

        public List<string> CurrentRoles { get; set; } = new();
        public List<string> AllRoles { get; set; } = new();
        public List<string>? SelectedRoles { get; set; }
    }
}
