using ShipRate.Application.DTOs.BillOfLadings;

namespace ShipRate.Application.Interfaces.BillOfLadings
{
    public interface IBLService
    {
        Task<BLUpsertDto> GetCreateModelAsync(BLContextDto context);
        Task<BLUpsertDto> GetEditModelAsync(Guid id);
        Task<(bool Success, string Message, Guid? Id)> SaveAsync(BLUpsertDto dto);
        Task<bool> DeleteAsync(Guid id);
        Task<BLLookupDto> GetLookupsAsync();
    }
}
