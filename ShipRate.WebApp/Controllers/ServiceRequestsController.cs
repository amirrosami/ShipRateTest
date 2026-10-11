using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ShipRate.Application.DTOs;
using ShipRate.Application.Interfaces;
using ShipRate.Application.ViewModels;

namespace ShipRate.WebApp.Controllers
{
    [Authorize]
    public class ServiceRequestsController : Controller
    {
        private readonly IServiceRequestService _service;
        private readonly ITransportDocumentService _transportDocumentService;
        public ServiceRequestsController(IServiceRequestService service, ITransportDocumentService transportDocumentService)
        {
            _service = service;
            _transportDocumentService = transportDocumentService;
        }
        [HttpGet]
        [Authorize("Operations.ServiceRequests.View")]
        public async Task<IActionResult> Index([FromQuery] ServiceRequestSearchDto input)
        {
            var model = await _service.GetIndexDataAsync(input);

            if (Request.Headers["HX-Request"] == "true")
                return PartialView("_Grid", model);

            return View(model);
        }
        [HttpGet]
        [Authorize("Operations.ServiceRequests.Edit")]
        public async Task<IActionResult> Create()
        {
            var model = await _service.GetCreateFormAsync();
            return PartialView("_CreateEdit", model);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize("Operations.ServiceRequests.Edit")]
        public async Task<IActionResult> Create(ServiceRequestFormDto form)
        {
            if (!ModelState.IsValid)
            {
                var reload = await _service.GetCreateFormAsync();
                reload.Model = form.Model;
                reload.PartyDisplayName = form.PartyDisplayName;
                return PartialView("_CreateEdit", reload);
            }

            try
            {
                await _service.CreateAsync(form.Model);
                Response.Headers["HX-Trigger"] = "serviceRequestChanged";
                return Content(string.Empty);
            }
            catch (Exception ex)
            {
                ModelState.AddModelError(string.Empty, ex.Message);
                var reload = await _service.GetCreateFormAsync();
                reload.Model = form.Model;
                reload.PartyDisplayName = form.PartyDisplayName;
                return PartialView("_CreateEdit", reload);
            }
        }

