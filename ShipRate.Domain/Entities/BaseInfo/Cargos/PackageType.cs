using ShipRate.Domain.Entities.Base;

namespace ShipRate.Domain.Entities.BaseInfo.Cargos
{
    public class PackageType : SystemEntity
    {
        public int Sequence { get; set; }
        public string PackageTypeCode { get; set; }
        public string PackageTypeName { get; set; } = string.Empty;
        public string PackageTypeDescription { get; set; } = string.Empty;
        public string? NumericCode { get; set; }
        public string? ENSCode { get; set; }
    }
}
