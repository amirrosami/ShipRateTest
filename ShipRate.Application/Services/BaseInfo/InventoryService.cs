using Microsoft.EntityFrameworkCore;
using ShipRate.Application.DTOs;
using ShipRate.Application.Interfaces.BaseInfo;
using ShipRate.Domain.Entities.InventoryOperation;
using ShipRate.Domain.Interfaces;

namespace ShipRate.Application.Services.BaseInfo
{
    public class InventoryService : IInventoryService
    {
        private readonly IInventoryRepository _repository;
        public InventoryService(IInventoryRepository repository)
        {
            _repository = repository;
        }

        public async Task<PagedResultDto<InventoryDto>> GetPagedAsync(PagedRequestDto request)
        {
            var query = await _repository.GetAllAsQueryableAsync();

            // Search
            if (!string.IsNullOrWhiteSpace(request.SearchTerm))
            {
                var term = request.SearchTerm.Trim().ToLower();
                query = query.Where(x =>
                    x.InventoryName.ToLower().Contains(term) ||
                    x.InventoryCode.ToLower().Contains(term));
            }

            // Filter by IsActive
            if (request.IsActive.HasValue)
            {
                query = query.Where(x => x.IsActive == request.IsActive.Value);
            }

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

            return new PagedResultDto<InventoryDto>(items, totalCount, request.PageNumber, request.PageSize);
        }




        public async Task<InventoryDto?> GetByIdAsync(Guid id)
        {
            var entity = await _repository.GetByIdWithRelationsAsync(id);
            return entity == null ? null : MapToDto(entity);
        }

        public async Task<(bool Success, string? Error)> CreateAsync(InventoryCreateDto dto)
        {

            if (await _repository.AnyAsync(x => x.InventoryCode == dto.InventoryCode))
                return (false, $"Inventory Code '{dto.InventoryCode}' already exists.");


            var entity = new Inventory
            {
                InventoryName = dto.InventoryName,
                InventoryCode = dto.InventoryCode,
                CompanyId = dto.CompanyId,
                Remarks = dto.Remarks,
                IsActive = true,
                IsDeleted = false
            };

            await _repository.AddAsync(entity);
            await _repository.SaveChangesAsync();
            return (true, null);
        }





        public async Task<(bool Success, string? Error)> UpdateAsync(InventoryUpdateDto dto)
        {
            var entity = await _repository.GetByGuidAsync(dto.Id);
            if (entity == null || entity.IsDeleted)
                return (false, "Inventory not found.");

            entity.CompanyId = dto.CompanyId;
            entity.Remarks = dto.Remarks;
            entity.InventoryCode = dto.InventoryCode;
            entity.InventoryName = dto.InventoryName;


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

        private static InventoryDto MapToDto(Inventory entity)
        {
            return new InventoryDto
            {
                Id = entity.Id,
                InventoryName = entity.InventoryName,
                InventoryCode = entity.InventoryCode,
                CompanyId = entity.CompanyId,
                Remarks = entity.Remarks,
                CompanyName = entity.Company.Person.PersonName,
            };
        }


        public async Task<(bool Success, string? Error)> DeleteAsync(Guid id)
        {
            var entity = await _repository.GetByGuidAsync(id);
            if (entity == null || entity.IsDeleted)
                return (false, "Inventory not found.");

            entity.IsDeleted = true;
            await _repository.UpdateAsync(entity);
            await _repository.SaveChangesAsync();
            return (true, null);
        }

        public async Task<(bool Success, string? Error)> ToggleActiveAsync(Guid id)
        {
            var entity = await _repository.GetByGuidAsync(id);
            if (entity == null || entity.IsDeleted)
                return (false, "Inventory not found.");

            entity.IsActive = !entity.IsActive;
            //entity.UpdatedAt = DateTime.Now;
            await _repository.UpdateAsync(entity);
            await _repository.SaveChangesAsync();
            return (true, null);
        }
    }
}
