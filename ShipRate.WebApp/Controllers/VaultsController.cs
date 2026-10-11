using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ShipRate.Application.DTOs;
using ShipRate.Application.Interfaces;
using ShipRate.Application.Interfaces.Finance;
using System.Security.Claims;
namespace ShipRate.WebApp.Controllers
{
    [Authorize]
    public class VaultsController : Controller
    {
        private readonly IVaultService _vaultService;
        private readonly IPaymentDocumentService _paymentDocumentService;

        public VaultsController(IVaultService vaultService, IPaymentDocumentService paymentDocumentService)
        {
            _vaultService = vaultService;
            _paymentDocumentService = paymentDocumentService;
        }
        private Guid CurrentUserId
        {
            get
            {
                var id = User.FindFirstValue(ClaimTypes.NameIdentifier);
                return Guid.TryParse(id, out var guid) ? guid : Guid.Empty;
            }
        }

        [HttpGet]
        public async Task<IActionResult> Index(Guid? vaultId)
        {
            var cockpit = await _vaultService.GetCockpitDataAsync(vaultId);
            return View(cockpit);
        }

        #region Assets Tab
        [HttpGet]
        public async Task<IActionResult> GetAssetsTab(Guid vaultId)
        {
            var assets = await _vaultService.GetVaultAssetsAsync(vaultId);
            ViewBag.VaultId = vaultId;
            return PartialView("_AssetsTab", assets);
        }
        #endregion

        #region Movements Tab & Actions
        [HttpGet]
        public async Task<IActionResult> GetMovementsTab(VaultMovementFilterDto filter)
        {
            var movements = await _vaultService.GetMovementsPagedAsync(filter);
            ViewBag.Filter = filter;
            return PartialView("_MovementsTab", movements);
        }

        [HttpGet]
        public async Task<IActionResult> CreateMovementModal(Guid vaultId)
        {
            ViewBag.Currencies = await _vaultService.GetCurrencyLookupAsync();

            var model = new CreateVaultMovementDto { VaultId = vaultId, InOutId = 1 };
            return PartialView("_CreateMovementModal", model);
        }

