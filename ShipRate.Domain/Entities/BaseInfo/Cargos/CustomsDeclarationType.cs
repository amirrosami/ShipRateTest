using ShipRate.Domain.Entities.Base;

namespace ShipRate.Domain.Entities.BaseInfo.Cargos
{
    public class CustomsDeclarationType : SystemEntity
    {
        public string CustomsDeclarationTypeName { get; set; }
        public string CustomsDeclarationTypeLocalName { get; set; }
        public int SortOrder { get; set; }
    }
}
