using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using ShipRate.Application.DTOs.RouteDTOs;
using ShipRate.Application.Interfaces;
using ShipRate.Infrastructure.Data;
using ShipRate.WebApp.ViewModels.RouteVms;

namespace ShipRate.WebApp.Controllers
{
    [Authorize]
    public class RoutesController : Controller
    {
        private readonly IRouteService _service;
        private readonly ApplicationDbContext _db;

        public RoutesController(IRouteService service, ApplicationDbContext db)
        {
            _service = service;
            _db = db;
        }
        [Authorize("BaseOperations.Routes.View")]
        public async Task<IActionResult> Index()
        {
            var vm = new RouteIndexViewModel
            {
                //Locations = (await _db.Locations.ToListAsync()).Select(x => new SelectListItem
                //{
                //    Value = x.Id.ToString(),
                //    Text = x.LocationName
                //}).ToList(),
                RouteModes = (await _db.RouteModes.ToListAsync()).Select(x => new SelectListItem
                {
                    Value = x.Id.ToString(),
                    Text = x.RouteModeName
                }).ToList()
            };

            return View(vm);
        }

        [Authorize("BaseOperations.Routes.View")]
        public async Task<IActionResult> List(RouteSearchDto search)
        {
            var result = await _service.SearchAsync(search);
            return PartialView("_List", result);
        }

        [Authorize("BaseOperations.Routes.Edit")]
        [HttpGet]
        public async Task<IActionResult> Create()
        {
            var vm = new RouteFormViewModel
            {
                //Locations = (await _db.Locations.ToListAsync()).Select(x => new SelectListItem
                //{
                //    Value = x.Id.ToString(),
                //    Text = x.LocationName
                //}).ToList(),
                RouteModes = (await _db.RouteModes.ToListAsync()).Select(x => new SelectListItem
                {
                    Value = x.Id.ToString(),
                    Text = x.RouteModeName
                }).ToList()
            };

            return PartialView("_Form", vm);
        }

        [Authorize("BaseOperations.Routes.Edit")]
        [HttpGet]
        public async Task<IActionResult> Edit(Guid id)
        {
            var route = await _service.GetAsync(id);

            var vm = new RouteFormViewModel
            {
                RouteId = route.RouteId,
                RouteName = route.RouteName,
                OriginId = route.OriginId,
                DestinationId = route.DestinationId,
                RouteModeId = route.RouteModeId,
                DistanceMiles = route.DistanceMiles,
                DistanceKms = route.DistanceKms,
                TransitTime = route.TransitTime,
                OriginName = route.OriginName,
                DestinationName = route.DestinationName,
                IsActive = route.IsActive,
                RouteModes = (await _db.RouteModes.ToListAsync()).Select(x => new SelectListItem
                {
                    Value = x.Id.ToString(),
                    Text = x.RouteModeName
                }).ToList()
            };
            return PartialView("_Form", vm);
        }

        [Authorize("BaseOperations.Routes.Edit")]
        [HttpPost]
        public async Task<IActionResult> Save(RouteFormViewModel vm)
        {
            if (vm.RouteId is null)
            {
                await _service.CreateAsync(new CreateRouteDto
                {
                    RouteName = vm.RouteName,
                    OriginId = vm.OriginId,
                    DestinationId = vm.DestinationId,
                    RouteModeId = vm.RouteModeId,
                    DistanceMiles = vm.DistanceMiles,
                    DistanceKms = vm.DistanceKms,
                    TransitTime = vm.TransitTime,
                    IsActive = vm.IsActive
                });
            }
            else
            {
                await _service.UpdateAsync(new UpdateRouteDto
                {
                    RouteId = vm.RouteId.Value,
                    RouteName = vm.RouteName,
                    OriginId = vm.OriginId,
                    DestinationId = vm.DestinationId,
                    RouteModeId = vm.RouteModeId,
                    DistanceMiles = vm.DistanceMiles,
                    DistanceKms = vm.DistanceKms,
                    TransitTime = vm.TransitTime,
                    IsActive = vm.IsActive
                });
            }

            Response.Headers.Add("HX-Trigger", "routeChanged");

            return Ok();
        }

        [Authorize("BaseOperations.Routes.Edit")]
        public async Task<IActionResult> Delete(Guid id)
        {
            var route = await _service.GetAsync(id);
            return PartialView("_DeleteConfirmation", route);
        }

        [Authorize("BaseOperations.Routes.Edit")]
        [HttpPost]
        public async Task<IActionResult> DeleteConfirmed(Guid id)
        {
            await _service.DeleteAsync(id);

            Response.Headers.Add("HX-Trigger", "routeChanged");

            return Ok();
        }
    }
}
