using Microsoft.AspNetCore.Mvc.Rendering;
using System.ComponentModel.DataAnnotations;

namespace ShipRate.Application.Base
{
    public class LocationCreateEditDto
    {
        public Guid Id { get; set; }

        [Display(Name = "Created Date")]
        public DateTime CreatedDate { get; set; }

        [Display(Name = "Modified Date")]
        public DateTime? ModifiedDate { get; set; }
        [Required(ErrorMessage = "Location Code is required.")]
        [StringLength(20, ErrorMessage = "Location Code cannot exceed 20 characters.")]
        [Display(Name = "Location Code")]
        public string LocationCode { get; set; } = string.Empty;

        [Required(ErrorMessage = "Location Name is required.")]
        [StringLength(200, ErrorMessage = "Location Name cannot exceed 200 characters.")]
        [Display(Name = "Location Name")]
        public string LocationName { get; set; } = string.Empty;

        [StringLength(200)]
        [Display(Name = "Location Local Name")]
        public string? LocationLocalName { get; set; }

        [Required(ErrorMessage = "Location Type is required.")]
        [Display(Name = "Location Type")]
        public int LocationTypeId { get; set; }

        [Display(Name = "Country")]
        public int? CountryId { get; set; }

        [Display(Name = "Second Country (for Border)")]
        public int? CountryId2 { get; set; }

        [Display(Name = "City Type")]
        public int? CityTypeId { get; set; }

        [Display(Name = "Parent Location")]
        public Guid? ParentLocationId { get; set; }
        public string? ParentLocationName { get; set; }

        [Display(Name = "Depot Type")]
        public int? DepotTypeId { get; set; }

        [Display(Name = "Terminal Type")]
        public int? TerminalTypeId { get; set; }

        [StringLength(10)]
        [Display(Name = "IATA Code")]
        public string? IATACode { get; set; }

        [StringLength(10)]
        [Display(Name = "ICAO Code")]
        public string? ICAOCode { get; set; }

        [Display(Name = "Latitude")]
        public float? Latitude { get; set; }

        [Display(Name = "Longitude")]
        public float? Longitude { get; set; }


        [StringLength(100)]
        [Display(Name = "Address")]
        public string? Address { get; set; }

        [StringLength(50)]
        [Display(Name = "Phone")]
        public string? Phone { get; set; }

        [StringLength(35)]
        [Display(Name = "Email")]
        public string? Email { get; set; }

        // Dropdowns
        public List<SelectListItem> LocationTypes { get; set; } = new();
        public List<SelectListItem> Countries { get; set; } = new();
        public List<SelectListItem> CityTypes { get; set; } = new();
        public List<SelectListItem> DepotTypes { get; set; } = new();
        public List<SelectListItem> TerminalTypes { get; set; } = new();
        public List<SelectListItem> ParentLocations { get; set; } = new();

    }

}
