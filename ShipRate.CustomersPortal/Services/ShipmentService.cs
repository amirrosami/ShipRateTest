using ShippingTrackingMvc.Models;

namespace ShippingTrackingMvc.Services;

public interface IShipmentService
{
    Shipment? Get(string trackingNumber);
    IReadOnlyList<Shipment> GetAll();
}

public sealed class ShipmentService : IShipmentService
{
    private readonly List<Shipment> _shipments = [CreateMain(), CreateShanghai(), CreateHold()];

    public Shipment? Get(string trackingNumber) =>
        _shipments.FirstOrDefault(x => x.JobNumber.Equals(trackingNumber, StringComparison.OrdinalIgnoreCase)
            || x.FileNumber.Equals(trackingNumber, StringComparison.OrdinalIgnoreCase)
            || x.Containers.Any(c => c.Number.Equals(trackingNumber, StringComparison.OrdinalIgnoreCase)));

    public IReadOnlyList<Shipment> GetAll() => _shipments;

    private static Shipment CreateMain() => new()
    {
        JobNumber = "14050510001", FileNumber = "K12560", JobDate = "Aug 01, 2026", TradeType = "Export",
        TransportMode = "Sea", LoadType = "FCL", Status = "In progress", BlStatus = "Released", MovementType = "Port to port",
        Incoterm = "EXW · Isfahan factory", FreightPayment = "Prepaid", CargoHandling = "Bulk", CargoNature = "General cargo",
        CargoReadyDate = "Aug 05, 2026", SalesPerson = "Abbasi", TotalPackages = 480, GrossWeight = "12,400 kg", Volume = "58.2 CBM",
        ChargeableWeight = "12,400 kg", ContainerSummary = "1 × 40' HC", GoodsDescription = "Ceramic tiles", Origin = "Isfahan CFS",
        PortOfLoading = "Bandar Abbas", PortOfDischarge = "Shenzhen", FinalDestination = "Shenzhen CFS", Voyage = "MSC Aurora · Voyage 226E", Carrier = "MSC",
        Containers = [new() { Number="MSCU7213540", SizeType="40' HC", SealNumber="SL881023", Packages="480 CTN", GrossWeight="12,400 kg", Volume="58.2 CBM" }],
        Parties = [
            new() { Role="Shipper", Name="Ali Alizadeh Trading Co.", Location="Isfahan, Iran", Contact="+98 31 3555 0192", ContactIcon="phone" },
            new() { Role="Consignee", Name="Shenzhen Ceramics Import Co., Ltd.", Location="Shenzhen, Guangdong, China", Contact="ops@szceramics-import.cn", ContactIcon="mail" },
            new() { Role="Notify party", Name="Same as consignee" }
        ],
        BillOfLading = new() { FileName="BLK12560OCE.pdf", MasterBlNumber="MSCUBL998211", IssuedDate="Aug 12, 2026", IssuedAt="Bandar Abbas", Status="Released", ReleaseType="Telex release", ReleaseDate="Aug 13, 2026", OriginalsCopies="3 originals · 3 copies", Carrier="MSC", IssuedBy="Saei Tarabar Arsham", FreightTerms="Freight prepaid", PaymentType="Prepaid", FinalDestination="Shenzhen, CN" },
        Milestones = [
            new() { Title="Arrived at port of discharge — Shenzhen", Date="Estimated Sep 04, 2026", Pending=true },
            new() { Title="Bill of lading released (telex)", Date="Aug 13, 2026" },
            new() { Title="Shipped on board — MSC Aurora, voyage 226E", Date="Aug 12, 2026" },
            new() { Title="Received for shipment at Isfahan CFS", Date="Aug 09, 2026" },
            new() { Title="Cargo ready at shipper's warehouse", Date="Aug 05, 2026" },
            new() { Title="Job created", Date="Aug 01, 2026" }
        ]
    };

    private static Shipment CreateShanghai() => new() { JobNumber="14050405001", FileNumber="K12540", JobDate="Jul 25, 2026", TradeType="Export", TransportMode="Sea", LoadType="FCL", Status="Completed", BlStatus="Released", MovementType="Port to port", Origin="Bandar Abbas", PortOfLoading="Bandar Abbas", PortOfDischarge="Shanghai", FinalDestination="Shanghai CFS", Voyage="MSC · Voyage 218E", Carrier="MSC" };
    private static Shipment CreateHold() => new() { JobNumber="14050331001", FileNumber="K12490", JobDate="Jul 15, 2026", TradeType="Import", TransportMode="Sea", LoadType="FCL", Status="On hold", BlStatus="Pending", MovementType="Port to port", Origin="Shanghai", PortOfLoading="Shanghai", PortOfDischarge="Bandar Abbas", FinalDestination="Bandar Abbas CFS", Voyage="MSC · Voyage 205E", Carrier="MSC" };
}
