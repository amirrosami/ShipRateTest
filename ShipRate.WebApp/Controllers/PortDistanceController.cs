using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using ShipRate.Application.DTOs;
using ShipRate.Application.Interfaces.BaseInfo;
using ShipRate.Infrastructure.Data;
using ShipRate.WebApp.ViewModels;
using System.Text.Json;
using static ShipRate.Application.DTOs.PortDistanceDto;

namespace ShipRate.WebApp.Controllers
{
    [Authorize]
    public class PortDistanceController : Controller
    {
        private readonly IPortDistanceService _service;
        private readonly IBaseDataService _basedataservice;
        private readonly ApplicationDbContext _db;


        public PortDistanceController(IPortDistanceService service, IBaseDataService basedataservice, ApplicationDbContext db)
        {
            _service = service;
            _basedataservice = basedataservice;
            _db = db;
        }

        private bool IsHtmx => Request.Headers.ContainsKey("HX-Request");
        private void HxTrigger(object triggers)
        {
            Response.Headers["HX-Trigger"] = JsonSerializer.Serialize(triggers);
        }

        [Authorize("BaseOperations.Distances.View")]
        [HttpGet]
        public async Task<IActionResult> Index()
        {
            var result = await _service.GetPagedAsync(new PagedRequestDto());
            return View(result);
        }


        [HttpGet]
        [Authorize("BaseOperations.Distances.View")]
        public async Task<IActionResult> List(PagedRequestDto request)
        {
            var result = await _service.GetPagedAsync(request);
            return PartialView("_List", result);
        }



        [HttpGet]
        [Authorize("BaseOperations.Distances.Edit")]
        public async Task<IActionResult> Create()
        {
            var transportTypes = await _basedataservice.GetTransportTypesAsync();
            ViewBag.TransPortTypes = new SelectList(transportTypes, "Id", "TransportTypeName");
            return PartialView("_FormCreate", new PortDistanceCreateDto { });
        }


        // POST: /PackageTypes/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize("BaseOperations.Distances.Edit")]
        public async Task<IActionResult> Create(PortDistanceCreateDto dto)
        {
            // Check unique code
            //if (!await _service.IsCodeUniqueAsync(dto.PackageTypeCode))
            //{
            //    ModelState.AddModelError("Code", "This code already exists.");
            //}

            //if (!ModelState.IsValid)
            //{
            //    Response.Headers.Append("HX-Retarget", "#modal-body");
            //    Response.Headers.Append("HX-Reswap", "innerHTML");
            //    return PartialView("_FormCreate", dto);
            //}

            if (!ModelState.IsValid)
            {
                Response.StatusCode = 422;
                var transportTypes = await _basedataservice.GetTransportTypesAsync();
                ViewBag.TransPortTypes = new SelectList(transportTypes, "Id", "TransportTypeName");
                return PartialView("_FormCreate", dto);
            }


            var (success, error) = await _service.CreateAsync(dto);

            if (!success)
            {
                ModelState.AddModelError("Code", error!);
                Response.StatusCode = 422;
                var transportTypes = await _basedataservice.GetTransportTypesAsync();
                ViewBag.TransPortTypes = new SelectList(transportTypes, "Id", "TransportTypeName");
                return PartialView("_FormCreate", dto);
            }

            HxTrigger(new
            {
                closeModal = new { },
                refreshPortDistanceList = new { },
                showToast = new { message = "Distance created successfully.", type = "success" }
            });

            //Response.Headers.Append("HX-Trigger", "closeModal, refreshList, showToast");
            return Content("");
        }

