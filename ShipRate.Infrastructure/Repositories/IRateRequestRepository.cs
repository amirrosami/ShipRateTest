using ShipRate.Domain.Entities.RateQuotation;
using ShipRate.Domain.Interfaces;

namespace ShipRate.Infrastructure.Repositories
{
    public interface IRateRequestRepository : IRepository<RateRequest>
    {
        Task<RateRequest> GetByIdWithDetailsAsync(Guid id);
        //Task<(IEnumerable<RateRequest> Items, int TotalCount)> SearchAsync(RateRequestSearchDto searchDto);
    }
}
