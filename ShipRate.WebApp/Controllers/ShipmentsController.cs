using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ShipRate.Application.Interfaces.Jobs;
using ShipRate.Application.ViewModels;
using System.Security.Claims;

namespace ShipRate.WebApp.Controllers;

[Authorize]
public partial class ShipmentsController : Controller
{
    private readonly IShipmentService _shipmentService;
    public ShipmentsController(IShipmentService shipmentService)
    {
        this._shipmentService = shipmentService;
    }
    public IActionResult ShipmentLegPanel(Guid shipmentId)
    {
        var vm = new ShipmentLegFilterVm
        {
            ShipmentId = shipmentId
        };

        return PartialView("~/Views/Shipments/ShipmentLegs/_ShipmentLegPanel.cshtml", vm);
    }

    public async Task<IActionResult> ShipmentLegList(ShipmentLegFilterVm filter)
    {
        var result = await _shipmentService.GetShipmentLegListAsync(filter);
        ViewBag.ShipmentId = filter.ShipmentId;
        return PartialView("~/Views/Shipments/ShipmentLegs/_ShipmentLegList.cshtml", result);
    }

    [HttpGet]
    public async Task<IActionResult> CreateShipmentLeg(Guid shipmentId)
    {
        var vm = await _shipmentService.BuildShipmentLegCreateVmAsync(shipmentId);
        return PartialView("~/Views/Shipments/ShipmentLegs/_CreateShipmentLeg.cshtml", vm);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> CreateShipmentLeg(ShipmentLegFormVm vm)
    {
        if (!ModelState.IsValid)
        {
            await _shipmentService.FillShipmentLegLookupsAsync(vm);
            return PartialView("~/Views/Shipments/ShipmentLegs/_CreateShipmentLeg.cshtml", vm);
        }

        var result = await _shipmentService.CreateShipmentLegAsync(vm, GetCurrentUserId());

        if (!result.Success)
        {
            ModelState.AddModelError(string.Empty, result.Error!);
            await _shipmentService.FillShipmentLegLookupsAsync(vm);
            return PartialView("~/Views/Shipments/ShipmentLegs/_CreateShipmentLeg.cshtml", vm);
        }

        Response.Headers["HX-Trigger"] = "shipmentLegChanged";
        return Content(string.Empty);
    }

    [HttpGet]
    public async Task<IActionResult> EditShipmentLeg(Guid id)
    {
        var vm = await _shipmentService.BuildShipmentLegEditVmAsync(id);
        if (vm == null)
            return NotFound();

        return PartialView("~/Views/Shipments/ShipmentLegs/_EditShipmentLeg.cshtml", vm);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> EditShipmentLeg(ShipmentLegFormVm vm)
    {
        if (!ModelState.IsValid)
        {
            await _shipmentService.FillShipmentLegLookupsAsync(vm);
            return PartialView("~/Views/Shipments/ShipmentLegs/_EditShipmentLeg.cshtml", vm);
        }

        var result = await _shipmentService.UpdateShipmentLegAsync(vm, GetCurrentUserId());

        if (!result.Success)
        {
            ModelState.AddModelError(string.Empty, result.Error!);
            await _shipmentService.FillShipmentLegLookupsAsync(vm);
            return PartialView("~/Views/Shipments/ShipmentLegs/_EditShipmentLeg.cshtml", vm);
        }

        Response.Headers["HX-Trigger"] = "shipmentLegChanged";

        return Content(string.Empty);
    }

    [HttpGet]
    public async Task<IActionResult> ShipmentLegDetails(Guid id)
    {
        var vm = await _shipmentService.GetShipmentLegDetailsAsync(id);
        if (vm == null)
            return NotFound();

        return PartialView("~/Views/Shipments/ShipmentLegs/_ShipmentLegDetails.cshtml", vm);
    }

    [HttpGet]
    public async Task<IActionResult> DeleteShipmentLeg(Guid id)
    {
        var vm = await _shipmentService.GetShipmentLegDeleteVmAsync(id);
        if (vm == null)
            return NotFound();

        return PartialView("~/Views/Shipments/ShipmentLegs/_DeleteShipmentLeg.cshtml", vm);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteShipmentLegConfirmed(Guid id)
    {
        var result = await _shipmentService.DeleteShipmentLegAsync(id, GetCurrentUserId());

        if (!result.Success)
            return BadRequest(result.Error);

        Response.Headers["HX-Trigger"] = "shipmentLegChanged";
        return Content(string.Empty);
    }

    private Guid GetCurrentUserId()
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        return Guid.TryParse(userId, out var id) ? id : Guid.Empty;
    }
}
