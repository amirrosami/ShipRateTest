using ShipRate.Application.DTOs.EDI;

namespace ShipRate.Application.Interfaces
{
    public interface IEDIService
    {
        Task<List<EDIMessageTypeRowDto>> GetMessageTypesAsync();
        Task<EDIMessageListViewModel> GetMessagesAsync(EDIMessageSearchInput input);
        Task<(byte[] Content, string FileName, string ContentType)?> DownloadPayloadAsync(Guid id);
        Task<EDIActionModalViewModel?> GetActionModalInfoAsync(int messageTypeId, string actionName);

        //Generate EDI Messages
        Task<string> GenerateEPLAsync(Guid voyageId, Guid portId, List<Guid> blIds, int CompanyId);
        Task<string> GenerateCoparnAsync(Guid voyageId, Guid portId, List<Guid> blIds, int CompanyId);
        Task<string> GenerateTCTSFile(Guid voyageId, Guid portId, List<Guid> blIds, int CompanyId);
    }
}
