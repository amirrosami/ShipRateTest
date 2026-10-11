using ShipRate.Domain.Entities.Base;
using ShipRate.Domain.Entities.BaseInfo.Generals;
using ShipRate.Domain.Entities.BaseInfo.Locations;
using ShipRate.Domain.Entities.BaseInfo.Vehicles;
using ShipRate.Domain.Entities.Lines;
using ShipRate.Domain.Entities.Security;
using ShipRate.Domain.Entities.Settings;

namespace ShipRate.Domain.Entities.Operations
{
    public class Voyage : BusinessEntity
    {
        public int CompanyId { get; set; }
        public Guid VesselId { get; set; }
        public string VoyageNo { get; set; } = string.Empty;
        public string? DirectionCode { get; set; } = string.Empty;
        public Guid ShippingLineId { get; set; }
        public Guid? ShippingLineServiceId { get; set; }
        public Guid OriginPortId { get; set; }
        public Guid DestinationPortId { get; set; }
        public int? VoyageOwnTypeId { get; set; }
        public Guid? LoopId { get; set; }
        public DateTime? VoyageStartDate { get; set; }
        public DateTime? VoyageEndDate { get; set; }
        public int? TransitTime { get; set; }
        public int VoyageStatusId { get; set; }
        public string Remarks { get; set; }

        //Relations
        public Company? Company { get; set; }
        public Vessel? Vessel { get; set; }
        public Carrier ShippingLine { get; set; }
        public ShippingLineService? ShippingLineService { get; set; }
        public VoyageOwnType? VoyageOwnType { get; set; }
        public Loop? Loop { get; set; }
        //public TransportMovement TransportMovement { get; set; }
        public Location OriginPort { get; set; }
        public Location DestinationPort { get; set; }
        public ICollection<VoyagePort> VoyagePorts { get; set; } = new List<VoyagePort>();
    }
    public class VoyagePort : BusinessEntity
    {
        public Guid InboundVoyageId { get; set; }
        public int VoyagePortSequence { get; set; }
        public Guid PortId { get; set; }
        public Guid? TerminalId { get; set; }
        public DateTime? ETA { get; set; }
        public DateTime? ETD { get; set; }
        public DateTime? ATA { get; set; }
        public DateTime? ATD { get; set; }
        public Guid? OutboundVoyageId { get; set; }
        public Guid? ShippingLineAgentId { get; set; }

        //Relations
        public Voyage? InboundVoyage { get; set; }
        public Voyage? OutboundVoyage { get; set; }
        public Location? Port { get; set; }
        public Location? Terminal { get; set; }
        public ICollection<VoyagePortEvent> VoyagePortEvents { get; set; } = new List<VoyagePortEvent>();

    }
    public class VoyagePortEvent : BusinessEntity
    {
        public Guid VoyagePortId { get; set; }
        public int EventTypeId { get; set; }
        public DateTime VoyagePortEventDateTime { get; set; }
        public string VoyagePortEventRemarks { get; set; } = string.Empty;

        //Relations
        public VoyagePort? VoyagePort { get; set; }
        public EventType? VoyageEventType { get; set; }
        public ApplicationUser CreateUser { get; set; }
        public ApplicationUser UpdateUser { get; set; }

    }



}

