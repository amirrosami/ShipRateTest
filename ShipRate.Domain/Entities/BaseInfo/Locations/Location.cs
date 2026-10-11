using ShipRate.Domain.Entities.Base;

namespace ShipRate.Domain.Entities.BaseInfo.Locations
{
    public class Location : BusinessEntity
    {
        public int CountryId { get; set; }
        public Guid? ParentLocationId { get; set; }
        public int LocationTypeId { get; set; }
        public string? LocationCode { get; set; } = string.Empty;
        public string LocationName { get; set; } = string.Empty;
        public string LocationLocalName { get; set; } = string.Empty;
        public float? Latitude { get; set; }
        public float? Longitude { get; set; }


        //For City
        public int? CityTypeId { get; set; }
        public Guid? CityIdForPort { get; set; }    //Only For ports = The City Id if CityTypeId=2>یعنی فقط برای بنادر میبایست پر شود 




        //For Airport
        public string? IATACode { get; set; } = string.Empty;
        public string? ICAOCode { get; set; } = string.Empty;
        public bool? HasCargoTerminal { get; set; } = false;


        //For Border
        public bool? SupportsImport { get; set; }
        public bool? SupportsExport { get; set; }
        public bool? SupportsTransit { get; set; }
        public int? CountryId2 { get; set; }



        //For Depot
        public int? DepotTypeId { get; set; }


        //For Terminal
        public int? TerminalTypeId { get; set; }
        public Guid? OperatorCompanyId { get; set; }         //// ممکن است متعلق به یک شرکت خاص باشد (OperatorCompanyId)


        //For Terminal And Depot
        public string? Address { get; set; }
        public string? Phone { get; set; }
        public string? Email { get; set; }
        public bool SystemDefined { get; set; }
        public bool IsActive { get; set; } = true;
        public bool IsPort { get; set; } = false;

        //Relations
        public LocationType LocationType { get; set; }
        public Country Country { get; set; } = null!;
        public Country Country2 { get; set; } = null!;
        public Location ParentLocation { get; set; }
        public Location CityForPort { get; set; }
        public DepotType DepotType { get; set; }
        public TerminalType TerminalType { get; set; }
        public CityType CityType { get; set; }
    }
}