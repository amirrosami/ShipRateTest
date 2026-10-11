using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using ShipRate.Application.DTOs;
using ShipRate.Application.Interfaces;
using ShipRate.Domain.Entities.Finance;
using ShipRate.Infrastructure.Data;

namespace ShipRate.Application.Services
{
    public class VaultService : IVaultService
    {
        private readonly ApplicationDbContext _db;
        public VaultService(ApplicationDbContext db, ICurrentUserService currentUserService)
        {
            _db = db;
        }
        #region Vault Management
        public async Task<List<VaultDto>> GetAllVaultsAsync()
        {
            return await _db.Vaults
                .AsNoTracking()
                .Include(x => x.Branch)
                .Include(x => x.KeeperUser)
                .Include(x => x.Assets).ThenInclude(a => a.Currency)
                .Select(x => new VaultDto
                {
                    Id = x.Id,
                    CompanyId = x.CompanyId,
                    BranchId = x.BranchId,
                    KeeperUserId = x.KeeperUserId.Value,
                    VaultName = x.VaultName,
                    Remarks = x.Remarks,
                    IsActive = x.IsActive,
                    KeeperUserName = x.KeeperUser != null ? x.KeeperUser.UserName : null,
                    BranchName = x.Branch != null ? x.Branch.BranchName : null,
                    Assets = x.Assets.Select(a => new VaultAssetDto
                    {
                        Id = a.Id,
                        VaultId = a.VaultId,
                        CurrencyId = a.CurrencyId,
                        CurrencyCode = a.Currency.CurrencyCode,
                        Amount = a.Amount,
                        LastUpdated = a.LastUpdated
                    }).ToList()
                }).ToListAsync();
        }

        public async Task<VaultDto?> GetVaultByIdAsync(Guid vaultId)
        {
            var vaults = await GetAllVaultsAsync();
            return vaults.FirstOrDefault(x => x.Id == vaultId);
        }

        public async Task<Guid> UpsertVaultAsync(VaultUpsertDto dto, Guid currentUserId)
        {
            Vault? vault;
            if (dto.Id.HasValue && dto.Id != Guid.Empty)
            {
                vault = await _db.Vaults.FirstOrDefaultAsync(x => x.Id == dto.Id.Value);
                if (vault == null) throw new KeyNotFoundException("Vault not found.");
            }
            else
            {
                vault = new Vault { Id = Guid.CreateVersion7() };
                await _db.Vaults.AddAsync(vault);
            }

            vault.VaultName = dto.VaultName;
            vault.CompanyId = dto.CompanyId;
            vault.BranchId = dto.BranchId;
            vault.KeeperUserId = dto.KeeperUserId;
            vault.Remarks = dto.Remarks ?? string.Empty;
            vault.IsActive = dto.IsActive;

            await _db.SaveChangesAsync();
            return vault.Id;
        }

        public async Task<bool> ToggleVaultStatusAsync(Guid vaultId)
        {
            var vault = await _db.Vaults.FindAsync(vaultId);
            if (vault == null) return false;
            vault.IsActive = !vault.IsActive;
            await _db.SaveChangesAsync();
            return true;
        }
        #endregion

        #region Cockpit & Assets
        public async Task<VaultCockpitVm> GetCockpitDataAsync(Guid? vaultId)
        {
            var vaults = await GetAllVaultsAsync();
            var activeVaultId = vaultId ?? vaults.FirstOrDefault()?.Id ?? Guid.Empty;

            var movements = await GetMovementsPagedAsync(new VaultMovementFilterDto { VaultId = activeVaultId, PageSize = 10 });
            var pendingRequests = await GetPaymentRequestsPagedAsync(new CashPaymentRequestFilterDto { VaultId = activeVaultId, StatusId = (int)EnumCashPaymentRequestStatus.Pending, PageSize = 10 });
            var items = await GetVaultItemsPagedAsync(new VaultItemFilterDto { VaultId = activeVaultId, PageSize = 10 });

            return new VaultCockpitVm
            {
                Vaults = vaults,
                SelectedVaultId = activeVaultId,
                RecentMovements = movements,
                PendingPaymentRequests = pendingRequests,
                VaultItems = items
            };
        }

        public async Task<List<VaultAssetDto>> GetVaultAssetsAsync(Guid vaultId)
        {
            return await _db.VaultAssets
                .AsNoTracking()
                .Where(x => x.VaultId == vaultId)
                .Include(x => x.Currency)
                .Select(x => new VaultAssetDto
                {
                    Id = x.Id,
                    VaultId = x.VaultId,
                    CurrencyId = x.CurrencyId,
                    CurrencyCode = x.Currency.CurrencyCode,
                    Amount = x.Amount,
                    LastUpdated = x.LastUpdated
                }).ToListAsync();
        }
        #endregion

