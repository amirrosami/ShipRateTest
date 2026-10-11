using Microsoft.EntityFrameworkCore;
using ShipRate.Application.DTOs;
using ShipRate.Application.DTOs.EDI;
using ShipRate.Application.Interfaces;
using ShipRate.Common.Utilities;
using ShipRate.Domain.Entities.Operations;
using ShipRate.Infrastructure.Data;

namespace ShipRate.Application.Services
{
    public class EDIService : IEDIService
    {
        private readonly ApplicationDbContext _db;

        public EDIService(ApplicationDbContext db)
        {
            _db = db;
        }

        public async Task<List<EDIMessageTypeRowDto>> GetMessageTypesAsync()
        {
            return await _db.EDIMessageTypes
                .Where(x => x.IsActive == true)
                .Select(x => new EDIMessageTypeRowDto
                {
                    Id = x.Id,
                    EDIMessageTypeCode = x.EDIMessageTypeCode,
                    EDIMessageTypeName = x.EDIMessageTypeName,
                    EDIMessageTypeStandard = x.EDIMessageTypeStandard,
                    IsActive = x.IsActive
                })
                .ToListAsync();
        }

        public async Task<EDIMessageListViewModel> GetMessagesAsync(EDIMessageSearchInput input)
        {
            var messageType = await _db.EDIMessageTypes
                .AsNoTracking()
                .Where(x => x.Id == input.MessageTypeId && !x.IsDeleted)
                .Select(x => new
                {
                    x.Id,
                    x.EDIMessageTypeCode,
                    x.EDIMessageTypeName
                })
                .FirstOrDefaultAsync();

            if (messageType == null)
            {
                return new EDIMessageListViewModel
                {
                    MessageTypeId = input.MessageTypeId,
                    SearchTerm = input.SearchTerm,
                    PageNumber = input.PageNumber,
                    PageSize = input.PageSize,
                    Result = new PagedResultDto<EDIMessageRowDto>()
                };
            }

            var query = from m in _db.EDIMessages.AsNoTracking()
                        join mt in _db.EDIMessageTypes.AsNoTracking() on m.MessageTypeId equals mt.Id
                        join io in _db.InOut.AsNoTracking() on m.InOutId equals io.Id
                        where !m.IsDeleted && m.MessageTypeId == input.MessageTypeId
                        select new EDIMessageRowDto
                        {
                            Id = m.Id,
                            MessageTypeId = m.MessageTypeId,
                            MessageTypeCode = mt.EDIMessageTypeCode,
                            InOutName = io.InOutName,
                            ReferenceNumber = m.ReferenceNumber,
                            ReceivedAt = m.ReceivedAt,
                            ProcessedAt = m.ProcessedAt,
                            ErrorMessage = m.ErrorMessage,
                            Remarks = m.Remarks,
                            //IsActive = m.IsActive,
                            CreatedAt = m.CreatedAt
                        };

            if (!string.IsNullOrWhiteSpace(input.SearchTerm))
            {
                var term = input.SearchTerm.Trim();
                query = query.Where(x =>
                    x.ReferenceNumber.Contains(term) ||
                    x.InOutName.Contains(term) ||
                    x.ErrorMessage.Contains(term) ||
                    x.Remarks.Contains(term));
            }

            var totalCount = await query.CountAsync();

            var items = await query
                .OrderByDescending(x => x.CreatedAt)
                .Skip((input.PageNumber - 1) * input.PageSize)
                .Take(input.PageSize)
                .ToListAsync();

            return new EDIMessageListViewModel
            {
                MessageTypeId = messageType.Id,
                MessageTypeCode = messageType.EDIMessageTypeCode,
                MessageTypeName = messageType.EDIMessageTypeName,
                SearchTerm = input.SearchTerm,
                PageNumber = input.PageNumber,
                PageSize = input.PageSize,
                Result = new PagedResultDto<EDIMessageRowDto>
                {
                    Items = items,
                    TotalCount = totalCount
                }
            };
        }

        public async Task<(byte[] Content, string FileName, string ContentType)?> DownloadPayloadAsync(Guid id)
        {
            var item = await _db.Set<EDIMessage>()
                .AsNoTracking()
                .Where(x => x.Id == id && !x.IsDeleted)
                .Select(x => new
                {
                    x.Id,
                    x.ReferenceNumber,
                    x.Payload
                })
                .FirstOrDefaultAsync();

            if (item == null)
                return null;

            var content = System.Text.Encoding.UTF8.GetBytes(item.Payload ?? string.Empty);
            var fileName = $"EDI_{item.ReferenceNumber}_{item.Id:N}.txt";

            return (content, fileName, "text/plain");
        }

        public async Task<EDIActionModalViewModel?> GetActionModalInfoAsync(int messageTypeId, string actionName)
        {
            var item = await _db.EDIMessageTypes
                .AsNoTracking()
                .Where(x => x.Id == messageTypeId && !x.IsDeleted)
                .Select(x => new EDIActionModalViewModel
                {
                    MessageTypeId = x.Id,
                    MessageTypeCode = x.EDIMessageTypeCode,
                    MessageTypeName = x.EDIMessageTypeName,
                    ActionName = actionName
                })
                .FirstOrDefaultAsync();

            return item;
        }
        public async Task<string> GenerateEPLAsync(Guid voyageId, Guid portId, List<Guid> blIds, int companyId)
        {
            string output = "VOY".DoubleQuoted();
            var voyage = await _db.Voyages
                .AsNoTracking()
                .Where(x => x.Id == voyageId).FirstOrDefaultAsync();

            var dash = "-".DoubleQuoted();
            var space = "".DoubleQuoted();
            var MFI = "MFI".DoubleQuoted();
            var Zero = "0".DoubleQuoted();
            var BOL = "BOL".DoubleQuoted();

            var person = await _db.Companies
                .AsNoTracking()
                .Include(x => x.Person)
                .Where(x => x.Id == companyId).FirstOrDefaultAsync();


            output += $",{person.Person.NationalID.DoubleQuoted()},{person.Person.NationalID.DoubleQuoted()},{voyage.Vessel.VesselName.DoubleQuoted()}";
            output += $",{voyage.VoyageNo.DoubleQuoted()}";
            output += $"{dash},{dash},{space},{MFI},{Zero},{voyage.VoyageNo.DoubleQuoted()}";
            output += Environment.NewLine;

            foreach (var blId in blIds)
            {
                output += GetEPLForBL(blId);
                output += Environment.NewLine;
            }
            return output;
        }
        private string GetEPLForBL(Guid blId)
        {
            var output = "BOL".DoubleQuoted();
            return output;
        }
        public async Task<string> GenerateCoparnAsync(Guid voyageId, Guid portId, List<Guid> blIds, int companyId)
        {
            var output = "";
            return output;
        }
        public async Task<string> GenerateTCTSFile(Guid voyageId, Guid portId, List<Guid> blIds, int companyId)
        {
            var output = "";
            return output;
        }
    }
}
