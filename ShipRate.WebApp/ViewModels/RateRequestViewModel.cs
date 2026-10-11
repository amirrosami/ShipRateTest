using System.ComponentModel.DataAnnotations;

namespace ShipRate.WebApp.ViewModels
{
    public class RateRequestViewModel
    {

        //Customer Information
        public Guid CustomerId { get; set; }
        public Guid LeadId { get; set; }
        public string CustomerName { get; set; } = string.Empty;
        public string ContactName { get; set; } = string.Empty;
        public string MobileNo { get; set; } = string.Empty;
        public string EmailAddress { get; set; } = string.Empty;


        [Required]
        public Guid OriginId { get; set; }

        [Required]
        public Guid DestinationId { get; set; }

        [Required]
        public int TransPortTypeId { get; set; }



        [Required]
        public int CargoHandlingTypeId { get; set; }
        [Required]
        public int CargoNatureId { get; set; }
        [Required]
        public Guid CargoCommodityId { get; set; }
        public string CargoCommodityDescription { get; set; } = string.Empty;
        public bool IsHazardous { get; set; }
        public string OriginHSCode { get; set; } = string.Empty;
        public string DestinationHSCode { get; set; } = string.Empty;
        public int LoadTypeId { get; set; }
        public int TradeTypeId { get; set; }

        public int PackageTypeId { get; set; }
        public decimal PackageCounts { get; set; }
        public DateTime CargoReadinessDate { get; set; }


    }
}
