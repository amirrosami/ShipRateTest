using Microsoft.AspNetCore.Mvc.Rendering;

namespace ShipRate.Application.DTOs
{
    public class VoyageMonitoringRowDto
    {
        public Guid VoyagePortId { get; set; }
        public Guid InboundVoyageId { get; set; }
        public Guid? OutboundVoyageId { get; set; }

        public string VesselName { get; set; } = string.Empty;
        public string ShippingLineName { get; set; } = string.Empty;

        public string VoyageNoIn { get; set; } = string.Empty;
        public string VoyageNoOut { get; set; } = string.Empty;
        public string DirectionCode { get; set; } = string.Empty;

        public string PortName { get; set; } = string.Empty;
        public string TerminalName { get; set; } = string.Empty;
        public int VoyagePortSequence { get; set; }

        public DateTime? ETA { get; set; }
        public DateTime? ATA { get; set; }
        public DateTime? BerthingDate { get; set; }
        public DateTime? OperationsCommencedDate { get; set; }
        public DateTime? OperationsCompletedDate { get; set; }
        public DateTime? VoyageCompletedDate { get; set; }

        public string LastEventName { get; set; } = string.Empty;
        public DateTime? LastEventDate { get; set; }

        public string CurrentStage { get; set; } = string.Empty;

        public string StageCssClass =>
            CurrentStage switch
            {
                "Voyage Completed" => "bg-success",
                "Operations Completed" => "bg-primary",
                "Under Operations" => "bg-warning text-dark",
                "Berthed" => "bg-info text-dark",
                "Arrived" => "bg-secondary",
                "Expected" => "bg-light text-dark border",
                _ => "bg-dark"
            };






    }

    public class VoyageSearchRequestDto
    {
        public string? VoyageNo { get; set; }
        public Guid? PortId { get; set; }
        public Guid? VesselId { get; set; }
        public Guid? ShippingLineId { get; set; }
        public int? EventTypeId { get; set; }
        public DateTime? EventDateFrom { get; set; }
        public DateTime? EventDateTo { get; set; }
        public DateTime? EtaFrom { get; set; }
        public DateTime? EtaTo { get; set; }
        public DateTime? AtaFrom { get; set; }
        public DateTime? AtaTo { get; set; }
        public int PageNumber { get; set; } = 1;
        public int PageSize { get; set; } = 20;
        public string SortBy { get; set; } = "eta";
        public string PortName { get; set; }
        public bool SortDesc { get; set; }
    }
    public static class VoyageEventTypeIds
    {
        public const int VesselEtaUpdated = 100;
        public const int ArrivalToPort = 105;
        public const int VesselBerthed = 110;
        public const int OperationsCommenced = 115;
        public const int DischargeCommenced = 116;
        public const int LoadingCommenced = 117;
        public const int OperationsCompleted = 120;
        public const int DischargeCompleted = 121;
        public const int LoadingCompleted = 122;
        public const int VoyageCompleted = 125;
    }


    public class VoyageManagementIndexViewModel
    {
        public VoyageSearchRequestDto Search { get; set; } = new();

        public List<SelectListItem> Ports { get; set; } = new();
        public List<SelectListItem> Vessels { get; set; } = new();
        public List<SelectListItem> ShippingLines { get; set; } = new();
        public List<SelectListItem> EventTypes { get; set; } = new();
    }



    public class VoyagePortTimelineItemDto
    {
        public Guid VoyagePortEventId { get; set; }
        public int EventTypeId { get; set; }
        public string EventTypeName { get; set; } = string.Empty;
        public DateTime EventDateTime { get; set; }
        public string Remarks { get; set; } = string.Empty;
    }
    public class VoyagePortDetailsDto
    {
        public Guid VoyagePortId { get; set; }
        public string VesselName { get; set; } = string.Empty;
        public string InboundVoyageNo { get; set; } = string.Empty;
        public string OutboundVoyageNo { get; set; } = string.Empty;
        public string PortName { get; set; } = string.Empty;
        public string? TerminalName { get; set; }

        public DateTime? ETA { get; set; }
        public DateTime? ATA { get; set; }
        public DateTime? ETD { get; set; }
        public DateTime? ATD { get; set; }

        public List<VoyagePortTimelineItemDto> Timeline { get; set; } = new();
    }

    public class VoyageMonitoringDetailsDto
    {
        public Guid VoyagePortId { get; set; }
        public string VesselName { get; set; } = string.Empty;
        public string ShippingLineName { get; set; } = string.Empty;
        public string PortName { get; set; } = string.Empty;
        public string TerminalName { get; set; } = string.Empty;
        public string VoyageNoIn { get; set; } = string.Empty;
        public string VoyageNoOut { get; set; } = string.Empty;

        public DateTime? ETA { get; set; }
        public DateTime? ATA { get; set; }
        public DateTime? ETD { get; set; }
        public DateTime? ATD { get; set; }

        public List<VoyageEventItemDto> Events { get; set; } = new();
    }

    public class VoyageEventItemDto
    {
        public int EventTypeId { get; set; }
        public string EventTypeName { get; set; } = string.Empty;
        public DateTime EventDateTime { get; set; }
        public string Remarks { get; set; } = string.Empty;
    }

    public class VoyageActionRequestDto
    {
        public Guid VoyagePortId { get; set; }
        public string ActionCode { get; set; } = string.Empty;
        public DateTime? ActionDateTime { get; set; }
        public string? Remarks { get; set; }
    }



}
