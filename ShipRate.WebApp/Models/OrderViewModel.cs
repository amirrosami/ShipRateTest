using Microsoft.AspNetCore.Mvc.Rendering;
using System.ComponentModel.DataAnnotations;

namespace ShipRate.WebApp.Models
{
    public enum OrderStatus
    {
        Pending,
        Confirmed,
        Processing,
        Shipped,
        Delivered,
        Cancelled
    }
    // ViewModels/OrderViewModel.cs
    public class OrderViewModel
    {
        public int Id { get; set; }

        [Required]
        [Display(Name = "Order Number")]
        public string OrderNumber { get; set; }

        [Required]
        [Display(Name = "Order Date")]
        public DateTime OrderDate { get; set; }

        [Required]
        [Display(Name = "Customer Name")]
        public string CustomerName { get; set; }

        [EmailAddress]
        [Display(Name = "Customer Email")]
        public string CustomerEmail { get; set; }

        [Display(Name = "Phone")]
        public string CustomerPhone { get; set; }

        [Required]
        [Display(Name = "Shipping Address")]
        public string ShippingAddress { get; set; }
        public double TotalAmount { get; set; }

        public int Status { get; set; }

        public string Notes { get; set; }

        public List<OrderDetailViewModel> OrderDetails { get; set; } = new();
    }

    public class OrderDetailViewModel
    {
        public int Id { get; set; }
        public bool IsDeleted { get; set; } = false;

        public int OrderId { get; set; }
        public string ProductName { get; set; }
        [Required]
        public int ProductId { get; set; }

        [Required]
        [Range(1, int.MaxValue)]
        public int Quantity { get; set; }

        [Required]
        [Range(0.01, double.MaxValue)]
        public decimal UnitPrice { get; set; }

        [Range(0, 100)]
        public decimal DiscountPercent { get; set; }

        public decimal LineTotal { get; set; }

        public string Description { get; set; }
    }
}
