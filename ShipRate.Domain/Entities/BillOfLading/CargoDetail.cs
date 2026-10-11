using ShipRate.Domain.Entities.Base;
using ShipRate.Domain.Entities.BaseInfo.Cargos;
using ShipRate.Domain.Entities.BaseInfo.Generals;
using ShipRate.Domain.Entities.BookingOperation;
using ShipRate.Domain.Entities.Operations;
using ShipRate.Domain.Entities.RateQuotation;
namespace ShipRate.Domain.Entities.BillOfLading
{
    public class CargoDetail : BusinessEntity
    {
        public Guid? RateRequestId { get; set; }
        public Guid? JobId { get; set; }

        public Guid? BLId { get; set; }   //برای Air

        //public Guid? BLContainerId { get; set; }

        public Guid? BookingId { get; set; }
        public Guid? BookingLegId { get; set; }
        public Guid? BookingContainerTypeId { get; set; }

        public Guid? TransitShipmentAssignmentId { get; set; }

        public int Sequence { get; set; }

        public int? PackageTypeId { get; set; }
        public decimal? PackageCount { get; set; }

        public int? InnerPackageTypeId { get; set; }
        public decimal? InnerPackageCount { get; set; }

        public decimal? GrossWeight { get; set; }
        public decimal? NetWeight { get; set; }
        public decimal? ChargeableWeight { get; set; }
        public int? WeightUnitId { get; set; }

        public decimal? Volume { get; set; }
        public int? VolumeUnitId { get; set; }

        public bool IsOverSized { get; set; }
        public decimal? Length { get; set; }
        public decimal? Width { get; set; }
        public decimal? Height { get; set; }
        public int? DimensionUnitId { get; set; }

        public Guid? CountryHSCodeId { get; set; }
        public string? HSCodeToSave { get; set; } = string.Empty;


        public bool IsDG { get; set; }
        public int? DGClassId { get; set; }
        public string? UNNumber { get; set; }
        public decimal? DGFlashPoint { get; set; }
        public int? DGPackingGroupId { get; set; }



        public bool IsTemperatureControlled { get; set; }
        public decimal? MinTemp { get; set; }
        public decimal? NormalTemp { get; set; }
        public decimal? MaxTemp { get; set; }
        public int? TempUnitId { get; set; }


        public bool IsStackable { get; set; }
        public bool IsRotateable { get; set; }
        public bool IsPerishable { get; set; }
        public bool IsFragile { get; set; }

        public Guid? CargoCommodityId { get; set; }

        public string CargoDescription { get; set; } = string.Empty;
        public string? MarksAndNumbers { get; set; } = string.Empty;
        public string? Remarks { get; set; }

        //New
        public string? MarinePolutant { get; set; }
        public string? MSDSPageNumber { get; set; }
        public string? PSN { get; set; }

        //Relations
        public RateRequest? RateRequest { get; set; }
        public Job? Job { get; set; }
        public BL? BL { get; set; }
        //public BLContainer? BLContainer { get; set; }

        public Booking? Booking { get; set; }
        public BookingLeg? BookingLeg { get; set; }
        public BookingContainerType? BookingContainerType { get; set; }

        public PackageType? PackageType { get; set; }
        public PackageType? InnerPackageType { get; set; }
        public CountryHSCode? CountryHSCode { get; set; }
        public MeasurementUnit? WeightUnit { get; set; }
        public MeasurementUnit? VolumeUnit { get; set; }
        public MeasurementUnit? DimensionUnit { get; set; }
        public MeasurementUnit? TempUnit { get; set; }

        public DGClass DGClass { get; set; }
        public DGPackingGroup DGPackingGroup { get; set; }
        public CargoCommodity CargoCommodity { get; set; }



    }
}




/*یا ریز کالاهای داخل کانتینر هست 
یا ریز کالاهای روی کشنده
به خاطر همین مساله کلیدهای خارجی قابلیت نال شدن دارند و فقط یکی باید پر شود
*/



