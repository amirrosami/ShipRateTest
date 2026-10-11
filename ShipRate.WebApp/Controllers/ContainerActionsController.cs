using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ShipRate.Application.Interfaces.ContainerActions;
using ShipRate.Application.ViewModels;

namespace ShipRate.WebApp.Controllers
{
    [Authorize]
    public class ContainerActionsController : Controller
    {
        private readonly IContainerActionService _containerActionService;

        public ContainerActionsController(IContainerActionService containerActionService)
        {
            _containerActionService = containerActionService;
        }

        [HttpGet]
        [Authorize("Containers.ContainerActions.View")]
        public async Task<IActionResult> Index(
            ContainerActionSearchFormVm searchForm,
            int pageNumber = 1,
            int pageSize = 10)
        {
            var vm = await _containerActionService.SearchActionsAsync(
                searchForm,
                pageNumber,
                pageSize);

            if (Request.Headers["HX-Request"] == "true")
            {
                return PartialView("_ContainerActionTable", vm);
            }

            return View(vm);
        }


        [HttpGet]
        [Authorize("Containers.ContainerActions.View")]
        public IActionResult Details(Guid id)
        {
            return Content($"Details for action: {id} will be implemented next.");
        }

        [HttpGet]
        [Authorize("Containers.ContainerActions.Edit")]
        public IActionResult Edit(Guid id)
        {
            return Content($"Edit for action: {id} will be implemented next.");
        }

        [HttpDelete]
        [Authorize("Containers.ContainerActions.Edit")]
        public IActionResult Delete(Guid id)
        {
            return Content($"Delete for action: {id} will be implemented next.");
        }




        [HttpGet]
        [Authorize("Containers.ContainerActions.ReleaseToShipper")]
        public async Task<IActionResult> ReleaseToShipper()
        {
            var vm = await _containerActionService.PrepareReleaseToShipperVmAsync();

            return PartialView("_ReleaseToShipper", vm);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize("Containers.ContainerActions.ReleaseToShipper")]
        public async Task<IActionResult> ReleaseToShipper(ReleaseToShipperVm model)
        {
            if (!ModelState.IsValid)
            {
                model = await _containerActionService.PrepareReleaseToShipperVmAsync(model);

                return PartialView("_ReleaseToShipper", model);
            }

            try
            {
                await _containerActionService.ReleaseToShipperAsync(model);
            }
            catch (InvalidOperationException ex)
            {
                ModelState.AddModelError(string.Empty, ex.Message);

                model = await _containerActionService.PrepareReleaseToShipperVmAsync(model);

                return PartialView("_ReleaseToShipper", model);
            }
            Response.Headers["HX-Trigger"] = "container-actions-refresh, close-modal";
            //Response.Headers["HX-Trigger"] = "{\"container-actions-refresh\": true, \"close-modal\": true}";

            return Content(string.Empty);
        }

        [HttpGet]
        [Authorize("Containers.ContainerActions.LoadOnBoard")]
        public async Task<IActionResult> LoadOnBoard()
        {
            var vm = await _containerActionService.PrepareLoadOnBoardVmAsync();

            return PartialView("_LoadOnBoardSimple", vm);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize("Containers.ContainerActions.LoadOnBoard")]
        public async Task<IActionResult> LoadOnBoard(LoadOnBoardVm model)
        {
            if (!ModelState.IsValid)
            {
                model = await _containerActionService.PrepareLoadOnBoardVmAsync(model);

                return PartialView("_LoadOnBoardSimple", model);
            }
            try
            {
                await _containerActionService.LoadOnBoardAsync(model);
            }
            catch (InvalidOperationException ex)
            {
                ModelState.AddModelError(string.Empty, ex.Message);

                model = await _containerActionService.PrepareLoadOnBoardVmAsync(model);

                return PartialView("_LoadOnBoardSimple", model);
            }
            Response.Headers["HX-Trigger"] = "container-actions-refresh, close-modal";
            //Response.Headers["HX-Trigger"] = "{\"container-actions-refresh\": true, \"close-modal\": true}";
            return Content(string.Empty);
        }










        [HttpGet]
        [Authorize("Containers.ContainerActions.Discharge")]
        public async Task<IActionResult> Discharge()
        {
            var vm = await _containerActionService.PrepareDischargeVmAsync();

            return PartialView("_DischargeSimple", vm);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize("Containers.ContainerActions.Discharge")]
        public async Task<IActionResult> Discharge(DischargeVm model)
        {
            if (!ModelState.IsValid)
            {
                model = await _containerActionService.PrepareDischargeVmAsync(model);

                return PartialView("_DischargeSimple", model);
            }
            try
            {
                await _containerActionService.DischargeAsync(model);
            }
            catch (InvalidOperationException ex)
            {
                ModelState.AddModelError(string.Empty, ex.Message);

                model = await _containerActionService.PrepareDischargeVmAsync(model);

                return PartialView("_DischargeSimple", model);
            }
            Response.Headers["HX-Trigger"] = "container-actions-refresh, close-modal";
            //Response.Headers["HX-Trigger"] = "{\"container-actions-refresh\": true, \"close-modal\": true}";
            return Content(string.Empty);
        }

        [HttpGet]
        [Authorize("Containers.ContainerActions.ReturnByConsignee")]
        public async Task<IActionResult> RBC()
        {
            var vm = await _containerActionService.PrepareRBCVmAsync();

            return PartialView("_RBCSimple", vm);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize("Containers.ContainerActions.ReturnByConsignee")]
        public async Task<IActionResult> RBC(RBCVm model)
        {
            if (!ModelState.IsValid)
            {
                model = await _containerActionService.PrepareRBCVmAsync(model);

                return PartialView("_RBCSimple", model);
            }
            try
            {
                await _containerActionService.RBCAsync(model);
            }
            catch (InvalidOperationException ex)
            {
                ModelState.AddModelError(string.Empty, ex.Message);

                model = await _containerActionService.PrepareRBCVmAsync(model);

                return PartialView("_RBCSimple", model);
            }
            Response.Headers["HX-Trigger"] = "container-actions-refresh, close-modal";
            //Response.Headers["HX-Trigger"] = "{\"container-actions-refresh\": true, \"close-modal\": true}";
            return Content(string.Empty);
        }
    }
}
