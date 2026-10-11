namespace ShipRate.WebApp.ViewModels.ContainerViewModels
{
    public class ContainerListVM
    {
        public Guid Id { get; set; }
        public string ContainerNumber { get; set; }
        public string SizeType { get; set; }
        public string OwnLeaseType { get; set; }
        public string CntOwnType { get; set; }
        public string? OwnerName { get; set; }
        public bool IsActive { get; set; }
        public bool ResponsibilityByUs { get; set; }
        public string? ResponsibleCompany { get; set; }
        public int MaxPayLoad { get; set; }
        public int TareWeight { get; set; }
    }
    // ViewModels/ContainerViewModels/ContainerFormVM.cs

}
