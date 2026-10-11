using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using ShipRate.Application.DTOs;
using ShipRate.Application.Interfaces;
using ShipRate.Domain.Entities.InventoryOperation;
using ShipRate.Infrastructure.Data;

namespace ShipRate.Application.Services
{
    public class ProductStockTransactionService : IProductStockTransactionService
    {
        private static readonly int[] AllowedCreateTypeIds = [2, 3, 4];

        private readonly ApplicationDbContext _db;

        public ProductStockTransactionService(ApplicationDbContext db)
        {
            _db = db;
        }

        public async Task<PagedResultDto<ProductStockTransactionListDto>> GetPagedAsync(
            ProductStockTransactionFilterDto filter,
            CancellationToken cancellationToken = default)
        {
            NormalizePaging(filter);

            var query =
                from t in _db.ProductStockTransactions.AsNoTracking()
                join inv in _db.Inventories.AsNoTracking() on t.InventoryId equals inv.Id
                join p in _db.Products.AsNoTracking() on t.ProductId equals p.Id
                join tt in _db.ProductTransactionTypes.AsNoTracking() on t.ProductTransactionTypeId equals tt.Id
                where !t.IsDeleted
                select new
                {
                    t.Id,
                    t.InventoryId,
                    t.ProductId,
                    t.ProductTransactionTypeId,
                    InventoryName = inv.InventoryName,
                    p.ProductCode,
                    p.ProductName,
                    t.Quantity,
                    TransactionTypeName = tt.ProductTransactionTypeName,
                    t.Reference,
                    t.TransactionDate,
                    t.Remarks
                };

            if (filter.InventoryId.HasValue)
                query = query.Where(x => x.InventoryId == filter.InventoryId.Value);

            if (filter.ProductId.HasValue)
                query = query.Where(x => x.ProductId == filter.ProductId.Value);

            if (filter.ProductTransactionTypeId.HasValue)
                query = query.Where(x => x.ProductTransactionTypeId == filter.ProductTransactionTypeId.Value);

            if (filter.FromDate.HasValue)
                query = query.Where(x => x.TransactionDate >= filter.FromDate.Value);

            if (filter.ToDate.HasValue)
            {
                var toDate = filter.ToDate.Value.Date.AddDays(1);
                query = query.Where(x => x.TransactionDate < toDate);
            }

            if (!string.IsNullOrWhiteSpace(filter.SearchText))
            {
                var search = filter.SearchText.Trim();
                query = query.Where(x =>
                    x.InventoryName.Contains(search) ||
                    x.ProductCode.Contains(search) ||
                    x.ProductName.Contains(search) ||
                    x.Reference.Contains(search) ||
                    x.Remarks.Contains(search));
            }

            var totalCount = await query.CountAsync(cancellationToken);

            var items = await query
                .OrderByDescending(x => x.TransactionDate)
                .ThenByDescending(x => x.Id)
                .Skip((filter.Page - 1) * filter.PageSize)
                .Take(filter.PageSize)
                .Select(x => new ProductStockTransactionListDto
                {
                    Id = x.Id,
                    InventoryName = x.InventoryName,
                    ProductCode = x.ProductCode,
                    ProductName = x.ProductName,
                    Quantity = x.Quantity,
                    TransactionTypeName = x.TransactionTypeName,
                    Reference = x.Reference,
                    TransactionDate = x.TransactionDate,
                    Remarks = x.Remarks
                })
                .ToListAsync(cancellationToken);

            return new PagedResultDto<ProductStockTransactionListDto>
            {
                Items = items,
                TotalCount = totalCount,
                PageNumber = filter.Page,
                PageSize = filter.PageSize
            };
        }

