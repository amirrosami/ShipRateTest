namespace ShippingTrackingMvc.Models;

public class Shipment
{
    public string JobNumber { get; set; } = "";
    public string FileNumber { get; set; } = "";
    public string JobDate { get; set; } = "";
    public string TradeType { get; set; } = "";
    public string TransportMode { get; set; } = "";
    public string LoadType { get; set; } = "";
    public string Status { get; set; } = "";
    public string BlStatus { get; set; } = "";
    public string MovementType { get; set; } = "";
    public string Incoterm { get; set; } = "";
    public string FreightPayment { get; set; } = "";
    public string CargoHandling { get; set; } = "";
    public string CargoNature { get; set; } = "";
    public string CargoReadyDate { get; set; } = "";
    public string SalesPerson { get; set; } = "";
    public int TotalPackages { get; set; }
    public string GrossWeight { get; set; } = "";
    public string Volume { get; set; } = "";
    public string ChargeableWeight { get; set; } = "";
    public string ContainerSummary { get; set; } = "";
    public string GoodsDescription { get; set; } = "";
    public string Origin { get; set; } = "";
    public string PortOfLoading { get; set; } = "";
    public string PortOfDischarge { get; set; } = "";
    public string FinalDestination { get; set; } = "";
    public string Voyage { get; set; } = "";
    public string Carrier { get; set; } = "";
    public List<Container> Containers { get; set; } = [];
    public List<Party> Parties { get; set; } = [];
    public BillOfLading? BillOfLading { get; set; }
    public List<Milestone> Milestones { get; set; } = [];
}

public class Container
{
    public string Number { get; set; } = "";
    public string SizeType { get; set; } = "";
    public string SealNumber { get; set; } = "";
    public string Packages { get; set; } = "";
    public string GrossWeight { get; set; } = "";
    public string Volume { get; set; } = "";
}

public class Party
{
    public string Role { get; set; } = "";
    public string Name { get; set; } = "";
    public string Location { get; set; } = "";
    public string Contact { get; set; } = "";
    public string ContactIcon { get; set; } = "";
}

public class BillOfLading
{
    public string FileName { get; set; } = "";
    public string MasterBlNumber { get; set; } = "";
    public string IssuedDate { get; set; } = "";
    public string IssuedAt { get; set; } = "";
    public string Status { get; set; } = "";
    public string ReleaseType { get; set; } = "";
    public string ReleaseDate { get; set; } = "";
    public string OriginalsCopies { get; set; } = "";
    public string Carrier { get; set; } = "";
    public string IssuedBy { get; set; } = "";
    public string FreightTerms { get; set; } = "";
    public string PaymentType { get; set; } = "";
    public string FinalDestination { get; set; } = "";
}

public class Milestone
{
    public string Title { get; set; } = "";
    public string Date { get; set; } = "";
    public bool Pending { get; set; }
}
