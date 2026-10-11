using ShipRate.Domain.Entities.Finance;

namespace ShipRate.Domain.Interfaces
{
    public interface IBankRepository : IRepository<Bank>
    {
        Task<IEnumerable<Bank>> GetAllWithCountryAsync();
        Task<Bank?> GetByIdWithCountryAsync(Guid id);
        Task<IQueryable<Bank>> GetAllAsQueryableAsync();
        Task<IEnumerable<Bank>> GetByCountryIdAsync(int countryId);
    }
}
