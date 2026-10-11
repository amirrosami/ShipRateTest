using ShipRate.Domain.Entities.Base;
using ShipRate.Domain.Entities.BaseInfo.Generals;

namespace ShipRate.Domain.Entities.Settings
{
    public class CompanyFormat : BusinessEntity
    {
        public int CompanyId { get; set; }
        public int EntityTypeId { get; set; }
        public int FormatTypeId { get; set; }
        public string CompanyFormatName { get; set; }
        public string CompanyFormatLocalName { get; set; }

        //Relations
        public Company Company { get; set; } = new();
        public EntityType EntityType { get; set; } = new();
        public FormatType FormatType { get; set; } = new();

        public List<CompanyFormatParam> Params { get; set; } = new();
    }

    public class CompanyFormatParam : BusinessEntity
    {
        public Guid CompanyFormatId { get; set; }
        public string ParamName { get; set; }
        public string ParamLocalName { get; set; }
        public string? ParamFixedValue { get; set; }
        public string? ParamFormula { get; set; }
        //Relations
        public CompanyFormat CompanyFormat { get; set; } = new();
    }

    public class FormatType : SystemEntity
    {
        public string? FormatTypeName { get; set; }
        public string? FormatTypeLocalName { get; set; }
    }

}
