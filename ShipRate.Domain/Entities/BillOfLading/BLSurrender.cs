using ShipRate.Domain.Entities.Base;
using ShipRate.Domain.Entities.BaseInfo.BLSpec;
using ShipRate.Domain.Entities.Security;
using ShipRate.Domain.Entities.Settings;

namespace ShipRate.Domain.Entities.BillOfLading
{
    public class BLSurrender : BusinessEntity
    {
        public int CompanyId { get; set; }
        public int BLTypeId { get; set; }
        public Guid BLId { get; set; }
        public DateTime SurrenderDate { get; set; }
        public Guid? ConfirmByUserId { get; set; }
        public DateTime? ConfirmDate { get; set; }
        public string? Remarks { get; set; }
        //Relations
        public BL BL { get; set; }
        public Company Company { get; set; }
        public BLType BLType { get; set; }
        public ApplicationUser ConfirmByUser { get; set; }

    }
}
