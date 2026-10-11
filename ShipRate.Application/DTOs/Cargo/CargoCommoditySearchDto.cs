namespace ShipRate.Application.DTOs.Cargo
{
    public class CargoCommoditySearchDto
    {
        public string SearchName { get; set; }
        public string SearchLocalName { get; set; }
        public int PageNumber { get; set; } = 1;
        public int PageSize { get; set; } = 10;
        public string SortColumn { get; set; } = "CreatedAt";
        public string SortDirection { get; set; } = "desc";
    }
}