        #region Movements
        public async Task<PagedResultDto<VaultMovementDto>> GetMovementsPagedAsync(VaultMovementFilterDto filter)
        {
            var query = _db.VaultMovements
                .AsNoTracking()
                .Include(x => x.Vault)
                .Include(x => x.Currency)
                .Include(x => x.InOut)
                .Include(x => x.SourceType)
                .Include(x => x.ExecutedByUser)
                .AsQueryable();

            if (filter.VaultId.HasValue && filter.VaultId.Value != Guid.Empty)
                query = query.Where(x => x.VaultId == filter.VaultId.Value);

            if (filter.CurrencyId.HasValue)
                query = query.Where(x => x.CurrencyId == filter.CurrencyId.Value);

            if (filter.InOutId.HasValue)
                query = query.Where(x => x.InOutId == filter.InOutId.Value);

            if (filter.SourceTypeId.HasValue)
                query = query.Where(x => x.VaultMovementSourceTypeId == filter.SourceTypeId.Value);

            if (filter.FromDate.HasValue)
                query = query.Where(x => x.MovementDate >= filter.FromDate.Value);

            if (filter.ToDate.HasValue)
                query = query.Where(x => x.MovementDate <= filter.ToDate.Value);

            if (!string.IsNullOrWhiteSpace(filter.SearchTerm))
            {
                var term = filter.SearchTerm.Trim().ToLower();
                query = query.Where(x => (x.ReferenceNo != null && x.ReferenceNo.ToLower().Contains(term)) ||
                                         (x.ReferenceNumber != null && x.ReferenceNumber.ToLower().Contains(term)) ||
                                         (x.DepositorOrReceiver != null && x.DepositorOrReceiver.ToLower().Contains(term)) ||
                                         (x.Remarks != null && x.Remarks.ToLower().Contains(term)));
            }

            var totalCount = await query.CountAsync();
            var items = await query.OrderByDescending(x => x.MovementDate)
                .Skip((filter.PageNumber - 1) * filter.PageSize)
                .Take(filter.PageSize)
                .Select(x => new VaultMovementDto
                {
                    Id = x.Id,
                    VaultId = x.VaultId,
                    VaultName = x.Vault.VaultName,
                    CurrencyId = x.CurrencyId,
                    CurrencyCode = x.Currency.CurrencyCode,
                    InOutId = x.InOutId,
                    InOutTitle = x.InOut.InOutName,
                    Amount = x.Amount,
                    ReferenceNo = x.ReferenceNo,
                    ReferenceNumber = x.ReferenceNumber,
                    SourceTypeName = x.SourceType.VaultMovementSourceTypeName,
                    DepositorOrReceiver = x.DepositorOrReceiver,
                    Remarks = x.Remarks,
                    ExecutedByUserName = x.ExecutedByUser != null ? x.ExecutedByUser.UserName : null,
                    MovementDate = x.MovementDate
                }).ToListAsync();

            return new PagedResultDto<VaultMovementDto>
            {
                Items = items,
                TotalCount = totalCount,
                PageNumber = filter.PageNumber,
                PageSize = filter.PageSize
            };
        }

        public async Task<Guid> CreateMovementAsync(CreateVaultMovementDto dto, Guid currentUserId)
        {
            using var tx = await _db.Database.BeginTransactionAsync();
            try
            {
                var asset = await _db.VaultAssets
                    .FirstOrDefaultAsync(x => x.VaultId == dto.VaultId && x.CurrencyId == dto.CurrencyId);

                if (asset == null)
                {
                    asset = new VaultAsset
                    {
                        Id = Guid.CreateVersion7(),
                        VaultId = dto.VaultId,
                        CurrencyId = dto.CurrencyId,
                        Amount = 0,
                        LastUpdated = DateTime.Now
                    };
                    await _db.VaultAssets.AddAsync(asset);
                }

                // InOutId: 1 = In, 2 = Out
                if (dto.InOutId == 2 && asset.Amount < dto.Amount)
                {
                    throw new InvalidOperationException($"Insufficient vault liquidity. Current balance is {asset.Amount:N2}.");
                }

                if (dto.InOutId == 1)
                    asset.Amount += dto.Amount;
                else
                    asset.Amount -= dto.Amount;

                asset.LastUpdated = DateTime.Now;

                var movement = new VaultMovement
                {
                    Id = Guid.CreateVersion7(),
                    VaultId = dto.VaultId,
                    CurrencyId = dto.CurrencyId,
                    InOutId = dto.InOutId,
                    Amount = dto.Amount,
                    VaultMovementSourceTypeId = dto.VaultMovementSourceTypeId,
                    ReferenceNo = dto.ReferenceNo,
                    ReferenceNumber = dto.ReferenceNumber,
                    DepositorOrReceiver = dto.DepositorOrReceiver,
                    Remarks = dto.Remarks,
                    ExecutedByUserId = currentUserId,
                    MovementDate = DateTime.Now,
                    SourceEntityId = dto.PaymentDocumentId,
                    CreateUserId = currentUserId,
                    CreatedAt = DateTime.Now,
                };

                await _db.VaultMovements.AddAsync(movement);
                await _db.SaveChangesAsync();
                await tx.CommitAsync();

                return movement.Id;
            }
            catch
            {
                await tx.RollbackAsync();
                throw;
            }
        }
        #endregion

