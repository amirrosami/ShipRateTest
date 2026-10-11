using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using ShipRate.Application.Interfaces.Cargos;
using ShipRate.Application.ViewModels;
using ShipRate.Domain.Entities.RateQuotation;
using ShipRate.Infrastructure.Data;

namespace ShipRate.Application.Services.Cargos
{
    public class PackagesService : IPackagesService
    {
        private readonly ApplicationDbContext _db;

        public PackagesService(ApplicationDbContext db)
        {
            _db = db;
        }

        public async Task<List<RateRequestPackageListItemVm>> GetListAsync(RateRequestPackageFilterVm filter)
        {
            var query = _db.RateRequestPackages
                .AsNoTracking()
                .Where(x => !x.IsDeleted);

            query = ApplyParentFilter(query, filter);

            return await query
                .OrderByDescending(x => x.CreatedAt)
                .Select(x => new RateRequestPackageListItemVm
                {
                    Id = x.Id,
                    PackageTypeName = x.PackageType != null ? x.PackageType.PackageTypeName : "",
                    UnitCount = x.UnitCount,

                    UnitLength = x.UnitLength,
                    UnitWidth = x.UnitWidth,
                    UnitHeight = x.UnitHeight,
                    LengthUnitName = x.LengthMeasurementUnit != null ? x.LengthMeasurementUnit.MeasurementUnitName : null,

                    UnitVolume = x.UnitVolume,
                    VolumeUnitName = x.VolumeMeasurementUnit != null ? x.VolumeMeasurementUnit.MeasurementUnitName : null,

                    UnitGrossWeight = x.UnitGrossWeight,
                    UnitNetWeight = x.UnitNetWeight,
                    WeightUnitName = x.WeightMeasurementUnit != null ? x.WeightMeasurementUnit.MeasurementUnitName : null,

                    TotalVolume = x.TotalVolume,
                    TotalGrossWeight = x.TotalGrossWeight,
                    TotalNetWeight = x.TotalNetWeight,

                    Remarks = x.Remarks,
                    //IsActive = x.IsActive
                })
                .ToListAsync();
        }

        public async Task<RateRequestPackageFormVm> BuildCreateVmAsync(RateRequestPackageFilterVm filter)
        {
            var vm = new RateRequestPackageFormVm
            {
                RateRequestId = filter.RateRequestId,
                JobId = filter.JobId,
                BookingId = filter.BookingId,
                IsActive = true
            };

            await FillLookupsAsync(vm);
            return vm;
        }

        public async Task<RateRequestPackageFormVm?> BuildEditVmAsync(Guid id)
        {
            var entity = await _db.RateRequestPackages
                .AsNoTracking()
                .FirstOrDefaultAsync(x => x.Id == id && !x.IsDeleted);

            if (entity == null)
                return null;

            var vm = new RateRequestPackageFormVm
            {
                Id = entity.Id,
                RateRequestId = entity.RateRequestId,
                JobId = entity.JobId,
                BookingId = entity.BookingId,

                PackageTypeId = entity.PackageTypeId,
                UnitCount = entity.UnitCount,
                UnitLength = entity.UnitLength,
                UnitWidth = entity.UnitWidth,
                UnitHeight = entity.UnitHeight,
                LengthMeasurementUnitId = entity.LengthMeasurementUnitId,

                UnitVolume = entity.UnitVolume,
                VolumeMeasurementUnitId = entity.VolumeMeasurementUnitId,

                UnitGrossWeight = entity.UnitGrossWeight,
                UnitNetWeight = entity.UnitNetWeight,
                WeightMeasurementUnitId = entity.WeightMeasurementUnitId,

                TotalLength = entity.TotalLength,
                TotalWidth = entity.TotalWidth,
                TotalHeight = entity.TotalHeight,
                TotalVolume = entity.TotalVolume,
                TotalGrossWeight = entity.TotalGrossWeight,
                TotalNetWeight = entity.TotalNetWeight,

                Remarks = entity.Remarks,
                //IsActive = entity.IsActive
            };

            await FillLookupsAsync(vm);
            return vm;
        }

