namespace ShipRate.Application.Interfaces
{
    public interface IRequestContext
    {
        int CompanyId { get; }
        Guid UserId { get; }
    }

    public class RequestContext : IRequestContext
    {
        public ICurrentUserService _currentUserService { get; set; }
        public RequestContext(ICurrentUserService currentUserService)
        {
            _currentUserService = currentUserService;
        }
        public int CompanyId => 1; // replace with real claim/session-based resolution
        //public Guid UserId => Guid.Parse("11111111-1111-1111-1111-111111111111"); // replace
        public Guid UserId => _currentUserService.UserId;
    }
}
