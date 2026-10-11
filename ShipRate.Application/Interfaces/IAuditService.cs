namespace ShipRate.Application.Interfaces
{
    public interface IAuditService
    {
        Task LogAsync(
            string action,
            string entityName,
            string? entityId = null,
            object? oldValues = null,
            object? newValues = null
        );
    }
}
