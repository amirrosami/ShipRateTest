namespace ShipRate.Application.DTOs
{
    public class BlackListSearchDto
    {
        public string? SearchTerm { get; set; } = "";
        public int? BlackListReasonId { get; set; }
        public bool? IsActiveInBlackList { get; set; }  // true: هنوز خارج نشده، false: خارج شده
        public int PageNumber { get; set; } = 1;
        public int PageSize { get; set; } = 10;
    }
    public class BlackListDto
    {
        public Guid Id { get; set; }
        public Guid PersonId { get; set; }
        public string PersonName { get; set; } = string.Empty;
        public string? NationalId { get; set; }
        public int BlackListReasonId { get; set; }
        public string ReasonTitle { get; set; } = string.Empty;
        public DateTime BlackListEnterDateTime { get; set; }
        public string EnterRemarks { get; set; } = string.Empty;
        public DateTime? BlackListExitDateTime { get; set; }
        public string ExitRemarks { get; set; } = string.Empty;
        public bool IsExited => BlackListExitDateTime.HasValue;
    }

    // مدل ایجاد ورود به لیست سیاه
    public class CreateBlackListDto
    {
        public Guid PersonId { get; set; }
        public int BlackListReasonId { get; set; }
        public string BlackListEnterRemarks { get; set; } = string.Empty;
    }

    // مدل خروج از لیست سیاه
    public class ExitBlackListDto
    {
        public Guid Id { get; set; }
        public string BlackListExitRemarks { get; set; } = string.Empty;
    }



}
