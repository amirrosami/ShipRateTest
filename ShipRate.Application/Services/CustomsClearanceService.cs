using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using ShipRate.Application.Interfaces;
using ShipRate.Application.ViewModels;
using ShipRate.Domain.Entities.Customs;
using ShipRate.Infrastructure.Data;

namespace ShipRate.Application.Services
{
    public class CustomsClearanceService : ICustomsClearanceService
    {
        private readonly ApplicationDbContext _db;
        private readonly ICurrentUserService _currentUserService;

        public CustomsClearanceService(ApplicationDbContext db, ICurrentUserService currentUserService)
        {
            _db = db;
            _currentUserService = currentUserService;
        }

        public async Task<CustomsClearanceJobTabVm> GetJobTabAsync(Guid jobId)
        {
            var items = await _db.CustomsClearances
                .AsNoTracking()
                .Where(x => x.JobId == jobId)
                .OrderByDescending(x => x.DeclarationDate)
                .Select(x => new CustomsClearanceListItemVm
                {
                    Id = x.Id,
                    DeclarationNumber = x.DeclarationNumber,
                    DeclarationTypeTitle = x.DeclarationType != null ? x.DeclarationType.CustomsDeclarationTypeName : "",
                    CustomOfficeName = x.CustomOffice != null ? x.CustomOffice.LocationName : "",
                    ImporterExporterName = x.ImporterExporter != null ? x.ImporterExporter.PersonName : "",
                    DeclarationDate = x.DeclarationDate,
                    StatusTitle = x.CustomsStatus != null ? x.CustomsStatus.CustomsStatusName : ""
                })
                .ToListAsync();

            return new CustomsClearanceJobTabVm
            {
                JobId = jobId,
                Items = items
            };
        }

        public async Task<CustomsClearanceCreateEditVm> BuildCreateVmAsync(Guid jobId, int companyId)
        {
            return new CustomsClearanceCreateEditVm
            {
                JobId = jobId,
                CompanyId = companyId,
                DeclarationTypes = await GetDeclarationTypesAsync(),
                CustomOffices = await GetCustomOfficesAsync(),
                Persons = await GetPersonsAsync()
            };
        }

        public Task<CustomsClearanceCreateEditVm?> BuildEditVmAsync(Guid id, int companyId)
            => BuildVmAsync(id, companyId);



        public async Task CreateAsync(CustomsClearanceCreateEditVm model)
        {
            var draftStatusId = await _db.CustomsStatuses
                .Where(x => x.CustomsStatusName == "Draft")
                .Select(x => x.Id)
                .FirstAsync();

            var entity = new CustomsClearance
            {
                Id = Guid.CreateVersion7(),
                JobId = model.JobId,
                CompanyId = model.CompanyId,
                DeclarationTypeId = model.DeclarationTypeId,
                CustomOfficeId = model.CustomOfficeId,
                ImporterExporterId = model.ImporterExporterId!.Value,
                CustomsBrokerId = model.CustomsBrokerId!.Value,
                DeclarationNumber = model.DeclarationNumber,
                DeclarationDate = model.DeclarationDate,
                ReferenceNumber = model.ReferenceNumber,
                Remarks = model.Remarks,
                CustomsStatusId = draftStatusId,
                DeclarationVersion = 1,
                CreatedAt = DateTime.Now,
                CreateUserId = _currentUserService.UserId,
                CIFValue = 0,
                FOBValue = 0,
                FreightValue = 0,
                InsuranceValue = 0,
                CustomsValue = 0,
                TotalDutyAmount = 0,
                TotalTaxAmount = 0,
                TotalOtherChargesAmount = 0,
                GrandTotalAmount = 0,
                GoodsDescription = "N/A",

            };

            _db.CustomsClearances.Add(entity);
            await _db.SaveChangesAsync();
        }

        public async Task UpdateAsync(CustomsClearanceCreateEditVm model)
        {
            var entity = await _db.CustomsClearances
                .FirstOrDefaultAsync(x => x.Id == model.Id && x.CompanyId == model.CompanyId);

            if (entity == null)
                return;

            entity.DeclarationTypeId = model.DeclarationTypeId;
            entity.CustomOfficeId = model.CustomOfficeId;
            entity.ImporterExporterId = model.ImporterExporterId!.Value;
            entity.CustomsBrokerId = model.CustomsBrokerId!.Value;
            entity.DeclarationNumber = model.DeclarationNumber;
            entity.DeclarationDate = model.DeclarationDate;
            entity.ReferenceNumber = model.ReferenceNumber;
            entity.Remarks = model.Remarks;

            await _db.SaveChangesAsync();
        }

        public async Task DeleteAsync(Guid id, int companyId)
        {
            var entity = await _db.CustomsClearances
                .FirstOrDefaultAsync(x => x.Id == id && x.CompanyId == companyId);

            if (entity == null)
                return;

            _db.CustomsClearances.Remove(entity);
            await _db.SaveChangesAsync();
        }

        private async Task<IEnumerable<SelectListItem>> GetDeclarationTypesAsync()
        {
            return await _db.CustomsDeclarationTypes
                .AsNoTracking()
                .OrderBy(x => x.Id)
                .Select(x => new SelectListItem
                {
                    Value = x.Id.ToString(),
                    Text = x.CustomsDeclarationTypeName
                })
                .ToListAsync();
        }

        private async Task<IEnumerable<SelectListItem>> GetCustomOfficesAsync()
        {
            return await _db.Locations
                .Where(x => x.LocationTypeId == 5)
                .AsNoTracking()
                .OrderBy(x => x.LocationName)
                .Select(x => new SelectListItem
                {
                    Value = x.Id.ToString(),
                    Text = x.LocationName
                })
                .ToListAsync();
        }

        private async Task<IEnumerable<SelectListItem>> GetPersonsAsync()
        {
            return await _db.Persons
                .AsNoTracking()
                .OrderBy(x => x.PersonName)
                .Select(x => new SelectListItem
                {
                    Value = x.Id.ToString(),
                    Text = x.PersonName
                })
                .ToListAsync();
        }


        public async Task<CustomsClearanceDetailsVm?> GetDetailsAsync(Guid id, int companyId)
        {
            return await _db.CustomsClearances
                .AsNoTracking()
                .Include(x => x.Job)
                .Include(x => x.ImporterExporter)
                .Include(x => x.CustomsStatus)
                .Where(x => x.Id == id && x.CompanyId == companyId)
                .Select(x => new CustomsClearanceDetailsVm
                {
                    Id = x.Id,
                    JobId = x.JobId,
                    JobNumber = x.Job != null ? x.Job.JobNumber : "",
                    DeclarationNumber = x.DeclarationNumber,
                    StatusTitle = x.CustomsStatus != null ? x.CustomsStatus.CustomsStatusName : "N/A",
                    ImporterExporterName = x.ImporterExporter != null ? x.ImporterExporter.PersonName : "N/A",
                    GrandTotalAmount = x.GrandTotalAmount
                })
                .FirstOrDefaultAsync();
        }

        public Task<CustomsClearanceCreateEditVm?> GetGeneralTabVmAsync(Guid id, int companyId)
            => BuildVmAsync(id, companyId);



