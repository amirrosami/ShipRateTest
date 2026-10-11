using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ShipRate.Application.DTOs.EDI;
using ShipRate.Application.Interfaces;

namespace ShipRate.WebApp.Controllers
{
    [Authorize]
    public class EDIController : Controller
    {
        private readonly IEDIService _ediService;

        public EDIController(IEDIService ediService)
        {
            _ediService = ediService;
        }
        [Authorize("Utilities.EDIMessages.View")]
        public async Task<IActionResult> Index()
        {
            var vm = new EDIPageViewModel
            {
                MessageTypes = await _ediService.GetMessageTypesAsync()
            };

            return View(vm);
        }

        [HttpGet]
        [Authorize("Utilities.EDIMessages.View")]
        public async Task<IActionResult> MessageList(int messageTypeId, string? searchTerm, int pageNumber = 1, int pageSize = 10)
        {
            var input = new EDIMessageSearchInput
            {
                MessageTypeId = messageTypeId,
                SearchTerm = searchTerm,
                PageNumber = pageNumber < 1 ? 1 : pageNumber,
                PageSize = pageSize <= 0 ? 10 : pageSize
            };

            var vm = await _ediService.GetMessagesAsync(input);
            return PartialView("_MessageList", vm);
        }

        [HttpGet]
        [Authorize("Utilities.EDIMessages.Edit")]
        public async Task<IActionResult> CreateModal(int messageTypeId)
        {
            var vm = await _ediService.GetActionModalInfoAsync(messageTypeId, "Create");
            if (vm == null) return NotFound();

            return PartialView("_ActionModalBody", vm);
        }

        [HttpGet]
        [Authorize("Utilities.EDIMessages.Edit")]
        public async Task<IActionResult> GetModal(int messageTypeId)
        {
            var vm = await _ediService.GetActionModalInfoAsync(messageTypeId, "Get");
            if (vm == null) return NotFound();

            return PartialView("_ActionModalBody", vm);
        }

        [HttpGet]
        [Authorize("Utilities.EDIMessages.View")]
        public async Task<IActionResult> DownloadPayload(Guid id)
        {
            var file = await _ediService.DownloadPayloadAsync(id);
            if (file == null) return NotFound();

            return File(file.Value.Content, file.Value.ContentType, file.Value.FileName);
        }
    }
}