        // 2. اکشن باز کردن مودال انتخاب اسناد پرداخت فاقد Movement
        [HttpGet]
        public async Task<IActionResult> SelectPaymentDocumentModal()
        {
            var documents = await _paymentDocumentService.GetAvailablePaymentDocumentsForVaultAsync();
            return PartialView("_SelectPaymentDocumentModal", documents);
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateMovement(CreateVaultMovementDto dto)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            try
            {
                await _vaultService.CreateMovementAsync(dto, CurrentUserId);
                Response.Headers.Append("HX-Trigger", "reloadVaultCockpit");
                return Ok();
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }
        #endregion

        #region Payment Requests Tab & Actions
        [HttpGet]
        public async Task<IActionResult> GetPaymentRequestsTab(CashPaymentRequestFilterDto filter)
        {
            var requests = await _vaultService.GetPaymentRequestsPagedAsync(filter);
            ViewBag.Filter = filter;
            return PartialView("_PaymentRequestsTab", requests);
        }

        [HttpGet]
        public async Task<IActionResult> CreatePaymentRequestModal(Guid? vaultId)
        {
            // دریافت لیست صندوق‌های فعال و ارزها از طریق سرویس
            ViewBag.Vaults = await _vaultService.GetActiveVaultLookupAsync();
            ViewBag.Currencies = await _vaultService.GetCurrencyLookupAsync();

            var model = new CreateCashPaymentRequestDto();
            if (vaultId.HasValue && vaultId.Value != Guid.Empty)
            {
                model.VaultId = vaultId.Value;
            }

            return PartialView("_CreatePaymentRequestModal", model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreatePaymentRequest(CreateCashPaymentRequestDto dto)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            await _vaultService.CreatePaymentRequestAsync(dto, CurrentUserId);
            Response.Headers.Append("HX-Trigger", "reloadPaymentRequests");
            return Ok();
        }

        [HttpPost]
        public async Task<IActionResult> ApproveInitialRequest(Guid id)
        {
            var res = await _vaultService.ApproveInitialPaymentRequestAsync(id, CurrentUserId);
            if (!res) return BadRequest("Could not initial-approve this request.");
            Response.Headers.Append("HX-Trigger", "reloadPaymentRequests");
            return Ok();
        }

        [HttpPost]
        public async Task<IActionResult> ApproveFinalRequest(Guid id)
        {
            var res = await _vaultService.ApproveFinalPaymentRequestAsync(id, CurrentUserId);
            if (!res) return BadRequest("Could not final-approve this request.");
            Response.Headers.Append("HX-Trigger", "reloadPaymentRequests");
            return Ok();
        }

        [HttpPost]
        public async Task<IActionResult> RejectRequest(Guid id, string reason)
        {
            var res = await _vaultService.RejectPaymentRequestAsync(id, reason, CurrentUserId);
            if (!res) return BadRequest("Could not reject this request.");
            Response.Headers.Append("HX-Trigger", "reloadPaymentRequests");
            return Ok();
        }

        [HttpPost]
        public async Task<IActionResult> ExecutePayment(Guid id)
        {
            try
            {
                var res = await _vaultService.ExecutePaymentRequestAsync(id, CurrentUserId);
                if (!res) return BadRequest("Execution failed.");
                Response.Headers.Append("HX-Trigger", "reloadVaultCockpit");
                return Ok();
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
        #endregion

        #region Vault Items Tab & Custody
        [HttpGet]
        public async Task<IActionResult> GetItemsTab(VaultItemFilterDto filter)
        {
            var items = await _vaultService.GetVaultItemsPagedAsync(filter);
            ViewBag.Filter = filter;
            return PartialView("_ItemsTab", items);
        }

        [HttpGet]
        public async Task<IActionResult> UpsertItemModal(Guid vaultId, Guid? id)
        {
            var model = new VaultItemUpsertDto { VaultId = vaultId };
            if (id.HasValue && id.Value != Guid.Empty)
            {
                var existing = await _vaultService.GetVaultItemByIdAsync(id.Value);
                if (existing != null)
                {
                    model.Id = existing.Id;
                    model.Title = existing.Title;
                    model.VaultItemCategoryId = existing.VaultItemCategoryId;
                    model.IdentifierNo = existing.IdentifierNo;
                    model.OwnerOrBeneficiary = existing.OwnerOrBeneficiary;
                    model.NominalValue = existing.NominalValue;
                    model.IssueDate = existing.IssueDate;
                    model.ExpiryDate = existing.ExpiryDate;
                    model.ShelfLocation = existing.ShelfLocation;
                }
            }
            return PartialView("_UpsertItemModal", model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> UpsertItem(VaultItemUpsertDto dto)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);
            await _vaultService.UpsertVaultItemAsync(dto, CurrentUserId);
            Response.Headers.Append("HX-Trigger", "reloadVaultItems");
            return Ok();
        }

        [HttpGet]
        public IActionResult CustodyActionModal(Guid itemId, int actionTypeId)
        {
            var model = new VaultCustodyActionDto
            {
                VaultItemId = itemId,
                ActionTypeId = actionTypeId
            };
            return PartialView("_CustodyActionModal", model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ExecuteCustodyAction(VaultCustodyActionDto dto)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);
            await _vaultService.ExecuteCustodyActionAsync(dto, CurrentUserId);
            Response.Headers.Append("HX-Trigger", "reloadVaultItems");
            return Ok();
        }
        #endregion

        #region Base Tables Tab
        [HttpGet]
        public async Task<IActionResult> GetBaseTablesTab(string entityType = "category")
        {
            var data = await _vaultService.GetBaseTableDataAsync(entityType);
            ViewBag.EntityType = entityType;
            return PartialView("_BaseTablesTab", data);
        }

        [HttpPost]
        public async Task<IActionResult> UpsertBaseTableItem(LookupUpsertDto dto)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);
            await _vaultService.UpsertBaseTableItemAsync(dto);
            return RedirectToAction(nameof(GetBaseTablesTab), new { entityType = dto.EntityType });
        }

        [HttpPost]
        public async Task<IActionResult> DeleteBaseTableItem(string entityType, int id)
        {
            await _vaultService.DeleteBaseTableItemAsync(entityType, id);
            return RedirectToAction(nameof(GetBaseTablesTab), new { entityType });
        }
        #endregion




        #region Vault Registry (CRUD & List)
        [HttpGet]
        public async Task<IActionResult> GetVaultsTab()
        {
            var vaults = await _vaultService.GetAllVaultsAsync();
            return PartialView("_VaultsTab", vaults);
        }

        [HttpGet]
        public async Task<IActionResult> UpsertVaultModal(Guid? id)
        {
            var model = new VaultUpsertDto();
            if (id.HasValue && id.Value != Guid.Empty)
            {
                var existing = await _vaultService.GetVaultByIdAsync(id.Value);
                if (existing != null)
                {
                    model.Id = existing.Id;
                    model.VaultName = existing.VaultName;
                    model.Remarks = existing.Remarks;
                    model.IsActive = existing.IsActive;
                    model.CompanyId = existing.CompanyId;
                    model.KeeperUserId = existing.KeeperUserId;
                    model.BranchId = existing.BranchId;
                }
            }

            var model2 = await _vaultService.GetVaultFormVmAsync(id);

            model.CompanyList = model2.Companies;
            model.BranchList = model2.Branches;
            model.KeeperUserList = model2.Keepers;
            return PartialView("_UpsertVaultModal", model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> UpsertVault(VaultUpsertDto dto)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            await _vaultService.UpsertVaultAsync(dto, CurrentUserId);
            Response.Headers.Append("HX-Trigger", "reloadVaultList");
            return Ok();
        }

        [HttpPost]
        public async Task<IActionResult> ToggleVaultStatus(Guid id)
        {
            var res = await _vaultService.ToggleVaultStatusAsync(id);
            if (!res) return BadRequest("Could not change vault status.");
            Response.Headers.Append("HX-Trigger", "reloadVaultList");
            return Ok();
        }
        #endregion
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ApproveInitialPaymentRequest(Guid id)
        {
            var result = await _vaultService.ApproveInitialPaymentRequestAsync(id, CurrentUserId);
            // بازگرداندن تب یا سطر به‌روزشده با HTMX
            return RedirectToAction(nameof(GetPaymentRequestsTab));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ApproveFinalPaymentRequest(Guid id)
        {
            var result = await _vaultService.ApproveFinalPaymentRequestAsync(id, CurrentUserId);
            return RedirectToAction(nameof(GetPaymentRequestsTab));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> RejectPaymentRequest(Guid id, string reason)
        {
            var result = await _vaultService.RejectPaymentRequestAsync(id, reason, CurrentUserId);
            return RedirectToAction(nameof(GetPaymentRequestsTab));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ExecutePaymentRequest(Guid id)
        {
            var result = await _vaultService.ExecutePaymentRequestAsync(id, CurrentUserId);
            return RedirectToAction(nameof(GetPaymentRequestsTab));
        }

        [HttpGet]
        public async Task<IActionResult> CustodyHistoryModal(Guid itemId)
        {
            var model = await _vaultService.GetItemCustodyHistoryAsync(itemId);
            if (model == null) return NotFound();

            return PartialView("_CustodyHistoryModal", model);
        }





    }
}
