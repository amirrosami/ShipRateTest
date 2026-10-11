namespace ShipRate.Domain.Enums
{
    public enum EnumManifestType
    {
        CargoManifest = 1,          // مانیفست بار (کلی)
        ContainerManifest = 2,      // مانیفست کانتینر
        HazardousManifest = 3,      // مانیفست کالای خطرناک
        ReeferManifest = 4,         // مانیفست کانتینر یخچالی
        OutOfGaugeManifest = 5,     // مانیفست بار خارج از اندازه
        VesselManifest = 6,         // مانیفست کشتی (برای گمرک)
        FlightManifest = 7,         // مانیفست پرواز
        TruckManifest = 8,          // مانیفست کامیون (برای حمل جاده‌ای)
        RailManifest = 9,           // مانیفست قطار
        CustomsManifest = 10,        // مانیفست گمرکی (مانند ENS، Import/Export Declaration)
        TransshipmentManifest = 11   // مانیفست ترانشیپ
    }
}