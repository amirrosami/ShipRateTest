using ShipRate.Domain.Entities.Base;
using ShipRate.Domain.enums;

namespace ShipRate.Domain.Entities.BaseInfo.Cargos
{
    public class DGRegulationSet : SystemEntity
    {
        public EnumTransportType TransportTypeId { get; set; }
        public string DGRegulationSetName { get; set; } = string.Empty;

        ////Relations 
        //public TransportType TransportType { get; set; }

    }
}

/*IMDG ===> Sea
 *IATA ===> Air
 *ADR  ===> Road
 *RID  ===> Rail
 */