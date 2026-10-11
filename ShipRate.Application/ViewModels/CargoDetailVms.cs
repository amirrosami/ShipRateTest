using Microsoft.AspNetCore.Mvc.Rendering;
using System.ComponentModel.DataAnnotations;

namespace ShipRate.Application.ViewModels
{
    public class CargoDetailFilterVm
    {
        public Guid? JobId { get; set; }
        public Guid? BookingId { get; set; }
        public Guid? BookingLegId { get; set; }
        public Guid? BLId { get; set; }
        public Guid? BLContainerId { get; set; }
        public Guid? TransitShipmentAssignmentId { get; set; }
        public Guid? RateRequestId { get; set; }

        public bool HasParent =>
            JobId.HasValue ||
            BLId.HasValue ||
            BLContainerId.HasValue ||
            TransitShipmentAssignmentId.HasValue ||
            RateRequestId.HasValue ||
            BookingId.HasValue ||
            BookingLegId.HasValue;
    }


    public class CargoDetailListItemVm
    {
        public Guid Id { get; set; }

        public int Sequence { get; set; }

        public string CargoDescription { get; set; } = null!;

        public string? PackageTypeName { get; set; }

        public decimal? PackageCount { get; set; }

        public decimal? GrossWeight { get; set; }

        public decimal? NetWeight { get; set; }

        public string? WeightUnitName { get; set; }

        public decimal? Volume { get; set; }

        public string? VolumeUnitName { get; set; }

        public bool IsDG { get; set; }

        public bool IsOverSized { get; set; }

        public bool IsTemperatureControlled { get; set; }

        public bool IsFragile { get; set; }

        public bool IsPerishable { get; set; }

        public bool IsStackable { get; set; }

        public bool IsRotateable { get; set; }

        public string HSCode { get; set; } = "";
    }


    public class CargoDetailFormVm : IValidatableObject
    {
        public Guid? Id { get; set; }

        public Guid? JobId { get; set; }
        public Guid? BookingId { get; set; }
        public Guid? BookingLegId { get; set; }
        public Guid? BLId { get; set; }
        public Guid? BLContainerId { get; set; }
        public Guid? TransitShipmentAssignmentId { get; set; }
        public Guid? RateRequestId { get; set; }

        [Required]
        public int Sequence { get; set; }

        [Required]
        public int? PackageTypeId { get; set; }
        public string? PackageTypeName { get; set; }


        [Required]
        public decimal? PackageCount { get; set; }

        public int? InnerPackageTypeId { get; set; }
        public string? InnerPackageTypeName { get; set; }

        public decimal? InnerPackageCount { get; set; }

        [Required]
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

        [Required]
        [StringLength(12)]
        public string? HSCodeToSave { get; set; }

        public bool IsDG { get; set; }

        public int? DGClassId { get; set; }

        [StringLength(30)]
        public string? UNNumber { get; set; }

        public decimal? DGFlashPoint { get; set; }

        public int? DGPackingGroupId { get; set; }

        public bool IsTemperatureControlled { get; set; }

        public decimal? MinTemp { get; set; }

        public decimal? NormalTemp { get; set; }

        public decimal? MaxTemp { get; set; }

        public int? TempUnitId { get; set; }
        public Guid? CargoCommodityId { get; set; }


        public string? MarinePolutant { get; set; }
        public string? MSDSPageNumber { get; set; }
        public string? PSN { get; set; }








        [Required]
        [StringLength(1000)]
        public string CargoDescription { get; set; } = null!;

        [StringLength(500)]
        public string? MarksAndNumbers { get; set; }

        [StringLength(200)]
        public string? Remarks { get; set; }

        public bool IsFragile { get; set; }

        public bool IsPerishable { get; set; }

        public bool IsRotateable { get; set; }

        public bool IsStackable { get; set; }

        public List<SelectListItem> PackageTypes { get; set; } = [];
        public List<SelectListItem> InnerPackageTypes { get; set; } = [];
        public List<SelectListItem> WeightUnits { get; set; } = [];
        public List<SelectListItem> VolumeUnits { get; set; } = [];
        public List<SelectListItem> DimensionUnits { get; set; } = [];
        public List<SelectListItem> TempUnits { get; set; } = [];
        public List<SelectListItem> CountryHSCodes { get; set; } = [];
        public List<SelectListItem> DGClasses { get; set; } = [];
        public List<SelectListItem> DGPackingGroups { get; set; } = [];
        public List<SelectListItem> CargoCommodities { get; set; } = [];

        public CargoDetailFilterVm ToFilter()
        {
            return new CargoDetailFilterVm
            {
                JobId = JobId,
                BLId = BLId,
                BLContainerId = BLContainerId,
                TransitShipmentAssignmentId = TransitShipmentAssignmentId,
                RateRequestId = RateRequestId,
            };
        }

        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            if (IsDG)
            {
                if (!DGClassId.HasValue)
                    yield return new ValidationResult("DG Class is required when Is DG is checked.", new[] { nameof(DGClassId) });

                if (string.IsNullOrWhiteSpace(UNNumber))
                    yield return new ValidationResult("UN Number is required when Is DG is checked.", new[] { nameof(UNNumber) });
            }

            if (IsTemperatureControlled)
            {
                if (!TempUnitId.HasValue)
                    yield return new ValidationResult("Temperature Unit is required when Temperature Controlled is checked.", new[] { nameof(TempUnitId) });
            }
        }
    }

    public class CargoDetailDetailsVm
    {
        public Guid Id { get; set; }

        public int Sequence { get; set; }

        public string CargoDescription { get; set; } = null!;

        public string? PackageTypeName { get; set; }

        public decimal? PackageCount { get; set; }

        public string? InnerPackageTypeName { get; set; }

        public decimal? InnerPackageCount { get; set; }

        public decimal? GrossWeight { get; set; }

        public decimal? NetWeight { get; set; }

        public string? WeightUnitName { get; set; }

        public decimal? Volume { get; set; }

        public string? VolumeUnitName { get; set; }

        public bool IsOverSized { get; set; }

        public decimal? Length { get; set; }

        public decimal? Width { get; set; }

        public decimal? Height { get; set; }

        public string? DimensionUnitName { get; set; }

        public string? HSCode { get; set; }

        public bool IsDG { get; set; }

        public string? DGClassName { get; set; }

        public string? UNNumber { get; set; }

        public decimal? DGFlashPoint { get; set; }

        public string? DGPackingGroupName { get; set; }

        public bool IsTemperatureControlled { get; set; }

        public decimal? MinTemp { get; set; }

        public decimal? NormalTemp { get; set; }

        public decimal? MaxTemp { get; set; }

        public string? TempUnitName { get; set; }

        public string? MarinePolutant { get; set; }
        public string? MSDSPageNumber { get; set; }
        public string? PSN { get; set; }



        public string? MarksAndNumbers { get; set; }

        public string? Remarks { get; set; }

        public bool IsFragile { get; set; }

        public bool IsPerishable { get; set; }

        public bool IsRotateable { get; set; }

        public bool IsStackable { get; set; }
    }
    public class CargoDetailDeleteVm
    {
        public Guid Id { get; set; }

        public string CargoDescription { get; set; } = null!;

        public int Sequence { get; set; }

        public CargoDetailFilterVm Filter { get; set; } = new();
    }
}
