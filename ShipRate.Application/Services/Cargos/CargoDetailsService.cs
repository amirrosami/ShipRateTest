using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using ShipRate.Application.Interfaces;
using ShipRate.Application.Interfaces.Cargos;
using ShipRate.Application.Interfaces.Jobs;
using ShipRate.Application.ViewModels;
using ShipRate.Domain.Entities.BillOfLading;
using ShipRate.Infrastructure.Data;

namespace ShipRate.Application.Services.Cargos
{
    public class CargoDetailsService : ICargoDetailsService
    {
        private readonly ApplicationDbContext _context;
        private readonly IJobLookupService _lookupService;
        private readonly IActivityLogService _activityLogService;
        public CargoDetailsService(ApplicationDbContext context, IJobLookupService lookupService, IActivityLogService activityLogService)
        {
            _context = context;
            _lookupService = lookupService;
            _activityLogService = activityLogService;
        }
        public async Task<List<CargoDetailListItemVm>> GetListAsync(CargoDetailFilterVm filter)
        {
            if (!filter.HasParent)
                return [];

            var query = _context.CargoDetails
                .AsNoTracking()
                .Where(x => x.IsDeleted == false);

            query = ApplyFilter(query, filter);

            return await query
                .OrderBy(x => x.Sequence)
                .Select(x => new CargoDetailListItemVm
                {
                    Id = x.Id,
                    Sequence = x.Sequence,
                    CargoDescription = x.CargoDescription,
                    PackageCount = x.PackageCount,
                    PackageTypeName = x.PackageType != null ? x.PackageType.PackageTypeName : null,
                    GrossWeight = x.GrossWeight,
                    NetWeight = x.NetWeight,
                    WeightUnitName = x.WeightUnit != null ? x.WeightUnit.MeasurementUnitName : null,
                    Volume = x.Volume,
                    VolumeUnitName = x.VolumeUnit != null ? x.VolumeUnit.MeasurementUnitName : null,
                    IsDG = x.IsDG,
                    IsTemperatureControlled = x.IsTemperatureControlled,
                    IsFragile = x.IsFragile,
                    IsPerishable = x.IsPerishable,
                    IsRotateable = x.IsRotateable,
                    IsStackable = x.IsStackable,
                    IsOverSized = x.IsOverSized,
                    HSCode = x.HSCodeToSave
                })
                .ToListAsync();
        }

        public async Task<CargoDetailFormVm> BuildCreateModelAsync(CargoDetailFilterVm filter)
        {
            var nextSequence = 1;

            if (filter.HasParent)
            {
                var query = ApplyFilter(
                    _context.CargoDetails.Where(x => x.IsDeleted == false),
                    filter);

                var maxSequence = await query.Select(x => (int?)x.Sequence).MaxAsync();
                nextSequence = (maxSequence ?? 0) + 1;
            }

            var model = new CargoDetailFormVm
            {
                JobId = filter.JobId,
                BLId = filter.BLId,
                BLContainerId = filter.BLContainerId,
                TransitShipmentAssignmentId = filter.TransitShipmentAssignmentId,
                RateRequestId = filter.RateRequestId,
                BookingId = filter.BookingId,
                BookingLegId = filter.BookingLegId,
                Sequence = nextSequence
            };
            await FillLookupsAsync(model);

            if (model.BLId != null && model.BLId.HasValue && model.BLId != Guid.Empty)
            {
                var bl = await _context.BLs.Include(x => x.PackageType).Where(x => x.Id == model.BLId).FirstOrDefaultAsync();
                if (bl != null)
                {
                    model.PackageTypeId = bl.PackageTypeId;
                    model.PackageCount = bl.TotalPackages;
                    model.CargoDescription = bl.GoodsDescription;
                    model.GrossWeight = bl.GrossWeightKg;
                    model.ChargeableWeight = bl.ChargeableWeight;
                    model.PackageTypeName = bl.PackageType?.PackageTypeName;
                    model.WeightUnitId = 1;
                    model.VolumeUnitId = 2;
                    model.Volume = bl.VolumeCBM;
                }
            }
            return model;
        }

