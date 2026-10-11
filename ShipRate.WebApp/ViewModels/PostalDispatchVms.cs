using System.ComponentModel.DataAnnotations;

namespace ShipRate.WebApp.ViewModels
{
    public class PostalDispatchViewModel
    {
        public Guid? Id { get; set; }

        [Required]
        public DateTime DispatchDate { get; set; } = DateTime.Now;

        [Required]
        public Guid SenderId { get; set; }

        [Required]
        public Guid ReceiverId { get; set; }

        public Guid? CourierCompanyId { get; set; }

        [Required, MaxLength(50)]
        public string TrackingNumber { get; set; } = string.Empty;

        [Required]
        public int CourierMethodId { get; set; }

        [Required]
        public int StatusId { get; set; } = 1;

        public List<PostalDispatchItemViewModel> Items { get; set; } = new();
    }

    public class PostalDispatchItemViewModel
    {
        public Guid? Id { get; set; }
        public Guid? ObjectId { get; set; }
        public bool IsOriginal { get; set; }

        [Required, MaxLength(500)]
        public string DocumentDescription { get; set; } = string.Empty;

        [MaxLength(100)]
        public string ItemReference { get; set; } = string.Empty;
    }

    public class PostalDispatchSearchViewModel
    {
        public string? TrackingNumber { get; set; }
        public string? SenderName { get; set; }
        public string? ReceiverName { get; set; }
        public string? DocumentDescription { get; set; }
        public string? ItemReference { get; set; }
        public int? StatusId { get; set; }
        public DateTime? FromDate { get; set; }
        public DateTime? ToDate { get; set; }
        public int Page { get; set; } = 1;
        public int PageSize { get; set; } = 10;
    }

    public class PostalDispatchConfirmViewModel
    {
        [Required]
        public Guid PostalDispatchId { get; set; }

        [Required]
        public DateTime ConfirmationDate { get; set; } = DateTime.Now;

        [Required, MaxLength(50)]
        public string ConditionOnReceipt { get; set; } = string.Empty;

        [MaxLength(200)]
        public string Remarks { get; set; } = string.Empty;
    }

    public class DispatchGridRow
    {
        public Guid Id { get; set; }
        public DateTime DispatchDate { get; set; }
        public string SenderName { get; set; } = string.Empty;
        public string ReceiverName { get; set; } = string.Empty;
        public string CourierCompanyName { get; set; } = string.Empty;
        public string TrackingNumber { get; set; } = string.Empty;
        public string CourierMethodName { get; set; } = string.Empty;
        public string StatusName { get; set; } = string.Empty;
        public int StatusId { get; set; }
        public bool IsActive { get; set; }
    }

    public class ItemGridRow
    {
        public Guid Id { get; set; }
        public Guid PostalDispatchId { get; set; }
        public string DocumentDescription { get; set; } = string.Empty;
        public string ItemReference { get; set; } = string.Empty;
        public bool IsOriginal { get; set; }
        public bool IsActive { get; set; }
    }

    public class PagedResult<T>
    {
        public List<T> Items { get; set; } = new();
        public int TotalCount { get; set; }
        public int Page { get; set; }
        public int PageSize { get; set; }
        public int TotalPages => (int)Math.Ceiling(TotalCount / (double)PageSize);
        public string SearchTerm { get; set; }
        public string SortColumn { get; set; }
        public string SortDirection { get; set; }
        public object CustomFilterValues { get; set; }

        public PagedResult<object> toObject()
        {
            return new PagedResult<object>
            {
                Items = this.Items.Select(x => (object)x).ToList(),
                TotalCount = this.TotalCount,
                Page = this.Page,
                PageSize = this.PageSize,
                SearchTerm = this.SearchTerm,
                SortDirection = this.SortDirection,
                CustomFilterValues = this.CustomFilterValues,
                SortColumn = this.SortColumn,
            };
        }
    }

    public class GridViewModel
    {
        public List<GridColumn> columns { get; set; }
        public PagedResult<object> Data { get; set; }
        public string ListUrl { get; set; }
        public string GridPartialName { get; set; }
    }


    public class DetailGridViewModel
    {
        public Guid MasterId { get; set; }
        public List<GridColumn> Columns { get; set; }
        public List<object> Data { get; set; }
        public string ListUrl { get; set; }
        public string DetailGridPartialName { get; set; }
        public string Controller { get; set; }
        public string MasterTitle { get; set; }
        public string MasterDisplayValue { get; set; }
        public string CreateAction { get; set; }

    }


    public class GridColumn
    {
        public string Name { get; set; }
        public string DisplayName { get; set; }
        public GridColumn()
        {
            
        }

        public GridColumn(string name, string displayName)
        {
            Name = name;
            DisplayName = displayName;
        }
    }
}
