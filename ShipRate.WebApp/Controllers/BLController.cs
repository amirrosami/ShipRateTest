using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ShipRate.Application.DTOs.BillOfLadings;
using ShipRate.Application.Interfaces.BillOfLadings;

namespace ShipRate.WebApp.Controllers
{
    [Authorize]

    public class BLController : Controller
    {
        private readonly IBLService _blService;

        public BLController(IBLService blService)
        {
            _blService = blService;
        }

        [HttpGet]
        public async Task<IActionResult> Index(Guid? jobId, Guid? bookingId, Guid? shipmentId, Guid? shipmentLegId)
        {
            var context = new BLContextDto
            {
                JobId = jobId,
                BookingId = bookingId,
                ShipmentId = shipmentId,
                ShipmentLegId = shipmentLegId
            };

            //var model = await _blService.GetManagePageAsync(context);
            //return View(model);
            return View();
        }


        [HttpGet]
        public async Task<IActionResult> CreateModal(
            BLContextDto context,
            string? listTargetId,
            string? modalId)
        {
            var model = await _blService.GetCreateModelAsync(context);

            model.ListTargetId = listTargetId;
            model.ModalId = modalId;
            model.IsContainerized = true;

            ViewBag.Lookups = await _blService.GetLookupsAsync();

            return PartialView("_BLFormModal", model);
        }

        [HttpGet]
        public async Task<IActionResult> EditModal(
            Guid id,
            string? listTargetId,
            string? modalId)
        {
            var model = await _blService.GetEditModelAsync(id);

            model.ListTargetId = listTargetId;
            model.ModalId = modalId;

            ViewBag.Lookups = await _blService.GetLookupsAsync();

            return PartialView("_BLFormModal", model);
        }

        //[HttpPost]
        //[ValidateAntiForgeryToken]
        //public async Task<IActionResult> Save(BLUpsertDto dto)
        //{
        //    var result = await _blService.SaveAsync(dto);

        //    if (!result.Success)
        //    {
        //        ViewBag.Lookups = await _blService.GetLookupsAsync();
        //        ViewBag.ErrorMessage = result.Message;
        //        return PartialView("_BLFormModal", dto);
        //    }

        //    var model = await _blService.GetManagePageAsync(new BLContextDto
        //    {
        //        JobId = dto.JobId,
        //        BookingId = dto.BookingId,
        //        ShipmentId = dto.ShipmentId,
        //        ShipmentLegId = dto.ShipmentLegId
        //    });

        //    Response.Headers["HX-Trigger"] = "blSaved";
        //    return PartialView("_BLList", model);
        //}

    }
}