        public async Task<CargoDetailFormVm?> BuildEditModelAsync(Guid id)
        {
            var entity = await _context.CargoDetails
                .Include(x => x.PackageType)
                .Include(x => x.InnerPackageType)
                .AsNoTracking()
                .FirstOrDefaultAsync(x => x.Id == id && x.IsDeleted == false);

            if (entity == null)
                return null;

            var model = new CargoDetailFormVm
            {
                Id = entity.Id,

                JobId = entity.JobId,
                BLId = entity.BLId,
                TransitShipmentAssignmentId = entity.TransitShipmentAssignmentId,
                RateRequestId = entity.RateRequestId,
                Sequence = entity.Sequence,
                PackageTypeId = entity.PackageTypeId,
                PackageTypeName = entity.PackageType?.PackageTypeName,
                PackageCount = entity.PackageCount,
                InnerPackageTypeId = entity.InnerPackageTypeId,
                InnerPackageTypeName = entity.InnerPackageType?.PackageTypeName,
                InnerPackageCount = entity.InnerPackageCount,
                GrossWeight = entity.GrossWeight,
                NetWeight = entity.NetWeight,
                ChargeableWeight = entity.ChargeableWeight,
                WeightUnitId = entity.WeightUnitId,
                Volume = entity.Volume,
                VolumeUnitId = entity.VolumeUnitId,
                IsOverSized = entity.IsOverSized,
                Length = entity.Length,
                Width = entity.Width,
                Height = entity.Height,
                DimensionUnitId = entity.DimensionUnitId,
                //CountryHSCodeId = entity.CountryHSCodeId,
                HSCodeToSave = entity.HSCodeToSave,
                IsDG = entity.IsDG,
                DGClassId = entity.DGClassId,
                UNNumber = entity.UNNumber,
                DGFlashPoint = entity.DGFlashPoint,
                DGPackingGroupId = entity.DGPackingGroupId,
                IsTemperatureControlled = entity.IsTemperatureControlled,
                MinTemp = entity.MinTemp,
                NormalTemp = entity.NormalTemp,
                MaxTemp = entity.MaxTemp,
                TempUnitId = entity.TempUnitId,
                CargoDescription = entity.CargoDescription,
                MarksAndNumbers = entity.MarksAndNumbers,
                Remarks = entity.Remarks,
                IsFragile = entity.IsFragile,
                IsPerishable = entity.IsPerishable,
                IsRotateable = entity.IsRotateable,
                IsStackable = entity.IsStackable,
                CargoCommodityId = entity.CargoCommodityId,
                MarinePolutant = entity.MarinePolutant,
                PSN = entity.PSN,
                MSDSPageNumber = entity.MSDSPageNumber,
            };

            await FillLookupsAsync(model);

            return model;
        }

        public async Task<CargoDetailDetailsVm?> GetDetailsAsync(Guid id)
        {
            return await _context.CargoDetails
                .AsNoTracking()
                .Where(x => x.Id == id && x.IsDeleted == false)
                .Select(x => new CargoDetailDetailsVm
                {
                    Id = x.Id,
                    Sequence = x.Sequence,
                    CargoDescription = x.CargoDescription,
                    PackageTypeName = x.PackageType != null ? x.PackageType.PackageTypeName : null,
                    PackageCount = x.PackageCount,
                    //InnerPackageTypeName = x.InnerPackageType != null ? x.InnerPackageType.PackageTypeName : null,
                    InnerPackageCount = x.InnerPackageCount,
                    GrossWeight = x.GrossWeight,
                    NetWeight = x.NetWeight,
                    WeightUnitName = x.WeightUnit != null ? x.WeightUnit.MeasurementUnitName : null,
                    Volume = x.Volume,
                    VolumeUnitName = x.VolumeUnit != null ? x.VolumeUnit.MeasurementUnitName : null,
                    IsOverSized = x.IsOverSized,
                    Length = x.Length,
                    Width = x.Width,
                    Height = x.Height,
                    DimensionUnitName = x.DimensionUnit != null ? x.DimensionUnit.MeasurementUnitName : null,
                    //HSCode = x.CountryHSCode != null ? x.CountryHSCode.CHSCode : x.HSCodeToSave,
                    HSCode = x.HSCodeToSave,
                    IsDG = x.IsDG,
                    DGClassName = x.DGClass != null ? x.DGClass.DGClassName : null,
                    UNNumber = x.UNNumber,
                    DGFlashPoint = x.DGFlashPoint,
                    DGPackingGroupName = x.DGPackingGroup != null ? x.DGPackingGroup.DGPackingGroupCode : null,
                    IsTemperatureControlled = x.IsTemperatureControlled,
                    MinTemp = x.MinTemp,
                    NormalTemp = x.NormalTemp,
                    MaxTemp = x.MaxTemp,
                    TempUnitName = x.TempUnit != null ? x.TempUnit.MeasurementUnitName : null,
                    MarksAndNumbers = x.MarksAndNumbers,
                    Remarks = x.Remarks,
                    IsFragile = x.IsFragile,
                    IsPerishable = x.IsPerishable,
                    IsRotateable = x.IsRotateable,
                    IsStackable = x.IsStackable,
                    MarinePolutant = x.MarinePolutant,
                    MSDSPageNumber = x.MSDSPageNumber,
                    PSN = x.PSN,
                })
                .FirstOrDefaultAsync();
        }

