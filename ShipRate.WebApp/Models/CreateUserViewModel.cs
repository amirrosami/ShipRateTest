using Microsoft.AspNetCore.Mvc.Rendering;
using ShipRate.Domain.Entities.Security;
using System.ComponentModel.DataAnnotations;

namespace ShipRate.Web.Models
{
    public class UserFormViewModel
    {

        public Guid Id { get; set; }

        [Required(ErrorMessage = "User Name is Required")]
        [Display(Name = "User Name")]
        public string UserName { get; set; } = string.Empty;

        [Required(ErrorMessage = "Email Address is Required")]
        [EmailAddress]
        [Display(Name = "Email Address")]
        public string Email { get; set; } = string.Empty;

        [Required(ErrorMessage = "Name is Required")]
        [Display(Name = "Full Name")]
        public string FullName { get; set; } = string.Empty;

        //[Required(ErrorMessage = "Password is Required")]
        [StringLength(100, MinimumLength = 6)]
        [DataType(DataType.Password)]
        [Display(Name = "Password")]
        public string Password { get; set; } = string.Empty;

        [DataType(DataType.Password)]
        [Display(Name = "Confirm Password")]
        [Compare("Password", ErrorMessage = "Passwords are not equal")]
        public string ConfirmPassword { get; set; } = string.Empty;

        public int UserTypeId { get; set; }
        public int? TwoFactorMethodId { get; set; }

        public bool IsEditMode { get; set; }
        public bool TwoFactorEnabled { get; set; }

        public Guid PersonId { get; set; }
        public string? PersonName { get; set; }

        [Display(Name = "Role")]
        public string? RoleName { get; set; }

        [Display(Name = "Job Title")]
        public string? JobTitle { get; set; }

        [Display(Name = "Phone Number")]
        [Required(ErrorMessage = "Phone Number is Required")]
        public string? PhoneNumber { get; set; }

        [Display(Name = "Mobile No")]
        [Required(ErrorMessage = "Mobile No is Required")]
        public string? Mobile { get; set; }

        [Display(Name = "Sales File Prefix")]
        public string? SalesFilePrefix { get; set; } = string.Empty;

        public List<SelectListItem>? UserTypes { get; set; }
        public List<SelectListItem>? TwoFactorMethods { get; set; }

        public List<int> SelectedCompanyIds { get; set; } = new();
        public List<CompanyDropdownItem> Companies { get; set; } = new();


        // ===== عکس =====
        [Display(Name = "User Picture")]
        public IFormFile? PicFile { get; set; }

        public string? ExistingPicBase64 { get; set; }

        public bool RemovePic { get; set; } = false;


        public List<string> SelectedRoles { get; set; }
        public List<ApplicationRole> Roles { get; set; } = new();



    }

    public class CompanyDropdownItem
    {
        public int Id { get; set; }
        public string Name { get; set; } = "";
    }



}