        public async Task<RateRequestPackageDetailsVm?> GetDetailsAsync(Guid id)
        {
            return await _db.RateRequestPackages
                .AsNoTracking()
                .Where(x => x.Id == id && !x.IsDeleted)
                .Select(x => new RateRequestPackageDetailsVm
                {
                    Id = x.Id,
                    RateRequestId = x.RateRequestId,
                    JobId = x.JobId,
                    BookingId = x.BookingId,

                    PackageTypeName = x.PackageType != null ? x.PackageType.PackageTypeName : "",
                    UnitCount = x.UnitCount,

                    UnitLength = x.UnitLength,
                    UnitWidth = x.UnitWidth,
                    UnitHeight = x.UnitHeight,
                    LengthUnitName = x.LengthMeasurementUnit != null ? x.LengthMeasurementUnit.MeasurementUnitName : null,

                    UnitVolume = x.UnitVolume,
                    VolumeUnitName = x.VolumeMeasurementUnit != null ? x.VolumeMeasurementUnit.MeasurementUnitName : null,

                    UnitGrossWeight = x.UnitGrossWeight,
                    UnitNetWeight = x.UnitNetWeight,
                    WeightUnitName = x.WeightMeasurementUnit != null ? x.WeightMeasurementUnit.MeasurementUnitName : null,

                    TotalLength = x.TotalLength,
                    TotalWidth = x.TotalWidth,
                    TotalHeight = x.TotalHeight,
                    TotalVolume = x.TotalVolume,
                    TotalGrossWeight = x.TotalGrossWeight,
                    TotalNetWeight = x.TotalNetWeight,

                    Remarks = x.Remarks,
                    //IsActive = x.IsActive,
                    CreatedAt = x.CreatedAt,
                    UpdatedAt = x.UpdatedAt
                })
                .FirstOrDefaultAsync();
        }

        public async Task<RateRequestPackageDeleteVm?> GetDeleteVmAsync(Guid id)
        {
            return await _db.RateRequestPackages
                .AsNoTracking()
                .Where(x => x.Id == id && !x.IsDeleted)
                .Select(x => new RateRequestPackageDeleteVm
                {
                    Id = x.Id,
                    RateRequestId = x.RateRequestId,
                    JobId = x.JobId,
                    BookingId = x.BookingId,
                    PackageTypeName = x.PackageType != null ? x.PackageType.PackageTypeName : "",
                    UnitCount = x.UnitCount
                })
                .FirstOrDefaultAsync();
        }

        public async Task<(bool Success, string? Error)> CreateAsync(RateRequestPackageFormVm model, Guid userId)
        {
            if (!await ParentReferenceIsValid(model))
                return (false, "Parent reference is invalid.");

            var entity = new RateRequestPackage
            {
                Id = Guid.CreateVersion7(),
                RateRequestId = model.RateRequestId,
                JobId = model.JobId,
                BookingId = model.BookingId,

                PackageTypeId = model.PackageTypeId,
                UnitCount = model.UnitCount,

                UnitLength = model.UnitLength,
                UnitWidth = model.UnitWidth,
                UnitHeight = model.UnitHeight,
                LengthMeasurementUnitId = model.LengthMeasurementUnitId,

                UnitVolume = model.UnitVolume,
                VolumeMeasurementUnitId = model.VolumeMeasurementUnitId,

                UnitGrossWeight = model.UnitGrossWeight,
                UnitNetWeight = model.UnitNetWeight,
                WeightMeasurementUnitId = model.WeightMeasurementUnitId,

                TotalLength = model.TotalLength,
                TotalWidth = model.TotalWidth,
                TotalHeight = model.TotalHeight,
                TotalVolume = model.TotalVolume,
                TotalGrossWeight = model.TotalGrossWeight,
                TotalNetWeight = model.TotalNetWeight,

                Remarks = model.Remarks,
                //IsActive = model.IsActive,
                IsDeleted = false,
                CreatedAt = DateTime.Now,
                CreateUserId = userId
            };

            _db.RateRequestPackages.Add(entity);
            await _db.SaveChangesAsync();

            return (true, null);
        }

