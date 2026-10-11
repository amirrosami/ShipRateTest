using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ShipRate.Application.Interfaces;
using ShipRate.Application.Interfaces.Jobs;
using ShipRate.Application.ViewModels;

namespace ShipRate.WebApp.Controllers
{
    [Authorize]
    public class CustomsClearanceController : Controller
    {
        private readonly ICustomsClearanceService _service;
        private readonly IJobLookupService _lookupService;
        private readonly ICustomsClearanceItemService _customsClearanceItemService;
        private readonly ICustomsClearanceContainerService _customsClearanceContainerService;
        private readonly ICustomsClearanceChargeService _customsClearanceChargeService;
        private readonly ICustomsClearanceTimelineService _customsClearanceTimelineService;
        private readonly IActivityLogService _activityLogService;
        private readonly ICustomsClearanceLicenseService _customsClearanceLicenseService;
        private readonly ICustomsInspectionService _inspectionService;

        public CustomsClearanceController(ICustomsClearanceService service, IJobLookupService lookupService, ICustomsClearanceItemService customsClearanceItemService, ICustomsClearanceContainerService customsClearanceContainerService, ICustomsClearanceChargeService customsClearanceChargeService, ICustomsClearanceTimelineService customsClearanceTimelineService, IActivityLogService activityLogService, ICustomsClearanceLicenseService customsClearanceLicenseService, ICustomsInspectionService inspectionService)
        {
            _service = service;
            _lookupService = lookupService;
            _customsClearanceItemService = customsClearanceItemService;
            _customsClearanceContainerService = customsClearanceContainerService;
            _customsClearanceChargeService = customsClearanceChargeService;
            _customsClearanceTimelineService = customsClearanceTimelineService;
            _activityLogService = activityLogService;
            _customsClearanceLicenseService = customsClearanceLicenseService;
            _inspectionService = inspectionService;
        }

        [HttpGet]
        public async Task<IActionResult> JobTab(Guid jobId)
        {
            var vm = await _service.GetJobTabAsync(jobId);
            return PartialView("~/Views/CustomsClearances/_JobTab.cshtml", vm);
        }

