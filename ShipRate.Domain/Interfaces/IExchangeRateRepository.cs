using ShipRate.Domain.Entities.Finance;

namespace ShipRate.Domain.Interfaces
{
    public interface IExchangeRateRepository : IRepository<ExchangeRate>
    {
        Task<IEnumerable<ExchangeRate>> GetAllWithRelationsAsync();
        Task<ExchangeRate?> GetByIdWithRelationsAsync(Guid id);
        Task<IQueryable<ExchangeRate>> GetAllAsQueryableAsync();
    }
}
