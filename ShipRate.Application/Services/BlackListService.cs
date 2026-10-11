using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using ShipRate.Application.DTOs;
using ShipRate.Application.Interfaces;
using ShipRate.Domaiun.Entities.Customers;
using ShipRate.Infrastructure.Data;

namespace ShipRate.Application.Services
{
    public class BlackListService : IBlackListService
    {
        private readonly ApplicationDbContext _context;
        private readonly ICurrentUserService _currentUserService;

        public BlackListService(ApplicationDbContext context, ICurrentUserService currentUserService)
        {
            _context = context;
            _currentUserService = currentUserService;
        }

        public async Task<PagedResultDto<BlackListDto>> GetPagedListAsync(BlackListSearchDto filter)
        {
            var query = _context.BlackLists
                .Include(b => b.Person)
                .Include(b => b.BlackListReason)
                .AsNoTracking();

            // فیلترینگ بر اساس کلیدواژه جستجو (نام شخص یا توضیحات)
            if (!string.IsNullOrWhiteSpace(filter.SearchTerm))
            {
                var term = filter.SearchTerm.Trim().ToLower();
                query = query.Where(b =>
                    b.Person!.PersonName.ToLower().Contains(term) ||
                    b.BlackListEnterRemarks.ToLower().Contains(term) ||
                    b.BlackListExitRemarks.ToLower().Contains(term));
            }

            // فیلترینگ بر اساس دلیل
            if (filter.BlackListReasonId.HasValue)
            {
                query = query.Where(b => b.BlackListReasonId == filter.BlackListReasonId.Value);
            }

            // فیلترینگ بر اساس وضعیت خروج
            if (filter.IsActiveInBlackList.HasValue)
            {
                if (filter.IsActiveInBlackList.Value)
                    query = query.Where(b => b.BlackListExitDateTime == null); // هنوز در لیست سیاه است
                else
                    query = query.Where(b => b.BlackListExitDateTime != null); // خارج شده است
            }

            var totalCount = await query.CountAsync();

            var items = await query
                .OrderByDescending(b => b.BlackListEnterDateTime)
                .Skip((filter.PageNumber - 1) * filter.PageSize)
                .Take(filter.PageSize)
                .Select(b => new BlackListDto
                {
                    Id = b.Id,
                    PersonId = b.PersonId,
                    PersonName = b.Person != null ? b.Person.PersonName : "N/A",
                    NationalId = b.Person != null ? b.Person.NationalID : "N/A",
                    BlackListReasonId = b.BlackListReasonId,
                    ReasonTitle = b.BlackListReason != null ? b.BlackListReason.BlackListReasonName : "N/A",
                    BlackListEnterDateTime = b.BlackListEnterDateTime,
                    EnterRemarks = b.BlackListEnterRemarks,
                    BlackListExitDateTime = b.BlackListExitDateTime,
                    ExitRemarks = b.BlackListExitRemarks
                })
                .ToListAsync();

            return new PagedResultDto<BlackListDto>
            {
                Items = items,
                TotalCount = totalCount,
                PageNumber = filter.PageNumber,
                PageSize = filter.PageSize
            };
        }

        public async Task<BlackListDto?> GetByIdAsync(Guid id)
        {
            return await _context.BlackLists
                .Include(b => b.Person)
                .Include(b => b.BlackListReason)
                .Where(b => b.Id == id)
                .Select(b => new BlackListDto
                {
                    Id = b.Id,
                    PersonId = b.PersonId,
                    PersonName = b.Person != null ? b.Person.PersonName : "N/A",
                    NationalId = b.Person != null ? b.Person.NationalID : "N/A",
                    BlackListReasonId = b.BlackListReasonId,
                    ReasonTitle = b.BlackListReason != null ? b.BlackListReason.BlackListReasonName : "N/A",
                    BlackListEnterDateTime = b.BlackListEnterDateTime,
                    EnterRemarks = b.BlackListEnterRemarks,
                    BlackListExitDateTime = b.BlackListExitDateTime,
                    ExitRemarks = b.BlackListExitRemarks
                })
                .FirstOrDefaultAsync();
        }

        public async Task CreateAsync(CreateBlackListDto dto, Guid currentUserId)
        {
            var entity = new BlackList
            {
                Id = Guid.NewGuid(),
                PersonId = dto.PersonId,
                BlackListReasonId = dto.BlackListReasonId,
                BlackListEnterDateTime = DateTime.Now,
                BlackListEnterUserId = currentUserId,
                BlackListEnterRemarks = dto.BlackListEnterRemarks,
                CreateUserId = currentUserId,
                CreatedAt = DateTime.Now,
            };

            _context.BlackLists.Add(entity);
            await _context.SaveChangesAsync();
        }

        public async Task ExitAsync(ExitBlackListDto dto, Guid currentUserId)
        {
            var entity = await _context.BlackLists.FindAsync(dto.Id);
            if (entity == null) throw new KeyNotFoundException("BlackList record not found.");

            entity.BlackListExitDateTime = DateTime.Now;
            entity.BlackListExitUserId = currentUserId;
            entity.BlackListExitRemarks = dto.BlackListExitRemarks;
            entity.UpdatedAt = DateTime.Now;
            entity.EditUserId = currentUserId;

            await _context.SaveChangesAsync();
        }
        public async Task DeleteAsync(Guid id)
        {
            return;
            var entity = await _context.BlackLists.FindAsync(id);
            if (entity != null)
            {
                entity.IsDeleted = true;
                entity.DeletedAt = DateTime.Now;
                entity.DeleteUserId = _currentUserService.UserId;
                await _context.SaveChangesAsync();
            }
        }
        public async Task<SelectList> GetPeopleSelectListAsync()
        {
            var list = await _context.Persons
                .OrderBy(p => p.PersonName)
                .Select(p => new { p.Id, p.PersonName })
                .ToListAsync();
            return new SelectList(list, "Id", "PersonName");
        }

        public async Task<SelectList> GetReasonsSelectListAsync()
        {
            var list = await _context.BlackListReasons
                .OrderBy(r => r.BlackListReasonName)
                .Select(r => new { r.Id, r.BlackListReasonName })
                .ToListAsync();
            return new SelectList(list, "Id", "BlackListReasonName");
        }
    }
}
