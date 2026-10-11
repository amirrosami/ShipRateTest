using ShipRate.Domain.Entities.Base;

namespace ShipRate.Domain.Entities.BillOfLading
{
    public class HologramStatus : SystemEntity
    {
        public string HologramStatusName { get; set; }
    }
}


//Available = 1,
//Assigned = 2,
//Voided = 3,
//Lost = 4

