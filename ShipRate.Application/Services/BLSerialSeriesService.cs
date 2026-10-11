using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using ShipRate.Application.DTOs;
using ShipRate.Application.Interfaces;
using ShipRate.Domain.Entities.BillOfLading;
using ShipRate.Infrastructure.Data;

namespace ShipRate.Application.Services
{
    public class BLSerialSeriesService : IBLSerialSeriesService
    {
        private readonly ApplicationDbContext _db;

        public BLSerialSeriesService(ApplicationDbContext db)
        {
            _db = db;
        }

        public async Task<BLSerialSeriesIndexViewModel> GetIndexViewModelAsync()
        {
            var carrierTypes = await _db.CarrierBLTypes
                .Where(x => !x.IsDeleted)
                .OrderBy(x => x.CarrierBLTypeName)
                .Select(x => new SelectListItem
                {
                    Value = x.Id.ToString(),
                    Text = x.CarrierBLTypeName
                })
                .ToListAsync();

            return new BLSerialSeriesIndexViewModel
            {
                CarrierBLTypes = carrierTypes
            };
        }

        public async Task<List<BLSerialSeriesListItemDto>> SearchSeriesAsync(BLSerialSeriesSearchDto input)
        {
            var query = _db.BLSerialSeries
                .Include(s => s.CarrierBLType)
                .Where(s => !s.IsDeleted);

            if (input.CarrierBLTypeId.HasValue)
            {
                query = query.Where(s => s.CarrierBLTypeId == input.CarrierBLTypeId.Value);
            }

            // اگر بر اساس SerialNumber هم جستجو داریم
            if (input.SerialNumber.HasValue)
            {
                var serialSeriesIds = await _db.BLSerials
                    .Where(x => !x.IsDeleted && x.SerialNumber == input.SerialNumber.Value)
                    .Select(x => x.BLSerialSeriesId)
                    .Distinct()
                    .ToListAsync();

                if (serialSeriesIds.Any())
                    query = query.Where(s => serialSeriesIds.Contains(s.Id));
                else
                    return new List<BLSerialSeriesListItemDto>();
            }

            var list = await query
                .OrderByDescending(s => s.IssueDate)
                .ThenBy(s => s.SeriesCode)
                .Select(s => new BLSerialSeriesListItemDto
                {
                    Id = s.Id,
                    SeriesCode = s.SeriesCode,
                    SeriesName = s.SeriesName,
                    Prefix = s.Prefix,
                    FromNumber = s.FromNumber,
                    ToNumber = s.ToNumber,
                    IssueDate = s.IssueDate,
                    CarrierBLTypeName = s.CarrierBLType.CarrierBLTypeName,
                })
                .ToListAsync();

            return list;
        }

        public async Task<BLSerialSeriesEditDto> GetSeriesForEditAsync(Guid? id)
        {
            if (!id.HasValue)
            {
                return new BLSerialSeriesEditDto
                {
                    IssueDate = DateTime.Today,
                    IsActive = true
                };
            }

            var entity = await _db.BLSerialSeries
                .AsNoTracking()
                .FirstOrDefaultAsync(x => x.Id == id.Value && !x.IsDeleted);

            if (entity == null) return null;

            return new BLSerialSeriesEditDto
            {
                Id = entity.Id,
                CarrierBLTypeId = entity.CarrierBLTypeId,
                SeriesCode = entity.SeriesCode,
                SeriesName = entity.SeriesName,
                Prefix = entity.Prefix,
                FromNumber = entity.FromNumber,
                ToNumber = entity.ToNumber,
                IssueDate = entity.IssueDate,
                Remarks = entity.Remarks
            };
        }

        public async Task<Guid> CreateOrUpdateSeriesAsync(BLSerialSeriesEditDto dto, Guid currentUserId)
        {
            if (dto.Id.HasValue)
            {
                var entity = await _db.BLSerialSeries.FirstOrDefaultAsync(x => x.Id == dto.Id.Value && !x.IsDeleted);
                if (entity == null)
                    throw new Exception("Series not found");

                entity.CarrierBLTypeId = dto.CarrierBLTypeId;
                entity.SeriesCode = dto.SeriesCode;
                entity.SeriesName = dto.SeriesName;
                entity.Prefix = dto.Prefix;
                entity.FromNumber = dto.FromNumber;
                entity.ToNumber = dto.ToNumber;
                entity.IssueDate = dto.IssueDate;
                entity.Remarks = dto.Remarks;
                entity.UpdatedAt = DateTime.Now;
                entity.EditUserId = currentUserId;

                await _db.SaveChangesAsync();
                return entity.Id;
            }
            else
            {
                var entity = new BLSerialSeries
                {
                    Id = Guid.CreateVersion7(),
                    CarrierBLTypeId = dto.CarrierBLTypeId,
                    SeriesCode = dto.SeriesCode,
                    SeriesName = dto.SeriesName,
                    Prefix = dto.Prefix,
                    FromNumber = dto.FromNumber,
                    ToNumber = dto.ToNumber,
                    IssueDate = dto.IssueDate,
                    IsDeleted = false,
                    CreatedAt = DateTime.Now,
                    CreateUserId = currentUserId,
                    Remarks = dto.Remarks
                };

                _db.BLSerialSeries.Add(entity);
                await _db.SaveChangesAsync();
                return entity.Id;
            }
        }

