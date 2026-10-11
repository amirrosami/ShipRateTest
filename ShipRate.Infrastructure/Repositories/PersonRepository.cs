using Microsoft.EntityFrameworkCore;
using ShipRate.Domain.Entities.BaseInfo.Persons;
using ShipRate.Domain.Interfaces;
using ShipRate.Infrastructure.Data;

namespace ShipRate.Infrastructure.Repositories
{
    public class PersonRepository : Repository<Person>, IPersonRepository
    {
        public PersonRepository(ApplicationDbContext context) : base(context)
        {
        }

        public async Task<IEnumerable<Person>> GetAllWithForeignAsync()
        {
            return await _dbSet
                .Include(m => m.Country)
                .Include(m => m.City)
                .Include(m => m.PersonalityType)
                .ToListAsync();
        }

        public async Task<IQueryable<Person>> GetAllAsQueryableAsync()
        {
            return _context.Set<Person>()
                .Include(m => m.Country)
                .Include(m => m.City)
                .Include(m => m.PersonalityType)
                .AsNoTracking();
        }
        public async Task<Person?> GetByIdWithForeignAsync(Guid id)
        {
            return await _dbSet
                .Include(m => m.Country)
                .Include(m => m.City)
                .Include(m => m.PersonalityType)
                .Include(m => m.PersonContacts)
                .Include(m => m.PersonExtraInfos)
                .ThenInclude(m => m.InfoType)
                .Include(m => m.BankAccounts)
                .ThenInclude(m => m.Currency)
                .Include(m => m.BankAccounts)
                .ThenInclude(m => m.Bank)
                .Include(m => m.BankAccounts)
                .ThenInclude(m => m.BankAccountPurpose)
                .FirstOrDefaultAsync(m => m.Id == id);
        }

        //public async Task<MeasurementUnit?> GetByGuidWithCategoryAsync(Guid id)
        //{
        //    return await _dbSet
        //        .Include(m => m.MeasurementUnitCategory)
        //        .FirstOrDefaultAsync(m => m.Id == id);
        //}
    }
}