        #region Cash Payment Requests
        public async Task<PagedResultDto<CashPaymentRequestDto>> GetPaymentRequestsPagedAsync(CashPaymentRequestFilterDto filter)
        {
            var query = _db.CashPaymentRequests
                .AsNoTracking()
                .Include(x => x.Vault)
                .Include(x => x.Currency)
                .Include(x => x.CashPaymentRequestStatus)
                .Include(x => x.CreateUser)
                .Include(x => x.InitialConfirmUser)
                .Include(x => x.FinalConfirmUser)
                .Include(x => x.PaidByUser)
                .AsQueryable();

            if (filter.VaultId.HasValue && filter.VaultId.Value != Guid.Empty)
                query = query.Where(x => x.VaultId == filter.VaultId.Value);

            if (filter.StatusId.HasValue)
                query = query.Where(x => x.StatusId == filter.StatusId.Value);

            if (!string.IsNullOrWhiteSpace(filter.SearchTerm))
            {
                var term = filter.SearchTerm.Trim().ToLower();
                query = query.Where(x => x.RequestNo.ToLower().Contains(term) ||
                                         (x.PayeeName != null && x.PayeeName.ToLower().Contains(term)) ||
                                         x.Reason.ToLower().Contains(term));
            }

            var totalCount = await query.CountAsync();
            var items = await query.OrderByDescending(x => x.Id)
                .Skip((filter.PageNumber - 1) * filter.PageSize)
                .Take(filter.PageSize)
                .Select(x => new CashPaymentRequestDto
                {
                    Id = x.Id,
                    RequestNo = x.RequestNo,
                    VaultId = x.VaultId,
                    VaultName = x.Vault.VaultName,
                    PayeeName = x.PayeeName,
                    Amount = x.Amount,
                    CurrencyCode = x.Currency.CurrencyCode,
                    Reason = x.Reason,
                    StatusId = x.StatusId,
                    StatusName = x.CashPaymentRequestStatus.CashPaymentRequestStatusName,
                    CreatedByName = x.CreateUser != null ? x.CreateUser.UserName : null,
                    InitialConfirmedByName = x.InitialConfirmUser != null ? x.InitialConfirmUser.UserName : null,
                    FinalConfirmedByName = x.FinalConfirmUser != null ? x.FinalConfirmUser.UserName : null,
                    PaidByName = x.PaidByUser != null ? x.PaidByUser.UserName : null,
                    PaidDate = x.PaidDate,
                    RejectReason = x.RejectReason
                }).ToListAsync();

            return new PagedResultDto<CashPaymentRequestDto>
            {
                Items = items,
                TotalCount = totalCount,
                PageNumber = filter.PageNumber,
                PageSize = filter.PageSize
            };
        }

        public async Task<CashPaymentRequestDto?> GetPaymentRequestByIdAsync(Guid id)
        {
            var req = await _db.CashPaymentRequests
                .AsNoTracking()
                .Include(x => x.Vault)
                .Include(x => x.Currency)
                .Include(x => x.CashPaymentRequestStatus)
                .FirstOrDefaultAsync(x => x.Id == id);

            if (req == null) return null;

            return new CashPaymentRequestDto
            {
                Id = req.Id,
                RequestNo = req.RequestNo,
                VaultId = req.VaultId,
                VaultName = req.Vault.VaultName,
                PayeeName = req.PayeeName,
                Amount = req.Amount,
                CurrencyCode = req.Currency.CurrencyCode,
                Reason = req.Reason,
                StatusId = req.StatusId,
                StatusName = req.CashPaymentRequestStatus.CashPaymentRequestStatusName,
                PaidDate = req.PaidDate,
                RejectReason = req.RejectReason
            };
        }

