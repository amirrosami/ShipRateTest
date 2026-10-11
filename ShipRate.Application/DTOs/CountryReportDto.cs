using System;
using System.Collections.Generic;
using System.Text;

namespace ShipRate.Application.DTOs
{
    public class CountryReportDto
    {
        public string Name { get; set; } = string.Empty;
        public string Code { get; set; } = string.Empty;
        public string ZoneName { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; } 
    }
}
