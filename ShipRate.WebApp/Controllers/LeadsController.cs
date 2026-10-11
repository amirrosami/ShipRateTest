using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ShipRate.Application.DTOs;
using ShipRate.Application.Interfaces;

namespace ShipRate.WebApp.Controllers
{
    [Authorize]
    public class LeadsController : Controller
    {
        private readonly ILeadService _leadService;
        private readonly ICurrentUserService _currentUserService;

        public LeadsController(ILeadService leadService, ICurrentUserService currentUserService)
        {
            _leadService = leadService;
            _currentUserService = currentUserService;
        }

        [Authorize("Marketing.Leads.View")]
        public IActionResult Index()
        {
            return View();
        }
        [Authorize("Marketing.Leads.View")]
        public async Task<IActionResult> SearchPanel()
        {
            ViewBag.Statuses = await _leadService.GetLeadStatusesAsync();
            ViewBag.Sources = await _leadService.GetLeadSourcesAsync();
            ViewBag.Users = await _leadService.GetUsersAsync();
            ViewBag.Industries = await _leadService.GetIndustriesAsync();
            return PartialView("_SearchPanel");
        }

        [Authorize("Marketing.Leads.View")]
        public async Task<IActionResult> Grid([FromQuery] LeadSearchDto filter)
        {
            var result = await _leadService.GetLeadsAsync(filter);
            return PartialView("_LeadGrid", result);
        }

        [Authorize("Marketing.Leads.Edit")]
        public async Task<IActionResult> CreateForm()
        {
            ViewBag.Statuses = await _leadService.GetLeadStatusesAsync();
            ViewBag.Sources = await _leadService.GetLeadSourcesAsync();
            ViewBag.Industries = await _leadService.GetIndustriesAsync();
            ViewBag.Currencies = await _leadService.GetCurrenciesAsync();
            ViewBag.TradeTypes = await _leadService.GetTradeTypesAsync();
            ViewBag.Users = await _leadService.GetUsersAsync();

            return PartialView("_LeadForm", new LeadDto());
        }

        [Authorize("Marketing.Leads.Edit")]
        public async Task<IActionResult> EditForm(Guid id)
        {
            var lead = await _leadService.GetLeadAsync(id);
            if (lead == null) return NotFound();

            ViewBag.Statuses = await _leadService.GetLeadStatusesAsync();
            ViewBag.Sources = await _leadService.GetLeadSourcesAsync();
            ViewBag.Industries = await _leadService.GetIndustriesAsync();
            ViewBag.Currencies = await _leadService.GetCurrenciesAsync();
            ViewBag.TradeTypes = await _leadService.GetTradeTypesAsync();
            ViewBag.Users = await _leadService.GetUsersAsync();

            return PartialView("_LeadForm", lead);
        }

        [HttpPost]
        [Authorize("Marketing.Leads.Edit")]
        public async Task<IActionResult> Save([FromForm] LeadDto dto)
        {
            if (!ModelState.IsValid)
            {
                ViewBag.Statuses = await _leadService.GetLeadStatusesAsync();
                ViewBag.Sources = await _leadService.GetLeadSourcesAsync();
                ViewBag.Industries = await _leadService.GetIndustriesAsync();
                ViewBag.Currencies = await _leadService.GetCurrenciesAsync();
                ViewBag.TradeTypes = await _leadService.GetTradeTypesAsync();
                ViewBag.Users = await _leadService.GetUsersAsync();

                return PartialView("_LeadForm", dto);
            }

            var userId = _currentUserService.UserId;

            if (dto.Id is null || dto.Id == Guid.Empty)
                await _leadService.CreateLeadAsync(dto, userId);
            else
                await _leadService.UpdateLeadAsync(dto, userId);

            Response.Headers.Add("HX-Trigger", "leadSaved");
            return Ok();
        }
        [HttpDelete]
        public async Task<IActionResult> Delete(Guid id)
        {
            await _leadService.DeleteLeadAsync(id);
            Response.Headers.Add("HX-Trigger", "leadDeleted");
            return Ok();
        }

        [Authorize("Marketing.Leads.Assign")]
        public async Task<IActionResult> AssignModal(Guid id)
        {
            var model = await _leadService.GetAssignModelAsync(id);
            return PartialView("_AssignUserModal", model);
        }

        [HttpPost]
        [Authorize("Marketing.Leads.Assign")]
        public async Task<IActionResult> Assign([FromForm] Guid leadId, [FromForm] Guid userId)
        {
            var performedBy = Guid.Parse(User.FindFirst("UserId")?.Value ?? Guid.Empty.ToString());
            await _leadService.AssignLeadAsync(leadId, userId, performedBy);

            Response.Headers.Add("HX-Trigger", "leadAssigned");
            return Content("");
        }
        [Authorize("Marketing.Leads.View")]
        public async Task<IActionResult> ActivitiesGrid(Guid leadId)
        {
            var activities = await _leadService.GetActivitiesAsync(leadId);
            ViewBag.LeadId = leadId;
            return PartialView("_LeadActivityGrid", activities);
        }

        [Authorize("Marketing.Leads.Edit")]
        public async Task<IActionResult> ActivityCreateForm(Guid leadId)
        {
            ViewBag.ActivityTypes = await _leadService.GetActivityTypesAsync();
            ViewBag.ActivityResults = await _leadService.GetActivityResultsAsync();
            ViewBag.ActivityOutcomes = await _leadService.GetActivityOutcomesAsync();

            var model = new LeadActivityDto
            {
                LeadId = leadId,
                ActivityDate = DateTime.Now
            };

            return PartialView("_LeadActivityForm", model);
        }

        [Authorize("Marketing.Leads.Edit")]
        public async Task<IActionResult> ActivityEditForm(Guid id)
        {
            var activity = await _leadService.GetActivityAsync(id);
            if (activity == null) return NotFound();

            ViewBag.ActivityTypes = await _leadService.GetActivityTypesAsync();
            ViewBag.ActivityResults = await _leadService.GetActivityResultsAsync();
            ViewBag.ActivityOutcomes = await _leadService.GetActivityOutcomesAsync();

            return PartialView("_LeadActivityForm", activity);
        }
        [HttpPost]
        [Authorize("Marketing.Leads.Edit")]
        public async Task<IActionResult> ActivitySave([FromForm] LeadActivityDto dto)
        {
            if (!ModelState.IsValid)
            {
                ViewBag.ActivityTypes = await _leadService.GetActivityTypesAsync();
                ViewBag.ActivityResults = await _leadService.GetActivityResultsAsync();
                ViewBag.ActivityOutcomes = await _leadService.GetActivityOutcomesAsync();

                return PartialView("_LeadActivityForm", dto);
            }
            var userId = _currentUserService.UserId;
            if (dto.Id == Guid.Empty)
                await _leadService.CreateActivityAsync(dto, userId);
            else
                await _leadService.UpdateActivityAsync(dto, userId);
            Response.Headers.Add("HX-Trigger", "activitySaved");
            return Ok();
        }

        [HttpDelete]
        [Authorize("Marketing.Leads.Edit")]
        public async Task<IActionResult> ActivityDelete(Guid id)
        {
            await _leadService.DeleteActivityAsync(id);
            Response.Headers.Add("HX-Trigger", "activityDeleted");
            return Ok();
        }
    }
}