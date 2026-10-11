using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using ShipRate.Application.DTOs.Documents;
using ShipRate.Application.Interfaces.Documents;

namespace ShipRate.WebApp.Controllers
{
    [Authorize]
    public class DocumentTypesController : Controller
    {
        private readonly IDocumentTypeService _docTypeService;

        public DocumentTypesController(IDocumentTypeService docTypeService)
        {
            _docTypeService = docTypeService;
        }

        // صفحه اصلی مدیریت DocumentTypes
        public async Task<IActionResult> Index(string? searchTerm, int pageNumber = 1, int pageSize = 10)
        {
            var model = await _docTypeService.GetPagedListAsync(searchTerm, pageNumber, pageSize);
            ViewData["SearchTerm"] = searchTerm;

            // اگر درخواست از نوع HTMX بود، فقط بدنه جدول را بروزرسانی کن
            if (Request.Headers.ContainsKey("HX-Request"))
            {
                return PartialView("_ListPartial", model);
            }

            return View(model);
        }

        // دریافت فرم Create درون Modal
        [HttpGet]
        public async Task<IActionResult> Create()
        {
            await PopulateCategoriesAsync();
            var model = new DocumentTypeInputDto { AlarmDaysBeforeExpire = 0 };
            return PartialView("_CreateModalPartial", model);
        }

        // ذخیره فرم Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(DocumentTypeInputDto input)
        {
            if (ModelState.IsValid)
            {
                await _docTypeService.CreateAsync(input);
                // با موفقیت آمیز بودن ثبت، تریگر بسته شدن مودال و بروزرسانی جدول فرستاده می‌شود
                Response.Headers.Add("HX-Trigger", "listChanged");
                return Content(string.Empty); // بستن مودال
            }

            await PopulateCategoriesAsync();
            return PartialView("_CreateModalPartial", input);
        }

        // دریافت فرم Edit درون Modal
        [HttpGet]
        public async Task<IActionResult> Edit(int id)
        {
            var model = await _docTypeService.GetByIdAsync(id);
            if (model == null) return NotFound();
            if (model.SystemDefined) return BadRequest("System defined documents cannot be modified.");

            await PopulateCategoriesAsync();
            return PartialView("_EditModalPartial", model);
        }

        // ذخیره فرم Edit
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(DocumentTypeInputDto input)
        {
            if (ModelState.IsValid)
            {
                await _docTypeService.UpdateAsync(input);
                Response.Headers.Add("HX-Trigger", "listChanged");
                return Content(string.Empty);
            }

            await PopulateCategoriesAsync();
            return PartialView("_EditModalPartial", input);
        }

        // تاییدیه حذف درون Modal
        [HttpGet]
        public async Task<IActionResult> Delete(int id)
        {
            var model = await _docTypeService.GetByIdAsync(id);
            if (model == null) return NotFound();
            if (model.SystemDefined) return BadRequest("System defined documents cannot be deleted.");

            return PartialView("_DeleteModalPartial", model);
        }

        // اجرای حذف نهایی
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            await _docTypeService.DeleteAsync(id);
            Response.Headers.Add("HX-Trigger", "listChanged");
            return Content(string.Empty);
        }

        private async Task PopulateCategoriesAsync()
        {
            var categories = await _docTypeService.GetCategoryLookupAsync();
            ViewBag.Categories = new SelectList(categories, "Key", "Value");
        }
    }
}