        public async Task UpdateGeneralInfoAsync(CustomsClearanceCreateEditVm model, int companyId)
        {
            var entity = await _db.CustomsClearances
                .FirstOrDefaultAsync(x => x.Id == model.Id && x.CompanyId == companyId);

            if (entity == null)
                throw new KeyNotFoundException("Customs clearance not found.");

            // فقط فیلدهای مجاز در General Tab
            entity.DeclarationTypeId = model.DeclarationTypeId;
            entity.CustomOfficeId = model.CustomOfficeId;
            entity.ImporterExporterId = model.ImporterExporterId.Value;
            entity.CustomsBrokerId = model.CustomsBrokerId.Value;
            entity.DeclarationNumber = model.DeclarationNumber;
            entity.DeclarationDate = model.DeclarationDate;
            entity.ReferenceNumber = model.ReferenceNumber;
            entity.Remarks = model.Remarks;

            entity.UpdatedAt = DateTime.Now;
            entity.EditUserId = _currentUserService.UserId;

            await _db.SaveChangesAsync();
        }
        private async Task<CustomsClearanceCreateEditVm?> BuildVmAsync(Guid id, int companyId)
        {
            var vm = await _db.CustomsClearances
                .AsNoTracking()
                .Where(x => x.Id == id && x.CompanyId == companyId)
                .Select(x => new CustomsClearanceCreateEditVm
                {
                    Id = x.Id,
                    JobId = x.JobId,
                    CompanyId = x.CompanyId,
                    DeclarationTypeId = x.DeclarationTypeId,
                    CustomOfficeId = x.CustomOfficeId,
                    ImporterExporterId = x.ImporterExporterId,
                    CustomsBrokerId = x.CustomsBrokerId,
                    DeclarationNumber = x.DeclarationNumber,
                    DeclarationDate = x.DeclarationDate,
                    ReferenceNumber = x.ReferenceNumber,
                    Remarks = x.Remarks,
                }).FirstOrDefaultAsync();
            vm.CustomOffices = await GetCustomOfficesAsync();
            vm.DeclarationTypes = await GetDeclarationTypesAsync();
            vm.Persons = await GetPersonsAsync();
            return vm;
        }
    }







    public class CustomsClearanceItemService : ICustomsClearanceItemService
    {
        private readonly ApplicationDbContext _db;
        private readonly IActivityLogService _activityLogService;

        public CustomsClearanceItemService(ApplicationDbContext db, IActivityLogService activityLogService)
        {
            _db = db;
            _activityLogService = activityLogService;
        }
        public async Task PopulateLookupsAsync(CustomsClearanceItemCreateEditVm vm, int companyId)
        {
            await FillLookupsAsync(vm, companyId);
        }
        public async Task<CustomsClearanceItemsTabVm> GetItemsTabAsync(Guid customsClearanceId, int companyId)
        {
            var items = await _db.CustomsClearanceItems
                .AsNoTracking()
                .Where(x => x.CustomsClearanceId == customsClearanceId)
                .OrderBy(x => x.ItemSequence)
                .Select(x => new CustomsClearanceItemListItemVm
                {
                    Id = x.Id,
                    ItemSequence = x.ItemSequence,
                    HSCodeToSave = x.HSCodeToSave,
                    GoodsDescriptionEN = x.GoodsDescriptionEN ?? string.Empty,
                    GoodsDescriptionLocal = x.GoodsDescriptionLocal,
                    Quantity = x.Quantity,
                    MeasurementUnitName = x.MeasurementUnitId != null
                        ? _db.MeasurementUnits
                            .Where(mu => mu.Id == x.MeasurementUnitId)
                            .Select(mu => mu.MeasurementUnitName)
                            .FirstOrDefault()
                        : null,
                    UnitPrice = x.UnitPrice,
                    CurrencyCode = x.CurrencyId != null
                        ? _db.Currencies
                            .Where(c => c.Id == x.CurrencyId)
                            .Select(c => c.CurrencyCode)
                            .FirstOrDefault()
                        : null,
                    TotalValue = x.TotalValue,
                    TotalDutiesItem = x.TotalDutiesItem
                })
                .ToListAsync();

            return new CustomsClearanceItemsTabVm
            {
                CustomsClearanceId = customsClearanceId,
                Items = items
            };
        }

        public async Task<CustomsClearanceItemCreateEditVm> BuildCreateVmAsync(Guid customsClearanceId, int companyId)
        {
            await EnsureClearanceExistsAsync(customsClearanceId, companyId);

            var nextSequence = (await _db.CustomsClearanceItems
                .Where(x => x.CustomsClearanceId == customsClearanceId)
                .Select(x => (int?)x.ItemSequence)
                .MaxAsync()) ?? 0;

            var vm = new CustomsClearanceItemCreateEditVm
            {
                CustomsClearanceId = customsClearanceId,
                ItemSequence = nextSequence + 1,
                Quantity = 1
            };

            await FillLookupsAsync(vm, companyId);
            return vm;
        }

        public async Task<CustomsClearanceItemCreateEditVm?> BuildEditVmAsync(Guid id, int companyId)
        {
            var vm = await _db.CustomsClearanceItems
                .AsNoTracking()
                .Where(x => x.Id == id)
                .Select(x => new CustomsClearanceItemCreateEditVm
                {
                    Id = x.Id,
                    CustomsClearanceId = x.CustomsClearanceId,
                    ItemSequence = x.ItemSequence,
                    CountryHSCodeId = x.CountryHSCodeId,
                    HSCodeToSave = x.HSCodeToSave,
                    GoodsDescriptionEN = x.GoodsDescriptionEN ?? string.Empty,
                    GoodsDescriptionLocal = x.GoodsDescriptionLocal,
                    Quantity = x.Quantity,
                    MeasurementUnitId = x.MeasurementUnitId,
                    StatisticalQuantity = x.StatisticalQuantity,
                    StatisticalMeasurementUnitId = x.StatisticalMeasurementUnitId,
                    UnitPrice = x.UnitPrice,
                    CurrencyId = x.CurrencyId,
                    TotalValue = x.TotalValue,
                    Remarks = x.Remarks
                })
                .FirstOrDefaultAsync();

            if (vm == null)
                return null;

            await FillLookupsAsync(vm, companyId);
            return vm;
        }

        public async Task CreateAsync(CustomsClearanceItemCreateEditVm model, int companyId)
        {
            await EnsureClearanceIsEditableAsync(model.CustomsClearanceId, companyId);

            var entity = new CustomsClearanceItem
            {
                Id = Guid.CreateVersion7(),
                CustomsClearanceId = model.CustomsClearanceId,
                ItemSequence = model.ItemSequence,
                CountryHSCodeId = model.CountryHSCodeId,
                HSCodeToSave = model.HSCodeToSave.Trim(),
                GoodsDescriptionEN = model.GoodsDescriptionEN.Trim(),
                GoodsDescriptionLocal = model.GoodsDescriptionLocal?.Trim(),
                Quantity = model.Quantity,
                MeasurementUnitId = model.MeasurementUnitId,
                StatisticalQuantity = model.StatisticalQuantity,
                StatisticalMeasurementUnitId = model.StatisticalMeasurementUnitId,
                UnitPrice = model.UnitPrice,
                CurrencyId = model.CurrencyId,
                TotalValue = ResolveTotalValue(model.Quantity, model.UnitPrice, model.TotalValue),
                Remarks = model.Remarks?.Trim(),

                // Phase 1 defaults
                CustomsValue = null,
                CustomsValueIRR = null,
                DutyRate = 0,
                DutyAmount = 0,
                CommercialBenefitRate = 0,
                CommercialBenefitAmount = 0,
                VATRate = 0,
                VATAmount = 0,
                OtherCharges = 0,
                TotalDutiesItem = 0,
                PreferentialTariffCode = null,
                PreferentialDutyRate = null,
                CertificateOfOriginRef = null
            };

            _db.CustomsClearanceItems.Add(entity);
            await _db.SaveChangesAsync();

            await _activityLogService.LogEventAsync(new CreateActivityLogRequest
            {
                EntityTypeId = 14,
                EntityId = entity.CustomsClearanceId,
                Title = "Item added",
                Description = $"Item #{entity.ItemSequence} added: {entity.GoodsDescriptionEN}",
                ActivityType = "ItemCreated",
                Severity = "success",
                IsSystemGenerated = false,
                ReferenceNo = entity.HSCodeToSave
            });










        }

