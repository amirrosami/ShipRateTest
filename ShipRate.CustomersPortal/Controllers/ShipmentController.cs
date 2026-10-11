using Microsoft.AspNetCore.Mvc;
using ShippingTrackingMvc.Services;

namespace ShippingTrackingMvc.Controllers;

public class ShipmentController : Controller
{
    private readonly IShipmentService _service;
    public ShipmentController(IShipmentService service) => _service = service;

    public IActionResult Index(string? trackingNumber)
    {
        var shipment = _service.Get(trackingNumber ?? "14050510001") ?? _service.GetAll()[0];
        ViewBag.Shipments = _service.GetAll();
        return View(shipment);
    }

    [HttpGet]
    public IActionResult Track(string trackingNumber)
    {
        var shipment = _service.Get(trackingNumber);
        if (shipment is null)
            return PartialView("_NotFound", trackingNumber);
        return PartialView("_ShipmentContent", shipment);
    }

    [HttpGet] public IActionResult Overview(string id) => PartialView("_Overview", _service.Get(id));
    [HttpGet] public IActionResult Containers(string id) => PartialView("_Containers", _service.Get(id));
    [HttpGet] public IActionResult Parties(string id) => PartialView("_Parties", _service.Get(id));
    [HttpGet] public IActionResult BillOfLading(string id) => PartialView("_BillOfLading", _service.Get(id));
    [HttpGet] public IActionResult Milestones(string id) => PartialView("_Milestones", _service.Get(id));

    [HttpGet]
    public IActionResult Select(string id)
    {
        var shipment = _service.Get(id);
        if (shipment is null) return NotFound();
        return PartialView("_ShipmentContent", shipment);
    }
}
