namespace ShipRate.Application.DTOs.Base_Info
{
    public class VesselDto
    {
        public Guid Id { get; set; }
        public int? CompanyId { get; set; } //مالک یا عامل

        public int VesselTypeId { get; set; }
        public string VesselTypeName { get; set; } = string.Empty;
        public string MMSI { get; set; }
        public string VesselCode { get; set; } = string.Empty;
        public string VesselName { get; set; } = string.Empty;
        public string CallSign { get; set; } = string.Empty;
        public string IMONumber { get; set; } = string.Empty;
        public long DWT { get; set; }

        public long? GrossTonnage { get; set; }
        public long? NetTonnage { get; set; }

        public int? BuiltYear { get; set; }
        public int? CapacityTEU { get; set; }

        public int? FlagCountryId { get; set; }
        public string FlagCountryName { get; set; } = string.Empty;

        public Guid? OwnerId { get; set; }    //Owner
        public string OwnerName { get; set; } = string.Empty;

        public Guid? ShippingLineId { get; set; }    //Operator
        public string ShippingLineName { get; set; } = string.Empty;

        public int? OwnTypeId { get; set; }
        public string OwnTypeName { get; set; }
    }

    public class VesselCreateEditDto
    {
        public Guid Id { get; set; }
        public int VesselTypeId { get; set; }
        public string VesselCode { get; set; } = string.Empty;
        public string VesselName { get; set; } = string.Empty;
        public string CallSign { get; set; } = string.Empty;
        public string IMONumber { get; set; } = string.Empty;
        public string MMSI { get; set; } = string.Empty;
        public long DWT { get; set; }

        public long? GrossTonnage { get; set; }
        public long? NetTonnage { get; set; }
        public int? BuiltYear { get; set; }
        public int? CapacityTEU { get; set; }
        public int? FlagCountryId { get; set; }
        public Guid? OwnerId { get; set; }    //Owner
        public Guid? ShippingLineId { get; set; }    //Operator
        public int? OwnTypeId { get; set; }
    }





}