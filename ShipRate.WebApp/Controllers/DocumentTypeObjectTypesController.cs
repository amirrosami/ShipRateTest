using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using ShipRate.Application.DTOs.Documents;
using ShipRate.Application.Interfaces.Documents;

namespace ShipRate.WebApp.Controllers
{
    [Authorize]
    public class DocumentTypeObjectTypesController : Controller
    {
        private readonly IDocumentTypeObjectTypeService _service;
        public DocumentTypeObjectTypesController(IDocumentTypeObjectTypeService service)
        {
            _service = service;
        }

        // صفحه اصلی مدیریت و گرید بالا
        public async Task<IActionResult> Index(string? searchTerm, int pageNumber = 1, int pageSize = 5)
        {
            var model = await _service.GetPagedSystemObjectsAsync(searchTerm, pageNumber, pageSize);
            ViewData["SearchTerm"] = searchTerm;

            if (Request.Headers.ContainsKey("HX-Request"))
            {
                return PartialView("_SystemObjectsListPartial", model);
            }

            return View(model);
        }

        // لود کردن گرید پایینی بر اساس آیدی انتخاب شده
        [HttpGet]
        public async Task<IActionResult> LoadDetailList(int objectTypeId)
        {
            var systemObject = await _service.GetSystemObjectByIdAsync(objectTypeId);
            if (systemObject == null) return NotFound();

            var details = await _service.GetByObjectTypeIdAsync(objectTypeId);
            ViewData["SelectedObjectName"] = systemObject.SystemObjectTypeName;
            ViewData["SelectedObjectId"] = objectTypeId;

            return PartialView("_DetailListPartial", details);
        }

        // فرم ایجاد
        [HttpGet]
        public async Task<IActionResult> Create(int objectTypeId)
        {
            var systemObject = await _service.GetSystemObjectByIdAsync(objectTypeId);
            if (systemObject == null) return NotFound();

            await PopulateLookupsAsync(objectTypeId);
            var model = new DocumentTypeObjectTypeInputDto { ObjectTypeId = objectTypeId };

            ViewData["SelectedObjectName"] = systemObject.SystemObjectTypeName;
            return PartialView("_CreateModalPartial", model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(DocumentTypeObjectTypeInputDto input)
        {
            if (ModelState.IsValid)
            {
                await _service.CreateAsync(input);
                Response.Headers.Add("HX-Trigger", $"detailListChanged-{input.ObjectTypeId}");
                return Content(string.Empty);
            }

            await PopulateLookupsAsync(input.ObjectTypeId);
            return PartialView("_CreateModalPartial", input);
        }

        // فرم ویرایش
        [HttpGet]
        public async Task<IActionResult> Edit(int id)
        {
            var model = await _service.GetByIdAsync(id);
            if (model == null) return NotFound();
            if (model.SystemDefined) return BadRequest("System defined records cannot be edited.");

            var systemObject = await _service.GetSystemObjectByIdAsync(model.ObjectTypeId);
            ViewData["SelectedObjectName"] = systemObject?.SystemObjectTypeName;

            await PopulateLookupsAsync(model.ObjectTypeId);
            return PartialView("_EditModalPartial", model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(DocumentTypeObjectTypeInputDto input)
        {
            if (ModelState.IsValid)
            {
                await _service.UpdateAsync(input);
                Response.Headers.Add("HX-Trigger", $"detailListChanged-{input.ObjectTypeId}");
                return Content(string.Empty);
            }

            await PopulateLookupsAsync(input.ObjectTypeId);
            return PartialView("_EditModalPartial", input);
        }

        // حذف نقشه
        [HttpGet]
        public async Task<IActionResult> Delete(int id)
        {
            var model = await _service.GetByIdAsync(id);
            if (model == null) return NotFound();
            if (model.SystemDefined) return BadRequest("System defined records cannot be deleted.");

            return PartialView("_DeleteModalPartial", model);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id, int objectTypeId)
        {
            await _service.DeleteAsync(id);
            Response.Headers.Add("HX-Trigger", $"detailListChanged-{objectTypeId}");
            return Content(string.Empty);
        }

        private async Task PopulateLookupsAsync(int objectTypeId)
        {
            var lookup = await _service.GetDocumentTypeLookupAsync(objectTypeId);
            ViewBag.DocumentTypes = new SelectList(lookup, "Key", "Value");
        }
    }
}
