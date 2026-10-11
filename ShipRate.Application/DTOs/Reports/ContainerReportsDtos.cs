namespace ShipRate.Application.DTOs.Reports
{
    public class Rpt_ContainerDto
    {
        public Guid Id { get; set; }
        public string ContainerNumber { get; set; }
        public string ShortName { get; set; }
        public string ContainerSizeName { get; set; }
        public string ContainerType { get; set; }
        public string Remarks { get; set; }
        public decimal TareWeight { get; set; }
        public bool IsActive { get; set; }
    }
}
