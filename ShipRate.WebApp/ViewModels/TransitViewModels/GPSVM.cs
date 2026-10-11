using Microsoft.AspNetCore.Mvc.Rendering;
using System.ComponentModel.DataAnnotations;

namespace ShipRate.WebApp.ViewModels.TransitViewModels
{
    public class GPSDeviceIndexViewModel
    {
        public List<GPSDeviceListItem> Devices { get; set; } = new();
        public string? SearchSerial { get; set; }
        public Guid? SearchProviderId { get; set; }
        public int? SearchStatusId { get; set; }
        public int CurrentPage { get; set; } = 1;
        public int TotalPages { get; set; }
        public int PageSize { get; set; } = 10;

        // Dropdowns
        public List<SelectListItem> Providers { get; set; } = new();
        public List<SelectListItem> Statuses { get; set; } = new();
    }

    public class GPSDeviceListItem
    {
        public Guid Id { get; set; }
        public string GPSDeviceSerialNumber { get; set; } = "";
        public string ProviderName { get; set; } = "";
        public string StatusName { get; set; } = "";
        public int GPSDeviceStatusId { get; set; }
        public string? Remarks { get; set; }
        public bool IsActive { get; set; }
    }

    public class GPSDeviceFormViewModel
    {
        public Guid? Id { get; set; }

        [Required(ErrorMessage = "Serial Number is required")]
        [MaxLength(50)]
        [Display(Name = "Serial Number")]
        public string GPSDeviceSerialNumber { get; set; } = "";

        [Required(ErrorMessage = "GPS Provider is required")]
        [Display(Name = "GPS Provider")]
        public Guid GPSProviderId { get; set; }

        [Required(ErrorMessage = "Status is required")]
        [Display(Name = "Status")]
        public int GPSDeviceStatusId { get; set; }

        [MaxLength(100)]
        public string? Remarks { get; set; }

        [Display(Name = "Active")]
        public bool IsActive { get; set; } = true;

        // Dropdowns
        public List<SelectListItem> Providers { get; set; } = new();
        public List<SelectListItem> Statuses { get; set; } = new();
    }
}