        [HttpGet]
        [Authorize("Operations.ServiceRequests.Edit")]
        public async Task<IActionResult> Edit(Guid id)
        {
            var model = await _service.GetEditFormAsync(id);
            return PartialView("_CreateEdit", model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize("Operations.ServiceRequests.Edit")]
        public async Task<IActionResult> Edit(ServiceRequestFormDto form)
        {
            if (!ModelState.IsValid)
            {
                var reload = await _service.GetCreateFormAsync();
                reload.Model = form.Model;
                reload.PartyDisplayName = form.PartyDisplayName;
                return PartialView("_CreateEdit", reload);
            }

            try
            {
                await _service.UpdateAsync(form.Model);
                Response.Headers["HX-Trigger"] = "serviceRequestChanged";
                return Content(string.Empty);
            }
            catch (Exception ex)
            {
                ModelState.AddModelError(string.Empty, ex.Message);
                var reload = await _service.GetCreateFormAsync();
                reload.Model = form.Model;
                reload.PartyDisplayName = form.PartyDisplayName;
                return PartialView("_CreateEdit", reload);
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize("Operations.ServiceRequests.Edit")]
        public async Task<IActionResult> Delete(Guid id)
        {
            try
            {
                await _service.DeleteAsync(id);
                Response.Headers["HX-Trigger"] = "serviceRequestChanged";
                return Content(string.Empty);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpGet]
        public async Task<IActionResult> SearchPersons(string term, string target)
        {
            var items = await _service.SearchPersonsAsync(term);
            ViewBag.Target = target;
            return PartialView("_PersonSearchResults", items);
        }

        [HttpGet]
        [Authorize("Operations.ServiceRequests.View")]
        public async Task<IActionResult> ApprovalsPanel(Guid serviceRequestId)
        {
            var model = await _service.GetApprovalsPanelAsync(serviceRequestId);
            return PartialView("_ApprovalsPanel", model);
        }

        [HttpGet]
        [Authorize("Operations.ServiceRequests.Approve")]
        public async Task<IActionResult> CreateApproval(Guid serviceRequestId)
        {
            var model = await _service.GetApprovalCreateFormAsync(serviceRequestId);

            return PartialView("_ApprovalFormModal", model);
        }

        [HttpGet]
        [Authorize("Operations.ServiceRequests.Approve")]
        public async Task<IActionResult> EditApproval(Guid id)
        {
            var model = await _service.GetApprovalEditFormAsync(id);

            return PartialView("_ApprovalFormModal", model);
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize("Operations.ServiceRequests.Approve")]
        public async Task<IActionResult> SaveApproval(ServiceRequestApprovalFormDto model)
        {
            if (!ModelState.IsValid)
            {
                // در صورت خطا باید lookupها دوباره پر شوند
                // بهتر است یک متد در سرویس برای این کار عمومی شود
                model = model.Id.HasValue
                    ? await _service.GetApprovalEditFormAsync(model.Id.Value)
                    : await _service.GetApprovalCreateFormAsync(model.ServiceRequestId);

                return PartialView("_ApprovalFormModal", model);
            }

            await _service.SaveApprovalAsync(model);

            Response.Headers["HX-Trigger"] = $"approvalSaved";

            var panel = await _service.GetApprovalsPanelAsync(model.ServiceRequestId);

            return PartialView("_ApprovalsPanel", panel);
        }

        [HttpDelete]
        [Authorize("Operations.ServiceRequests.Approve")]
        public async Task<IActionResult> DeleteApproval(Guid id, Guid serviceRequestId)
        {
            await _service.DeleteApprovalAsync(id);

            var panel = await _service.GetApprovalsPanelAsync(serviceRequestId);

            return PartialView("_ApprovalsPanel", panel);
        }

        [HttpGet]
        public IActionResult EmptyApprovalsPanel()
        {
            return Content("");
        }


        [HttpGet]
        [Authorize("Operations.ServiceRequests.View")]
        public async Task<IActionResult> ChargesPanel(Guid serviceRequestId)
        {
            var model = await _service.GetChargesPanelAsync(serviceRequestId);
            return PartialView("_ChargesPanel", model);
        }

        [HttpGet]
        [Authorize("Operations.ServiceRequests.Edit")]
        public async Task<IActionResult> CreateCharge(Guid serviceRequestId)
        {
            var model = await _service.GetCreateChargeFormAsync(serviceRequestId);
            return PartialView("_ChargeFormModal", model);
        }


        [HttpGet]
        [Authorize("Operations.ServiceRequests.Edit")]
        public async Task<IActionResult> EditCharge(Guid id)
        {
            var model = await _service.GetEditChargeFormAsync(id);

            if (model == null)
                return NotFound();

            return PartialView("_ChargeFormModal", model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize("Operations.ServiceRequests.Edit")]
        public async Task<IActionResult> SaveCharge(ServiceRequestChargeFormDto model)
        {
            if (!ModelState.IsValid)
            {
                model.ChargeTypes = await _service.GetChargeTypeSelectListAsync();
                model.Currencies = await _service.GetCurrencySelectListAsync();
                return PartialView("_ChargeFormModal", model);
            }

            await _service.SaveChargeAsync(model);

            var panelModel = await _service.GetChargesPanelAsync(model.ServiceRequestId);
            return PartialView("_ChargesPanel", panelModel);
        }

        [HttpDelete]
        [Authorize("Operations.ServiceRequests.Edit")]
        public async Task<IActionResult> DeleteCharge(Guid id, Guid serviceRequestId)
        {
            await _service.DeleteChargeAsync(id);

            var panelModel = await _service.GetChargesPanelAsync(serviceRequestId);
            return PartialView("_ChargesPanel", panelModel);
        }
        [HttpGet]
        [Authorize("Operations.ServiceRequests.Edit")]
        public async Task<IActionResult> ShowSpecificPanel(Guid serviceRequestId, int requestTypeId)
        {

            switch (requestTypeId)
            {
                case 1:
                    var model = await _service.GetBLAmendmentsPanelAsync(serviceRequestId);
                    return PartialView("_BLAmendmentsPanel", model);
                //case 3:
                //    var smodel = await _service.GetSplitRequestsPanelAsync(serviceRequestId);
                //    return PartialView("_SplitRequestsPanel", smodel);
                default:
                    return NoContent();
            }
        }

        [HttpGet]
        [Authorize("Operations.ServiceRequests.Edit")]
        public async Task<IActionResult> SplitRequestsPanel(Guid serviceRequestId)
        {
            var vm = await _service.GetSplitRequestPanelAsync(serviceRequestId);
            if (vm == null) return Content("Split Request data not found.");
            return PartialView("_SplitRequestsPanel", vm);
        }




        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize("Operations.ServiceRequests.Edit")]
        public async Task<IActionResult> CreateSplitRequest(CreateSplitRequestVm model)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest("Source bill of lading is required.");
            }

            var result = await _service.CreateSplitRequestAsync(model);

            if (!result.Success)
            {
                return result.StatusCode == 404
                    ? NotFound(result.Message)
                    : BadRequest(result.Message);
            }

            return RedirectToAction(nameof(AdvancedSplitEditor), new
            {
                splitRequestId = result.SplitRequestId
            });
        }






        [HttpGet]
        [Authorize("Operations.ServiceRequests.Edit")]
        public async Task<IActionResult> CreateBLAmendmentRequest(Guid serviceRequestId)
        {
            var model = await _service.GetCreateBLAmendmentRequestFormAsync(serviceRequestId);
            return PartialView("_BLAmendmentRequestFormModal", model);
        }
        [HttpGet]
        [Authorize("Operations.ServiceRequests.Edit")]
        public async Task<IActionResult> EditBLAmendmentRequest(Guid id)
        {
            var model = await _service.GetEditBLAmendmentRequestFormAsync(id);
            if (model == null)
                return NotFound();

            return PartialView("_BLAmendmentRequestFormModal", model);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize("Operations.ServiceRequests.Edit")]
        public async Task<IActionResult> SaveBLAmendmentRequest(BLAmendmentRequestFormDto model)
        {
            if (!ModelState.IsValid)
            {
                model.BLs = await _service.GetBLSelectListAsync(model.ServiceRequestId);
                return PartialView("_BLAmendmentRequestFormModal", model);
            }

            await _service.SaveBLAmendmentRequestAsync(model);

            var panelModel = await _service.GetBLAmendmentsPanelAsync(model.ServiceRequestId);
            return PartialView("_BLAmendmentsPanel", panelModel);
        }
        [HttpDelete]
        [Authorize("Operations.ServiceRequests.Edit")]
        public async Task<IActionResult> DeleteBLAmendmentRequest(Guid id, Guid serviceRequestId)
        {
            await _service.DeleteBLAmendmentRequestAsync(id);

            var panelModel = await _service.GetBLAmendmentsPanelAsync(serviceRequestId);
            return PartialView("_BLAmendmentsPanel", panelModel);
        }
        [HttpGet]
        [Authorize("Operations.ServiceRequests.Edit")]
        public async Task<IActionResult> CreateBLAmendmentRequestLine(Guid blAmendmentRequestId)
        {
            var model = await _service.GetCreateBLAmendmentRequestLineFormAsync(blAmendmentRequestId);
            return PartialView("_BLAmendmentRequestLineFormModal", model);
        }
        [HttpGet]
        [Authorize("Operations.ServiceRequests.Edit")]
        public async Task<IActionResult> EditBLAmendmentRequestLine(Guid id)
        {
            var model = await _service.GetEditBLAmendmentRequestLineFormAsync(id);

            if (model == null)
                return NotFound();

            return PartialView("_BLAmendmentRequestLineFormModal", model);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize("Operations.ServiceRequests.Edit")]
        public async Task<IActionResult> SaveBLAmendmentRequestLine(BLAmendmentRequestLineFormDto model)
        {
            if (!ModelState.IsValid)
            {
                model.FieldTypes = await _service.GetBLAmendmentFieldTypeSelectListAsync();
                return PartialView("_BLAmendmentRequestLineFormModal", model);
            }

            await _service.SaveBLAmendmentRequestLineAsync(model);

            var panelModel = await _service.GetBLAmendmentsPanelAsync(model.ServiceRequestId);
            return PartialView("_BLAmendmentsPanel", panelModel);
        }

        [HttpDelete]
        [Authorize("Operations.ServiceRequests.Edit")]
        public async Task<IActionResult> DeleteBLAmendmentRequestLine(Guid id, Guid serviceRequestId)
        {
            await _service.DeleteBLAmendmentRequestLineAsync(id);

            var panelModel = await _service.GetBLAmendmentsPanelAsync(serviceRequestId);
            return PartialView("_BLAmendmentsPanel", panelModel);
        }










        [HttpGet]
        [Authorize("Operations.ServiceRequests.Edit")]
        public async Task<IActionResult> CreateBLAmendmentFullRequest()
        {
            var model = await _service.GetCreateBLAmendmentFullRequestFormAsync();
            return PartialView("_BLAmendmentFullRequestFormModal", model);
        }



        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize("Operations.ServiceRequests.Edit")]
        public async Task<IActionResult> SaveBLAmendmentFullRequest(BLAmendmentFullRequestFormDto model)
        {
            if (!ModelState.IsValid)
            {
                model = await _service.RebuildBLAmendmentFullRequestFormAsync(model);
                Response.Headers["HX-Retarget"] = "#modalBody";
                Response.Headers["HX-Reswap"] = "innerHTML";

                return PartialView("_BLAmendmentFullRequestFormModal", model);
            }

            var result = await _service.SaveBLAmendmentFullRequestAsync(model);

            if (!result.Succeeded)
            {
                ModelState.AddModelError(string.Empty, result.Message);
                model = await _service.RebuildBLAmendmentFullRequestFormAsync(model);
                return PartialView("_BLAmendmentFullRequestFormModal", model);
            }
            Response.Headers["HX-Trigger"] = "serviceRequestChanged";
            return Content(string.Empty);
        }



