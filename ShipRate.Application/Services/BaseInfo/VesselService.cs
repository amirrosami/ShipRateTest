using Microsoft.EntityFrameworkCore;
using ShipRate.Application.DTOs;
using ShipRate.Application.DTOs.Base_Info;
using ShipRate.Application.Interfaces;
using ShipRate.Application.Interfaces.BaseInfo;
using ShipRate.Domain.Entities.BaseInfo.Vehicles;
using ShipRate.Domain.Interfaces;

namespace ShipRate.Application.Services.BaseInfo
{
    public class VesselService : IVesselService
    {
        private readonly ICurrentUserService _currentUserService;
        private readonly IVesselRepository _repository;
        public VesselService(IVesselRepository repository, ICurrentUserService currentUserService)
        {
            _repository = repository;
            _currentUserService = currentUserService;
        }
        public async Task<PagedResultDto<VesselDto>> GetPagedAsync(PagedRequestDto request)
        {
            var query = await _repository.GetAllAsQueryableAsync();

            // Search
            if (!string.IsNullOrWhiteSpace(request.SearchTerm))
            {
                var term = request.SearchTerm.Trim().ToLower();
                query = query.Where(x =>
                    x.VesselCode.ToLower().Contains(term) ||
                    x.VesselName.ToLower().Contains(term));
            }
            // Filter by IsActive
            //if (request.IsActive.HasValue)
            //{
            //    query = query.Where(x => x.IsActive == request.IsActive.Value);
            //}

            // Sorting
            //query = request.SortColumn?.ToLower() switch
            //{
            //    "packagetypecode" => request.SortDirection == "desc"
            //        ? query.OrderByDescending(x => x.PackageTypeCode)
            //        : query.OrderBy(x => x.PackageTypeCode),
            //    "packagetypename" => request.SortDirection == "desc"
            //        ? query.OrderByDescending(x => x.PackageTypeName)
            //        : query.OrderBy(x => x.PackageTypeName),
            //    "numericcode" => request.SortDirection == "desc"
            //        ? query.OrderByDescending(x => x.NumericCode)
            //        : query.OrderBy(x => x.NumericCode),
            //    "enscode" => request.SortDirection == "desc"
            //        ? query.OrderByDescending(x => x.ENSCode)
            //        : query.OrderBy(x => x.ENSCode),
            //    "isactive" => request.SortDirection == "desc"
            //        ? query.OrderByDescending(x => x.IsActive)
            //        : query.OrderBy(x => x.IsActive),
            //    _ => request.SortDirection == "desc"
            //        ? query.OrderByDescending(x => x.Sequence)
            //        : query.OrderBy(x => x.Sequence)
            //};

            var totalCount = await query.CountAsync();

            var items = await query
                .Skip((request.PageNumber - 1) * request.PageSize)
                .Take(request.PageSize)
                .Select(x => MapToDto(x))
                .ToListAsync();

            return new PagedResultDto<VesselDto>(items, totalCount, request.PageNumber, request.PageSize);
        }

        public async Task<VesselDto?> GetByIdAsync(Guid id)
        {
            var entity = await _repository.GetByIdWithRelationsAsync(id);
            return entity == null ? null : MapToDto(entity);
        }

        public async Task<(bool Success, string? Error)> CreateAsync(VesselCreateEditDto dto)
        {

            if (await _repository.AnyAsync(x => x.VesselCode == dto.VesselCode))
                return (false, $"Vessel Code '{dto.VesselCode}' already exists.");

            var entity = new Vessel
            {
                VesselCode = dto.VesselCode,
                VesselName = dto.VesselName,
                GrossTonnage = dto.GrossTonnage,
                IMONumber = dto.IMONumber,
                OwnerId = dto.OwnerId,
                OwnTypeId = dto.OwnTypeId,
                ShippingLineId = dto.ShippingLineId,
                FlagCountryId = dto.FlagCountryId,
                NetTonnage = dto.NetTonnage,
                //IsActive = true,
                IsDeleted = false,
                BuiltYear = dto.BuiltYear,
                CapacityTEU = dto.CapacityTEU,
                CallSign = dto.CallSign,
                DWT = dto.DWT,
                VesselTypeId = dto.VesselTypeId,
                CreateUserId = _currentUserService.UserId,
                CreatedAt = DateTime.Now,
                MMSI = dto.MMSI,
            };

            await _repository.AddAsync(entity);
            await _repository.SaveChangesAsync();
            return (true, null);
        }