        public async Task<CargoDetailDeleteVm?> BuildDeleteModelAsync(Guid id)
        {
            var entity = await _context.CargoDetails
                .AsNoTracking()
                .FirstOrDefaultAsync(x => x.Id == id && x.IsDeleted == false);

            if (entity == null)
                return null;

            return new CargoDetailDeleteVm
            {
                Id = entity.Id,
                Sequence = entity.Sequence,
                CargoDescription = entity.CargoDescription,
                Filter = new CargoDetailFilterVm
                {
                    JobId = entity.JobId,
                    BLId = entity.BLId,
                    TransitShipmentAssignmentId = entity.TransitShipmentAssignmentId,
                    RateRequestId = entity.RateRequestId,
                }
            };
        }

        public async Task CreateAsync(CargoDetailFormVm model, Guid userId)
        {
            NormalizeConditionalFields(model);

            var entity = new CargoDetail
            {
                Id = Guid.CreateVersion7(),

                JobId = model.JobId,
                BLId = model.BLId,
                //BLContainerId = model.BLContainerId,
                TransitShipmentAssignmentId = model.TransitShipmentAssignmentId,
                RateRequestId = model.RateRequestId,
                BookingId = model.BookingId,
                BookingLegId = model.BookingLegId,
                Sequence = model.Sequence,
                PackageTypeId = model.PackageTypeId,
                PackageCount = model.PackageCount,
                InnerPackageTypeId = model.InnerPackageTypeId,
                InnerPackageCount = model.InnerPackageCount,
                GrossWeight = model.GrossWeight,
                NetWeight = model.NetWeight,
                WeightUnitId = model.WeightUnitId,
                Volume = model.Volume,
                VolumeUnitId = model.VolumeUnitId,
                IsOverSized = model.IsOverSized,
                Length = model.Length,
                Width = model.Width,
                Height = model.Height,
                DimensionUnitId = model.DimensionUnitId,
                //CountryHSCodeId = model.CountryHSCodeId,
                HSCodeToSave = model.HSCodeToSave,
                IsDG = model.IsDG,
                DGClassId = model.DGClassId,
                UNNumber = model.UNNumber,
                DGFlashPoint = model.DGFlashPoint,
                DGPackingGroupId = model.DGPackingGroupId,
                IsTemperatureControlled = model.IsTemperatureControlled,
                MinTemp = model.MinTemp,
                NormalTemp = model.NormalTemp,
                MaxTemp = model.MaxTemp,
                TempUnitId = model.TempUnitId,
                CargoDescription = model.CargoDescription,
                MarksAndNumbers = model.MarksAndNumbers,
                Remarks = model.Remarks,
                IsFragile = model.IsFragile,
                IsPerishable = model.IsPerishable,
                IsRotateable = model.IsRotateable,
                IsStackable = model.IsStackable,
                CargoCommodityId = model.CargoCommodityId,
                ChargeableWeight = model.ChargeableWeight,
                MarinePolutant = model.MarinePolutant,
                PSN = model.PSN,
                MSDSPageNumber = model.MSDSPageNumber,
                IsDeleted = false,
                CreatedAt = DateTime.Now,
                CreateUserId = userId
            };
            _context.CargoDetails.Add(entity);
            await _context.SaveChangesAsync();

            if (model.JobId != null && model.JobId != Guid.Empty)
            {
                await _activityLogService.LogHistoryAsync(
                    new CreateActivityLogRequest
                    {
                        ActivityCategoryId = 2,
                        ActivityDateTime = DateTime.Now,
                        Title = "Cargo Details Created",
                        Description = "Cargo Details Created",
                        EntityTypeId = 1,
                        EntityId = model.JobId.Value,
                        PerformedByUserId = userId,
                        IsSystemGenerated = true,
                    }
                    );
            }
        }
        public async Task<bool> UpdateAsync(CargoDetailFormVm model, Guid userId)
        {
            if (!model.Id.HasValue)
                return false;

            NormalizeConditionalFields(model);


            var entity = await _context.CargoDetails
                .FirstOrDefaultAsync(x => x.Id == model.Id.Value && x.IsDeleted == false);

            if (entity == null)
                return false;

            entity.Sequence = model.Sequence;
            entity.PackageTypeId = model.PackageTypeId;
            entity.PackageCount = model.PackageCount;
            entity.InnerPackageTypeId = model.InnerPackageTypeId;
            entity.InnerPackageCount = model.InnerPackageCount;
            entity.GrossWeight = model.GrossWeight;
            entity.NetWeight = model.NetWeight;
            entity.ChargeableWeight = model.ChargeableWeight;
            entity.WeightUnitId = model.WeightUnitId;
            entity.Volume = model.Volume;
            entity.VolumeUnitId = model.VolumeUnitId;
            entity.IsOverSized = model.IsOverSized;
            entity.Length = model.Length;
            entity.Width = model.Width;
            entity.Height = model.Height;
            entity.DimensionUnitId = model.DimensionUnitId;
            //entity.CountryHSCodeId = model.CountryHSCodeId;
            entity.HSCodeToSave = model.HSCodeToSave;
            entity.IsDG = model.IsDG;
            entity.DGClassId = model.DGClassId;
            entity.UNNumber = model.UNNumber;
            entity.DGFlashPoint = model.DGFlashPoint;
            entity.DGPackingGroupId = model.DGPackingGroupId;
            entity.IsTemperatureControlled = model.IsTemperatureControlled;
            entity.MinTemp = model.MinTemp;
            entity.NormalTemp = model.NormalTemp;
            entity.MaxTemp = model.MaxTemp;
            entity.TempUnitId = model.TempUnitId;
            entity.CargoDescription = model.CargoDescription;
            entity.MarksAndNumbers = model.MarksAndNumbers;
            entity.Remarks = model.Remarks;
            entity.IsFragile = model.IsFragile;
            entity.IsPerishable = model.IsPerishable;
            entity.IsRotateable = model.IsRotateable;
            entity.IsStackable = model.IsStackable;
            entity.CargoCommodityId = model.CargoCommodityId;
            entity.PSN = model.PSN;
            entity.MarinePolutant = model.MarinePolutant;
            entity.MSDSPageNumber = model.MSDSPageNumber;
            entity.UpdatedAt = DateTime.Now;
            entity.EditUserId = userId;
            await _context.SaveChangesAsync();


            Guid entityId = Guid.Empty;
            int entityTypeId = 1;
            if (model.JobId != null && model.JobId != Guid.Empty)
            {
                entityId = model.JobId.Value;
                entityTypeId = 1;
            }
            if (model.RateRequestId != null && model.RateRequestId != Guid.Empty)
            {
                entityId = model.RateRequestId.Value;
                entityTypeId = 1;
            }

            await _activityLogService.LogHistoryAsync(
                new CreateActivityLogRequest
                {
                    ActivityCategoryId = 2,
                    ActivityDateTime = DateTime.Now,
                    Title = "Cargo Details Updated",
                    Description = "Cargo Details Edited and Updated!!!!",
                    EntityTypeId = entityTypeId,
                    EntityId = entityId,
                    PerformedByUserId = userId,
                    IsSystemGenerated = true,
                }
                );

            return true;
        }