        public async Task<Guid> CreatePaymentRequestAsync(CreateCashPaymentRequestDto dto, Guid currentUserId)
        {
            var seq = await _db.CashPaymentRequests.CountAsync() + 1;
            var request = new CashPaymentRequest
            {
                Id = Guid.CreateVersion7(),
                RequestNo = $"CPR-{DateTime.Now:yyyyMM}-{seq:D4}",
                VaultId = dto.VaultId,
                PartyId = dto.PartyId,
                PayeeName = dto.PayeeName,
                Amount = dto.Amount,
                CurrencyId = dto.CurrencyId,
                Reason = dto.Reason,
                StatusId = (int)EnumCashPaymentRequestStatus.Pending,
                CreateUserId = currentUserId
            };

            await _db.CashPaymentRequests.AddAsync(request);
            await _db.SaveChangesAsync();
            return request.Id;
        }

        public async Task<bool> ApproveInitialPaymentRequestAsync(Guid requestId, Guid currentUserId)
        {
            var req = await _db.CashPaymentRequests.FindAsync(requestId);
            if (req == null || req.StatusId != (int)EnumCashPaymentRequestStatus.Pending) return false;
            req.InitialConfirmedByUserId = currentUserId;
            req.InitialConfirmDate = DateTime.Now;
            await _db.SaveChangesAsync();
            return true;
        }

        public async Task<bool> ApproveFinalPaymentRequestAsync(Guid requestId, Guid currentUserId)
        {
            var req = await _db.CashPaymentRequests.FindAsync(requestId);
            if (req == null) return false;
            req.FinalConfirmedByUserId = currentUserId;
            req.FinalConfirmDate = DateTime.Now;
            await _db.SaveChangesAsync();
            return true;
        }

        public async Task<bool> RejectPaymentRequestAsync(Guid requestId, string reason, Guid currentUserId)
        {
            var req = await _db.CashPaymentRequests.FindAsync(requestId);
            if (req == null || req.StatusId == (int)EnumCashPaymentRequestStatus.Paid) return false;

            req.StatusId = (int)EnumCashPaymentRequestStatus.Rejected;
            req.RejectReason = reason;
            await _db.SaveChangesAsync();
            return true;
        }

        public async Task<bool> ExecutePaymentRequestAsync(Guid requestId, Guid currentUserId)
        {
            using var tx = await _db.Database.BeginTransactionAsync();
            try
            {
                var req = await _db.CashPaymentRequests.FindAsync(requestId);
                if (req == null || req.StatusId == (int)EnumCashPaymentRequestStatus.Paid) return false;

                var asset = await _db.VaultAssets
                    .FirstOrDefaultAsync(x => x.VaultId == req.VaultId && x.CurrencyId == req.CurrencyId);

                if (asset == null || asset.Amount < req.Amount)
                {
                    throw new InvalidOperationException("Insufficient vault liquidity to complete this payment.");
                }

                // 1. Deduct Liquidity
                asset.Amount -= req.Amount;
                asset.LastUpdated = DateTime.Now;

                // 2. Generate Cardex VaultMovement (InOut: 2 = Out)
                var movement = new VaultMovement
                {
                    Id = Guid.CreateVersion7(),
                    VaultId = req.VaultId,
                    CurrencyId = req.CurrencyId,
                    InOutId = 2,
                    Amount = req.Amount,
                    VaultMovementSourceTypeId = (int)EnumVaultMovementSourceType.CashPaymentRequest,
                    SourceEntityId = req.Id,
                    ReferenceNo = req.RequestNo,
                    DepositorOrReceiver = req.PayeeName,
                    Remarks = $"Payment for CPR: {req.RequestNo} - {req.Reason}",
                    ExecutedByUserId = currentUserId,
                    MovementDate = DateTime.Now
                };
                await _db.VaultMovements.AddAsync(movement);

                // 3. Mark request as Paid
                req.StatusId = (int)EnumCashPaymentRequestStatus.Paid;
                req.PaidByUserId = currentUserId;
                req.PaidDate = DateTime.Now;
                req.VaultMovementId = movement.Id;

                await _db.SaveChangesAsync();
                await tx.CommitAsync();
                return true;
            }
            catch
            {
                await tx.RollbackAsync();
                throw;
            }
        }
        #endregion

