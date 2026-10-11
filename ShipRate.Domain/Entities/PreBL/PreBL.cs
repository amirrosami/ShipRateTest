using ShipRate.Domain.Entities.Base;

namespace ShipRate.Domain.Entities.PreBL
{
    public class PreBL : BusinessEntity
    {
        public Guid BookingId { get; set; }
        public Guid PORId { get; set; }
        public string PORName { get; set; }
        public Guid POLId { get; set; }
        public string POLName { get; set; }
        public Guid PODId { get; set; }
        public string PODName { get; set; }
        public int DestId { get; set; }
        public string DestName { get; set; }
        public string PreBLGoodsDescription { get; set; } = string.Empty;
        public string PreBLShipper { get; set; } = string.Empty;
        public string PreBLConsignee { get; set; } = string.Empty;
        public string PreBLNotify { get; set; } = string.Empty;
        ///takmil shavad
    }
}