        public async Task<CargoDetailFilterVm?> DeleteAsync(Guid id, Guid userId)
        {
            var entity = await _context.CargoDetails
                .FirstOrDefaultAsync(x => x.Id == id && x.IsDeleted == false);

            if (entity == null)
                return null;

            var filter = new CargoDetailFilterVm
            {
                JobId = entity.JobId,
                BLId = entity.BLId,
                TransitShipmentAssignmentId = entity.TransitShipmentAssignmentId
            };

            entity.IsDeleted = true;
            entity.UpdatedAt = DateTime.Now;
            entity.EditUserId = userId;

            await _context.SaveChangesAsync();

            return filter;
        }

        public async Task FillLookupsAsync(CargoDetailFormVm model)
        {
            model.PackageTypes = await _context.PackageTypes
                .AsNoTracking()
                .Where(x => x.IsActive && !x.IsDeleted)
                .OrderBy(x => x.PackageTypeName)
                .Select(x => new SelectListItem
                {
                    Value = x.Id.ToString(),
                    Text = x.PackageTypeName
                })
                .ToListAsync();

            model.InnerPackageTypes = model.PackageTypes.ToList();
            model.WeightUnits = await _lookupService.GetWeightUnitsAsync();
            model.VolumeUnits = await _lookupService.GetVolumeUnitsAsync();
            model.DimensionUnits = await _lookupService.GetDimUnitsAsync();
            model.TempUnits = await _lookupService.GetTempUnitsAsync();
            model.CargoCommodities = await _lookupService.GetCargoCommoditiesAsync();
            //model.CountryHSCodes = await _context.CountryHSCodes
            //    .Include(x => x.Country)
            //    .AsNoTracking()
            //    .Where(x => x.IsActive && !x.IsDeleted)
            //    .OrderBy(x => x.CHSCode)
            //    .Select(x => new SelectListItem
            //    {
            //        Value = x.Id.ToString(),
            //        Text = x.CHSCode + "(" + x.Country.CountryIsoCode2 + ")" + "-" + x.CHSName.Substring(1, 30) + "-" + x.LocalName.Substring(1, 30)
            //    })
            //    .ToListAsync();

            model.DGClasses = await _context.DGClasses
                .AsNoTracking()
                .Where(x => x.IsActive && !x.IsDeleted)
                .OrderBy(x => x.DGClassNo)
                .Select(x => new SelectListItem
                {
                    Value = x.Id.ToString(),
                    Text = x.DGClassNo + " - " + x.DGClassName
                })
                .ToListAsync();

            model.DGPackingGroups = await _context.DGPackingGroups
                .AsNoTracking()
                .Where(x => x.IsActive && !x.IsDeleted)
                .OrderBy(x => x.DGPackingGroupCode)
                .Select(x => new SelectListItem
                {
                    Value = x.Id.ToString(),
                    Text = x.DGPackingGroupCode
                })
                .ToListAsync();
        }