        [HttpGet]
        public async Task<IActionResult> CreateModal(Guid jobId)
        {
            var companyId = GetCurrentCompanyId();

            var vm = await _service.BuildCreateVmAsync(jobId, companyId);
            return PartialView("~/Views/CustomsClearances/_CreateEditModal.cshtml", vm);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(CustomsClearanceCreateEditVm model)
        {
            if (!ModelState.IsValid)
            {
                model.DeclarationTypes = await _lookupService.GetDeclarationTypesAsync();
                model.CustomOffices = await _lookupService.GetCustomOfficesAsync();
                model.Persons = await _lookupService.GetPersonsAsync();

                return PartialView("~/Views/CustomsClearances/_CreateEditModal.cshtml", model);
            }

            await _service.CreateAsync(model);

            Response.Headers["HX-Trigger"] = "customsClearanceChanged";
            return Content(string.Empty);
        }


        [HttpGet]
        public async Task<IActionResult> EditModal(Guid id)
        {
            var companyId = GetCurrentCompanyId();

            var vm = await _service.BuildEditVmAsync(id, companyId);
            if (vm == null)
                return NotFound();

            return PartialView("~/Views/CustomsClearances/_CreateEditModal.cshtml", vm);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(CustomsClearanceCreateEditVm model)
        {
            if (!ModelState.IsValid || model.Id == null)
            {
                var vm = await _service.BuildEditVmAsync(model.Id ?? Guid.Empty, model.CompanyId);
                if (vm == null)
                    return NotFound();

                return PartialView("~/Views/CustomsClearances/_CreateEditModal.cshtml", vm);
            }

            await _service.UpdateAsync(model);

            Response.Headers["HX-Trigger"] = "customsClearanceChanged";
            return Content(string.Empty);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(Guid id, Guid jobId)
        {
            var companyId = GetCurrentCompanyId();

            await _service.DeleteAsync(id, companyId);

            var vm = await _service.GetJobTabAsync(jobId);
            return PartialView("~/Views/CustomsClearances/_JobTab.cshtml", vm);
        }
        private int GetCurrentCompanyId()
        {
            return 1;
        }

        [HttpGet]
        public async Task<IActionResult> Details(Guid id)
        {
            var companyId = GetCurrentCompanyId();

            // فراخوانی داده‌ها از طریق لایه سرویس
            var details = await _service.GetDetailsAsync(id, companyId);

            if (details == null)
                return NotFound();

            return View("~/Views/CustomsClearances/Details.cshtml", details);
        }
        [HttpGet]
        public async Task<IActionResult> GetGeneralTab(Guid id)
        {
            var companyId = GetCurrentCompanyId();

            var model = await _service.GetGeneralTabVmAsync(id, companyId);
            if (model == null)
                return NotFound();

            return PartialView("~/Views/CustomsClearances/_TabGeneral.cshtml", model);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> UpdateGeneralInfo(CustomsClearanceCreateEditVm model)
        {
            var companyId = GetCurrentCompanyId();

            if (!ModelState.IsValid)
            {
                model = await _service.GetGeneralTabVmAsync(model.Id.Value, companyId) ?? model;
                return PartialView("~/Views/CustomsClearances/_TabGeneral.cshtml", model);
            }

            await _service.UpdateGeneralInfoAsync(model, companyId);

            Response.Headers["HX-Trigger"] = "customsClearanceChanged, closeModal";
            //Response.Headers["HX-Trigger"] = JsonSerializer.Serialize(new
            //{
            //    customsClearanceChanged = true,
            //    closeModal = true
            //});


            return Ok();
        }





        public async Task<IActionResult> GetItemsTab(Guid id)
        {
            var companyId = GetCurrentCompanyId();
            var vm = await _customsClearanceItemService.GetItemsTabAsync(id, companyId);
            return PartialView("~/Views/CustomsClearances/_TabItems.cshtml", vm);
        }

        [HttpGet]
        public async Task<IActionResult> CreateItemModal(Guid customsClearanceId)
        {
            var companyId = GetCurrentCompanyId();
            var vm = await _customsClearanceItemService.BuildCreateVmAsync(customsClearanceId, companyId);
            return PartialView("~/Views/CustomsClearances/_CreateEditItemModal.cshtml", vm);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateItem(CustomsClearanceItemCreateEditVm model)
        {
            var companyId = GetCurrentCompanyId();

            if (!ModelState.IsValid)
            {
                // چون lookupها در post از بین می‌روند، باید دوباره بسازیم
                var rebuilt = await _customsClearanceItemService.BuildCreateVmAsync(model.CustomsClearanceId, companyId);
                await _customsClearanceItemService.PopulateLookupsAsync(rebuilt, companyId);

                rebuilt.Id = model.Id;
                rebuilt.ItemSequence = model.ItemSequence;
                rebuilt.CountryHSCodeId = model.CountryHSCodeId;
                rebuilt.HSCodeToSave = model.HSCodeToSave;
                rebuilt.GoodsDescriptionEN = model.GoodsDescriptionEN;
                rebuilt.GoodsDescriptionLocal = model.GoodsDescriptionLocal;
                rebuilt.Quantity = model.Quantity;
                rebuilt.MeasurementUnitId = model.MeasurementUnitId;
                rebuilt.StatisticalQuantity = model.StatisticalQuantity;
                rebuilt.StatisticalMeasurementUnitId = model.StatisticalMeasurementUnitId;
                rebuilt.UnitPrice = model.UnitPrice;
                rebuilt.CurrencyId = model.CurrencyId;
                rebuilt.TotalValue = model.TotalValue;
                rebuilt.Remarks = model.Remarks;

                return PartialView("~/Views/CustomsClearances/_CreateEditItemModal.cshtml", rebuilt);
            }

            await _customsClearanceItemService.CreateAsync(model, companyId);

            Response.Headers["HX-Trigger"] = """{"customsClearanceItemsChanged":true,"closeModal":true}""";
            return Ok();
        }

        [HttpGet]
        public async Task<IActionResult> EditItemModal(Guid id)
        {
            var companyId = GetCurrentCompanyId();
            var vm = await _customsClearanceItemService.BuildEditVmAsync(id, companyId);

            if (vm == null)
                return NotFound();

            return PartialView("~/Views/CustomsClearances/_CreateEditItemModal.cshtml", vm);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditItem(CustomsClearanceItemCreateEditVm model)
        {
            var companyId = GetCurrentCompanyId();

            if (!ModelState.IsValid)
            {
                if (!model.Id.HasValue)
                    return BadRequest();

                var rebuilt = await _customsClearanceItemService.BuildEditVmAsync(model.Id.Value, companyId);
                if (rebuilt == null)
                    return NotFound();

                rebuilt.ItemSequence = model.ItemSequence;
                rebuilt.CountryHSCodeId = model.CountryHSCodeId;
                rebuilt.HSCodeToSave = model.HSCodeToSave;
                rebuilt.GoodsDescriptionEN = model.GoodsDescriptionEN;
                rebuilt.GoodsDescriptionLocal = model.GoodsDescriptionLocal;
                rebuilt.Quantity = model.Quantity;
                rebuilt.MeasurementUnitId = model.MeasurementUnitId;
                rebuilt.StatisticalQuantity = model.StatisticalQuantity;
                rebuilt.StatisticalMeasurementUnitId = model.StatisticalMeasurementUnitId;
                rebuilt.UnitPrice = model.UnitPrice;
                rebuilt.CurrencyId = model.CurrencyId;
                rebuilt.TotalValue = model.TotalValue;
                rebuilt.Remarks = model.Remarks;

                return PartialView("~/Views/CustomsClearances/_CreateEditItemModal.cshtml", rebuilt);
            }

            await _customsClearanceItemService.UpdateAsync(model, companyId);

            Response.Headers["HX-Trigger"] = """{"customsClearanceItemsChanged":true,"closeModal":true}""";
            return Ok();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteItem(Guid id)
        {
            var companyId = GetCurrentCompanyId();
            await _customsClearanceItemService.DeleteAsync(id, companyId);

            Response.Headers["HX-Trigger"] = """{"customsClearanceItemsChanged":true}""";
            return Ok();
        }



        public async Task<IActionResult> GetContainersTab(Guid id)
        {
            var companyId = GetCurrentCompanyId();
            var vm = await _customsClearanceContainerService.GetContainersTabAsync(id, companyId);
            return PartialView("~/Views/CustomsClearances/_TabContainers.cshtml", vm);
        }

        [HttpGet]
        public async Task<IActionResult> CreateContainerModal(Guid customsClearanceId)
        {
            var companyId = GetCurrentCompanyId();
            var vm = await _customsClearanceContainerService.BuildCreateVmAsync(customsClearanceId, companyId);
            return PartialView("~/Views/CustomsClearances/_CreateEditContainerModal.cshtml", vm);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateContainer(CustomsClearanceContainerCreateEditVm model)
        {
            var companyId = GetCurrentCompanyId();

            if (!ModelState.IsValid)
            {
                await _customsClearanceContainerService.PopulateLookupsAsync(model, companyId);
                return PartialView("~/Views/CustomsClearances/_CreateEditContainerModal.cshtml", model);
            }

            try
            {
                await _customsClearanceContainerService.CreateAsync(model, companyId);
            }
            catch (InvalidOperationException ex)
            {
                ModelState.AddModelError(string.Empty, ex.Message);
                await _customsClearanceContainerService.PopulateLookupsAsync(model, companyId);
                return PartialView("~/Views/CustomsClearances/_CreateEditContainerModal.cshtml", model);
            }

            Response.Headers["HX-Trigger"] = """{"customsClearanceContainersChanged":true,"closeModal":true}""";
            return Ok();
        }

        [HttpGet]
        public async Task<IActionResult> EditContainerModal(Guid id)
        {
            var companyId = GetCurrentCompanyId();
            var vm = await _customsClearanceContainerService.BuildEditVmAsync(id, companyId);

            if (vm == null)
                return NotFound();

            return PartialView("~/Views/CustomsClearances/_CreateEditContainerModal.cshtml", vm);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditContainer(CustomsClearanceContainerCreateEditVm model)
        {
            var companyId = GetCurrentCompanyId();

            if (!ModelState.IsValid)
            {
                await _customsClearanceContainerService.PopulateLookupsAsync(model, companyId);
                return PartialView("~/Views/CustomsClearances/_CreateEditContainerModal.cshtml", model);
            }

            try
            {
                await _customsClearanceContainerService.UpdateAsync(model, companyId);
            }
            catch (InvalidOperationException ex)
            {
                ModelState.AddModelError(string.Empty, ex.Message);
                await _customsClearanceContainerService.PopulateLookupsAsync(model, companyId);
                return PartialView("~/Views/CustomsClearances/_CreateEditContainerModal.cshtml", model);
            }

            Response.Headers["HX-Trigger"] = """{"customsClearanceContainersChanged":true,"closeModal":true}""";
            return Ok();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteContainer(Guid id)
        {
            var companyId = GetCurrentCompanyId();

            try
            {
                await _customsClearanceContainerService.DeleteAsync(id, companyId);
            }
            catch (InvalidOperationException ex)
            {
                Response.StatusCode = 400;
                return Content(ex.Message);
            }

            Response.Headers["HX-Trigger"] = """{"customsClearanceContainersChanged":true}""";
            return Ok();
        }


        public async Task<IActionResult> GetChargesTab(Guid id)
        {
            var companyId = GetCurrentCompanyId();
            var vm = await _customsClearanceChargeService.GetChargesTabAsync(id, companyId);
            return PartialView("~/Views/CustomsClearances/_TabCharges.cshtml", vm);
        }

        [HttpGet]
        public async Task<IActionResult> CreateChargeModal(Guid customsClearanceId)
        {
            var companyId = GetCurrentCompanyId();
            var vm = await _customsClearanceChargeService.BuildCreateVmAsync(customsClearanceId, companyId);
            return PartialView("~/Views/CustomsClearances/_CreateEditChargeModal.cshtml", vm);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateCharge(CustomsClearanceChargeCreateEditVm model)
        {
            var companyId = GetCurrentCompanyId();

            if (!ModelState.IsValid)
            {
                await _customsClearanceChargeService.PopulateLookupsAsync(model, companyId);
                return PartialView("~/Views/CustomsClearances/_CreateEditChargeModal.cshtml", model);
            }

            try
            {
                await _customsClearanceChargeService.CreateAsync(model, companyId);
            }
            catch (InvalidOperationException ex)
            {
                ModelState.AddModelError(string.Empty, ex.Message);
                await _customsClearanceChargeService.PopulateLookupsAsync(model, companyId);
                return PartialView("~/Views/CustomsClearances/_CreateEditChargeModal.cshtml", model);
            }

            Response.Headers["HX-Trigger"] = """{"customsClearanceChargesChanged":true,"closeModal":true}""";
            return Ok();
        }

        [HttpGet]
        public async Task<IActionResult> EditChargeModal(Guid id)
        {
            var companyId = GetCurrentCompanyId();
            var vm = await _customsClearanceChargeService.BuildEditVmAsync(id, companyId);

            if (vm == null)
                return NotFound();

            return PartialView("~/Views/CustomsClearances/_CreateEditChargeModal.cshtml", vm);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditCharge(CustomsClearanceChargeCreateEditVm model)
        {
            var companyId = GetCurrentCompanyId();

            if (!ModelState.IsValid)
            {
                await _customsClearanceChargeService.PopulateLookupsAsync(model, companyId);
                return PartialView("~/Views/CustomsClearances/_CreateEditChargeModal.cshtml", model);
            }

            try
            {
                await _customsClearanceChargeService.UpdateAsync(model, companyId);
            }
            catch (InvalidOperationException ex)
            {
                ModelState.AddModelError(string.Empty, ex.Message);
                await _customsClearanceChargeService.PopulateLookupsAsync(model, companyId);
                return PartialView("~/Views/CustomsClearances/_CreateEditChargeModal.cshtml", model);
            }

            Response.Headers["HX-Trigger"] = """{"customsClearanceChargesChanged":true,"closeModal":true}""";
            return Ok();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteCharge(Guid id)
        {
            var companyId = GetCurrentCompanyId();

            try
            {
                await _customsClearanceChargeService.DeleteAsync(id, companyId);
            }
            catch (InvalidOperationException ex)
            {
                Response.StatusCode = 400;
                return Content(ex.Message);
            }

            Response.Headers["HX-Trigger"] = """{"customsClearanceChargesChanged":true}""";
            return Ok();
        }

        public async Task<IActionResult> GetTimelineTab(Guid id, CancellationToken cancellationToken)
        {
            const int customsClearanceEntityTypeId = 14;

            var timeline = await _activityLogService.GetEntityTimelineAsync(
                customsClearanceEntityTypeId,
                id,
                cancellationToken);

            var vm = new CustomsClearanceTimelineTabVm
            {
                CustomsClearanceId = id,
                Entries = timeline.Select(x => new ActivityLogListItemVm
                {
                    Id = x.Id,
                    ActivityDateTime = x.ActivityDateTime,
                    Title = x.Title,
                    Description = x.Description,
                    ActivityType = x.ActivityType,
                    Severity = x.Severity,
                    IconCssClass = x.IconCssClass,
                    PerformedByUserName = x.PerformedByUserName
                }).ToList()
            };
            return PartialView("~/Views/CustomsClearances/_TabTimeline.cshtml", vm);
        }


        public async Task<IActionResult> GetLicensesTab(Guid id)
        {
            var companyId = GetCurrentCompanyId();
            var vm = await _customsClearanceLicenseService.GetLicensesTabAsync(id, companyId);
            return PartialView("~/Views/CustomsClearances/_TabLicenses.cshtml", vm);
        }
        public async Task<IActionResult> CreateLicenseModal(Guid customsClearanceId)
        {
            var companyId = GetCurrentCompanyId();
            var vm = await _customsClearanceLicenseService.BuildCreateVmAsync(customsClearanceId, companyId);
            return PartialView("~/Views/CustomsClearances/_CreateEditLicenseModal.cshtml", vm);
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateLicense(CustomsClearanceLicenseCreateEditVm vm)
        {
            var companyId = GetCurrentCompanyId();

            if (!ModelState.IsValid)
            {
                await RebuildLicenseVmLookups(vm, companyId);
                return PartialView("~/Views/CustomsClearances/_CreateEditLicenseModal.cshtml", vm);
            }

            try
            {
                await _customsClearanceLicenseService.CreateAsync(vm, companyId);

                Response.Headers["HX-Trigger"] = @"{""customsClearanceLicensesChanged"":true,""closeModal"":true}";
                return Content(string.Empty);
            }
            catch (InvalidOperationException ex)
            {
                ModelState.AddModelError(string.Empty, ex.Message);
                await RebuildLicenseVmLookups(vm, companyId);
                return PartialView("~/Views/CustomsClearances/_CreateEditLicenseModal.cshtml", vm);
            }
        }

        public async Task<IActionResult> EditLicenseModal(Guid id)
        {
            var companyId = GetCurrentCompanyId();
            var vm = await _customsClearanceLicenseService.BuildEditVmAsync(id, companyId);
            return PartialView("~/Views/CustomsClearances/_CreateEditLicenseModal.cshtml", vm);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditLicense(CustomsClearanceLicenseCreateEditVm vm)
        {
            var companyId = GetCurrentCompanyId();

            if (!ModelState.IsValid)
            {
                await RebuildLicenseVmLookups(vm, companyId);
                return PartialView("~/Views/CustomsClearances/_CreateEditLicenseModal.cshtml", vm);
            }

            try
            {
                await _customsClearanceLicenseService.UpdateAsync(vm, companyId);

                Response.Headers["HX-Trigger"] = @"{""customsClearanceLicensesChanged"":true,""closeModal"":true}";
                return Content(string.Empty);
            }
            catch (InvalidOperationException ex)
            {
                ModelState.AddModelError(string.Empty, ex.Message);
                await RebuildLicenseVmLookups(vm, companyId);
                return PartialView("~/Views/CustomsClearances/_CreateEditLicenseModal.cshtml", vm);
            }
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteLicense(Guid id)
        {
            var companyId = GetCurrentCompanyId();

            try
            {
                await _customsClearanceLicenseService.DeleteAsync(id, companyId);
                Response.Headers["HX-Trigger"] = @"{""customsClearanceLicensesChanged"":true}";
                return Content(string.Empty);
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(ex.Message);
            }
        }

        private async Task RebuildLicenseVmLookups(CustomsClearanceLicenseCreateEditVm vm, int companyId)
        {
            CustomsClearanceLicenseCreateEditVm freshVm;

            if (vm.Id.HasValue)
                freshVm = await _customsClearanceLicenseService.BuildEditVmAsync(vm.Id.Value, companyId);
            else
                freshVm = await _customsClearanceLicenseService.BuildCreateVmAsync(vm.CustomsClearanceId, companyId);

            vm.LicenseTypeLookup = freshVm.LicenseTypeLookup;
            vm.LicenseStatusLookup = freshVm.LicenseStatusLookup;
            vm.CanEdit = freshVm.CanEdit;
        }

        #region Inspections Tab

        [HttpGet]
        public async Task<IActionResult> GetInspectionsTab(Guid id)
        {
            var CurrentCompanyId = GetCurrentCompanyId();

            var viewModel = await _inspectionService.GetInspectionsTabAsync(id, CurrentCompanyId);
            return PartialView("~/Views/CustomsClearances/_TabInspections.cshtml", viewModel);
        }

        [HttpGet]
        public async Task<IActionResult> CreateInspectionModal(Guid customsClearanceId)
        {
            try
            {
                var CurrentCompanyId = GetCurrentCompanyId();
                var vm = await _inspectionService.BuildCreateVmAsync(customsClearanceId, CurrentCompanyId);
                return PartialView("~/Views/CustomsClearances/_CreateEditInspectionModal.cshtml", vm);
            }
            catch (Exception ex) when (ex is KeyNotFoundException || ex is InvalidOperationException)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateInspection(CustomsInspectionCreateEditVm vm)
        {
            if (!ModelState.IsValid)
            {
                var CurrentCompanyId = GetCurrentCompanyId();
                await _inspectionService.PopulateLookupsAsync(vm, CurrentCompanyId);
                return PartialView("~/Views/CustomsClearances/_CreateEditInspectionModal.cshtml", vm);
            }

            try
            {
                var CurrentCompanyId = GetCurrentCompanyId();
                await _inspectionService.CreateAsync(vm, CurrentCompanyId);

                //Response.Htmx(h =>
                //{
                //    h.Trigger("customsClearanceInspectionsChanged");
                //    h.Trigger("closeModal");
                //});

                Response.Headers["HX-Trigger"] = "customsClearanceInspectionsChanged,closeModal";



                return Ok();
            }
            catch (Exception ex)
            {
                var CurrentCompanyId = GetCurrentCompanyId();
                ModelState.AddModelError("", ex.Message);
                await _inspectionService.PopulateLookupsAsync(vm, CurrentCompanyId);
                return PartialView("~/Views/CustomsClearances/_CreateEditInspectionModal.cshtml", vm);
            }
        }

        [HttpGet]
        public async Task<IActionResult> EditInspectionModal(Guid id)
        {
            try
            {
                var CurrentCompanyId = GetCurrentCompanyId();
                var vm = await _inspectionService.BuildEditVmAsync(id, CurrentCompanyId);
                return PartialView("~/Views/CustomsClearances/_CreateEditInspectionModal.cshtml", vm);
            }
            catch (Exception ex) when (ex is KeyNotFoundException || ex is InvalidOperationException)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditInspection(CustomsInspectionCreateEditVm vm)
        {
            if (!ModelState.IsValid)
            {
                var CurrentCompanyId = GetCurrentCompanyId();
                await _inspectionService.PopulateLookupsAsync(vm, CurrentCompanyId);
                return PartialView("~/Views/CustomsClearances/_CreateEditInspectionModal.cshtml", vm);
            }

            try
            {
                var CurrentCompanyId = GetCurrentCompanyId();
                await _inspectionService.UpdateAsync(vm, CurrentCompanyId);

                //Response.Htmx(h =>
                //{
                //    h.Trigger("customsClearanceInspectionsChanged");
                //    h.Trigger("closeModal");
                //});

                Response.Headers["HX-Trigger"] = "customsClearanceInspectionsChanged,closeModal";

                return Ok();
            }
            catch (Exception ex)
            {
                var CurrentCompanyId = GetCurrentCompanyId();
                ModelState.AddModelError("", ex.Message);
                await _inspectionService.PopulateLookupsAsync(vm, CurrentCompanyId);
                return PartialView("~/Views/CustomsClearances/_CreateEditInspectionModal.cshtml", vm);
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteInspection(Guid id)
        {
            try
            {
                // حذف رکورد بازرسی و دریافت شناسه والد از لایه سرویس
                var CurrentCompanyId = GetCurrentCompanyId();
                var customsClearanceId = await _inspectionService.DeleteAsync(id, CurrentCompanyId);

                Response.Headers["HX-Trigger"] = "customsClearanceInspectionsChanged";
                return Ok();
            }
            catch (Exception ex) when (ex is KeyNotFoundException || ex is InvalidOperationException)
            {
                return BadRequest(ex.Message);
            }
        }
        #endregion
    }
}
