namespace ShipRate.WebApp.ViewModels.ShippingRouteVms
{
    // === Main Page ViewModel ===
    public class ShippingRoutePageViewModel
    {
        public List<CarrierListItem> Carriers { get; set; } = new();
        public Guid? SelectedCarrierId { get; set; }
        public string? SelectedCarrierName { get; set; }
    }

    public class CarrierListItem
    {
        public Guid CarrierId { get; set; }
        public string Code { get; set; } = string.Empty;
        public string FullName { get; set; } = string.Empty;
    }

    // === Shipping Line Service ===
    public class ServiceListViewModel
    {
        public Guid CarrierId { get; set; }
        public string CarrierName { get; set; } = string.Empty;
        public List<ServiceListItem> Services { get; set; } = new();
        public Guid? SelectedServiceId { get; set; }
    }

    public class ServiceListItem
    {
        public Guid ShippingLineServiceId { get; set; }
        public string ServiceName { get; set; } = string.Empty;
        public string? ServiceCode { get; set; }
        public bool IsActive { get; set; }
    }

    public class ServiceFormViewModel
    {
        public Guid ShippingLineServiceId { get; set; }
        public Guid CarrierId { get; set; }

        [System.ComponentModel.DataAnnotations.Required(ErrorMessage = "Service Name is required")]
        [System.ComponentModel.DataAnnotations.MaxLength(200)]
        public string ServiceName { get; set; } = string.Empty;

        [System.ComponentModel.DataAnnotations.MaxLength(50)]
        public string? ServiceCode { get; set; }

        public bool IsActive { get; set; } = true;
    }

    // === Loop ===
    public class LoopListViewModel
    {
        public Guid ShippingLineServiceId { get; set; }
        public string ServiceName { get; set; } = string.Empty;
        public List<LoopListItem> Loops { get; set; } = new();
        public Guid? SelectedLoopId { get; set; }
        public int Page { get; set; } = 1;
        public int PageSize { get; set; } = 10;
        public int TotalCount { get; set; }
        public int TotalPages => (int)Math.Ceiling((double)TotalCount / PageSize);
    }

    public class LoopListItem
    {
        public Guid LoopId { get; set; }
        public string LoopName { get; set; } = string.Empty;
        public string? LoopCode { get; set; }
        public string? CompanyName { get; set; }
        public bool IsActive { get; set; }
    }

    public class LoopFormViewModel
    {
        public Guid LoopId { get; set; }
        public Guid ShippingLineServiceId { get; set; }
        public Guid ShippingLineId { get; set; }

        [System.ComponentModel.DataAnnotations.Required(ErrorMessage = "Loop Name is required")]
        [System.ComponentModel.DataAnnotations.MaxLength(200)]
        public string LoopName { get; set; } = string.Empty;

        [System.ComponentModel.DataAnnotations.MaxLength(50)]
        public string? LoopCode { get; set; }

        public int? CompanyId { get; set; }

        public bool IsActive { get; set; } = true;

        // For dropdown
        public List<CompanyDropdownItem> Companies { get; set; } = new();
    }

    public class CompanyDropdownItem
    {
        public int CompanyId { get; set; }
        public string CompanyName { get; set; } = string.Empty;
    }

    // === Loop Port ===
    public class LoopPortListViewModel
    {
        public Guid LoopId { get; set; }
        public string LoopName { get; set; } = string.Empty;
        public List<LoopPortListItem> Ports { get; set; } = new();
        public List<LocationDropdownItem> AvailableLocations { get; set; } = new();
        public Guid? EditingPortId { get; set; }
    }

    public class LoopPortListItem
    {
        public Guid LoopPortId { get; set; }
        public Guid LoopId { get; set; }
        public Guid LocationId { get; set; }
        public string LocationName { get; set; } = string.Empty;
        public string? LocationCode { get; set; }
        public int SequenceNo { get; set; }
    }

    public class LoopPortFormViewModel
    {
        public Guid LoopPortId { get; set; }
        public Guid LoopId { get; set; }

        [System.ComponentModel.DataAnnotations.Required(ErrorMessage = "Location is required")]
        public Guid LocationId { get; set; }

        [System.ComponentModel.DataAnnotations.Required(ErrorMessage = "Sequence No is required")]
        [System.ComponentModel.DataAnnotations.Range(1, 9999)]
        public int SequenceNo { get; set; }

        public List<LocationDropdownItem> AvailableLocations { get; set; } = new();
    }

    public class LocationDropdownItem
    {
        public Guid LocationId { get; set; }
        public string LocationName { get; set; } = string.Empty;
        public string? LocationCode { get; set; }
        public string Display => string.IsNullOrEmpty(LocationCode)
            ? LocationName
            : $"{LocationName} ({LocationCode})";
    }
}