        public async Task DeleteSeriesAsync(Guid id, Guid currentUserId)
        {
            var entity = await _db.BLSerialSeries.FirstOrDefaultAsync(x => x.Id == id && !x.IsDeleted);
            if (entity == null)
                return;

            entity.IsDeleted = true;
            entity.UpdatedAt = DateTime.Now;
            entity.EditUserId = currentUserId;

            await _db.SaveChangesAsync();
        }

        public async Task<List<BLSerialBatchAssignListItemDto>> GetBatchAssignsAsync(Guid seriesId)
        {
            var list = await _db.BLSerialBatchAssigns
                .Include(b => b.Agent)
                .Where(b => !b.IsDeleted && b.BLSerialSeriesId == seriesId)
                .OrderBy(b => b.FromNumber)
                .Select(b => new BLSerialBatchAssignListItemDto
                {
                    Id = b.Id,
                    BLSerialSeriesId = b.BLSerialSeriesId,
                    FromNumber = b.FromNumber,
                    ToNumber = b.ToNumber,
                    AssignDate = b.AssignDate,
                    AgentName = b.Agent.PersonName, // یا FirstName + LastName
                    Remarks = b.Remarks
                })
                .ToListAsync();

            return list;
        }

        public async Task<BLSerialBatchAssignEditDto> GetBatchAssignForEditAsync(Guid? id, Guid seriesId)
        {
            if (!id.HasValue)
            {
                return new BLSerialBatchAssignEditDto
                {
                    BLSerialSeriesId = seriesId,
                    AssignDate = DateTime.Today,
                    IsActive = true
                };
            }

            var entity = await _db.BLSerialBatchAssigns
                .FirstOrDefaultAsync(x => x.Id == id.Value && !x.IsDeleted);

            if (entity == null) return null;

            return new BLSerialBatchAssignEditDto
            {
                Id = entity.Id,
                BLSerialSeriesId = entity.BLSerialSeriesId,
                AgentId = entity.AgentId,
                FromNumber = entity.FromNumber,
                ToNumber = entity.ToNumber,
                AssignDate = entity.AssignDate,
                Remarks = entity.Remarks
            };
        }

        public async Task<Guid> CreateOrUpdateBatchAssignAsync(BLSerialBatchAssignEditDto dto, Guid currentUserId)
        {
            if (dto.FromNumber > dto.ToNumber)
                throw new Exception("From Number cannot be greater than To Number.");

            await using var transaction = await _db.Database.BeginTransactionAsync();

            try
            {
                var series = await _db.BLSerialSeries
                    .FirstOrDefaultAsync(x => x.Id == dto.BLSerialSeriesId && !x.IsDeleted);

                if (series == null)
                    throw new Exception("BL Serial Series not found.");

                // کنترل اینکه بازه تخصیص داخل بازه اصلی Series باشد
                if (dto.FromNumber < series.FromNumber || dto.ToNumber > series.ToNumber)
                {
                    throw new Exception(
                        $"Assignment range must be inside series range: {series.FromNumber} - {series.ToNumber}.");
                }

                // کنترل تداخل بازه با BatchAssignهای دیگر
                var hasOverlap = await _db.BLSerialBatchAssigns
                    .Where(x =>
                        !x.IsDeleted &&
                        x.BLSerialSeriesId == dto.BLSerialSeriesId &&
                        (!dto.Id.HasValue || x.Id != dto.Id.Value))
                    .AnyAsync(x =>
                        dto.FromNumber <= x.ToNumber &&
                        dto.ToNumber >= x.FromNumber);

                if (hasOverlap)
                    throw new Exception("This serial range overlaps with another batch assignment.");

                BLSerialBatchAssign entity;

                if (dto.Id.HasValue)
                {
                    entity = await _db.BLSerialBatchAssigns
                        .FirstOrDefaultAsync(x => x.Id == dto.Id.Value && !x.IsDeleted);

                    if (entity == null)
                        throw new Exception("Batch assignment not found.");

                    entity.AgentId = dto.AgentId;
                    entity.FromNumber = dto.FromNumber;
                    entity.ToNumber = dto.ToNumber;
                    entity.AssignDate = dto.AssignDate;
                    entity.Remarks = dto.Remarks;
                    entity.UpdatedAt = DateTime.Now;
                    entity.EditUserId = currentUserId;
                }
                else
                {
                    entity = new BLSerialBatchAssign
                    {
                        Id = Guid.CreateVersion7(),
                        BLSerialSeriesId = dto.BLSerialSeriesId,
                        AgentId = dto.AgentId,
                        FromNumber = dto.FromNumber,
                        ToNumber = dto.ToNumber,
                        AssignDate = dto.AssignDate,
                        IsDeleted = false,
                        CreatedAt = DateTime.Now,
                        CreateUserId = currentUserId,
                        Remarks = dto.Remarks
                    };

                    _db.BLSerialBatchAssigns.Add(entity);
                }

                await _db.SaveChangesAsync();

                await GenerateOrUpdateBLSerialsForBatchAssignAsync(entity, currentUserId);

                await transaction.CommitAsync();

                return entity.Id;
            }
            catch
            {
                await transaction.RollbackAsync();
                throw;
            }
        }