        #region Vault Items (Non-Fungible Custody)
        public async Task<PagedResultDto<VaultItemDto>> GetVaultItemsPagedAsync(VaultItemFilterDto filter)
        {
            var query = _db.VaultItems
                .AsNoTracking()
                .Include(x => x.Vault)
                .Include(x => x.VaultItemCategory)
                .Include(x => x.VaultItemStatus)
                .Include(x => x.Currency)
                .Include(x => x.CurrentHolderUser)
                .AsQueryable();

            if (filter.VaultId.HasValue && filter.VaultId.Value != Guid.Empty)
                query = query.Where(x => x.VaultId == filter.VaultId.Value);

            if (filter.CategoryId.HasValue)
                query = query.Where(x => x.VaultItemCategoryId == filter.CategoryId.Value);

            if (filter.StatusId.HasValue)
                query = query.Where(x => x.VaultItemStatusId == filter.StatusId.Value);

            if (filter.OnlyExpiringSoon)
            {
                var next30Days = DateTime.Now.AddDays(30);
                query = query.Where(x => x.ExpiryDate.HasValue && x.ExpiryDate <= next30Days);
            }

            if (!string.IsNullOrWhiteSpace(filter.SearchTerm))
            {
                var term = filter.SearchTerm.Trim().ToLower();
                query = query.Where(x => x.Title.ToLower().Contains(term) ||
                                         (x.IdentifierNo != null && x.IdentifierNo.ToLower().Contains(term)) ||
                                         (x.OwnerOrBeneficiary != null && x.OwnerOrBeneficiary.ToLower().Contains(term)));
            }

            var totalCount = await query.CountAsync();
            var items = await query.OrderByDescending(x => x.Id)
                .Skip((filter.PageNumber - 1) * filter.PageSize)
                .Take(filter.PageSize)
                .Select(x => new VaultItemDto
                {
                    Id = x.Id,
                    VaultId = x.VaultId,
                    VaultName = x.Vault.VaultName,
                    VaultItemCategoryId = x.VaultItemCategoryId,
                    CategoryName = x.VaultItemCategory.VaultItemCategoryName,
                    Title = x.Title,
                    IdentifierNo = x.IdentifierNo,
                    OwnerOrBeneficiary = x.OwnerOrBeneficiary,
                    NominalValue = x.NominalValue,
                    CurrencyCode = x.Currency != null ? x.Currency.CurrencyCode : null,
                    IssueDate = x.IssueDate,
                    ExpiryDate = x.ExpiryDate,
                    ShelfLocation = x.ShelfLocation,
                    VaultItemStatusId = x.VaultItemStatusId,
                    StatusName = x.VaultItemStatus.VaultItemStatusName,
                    CurrentHolderName = x.CurrentHolderUser != null ? x.CurrentHolderUser.UserName : x.CurrentHolderExternalName,
                    CheckedOutDate = x.CheckedOutDate,
                    ExpectedReturnDate = x.ExpectedReturnDate,
                    AttachmentPath = x.AttachmentPath
                }).ToListAsync();

            return new PagedResultDto<VaultItemDto>
            {
                Items = items,
                TotalCount = totalCount,
                PageNumber = filter.PageNumber,
                PageSize = filter.PageSize
            };
        }

        public async Task<VaultItemDto?> GetVaultItemByIdAsync(Guid id)
        {
            var item = await _db.VaultItems
                .AsNoTracking()
                .Include(x => x.Vault)
                .Include(x => x.VaultItemCategory)
                .Include(x => x.VaultItemStatus)
                .Include(x => x.Currency)
                .Include(x => x.CurrentHolderUser)
                .FirstOrDefaultAsync(x => x.Id == id);

            if (item == null) return null;

            return new VaultItemDto
            {
                Id = item.Id,
                VaultId = item.VaultId,
                VaultName = item.Vault.VaultName,
                VaultItemCategoryId = item.VaultItemCategoryId,
                CategoryName = item.VaultItemCategory.VaultItemCategoryName,
                Title = item.Title,
                IdentifierNo = item.IdentifierNo,
                OwnerOrBeneficiary = item.OwnerOrBeneficiary,
                NominalValue = item.NominalValue,
                CurrencyCode = item.Currency != null ? item.Currency.CurrencyCode : null,
                IssueDate = item.IssueDate,
                ExpiryDate = item.ExpiryDate,
                ShelfLocation = item.ShelfLocation,
                VaultItemStatusId = item.VaultItemStatusId,
                StatusName = item.VaultItemStatus.VaultItemStatusName,
                CurrentHolderName = item.CurrentHolderUser != null ? item.CurrentHolderUser.UserName : item.CurrentHolderExternalName,
                CheckedOutDate = item.CheckedOutDate,
                ExpectedReturnDate = item.ExpectedReturnDate,
                AttachmentPath = item.AttachmentPath
            };
        }