        private static IQueryable<CargoDetail> ApplyFilter(
            IQueryable<CargoDetail> query,
            CargoDetailFilterVm filter)
        {
            if (filter.JobId.HasValue)
                query = query.Where(x => x.JobId == filter.JobId.Value);

            if (filter.BLId.HasValue)
                query = query.Where(x => x.BLId == filter.BLId.Value);

            if (filter.BookingId.HasValue)
                query = query.Where(x => x.BookingId == filter.BookingId.Value);

            //if (filter.BLContainerId.HasValue)
            //    query = query.Where(x => x.BLContainerId == filter.BLContainerId.Value);

            if (filter.TransitShipmentAssignmentId.HasValue)
                query = query.Where(x => x.TransitShipmentAssignmentId == filter.TransitShipmentAssignmentId.Value);

            if (filter.RateRequestId.HasValue)
                query = query.Where(x => x.RateRequestId == filter.RateRequestId.Value);

            return query;
        }


        private void NormalizeConditionalFields(CargoDetailFormVm model)
        {
            if (!model.IsDG)
            {
                model.DGClassId = null;
                model.UNNumber = null;
                model.DGFlashPoint = null;
                model.DGPackingGroupId = null;
            }

            if (!model.IsTemperatureControlled)
            {
                model.MinTemp = null;
                model.NormalTemp = null;
                model.MaxTemp = null;
                model.TempUnitId = null;
            }
        }

