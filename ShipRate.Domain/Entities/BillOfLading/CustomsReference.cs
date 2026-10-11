using ShipRate.Domain.Entities.Base;
using ShipRate.Domain.Entities.BaseInfo.Locations;
using ShipRate.Domain.Entities.Customs;

namespace ShipRate.Domain.Entities.BillOfLading
{
    public class CustomsReference : BusinessEntity
    {
        public Guid? CustomsClearanceId { get; set; }
        public Guid? BLId { get; private set; }
        public string ProcedureType { get; set; } = string.Empty;     // Import, Export, Transit, ENS
        public string MRN { get; set; }
        public int CountryId { get; set; }
        public DateTime IssuedAt { get; set; }

        //Relations
        public BL BL { get; set; }
        public Country Country { get; set; }
        public CustomsClearance CustomsClearance { get; set; }

    }
}