using Microsoft.AspNetCore.Mvc.Rendering;
using System.ComponentModel.DataAnnotations;
namespace ShipRate.WebApp.ViewModels.Location
{
    // ── Index ──────────────────────────────────────────────
    public class LocationIndexViewModel
    {
        public List<LocationListItemViewModel> Locations { get; set; } = new();

        // Filters
        public string? SearchTerm { get; set; }
        public int? FilterLocationTypeId { get; set; }
        public int? FilterCountryId { get; set; }
        public bool? FilterIsActive { get; set; }

        // Sorting
        public string SortColumn { get; set; } = "LocationCode";
        public string SortDirection { get; set; } = "asc";

        // Paging
        public int PageNumber { get; set; } = 1;
        public int PageSize { get; set; } = 10;
        public int TotalCount { get; set; }
        public int TotalPages => (int)Math.Ceiling((double)TotalCount / PageSize);

        // Dropdowns for filters
        public List<DropdownItem> LocationTypes { get; set; } = new();
        public List<DropdownItem> Countries { get; set; } = new();
    }

    public class LocationListItemViewModel
    {
        public Guid LocationId { get; set; }
        public string LocationCode { get; set; } = string.Empty;
        public string LocationName { get; set; } = string.Empty;
        public string LocationLocalName { get; set; } = string.Empty;
        public string? LocationTypeName { get; set; }
        public string? CountryName { get; set; }
        public string? CityTypeName { get; set; }
        public string? ParentLocationName { get; set; }
        public bool IsActive { get; set; }
        public DateTime CreatedDate { get; set; }
    }

    // ── Create ─────────────────────────────────────────────
    public class LocationCreateViewModel
    {
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
        public int? ParentLocationId { get; set; }

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
        public List<DropdownItem> LocationTypes { get; set; } = new();
        public List<DropdownItem> Countries { get; set; } = new();
        public List<DropdownItem> CityTypes { get; set; } = new();
        public List<DropdownItem> DepotTypes { get; set; } = new();
        public List<DropdownItem> TerminalTypes { get; set; } = new();
        public List<DropdownItem> ParentLocations { get; set; } = new();
    }

    // ── Edit ───────────────────────────────────────────────
    public class LocationEditViewModel : LocationCreateViewModel
    {
        public Guid LocationId { get; set; }

        [Display(Name = "Created Date")]
        public DateTime CreatedDate { get; set; }

        [Display(Name = "Modified Date")]
        public DateTime? ModifiedDate { get; set; }
    }


    public class LocationCreateEditViewModel
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

    // ── Detail ─────────────────────────────────────────────
    public class LocationDetailViewModel
    {
        public Guid LocationId { get; set; }
        public string LocationCode { get; set; } = string.Empty;
        public string LocationName { get; set; } = string.Empty;
        public string? LocationLocalName { get; set; }
        public string? LocationTypeName { get; set; }
        public int LocationTypeId { get; set; }
        public string? CountryName { get; set; }
        public string? CountryName2 { get; set; }
        public string? CityTypeName { get; set; }
        public string? ParentLocationName { get; set; }
        public string? DepotTypeName { get; set; }
        public string? TerminalTypeName { get; set; }
        public string? IATACode { get; set; }
        public string? ICAOCode { get; set; }
        public float? Latitude { get; set; }
        public float? Longitude { get; set; }
        public bool IsActive { get; set; } = true;
        public DateTime CreatedDate { get; set; }
        public DateTime? ModifiedDate { get; set; }
        public string? CreatedBy { get; set; }
        public string? ModifiedBy { get; set; }
        public string? Address { get; set; }


        public string? Phone { get; set; }

        public string? Email { get; set; }
        // Child locations count (for delete guard)
        public int ChildLocationsCount { get; set; }
    }

    // ── Delete ─────────────────────────────────────────────
    public class LocationDeleteViewModel
    {
        public Guid LocationId { get; set; }
        public string LocationCode { get; set; } = string.Empty;
        public string LocationName { get; set; } = string.Empty;
        public string? LocationTypeName { get; set; }
        public string? CountryName { get; set; }
        public bool IsActive { get; set; }
        public int ChildLocationsCount { get; set; }
        public bool CanDelete => ChildLocationsCount == 0;
    }

    // ── Shared ─────────────────────────────────────────────
    public class DropdownItem<T>
    {
        public T Value { get; set; }
        public string Text { get; set; } = string.Empty;
    }
    public class DropdownItem : DropdownItem<int>
    {
    }
}