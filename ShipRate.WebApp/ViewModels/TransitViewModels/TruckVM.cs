namespace ShipRate.WebApp.ViewModels.TransitViewModels
{
    public class TruckVM
    {
        public Guid Id { get; set; }
        public string TruckReferenceTitle { get; set; } = string.Empty;
        public string TruckTypeName { get; set; } = string.Empty;
        public string TruckPlateNumber { get; set; } = string.Empty;
        public string TrailerPlateNumber { get; set; } = string.Empty;
        public string OwnerName { get; set; } = string.Empty;
        public string CarrierName { get; set; } = string.Empty;
        public string LastDriverName { get; set; } = string.Empty;
        public string CountryName { get; set; } = string.Empty;
        public string BrandModel { get; set; } = string.Empty;   // "Brand - Model"
        public decimal? MaxPayloadKg { get; set; }
        public decimal? MaxVolumeCBM { get; set; }
        public bool IsActive { get; set; }
    }

    public class TruckFormVM
    {
        public Guid? Id { get; set; }
        public int CompanyId { get; set; }
        public int TruckTypeId { get; set; }
        public Guid? CarrierId { get; set; }
        public Guid? OwnerId { get; set; }
        public Guid? LastDriverId { get; set; }

        public string TruckReferenceTitle { get; set; } = string.Empty;
        public string TruckPlateNumber { get; set; } = string.Empty;
        public string TrailerPlateNumber { get; set; } = string.Empty;

        public decimal? MaxPayloadKg { get; set; }
        public decimal? MaxVolumeCBM { get; set; }

        public int CountryId { get; set; }
        public string FleetSmartCardNo { get; set; } = string.Empty;
        public string TPInsuranceNo { get; set; } = string.Empty;
        public DateTime? TPInsuranceExpiryDate { get; set; }

        public int? AxleCount { get; set; }
        public string Brand { get; set; } = string.Empty;
        public string Model { get; set; } = string.Empty;
        public int? YearManufactured { get; set; }

        public bool IsActive { get; set; } = true;
    }


}
