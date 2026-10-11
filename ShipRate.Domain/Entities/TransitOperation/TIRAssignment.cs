using ShipRate.Domain.Entities.Base;
using ShipRate.Domain.Entities.Operations;
using ShipRate.Domain.Entities.Settings;

namespace ShipRate.Domain.Entities.TransitOperation
{
    public class TIRAssignment : BusinessEntity
    {
        public int CompanyId { get; set; }
        public Guid TIRCarnetId { get; set; }
        public Guid TruckId { get; set; }
        public DateTime TIRAssignmentAssignDate { get; set; }
        public DateTime? TIRAssignmentReleaseDate { get; set; }
        public string? Remarks { get; set; }
        //Status NVARCHAR(20),

        //Relations
        public Company Company { get; set; }
        public Truck Truck { get; set; }
        public TIRCarnet TIRCarnet { get; set; }

    }
}
