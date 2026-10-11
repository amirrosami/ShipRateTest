using Microsoft.EntityFrameworkCore;
using ShipRate.Application.DTOs;
using ShipRate.Application.DTOs.Base_Info;
using ShipRate.Application.Interfaces.BaseInfo;
using ShipRate.Domain.Entities.Customers;
using ShipRate.Domain.Interfaces;

namespace ShipRate.Application.Services.BaseInfo
{
    public class IndustryService : IIndustryService
    {
        private readonly IRepository<Industry> _repository;

        public IndustryService(IRepository<Industry> repository)
        {
            _repository = repository;
        }

        public async Task<PagedResultDto<IndustryDto>> GetPagedAsync(PagedRequestDto request)
        {
            var query = _repository.QueryNoTracking();

            // Search
            if (!string.IsNullOrWhiteSpace(request.SearchTerm))
            {
                var term = request.SearchTerm.Trim().ToLower();
                query = query.Where(x =>
                    x.IndustryName.ToLower().Contains(term));
            }


            // Sorting
            //query = request.SortColumn?.ToLower() switch
            //{
            //    "Industryname" => request.SortDirection == "desc"
            //        ? query.OrderByDescending(x => x.IndustryName)
            //        : query.OrderBy(x => x.IndustryName),
            //};

            var totalCount = await query.CountAsync();

            var items = await query
                .Skip((request.PageNumber - 1) * request.PageSize)
                .Take(request.PageSize)
                .Select(x => MapToDto(x))
                .ToListAsync();

            return new PagedResultDto<IndustryDto>(items, totalCount, request.PageNumber, request.PageSize);
        }





        public async Task<IndustryDto?> GetByIdAsync(Guid id)
        {
            var entity = await _repository.GetByGuidAsync(id);
            return entity == null ? null : MapToDto(entity);
        }

        public async Task<(bool Success, string? Error)> CreateAsync(IndustryCreateDto dto)
        {

            if (await _repository.AnyAsync(x => x.IndustryName == dto.IndustryName))
                return (false, $"Code '{dto.IndustryName}' already exists.");


            var entity = new Industry
            {
                IndustryName = dto.IndustryName.Trim(),
                IsDeleted = false
            };

            await _repository.AddAsync(entity);
            await _repository.SaveChangesAsync();
            return (true, null);
        }




        public async Task<(bool Success, string? Error)> UpdateAsync(IndustryUpdateDto dto)
        {
            var entity = await _repository.GetByGuidAsync(dto.Id);
            if (entity == null || entity.IsDeleted)
                return (false, "Industry not found.");



            entity.IndustryName = dto.IndustryName.Trim();

            await _repository.UpdateAsync(entity);
            await _repository.SaveChangesAsync();

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


        private static IndustryDto MapToDto(Industry entity)
        {
            return new IndustryDto
            {
                Id = entity.Id,
                IndustryName = entity.IndustryName,
                IsDeleted = entity.IsDeleted
            };
        }


        public async Task<(bool Success, string? Error)> DeleteAsync(Guid id)
        {
            var entity = await _repository.GetByGuidAsync(id);
            if (entity == null || entity.IsDeleted)
                return (false, "Industry not found.");

            entity.IsDeleted = true;
            //entity. = DateTime.Now;
            await _repository.UpdateAsync(entity);
            await _repository.SaveChangesAsync();
            return (true, null);
        }

        public async Task<(bool Success, string? Error)> ToggleActiveAsync(Guid id)
        {
            var entity = await _repository.GetByGuidAsync(id);
            if (entity == null || entity.IsDeleted)
                return (false, "Industry not found.");

            //entity.UpdatedAt = DateTime.Now;
            await _repository.UpdateAsync(entity);
            await _repository.SaveChangesAsync();
            return (true, null);
        }
    }
}