        public async Task<Guid> UpsertVaultItemAsync(VaultItemUpsertDto dto, Guid currentUserId)
        {
            VaultItem? item;
            bool isNew = false;
            if (dto.Id.HasValue && dto.Id != Guid.Empty)
            {
                item = await _db.VaultItems.FindAsync(dto.Id.Value);
                if (item == null) throw new KeyNotFoundException("Vault item not found.");
            }
            else
            {
                item = new VaultItem
                {
                    Id = Guid.CreateVersion7(),
                    VaultItemStatusId = (int)EnumVaultItemStatus.InVault,
                    CreatedAt = DateTime.Now,
                    CreateUserId = currentUserId,
                };
                await _db.VaultItems.AddAsync(item);
                isNew = true;
            }

            item.VaultId = dto.VaultId;
            item.VaultItemCategoryId = dto.VaultItemCategoryId;
            item.Title = dto.Title;
            item.IdentifierNo = dto.IdentifierNo;
            item.OwnerOrBeneficiary = dto.OwnerOrBeneficiary;
            item.NominalValue = dto.NominalValue;
            item.CurrencyId = dto.CurrencyId;
            item.IssueDate = dto.IssueDate;
            item.ExpiryDate = dto.ExpiryDate;
            item.ShelfLocation = dto.ShelfLocation;
            item.Description = dto.Description;

            if (isNew)
            {
                var custodyLog = new VaultItemCustodyLog
                {
                    Id = Guid.CreateVersion7(),
                    VaultItemId = item.Id,
                    VaultCustodyActionTypeId = (int)EnumVaultCustodyActionType.InitialDeposit,
                    ActionDate = DateTime.Now,
                    ExecutedByUserId = currentUserId,
                    Reason = "Initial intake into vault",
                    CreateUserId = currentUserId,
                    CreatedAt = DateTime.Now
                };
                await _db.VaultItemCustodyLogs.AddAsync(custodyLog);
            }
            else
            {
                item.UpdatedAt = DateTime.Now;
                item.EditUserId = currentUserId;
            }
            await _db.SaveChangesAsync();
            return item.Id;
        }
        public async Task<bool> ExecuteCustodyActionAsync(VaultCustodyActionDto dto, Guid currentUserId)
        {
            var item = await _db.VaultItems.FindAsync(dto.VaultItemId);
            if (item == null) return false;
            var actionType = (EnumVaultCustodyActionType)dto.ActionTypeId;
            switch (actionType)
            {
                case EnumVaultCustodyActionType.BorrowCheckOut:
                    item.VaultItemStatusId = (int)EnumVaultItemStatus.CheckedOut;
                    item.CurrentHolderUserId = dto.HandedToUserId;
                    item.CurrentHolderExternalName = dto.HandedToExternalName;
                    item.CheckedOutDate = DateTime.Now;
                    item.ExpectedReturnDate = dto.ExpectedReturnDate;
                    break;

                case EnumVaultCustodyActionType.ReturnCheckIn:
                    item.VaultItemStatusId = (int)EnumVaultItemStatus.InVault;
                    item.CurrentHolderUserId = null;
                    item.CurrentHolderExternalName = null;
                    item.CheckedOutDate = null;
                    item.ExpectedReturnDate = null;
                    break;

                case EnumVaultCustodyActionType.PermanentOut:
                    item.VaultItemStatusId = (int)EnumVaultItemStatus.PermanentOut;
                    item.CurrentHolderUserId = dto.HandedToUserId;
                    item.CurrentHolderExternalName = dto.HandedToExternalName;
                    break;
            }

            var log = new VaultItemCustodyLog
            {
                Id = Guid.CreateVersion7(),
                VaultItemId = item.Id,
                VaultCustodyActionTypeId = dto.ActionTypeId,
                ActionDate = DateTime.Now,
                ExecutedByUserId = currentUserId,
                HandedToUserId = dto.HandedToUserId,
                HandedToExternalName = dto.HandedToExternalName,
                Reason = dto.Reason,
                ExpectedReturnDate = dto.ExpectedReturnDate
            };

            await _db.VaultItemCustodyLogs.AddAsync(log);
            await _db.SaveChangesAsync();
            return true;
        }
        #endregion

        #region Lookups & Base Tables
        public async Task<List<LookupItemDto>> GetBaseTableDataAsync(string entityType)
        {
            return entityType.ToLower() switch
            {
                "category" => await _db.VaultItemCategories.Select(x => new LookupItemDto { Id = x.Id, Name = x.VaultItemCategoryName, LocalName = x.VaultItemCategoryLocalName, EntityType = "category" }).ToListAsync(),
                "status" => await _db.VaultItemStatus.Select(x => new LookupItemDto { Id = x.Id, Name = x.VaultItemStatusName, LocalName = x.VaultItemStatusLocalName, EntityType = "status" }).ToListAsync(),
                "sourcetype" => await _db.VaultMovementSourceTypes.Select(x => new LookupItemDto { Id = x.Id, Name = x.VaultMovementSourceTypeName, LocalName = x.VaultMovementSourceTypeLocalName, EntityType = "sourcetype" }).ToListAsync(),
                "custodyaction" => await _db.VaultCustodyActionTypes.Select(x => new LookupItemDto { Id = x.Id, Name = x.VaultCustodyActionTypeName, LocalName = x.VaultCustodyActionTypeLocalName, EntityType = "custodyaction" }).ToListAsync(),
                _ => new List<LookupItemDto>()
            };
        }

