using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ShipRate.Application.DTOs;
using ShipRate.Application.Interfaces;
using ShipRate.Application.Interfaces.BaseInfo;

namespace ShipRate.WebApp.Controllers
{
    [Authorize]
    public class BlackListController : Controller
    {
        private readonly IBlackListService _blackListService;
        private readonly IBaseDataService _baseDataService;
        private readonly ICurrentUserService _currentUserService;

        public BlackListController(IBlackListService blackListService, ICurrentUserService currentUserService, IBaseDataService baseDataService)
        {
            _blackListService = blackListService;
            _currentUserService = currentUserService;
            _baseDataService = baseDataService;
        }


        private Guid CurrentUserId => _currentUserService.UserId;

        [Authorize("CRM.BlackLists.View")]
        public async Task<IActionResult> Index(BlackListSearchDto searchDto)
        {

            if (!Request.Headers.ContainsKey("HX-Request") && searchDto.IsActiveInBlackList == null)
            {
                searchDto.IsActiveInBlackList = true;
            }


            var pagedData = await _blackListService.GetPagedListAsync(searchDto);

            ViewBag.Reasons = await _blackListService.GetReasonsSelectListAsync();
            ViewBag.SearchModel = searchDto;

            // اگر درخواست HTMX بود، فقط بدنه جدول برگشت داده می‌شود
            if (Request.Headers.ContainsKey("HX-Request"))
            {
                return PartialView("_TableRows", pagedData);
            }

            return View(pagedData);
        }

        [Authorize("CRM.BlackLists.Edit")]
        [HttpGet]
        public async Task<IActionResult> Create()
        {
            //ViewBag.People = new SelectList(await _blackListService.GetPeopleSelectListAsync(), "Id", "PersonName");
            //ViewBag.Reasons = new SelectList(await _blackListService.GetReasonsSelectListAsync(), "Id", "BlackListReasonName");

            ViewBag.People = await _blackListService.GetPeopleSelectListAsync();
            ViewBag.Reasons = await _blackListService.GetReasonsSelectListAsync();

            return PartialView("_CreateModal");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize("CRM.BlackLists.Edit")]
        public async Task<IActionResult> Create(CreateBlackListDto dto)
        {
            if (ModelState.IsValid)
            {
                await _blackListService.CreateAsync(dto, CurrentUserId);
                // تریگر به HTMX برای رفرش کردن جدول صفحه اصلی پس از اتمام موفقیت‌آمیز عملیات
                Response.Headers.Add("HX-Trigger", "refreshBlackListTable");
                return Content(string.Empty); // بستن مودال
            }

            //ViewBag.People = new SelectList(await _blackListService.GetPeopleSelectListAsync(), "Id", "PersonName", dto.PersonId);
            //ViewBag.Reasons = new SelectList(await _blackListService.GetReasonsSelectListAsync(), "Id", "BlackListReasonName", dto.BlackListReasonId);

            ViewBag.People = await _blackListService.GetPeopleSelectListAsync();
            ViewBag.Reasons = await _blackListService.GetReasonsSelectListAsync();

            return PartialView("_CreateModal", dto);
        }

        // دکمه بازکردن مودال خروج از لیست سیاه
        [HttpGet]
        [Authorize("CRM.BlackLists.Exit")]
        public async Task<IActionResult> Exit(Guid id)
        {
            var item = await _blackListService.GetByIdAsync(id);
            if (item == null || item.IsExited)
            {
                return NotFound();
            }

            var dto = new ExitBlackListDto { Id = id };
            ViewBag.PersonFullName = item.PersonName;

            return PartialView("_ExitModal", dto);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize("CRM.BlackLists.Exit")]
        public async Task<IActionResult> Exit(ExitBlackListDto dto)
        {
            if (ModelState.IsValid)
            {
                await _blackListService.ExitAsync(dto, CurrentUserId);
                Response.Headers.Add("HX-Trigger", "refreshBlackListTable");
                return Content(string.Empty);
            }

            ModelState.AddModelError("", "Failed to exit record.");
            return PartialView("_ExitModal", dto);
        }

        [HttpDelete]
        [Authorize("CRM.BlackLists.Edit")]
        public async Task<IActionResult> Delete(Guid id)
        {
            await _blackListService.DeleteAsync(id);
            Response.Headers.Add("HX-Trigger", "refreshBlackListTable");
            return Ok();
        }
    }
}
