namespace ShipRate.Application.ViewModels
{
    public class MapPointDto
    {
        public string Name { get; set; } = "";
        public double Lat { get; set; }
        public double Lng { get; set; }
        public string Type { get; set; } = ""; // Port, Airport, Office, Warehouse
    }

    public class ShipmentRouteDto
    {
        public string ShipmentNo { get; set; } = "";
        public double FromLat { get; set; }
        public double FromLng { get; set; }
        public double ToLat { get; set; }
        public double ToLng { get; set; }
        public string Mode { get; set; } = ""; // Sea, Air, Road
    }
}