        public async Task<(bool Success, string? Error)> UpdateAsync(VesselCreateEditDto dto)
        {
            var entity = await _repository.GetByGuidAsync(dto.Id);
            if (entity == null || entity.IsDeleted)
                return (false, "Vessel not found.");


            entity.VesselCode = dto.VesselCode;
            entity.VesselName = dto.VesselName;
            entity.GrossTonnage = dto.GrossTonnage;
            entity.IMONumber = dto.IMONumber;
            entity.OwnerId = dto.OwnerId;
            entity.OwnTypeId = dto.OwnTypeId;
            entity.ShippingLineId = dto.ShippingLineId;
            entity.FlagCountryId = dto.FlagCountryId;
            entity.NetTonnage = dto.NetTonnage;
            entity.BuiltYear = dto.BuiltYear;
            entity.CapacityTEU = dto.CapacityTEU;
            entity.CallSign = dto.CallSign;
            entity.DWT = dto.DWT;
            entity.VesselTypeId = dto.VesselTypeId;
            entity.UpdatedAt = DateTime.Now;
            entity.EditUserId = _currentUserService.UserId;
            entity.MMSI = dto.MMSI;

            await _repository.UpdateAsync(entity);
            await _repository.SaveChangesAsync();

            //return MapToDto(entity);
            return (true, null);
        }





        public async Task<bool> SoftDeleteAsync(Guid id)
        {
            var entity = await _repository.GetByGuidAsync(id);
            if (entity == null) return false;

            entity.IsDeleted = true;
            await _repository.UpdateAsync(entity);
            await _repository.SaveChangesAsync();
            return true;
        }


        private static VesselDto MapToDto(Vessel entity)
        {
            return new VesselDto
            {
                Id = entity.Id,
                VesselTypeId = entity.VesselTypeId,
                DWT = entity.DWT,
                BuiltYear = entity.BuiltYear,
                CallSign = entity.CallSign,
                CapacityTEU = entity.CapacityTEU,
                CompanyId = entity.CompanyId,
                FlagCountryId = entity.FlagCountryId,
                //FlagCountryName = entity.FlagCountry is null ? entity.FlagCountry.CountryName : "",
                GrossTonnage = entity.GrossTonnage,
                IMONumber = entity.IMONumber,
                NetTonnage = entity.NetTonnage,
                OwnerId = entity.OwnerId,
                //OwnerName = entity.Owner.PersonName,
                OwnTypeId = entity.OwnTypeId,
                OwnTypeName = entity.OwnType?.VesselOwnTypeName,
                ShippingLineId = entity.ShippingLineId,
                ShippingLineName = entity.ShippingLine?.Person?.PersonName,
                VesselCode = entity.VesselCode,
                VesselName = entity.VesselName,
                VesselTypeName = entity.VesselType?.VesselTypeName,
                MMSI = entity.MMSI
            };
        }
        public async Task<(bool Success, string? Error)> DeleteAsync(Guid id)
        {
            var entity = await _repository.GetByGuidAsync(id);
            if (entity == null || entity.IsDeleted)
                return (false, "Vessel not found.");

            entity.IsDeleted = true;
            //entity. = DateTime.Now;
            await _repository.UpdateAsync(entity);
            await _repository.SaveChangesAsync();
            return (true, null);
        }
    }
}