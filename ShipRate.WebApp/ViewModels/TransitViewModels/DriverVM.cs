namespace ShipRate.WebApp.ViewModels.TransitViewModels
{
    public class DriverVM
    {
        public Guid Id { get; set; }

        // From Person (join)
        public string DriverName { get; set; } = "";
        public string? NationalCode { get; set; }
        public string? Mobile { get; set; }

        // From Driver
        public string? CarrierName { get; set; }
        public Guid? CarrierId { get; set; }
        public string PassportNo { get; set; } = "";
        public string LicenseNumber { get; set; } = "";
        public string SmartCardNo { get; set; } = "";
        public bool IsActive { get; set; }
        public DateTime CreatedAt { get; set; }
    }

    public class DriverFormVM
    {
        public Guid Id { get; set; }           // = Person Id (existing person)
        public Guid? CarrierId { get; set; }
        public string PassportNo { get; set; } = "";
        public string LicenseNumber { get; set; } = "";
        public string SmartCardNo { get; set; } = "";
        public bool IsActive { get; set; } = true;

        // Display only (read-only in form)
        public string PersonName { get; set; } = "";
    }
    public class DriverSearchVM
    {
        public string? DriverName { get; set; }
        public Guid? CarrierId { get; set; }
        public bool? IsActive { get; set; }
        public int Page { get; set; } = 1;
        public int PageSize { get; set; } = 10;
    }

}
