using ShipRate.Domain.Entities.Base;

namespace ShipRate.Domain.Entities.PreBL
{
    public class PreBLContainer : BusinessEntity
    {
        public Guid PreBLId { get; set; }
        public string PreBLContainerNumber { get; set; } = string.Empty;
        public string PreBLContainerSealNumber { get; set; } = string.Empty;
        public int PreBLContainerGrossWeigth { get; set; }
        public int PreBLContainerTareWeigth { get; set; }
        public int PreBLContainerVolume { get; set; }
    }
}