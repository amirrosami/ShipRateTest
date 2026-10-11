using ShipRate.Domain.Entities.Base;

namespace ShipRate.Domain.Entities.BaseInfo.BLSpec
{
    public class BLReleaseType : SystemEntity
    {
        public string BLReleaseTypeCode { get; set; } = string.Empty;
        public string BLReleaseTypeName { get; set; } = string.Empty;
        public string BLReleaseTypeDescription { get; set; } = string.Empty;
        public bool RequiresSettlement { get; set; }
        public bool RequiresOriginalBl { get; set; }
        public bool IsNegotiable { get; set; }
    }
}


// ORIGINAL BL, TELEX Release,Sea WAYBILL, EXPRESS, SWITCH

