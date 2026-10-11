using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ShipRate.Application.ViewModels;

namespace ShipRate.WebApp.Controllers
{
    [Authorize]

    [Route("api/map")]
    [ApiController]
    public class MapApiController : Controller
    {
        [HttpGet("points")]
        public IActionResult GetPoints()
        {
            var points = new List<MapPointDto>
            {
                new MapPointDto { Name = "Bandar Abbas Port", Lat = 27.1832, Lng = 56.2666, Type = "Port" },
                new MapPointDto { Name = "Jebel Ali Port", Lat = 25.0118, Lng = 55.0616, Type = "Port" },
                new MapPointDto { Name = "Tehran Office", Lat = 35.6892, Lng = 51.3890, Type = "Office" },
                new MapPointDto { Name = "Dubai Airport", Lat = 25.2532, Lng = 55.3657, Type = "Airport" }
            };

            return Json(points);
        }

        [HttpGet("routes")]
        public IActionResult GetRoutes()
        {
            var routes = new List<ShipmentRouteDto>
            {
                new ShipmentRouteDto
                {
                    ShipmentNo = "SHP-1001",
                    FromLat = 27.1832,
                    FromLng = 56.2666,
                    ToLat = 25.0118,
                    ToLng = 55.0616,
                    Mode = "Sea"
                },
                new ShipmentRouteDto
                {
                    ShipmentNo = "SHP-1002",
                    FromLat = 35.6892,
                    FromLng = 51.3890,
                    ToLat = 25.2532,
                    ToLng = 55.3657,
                    Mode = "Air"
                }
            };

            return Json(routes);
        }
    }
}