        public async Task UpdateAsync(CustomsClearanceItemCreateEditVm model, int companyId)
        {
            if (!model.Id.HasValue)
                throw new InvalidOperationException("Item id is required.");

            var entity = await _db.CustomsClearanceItems
                .FirstOrDefaultAsync(x => x.Id == model.Id.Value);

            if (entity == null)
                throw new InvalidOperationException("Customs clearance item not found.");

            await EnsureClearanceIsEditableAsync(entity.CustomsClearanceId, companyId);

            entity.ItemSequence = model.ItemSequence;
            entity.CountryHSCodeId = model.CountryHSCodeId;
            entity.HSCodeToSave = model.HSCodeToSave.Trim();
            entity.GoodsDescriptionEN = model.GoodsDescriptionEN.Trim();
            entity.GoodsDescriptionLocal = model.GoodsDescriptionLocal?.Trim();
            entity.Quantity = model.Quantity;
            entity.MeasurementUnitId = model.MeasurementUnitId;
            entity.StatisticalQuantity = model.StatisticalQuantity;
            entity.StatisticalMeasurementUnitId = model.StatisticalMeasurementUnitId;
            entity.UnitPrice = model.UnitPrice;
            entity.CurrencyId = model.CurrencyId;
            entity.TotalValue = ResolveTotalValue(model.Quantity, model.UnitPrice, model.TotalValue);
            entity.Remarks = model.Remarks?.Trim();




            //var hsCode = await _db.CountryHSCodes
            //    .Where(x => x.Id == model.CountryHSCodeId)
            //    .Select(x => x.HSCode)
            //    .FirstOrDefaultAsync();

            //if (string.IsNullOrWhiteSpace(hsCode.))
            //    throw new InvalidOperationException("Selected HS code was not found.");

            //entity.HSCodeToSave = string.IsNullOrWhiteSpace(model.HSCodeToSave)
            //    ? hsCode
            //    : model.HSCodeToSave.Trim();




            await _db.SaveChangesAsync();
            await _activityLogService.LogEventAsync(new CreateActivityLogRequest
            {
                EntityTypeId = 14,
                EntityId = entity.CustomsClearanceId,
                Title = "Item updated",
                Description = $"Item #{entity.ItemSequence} was updated.",
                ActivityType = "ItemCreated",
                Severity = "success",
                IsSystemGenerated = false,
                ReferenceNo = entity.HSCodeToSave,
                FieldName = "GoodsDescriptionEN",
                NewValue = entity.GoodsDescriptionEN
            });
        }

        public async Task DeleteAsync(Guid id, int companyId)
        {
            var entity = await _db.CustomsClearanceItems
                .FirstOrDefaultAsync(x => x.Id == id);

            if (entity == null)
                return;

            await EnsureClearanceIsEditableAsync(entity.CustomsClearanceId, companyId);

            _db.CustomsClearanceItems.Remove(entity);
            await _db.SaveChangesAsync();
        }

        private async Task FillLookupsAsync(CustomsClearanceItemCreateEditVm vm, int companyId)
        {
            vm.CountryHsCodes = await _db.CountryHSCodes
                .AsNoTracking()
                .OrderBy(x => x.HSCode)
                .Select(x => new SelectListItem
                {
                    Value = x.Id.ToString(),
                    Text = x.HSCode + " - " + (x.CHSCode ?? x.CHSName ?? "")
                })
                .ToListAsync();

            vm.MeasurementUnits = await _db.MeasurementUnits
                .AsNoTracking()
                .OrderBy(x => x.MeasurementUnitName)
                .Select(x => new SelectListItem
                {
                    Value = x.Id.ToString(),
                    Text = x.MeasurementUnitName
                })
                .ToListAsync();

            vm.StatisticalMeasurementUnits = await _db.MeasurementUnits
                .AsNoTracking()
                .OrderBy(x => x.MeasurementUnitName)
                .Select(x => new SelectListItem
                {
                    Value = x.Id.ToString(),
                    Text = x.MeasurementUnitName
                })
                .ToListAsync();

            vm.Currencies = await _db.Currencies
                .AsNoTracking()
                .OrderBy(x => x.Id)
                .Select(x => new SelectListItem
                {
                    Value = x.Id.ToString(),
                    Text = x.CurrencyCode + " - " + x.CurrencyName
                })
                .ToListAsync();
        }

        private static decimal? ResolveTotalValue(decimal? quantity, decimal? unitPrice, decimal? totalValue)
        {
            if (totalValue.HasValue)
                return totalValue.Value;

            if (quantity.HasValue && unitPrice.HasValue)
                return quantity.Value * unitPrice.Value;

            return null;
        }

        private async Task EnsureClearanceExistsAsync(Guid customsClearanceId, int companyId)
        {
            var exists = await _db.CustomsClearances
                .AsNoTracking()
                .AnyAsync(x => x.Id == customsClearanceId && x.CompanyId == companyId);

            if (!exists)
                throw new InvalidOperationException("Customs clearance not found.");
        }

        private async Task EnsureClearanceIsEditableAsync(Guid customsClearanceId, int companyId)
        {
            var clearance = await _db.CustomsClearances
                .AsNoTracking()
                .Where(x => x.Id == customsClearanceId && x.CompanyId == companyId)
                .Select(x => new
                {
                    x.Id,
                    x.CustomsStatusId
                })
                .FirstOrDefaultAsync();

            if (clearance == null)
                throw new InvalidOperationException("Customs clearance not found.");

            // اگر Status شما enum/string متفاوتی دارد این شرط را با مدل خودتان align کنید
            if (clearance.CustomsStatusId != -1)
                throw new InvalidOperationException("Only draft customs clearance can be modified.");
        }
    }






    public class CustomsClearanceContainerService : ICustomsClearanceContainerService
    {
        private readonly ApplicationDbContext _db;

        public CustomsClearanceContainerService(ApplicationDbContext db)
        {
            _db = db;
        }

        public async Task<CustomsClearanceContainersTabVm> GetContainersTabAsync(Guid customsClearanceId, int companyId)
        {
            var containers = await _db.CustomsClearanceContainers
                .AsNoTracking()
                .Where(x => x.CustomsClearanceId == customsClearanceId)
                .OrderBy(x => x.ContainerNo)
                .Select(x => new CustomsClearanceContainerListItemVm
                {
                    Id = x.Id,
                    ContainerNo = x.ContainerNo,
                    ContainerSizeTypeName = x.ContainerSizeType.FullName,
                    SealNo = x.SealNo,
                    PackageTypeName = x.PackageType != null ? x.PackageType.PackageTypeName : null,
                    GrossWeightKg = x.GrossWeightKg,
                    NetWeightKg = x.NetWeightKg,
                    Remarks = x.Remarks
                })
                .ToListAsync();

            return new CustomsClearanceContainersTabVm
            {
                CustomsClearanceId = customsClearanceId,
                Containers = containers
            };
        }

