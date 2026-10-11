using System.ComponentModel.DataAnnotations;

namespace ShipRate.Application.DTOs.RouteDTOs
{
    // DTOs/RouteDto.cs
    public class RouteDto
    {
        public Guid RouteId { get; set; }
        public string RouteName { get; set; }
        public Guid OriginId { get; set; }
        public string OriginName { get; set; }
        public Guid DestinationId { get; set; }
        public string DestinationName { get; set; }
        public int RouteModeId { get; set; }
        public string RouteModeName { get; set; }
        public decimal? DistanceMiles { get; set; }
        public decimal? DistanceKms { get; set; }
        public decimal? TransitTime { get; set; }
        public bool IsActive { get; set; }
    }

    // DTOs/CreateRouteDto.cs
    public class CreateRouteDto
    {
        [Required]
        public string RouteName { get; set; }
        [Required]
        public Guid OriginId { get; set; }
        [Required]
        public Guid DestinationId { get; set; }
        [Required]
        public int RouteModeId { get; set; }
        public decimal? DistanceMiles { get; set; }
        public decimal? DistanceKms { get; set; }
        public decimal? TransitTime { get; set; }
        public bool IsActive { get; set; } = true;
    }

    // DTOs/UpdateRouteDto.cs
    public class UpdateRouteDto : CreateRouteDto
    {
        [Required]
        public Guid RouteId { get; set; }
    }

    // DTOs/RouteSearchDto.cs
    public class RouteSearchDto
    {
        public string SearchTerm { get; set; }
        public Guid? OriginId { get; set; }
        public Guid? DestinationId { get; set; }

        public string? OriginName { get; set; }
        public string? DestinationName { get; set; }

        public int? RouteModeId { get; set; }
        public bool? IsActive { get; set; }
        public string SortBy { get; set; } = "RouteName";
        public bool SortDescending { get; set; }
        public int PageNumber { get; set; } = 1;
        public int PageSize { get; set; } = 10;
    }
}
