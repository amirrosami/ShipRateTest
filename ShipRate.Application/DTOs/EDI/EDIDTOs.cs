namespace ShipRate.Application.DTOs.EDI
{
    public class EDIMessageTypeRowDto
    {
        public int Id { get; set; }
        public string EDIMessageTypeCode { get; set; } = string.Empty;
        public string EDIMessageTypeName { get; set; } = string.Empty;
        public string EDIMessageTypeStandard { get; set; } = string.Empty;
        public bool IsActive { get; set; }
    }

    public class EDIMessageRowDto
    {
        public Guid Id { get; set; }
        public int MessageTypeId { get; set; }
        public string MessageTypeCode { get; set; } = string.Empty;
        public string InOutName { get; set; } = string.Empty;
        public string ReferenceNumber { get; set; } = string.Empty;
        public DateTime? ReceivedAt { get; set; }
        public DateTime? ProcessedAt { get; set; }
        public string ErrorMessage { get; set; } = string.Empty;
        public string Remarks { get; set; } = string.Empty;
        public bool IsActive { get; set; }
        public DateTime CreatedAt { get; set; }
    }

    public class EDIMessageSearchInput
    {
        public int MessageTypeId { get; set; }
        public string? SearchTerm { get; set; }
        public int PageNumber { get; set; } = 1;
        public int PageSize { get; set; } = 10;
    }

    public class EDIMessageListViewModel
    {
        public int MessageTypeId { get; set; }
        public string MessageTypeCode { get; set; } = string.Empty;
        public string MessageTypeName { get; set; } = string.Empty;

        public string? SearchTerm { get; set; }
        public int PageNumber { get; set; } = 1;
        public int PageSize { get; set; } = 10;

        public PagedResultDto<EDIMessageRowDto> Result { get; set; } = new();
    }

    public class EDIPageViewModel
    {
        public List<EDIMessageTypeRowDto> MessageTypes { get; set; } = new();
    }

    public class EDIActionModalViewModel
    {
        public int MessageTypeId { get; set; }
        public string MessageTypeCode { get; set; } = string.Empty;
        public string MessageTypeName { get; set; } = string.Empty;
        public string ActionName { get; set; } = string.Empty;
    }



}