        public async Task<CustomsClearanceContainerCreateEditVm> BuildCreateVmAsync(Guid customsClearanceId, int companyId)
        {
            await EnsureClearanceExistsAsync(customsClearanceId, companyId);

            var vm = new CustomsClearanceContainerCreateEditVm
            {
                CustomsClearanceId = customsClearanceId
            };

            await PopulateLookupsAsync(vm, companyId);
            return vm;
        }

        public async Task<CustomsClearanceContainerCreateEditVm?> BuildEditVmAsync(Guid id, int companyId)
        {
            var vm = await _db.CustomsClearanceContainers
                .AsNoTracking()
                .Where(x => x.Id == id)
                .Select(x => new CustomsClearanceContainerCreateEditVm
                {
                    Id = x.Id,
                    CustomsClearanceId = x.CustomsClearanceId,
                    ContainerId = x.ContainerId,
                    ContainerNo = x.ContainerNo,
                    ContainerSizeTypeId = x.ContainerSizeTypeId,
                    FullGateInContainerId = x.FullGateInContainerId,
                    VGMContainerId = x.VGMContainerId,
                    SealNo = x.SealNo,
                    PackageTypeId = x.PackageTypeId,
                    GrossWeightKg = x.GrossWeightKg,
                    NetWeightKg = x.NetWeightKg,
                    Remarks = x.Remarks
                })
                .FirstOrDefaultAsync();

            if (vm == null)
                return null;

            await PopulateLookupsAsync(vm, companyId);
            return vm;
        }

        public async Task PopulateLookupsAsync(CustomsClearanceContainerCreateEditVm vm, int companyId)
        {
            vm.Containers = await _db.Containers
                .AsNoTracking()
                .OrderBy(x => x.ContainerNumber)
                .Select(x => new SelectListItem
                {
                    Value = x.Id.ToString(),
                    Text = x.ContainerNumber
                })
                .ToListAsync();

            vm.ContainerSizeTypes = await _db.ContainerSizeTypes
                .AsNoTracking()
                .OrderBy(x => x.FullName)
                .Select(x => new SelectListItem
                {
                    Value = x.Id.ToString(),
                    Text = x.FullName
                })
                .ToListAsync();

            //vm.FullGateInContainers = await _db.gate
            //    .AsNoTracking()
            //    .OrderByDescending(x => x.Id)
            //    .Select(x => new SelectListItem
            //    {
            //        Value = x.Id.ToString(),
            //        Text = x.ContainerNo
            //    })
            //    .ToListAsync();

            vm.VgmContainers = await _db.VGMContainers
                .AsNoTracking()
                .OrderByDescending(x => x.Id)
                .Select(x => new SelectListItem
                {
                    Value = x.Id.ToString(),
                    Text = x.ContainerNo
                })
                .ToListAsync();

            vm.PackageTypes = await _db.PackageTypes
                .AsNoTracking()
                .OrderBy(x => x.PackageTypeName)
                .Select(x => new SelectListItem
                {
                    Value = x.Id.ToString(),
                    Text = x.PackageTypeName
                })
                .ToListAsync();
        }

        public async Task CreateAsync(CustomsClearanceContainerCreateEditVm model, int companyId)
        {
            await EnsureClearanceIsEditableAsync(model.CustomsClearanceId, companyId);
            await ValidateContainerAsync(model);

            var entity = new CustomsClearanceContainer
            {
                Id = Guid.CreateVersion7(),
                CustomsClearanceId = model.CustomsClearanceId,
                ContainerId = model.ContainerId,
                ContainerNo = model.ContainerNo.Trim().ToUpper(),
                ContainerSizeTypeId = model.ContainerSizeTypeId,
                FullGateInContainerId = model.FullGateInContainerId,
                VGMContainerId = model.VGMContainerId,
                SealNo = model.SealNo?.Trim(),
                PackageTypeId = model.PackageTypeId,
                GrossWeightKg = model.GrossWeightKg,
                NetWeightKg = model.NetWeightKg,
                Remarks = model.Remarks?.Trim()
            };

            _db.CustomsClearanceContainers.Add(entity);
            await _db.SaveChangesAsync();
        }

        public async Task UpdateAsync(CustomsClearanceContainerCreateEditVm model, int companyId)
        {
            if (!model.Id.HasValue)
                throw new InvalidOperationException("Container id is required.");

            var entity = await _db.CustomsClearanceContainers
                .FirstOrDefaultAsync(x => x.Id == model.Id.Value);

            if (entity == null)
                throw new InvalidOperationException("Customs clearance container not found.");

            await EnsureClearanceIsEditableAsync(entity.CustomsClearanceId, companyId);
            await ValidateContainerAsync(model, entity.Id);

            entity.ContainerId = model.ContainerId;
            entity.ContainerNo = model.ContainerNo.Trim().ToUpper();
            entity.ContainerSizeTypeId = model.ContainerSizeTypeId;
            entity.FullGateInContainerId = model.FullGateInContainerId;
            entity.VGMContainerId = model.VGMContainerId;
            entity.SealNo = model.SealNo?.Trim();
            entity.PackageTypeId = model.PackageTypeId;
            entity.GrossWeightKg = model.GrossWeightKg;
            entity.NetWeightKg = model.NetWeightKg;
            entity.Remarks = model.Remarks?.Trim();

            await _db.SaveChangesAsync();
        }

        public async Task DeleteAsync(Guid id, int companyId)
        {
            var entity = await _db.CustomsClearanceContainers
                .FirstOrDefaultAsync(x => x.Id == id);

            if (entity == null)
                return;

            await EnsureClearanceIsEditableAsync(entity.CustomsClearanceId, companyId);

            _db.CustomsClearanceContainers.Remove(entity);
            await _db.SaveChangesAsync();
        }

        private async Task ValidateContainerAsync(CustomsClearanceContainerCreateEditVm model, Guid? editingId = null)
        {
            if (model.NetWeightKg > model.GrossWeightKg)
                throw new InvalidOperationException("Net weight cannot be greater than gross weight.");

            var normalizedContainerNo = model.ContainerNo.Trim().ToUpper();

            var duplicateExists = await _db.CustomsClearanceContainers
                .AnyAsync(x =>
                    x.CustomsClearanceId == model.CustomsClearanceId &&
                    x.ContainerNo == normalizedContainerNo &&
                    (!editingId.HasValue || x.Id != editingId.Value));

            if (duplicateExists)
                throw new InvalidOperationException("Container number already exists in this customs clearance.");
        }

        private async Task EnsureClearanceExistsAsync(Guid customsClearanceId, int companyId)
        {
            var exists = await _db.CustomsClearances
                .AsNoTracking()
                .AnyAsync(x => x.Id == customsClearanceId && x.CompanyId == companyId);

            if (!exists)
                throw new InvalidOperationException("Customs clearance not found.");
        }

        private async Task EnsureClearanceIsEditableAsync(Guid customsClearanceId, int companyId)
        {
            var clearance = await _db.CustomsClearances
                .AsNoTracking()
                .Where(x => x.Id == customsClearanceId && x.CompanyId == companyId)
                .Select(x => new
                {
                    x.Id,
                    x.CustomsStatusId
                })
                .FirstOrDefaultAsync();

            if (clearance == null)
                throw new InvalidOperationException("Customs clearance not found.");

            if (clearance.CustomsStatusId != -1)
                throw new InvalidOperationException("Only draft customs clearance can be modified.");
        }
    }




    public class CustomsClearanceChargeService : ICustomsClearanceChargeService
    {
        private readonly ApplicationDbContext _db;