        public async Task<ProductStockTransactionCreateVm> GetCreateVmAsync(
            CancellationToken cancellationToken = default)
        {
            return new ProductStockTransactionCreateVm
            {
                Input = new CreateProductStockTransactionDto
                {
                    TransactionDate = DateTime.Now
                },
                Inventories = await GetInventorySelectListAsync(cancellationToken),
                Products = await GetProductSelectListAsync(cancellationToken),
                TransactionTypes = await _db.ProductTransactionTypes
                    .AsNoTracking()
                    .Where(x => AllowedCreateTypeIds.Contains(x.Id) && x.IsActive && !x.IsDeleted)
                    .OrderBy(x => x.Id)
                    .Select(x => new SelectListItem
                    {
                        Value = x.Id.ToString(),
                        Text = x.ProductTransactionTypeName
                    })
                    .ToListAsync(cancellationToken)
            };
        }

        public async Task CreateAsync(
            CreateProductStockTransactionDto input,
            Guid userId,
            CancellationToken cancellationToken = default)
        {
            ValidateCreateInput(input);

            await using var transaction = await _db.Database.BeginTransactionAsync(cancellationToken);

            var inventoryProduct = await _db.InventoryProducts
                .FirstOrDefaultAsync(x =>
                    x.InventoryId == input.InventoryId &&
                    x.ProductId == input.ProductId &&
                    !x.IsDeleted,
                    cancellationToken);

            var quantity = ConvertQuantityToInt(input.Quantity);

            if (input.ProductTransactionTypeId == 4)
            {
                if (inventoryProduct == null)
                    throw new InvalidOperationException("Inventory stock record was not found.");

                if (inventoryProduct.Stock < quantity)
                    throw new InvalidOperationException("Stock cannot be negative.");

                inventoryProduct.Stock -= quantity;
                inventoryProduct.UpdatedAt = DateTime.Now;
                inventoryProduct.EditUserId = userId;
            }
            else
            {
                if (inventoryProduct == null)
                {
                    inventoryProduct = new InventoryProduct
                    {
                        Id = Guid.CreateVersion7(),
                        InventoryId = input.InventoryId,
                        ProductId = input.ProductId,
                        Stock = 0,
                        ReOrderLevel = 0,
                        //IsActive = true,
                        IsDeleted = false,
                        CreatedAt = DateTime.Now,
                        CreateUserId = userId
                    };

                    _db.InventoryProducts.Add(inventoryProduct);
                }

                inventoryProduct.Stock += quantity;
                inventoryProduct.UpdatedAt = DateTime.Now;
                inventoryProduct.EditUserId = userId;
            }

            var stockTransaction = new ProductStockTransaction
            {
                Id = Guid.CreateVersion7(),
                InventoryId = input.InventoryId,
                ProductId = input.ProductId,
                Quantity = input.Quantity,
                ProductTransactionTypeId = input.ProductTransactionTypeId,
                ProductRequestItemId = null,
                Reference = input.Reference.Trim(),
                TransactionDate = input.TransactionDate,
                Remarks = input.Remarks?.Trim() ?? "",
                //IsActive = true,
                IsDeleted = false,
                CreatedAt = DateTime.Now,
                CreateUserId = userId
            };

            _db.ProductStockTransactions.Add(stockTransaction);

            await _db.SaveChangesAsync(cancellationToken);
            await transaction.CommitAsync(cancellationToken);
        }

