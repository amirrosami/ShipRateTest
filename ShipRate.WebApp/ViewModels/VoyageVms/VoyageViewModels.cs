using Microsoft.AspNetCore.Mvc.Rendering;
using System.ComponentModel.DataAnnotations;

namespace ShipRate.WebApp.ViewModels.VoyageVms
{

    public class VoyageIndexViewModel
    {
        public List<VoyageListItem> Voyages { get; set; } = new();
        public int CurrentPage { get; set; } = 1;
        public int TotalPages { get; set; }
        public string? SearchVesselName { get; set; }
        public string? SearchVoyageNo { get; set; }
    }

    public class VoyageListItem
    {
        public Guid Id { get; set; }
        public string VoyageNo { get; set; } = string.Empty;
        public string VesselName { get; set; } = string.Empty;
        public string ShippingLineName { get; set; } = string.Empty;
        public string OriginPortName { get; set; } = string.Empty;
        public string DestinationPortName { get; set; } = string.Empty;
        public DateTime? VoyageStartDate { get; set; }
        public DateTime? VoyageEndDate { get; set; }
        public int? TransitTime { get; set; }
        public int VoyageStatusId { get; set; }
        public bool IsActive { get; set; }
    }

    public class VoyageFormViewModel
    {
        public Guid? Id { get; set; }
        public int CompanyId { get; set; }

        [Required, Display(Name = "Vessel")]
        public Guid VesselId { get; set; }

        [Required, MaxLength(30), Display(Name = "Voyage No")]
        public string VoyageNo { get; set; } = string.Empty;

        [Required, Display(Name = "Shipping Line")]
        public Guid ShippingLineId { get; set; }

        [Display(Name = "Shipping Line Service")]
        public Guid? ShippingLineServiceId { get; set; }

        [Required, Display(Name = "Origin Port")]
        public Guid OriginPortId { get; set; }
        public string? OriginPortName { get; set; }

        [Required, Display(Name = "Destination Port")]
        public Guid DestinationPortId { get; set; }
        public string? DestinationPortName { get; set; }

        [Display(Name = "Voyage Own Type")]
        public int? VoyageOwnTypeId { get; set; }

        [Display(Name = "Loop")]
        public Guid? LoopId { get; set; }

        [Display(Name = "Start Date")]
        public DateTime? VoyageStartDate { get; set; }

        [Display(Name = "End Date")]
        public DateTime? VoyageEndDate { get; set; }

        [Display(Name = "Transit Time (days)")]
        public int? TransitTime { get; set; }

        [Display(Name = "Status")]
        public int? VoyageStatusId { get; set; } = 1;

        [MaxLength(200), Display(Name = "Remarks")]
        public string Remarks { get; set; } = string.Empty;

        public bool IsActive { get; set; } = true;

        // Dropdown sources
        public List<SelectListItem> Vessels { get; set; } = new();
        public List<SelectListItem> ShippingLines { get; set; } = new();
        public List<SelectListItem> ShippingLineServices { get; set; } = new();
        public List<SelectListItem> Ports { get; set; } = new();
        public List<SelectListItem> VoyageOwnTypes { get; set; } = new();
        public List<SelectListItem> Loops { get; set; } = new();
        public List<SelectListItem> VoyageStatuses { get; set; } = new();
    }

    public class VoyagePortFormViewModel
    {
        public Guid? Id { get; set; }
        public Guid InboundVoyageId { get; set; }

        [Required, Display(Name = "Sequence")]
        public int VoyagePortSequence { get; set; }

        [Required, Display(Name = "Port")]
        public Guid PortId { get; set; }
        public string? PortName { get; set; }

        [Display(Name = "Terminal")]
        public Guid? TerminalId { get; set; }

        [Display(Name = "ETA")]
        public DateTime? ETA { get; set; }

        [Display(Name = "ETD")]
        public DateTime? ETD { get; set; }

        [Display(Name = "ATA")]
        public DateTime? ATA { get; set; }

        [Display(Name = "ATD")]
        public DateTime? ATD { get; set; }

        [Display(Name = "Outbound Voyage")]
        public Guid? OutboundVoyageId { get; set; }

        [Display(Name = "Shipping Line Agent")]
        public Guid? ShippingLineAgentId { get; set; }

        public bool IsActive { get; set; } = true;

        // Dropdown sources
        public List<SelectListItem> Ports { get; set; } = new();
        public List<SelectListItem> Terminals { get; set; } = new();
        public List<SelectListItem> OutboundVoyages { get; set; } = new();
        public List<SelectListItem> ShippingLineAgents { get; set; } = new();
    }




    public class VoyagePortListItem
    {
        public Guid Id { get; set; }
        public Guid InboundVoyageId { get; set; }
        public int VoyagePortSequence { get; set; }
        public string PortName { get; set; } = string.Empty;
        public string? TerminalName { get; set; }
        public DateTime? ETA { get; set; }
        public DateTime? ETD { get; set; }
        public DateTime? ATA { get; set; }
        public DateTime? ATD { get; set; }
        public string? OutboundVoyageNo { get; set; }
        public string ShippingLineAgentName { get; set; } = string.Empty;
        public bool IsActive { get; set; }
    }



    public class VoyagePortEventFormViewModel
    {
        public Guid? Id { get; set; }
        public Guid VoyagePortId { get; set; }

        [Required, Display(Name = "Event Type")]
        public int EventTypeId { get; set; }

        [Required, Display(Name = "Event Date/Time")]
        public DateTime VoyagePortEventDateTime { get; set; } = DateTime.Now;

        [MaxLength(200), Display(Name = "Remarks")]
        public string VoyagePortEventRemarks { get; set; } = string.Empty;

        public bool IsActive { get; set; } = true;

        // Dropdown sources
        public List<SelectListItem> EventTypes { get; set; } = new();
    }


    public class VoyagePortEventListItem
    {
        public Guid Id { get; set; }
        public Guid VoyagePortId { get; set; }
        public string EventTypeName { get; set; } = string.Empty;
        public DateTime VoyagePortEventDateTime { get; set; }
        public string VoyagePortEventRemarks { get; set; } = string.Empty;
        public bool IsActive { get; set; }

        public string? CreateUserName { get; set; }
        public string? UpdateUserName { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
    }

}

