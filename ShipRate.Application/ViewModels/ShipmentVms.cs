using Microsoft.AspNetCore.Mvc.Rendering;
using System.ComponentModel.DataAnnotations;

namespace ShipRate.Application.ViewModels
{
    public class ShipmentLegFilterVm
    {
        public Guid ShipmentId { get; set; }
    }

    public class ShipmentLegListItemVm
    {
        public Guid Id { get; set; }
        public Guid ShipmentId { get; set; }

        public int SequenceNo { get; set; }

        public int? LegTypeId { get; set; }
        public string? LegTypeName { get; set; }

        public int? TransportTypeId { get; set; }
        public string? TransportTypeName { get; set; }

        public Guid FromLocationId { get; set; }
        public string FromLocationName { get; set; } = string.Empty;

        public Guid ToLocationId { get; set; }
        public string ToLocationName { get; set; } = string.Empty;

        public Guid? CarrierId { get; set; }
        public string? CarrierName { get; set; }

        public DateTime? PlannedDepartureDate { get; set; }
        public DateTime? PlannedArrivalDate { get; set; }

        public DateTime? ActualDepartureDate { get; set; }
        public DateTime? ActualArrivalDate { get; set; }

        public int? LegStatusId { get; set; }
        public string? LegStatusName { get; set; }

        public int? TotalPackages { get; set; }
        public decimal? GrossWeightKg { get; set; }
        public decimal? NetWeightKg { get; set; }
        public decimal? ChargeableWeightKg { get; set; }
        public decimal? VolumeCBM { get; set; }

        public bool IsActive { get; set; }
    }

    public class ShipmentLegFormVm
    {
        public Guid? Id { get; set; }

        [Required]
        public Guid ShipmentId { get; set; }

        [Display(Name = "Sequence No")]
        [Required]
        public int SequenceNo { get; set; }

        [Display(Name = "Leg Type")]
        public int? LegTypeId { get; set; }

        [Display(Name = "Transport Type")]
        public int? TransportTypeId { get; set; }

        [Display(Name = "From Location")]
        [Required]
        public Guid? FromLocationId { get; set; }
        public string? FromLocationName { get; set; }


        [Display(Name = "To Location")]
        [Required]
        public Guid? ToLocationId { get; set; }
        public string? ToLocationName { get; set; }


        //[Display(Name = "Transport Movement")]
        //public Guid? TransportMovementId { get; set; }

        [Display(Name = "Carrier")]
        public Guid? CarrierId { get; set; }

        [Display(Name = "Planned Departure")]
        [DataType(DataType.DateTime)]
        public DateTime? PlannedDepartureDate { get; set; }

        [Display(Name = "Planned Arrival")]
        [DataType(DataType.DateTime)]
        public DateTime? PlannedArrivalDate { get; set; }

        [Display(Name = "Actual Departure")]
        [DataType(DataType.DateTime)]
        public DateTime? ActualDepartureDate { get; set; }

        [Display(Name = "Actual Arrival")]
        [DataType(DataType.DateTime)]
        public DateTime? ActualArrivalDate { get; set; }

        [Display(Name = "Chargeable Weight (Kg)")]
        public decimal? ChargeableWeightKg { get; set; }

        [Display(Name = "Gross Weight (Kg)")]
        public decimal? GrossWeightKg { get; set; }

        [Display(Name = "Net Weight (Kg)")]
        public decimal? NetWeightKg { get; set; }

        [Display(Name = "Volume (CBM)")]
        public decimal? VolumeCBM { get; set; }

        [Display(Name = "Total Packages")]
        public int? TotalPackages { get; set; }

        [Display(Name = "Package Type")]
        public int? PackageTypeId { get; set; }

        [Display(Name = "Receipt Place Text")]
        [MaxLength(60)]
        public string? ReceiptPlaceText { get; set; }

        [Display(Name = "Delivery Place Text")]
        [MaxLength(60)]
        public string? DeliveryPlaceText { get; set; }

        [Display(Name = "Flight")]
        public Guid? FlightId { get; set; }

        [Display(Name = "Flight Date")]
        public Guid? FlightDateId { get; set; }

        [Display(Name = "Voyage")]
        public Guid? VoyageId { get; set; }

        [Display(Name = "Vessel")]
        public Guid? VesselId { get; set; }