        [Authorize("BaseOperations.Distances.Edit")]
        public async Task<IActionResult> Edit(Guid id)
        {
            var entity = await _service.GetByIdAsync(id);
            if (entity == null) return NotFound();

            var transportTypes = await _basedataservice.GetTransportTypesAsync();
            ViewBag.TransPortTypes = new SelectList(transportTypes, "Id", "TransportTypeName");

            var dto = new PortDistanceUpdateDto
            {
                AvgHours = entity.AvgHours,
                AvgDays = entity.AvgDays,
                DistanceKM = entity.DistanceKM,
                DistanceNM = entity.DistanceNM,
                FromPortId = entity.FromPortId,
                Id = entity.Id,
                Source = entity.Source,
                ToPortId = entity.ToPortId,
                TransPortTypeId = entity.TransPortTypeId,
                FromPortName = entity.FromPortName,
                ToPortName = entity.ToPortName
            };

            return PartialView("_FormEdit", dto);
        }
        // POST: /PackageTypes/Edit
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize("BaseOperations.Distances.Edit")]
        public async Task<IActionResult> Edit(PortDistanceUpdateDto dto)
        {
            //if (!await _service.IsCodeUniqueAsync(dto.PackageTypeCode, dto.Id))
            //{
            //    ModelState.AddModelError("Code", "This code already exists.");
            //}

            if (!ModelState.IsValid)
            {
                Response.StatusCode = 422;
                var transportTypes = await _basedataservice.GetTransportTypesAsync();
                ViewBag.TransPortTypes = new SelectList(transportTypes, "Id", "TransportTypeName");
                return PartialView("_FormEdit", dto);
            }

            var (success, error) = await _service.UpdateAsync(dto);

            if (!success)
            {
                ModelState.AddModelError("Code", error!);
                var transportTypes = await _basedataservice.GetTransportTypesAsync();
                ViewBag.TransPortTypes = new SelectList(transportTypes, "Id", "TransportTypeName");
                Response.StatusCode = 422;
                return PartialView("_FormEdit", dto);
            }

            HxTrigger(new
            {
                closeModal = new { },
                refreshPortDistanceList = new { },
                showToast = new { message = "Distance updated successfully.", type = "success" }
            });
            return Content("");
        }

        // GET: /PackageTypes/DeleteConfirm/5

        [HttpGet]
        [Authorize("BaseOperations.Distances.Edit")]
        public async Task<IActionResult> Delete(Guid id)
        {
            var item = await _service.GetByIdAsync(id);
            if (item == null) return NotFound();
            return PartialView("_DeleteConfirm", item);
        }

        // POST: /PackageTypes/Delete/5
        //[HttpPost]
        //public async Task<IActionResult> Delete(int id)
        //{
        //    await _service.SoftDeleteAsync(id);
        //    Response.Headers.Append("HX-Trigger", "closeModal, refreshList, showToast");
        //    return Content("");
        //}

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize("BaseOperations.Distances.Edit")]
        public async Task<IActionResult> DeleteConfirmed(Guid id)
        {
            var (success, error) = await _service.DeleteAsync(id);

            if (!success)
            {
                HxTrigger(new
                {
                    closeModal = new { },
                    showToast = new { message = error, type = "danger" }
                });
                return Content("");
            }

            HxTrigger(new
            {
                closeModal = new { },
                refreshPortDistanceList = new { },
                showToast = new { message = "Distance deleted successfully.", type = "success" }
            });

            return Content("");
        }

        [HttpPost]
        [Authorize("BaseOperations.Distances.Edit")]
        public async Task<IActionResult> ToggleActive(Guid id)
        {
            var (success, error) = await _service.ToggleActiveAsync(id);

            if (!success)
            {
                HxTrigger(new
                {
                    showToast = new { message = error, type = "danger" }
                });
                return Content("");
            }

            HxTrigger(new
            {
                refreshPortDistanceList = new { },
                showToast = new { message = "Status toggled.", type = "info" }
            });

            return Content("");
        }


        [HttpGet]
        public IActionResult SearchFromLocation(string q)
        {
            var locations = _db.Locations
                .Where(x => x.LocationName.Contains(q) || x.LocationLocalName.Contains(q) || x.LocationCode.Contains(q))
                .Take(10)
                .Select(x => new LocatonViewModel { LocationId = x.Id, LocationName = x.LocationName })
                .ToList();
            return PartialView("_FromLocationResults", locations);
        }
        [HttpGet]
        public IActionResult SearchToLocation(string q)
        {
            var locations = _db.Locations
                .Where(x => x.LocationName.Contains(q) || x.LocationLocalName.Contains(q) || x.LocationCode.Contains(q))
                .Take(10)
                .Select(x => new LocatonViewModel { LocationId = x.Id, LocationName = x.LocationName })
                .ToList();
            return PartialView("_ToLocationResults", locations);
        }

    }
}
