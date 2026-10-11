using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Globalization;
using System.Text.Json;

namespace ShipRate.WebApp.Controllers
{
    [Authorize]
    public class MapsController : Controller
    {

        private readonly IHttpClientFactory _httpClientFactory;
        private readonly IConfiguration _configuration;

        public MapsController(IHttpClientFactory httpClientFactory, IConfiguration configuration)
        {
            _httpClientFactory = httpClientFactory;
            _configuration = configuration;
        }
        [Authorize("Utilities.Maps.View")]
        public IActionResult OnlineMap()
        {
            return View();
        }


        [Authorize("Utilities.Maps.View")]
        public IActionResult Index()
        {
            return View("OnlineMap");
        }
        [Authorize("Utilities.Maps.View")]
        public IActionResult Map()
        {
            return View();
        }

        [HttpGet]
        [Authorize("Utilities.Maps.View")]
        public async Task<IActionResult> GetVesselPosition(string imoOrMmsi)
        {
            if (string.IsNullOrWhiteSpace(imoOrMmsi))
                return BadRequest(new { message = "IMO or MMSI is required." });

            var apiKey = _configuration["Ais:DatalasticApiKey"];
            if (string.IsNullOrWhiteSpace(apiKey))
                return StatusCode(500, new { message = "DataDocked API key is not configured." });

            var client = _httpClientFactory.CreateClient();

            var url =
                $"https://datadocked.com/api/vessels_operations/get-vessel-info?imo_or_mmsi={Uri.EscapeDataString(imoOrMmsi)}";

            using var request = new HttpRequestMessage(HttpMethod.Get, url);
            request.Headers.Add("accept", "application/json");
            request.Headers.Add("x-api-key", apiKey);

            var response = await client.SendAsync(request);
            var json = await response.Content.ReadAsStringAsync();

            if (!response.IsSuccessStatusCode)
            {
                return StatusCode((int)response.StatusCode, new
                {
                    message = "Failed to fetch vessel data from DataDocked.",
                    details = json
                });
            }

            using var doc = JsonDocument.Parse(json);
            var root = doc.RootElement;

            // بعضی APIها داده را مستقیم در root می‌دهند، بعضی داخل data/result
            var vessel = root;
            if (root.ValueKind == JsonValueKind.Object)
            {
                if (root.TryGetProperty("data", out var dataElement) && dataElement.ValueKind == JsonValueKind.Object)
                    vessel = dataElement;
                else if (root.TryGetProperty("result", out var resultElement) && resultElement.ValueKind == JsonValueKind.Object)
                    vessel = resultElement;
            }

            var output = new
            {
                vesselName = GetString(vessel, "vessel_name", "name", "ship_name", "vessel"),
                mmsi = GetString(vessel, "mmsi"),
                imo = GetString(vessel, "imo"),
                lat = GetDouble(vessel, "lat", "latitude"),
                lng = GetDouble(vessel, "lng", "lon", "longitude", "long"),
                speed = GetDouble(vessel, "speed", "sog"),
                course = GetDouble(vessel, "course", "cog", "heading"),
                lastUpdate = GetString(vessel, "last_update", "timestamp", "updated_at", "last_position_utc"),
            };

            if (output.lat == null || output.lng == null)
            {
                return NotFound(new
                {
                    message = "Vessel found, but no coordinates were returned.",
                    raw = root
                });
            }

            return Json(output);
        }

        private static string? GetString(JsonElement element, params string[] names)
        {
            foreach (var name in names)
            {
                if (element.TryGetProperty(name, out var prop))
                {
                    if (prop.ValueKind == JsonValueKind.String)
                        return prop.GetString();

                    return prop.ToString();
                }
            }

            return null;
        }

        private static double? GetDouble(JsonElement element, params string[] names)
        {
            foreach (var name in names)
            {
                if (!element.TryGetProperty(name, out var prop))
                    continue;

                if (prop.ValueKind == JsonValueKind.Number && prop.TryGetDouble(out var number))
                    return number;

                if (prop.ValueKind == JsonValueKind.String &&
                    double.TryParse(prop.GetString(), NumberStyles.Any, CultureInfo.InvariantCulture, out var parsed))
                    return parsed;
            }

            return null;
        }
    }
}

