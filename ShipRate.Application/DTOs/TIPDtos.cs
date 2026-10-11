using Microsoft.AspNetCore.Mvc.Rendering;
using System.ComponentModel.DataAnnotations;

namespace ShipRate.Application.DTOs
{
    public class TipListDto
    {
        public Guid Id { get; set; }
        public string TIPNo { get; set; }
        public DateTime TIPDate { get; set; }
        public DateTime TIPExpiryDate { get; set; }

        public string VoyageNo { get; set; }
        public string VesselName { get; set; }
        public string PortName { get; set; }

        public string TIPStatusName { get; set; }
    }
    public class TipFilterDto
    {
        public string VoyageNo { get; set; }
        public string VesselName { get; set; }
        public string TIPNo { get; set; }
        public string Port { get; set; }

        public int PageNumber { get; set; } = 1;
        public int PageSize { get; set; } = 10;
    }
    public class TipEditDto
    {
        public Guid Id { get; set; }

        public int CompanyId { get; set; }
        public Guid ShippingLineId { get; set; }

        [Required]
        public Guid VoyagePortId { get; set; }

        [Required]
        [StringLength(50)]
        public string TIPNo { get; set; }

        [Required]
        public DateTime TIPDate { get; set; }

        [Required]
        public DateTime TIPExpiryDate { get; set; }

        public Guid CustomsOfficeId { get; set; }
        public int? TIPStatusId { get; set; }

        public bool IsActive { get; set; } = true;
        public string Remarks { get; set; }

        public string? VoyageDisplayText { get; set; }

        public List<SelectListItem> Companies { get; set; } = new();
        public List<SelectListItem> ShippingLines { get; set; } = new();
        public List<SelectListItem> CustomsOffices { get; set; } = new();


    }
    public class TipUsageListDto
    {
        public Guid Id { get; set; }
        public Guid TIPId { get; set; }

        public string BLNo { get; set; }
        public string CustomerName { get; set; }
        public string TIPNo { get; set; }
        public string LetterNo { get; set; }
        public DateTime LetterDate { get; set; }
        public string LetterLocalDate { get; set; }
        public string VesselName { get; set; }
        public string VoyageNo { get; set; }
        public string CustomsOfficeName { get; set; }

        public int F20 { get; set; }
        public int F40 { get; set; }

        public List<TipUsageContainerListDto> Containers { get; set; }

    }

    public class TipUsageContainerListDto
    {
        public Guid Id { get; set; }
        public Guid TIPUsageId { get; set; }

        public string ContainerNumber { get; set; }
        public string SizeTypeShortName { get; set; }
    }
    public class VoyagePortLookupFilterDto
    {
        public string VoyageNo { get; set; }
        public string VesselName { get; set; }
        public string Port { get; set; }

        public int PageNumber { get; set; } = 1;
        public int PageSize { get; set; } = 10;
    }

    public class VoyagePortLookupDto
    {
        public Guid VoyagePortId { get; set; }
        public string VoyageNo { get; set; }
        public string VesselName { get; set; }
        public string PortName { get; set; }

        public string DisplayText => $"{VoyageNo} - {VesselName} - {PortName}";
    }

    public class VoyagePortSelectorDto
    {
        public Guid VoyagePortId { get; set; }
        public string VoyageDisplayText { get; set; }
        public string TargetPrefix { get; set; }
    }
    public class VoyagePortLookupViewModel
    {
        public VoyagePortLookupFilterDto Filter { get; set; }
        public PagedResultDto<VoyagePortLookupDto> Result { get; set; }
        public string TargetPrefix { get; set; }
    }


}
