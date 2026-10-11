using Microsoft.AspNetCore.Mvc.Rendering;

namespace ShipRate.Application.DTOs
{
    public class ProductStockTransactionListDto
    {
        public Guid Id { get; set; }
        public string InventoryName { get; set; } = "";
        public string ProductCode { get; set; } = "";
        public string ProductName { get; set; } = "";
        public decimal Quantity { get; set; }
        public string TransactionTypeName { get; set; } = "";
        public string Reference { get; set; } = "";
        public DateTime TransactionDate { get; set; }
        public string Remarks { get; set; } = "";
    }

    public class ProductStockTransactionFilterDto
    {
        public Guid? InventoryId { get; set; }
        public Guid? ProductId { get; set; }
        public int? ProductTransactionTypeId { get; set; }
        public string? SearchText { get; set; }
        public DateTime? FromDate { get; set; }
        public DateTime? ToDate { get; set; }
        public int Page { get; set; } = 1;
        public int PageSize { get; set; } = 10;
    }

    public class CreateProductStockTransactionDto
    {
        public Guid InventoryId { get; set; }
        public Guid ProductId { get; set; }
        public decimal Quantity { get; set; }
        public int ProductTransactionTypeId { get; set; }
        public string Reference { get; set; } = "";
        public DateTime TransactionDate { get; set; } = DateTime.Now;
        public string Remarks { get; set; } = "";
    }

    public class ProductStockTransactionCreateVm
    {
        public CreateProductStockTransactionDto Input { get; set; } = new();
        public List<SelectListItem> Inventories { get; set; } = [];
        public List<SelectListItem> Products { get; set; } = [];
        public List<SelectListItem> TransactionTypes { get; set; } = [];
    }

    public class InventoryProductStockDto
    {
        public Guid Id { get; set; }
        public string InventoryName { get; set; } = "";
        public string ProductCode { get; set; } = "";
        public string ProductName { get; set; } = "";
        public int Stock { get; set; }
        public int ReOrderLevel { get; set; }
    }

    public class InventoryProductStockFilterDto
    {
        public Guid? InventoryId { get; set; }
        public Guid? ProductId { get; set; }
        public string? SearchText { get; set; }
        public int Page { get; set; } = 1;
        public int PageSize { get; set; } = 10;
    }

    public class DeliverProductRequestDto
    {
        public Guid ProductRequestId { get; set; }
        public Guid InventoryId { get; set; }
    }

    public class DeliverProductRequestVm
    {
        public DeliverProductRequestDto Input { get; set; } = new();
        public List<SelectListItem> Inventories { get; set; } = [];
    }


}
