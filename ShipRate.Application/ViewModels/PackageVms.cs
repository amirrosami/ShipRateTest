using Microsoft.AspNetCore.Mvc.Rendering;
using System.ComponentModel.DataAnnotations;

namespace ShipRate.Application.ViewModels
{

    public class RateRequestPackageFilterVm
    {
        public Guid? RateRequestId { get; set; }
        public Guid? JobId { get; set; }
        public Guid? BookingId { get; set; }
    }

    public class RateRequestContainerFilterVm
    {
        public Guid? RateRequestId { get; set; }
        public Guid? JobId { get; set; }
    }


    public class RateRequestContainerListItemVm
    {
        public Guid Id { get; set; }
        public int SizeTypeId { get; set; }
        public string ShortName { get; set; } = string.Empty;

        public int? CntOwnTypeId { get; set; }
        public string? CntOwnTypeName { get; set; }

        public int Counts { get; set; }
        public string? Remarks { get; set; }
    }

    public class RateRequestPackageListItemVm
    {
        public Guid Id { get; set; }

        public string PackageTypeName { get; set; } = string.Empty;
        public int UnitCount { get; set; }

        public decimal? UnitLength { get; set; }
        public decimal? UnitWidth { get; set; }
        public decimal? UnitHeight { get; set; }
        public string? LengthUnitName { get; set; }

        public decimal? UnitVolume { get; set; }
        public string? VolumeUnitName { get; set; }

        public decimal? UnitGrossWeight { get; set; }
        public decimal? UnitNetWeight { get; set; }
        public string? WeightUnitName { get; set; }

        public decimal? TotalVolume { get; set; }
        public decimal? TotalGrossWeight { get; set; }
        public decimal? TotalNetWeight { get; set; }

        public bool IsActive { get; set; }
        public string? Remarks { get; set; }
    }

    public class RateRequestPackageFormVm
    {
        public Guid? Id { get; set; }

        public Guid? RateRequestId { get; set; }
        public Guid? JobId { get; set; }
        public Guid? BookingId { get; set; }

        [Required]
        [Display(Name = "Package Type")]
        public int PackageTypeId { get; set; }

        [Required]
        [Display(Name = "Unit Count")]
        public int UnitCount { get; set; }

        [Display(Name = "Unit Length")]
        public decimal? UnitLength { get; set; }

        [Display(Name = "Unit Width")]
        public decimal? UnitWidth { get; set; }

        [Display(Name = "Unit Height")]
        public decimal? UnitHeight { get; set; }

        [Display(Name = "Length Unit")]
        public int? LengthMeasurementUnitId { get; set; }

        [Display(Name = "Unit Volume")]
        public decimal? UnitVolume { get; set; }

        [Display(Name = "Volume Unit")]
        public int? VolumeMeasurementUnitId { get; set; }

        [Display(Name = "Unit Gross Weight")]
        public decimal? UnitGrossWeight { get; set; }

        [Display(Name = "Unit Net Weight")]
        public decimal? UnitNetWeight { get; set; }

        [Display(Name = "Weight Unit")]
        public int? WeightMeasurementUnitId { get; set; }

        [Display(Name = "Total Length")]
        public decimal? TotalLength { get; set; }

        [Display(Name = "Total Width")]
        public decimal? TotalWidth { get; set; }

        [Display(Name = "Total Height")]
        public decimal? TotalHeight { get; set; }

        [Display(Name = "Total Volume")]
        public decimal? TotalVolume { get; set; }

        [Display(Name = "Total Gross Weight")]
        public decimal? TotalGrossWeight { get; set; }

        [Display(Name = "Total Net Weight")]
        public decimal? TotalNetWeight { get; set; }

        [Display(Name = "Remarks")]
        [StringLength(200)]
        public string? Remarks { get; set; }

        [Display(Name = "Active")]
        public bool IsActive { get; set; } = true;

        public List<SelectListItem> PackageTypes { get; set; } = new();
        public List<SelectListItem> LengthUnits { get; set; } = new();
        public List<SelectListItem> VolumeUnits { get; set; } = new();
        public List<SelectListItem> WeightUnits { get; set; } = new();
    }

    public class RateRequestPackageDetailsVm
    {
        public Guid Id { get; set; }

        public Guid? RateRequestId { get; set; }
        public Guid? JobId { get; set; }
        public Guid? BookingId { get; set; }

        public string PackageTypeName { get; set; } = string.Empty;
        public int UnitCount { get; set; }

        public decimal? UnitLength { get; set; }
        public decimal? UnitWidth { get; set; }
        public decimal? UnitHeight { get; set; }
        public string? LengthUnitName { get; set; }

        public decimal? UnitVolume { get; set; }
        public string? VolumeUnitName { get; set; }

        public decimal? UnitGrossWeight { get; set; }
        public decimal? UnitNetWeight { get; set; }
        public string? WeightUnitName { get; set; }

        public decimal? TotalLength { get; set; }
        public decimal? TotalWidth { get; set; }
        public decimal? TotalHeight { get; set; }
        public decimal? TotalVolume { get; set; }
        public decimal? TotalGrossWeight { get; set; }
        public decimal? TotalNetWeight { get; set; }

        public string? Remarks { get; set; }
        public bool IsActive { get; set; }

        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
    }

    public class RateRequestPackageDeleteVm
    {
        public Guid Id { get; set; }
        public Guid? RateRequestId { get; set; }
        public Guid? JobId { get; set; }
        public Guid? BookingId { get; set; }

        public string PackageTypeName { get; set; } = string.Empty;
        public int UnitCount { get; set; }
    }
}
