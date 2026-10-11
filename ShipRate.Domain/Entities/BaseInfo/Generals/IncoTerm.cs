using ShipRate.Domain.Entities.Base;

namespace ShipRate.Domain.Entities.BaseInfo.Generals
{
    public class Incoterm : SystemEntity
    {
        public int Sequence { get; set; }
        public string IncotermCode { get; set; } = string.Empty;
        public string IncoTermTitle { get; set; } = string.Empty;
        public string IncoTermLocalTitle { get; set; } = string.Empty;
        public string IncoTermDescription { get; set; } = string.Empty;
        public bool IsDoorBased { get; set; }
        public bool IsMarineOnly { get; set; }
        public bool RequiresInsurance { get; set; }
    }
}
