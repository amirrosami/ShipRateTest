using ShipRate.Domain.Entities.Base;

namespace ShipRate.Domain.Entities.BaseInfo.ContainerSpec
{
    public class ContainerSizeType : SystemEntity
    {
        public int ContainerSizeId { get; set; }
        public int ContainerTypeId { get; set; }
        public string FullName { get; set; } = string.Empty;
        public string ShortName { get; set; } = string.Empty;
        public string IsoCode { get; set; } = string.Empty;

        public decimal? LengthMeters { get; set; }
        public decimal? WidthMeters { get; set; }
        public decimal? HeightMeters { get; set; }

        public int DefaultTareWeight { get; set; }
        public int MaxPayLoadKg { get; set; }
        public decimal? MaxVolumeCBM { get; set; }

        public bool IsReefer => ContainerType.ContainerTypeAbb.ToUpper() == "RF";

        //Relations
        public ContainerSize ContainerSize { get; set; }
        public ContainerType ContainerType { get; set; }
    }
}