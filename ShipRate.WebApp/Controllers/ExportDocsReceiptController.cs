using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ShipRate.Application.Interfaces;
using ShipRate.Application.ViewModels;
using System.Text.Json;

namespace ShipRate.WebApp.Controllers
{
    [Authorize]
    public class ExportDocsReceiptController : Controller
    {
        private readonly IExportDocsReceiptService _service;
        public ExportDocsReceiptController(IExportDocsReceiptService service)
        {
            _service = service;
        }
        [Authorize("Operations.ExportDocsReceipts.View")]
        public IActionResult Index()
        {
            return View();
        }
        private void HxTrigger(object triggers)
        {
            Response.Headers["HX-Trigger"] = JsonSerializer.Serialize(triggers);
        }


        [HttpGet]
        [Authorize("Operations.ExportDocsReceipts.View")]
        public async Task<IActionResult> List(ExportDocsReceiptIndexVm input)
        {
            var model = await _service.GetPagedAsync(input);
            return PartialView("_List", model);
        }

        [HttpGet]
        [Authorize("Operations.ExportDocsReceipts.View")]
        public async Task<IActionResult> ListByBooking(Guid bookingId)
        {
            var search = new ExportDocsReceiptIndexVm { BookingId = bookingId };
            var model = await _service.GetPagedAsync(search);
            return PartialView("_List", model);
        }