        [HttpGet]
        [Authorize("Operations.ServiceRequests.Edit")]
        public async Task<IActionResult> AddBLAmendmentFullRequestLine(int index)
        {
            var model = await _service.GetBlankBLAmendmentLineAsync(index);
            return PartialView("_BLAmendmentFullRequest_LineItem", model);
        }

        [HttpGet]
        [Authorize("Operations.ServiceRequests.Edit")]
        public async Task<IActionResult> AdvancedSplitEditor(
            Guid splitRequestId)
        {
            var submodel = await _service.GetAdvancedSplitModelAsync(splitRequestId);
            if (submodel == null) { return NotFound(); }

            var model = await _transportDocumentService.GetAdvancedSplitWorkspaceAsync(submodel.BlId, submodel.JobId);
            model.ServiceRequestId = submodel.ServiceRequestId;
            model.SplitRequestId = splitRequestId;

            if (model == null) { return NotFound(); }
            return View("~/Views/TransportDocuments/Split.cshtml", model);

        }








        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize("Operations.ServiceRequests.Edit")]
        public async Task<IActionResult> SubmitAdvancedSplit([FromBody] SubmitAdvancedSplitVm model)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest("Invalid split payload.");
            }

            var result = await _service.SubmitAdvancedSplitAsync(model);

            if (!result.Success)
            {
                return BadRequest(result.Message);
            }

            return Json(new
            {
                success = true,
                message = result.Message,
                redirectUrl = Url.Action(nameof(SplitRequestsPanel), new
                {
                    serviceRequestId = result.ServiceRequestId
                })
            });
        }





    }
}