using ShipRate.Domain.Entities.Base;
using ShipRate.Domain.Entities.Settings;
namespace ShipRate.Domain.Entities.Agents
{
    public class Territory : BusinessEntity
    {
        public int CompanyId { get; set; }
        public string TerritoryName { get; set; } = string.Empty;
        public string Remarks { get; set; } = string.Empty;
        //Relations
        public Company? Company { get; set; }
        public List<TerritoryCity> TerritoryCities { get; set; }
    }
}