        [HttpGet]
        [Authorize("Operations.ExportDocsReceipts.Edit")]
        public async Task<IActionResult> Create(Guid? bookingId)
        {
            var model = await _service.GetCreateModelAsync(bookingId);
            return PartialView("_Form", model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize("Operations.ExportDocsReceipts.Edit")]
        public async Task<IActionResult> Create(ExportDocsReceiptFormVm input)
        {
            if (!ModelState.IsValid)
            {
                var model = await _service.GetCreateModelAsync(input.BookingId);
                model.CompanyId = input.CompanyId;
                model.ReceiptDirectionId = input.ReceiptDirectionId;
                model.PartyId = input.PartyId;
                model.BookingId = input.BookingId;
                model.ReceiptNo = input.ReceiptNo;
                model.ReceiptDate = input.ReceiptDate;
                model.PersonPersonName = input.PersonPersonName;
                model.Remarks = input.Remarks;
                return PartialView("_Form", model);
            }

            var id = await _service.CreateAsync(input);
            var editModel = await _service.GetEditModelAsync(id);
            Response.Headers["HX-Trigger"] = "receiptlistchanged";
            return PartialView("_Form", editModel);
        }

        [HttpGet]
        [Authorize("Operations.ExportDocsReceipts.Edit")]
        public async Task<IActionResult> Edit(Guid id)
        {
            var model = await _service.GetEditModelAsync(id);
            if (model == null)
                return NotFound();

            return PartialView("_Form", model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize("Operations.ExportDocsReceipts.Edit")]
        public async Task<IActionResult> Edit(ExportDocsReceiptFormVm input)
        {
            if (!ModelState.IsValid || !input.Id.HasValue)
            {
                var model = input.Id.HasValue
                    ? await _service.GetEditModelAsync(input.Id.Value)
                    : await _service.GetCreateModelAsync(input.BookingId);

                if (model != null)
                {
                    model.CompanyId = input.CompanyId;
                    model.ReceiptDirectionId = input.ReceiptDirectionId;
                    model.PartyId = input.PartyId;
                    model.BookingId = input.BookingId;
                    model.ReceiptNo = input.ReceiptNo;
                    model.ReceiptDate = input.ReceiptDate;
                    model.PersonPersonName = input.PersonPersonName;
                    model.Remarks = input.Remarks;
                }

                return PartialView("_Form", model);
            }

            await _service.UpdateAsync(input);
            var editModel = await _service.GetEditModelAsync(input.Id.Value);
            Response.Headers["HX-Trigger"] = "receiptlistchanged";
            return PartialView("_Form", editModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize("Operations.ExportDocsReceipts.Edit")]
        public async Task<IActionResult> Delete(Guid id)
        {
            await _service.DeleteAsync(id);
            Response.Headers["HX-Trigger"] = "receiptlistchanged";
            return Content(string.Empty);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize("Operations.ExportDocsReceipts.Edit")]
        public async Task<IActionResult> Confirm(Guid id)
        {
            bool b1 = await _service.ConfirmAsync(id);
            if (b1)
            {
                HxTrigger(
                    new
                    {
                        receiptlistchanged = true,
                        toastSuccess = "Receipt Confirmed Successfuly!!!"
                    }
                    );
            }
            else
            {
                HxTrigger(
                    new
                    {
                        toastSuccess = "Receipt Can not be confirmed!!!"
                    }
                    );
            }
            //Response.Headers["HX-Trigger"] = "receiptlistchanged";
            return Content(string.Empty);
        }





        [HttpGet]
        [Authorize("Operations.ExportDocsReceipts.Edit")]
        public async Task<IActionResult> AddDocument(Guid exportDocsReceiptId)
        {
            var model = await _service.GetAddDocumentModelAsync(exportDocsReceiptId);
            return PartialView("_AddDocumentForm", model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize("Operations.ExportDocsReceipts.Edit")]
        public async Task<IActionResult> AddDocument(AddExportDocsReceiptDocumentVm input)
        {
            if (!ModelState.IsValid)
            {
                var model = await _service.GetAddDocumentModelAsync(input.ExportDocsReceiptId);
                model.DocumentId = input.DocumentId;
                return PartialView("_AddDocumentForm", model);
            }

            await _service.AddDocumentAsync(input);
            var editModel = await _service.GetEditModelAsync(input.ExportDocsReceiptId);
            Response.Headers["HX-Trigger"] = "receiptlistchanged";
            return PartialView("_DocumentsTab", editModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize("Operations.ExportDocsReceipts.Edit")]
        public async Task<IActionResult> RemoveDocument(Guid exportDocsReceiptId, Guid exportDocsReceiptItemId)
        {
            await _service.RemoveDocumentAsync(exportDocsReceiptItemId);
            var editModel = await _service.GetEditModelAsync(exportDocsReceiptId);
            Response.Headers["HX-Trigger"] = "receiptlistchanged";
            return PartialView("_DocumentsTab", editModel);
        }

        [HttpGet]
        [Authorize("Operations.ExportDocsReceipts.Edit")]
        public async Task<IActionResult> AddContainer(Guid exportDocsReceiptId)
        {
            var model = await _service.GetAddContainerModelAsync(exportDocsReceiptId);
            return PartialView("_AddContainerForm", model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize("Operations.ExportDocsReceipts.Edit")]
        public async Task<IActionResult> AddContainer(AddExportDocsReceiptContainerVm input)
        {
            if (!ModelState.IsValid)
            {
                var model = await _service.GetAddContainerModelAsync(input.ExportDocsReceiptId);
                model.ContainerId = input.ContainerId;
                model.ContainerUseId = input.ContainerUseId;
                model.RBSDate = input.RBSDate;
                return PartialView("_AddContainerForm", model);
            }

            await _service.AddContainerAsync(input);
            var editModel = await _service.GetEditModelAsync(input.ExportDocsReceiptId);
            Response.Headers["HX-Trigger"] = "receiptlistchanged";
            return PartialView("_ContainersTab", editModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize("Operations.ExportDocsReceipts.Edit")]
        public async Task<IActionResult> RemoveContainer(Guid exportDocsReceiptId, Guid exportDocsReceiptContainerId)
        {
            await _service.RemoveContainerAsync(exportDocsReceiptContainerId);
            var editModel = await _service.GetEditModelAsync(exportDocsReceiptId);
            Response.Headers["HX-Trigger"] = "receiptlistchanged";
            return PartialView("_ContainersTab", editModel);
        }

        [HttpGet]
        public IActionResult EmptyDocumentsForm()
        {
            return Content(string.Empty);
        }

        [HttpGet]
        public IActionResult EmptyContainersForm()
        {
            return Content(string.Empty);
        }
    }
}
