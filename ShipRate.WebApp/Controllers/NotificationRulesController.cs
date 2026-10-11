using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ShipRate.Application.DTOs.Notif;
using ShipRate.Application.Interfaces.Notif;

namespace ShipRate.WebApp.Controllers
{
    [Authorize]

    public class NotificationRulesController : Controller
    {
        private readonly INotificationRuleService _notificationRuleService;

        public NotificationRulesController(INotificationRuleService notificationRuleService)
        {
            _notificationRuleService = notificationRuleService;
        }

        public async Task<IActionResult> Index(NotificationRuleGetListInput input)
        {
            var result = await _notificationRuleService.GetListAsync(input);
            ViewBag.Filter = input;
            return View(result);
        }

        [HttpGet]
        public async Task<IActionResult> Create()
        {
            var model = await _notificationRuleService.GetCreateDtoAsync();
            model.SpecificUsersModeId = 3;
            model.UserTypesModeId = 4;



            return PartialView("_Create", model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(NotificationRuleCreateDto input)
        {
            if (!ModelState.IsValid)
            {
                var model = await _notificationRuleService.GetCreateDtoAsync();
                input.EntityTypes = model.EntityTypes;
                input.RecipientModes = model.RecipientModes;
                input.Users = model.Users;
                input.UserTypes = model.UserTypes;
                return PartialView("_Create", input);
            }

            try
            {
                await _notificationRuleService.CreateAsync(input);
                return Json(new { success = true });
            }
            catch (Exception ex)
            {
                ModelState.AddModelError(string.Empty, ex.Message);
                var model = await _notificationRuleService.GetCreateDtoAsync();
                input.EntityTypes = model.EntityTypes;
                input.RecipientModes = model.RecipientModes;
                input.Users = model.Users;
                input.UserTypes = model.UserTypes;
                return PartialView("_Create", input);
            }
        }

        [HttpGet]
        public async Task<IActionResult> Edit(Guid id)
        {
            var model = await _notificationRuleService.GetEditDtoAsync(id);
            if (model == null)
                return NotFound();

            model.SpecificUsersModeId = 3;
            model.UserTypesModeId = 4;


            return PartialView("_Edit", model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(NotificationRuleEditDto input)
        {
            if (!ModelState.IsValid)
            {
                var model = await _notificationRuleService.GetEditDtoAsync(input.Id);
                if (model == null)
                    return NotFound();

                input.EntityTypes = model.EntityTypes;
                input.RecipientModes = model.RecipientModes;
                input.Users = model.Users;
                input.UserTypes = model.UserTypes;

                return PartialView("_Edit", input);
            }

            try
            {
                await _notificationRuleService.UpdateAsync(input);
                return Json(new { success = true });
            }
            catch (Exception ex)
            {
                ModelState.AddModelError(string.Empty, ex.Message);

                var model = await _notificationRuleService.GetEditDtoAsync(input.Id);
                if (model == null)
                    return NotFound();

                input.EntityTypes = model.EntityTypes;
                input.RecipientModes = model.RecipientModes;
                input.Users = model.Users;
                input.UserTypes = model.UserTypes;

                return PartialView("_Edit", input);
            }
        }

        [HttpGet]
        public async Task<IActionResult> Details(Guid id)
        {
            var model = await _notificationRuleService.GetDetailsAsync(id);
            if (model == null)
                return NotFound();

            return PartialView("_Details", model);
        }

        [HttpGet]
        public async Task<IActionResult> Delete(Guid id)
        {
            var model = await _notificationRuleService.GetDeleteDtoAsync(id);
            if (model == null)
                return NotFound();

            return PartialView("_Delete", model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(Guid id)
        {
            try
            {
                await _notificationRuleService.DeleteAsync(id);
                return Json(new { success = true });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message });
            }
        }
    }
}
