namespace ShipRate.Application.DTOs
{
    public class PortDistanceDto
    {
        public Guid Id { get; set; }

        public Guid FromPortId { get; set; }
        public string FromPortName { get; set; } = string.Empty;

        public Guid ToPortId { get; set; }
        public string ToPortName { get; set; } = string.Empty;

        public int TransPortTypeId { get; set; }
        public string TransPortTypeName { get; set; } = string.Empty;

        public decimal DistanceKM { get; set; }
        public decimal DistanceNM { get; set; }

        public decimal AvgDays { get; set; }
        public decimal AvgHours { get; set; }
        public string Source { get; set; } = string.Empty;


        public class PortDistanceCreateDto
        {
            public Guid FromPortId { get; set; }
            public Guid ToPortId { get; set; }

            public string? FromPortName { get; set; }
            public string? ToPortName { get; set; }

            public int TransPortTypeId { get; set; }
            public decimal DistanceKM { get; set; }
            public decimal DistanceNM { get; set; }
            public decimal AvgDays { get; set; }
            public decimal AvgHours { get; set; }
            public string Source { get; set; } = string.Empty;
        }


        public class PortDistanceUpdateDto
        {
            public Guid Id { get; set; }
            public Guid FromPortId { get; set; }
            public Guid ToPortId { get; set; }
            public int TransPortTypeId { get; set; }
            public decimal DistanceKM { get; set; }
            public decimal DistanceNM { get; set; }
            public decimal AvgDays { get; set; }
            public decimal AvgHours { get; set; }
            public string Source { get; set; } = string.Empty;
            public string FromPortName { get; set; } = string.Empty;
            public string ToPortName { get; set; } = string.Empty;
        }
    }
}
