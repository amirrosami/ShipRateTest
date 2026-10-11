namespace ShipRate.Infrastructure.Repositories
{
    public interface IUnitOfWork : IDisposable
    {
        IRateRequestRepository RateRequests { get; }
        Task<int> SaveChangesAsync();
        int SaveChanges();
    }
}