        public async Task<bool> UpsertBaseTableItemAsync(LookupUpsertDto dto)
        {
            switch (dto.EntityType.ToLower())
            {
                case "category":
                    var cat = await _db.VaultItemCategories.FindAsync(dto.Id);
                    if (cat == null)
                    {
                        cat = new VaultItemCategory { Id = dto.Id, VaultItemCategoryName = dto.Name, VaultItemCategoryLocalName = dto.LocalName };
                        await _db.VaultItemCategories.AddAsync(cat);
                    }
                    else
                    {
                        cat.VaultItemCategoryName = dto.Name;
                        cat.VaultItemCategoryLocalName = dto.LocalName;
                    }
                    break;
                case "sourcetype":
                    var st = await _db.VaultMovementSourceTypes.FindAsync(dto.Id);
                    if (st == null)
                    {
                        st = new VaultMovementSourceType { Id = dto.Id, VaultMovementSourceTypeName = dto.Name, VaultMovementSourceTypeLocalName = dto.LocalName };
                        await _db.VaultMovementSourceTypes.AddAsync(st);
                    }
                    else
                    {
                        st.VaultMovementSourceTypeName = dto.Name;
                        st.VaultMovementSourceTypeLocalName = dto.LocalName;
                    }
                    break;
                default:
                    return false;
            }
            await _db.SaveChangesAsync();
            return true;
        }

        public async Task<bool> DeleteBaseTableItemAsync(string entityType, int id)
        {
            switch (entityType.ToLower())
            {
                case "category":
                    var cat = await _db.VaultItemCategories.FindAsync(id);
                    if (cat != null) _db.VaultItemCategories.Remove(cat);
                    break;
                case "sourcetype":
                    var st = await _db.VaultMovementSourceTypes.FindAsync(id);
                    if (st != null) _db.VaultMovementSourceTypes.Remove(st);
                    break;
                default:
                    return false;
            }
            await _db.SaveChangesAsync();
            return true;
        }
        #endregion


        public async Task<PagedResultDto<VaultItemRowDto>> GetVaultsPagedAsync(VaultFilterDto filter)
        {
            var query = _db.Vaults
                .AsNoTracking()
                .Include(x => x.Company)
                .Include(x => x.Branch)
                .Include(x => x.KeeperUser)
                .Include(x => x.Assets)
                .Include(x => x.Items)
                .AsQueryable();

            if (filter.CompanyId.HasValue)
                query = query.Where(x => x.CompanyId == filter.CompanyId.Value);

            if (filter.BranchId.HasValue)
                query = query.Where(x => x.BranchId == filter.BranchId.Value);

            if (filter.IsActive.HasValue)
                query = query.Where(x => x.IsActive == filter.IsActive.Value);

            if (!string.IsNullOrWhiteSpace(filter.SearchTerm))
            {
                var term = filter.SearchTerm.Trim().ToLower();
                query = query.Where(x => x.VaultName.ToLower().Contains(term) ||
                                         (x.Remarks != null && x.Remarks.ToLower().Contains(term)) ||
                                         (x.KeeperUser != null && x.KeeperUser.UserName.ToLower().Contains(term)));
            }

            var total = await query.CountAsync();
            var items = await query.OrderByDescending(x => x.IsActive).ThenBy(x => x.VaultName)
                .Skip((filter.PageNumber - 1) * filter.PageSize)
                .Take(filter.PageSize)
                .Select(x => new VaultItemRowDto
                {
                    Id = x.Id,
                    VaultName = x.VaultName,
                    CompanyId = x.CompanyId,
                    CompanyName = x.Company != null ? x.Company.CompanyAbb : null,
                    BranchId = x.BranchId,
                    BranchName = x.Branch != null ? x.Branch.BranchName : null,
                    KeeperUserId = x.KeeperUserId,
                    KeeperUserName = x.KeeperUser != null ? x.KeeperUser.UserName : "-",
                    Remarks = x.Remarks,
                    IsActive = x.IsActive,
                    TotalCurrenciesCount = x.Assets.Count(a => a.Amount > 0),
                    ActiveCustodyItemsCount = x.Items.Count(i => i.VaultItemStatusId == (int)EnumVaultItemStatus.InVault)
                }).ToListAsync();

            return new PagedResultDto<VaultItemRowDto>
            {
                Items = items,
                TotalCount = total,
                PageNumber = filter.PageNumber,
                PageSize = filter.PageSize
            };
        }

