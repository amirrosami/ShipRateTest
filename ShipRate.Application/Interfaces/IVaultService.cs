using ShipRate.Application.DTOs;

namespace ShipRate.Application.Interfaces
{
    public interface IVaultService
    {
        // Vault Management
        Task<List<VaultDto>> GetAllVaultsAsync();
        Task<VaultDto?> GetVaultByIdAsync(Guid vaultId);
        Task<Guid> UpsertVaultAsync(VaultUpsertDto dto, Guid currentUserId);
        Task<bool> ToggleVaultStatusAsync(Guid vaultId);
        Task<PagedResultDto<VaultItemRowDto>> GetVaultsPagedAsync(VaultFilterDto filter);
        Task<VaultFormVm> GetVaultFormVmAsync(Guid? id);
        Task<Guid> SaveVaultAsync(VaultFormVm vm, Guid currentUserId);

        Task<List<SPersonLookupDto>> GetActiveVaultLookupAsync(CancellationToken cancellationToken = default);
        Task<List<SLookupDto>> GetCurrencyLookupAsync(CancellationToken cancellationToken = default);




        // Vault Assets & Cockpit
        Task<VaultCockpitVm> GetCockpitDataAsync(Guid? vaultId);
        Task<List<VaultAssetDto>> GetVaultAssetsAsync(Guid vaultId);

        // Movements (Cardex)
        Task<PagedResultDto<VaultMovementDto>> GetMovementsPagedAsync(VaultMovementFilterDto filter);
        Task<Guid> CreateMovementAsync(CreateVaultMovementDto dto, Guid currentUserId);

        // Cash Payment Requests
        Task<PagedResultDto<CashPaymentRequestDto>> GetPaymentRequestsPagedAsync(CashPaymentRequestFilterDto filter);
        Task<CashPaymentRequestDto?> GetPaymentRequestByIdAsync(Guid id);
        Task<Guid> CreatePaymentRequestAsync(CreateCashPaymentRequestDto dto, Guid currentUserId);
        Task<bool> ApproveInitialPaymentRequestAsync(Guid requestId, Guid currentUserId);
        Task<bool> ApproveFinalPaymentRequestAsync(Guid requestId, Guid currentUserId);
        Task<bool> RejectPaymentRequestAsync(Guid requestId, string reason, Guid currentUserId);
        Task<bool> ExecutePaymentRequestAsync(Guid requestId, Guid currentUserId);

        // Vault Items (Non-Fungible Custody)
        Task<PagedResultDto<VaultItemDto>> GetVaultItemsPagedAsync(VaultItemFilterDto filter);
        Task<VaultItemDto?> GetVaultItemByIdAsync(Guid id);
        Task<Guid> UpsertVaultItemAsync(VaultItemUpsertDto dto, Guid currentUserId);
        Task<bool> ExecuteCustodyActionAsync(VaultCustodyActionDto dto, Guid currentUserId);

        // System Base Tables / Lookups Management
        Task<List<LookupItemDto>> GetBaseTableDataAsync(string entityType);
        Task<bool> UpsertBaseTableItemAsync(LookupUpsertDto dto);
        Task<bool> DeleteBaseTableItemAsync(string entityType, int id);


        Task<VaultItemCustodyHistoryViewModel?> GetItemCustodyHistoryAsync(Guid vaultItemId);



    }
}