        public async Task<bool> PushDataFromJobToBookingAsync(CargoDetailFilterVm vm)
        {
            var booking = await _context.Bookings.FindAsync(vm.BookingId.Value);
            var sourceCargoDetails = await _context.CargoDetails
                .AsNoTracking()
                .Where(cd => cd.JobId == booking.JobId && !cd.IsDeleted)
                .ToListAsync();

            if (!sourceCargoDetails.Any()) return false;
            var newCargoDetails = new List<CargoDetail>();
            foreach (var source in sourceCargoDetails)
            {
                var replica = new CargoDetail
                {
                    Id = Guid.CreateVersion7(),
                    JobId = null,
                    BookingId = vm.BookingId,
                    Sequence = source.Sequence,
                    PackageTypeId = source.PackageTypeId,
                    PackageCount = source.PackageCount,
                    GrossWeight = source.GrossWeight,
                    CargoDescription = source.CargoDescription,
                    IsDG = source.IsDG,
                    CreatedAt = DateTime.Now,
                    CreateUserId = source.CreateUserId,
                    CargoCommodityId = source.CargoCommodityId,
                    ChargeableWeight = source.ChargeableWeight,
                    CountryHSCodeId = source.CountryHSCodeId,
                    DGClassId = source.DGClassId,
                    DGPackingGroupId = source.DGPackingGroupId,
                    DGFlashPoint = source.DGFlashPoint,
                    DimensionUnitId = source.DimensionUnitId,
                    Height = source.Height,
                    HSCodeToSave = source.HSCodeToSave,
                    InnerPackageCount = source.InnerPackageCount,
                    InnerPackageTypeId = source.InnerPackageTypeId,
                    IsFragile = source.IsFragile,
                    IsPerishable = source.IsPerishable,
                    IsOverSized = source.IsOverSized,
                    IsStackable = source.IsStackable,
                    IsRotateable = source.IsRotateable,
                    IsDeleted = false,
                    UNNumber = source.UNNumber,
                    MSDSPageNumber = source.MSDSPageNumber,
                    WeightUnitId = source.WeightUnitId,
                    VolumeUnitId = source.VolumeUnitId,
                    Volume = source.Volume,
                    Remarks = source.Remarks,
                    NormalTemp = source.NormalTemp,
                    Length = source.Length,
                    NetWeight = source.NetWeight,
                    MaxTemp = source.MaxTemp,
                    MinTemp = source.MinTemp,
                    TempUnitId = source.TempUnitId,
                    PSN = source.PSN,
                    MarksAndNumbers = source.MarksAndNumbers,
                    MarinePolutant = source.MarinePolutant,
                    IsTemperatureControlled = source.IsTemperatureControlled,
                };
                newCargoDetails.Add(replica);
            }
            // ۳. افزودن لیست جدید به Context
            await _context.CargoDetails.AddRangeAsync(newCargoDetails);
            // ۴. ذخیره تغییرات
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> PushDataFromJobToBLAsync(CargoDetailFilterVm vm)
        {
            var bl = await _context.BLs.FindAsync(vm.BLId.Value);
            var sourceCargoDetails = await _context.CargoDetails
                .AsNoTracking()
                .Where(cd => cd.JobId == bl.JobId && !cd.IsDeleted)
                .ToListAsync();

            if (!sourceCargoDetails.Any()) return false;
            var newCargoDetails = new List<CargoDetail>();
            foreach (var source in sourceCargoDetails)
            {
                var replica = new CargoDetail
                {
                    Id = Guid.CreateVersion7(),
                    JobId = null,
                    BLId = vm.BLId,
                    Sequence = source.Sequence,
                    PackageTypeId = source.PackageTypeId,
                    PackageCount = source.PackageCount,
                    GrossWeight = source.GrossWeight,
                    CargoDescription = source.CargoDescription,
                    IsDG = source.IsDG,
                    CreatedAt = DateTime.Now,
                    CreateUserId = source.CreateUserId,
                    CargoCommodityId = source.CargoCommodityId,
                    ChargeableWeight = source.ChargeableWeight,
                    CountryHSCodeId = source.CountryHSCodeId,
                    DGClassId = source.DGClassId,
                    DGPackingGroupId = source.DGPackingGroupId,
                    DGFlashPoint = source.DGFlashPoint,
                    DimensionUnitId = source.DimensionUnitId,
                    Height = source.Height,
                    HSCodeToSave = source.HSCodeToSave,
                    InnerPackageCount = source.InnerPackageCount,
                    InnerPackageTypeId = source.InnerPackageTypeId,
                    IsFragile = source.IsFragile,
                    IsPerishable = source.IsPerishable,
                    IsOverSized = source.IsOverSized,
                    IsStackable = source.IsStackable,
                    IsRotateable = source.IsRotateable,
                    IsDeleted = false,
                    UNNumber = source.UNNumber,
                    MSDSPageNumber = source.MSDSPageNumber,
                    WeightUnitId = source.WeightUnitId,
                    VolumeUnitId = source.VolumeUnitId,
                    Volume = source.Volume,
                    Remarks = source.Remarks,
                    NormalTemp = source.NormalTemp,
                    Length = source.Length,
                    NetWeight = source.NetWeight,
                    MaxTemp = source.MaxTemp,
                    MinTemp = source.MinTemp,
                    TempUnitId = source.TempUnitId,
                    PSN = source.PSN,
                    MarksAndNumbers = source.MarksAndNumbers,
                    MarinePolutant = source.MarinePolutant,
                    IsTemperatureControlled = source.IsTemperatureControlled,
                };
                newCargoDetails.Add(replica);
            }
            await _context.CargoDetails.AddRangeAsync(newCargoDetails);
            await _context.SaveChangesAsync();
            return true;
        }
    }
}
