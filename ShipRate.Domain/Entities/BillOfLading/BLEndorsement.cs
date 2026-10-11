using ShipRate.Domain.Entities.Base;
using ShipRate.Domain.Entities.BaseInfo.BLSpec;
using ShipRate.Domain.Entities.BaseInfo.Persons;

namespace ShipRate.Domain.Entities.BillOfLading
{
    public class BLEndorsement : BusinessEntity
    {
        public Guid BLId { get; set; }
        public int EndorsementSequence { get; set; }
        public DateTime EndorsementDate { get; set; }

        public Guid EndorserId { get; set; }
        public string EndorserName { get; set; }
        public Guid EndorseeId { get; set; }
        public string EndorseeName { get; set; }
        public int EndorsementTypeId { get; set; }
        public string Remarks { get; set; }
        //Relations
        public BL BL { get; set; }
        public Person Endorser { get; set; }
        public Person Endorsee { get; set; }
        public EndorsementType EndorsementType { get; set; }
    }
}