        public async Task<VaultFormVm> GetVaultFormVmAsync(Guid? id)
        {
            var vm = new VaultFormVm();

            if (id.HasValue && id.Value != Guid.Empty)
            {
                var vault = await _db.Vaults.AsNoTracking().FirstOrDefaultAsync(x => x.Id == id.Value);
                if (vault != null)
                {
                    vm.Id = vault.Id;
                    vm.VaultName = vault.VaultName;
                    vm.CompanyId = vault.CompanyId;
                    vm.BranchId = vault.BranchId;
                    vm.KeeperUserId = vault.KeeperUserId;
                    vm.Remarks = vault.Remarks;
                    vm.IsActive = vault.IsActive;
                }
            }

            // Populate Lookups cleanly via EF Core
            vm.Companies = await _db.Companies
                .AsNoTracking()
                .Select(c => new SelectListItem { Value = c.Id.ToString(), Text = c.CompanyAbb, Selected = (vm.CompanyId == c.Id) })
                .ToListAsync();

            vm.Branches = await _db.Branches
                .AsNoTracking()
                .Select(b => new SelectListItem { Value = b.Id.ToString(), Text = b.BranchName, Selected = (vm.BranchId == b.Id) })
                .ToListAsync();

            vm.Keepers = await _db.Users
                .AsNoTracking()
                .Select(u => new SelectListItem { Value = u.Id.ToString(), Text = u.UserName, Selected = (vm.KeeperUserId == u.Id) })
                .ToListAsync();

            return vm;
        }

        public async Task<Guid> SaveVaultAsync(VaultFormVm vm, Guid currentUserId)
        {
            Vault? vault;
            if (vm.Id.HasValue && vm.Id.Value != Guid.Empty)
            {
                vault = await _db.Vaults.FirstOrDefaultAsync(x => x.Id == vm.Id.Value);
                if (vault == null) throw new KeyNotFoundException("Vault not found.");
            }
            else
            {
                vault = new Vault { Id = Guid.CreateVersion7() };
                await _db.Vaults.AddAsync(vault);
            }
            vault.VaultName = vm.VaultName.Trim();
            vault.CompanyId = vm.CompanyId;
            vault.BranchId = vm.BranchId;
            vault.KeeperUserId = vm.KeeperUserId;
            vault.Remarks = vm.Remarks?.Trim() ?? string.Empty;
            vault.IsActive = vm.IsActive;

            await _db.SaveChangesAsync();
            return vault.Id;
        }
        public async Task<List<SPersonLookupDto>> GetActiveVaultLookupAsync(CancellationToken cancellationToken = default)
        {
            return await _db.Vaults
                .AsNoTracking()
                .Where(v => v.IsActive)
                .OrderBy(v => v.VaultName)
                .Select(v => new SPersonLookupDto
                {
                    Id = v.Id,
                    Name = v.VaultName
                })
                .ToListAsync(cancellationToken);
        }
        public async Task<List<SLookupDto>> GetCurrencyLookupAsync(CancellationToken cancellationToken = default)
        {
            return await _db.Currencies
                .AsNoTracking()
                .OrderBy(c => c.Id)
                .Select(c => new SLookupDto
                {
                    Id = c.Id,
                    Name = c.CurrencyCode,
                })
                .ToListAsync(cancellationToken);
        }
        public async Task<VaultItemCustodyHistoryViewModel?> GetItemCustodyHistoryAsync(Guid vaultItemId)
        {
            var item = await _db.VaultItems
                .AsNoTracking()
                .FirstOrDefaultAsync(x => x.Id == vaultItemId);

            if (item == null) return null;

            var logs = await _db.VaultItemCustodyLogs
                .AsNoTracking()
                .Where(x => x.VaultItemId == vaultItemId)
                .OrderByDescending(x => x.ActionDate)
                .Select(x => new VaultItemCustodyLogDto
                {
                    Id = x.Id,
                    VaultCustodyActionTypeId = x.VaultCustodyActionTypeId,
                    ActionTypeName = x.VaultCustodyActionType != null ? x.VaultCustodyActionType.VaultCustodyActionTypeName : "Action",
                    ActionDate = x.ActionDate,
                    ExecutedByName = x.ExecutedByUser != null ? (x.ExecutedByUser.FullName ?? x.ExecutedByUser.UserName) : "System",
                    ReceiverName = x.HandedToUser != null
                        ? (x.HandedToUser.FullName ?? x.HandedToUser.UserName)
                        : (!string.IsNullOrEmpty(x.HandedToExternalName) ? x.HandedToExternalName : "-"),
                    Reason = x.Reason,
                    ExpectedReturnDate = x.ExpectedReturnDate,
                    HandoverReceiptScanPath = x.HandoverReceiptScanPath
                })
                .ToListAsync();
            return new VaultItemCustodyHistoryViewModel
            {
                VaultItemId = item.Id,
                ItemTitle = item.Title,
                TrackingNo = "",
                CurrentCustodianName = "-",
                Logs = logs
            };
        }
    }
}