        public async Task<PagedResultDto<InventoryProductStockDto>> GetInventoryStocksAsync(
            InventoryProductStockFilterDto filter,
            CancellationToken cancellationToken = default)
        {
            NormalizePaging(filter);

            var query =
                from ip in _db.InventoryProducts.AsNoTracking()
                join inv in _db.Inventories.AsNoTracking() on ip.InventoryId equals inv.Id
                join p in _db.Products.AsNoTracking() on ip.ProductId equals p.Id
                where !ip.IsDeleted
                select new
                {
                    ip.Id,
                    ip.InventoryId,
                    ip.ProductId,
                    inv.InventoryName,
                    p.ProductCode,
                    p.ProductName,
                    ip.Stock,
                    ip.ReOrderLevel
                };

            if (filter.InventoryId.HasValue)
                query = query.Where(x => x.InventoryId == filter.InventoryId.Value);

            if (filter.ProductId.HasValue)
                query = query.Where(x => x.ProductId == filter.ProductId.Value);

            if (!string.IsNullOrWhiteSpace(filter.SearchText))
            {
                var search = filter.SearchText.Trim();
                query = query.Where(x =>
                    x.InventoryName.Contains(search) ||
                    x.ProductCode.Contains(search) ||
                    x.ProductName.Contains(search));
            }

            var totalCount = await query.CountAsync(cancellationToken);

            var items = await query
                .OrderBy(x => x.InventoryName)
                .ThenBy(x => x.ProductName)
                .Skip((filter.Page - 1) * filter.PageSize)
                .Take(filter.PageSize)
                .Select(x => new InventoryProductStockDto
                {
                    Id = x.Id,
                    InventoryName = x.InventoryName,
                    ProductCode = x.ProductCode,
                    ProductName = x.ProductName,
                    Stock = x.Stock,
                    ReOrderLevel = x.ReOrderLevel
                })
                .ToListAsync(cancellationToken);

            return new PagedResultDto<InventoryProductStockDto>
            {
                Items = items,
                TotalCount = totalCount,
                PageNumber = filter.Page,
                PageSize = filter.PageSize
            };
        }

        public Task<List<SelectListItem>> GetInventorySelectListAsync(CancellationToken cancellationToken = default)
        {
            return _db.Inventories
                .AsNoTracking()
                .Where(x => !x.IsDeleted)
                .OrderBy(x => x.InventoryName)
                .Select(x => new SelectListItem
                {
                    Value = x.Id.ToString(),
                    Text = x.InventoryName
                })
                .ToListAsync(cancellationToken);
        }

        public Task<List<SelectListItem>> GetProductSelectListAsync(CancellationToken cancellationToken = default)
        {
            return _db.Products
                .AsNoTracking()
                .Where(x => x.IsActive && !x.IsDeleted)
                .OrderBy(x => x.ProductName)
                .Select(x => new SelectListItem
                {
                    Value = x.Id.ToString(),
                    Text = x.ProductCode + " - " + x.ProductName
                })
                .ToListAsync(cancellationToken);
        }

        private static void ValidateCreateInput(CreateProductStockTransactionDto input)
        {
            if (!AllowedCreateTypeIds.Contains(input.ProductTransactionTypeId))
                throw new InvalidOperationException("Invalid transaction type.");

            if (input.InventoryId == Guid.Empty)
                throw new InvalidOperationException("Inventory is required.");

            if (input.ProductId == Guid.Empty)
                throw new InvalidOperationException("Product is required.");

            if (input.Quantity <= 0)
                throw new InvalidOperationException("Quantity must be greater than zero.");

            if (string.IsNullOrWhiteSpace(input.Reference))
                throw new InvalidOperationException("Reference is required.");
        }

        private static int ConvertQuantityToInt(decimal quantity)
        {
            if (quantity % 1 != 0)
                throw new InvalidOperationException("Quantity must be a whole number because stock is integer.");

            if (quantity > int.MaxValue)
                throw new InvalidOperationException("Quantity is too large.");

            return Convert.ToInt32(quantity);
        }

        private static void NormalizePaging(ProductStockTransactionFilterDto filter)
        {
            filter.Page = filter.Page < 1 ? 1 : filter.Page;
            filter.PageSize = filter.PageSize is 10 or 20 or 50 ? filter.PageSize : 10;
        }

        private static void NormalizePaging(InventoryProductStockFilterDto filter)
        {
            filter.Page = filter.Page < 1 ? 1 : filter.Page;
            filter.PageSize = filter.PageSize is 10 or 20 or 50 ? filter.PageSize : 10;
        }





