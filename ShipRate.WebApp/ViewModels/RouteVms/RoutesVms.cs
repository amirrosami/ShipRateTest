using Microsoft.AspNetCore.Mvc.Rendering;
using ShipRate.Application.DTOs;
using ShipRate.Application.DTOs.RouteDTOs;

namespace ShipRate.WebApp.ViewModels.RouteVms
{
    // ViewModels/RouteIndexViewModel.cs
    public class RouteIndexViewModel
    {
        public RouteSearchDto SearchParams { get; set; } = new();
        public PagedResultDto<RouteDto> Routes { get; set; }

        // Dropdowns
        public List<SelectListItem> RouteModes { get; set; } = new();
        public List<SelectListItem> StatusOptions { get; set; } = new()
    {
        new SelectListItem { Value = "", Text = "All" },
        new SelectListItem { Value = "true", Text = "Active" },
        new SelectListItem { Value = "false", Text = "Inactive" }
    };
    }

    // ViewModels/RouteFormViewModel.cs
    public class RouteFormViewModel
    {
        public Guid? RouteId { get; set; }
        public string RouteName { get; set; }
        public Guid OriginId { get; set; }
        public Guid DestinationId { get; set; }

        public string? OriginName { get; set; }
        public string? DestinationName { get; set; }

        public int RouteModeId { get; set; }
        public decimal? DistanceMiles { get; set; }
        public decimal? DistanceKms { get; set; }
        public decimal? TransitTime { get; set; }
        public bool IsActive { get; set; } = true;

        public List<SelectListItem> RouteModes { get; set; } = new();
    }
}
