namespace ShipRate.Application.Interfaces
{
    public interface ISecurityEventService
    {
        Task LogFailedLoginAsync(string email, string reason);
        Task LogSuccessfulLoginAsync(Guid userId);
        Task LogLockoutAsync(Guid userId);
        Task LogPasswordChangeAsync(Guid userId);
        Task LogPasswordResetAsync(string email);
        Task<int> GetRecentFailedLoginsCountAsync(string email, int minutes = 15);
    }
}
