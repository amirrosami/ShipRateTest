namespace ShipRate.Web.Models
{
    public class UserViewModel
    {
        public Guid Id { get; set; }
        public string UserName { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string FullName { get; set; } = string.Empty;
        public string PersonName { get; set; } = string.Empty;
        public string JobTitle { get; set; } = string.Empty;
        public string UserTypeName { get; set; } = string.Empty;
        public string SalesFilePrefix { get; set; } = string.Empty;
        public bool IsActive { get; set; }
        public List<string> Roles { get; set; } = new();
        public bool EmailConfirmed { get; set; }
        public DateTimeOffset? LockoutEnd { get; set; }
    }
}
