using ShipRate.Domain.Entities.BaseInfo.Persons;

namespace ShipRate.Domain.Interfaces
{
    public interface IPersonRepository : IRepository<Person>
    {
        Task<IEnumerable<Person>> GetAllWithForeignAsync();
        Task<Person?> GetByIdWithForeignAsync(Guid id);
        Task<IQueryable<Person>> GetAllAsQueryableAsync();
    }
}