        [Display(Name = "Voyage & Vessel")]
        public string? SelectedVoyageText { get; set; }

        [Display(Name = "Trip")]
        public Guid? TripId { get; set; }

        [Display(Name = "Trip & Truck ")]
        public string? SelectedTripText { get; set; }

        [Display(Name = "Truck")]
        public Guid? TruckId { get; set; }

        [Display(Name = "Driver")]
        public Guid? DriverId { get; set; }

        [Display(Name = "Leg Status")]
        public int? LegStatusId { get; set; }

        [Display(Name = "Remarks")]
        [MaxLength(200)]
        public string? Remarks { get; set; }

        [Display(Name = "Is Active")]
        public bool IsActive { get; set; } = true;






        public string? SelectedTransportTypeName { get; set; }

        public bool IsAir =>
            !string.IsNullOrWhiteSpace(SelectedTransportTypeName) &&
            SelectedTransportTypeName.Contains("air", StringComparison.OrdinalIgnoreCase);

        public bool IsSea =>
            !string.IsNullOrWhiteSpace(SelectedTransportTypeName) &&
            (SelectedTransportTypeName.Contains("sea", StringComparison.OrdinalIgnoreCase)
             || SelectedTransportTypeName.Contains("ocean", StringComparison.OrdinalIgnoreCase));

        public bool IsRoad =>
            !string.IsNullOrWhiteSpace(SelectedTransportTypeName) &&
            (SelectedTransportTypeName.Contains("road", StringComparison.OrdinalIgnoreCase)
             || SelectedTransportTypeName.Contains("truck", StringComparison.OrdinalIgnoreCase)
             || SelectedTransportTypeName.Contains("land", StringComparison.OrdinalIgnoreCase));







        public List<SelectListItem> LegTypes { get; set; } = new();
        public List<SelectListItem> TransportTypes { get; set; } = new();
        public List<SelectListItem> Locations { get; set; } = new();
        public List<SelectListItem> Carriers { get; set; } = new();
        public List<SelectListItem> Flights { get; set; } = new();
        public List<SelectListItem> FlightDates { get; set; } = new();
        public List<SelectListItem> Voyages { get; set; } = new();
        public List<SelectListItem> Vessels { get; set; } = new();
        public List<SelectListItem> Trips { get; set; } = new();
        public List<SelectListItem> Trucks { get; set; } = new();
        public List<SelectListItem> Drivers { get; set; } = new();
        public List<SelectListItem> LegStatuses { get; set; } = new();
        public List<SelectListItem> PackageTypes { get; set; } = new();
    }
    public class ShipmentLegDetailsVm
    {
        public Guid Id { get; set; }
        public Guid ShipmentId { get; set; }
        public int SequenceNo { get; set; }

        public string? LegTypeName { get; set; }
        public string? TransportTypeName { get; set; }

        public string FromLocationName { get; set; } = string.Empty;
        public string ToLocationName { get; set; } = string.Empty;

        public string? CarrierName { get; set; }
        public string? FlightName { get; set; }
        public string? VoyageName { get; set; }
        public string? VesselName { get; set; }
        public string? TripName { get; set; }
        public string? TruckName { get; set; }
        public string? DriverName { get; set; }
        public string? FlightDateName { get; set; }
        public string? LegStatusName { get; set; }
        public string? PackageTypeName { get; set; }

        public DateTime? PlannedDepartureDate { get; set; }
        public DateTime? PlannedArrivalDate { get; set; }
        public DateTime? ActualDepartureDate { get; set; }
        public DateTime? ActualArrivalDate { get; set; }

        public decimal? ChargeableWeightKg { get; set; }
        public decimal? GrossWeightKg { get; set; }
        public decimal? NetWeightKg { get; set; }
        public decimal? VolumeCBM { get; set; }
        public int? TotalPackages { get; set; }

        public string? ReceiptPlaceText { get; set; }
        public string? DeliveryPlaceText { get; set; }
        public string? Remarks { get; set; }

        public bool IsActive { get; set; }

        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
    }

    public class ShipmentLegDeleteVm
    {
        public Guid Id { get; set; }
        public Guid ShipmentId { get; set; }
        public int SequenceNo { get; set; }
        public string FromLocationName { get; set; } = string.Empty;
        public string ToLocationName { get; set; } = string.Empty;
    }
}
