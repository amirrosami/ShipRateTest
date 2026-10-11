using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ShipRate.Application.Interfaces.Finance;

namespace ShipRate.WebApp.Controllers
{
    [Authorize]
    public class LookupController : Controller
    {
        private readonly ISettlementService _settlementService;
        public LookupController(ISettlementService settlementService)
        {
            _settlementService = settlementService;
        }
        [HttpGet]
        public async Task<IActionResult> Parties(string PartyNameSearch, string targetId, string targetText)
        {
            var result = await _settlementService.SearchPartiesAsync(PartyNameSearch);

            ViewBag.TargetId = targetId;
            ViewBag.TargetText = targetText;

            return PartialView("~/Views/Settlements/_LookupResults.cshtml", result);
        }
        [HttpGet]
        public async Task<IActionResult> Invoices(Guid? partyId, string invoicesearchtext, string targetId, string targetText)
        {
            var result = await _settlementService.SearchInvoicesAsync(partyId, invoicesearchtext);

            ViewBag.TargetId = targetId;
            ViewBag.TargetText = targetText;

            return PartialView("~/Views/Settlements/_LookupResults.cshtml", result);
        }
        [HttpGet]
        public async Task<IActionResult> PaymentDocuments(Guid? partyId, string pdocsearchtext, string targetId, string targetText)
        {
            var result = await _settlementService.SearchPaymentDocumentsAsync(partyId, pdocsearchtext);

            ViewBag.TargetId = targetId;
            ViewBag.TargetText = targetText;

            return PartialView("~/Views/Settlements/_LookupResults.cshtml", result);
        }

        [HttpGet]
        public async Task<IActionResult> Charges(Guid? partyId, string chargesearchtext, string targetId, string targetText)
        {
            var result = await _settlementService.SearchChargesAsync(partyId, chargesearchtext);

            ViewBag.TargetId = targetId;
            ViewBag.TargetText = targetText;

            return PartialView("~/Views/Settlements/_LookupResults.cshtml", result);
        }
    }
}