        public CustomsClearanceChargeService(ApplicationDbContext db)
        {
            _db = db;
        }

        public async Task<CustomsClearanceChargesTabVm> GetChargesTabAsync(Guid customsClearanceId, int companyId)
        {
            var charges = await _db.CustomsClearanceCharges
                .AsNoTracking()
                .Where(x => x.CustomsClearanceId == customsClearanceId)
                .OrderBy(x => x.ChargeType.ChargeTypeName)
                .ThenBy(x => x.ChargeDescription)
                .Select(x => new CustomsClearanceChargeListItemVm
                {
                    Id = x.Id,
                    ChargeTypeName = x.ChargeType.ChargeTypeName,
                    ChargeDescription = x.ChargeDescription,
                    ItemReference = x.CustomsClearanceItem != null
                        ? x.CustomsClearanceItem.HSCodeToSave + " - " + x.CustomsClearanceItem.GoodsDescriptionEN
                        : "File Level",
                    Amount = x.Amount,
                    CurrencyName = x.Currency.CurrencyName,
                    ExchangeRate = x.ExchangeRate,
                    LocalAmount = x.LocalAmount,
                    IsTax = x.IsTax,
                    IsSystemCalculated = x.IsSystemCalculated,
                    IsPaid = x.IsPaid,
                    PaidAt = x.PaidAt,
                    PaymentReferenceNo = x.PaymentReferenceNo,
                    InvoiceNo = x.InvoiceNo,
                    InvoiceDate = x.InvoiceDate,
                    Remarks = x.Remarks
                })
                .ToListAsync();

            return new CustomsClearanceChargesTabVm
            {
                CustomsClearanceId = customsClearanceId,
                Charges = charges
            };
        }

        public async Task<CustomsClearanceChargeCreateEditVm> BuildCreateVmAsync(Guid customsClearanceId, int companyId)
        {
            await EnsureClearanceExistsAsync(customsClearanceId, companyId);

            var vm = new CustomsClearanceChargeCreateEditVm
            {
                CustomsClearanceId = customsClearanceId,
                ExchangeRate = 1
            };

            await PopulateLookupsAsync(vm, companyId);
            return vm;
        }

        public async Task<CustomsClearanceChargeCreateEditVm?> BuildEditVmAsync(Guid id, int companyId)
        {
            var vm = await _db.CustomsClearanceCharges
                .AsNoTracking()
                .Where(x => x.Id == id)
                .Select(x => new CustomsClearanceChargeCreateEditVm
                {
                    Id = x.Id,
                    CustomsClearanceId = x.CustomsClearanceId,
                    CustomsClearanceItemId = x.CustomsClearanceItemId,
                    ChargeTypeId = x.ChargeTypeId,
                    ChargeDescription = x.ChargeDescription,
                    Amount = x.Amount,
                    CurrencyId = x.CurrencyId,
                    ExchangeRate = x.ExchangeRate,
                    LocalAmount = x.LocalAmount,
                    IsTax = x.IsTax,
                    IsSystemCalculated = x.IsSystemCalculated,
                    IsPaid = x.IsPaid,
                    PaidAt = x.PaidAt,
                    PaymentReferenceNo = x.PaymentReferenceNo,
                    InvoiceNo = x.InvoiceNo,
                    InvoiceDate = x.InvoiceDate,
                    Remarks = x.Remarks
                })
                .FirstOrDefaultAsync();

            if (vm == null)
                return null;

            await PopulateLookupsAsync(vm, companyId);
            return vm;
        }

        public async Task PopulateLookupsAsync(CustomsClearanceChargeCreateEditVm vm, int companyId)
        {
            vm.ChargeTypes = await _db.ChargeTypes
                .AsNoTracking()
                .OrderBy(x => x.ChargeTypeName)
                .Select(x => new SelectListItem
                {
                    Value = x.Id.ToString(),
                    Text = x.ChargeTypeName
                })
                .ToListAsync();

            vm.Currencies = await _db.Currencies
                .AsNoTracking()
                .OrderBy(x => x.Id)
                .Select(x => new SelectListItem
                {
                    Value = x.Id.ToString(),
                    Text = x.CurrencyCode
                })
                .ToListAsync();

            vm.ClearanceItems = await _db.CustomsClearanceItems
                .AsNoTracking()
                .Where(x => x.CustomsClearanceId == vm.CustomsClearanceId)
                .OrderBy(x => x.HSCodeToSave)
                .ThenBy(x => x.GoodsDescriptionEN)
                .Select(x => new SelectListItem
                {
                    Value = x.Id.ToString(),
                    Text = x.HSCodeToSave + " - " + x.GoodsDescriptionEN
                })
                .ToListAsync();
        }

        public async Task CreateAsync(CustomsClearanceChargeCreateEditVm model, int companyId)
        {
            await EnsureClearanceIsEditableAsync(model.CustomsClearanceId, companyId);
            await ValidateChargeAsync(model, companyId);

            var entity = new CustomsClearanceCharge
            {
                Id = Guid.CreateVersion7(),
                CustomsClearanceId = model.CustomsClearanceId,
                CustomsClearanceItemId = model.CustomsClearanceItemId,
                ChargeTypeId = model.ChargeTypeId,
                ChargeDescription = model.ChargeDescription.Trim(),
                Amount = model.Amount,
                CurrencyId = model.CurrencyId,
                ExchangeRate = model.ExchangeRate,
                LocalAmount = CalculateLocalAmount(model.Amount, model.ExchangeRate),
                IsTax = model.IsTax,
                IsSystemCalculated = false,
                IsPaid = model.IsPaid,
                PaidAt = model.IsPaid ? model.PaidAt : null,
                PaymentReferenceNo = model.PaymentReferenceNo?.Trim(),
                InvoiceNo = model.InvoiceNo?.Trim(),
                InvoiceDate = model.InvoiceDate,
                Remarks = model.Remarks?.Trim()
            };

            _db.CustomsClearanceCharges.Add(entity);
            await _db.SaveChangesAsync();
        }

        public async Task UpdateAsync(CustomsClearanceChargeCreateEditVm model, int companyId)
        {
            if (!model.Id.HasValue)
                throw new InvalidOperationException("Charge id is required.");

            var entity = await _db.CustomsClearanceCharges
                .FirstOrDefaultAsync(x => x.Id == model.Id.Value);

            if (entity == null)
                throw new InvalidOperationException("Customs clearance charge not found.");

            await EnsureClearanceIsEditableAsync(entity.CustomsClearanceId, companyId);

            if (entity.IsSystemCalculated)
                throw new InvalidOperationException("System-calculated charges cannot be edited.");

            await ValidateChargeAsync(model, companyId, entity.Id);

            entity.CustomsClearanceItemId = model.CustomsClearanceItemId;
            entity.ChargeTypeId = model.ChargeTypeId;
            entity.ChargeDescription = model.ChargeDescription.Trim();
            entity.Amount = model.Amount;
            entity.CurrencyId = model.CurrencyId;
            entity.ExchangeRate = model.ExchangeRate;
            entity.LocalAmount = CalculateLocalAmount(model.Amount, model.ExchangeRate);
            entity.IsTax = model.IsTax;
            entity.IsPaid = model.IsPaid;
            entity.PaidAt = model.IsPaid ? model.PaidAt : null;
            entity.PaymentReferenceNo = model.PaymentReferenceNo?.Trim();
            entity.InvoiceNo = model.InvoiceNo?.Trim();
            entity.InvoiceDate = model.InvoiceDate;
            entity.Remarks = model.Remarks?.Trim();

            await _db.SaveChangesAsync();
        }

