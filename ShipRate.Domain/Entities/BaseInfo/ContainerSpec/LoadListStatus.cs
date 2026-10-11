namespace ShipRate.Domain.Entities.BaseInfo.ContainerSpec
{
    public class LoadListStatus
    {
        public int Id { get; set; }

        public string Code { get; set; } = null!;

        public string Name { get; set; } = null!;

        public string? Description { get; set; }

        public int SortOrder { get; set; }

        public bool IsActive { get; set; }
    }
}
