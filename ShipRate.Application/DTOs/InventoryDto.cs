using System.ComponentModel.DataAnnotations;

namespace ShipRate.Application.DTOs
{
    public class InventoryDto
    {
        public Guid Id { get; set; }
        public string InventoryCode { get; set; } = string.Empty;
        public string InventoryName { get; set; } = string.Empty;
        public string CompanyName { get; set; } = string.Empty;
        public int CompanyId { get; set; }
        public string Remarks { get; set; }
    }

    public class InventoryCreateDto
    {
        public string InventoryCode { get; set; } = string.Empty;
        public string InventoryName { get; set; } = string.Empty;
        public int CompanyId { get; set; }
        public string Remarks { get; set; }
    }


    public class InventoryUpdateDto
    {
        public Guid Id { get; set; }
        public string InventoryCode { get; set; } = string.Empty;
        public string InventoryName { get; set; } = string.Empty;
        public int CompanyId { get; set; }
        public string Remarks { get; set; }
    }

    public class ProductCategoryDto
    {
        public Guid Id { get; set; }
        public string ProductCategoryName { get; set; } = string.Empty;
        public bool IsActive { get; set; } = true;
        public bool IsDeleted { get; set; } = true;

    }
    public class ProductCategoryCreateDto
    {

        [Required(ErrorMessage = "Product Category Name is required.")]
        [StringLength(200, ErrorMessage = "Name cannot exceed 50 characters.")]
        public string ProductCategoryName { get; set; } = string.Empty;

        public bool IsActive { get; set; } = true;
    }

    public class ProductCategoryUpdateDto
    {
        public Guid Id { get; set; }

        [Required(ErrorMessage = "Product Category Name is required.")]
        [StringLength(200, ErrorMessage = "Name cannot exceed 50 characters.")]
        public string ProductCategoryName { get; set; } = string.Empty;
        public bool IsActive { get; set; } = true;
        public int? pageNumber { get; set; }
    }


    public class ProductDto
    {
        public Guid Id { get; set; }
        public Guid ProductCategoryId { get; set; }
        public string ProductCategoryName { get; set; } = string.Empty;
        public string ProductCode { get; set; } = string.Empty;
        public string ProductName { get; set; } = string.Empty;
        public decimal ProductMinStock { get; set; }
        public bool IsActive { get; set; } = true;
        public bool IsDeleted { get; set; } = true;
    }
    public class ProductCreateDto
    {

        [Required(ErrorMessage = "Product Category is required.")]
        public Guid ProductCategoryId { get; set; }

        [Required(ErrorMessage = "Product Code is required.")]
        [StringLength(20, ErrorMessage = "Name cannot exceed {0} characters.")]
        public string ProductCode { get; set; } = string.Empty;

        [Required(ErrorMessage = "Product Name is required.")]
        [StringLength(50, ErrorMessage = "Name cannot exceed 50 characters.")]
        public string ProductName { get; set; } = string.Empty;

        public decimal ProductMinStock { get; set; }

        public bool IsActive { get; set; } = true;
    }

    public class ProductUpdateDto
    {
        public Guid Id { get; set; }

        [Required(ErrorMessage = "Product Category is required.")]
        public Guid ProductCategoryId { get; set; }

        [Required(ErrorMessage = "Product Code is required.")]
        [StringLength(20, ErrorMessage = "Name cannot exceed {0} characters.")]
        public string ProductCode { get; set; } = string.Empty;

        [Required(ErrorMessage = "Product Name is required.")]
        [StringLength(50, ErrorMessage = "Name cannot exceed 50 characters.")]
        public string ProductName { get; set; } = string.Empty;

        public decimal ProductMinStock { get; set; }

        public bool IsActive { get; set; } = true;
    }
}
