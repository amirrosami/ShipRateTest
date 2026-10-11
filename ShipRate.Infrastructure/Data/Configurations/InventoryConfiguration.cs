using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using ShipRate.Domain.Entities.InventoryOperation;

namespace ShipRate.Infrastructure.Data.Configurations
{

    public class ProductCategoryConfiguration : IEntityTypeConfiguration<ProductCategory>
    {
        public void Configure(EntityTypeBuilder<ProductCategory> builder)
        {
            builder.ToTable("ProductCategory", "inv");
            builder.HasKey(c => c.Id);
            builder.Property(x => x.ProductCategoryName).IsRequired().HasMaxLength(50);
        }
    }

    public class ProductConfiguration : IEntityTypeConfiguration<Product>
    {
        public void Configure(EntityTypeBuilder<Product> builder)
        {
            builder.ToTable("Products", "inv");
            builder.HasKey(c => c.Id);
            builder.Property(x => x.ProductCode).IsRequired().HasMaxLength(20);
            builder.Property(x => x.ProductName).IsRequired().HasMaxLength(50);

            builder.HasOne(x => x.ProductCategory).WithMany().HasForeignKey(x => x.ProductCategoryId);
        }
    }

    public class InventoryConfiguration : IEntityTypeConfiguration<Inventory>
    {
        public void Configure(EntityTypeBuilder<Inventory> builder)
        {
            builder.ToTable("Inventories", "inv");
            builder.HasKey(c => c.Id);
            builder.Property(x => x.InventoryCode).IsRequired().HasMaxLength(10);
            builder.Property(x => x.InventoryName).IsRequired().HasMaxLength(50);
            builder.Property(x => x.Remarks).IsRequired().HasMaxLength(100);

            builder.HasOne(x => x.Company).WithMany().HasForeignKey(x => x.CompanyId);
        }
    }

    public class InventoryProductConfiguration : IEntityTypeConfiguration<InventoryProduct>
    {
        public void Configure(EntityTypeBuilder<InventoryProduct> builder)
        {
            builder.ToTable("InventoryProducts", "inv");
            builder.HasKey(c => c.Id);

            builder.HasOne(x => x.Inventory).WithMany().HasForeignKey(x => x.InventoryId);
            builder.HasOne(x => x.Product).WithMany().HasForeignKey(x => x.ProductId);
        }
    }

    public class ProductRequestConfiguration : IEntityTypeConfiguration<ProductRequest>
    {
        public void Configure(EntityTypeBuilder<ProductRequest> builder)
        {
            builder.ToTable("ProductRequests", "inv");
            builder.HasKey(c => c.Id);
            builder.Property(x => x.Remarks).IsRequired().HasMaxLength(200);
        }
    }

    public class ProductRequestItemConfiguration : IEntityTypeConfiguration<ProductRequestItem>
    {
        public void Configure(EntityTypeBuilder<ProductRequestItem> builder)
        {
            builder.ToTable("ProductRequestItems", "inv");
            builder.HasKey(c => c.Id);
            builder.HasOne(x => x.Product).WithMany().HasForeignKey(x => x.ProductId);
            builder.HasOne(x => x.ProductRequest).WithMany(x => x.Items).HasForeignKey(x => x.ProductRequestId);
        }
    }
    public class ProductStockTransactionConfiguration : IEntityTypeConfiguration<ProductStockTransaction>
    {
        public void Configure(EntityTypeBuilder<ProductStockTransaction> builder)
        {
            builder.ToTable("ProductStockTransactions", "inv");
            builder.HasKey(c => c.Id);
            builder.Property(x => x.Reference).HasMaxLength(50);
            builder.Property(x => x.Remarks).HasMaxLength(200);
            builder.HasOne(x => x.Product).WithMany().HasForeignKey(x => x.ProductId);
            builder.HasOne(x => x.Inventory).WithMany().HasForeignKey(x => x.InventoryId);
            builder.HasOne(x => x.ProductRequestItem).WithMany().HasForeignKey(x => x.ProductRequestItemId);
            builder.HasOne(x => x.ProductTransactionType).WithMany().HasForeignKey(x => x.ProductTransactionTypeId);
        }
    }
}