        public async Task<(bool Success, string? Error)> UpdateAsync(RateRequestPackageFormVm model, Guid userId)
        {
            if (!model.Id.HasValue)
                return (false, "Invalid package id.");

            var entity = await _db.RateRequestPackages
                .FirstOrDefaultAsync(x => x.Id == model.Id.Value && !x.IsDeleted);

            if (entity == null)
                return (false, "Package not found.");

            entity.PackageTypeId = model.PackageTypeId;
            entity.UnitCount = model.UnitCount;

            entity.UnitLength = model.UnitLength;
            entity.UnitWidth = model.UnitWidth;
            entity.UnitHeight = model.UnitHeight;
            entity.LengthMeasurementUnitId = model.LengthMeasurementUnitId;

            entity.UnitVolume = model.UnitVolume;
            entity.VolumeMeasurementUnitId = model.VolumeMeasurementUnitId;

            entity.UnitGrossWeight = model.UnitGrossWeight;
            entity.UnitNetWeight = model.UnitNetWeight;
            entity.WeightMeasurementUnitId = model.WeightMeasurementUnitId;

            entity.TotalLength = model.TotalLength;
            entity.TotalWidth = model.TotalWidth;
            entity.TotalHeight = model.TotalHeight;
            entity.TotalVolume = model.TotalVolume;
            entity.TotalGrossWeight = model.TotalGrossWeight;
            entity.TotalNetWeight = model.TotalNetWeight;

            entity.Remarks = model.Remarks;
            //entity.IsActive = model.IsActive;
            entity.UpdatedAt = DateTime.Now;
            entity.EditUserId = userId;

            await _db.SaveChangesAsync();

            return (true, null);
        }

        public async Task<(bool Success, string? Error)> DeleteAsync(Guid id, Guid userId)
        {
            var entity = await _db.RateRequestPackages
                .FirstOrDefaultAsync(x => x.Id == id && !x.IsDeleted);

            if (entity == null)
                return (false, "Package not found.");

            entity.IsDeleted = true;
            //entity.IsActive = false;
            entity.UpdatedAt = DateTime.Now;
            entity.EditUserId = userId;

            await _db.SaveChangesAsync();
            return (true, null);
        }

        public async Task FillLookupsAsync(RateRequestPackageFormVm vm)
        {
            vm.PackageTypes = await _db.PackageTypes
                .AsNoTracking()
                .Where(x => x.IsActive && !x.IsDeleted)
                .OrderBy(x => x.PackageTypeName)
                .Select(x => new SelectListItem
                {
                    Value = x.Id.ToString(),
                    Text = x.PackageTypeName
                })
                .ToListAsync();

            vm.LengthUnits = await _db.MeasurementUnits
                .AsNoTracking()
                .Where(x => x.IsActive && !x.IsDeleted && x.MeasurementUnitCategoryId == 3)
                .OrderBy(x => x.MeasurementUnitName)
                .Select(x => new SelectListItem
                {
                    Value = x.Id.ToString(),
                    Text = x.MeasurementUnitName
                })
                .ToListAsync();

            vm.VolumeUnits = await _db.MeasurementUnits
                .AsNoTracking()
                .Where(x => x.IsActive && !x.IsDeleted && x.MeasurementUnitCategoryId == 2)
                .OrderBy(x => x.MeasurementUnitName)
                .Select(x => new SelectListItem
                {
                    Value = x.Id.ToString(),
                    Text = x.MeasurementUnitName
                })
                .ToListAsync();

            vm.WeightUnits = await _db.MeasurementUnits
                .AsNoTracking()
                .Where(x => x.IsActive && !x.IsDeleted && x.MeasurementUnitCategoryId == 1)
                .OrderBy(x => x.MeasurementUnitName)
                .Select(x => new SelectListItem
                {
                    Value = x.Id.ToString(),
                    Text = x.MeasurementUnitName
                })
                .ToListAsync();
        }

        private IQueryable<RateRequestPackage> ApplyParentFilter(
            IQueryable<RateRequestPackage> query,
            RateRequestPackageFilterVm filter)
        {
            if (filter.RateRequestId.HasValue)
                query = query.Where(x => x.RateRequestId == filter.RateRequestId.Value);

            if (filter.JobId.HasValue)
                query = query.Where(x => x.JobId == filter.JobId.Value);

            if (filter.BookingId.HasValue)
                query = query.Where(x => x.BookingId == filter.BookingId.Value);

            return query;
        }

        private async Task<bool> ParentReferenceIsValid(RateRequestPackageFormVm model)
        {
            if (model.RateRequestId.HasValue)
            {
                var exists = await _db.RateRequests.AnyAsync(x => x.Id == model.RateRequestId.Value);
                if (!exists) return false;
            }

            if (model.JobId.HasValue)
            {
                var exists = await _db.Jobs.AnyAsync(x => x.Id == model.JobId.Value);
                if (!exists) return false;
            }

            if (model.BookingId.HasValue)
            {
                var exists = await _db.Bookings.AnyAsync(x => x.Id == model.BookingId.Value);
                if (!exists) return false;
            }

            return true;
        }
    }
}