        public async Task<DeliverProductRequestVm> GetDeliverVmAsync(Guid productRequestId, CancellationToken cancellationToken = default)
        {
            var requestExists = await _db.ProductRequests
                .AsNoTracking()
                .AnyAsync(x => x.Id == productRequestId && !x.IsDeleted, cancellationToken);

            if (!requestExists)
                throw new InvalidOperationException("Request not found.");

            return new DeliverProductRequestVm
            {
                Input = new DeliverProductRequestDto
                {
                    ProductRequestId = productRequestId
                },
                Inventories = await _db.Inventories
                    .AsNoTracking()
                    .Where(x => x.IsActive && !x.IsDeleted)
                    .OrderBy(x => x.InventoryName)
                    .Select(x => new SelectListItem
                    {
                        Value = x.Id.ToString(),
                        Text = x.InventoryName
                    })
                    .ToListAsync(cancellationToken)
            };
        }

        public async Task DeliverAsync(DeliverProductRequestDto input, Guid userId, CancellationToken cancellationToken = default)
        {
            if (input.ProductRequestId == Guid.Empty)
                throw new InvalidOperationException("Request is required.");

            if (input.InventoryId == Guid.Empty)
                throw new InvalidOperationException("Inventory is required.");

            await using var transaction = await _db.Database.BeginTransactionAsync(cancellationToken);

            var request = await _db.ProductRequests
                .FirstOrDefaultAsync(x => x.Id == input.ProductRequestId && !x.IsDeleted, cancellationToken);

            if (request == null)
                throw new InvalidOperationException("Request not found.");

            if (request.ProductRequestStatusId != 2) // مثلا Approved
                throw new InvalidOperationException("Only approved requests can be delivered.");

            var items = await _db.ProductRequestItems
                .Include(x => x.Product)
                .Where(x => x.ProductRequestId == input.ProductRequestId && !x.IsDeleted)
                .ToListAsync(cancellationToken);

            if (!items.Any())
                throw new InvalidOperationException("Request has no items.");

            var productIds = items.Select(x => x.ProductId).Distinct().ToList();

            var inventoryProducts = await _db.InventoryProducts
                .Where(x => x.InventoryId == input.InventoryId && productIds.Contains(x.ProductId) && !x.IsDeleted)
                .ToListAsync(cancellationToken);

            foreach (var item in items)
            {
                var stockRow = inventoryProducts.FirstOrDefault(x => x.ProductId == item.ProductId);

                if (stockRow == null)
                    throw new InvalidOperationException($"Stock not found for product {item.Product.ProductName} in selected inventory.");

                var requestedQty = Convert.ToInt32(item.Quantity);

                if (stockRow.Stock < requestedQty)
                    throw new InvalidOperationException("Selected inventory does not have enough stock for all requested items.");
            }

            foreach (var item in items)
            {
                var stockRow = inventoryProducts.First(x => x.ProductId == item.ProductId);
                var requestedQty = Convert.ToInt32(item.Quantity);

                stockRow.Stock -= requestedQty;
                stockRow.UpdatedAt = DateTime.Now;
                stockRow.EditUserId = userId;

                _db.ProductStockTransactions.Add(new ProductStockTransaction
                {
                    Id = Guid.CreateVersion7(),
                    InventoryId = input.InventoryId,
                    ProductId = item.ProductId,
                    Quantity = item.Quantity,
                    ProductTransactionTypeId = 1, // Deliver
                    ProductRequestItemId = item.Id,
                    Reference = request.RequestNo.ToString(),
                    TransactionDate = DateTime.Now,
                    Remarks = "Delivered against product request",
                    //IsActive = true,
                    IsDeleted = false,
                    CreatedAt = DateTime.Now,
                    CreateUserId = userId
                });
            }

            request.ProductRequestStatusId = 3;
            request.DeliveredByStoreKeeperId = userId;
            request.DeliveredAt = DateTime.Now;
            //request.UpdatedAt = DateTime.Now;
            //request.EditUserId = userId;

            await _db.SaveChangesAsync(cancellationToken);
            await transaction.CommitAsync(cancellationToken);
        }








    }
}
