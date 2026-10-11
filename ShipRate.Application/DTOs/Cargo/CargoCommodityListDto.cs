namespace ShipRate.Application.DTOs.Cargo
{
    public class CargoCommodityListDto
    {
        public Guid Id { get; set; }
        public string CargoCommodityName { get; set; }
        public string CargoCommodityLocalName { get; set; }
        public bool IsActive { get; set; }
    }
}
