namespace ShipRate.Web.Models
{
    public class UserClaimsViewModel
    {
        public string UserId { get; set; } = string.Empty;
        public string UserName { get; set; } = string.Empty;
        public List<ClaimItem> Claims { get; set; } = new();
    }

    public class ClaimItem
    {
        public string Type { get; set; } = string.Empty;
        public string DisplayName { get; set; } = string.Empty;
        public bool IsSelected { get; set; }
    }
}