        public async Task DeleteAsync(Guid id, int companyId)
        {
            var entity = await _db.CustomsClearanceCharges
                .FirstOrDefaultAsync(x => x.Id == id);

            if (entity == null)
                return;

            await EnsureClearanceIsEditableAsync(entity.CustomsClearanceId, companyId);

            if (entity.IsSystemCalculated)
                throw new InvalidOperationException("System-calculated charges cannot be deleted.");

            _db.CustomsClearanceCharges.Remove(entity);
            await _db.SaveChangesAsync();
        }

        private async Task ValidateChargeAsync(CustomsClearanceChargeCreateEditVm model, int companyId, Guid? editingId = null)
        {
            if (model.Amount < 0)
                throw new InvalidOperationException("Amount cannot be negative.");

            if (model.ExchangeRate <= 0)
                throw new InvalidOperationException("Exchange rate must be greater than zero.");

            if (model.IsPaid && !model.PaidAt.HasValue)
                throw new InvalidOperationException("Paid date is required when charge is marked as paid.");

            if (!model.IsPaid)
                model.PaidAt = null;

            if (model.CustomsClearanceItemId.HasValue)
            {
                var itemExists = await _db.CustomsClearanceItems
                    .AsNoTracking()
                    .AnyAsync(x =>
                        x.Id == model.CustomsClearanceItemId.Value &&
                        x.CustomsClearanceId == model.CustomsClearanceId
                        );

                if (!itemExists)
                    throw new InvalidOperationException("Selected item does not belong to this customs clearance.");
            }
        }

        private static decimal CalculateLocalAmount(decimal amount, decimal exchangeRate)
        {
            return Math.Round(amount * exchangeRate, 2, MidpointRounding.AwayFromZero);
        }

        private async Task EnsureClearanceExistsAsync(Guid customsClearanceId, int companyId)
        {
            var exists = await _db.CustomsClearances
                .AsNoTracking()
                .AnyAsync(x => x.Id == customsClearanceId && x.CompanyId == companyId);

            if (!exists)
                throw new InvalidOperationException("Customs clearance not found.");
        }

        private async Task EnsureClearanceIsEditableAsync(Guid customsClearanceId, int companyId)
        {
            var clearance = await _db.CustomsClearances
                .AsNoTracking()
                .Where(x => x.Id == customsClearanceId && x.CompanyId == companyId)
                .Select(x => new { x.Id, x.CustomsStatusId })
                .FirstOrDefaultAsync();

            if (clearance == null)
                throw new InvalidOperationException("Customs clearance not found.");

            if (clearance.CustomsStatusId != -1)
                throw new InvalidOperationException("Only draft customs clearance can be modified.");
        }
    }



    public class CustomsClearanceTimelineService : ICustomsClearanceTimelineService
    {
        private readonly ApplicationDbContext _db;

        public CustomsClearanceTimelineService(ApplicationDbContext db)
        {
            _db = db;
        }

        public async Task<CustomsClearanceTimelineTabVm> GetTimelineTabAsync(Guid customsClearanceId, int companyId)
        {
            await EnsureClearanceExistsAsync(customsClearanceId, companyId);

            var entries = await _db.ActivityLogs
                .AsNoTracking()
                .Where(x => x.EntityId == customsClearanceId)
                .OrderByDescending(x => x.ActivityDateTime)
                .Select(x => new ActivityLogListItemVm
                {
                    Id = x.Id,
                    ActivityDateTime = x.ActivityDateTime,
                    Title = x.Title,
                    Description = x.Description,
                    ReferenceNo = x.ReferenceNo,
                    ActivityType = x.ActivityType,
                    EventTypeName = x.EventType != null ? x.EventType.EventTypeName : null,
                    FieldName = x.FieldName,
                    OldValue = x.OldValue,
                    NewValue = x.NewValue,
                    Severity = x.Severity,
                    IconCssClass = x.IconCssClass,
                    IsSystemGenerated = x.IsSystemGenerated,
                    SourceSystem = x.SourceSystem,
                    ExternalReferenceNo = x.ExternalReferenceNo,
                    PerformedByUserName = x.PerformedByUser != null ? x.PerformedByUser.FullName : null,
                    LocationName = x.Location != null ? x.Location.LocationName : null
                })
                .ToListAsync();

            return new CustomsClearanceTimelineTabVm
            {
                CustomsClearanceId = customsClearanceId,
                Entries = entries
            };
        }

        private async Task EnsureClearanceExistsAsync(Guid customsClearanceId, int companyId)
        {
            var exists = await _db.CustomsClearances
                .AsNoTracking()
                .AnyAsync(x => x.Id == customsClearanceId && x.CompanyId == companyId);

            if (!exists)
                throw new InvalidOperationException("Customs clearance not found.");
        }
    }


    public class CustomsClearanceLicenseService : ICustomsClearanceLicenseService
    {
        private readonly ApplicationDbContext _db;
        private readonly IActivityLogService _activityLogService;

        public CustomsClearanceLicenseService(ApplicationDbContext db, IActivityLogService activityLogService)
        {
            _db = db;
            _activityLogService = activityLogService;
        }

        public async Task<CustomsClearanceLicensesTabVm> GetLicensesTabAsync(Guid customsClearanceId, int companyId)
        {
            var clearance = await GetClearanceAsync(customsClearanceId, companyId);

            var licenses = await _db.CustomsClearanceLicenses
                .AsNoTracking()
                .Where(x => x.CustomsClearanceId == customsClearanceId)
                .OrderBy(x => x.ExpiryDate)
                .ThenBy(x => x.LicenseType.LicenseTypeName)
                .Select(x => new CustomsClearanceLicenseListItemVm
                {
                    Id = x.Id,
                    CustomsClearanceId = x.CustomsClearanceId,
                    LicenseTypeName = x.LicenseType.LicenseTypeName,
                    IssuingOrganization = x.IssuingOrganization,
                    LicenseNumber = x.LicenseNumber,
                    IssueDate = x.IssueDate,
                    ExpiryDate = x.ExpiryDate,
                    LicenseStatusId = x.LicenseStatusId,
                    LicenseStatusName = x.LicenseStatus.LicenseStatusName,
                    Remarks = x.Remarks
                })
                .ToListAsync();

            var today = DateTime.Today;

            foreach (var item in licenses)
            {
                item.DaysToExpiry = (item.ExpiryDate.Date - today).Days;
                item.IsExpired = item.ExpiryDate.Date < today;
                item.IsExpiringSoon = !item.IsExpired && item.DaysToExpiry <= 30;
                item.CanEdit = clearance.CustomsStatusId == -1;
            }

            return new CustomsClearanceLicensesTabVm
            {
                CustomsClearanceId = customsClearanceId,
                CanEdit = clearance.CustomsStatusId == -1,
                Licenses = licenses
            };
        }

        public async Task<CustomsClearanceLicenseCreateEditVm> BuildCreateVmAsync(Guid customsClearanceId, int companyId)
        {
            var clearance = await GetClearanceAsync(customsClearanceId, companyId);

            var vm = new CustomsClearanceLicenseCreateEditVm
            {
                CustomsClearanceId = customsClearanceId,
                IssueDate = DateTime.Today,
                ExpiryDate = DateTime.Today,
                CanEdit = clearance.CustomsStatusId == -1,
                IsEdit = false
            };

            await FillLookupsAsync(vm, companyId);
            return vm;
        }

