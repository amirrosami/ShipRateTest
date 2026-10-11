namespace ShipRate.Application.DTOs
{

    public class PagedRequestDto<TRequest> : PagedRequestDto
    {
        public TRequest Filters { get; set; }
    }
    public class PagedRequestDto
    {
        private int _pageNumber = 1;
        private int _pageSize = 10;

        public int Page { get; set; } = 1;
        public int PageNumber
        {
            get => _pageNumber;
            set => _pageNumber = value < 1 ? 1 : value;
        }

        public int PageSize
        {
            get => _pageSize;
            set => _pageSize = value switch
            {
               
                < 1 => 5,
                > 100 => 100,
                _ => value
            };
        }

        public string? SearchTerm { get; set; }
        public string SortColumn { get; set; }
        public string SortDirection { get; set; }
        public bool? IsActive { get; set; }
    }
}
