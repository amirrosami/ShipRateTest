using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ShipRate.Application.DTOs;
using ShipRate.Application.Interfaces;
using System.Security.Claims;

namespace ShipRate.WebApp.Controllers
{
    [Authorize]
    public class GuaranteePlansController : Controller
    {
        private readonly IGuaranteePlanService _service;

        public GuaranteePlansController(IGuaranteePlanService service)
        {
            _service = service;
        }

        [Authorize("ImportSpecific.GuaranteePlans.View")]
        public async Task<IActionResult> Index(GuaranteePlanSearchDto searchDto)
        {
            var model = await _service.GetPagedListAsync(searchDto);
            if (Request.Headers["HX-Request"] == "true")
            {
                return PartialView("_PlanList", model);
            }
            return View(model);
        }

        // GET: GuaranteePlans/GetModal
        // هندل کردن ایجاد و ویرایش در قالب یک متد با پاس دادن BL متصل یا باز کردن فرم خالی
        [Authorize("ImportSpecific.GuaranteePlans.Edit")]
        public async Task<IActionResult> GetModal(Guid? id, Guid? blId, string blNo)
        {
            var model = new GuaranteePlanCreateOrUpdateDto();

            if (id.HasValue)
            {
                var planDto = await _service.GetByIdAsync(id.Value);
                if (planDto != null)
                {
                    model.Id = planDto.Id;
                    model.CompanyId = planDto.CompanyId;
                    model.LetterNo = planDto.LetterNo;
                    model.LetterDate = planDto.LetterDate;
                    model.InOutId = planDto.InOutId;
                    model.ShippingLineId = planDto.ShippingLineId;
                    model.CustomerId = planDto.CustomerId;
                    model.BLId = planDto.BLId;
                    model.BLNo = planDto.BLNo;
                    model.Remarks = planDto.Remarks;
                }
            }
            else if (blId.HasValue && !string.IsNullOrEmpty(blNo))
            {
                model.BLId = blId.Value;
                model.BLNo = blNo;
            }

            ViewBag.Companies = await _service.GetCompanyLookupAsync();
            ViewBag.InOuts = await _service.GetInOutLookupAsync();
            ViewBag.ShippingLines = await _service.GetShippingLineLookupAsync();
            ViewBag.Customers = await _service.GetCustomerLookupAsync();
            ViewBag.IsBlFixed = blId.HasValue; // مشخص کردن برای غیرفعال کردن انتخابگر بارنامه

            return PartialView("_CreateOrUpdateModal", model);
        }

        // POST: GuaranteePlans/Save
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize("ImportSpecific.GuaranteePlans.Edit")]
        public async Task<IActionResult> Save(GuaranteePlanCreateOrUpdateDto dto)
        {
            if (!ModelState.IsValid)
            {
                ViewBag.Companies = await _service.GetCompanyLookupAsync();
                ViewBag.InOuts = await _service.GetInOutLookupAsync();
                ViewBag.ShippingLines = await _service.GetShippingLineLookupAsync();
                ViewBag.Customers = await _service.GetCustomerLookupAsync();
                return PartialView("_CreateOrUpdateModal", dto);
            }

            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (dto.Id.HasValue && dto.Id.Value != Guid.Empty)
            {
                await _service.UpdateAsync(dto.Id.Value, dto, userId);
            }
            else
            {
                await _service.CreateAsync(dto, userId);
            }

            // پس از ذخیره موفق، لیست گرید بروزرسانی شود و سیگنال بستن مدال صادر گردد
            Response.Headers.Add("HX-Trigger", "refreshList");
            return Content("<script>bootstrap.Modal.getInstance(document.getElementById('guaranteeModal')).hide();</script>");
        }

        // POST: GuaranteePlans/Delete/5
        [HttpDelete]
        [ValidateAntiForgeryToken]
        [Authorize("ImportSpecific.GuaranteePlans.Edit")]
        public async Task<IActionResult> Delete(Guid id)
        {
            await _service.DeleteAsync(id);
            return TriggerRefreshList();
        }

        // GET: GuaranteePlans/SearchBL
        [HttpGet]
        [Authorize("ImportSpecific.GuaranteePlans.View")]
        public async Task<IActionResult> SearchBL(string term)
        {
            var results = await _service.SearchBlsAsync(term);
            return Json(results.Select(r => new { id = r.BLId, text = r.BLNo }));
        }
        private IActionResult TriggerRefreshList()
        {
            Response.Headers.Add("HX-Trigger", "refreshList");
            return Content("");
        }
    }
}