        public async Task<CustomsClearanceLicenseCreateEditVm> BuildEditVmAsync(Guid id, int companyId)
        {
            var entity = await _db.CustomsClearanceLicenses
                .AsNoTracking()
                .Include(x => x.CustomsClearance)
                .FirstOrDefaultAsync(x => x.Id == id);

            if (entity == null)
                throw new InvalidOperationException("License not found.");

            var vm = new CustomsClearanceLicenseCreateEditVm
            {
                Id = entity.Id,
                CustomsClearanceId = entity.CustomsClearanceId,
                LicenseTypeId = entity.LicenseTypeId,
                IssuingOrganization = entity.IssuingOrganization,
                LicenseNumber = entity.LicenseNumber,
                IssueDate = entity.IssueDate,
                ExpiryDate = entity.ExpiryDate,
                LicenseStatusId = entity.LicenseStatusId,
                Remarks = entity.Remarks,
                IsEdit = true,
                CanEdit = entity.CustomsClearance.CustomsStatusId == -1
            };

            await FillLookupsAsync(vm, companyId);
            return vm;
        }

        public async Task CreateAsync(CustomsClearanceLicenseCreateEditVm vm, int companyId)
        {
            var clearance = await GetClearanceAsync(vm.CustomsClearanceId, companyId);
            EnsureEditable(clearance);

            await ValidateAsync(vm, companyId, null);

            var entity = new CustomsClearanceLicense
            {
                Id = Guid.CreateVersion7(),
                CustomsClearanceId = vm.CustomsClearanceId,
                LicenseTypeId = vm.LicenseTypeId!.Value,
                IssuingOrganization = vm.IssuingOrganization?.Trim(),
                LicenseNumber = vm.LicenseNumber?.Trim(),
                IssueDate = vm.IssueDate!.Value.Date,
                ExpiryDate = vm.ExpiryDate!.Value.Date,
                LicenseStatusId = vm.LicenseStatusId!.Value,
                Remarks = vm.Remarks?.Trim()
            };

            _db.CustomsClearanceLicenses.Add(entity);
            await _db.SaveChangesAsync();


            await _activityLogService.LogEventAsync(new CreateActivityLogRequest
            {
                EntityTypeId = 14,
                EntityId = clearance.Id,
                Title = "License added",
                Description = $"License '{entity.LicenseNumber ?? "-"}' added .",
                ActivityType = "LicenseCreated",
                Severity = "success",
                IsSystemGenerated = false
            });


        }

        public async Task UpdateAsync(CustomsClearanceLicenseCreateEditVm vm, int companyId)
        {
            if (!vm.Id.HasValue)
                throw new InvalidOperationException("License id is required.");

            var entity = await _db.CustomsClearanceLicenses
                .Include(x => x.CustomsClearance)
                .FirstOrDefaultAsync(x => x.Id == vm.Id.Value);

            if (entity == null)
                throw new InvalidOperationException("License not found.");

            EnsureEditable(entity.CustomsClearance);

            await ValidateAsync(vm, companyId, entity.Id);

            entity.LicenseTypeId = vm.LicenseTypeId!.Value;
            entity.IssuingOrganization = vm.IssuingOrganization?.Trim();
            entity.LicenseNumber = vm.LicenseNumber?.Trim();
            entity.IssueDate = vm.IssueDate!.Value.Date;
            entity.ExpiryDate = vm.ExpiryDate!.Value.Date;
            entity.LicenseStatusId = vm.LicenseStatusId!.Value;
            entity.Remarks = vm.Remarks?.Trim();

            await _db.SaveChangesAsync();

            await _activityLogService.LogAuditAsync(new CreateActivityLogRequest
            {
                EntityTypeId = 14,
                EntityId = entity.CustomsClearanceId,
                Title = "License updated",
                Description = $"License '{entity.LicenseNumber ?? "-"}' was updated.",
                ActivityType = "LicenseUpdated",
                Severity = "primary"
            });

        }

        public async Task DeleteAsync(Guid id, int companyId)
        {
            var entity = await _db.CustomsClearanceLicenses
                .Include(x => x.CustomsClearance)
                .FirstOrDefaultAsync(x => x.Id == id);

            if (entity == null)
                throw new InvalidOperationException("License not found.");

            EnsureEditable(entity.CustomsClearance);

            _db.CustomsClearanceLicenses.Remove(entity);
            await _db.SaveChangesAsync();
        }

        private async Task FillLookupsAsync(CustomsClearanceLicenseCreateEditVm vm, int companyId)
        {
            vm.LicenseTypeLookup = await _db.LicenseTypes
                .AsNoTracking()
                .OrderBy(x => x.LicenseTypeName)
                .Select(x => new SelectListItem
                {
                    Value = x.Id.ToString(),
                    Text = x.LicenseTypeName
                })
                .ToListAsync();

            vm.LicenseStatusLookup = await _db.LicenseStatuses
                .AsNoTracking()
                .OrderBy(x => x.LicenseStatusName)
                .Select(x => new SelectListItem
                {
                    Value = x.Id.ToString(),
                    Text = x.LicenseStatusName
                })
                .ToListAsync();
        }

        private async Task ValidateAsync(CustomsClearanceLicenseCreateEditVm vm, int companyId, Guid? currentId)
        {
            if (!vm.LicenseTypeId.HasValue)
                throw new InvalidOperationException("License type is required.");

            if (!vm.LicenseStatusId.HasValue)
                throw new InvalidOperationException("License status is required.");

            if (!vm.IssueDate.HasValue)
                throw new InvalidOperationException("Issue date is required.");

            if (!vm.ExpiryDate.HasValue)
                throw new InvalidOperationException("Expiry date is required.");

            if (vm.ExpiryDate.Value.Date < vm.IssueDate.Value.Date)
                throw new InvalidOperationException("Expiry date cannot be earlier than issue date.");

            var normalizedLicenseNo = vm.LicenseNumber?.Trim();

            if (!string.IsNullOrWhiteSpace(normalizedLicenseNo))
            {
                var duplicateExists = await _db.CustomsClearanceLicenses
                    .AsNoTracking()
                    .AnyAsync(x =>
                        x.CustomsClearanceId == vm.CustomsClearanceId &&
                        x.LicenseTypeId == vm.LicenseTypeId.Value &&
                        x.LicenseNumber == normalizedLicenseNo &&
                        (!currentId.HasValue || x.Id != currentId.Value));

                if (duplicateExists)
                    throw new InvalidOperationException("A license with the same type and number already exists.");
            }
        }

        private async Task<CustomsClearance> GetClearanceAsync(Guid customsClearanceId, int companyId)
        {
            var clearance = await _db.CustomsClearances
                .FirstOrDefaultAsync(x => x.Id == customsClearanceId && x.CompanyId == companyId);

            if (clearance == null)
                throw new InvalidOperationException("Customs clearance not found.");

            return clearance;
        }

        private static void EnsureEditable(CustomsClearance clearance)
        {
            if (clearance.CustomsStatusId != -1)
                throw new InvalidOperationException("Only draft customs clearances can be modified.");
        }
    }






    public class CustomsInspectionService : ICustomsInspectionService
    {
        private readonly ApplicationDbContext _db;

        public CustomsInspectionService(ApplicationDbContext db)
        {
            _db = db;
        }

