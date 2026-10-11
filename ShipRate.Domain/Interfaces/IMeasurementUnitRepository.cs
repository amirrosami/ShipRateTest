using ShipRate.Domain.Entities.BaseInfo.Generals;

namespace ShipRate.Domain.Interfaces
{
    public interface IMeasurementUnitRepository : IRepository<MeasurementUnit>
    {
        Task<IEnumerable<MeasurementUnit>> GetAllWithCategoryAsync();
        Task<MeasurementUnit?> GetByIdWithCategoryAsync(int id);
        //Task<MeasurementUnit?> GetByGuidWithCategoryAsync(Guid id);
        Task<IQueryable<MeasurementUnit>> GetAllAsQueryableAsync();
        Task<IEnumerable<MeasurementUnit>> GetByCategoryIdAsync(int categoryId);
    }
}
