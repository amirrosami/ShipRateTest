using ShipRate.Domain.Entities.Base;
using ShipRate.Domain.Entities.BaseInfo.BLSpec;
using ShipRate.Domain.Entities.BaseInfo.Locations;
using ShipRate.Domain.Entities.Settings;

namespace ShipRate.Domain.Entities.BillOfLading
{
    public class ENSDeclaration : BusinessEntity
    {
        //public Guid ShipmentId { get; private set; }
        public int CompanyId { get; set; }
        public Guid BLId { get; set; }
        public string MRN { get; set; }

        public DateTime SubmittedAt { get; set; }
        public DateTime? AcceptedAt { get; set; }

        public int ENSDeclarationStatusId { get; set; }
        // Draft, Submitted, Accepted, Rejected

        public Guid? FirstEntryPortId { get; set; } // Port / Border Code
        public Guid? FirstEntryBorderId { get; set; } // Port / Border Code

        //Relations
        public Company Company { get; set; }
        public BL BL { get; set; }
        public ENSDeclarationStatus ENSDeclarationStatus { get; set; }
        public Location FirstEntryPort { get; set; }
        public Location FirstEntryBorder { get; set; }
    }
}