        public async Task<CustomsInspectionsTabVm> GetInspectionsTabAsync(Guid customsClearanceId, int companyId)
        {
            var clearance = await _db.CustomsClearances
                .AsNoTracking()
                .Select(c => new { c.Id, c.CustomsStatusId, c.CompanyId })
                .FirstOrDefaultAsync(c => c.Id == customsClearanceId && c.CompanyId == companyId);

            if (clearance == null)
                throw new KeyNotFoundException("Customs clearance record not found.");

            var isDraft = clearance.CustomsStatusId == -1;

            var list = await _db.CustomsInspections
                .AsNoTracking()
                .Where(x => x.CustomsClearanceId == customsClearanceId)
                .OrderByDescending(x => x.InspectionDate)
                .Select(x => new CustomsInspectionListItemVm
                {
                    Id = x.Id,
                    InspectionDate = x.InspectionDate,
                    InspectionTypeName = x.InspectionType != null ? x.InspectionType.InspectionTypeName : "N/A",
                    InspectionMethodName = x.InspectionMethod != null ? x.InspectionMethod.InspectionMethodName : "N/A",
                    InspectionResultName = x.InspectionResult != null ? x.InspectionResult.InspectionResultName : null,
                    InspectorName = x.InspectorName,
                    DiscrepancyFound = x.DiscrepancyFound,
                    DiscrepancyDescription = x.DiscrepancyDescription,
                    CorrectionsRequired = x.CorrectionsRequired,
                    CorrectionsDescription = x.CorrectionsDescription,
                    CanEdit = isDraft
                })
                .ToListAsync();

            return new CustomsInspectionsTabVm
            {
                CustomsClearanceId = customsClearanceId,
                CanEdit = isDraft,
                Inspections = list
            };
        }

        public async Task<CustomsInspectionCreateEditVm> BuildCreateVmAsync(Guid customsClearanceId, int companyId)
        {
            await VerifyClearanceIsEditableAsync(customsClearanceId, companyId);

            var vm = new CustomsInspectionCreateEditVm
            {
                CustomsClearanceId = customsClearanceId,
                InspectionDate = DateTime.Today,
                IsEdit = false,
                CanEdit = true
            };
            await PopulateLookupsAsync(vm, companyId);
            return vm;
        }

        public async Task<CustomsInspectionCreateEditVm> BuildEditVmAsync(Guid id, int companyId)
        {
            var entity = await _db.CustomsInspections
                .FirstOrDefaultAsync(x => x.Id == id);

            if (entity == null)
                throw new KeyNotFoundException("Inspection record not found.");

            await VerifyClearanceIsEditableAsync(entity.CustomsClearanceId, companyId);

            var vm = new CustomsInspectionCreateEditVm
            {
                Id = entity.Id,
                CustomsClearanceId = entity.CustomsClearanceId,
                InspectionTypeId = entity.InspectionTypeId,
                InspectionMethodId = entity.InspectionMethodId,
                InspectionResultId = entity.InspectionResultId,
                InspectionDate = entity.InspectionDate,
                InspectorName = entity.InspectorName,
                Location = entity.Location,
                Findings = entity.Findings,
                DiscrepancyFound = entity.DiscrepancyFound,
                DiscrepancyDescription = entity.DiscrepancyDescription,
                CorrectionsRequired = entity.CorrectionsRequired,
                CorrectionsDescription = entity.CorrectionsDescription,
                Remarks = entity.Remarks,
                IsEdit = true,
                CanEdit = true
            };

            await PopulateLookupsAsync(vm, companyId);
            return vm;
        }

        public async Task CreateAsync(CustomsInspectionCreateEditVm vm, int companyId)
        {
            await VerifyClearanceIsEditableAsync(vm.CustomsClearanceId, companyId);

            var entity = new CustomsInspection
            {
                Id = Guid.CreateVersion7(),
                CustomsClearanceId = vm.CustomsClearanceId,
                InspectionTypeId = vm.InspectionTypeId!.Value,
                InspectionMethodId = vm.InspectionMethodId!.Value,
                InspectionResultId = vm.InspectionResultId,
                InspectionDate = vm.InspectionDate!.Value,
                InspectorName = vm.InspectorName,
                Location = vm.Location,
                Findings = vm.Findings,
                DiscrepancyFound = vm.DiscrepancyFound,
                DiscrepancyDescription = vm.DiscrepancyFound ? vm.DiscrepancyDescription : null,
                CorrectionsRequired = vm.CorrectionsRequired,
                CorrectionsDescription = vm.CorrectionsRequired ? vm.CorrectionsDescription : null,
                Remarks = vm.Remarks
            };

            _db.CustomsInspections.Add(entity);
            await _db.SaveChangesAsync();
        }

        public async Task UpdateAsync(CustomsInspectionCreateEditVm vm, int companyId)
        {
            var entity = await _db.CustomsInspections
                .FirstOrDefaultAsync(x => x.Id == vm.Id);

            if (entity == null)
                throw new KeyNotFoundException("Inspection record not found.");

            // بررسی اینکه آیا پرونده والد قابل ویرایش است یا خیر
            await VerifyClearanceIsEditableAsync(entity.CustomsClearanceId, companyId);

            entity.InspectionTypeId = vm.InspectionTypeId!.Value;
            entity.InspectionMethodId = vm.InspectionMethodId!.Value;
            entity.InspectionResultId = vm.InspectionResultId;
            entity.InspectionDate = vm.InspectionDate!.Value;
            entity.InspectorName = vm.InspectorName;
            entity.Location = vm.Location;
            entity.Findings = vm.Findings;

            entity.DiscrepancyFound = vm.DiscrepancyFound;
            entity.DiscrepancyDescription = vm.DiscrepancyFound ? vm.DiscrepancyDescription : null;

            entity.CorrectionsRequired = vm.CorrectionsRequired;
            entity.CorrectionsDescription = vm.CorrectionsRequired ? vm.CorrectionsDescription : null;

            entity.Remarks = vm.Remarks;

            await _db.SaveChangesAsync();
        }

        public async Task<Guid> DeleteAsync(Guid id, int companyId)
        {
            var entity = await _db.CustomsInspections
                .FirstOrDefaultAsync(x => x.Id == id);

            if (entity == null)
                throw new KeyNotFoundException("Inspection record not found.");

            await VerifyClearanceIsEditableAsync(entity.CustomsClearanceId, companyId);

            _db.CustomsInspections.Remove(entity);
            await _db.SaveChangesAsync();

            return entity.CustomsClearanceId;
        }

        public async Task VerifyClearanceIsEditableAsync(Guid customsClearanceId, int companyId)
        {
            var status = await _db.CustomsClearances
                .Where(x => x.Id == customsClearanceId)
                .Select(x => x.CustomsStatusId)
                .FirstOrDefaultAsync();

            if (status == null)
                throw new KeyNotFoundException("Customs clearance record not found.");

            if (status != -1)
                throw new InvalidOperationException("This action is only allowed when customs clearance is in 'Draft' status.");
        }

        public async Task PopulateLookupsAsync(CustomsInspectionCreateEditVm vm, int companyId)
        {
            vm.InspectionTypeLookup = await _db.InspectionTypes
                .Select(x => new SelectListItem { Value = x.Id.ToString(), Text = x.InspectionTypeName })
                .ToListAsync();

            vm.InspectionMethodLookup = await _db.InspectionMethods
                .Select(x => new SelectListItem { Value = x.Id.ToString(), Text = x.InspectionMethodName })
                .ToListAsync();

            vm.InspectionResultLookup = await _db.InspectionResults
                .Select(x => new SelectListItem { Value = x.Id.ToString(), Text = x.InspectionResultName })
                .ToListAsync();
        }
    }




}
