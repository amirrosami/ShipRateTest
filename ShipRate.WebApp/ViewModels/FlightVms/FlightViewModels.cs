using Microsoft.AspNetCore.Mvc.Rendering;
using System.ComponentModel.DataAnnotations;

namespace ShipRate.WebApp.ViewModels.FlightVms
{
    public class FlightIndexViewModel
    {
        public List<FlightListItem> Flights { get; set; } = new();
        public int CurrentPage { get; set; } = 1;
        public int TotalPages { get; set; }
        public int PageSize { get; set; } = 10;
        public string SearchFlightNumber { get; set; }
        public Guid? SearchAirLineId { get; set; }

        public List<SelectListItem> AirLines { get; set; } = new();
    }

    public class FlightListItem
    {
        public Guid Id { get; set; }
        public string FlightNumber { get; set; }
        public string AirLineName { get; set; }
        public string OriginAirportName { get; set; }
        public string DestinationAirportName { get; set; }
        public bool IsActive { get; set; }
    }

    public class FlightDateListItem
    {
        public Guid Id { get; set; }
        public Guid FlightId { get; set; }
        public DateTime? ETD { get; set; }
        public DateTime? ETA { get; set; }
        public DateTime? ActualDeparture { get; set; }
        public DateTime? ActualArrival { get; set; }
        public decimal DelayHours { get; set; }
        public string Description { get; set; }
        public bool IsActive { get; set; }
    }

    public class FlightFormViewModel
    {
        public Guid Id { get; set; }

        [Required(ErrorMessage = "Flight Number is required")]
        [StringLength(20)]
        public string FlightNumber { get; set; }

        [Required(ErrorMessage = "Airline is required")]
        public Guid AirLineId { get; set; }

        [Required(ErrorMessage = "Origin Airport is required")]
        public Guid OriginAirportId { get; set; }

        [Required(ErrorMessage = "Destination Airport is required")]
        public Guid DestinationAirportId { get; set; }

        public bool IsActive { get; set; } = true;

        // Dropdowns
        public List<SelectListItem> AirLines { get; set; } = new();
        public List<SelectListItem> Airports { get; set; } = new();
    }

    public class FlightDateFormViewModel
    {
        public Guid Id { get; set; }

        [Required]
        public Guid FlightId { get; set; }

        public DateTime? ETD { get; set; }
        public DateTime? ETA { get; set; }
        public DateTime? ActualDeparture { get; set; }
        public DateTime? ActualArrival { get; set; }

        [Range(0, 999.99)]
        public decimal DelayHours { get; set; } = 0;

        //[Required(ErrorMessage = "Description is required")]
        [StringLength(200)]
        public string? Description { get; set; }

        public bool IsActive { get; set; } = true;

    }



}
