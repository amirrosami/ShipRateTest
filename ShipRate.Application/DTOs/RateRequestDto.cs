namespace ShipRate.Application.DTOs
{
    public class RateRequestDto
    {
        public Guid Id { get; set; }
        public Guid? CompanyId { get; set; }   //مالک دیتا
        public Guid? CustomerId { get; set; }
        public Guid? CustomerName { get; set; }
        public Guid? LeadId { get; set; }
        public Guid? LeadName { get; set; }
        public Guid? CustomerContactId { get; set; }
        public string ContactName { get; set; } = string.Empty;
        public string MobileNo { get; set; } = string.Empty;
        public string EmailAddress { get; set; } = string.Empty;

        public int CargoHandlingTypeId { get; set; }
        public int CargoNatureId { get; set; }
        public Guid? CargoCommodityId { get; set; }
        public string CargoCommodityDescription { get; set; }

        public bool IsHazardous { get; set; }
        public Guid DGId { get; set; }

        public string OriginHSCode { get; set; }
        public string DestinationHSCode { get; set; }

        public int TransportTypeId { get; set; }
        public string TransportTypeName { get; set; } = string.Empty;



        public int LoadTypeId { get; set; }

        public Guid? OriginPortId { get; set; }
        public Guid? DestinationPortId { get; set; }

        public string OriginPortName { get; set; } = string.Empty;
        public string DestinationPortName { get; set; } = string.Empty;

        public string OriginPortIsoCode { get; set; } = string.Empty;
        public string DestinationPortIsoCode { get; set; } = string.Empty;




        public Guid? OriginAirportId { get; set; }
        public Guid? DestinationAirportId { get; set; }

        public int? TradeTypeId { get; set; }


        public int StatusId { get; set; }
        public Guid? SalesManagerId { get; set; }
        public Guid? SalesmanId { get; set; }
        public int? PackageTypeId { get; set; }
        public decimal? PackageCounts { get; set; }
        public long? CargoValue { get; set; }
        public int? CurrencyId { get; set; }
        public DateTime CargoReadinessDate { get; set; }
        public int? QuotationPriorityId { get; set; }

        //Transit Specific
        public bool HasEx1 { get; set; }
        public string ZipCode { get; set; }  //برای ترانزیت خیلی مهم است
        public bool TransitHasTranshipment { get; set; } //حمل مستقیم یا حمل ترانشیپ برای ترانزیت وارداتی
        public int SellToCountryId { get; set; }//فروش کالا به کدام کشور است 


        public string Remarks { get; set; }

    }
}
