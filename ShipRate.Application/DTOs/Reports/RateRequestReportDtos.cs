namespace ShipRate.Application.DTOs.Reports
{
    public class RateRequestReportDto
    {
        public Guid Id { get; set; }
        public string RequestNumber { get; set; }
        public string CustomerName { get; set; }
        public DateTime RequestDate { get; set; }
        public string OriginPort { get; set; }
        public string DestinationPort { get; set; }

        public List<RateRequestPackageTypeDto> PackageTypes { get; set; } = new();
        public List<RateRequestContainerTypeDto> ContainerTypes { get; set; } = new();
        //public List<RateRequestInquiryDto> Inquiries { get; set; } = new();
    }

    public class RateRequestPackageTypeDto
    {
        public Guid Id { get; set; }
        public Guid RateRequestId { get; set; }
        public string PackageTypeName { get; set; }
        public int UnitCount { get; set; }
    }

    public class RateRequestContainerTypeDto
    {
        public Guid Id { get; set; }
        public Guid RateRequestId { get; set; }
        public string ContainerSizeTypeName { get; set; }
        public int Counts { get; set; }
    }

    public class RateRequestInquiryDto
    {
        public Guid Id { get; set; }
        public Guid RateRequestId { get; set; }
        public string InquiryNo { get; set; }
        public string VendorName { get; set; }
        public DateTime InquiryDate { get; set; }

        public List<RateRequestInquiryResponseDto> Responses { get; set; } = new();
    }

    public class RateRequestInquiryResponseDto
    {
        public Guid Id { get; set; }
        public Guid RateRequestInquiryId { get; set; }
        public Guid RateRequestId { get; set; }
        public string CarrierName { get; set; }
        public int TransitDays { get; set; }
    }
}