        public async Task DeleteBatchAssignAsync(Guid id, Guid currentUserId)
        {
            var entity = await _db.BLSerialBatchAssigns.FirstOrDefaultAsync(x => x.Id == id && !x.IsDeleted);
            if (entity == null)
                return;

            entity.IsDeleted = true;
            entity.UpdatedAt = DateTime.Now;
            entity.EditUserId = currentUserId;

            await _db.SaveChangesAsync();
        }

        public async Task<List<BLSerialListItemDto>> GetSerialsAsync(Guid seriesId)
        {
            var list = await _db.BLSerials
                .Include(s => s.Agent)
                .Include(s => s.BL)
                .Where(s => !s.IsDeleted && s.BLSerialSeriesId == seriesId)
                .OrderBy(s => s.SerialNumber)
                .Select(s => new BLSerialListItemDto
                {
                    Id = s.Id,
                    BLSerialSeriesId = s.BLSerialSeriesId,
                    SerialNumber = s.SerialNumber,
                    AgentName = s.Agent != null ? s.Agent.PersonName : null,
                    BLNo = s.BL != null ? s.BL.BLNo : null,
                    AssignedAt = s.AssignedAt,
                    AttachedAt = s.AttachedAt
                })
                .ToListAsync();

            return list;
        }

        private async Task GenerateOrUpdateBLSerialsForBatchAssignAsync(
            BLSerialBatchAssign batchAssign,
            Guid currentUserId)
        {
            var from = batchAssign.FromNumber;
            var to = batchAssign.ToNumber;

            var existingSerials = await _db.BLSerials
                .Where(x =>
                    !x.IsDeleted &&
                    x.BLSerialSeriesId == batchAssign.BLSerialSeriesId &&
                    x.SerialNumber >= from &&
                    x.SerialNumber <= to)
                .ToListAsync();

            var existingSerialNumbers = existingSerials
                .Select(x => x.SerialNumber)
                .ToHashSet();

            var now = DateTime.Now;

            // اگر می‌خواهید AssignedAt دقیقاً برابر AssignDate باشد:
            var assignedAt = batchAssign.AssignDate;

            // 1. آپدیت رکوردهای موجود
            foreach (var serial in existingSerials)
            {
                // اگر این سریال قبلاً به یک BL وصل شده باشد، نباید دوباره تخصیص داده شود
                // نام فیلدها را با Entity خودتان هماهنگ کنید.
                if (serial.BLId != null)
                {
                    throw new Exception($"Serial number {serial.SerialNumber} is already attached to a BL and cannot be reassigned.");
                }

                serial.AgentId = batchAssign.AgentId;
                serial.AssignedAt = assignedAt;
                serial.UpdatedAt = now;
                serial.EditUserId = currentUserId;

                // اگر فیلد BLSerialBatchAssignId را اضافه کردید:
                // serial.BLSerialBatchAssignId = batchAssign.Id;
            }

            // 2. ایجاد رکوردهای جدید
            var newSerials = new List<BLSerial>();

            for (long serialNumber = from; serialNumber <= to; serialNumber++)
            {
                if (existingSerialNumbers.Contains(serialNumber))
                    continue;

                var serial = new BLSerial
                {
                    Id = Guid.CreateVersion7(),
                    BLSerialSeriesId = batchAssign.BLSerialSeriesId,
                    SerialNumber = serialNumber,
                    AgentId = batchAssign.AgentId,
                    AssignedAt = assignedAt,
                    IsDeleted = false,
                    CreatedAt = now,
                    CreateUserId = currentUserId,

                    // اگر فیلد BLSerialBatchAssignId را اضافه کردید:
                    // BLSerialBatchAssignId = batchAssign.Id
                };

                newSerials.Add(serial);
            }

            if (newSerials.Any())
                await _db.BLSerials.AddRangeAsync(newSerials);

            await _db.SaveChangesAsync();
        }



    }
}
