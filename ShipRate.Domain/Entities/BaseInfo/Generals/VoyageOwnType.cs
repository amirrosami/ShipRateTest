using ShipRate.Domain.Entities.Base;

namespace ShipRate.Domain.Entities.BaseInfo.Generals
{
    public class VoyageOwnType : SystemEntity
    {
        public string VoyageOwnTypeName { get; set; } = string.Empty;

        //Relations
        //public List<Voyage>? Voyages { get; set; }
    }
}
