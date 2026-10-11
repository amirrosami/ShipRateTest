using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using ShipRate.Application.DTOs;
using ShipRate.Application.Interfaces;
using ShipRate.Domain.Entities.BillOfLading;
using ShipRate.Infrastructure.Data;

namespace ShipRate.Application.Services
{
    public class GuaranteePlanService : IGuaranteePlanService
    {
        private readonly ApplicationDbContext _db;

        public GuaranteePlanService(ApplicationDbContext context)
        {
            _db = context;
        }

        public async Task<PagedResultDto<GuaranteePlanDto>> GetPagedListAsync(GuaranteePlanSearchDto searchDto)
        {
            var query = _db.GuaranteePlans
                .Include(x => x.Company)
                .Include(x => x.InOut)
                .Include(x => x.ShippingLine)
                .Include(x => x.Customer)
                .AsNoTracking();

            if (!string.IsNullOrWhiteSpace(searchDto.SearchTerm))
            {
                query = query.Where(x => x.LetterNo.Contains(searchDto.SearchTerm) ||
                                         x.BLNo.Contains(searchDto.SearchTerm));
            }

            var totalCount = await query.CountAsync();
            var items = await query
                .OrderByDescending(x => x.CreatedAt)
                .Skip((searchDto.Page - 1) * searchDto.PageSize)
                .Take(searchDto.PageSize)
                .Select(x => new GuaranteePlanDto
                {
                    Id = x.Id,
                    CompanyId = x.CompanyId,
                    CompanyName = x.Company.Person.PersonName,
                    LetterNo = x.LetterNo,
                    LetterDate = x.LetterDate,
                    InOutId = x.InOutId,
                    InOutName = x.InOut.InOutName,
                    ShippingLineId = x.ShippingLineId,
                    ShippingLineName = x.ShippingLine.Person.PersonName,
                    CustomerId = x.CustomerId,
                    CustomerName = x.Customer.PersonName,
                    BLId = x.BLId,
                    BLNo = x.BLNo,
                    Remarks = x.Remarks
                }).ToListAsync();

            return new PagedResultDto<GuaranteePlanDto>(items, totalCount, searchDto.Page, searchDto.PageSize);
        }

        public async Task<GuaranteePlanDto> GetByIdAsync(Guid id)
        {
            var plan = await _db.GuaranteePlans
                .Include(x => x.Company)
                    .ThenInclude(x => x.Person)
                .Include(x => x.InOut)
                .Include(x => x.ShippingLine)
                    .ThenInclude(x => x.Person)
                .Include(x => x.Customer)
                .FirstOrDefaultAsync(x => x.Id == id);

            if (plan == null) return null;

            return new GuaranteePlanDto
            {
                Id = plan.Id,
                CompanyId = plan.CompanyId,
                CompanyName = plan.Company.Person.PersonName,
                LetterNo = plan.LetterNo,
                LetterDate = plan.LetterDate,
                InOutId = plan.InOutId,
                InOutName = plan.InOut.InOutName,
                ShippingLineId = plan.ShippingLineId,
                ShippingLineName = plan.ShippingLine?.Person.PersonName,
                CustomerId = plan.CustomerId,
                CustomerName = plan.Customer?.PersonName,
                BLId = plan.BLId,
                BLNo = plan.BLNo,
                Remarks = plan.Remarks
            };
        }

        public async Task<GuaranteePlanDto> CreateAsync(GuaranteePlanCreateOrUpdateDto dto, string userId)
        {
            var plan = new GuaranteePlan
            {
                Id = Guid.CreateVersion7(),
                CompanyId = dto.CompanyId,
                LetterNo = dto.LetterNo,
                LetterDate = dto.LetterDate,
                InOutId = dto.InOutId,
                ShippingLineId = dto.ShippingLineId,
                CustomerId = dto.CustomerId,
                BLId = dto.BLId,
                BLNo = dto.BLNo,
                Remarks = dto.Remarks,
                CreateUserId = Guid.Parse(userId),
                CreatedAt = DateTime.Now
            };

            _db.GuaranteePlans.Add(plan);
            await _db.SaveChangesAsync();
            return await GetByIdAsync(plan.Id);
        }

        public async Task<GuaranteePlanDto> UpdateAsync(Guid id, GuaranteePlanCreateOrUpdateDto dto, string userId)
        {
            var plan = await _db.GuaranteePlans.FindAsync(id);
            if (plan == null) throw new KeyNotFoundException("Guarantee Plan not found");

            plan.CompanyId = dto.CompanyId;
            plan.LetterNo = dto.LetterNo;
            plan.LetterDate = dto.LetterDate;
            plan.InOutId = dto.InOutId;
            plan.ShippingLineId = dto.ShippingLineId;
            plan.CustomerId = dto.CustomerId;
            plan.BLId = dto.BLId;
            plan.BLNo = dto.BLNo;
            plan.Remarks = dto.Remarks;
            plan.EditUserId = Guid.Parse(userId);
            plan.UpdatedAt = DateTime.Now;

            _db.GuaranteePlans.Update(plan);
            await _db.SaveChangesAsync();
            return await GetByIdAsync(id);
        }

        public async Task DeleteAsync(Guid id)
        {
            var plan = await _db.GuaranteePlans.FindAsync(id);
            if (plan != null)
            {
                _db.GuaranteePlans.Remove(plan);
                await _db.SaveChangesAsync();
            }
        }

        public async Task<List<SelectListItem>> GetCompanyLookupAsync() =>
            await _db.Companies.Select(c => new SelectListItem { Value = c.Id.ToString(), Text = c.Person.PersonName }).ToListAsync();

        public async Task<List<SelectListItem>> GetInOutLookupAsync() =>
            await _db.InOut.Select(i => new SelectListItem { Value = i.Id.ToString(), Text = i.InOutName }).ToListAsync();

        public async Task<List<SelectListItem>> GetShippingLineLookupAsync() =>
            await _db.Carriers.Select(s => new SelectListItem { Value = s.Id.ToString(), Text = s.Person.PersonName }).ToListAsync();

        public async Task<List<SelectListItem>> GetCustomerLookupAsync() =>
            await _db.Persons.Select(p => new SelectListItem { Value = p.Id.ToString(), Text = p.PersonName }).ToListAsync();

        public async Task<List<BlLookupResultDto>> SearchBlsAsync(string term)
        {
            // استخراج بارنامه‌هایی که در حال حاضر دارای طرح ضمانت هستند
            var assignedBlIds = _db.GuaranteePlans.Select(gp => gp.BLId);
            return await _db.BLs
                .Where(b => b.BLNo.Contains(term) && !assignedBlIds.Contains(b.Id))
                .Take(10)
                .Select(b => new BlLookupResultDto
                {
                    BLId = b.Id,
                    BLNo = b.BLNo
                })
                .ToListAsync();
        }
    }
}
