using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ShipRate.Application.DTOs;
using ShipRate.Application.Interfaces.BaseInfo;
using System.Text.Json;

namespace ShipRate.WebApp.Controllers
{
    [Authorize]
    public class IndustryController : Controller
    {
        private readonly IIndustryService _service;

        public IndustryController(IIndustryService service)
        {
            _service = service;
        }

        private bool IsHtmx => Request.Headers.ContainsKey("HX-Request");
        private void HxTrigger(object triggers)
        {
            Response.Headers["HX-Trigger"] = JsonSerializer.Serialize(triggers);
        }

        [HttpGet]
        [Authorize("CRM.Industries.View")]
        public async Task<IActionResult> Index()
        {
            var result = await _service.GetPagedAsync(new PagedRequestDto());
            return View(result);
        }

        [HttpGet]
        [Authorize("CRM.Industries.View")]
        public async Task<IActionResult> List(PagedRequestDto request)
        {
            var result = await _service.GetPagedAsync(request);
            return PartialView("_List", result);
        }

        [HttpGet]
        [Authorize("CRM.Industries.Edit")]
        public IActionResult Create()
        {
            return PartialView("_FormCreate", new IndustryCreateDto { IsActive = true });
        }


        // POST: /Industrys/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize("CRM.Industries.Edit")]
        public async Task<IActionResult> Create(IndustryCreateDto dto)
        {
            if (!ModelState.IsValid)
            {
                Response.StatusCode = 422;
                return PartialView("_FormCreate", dto);
            }


            var (success, error) = await _service.CreateAsync(dto);

            if (!success)
            {
                ModelState.AddModelError("Code", error!);
                Response.StatusCode = 422;
                return PartialView("_FormCreate", dto);
            }

            HxTrigger(new
            {
                closeModal = new { },
                refreshIndustryList = new { },
                showToast = new { message = "Industry created successfully.", type = "success" }
            });

            //Response.Headers.Append("HX-Trigger", "closeModal, refreshList, showToast");
            return Content("");
        }

        // GET: /Industrys/EditForm/5
        //public async Task<IActionResult> EditForm(int id)
        //{
        //    var entity = await _service.GetByIdAsync(id);
        //    if (entity == null) return NotFound();

        //    var dto = new IndustryUpdateDto
        //    {
        //        Id = entity.Id,
        //        IndustryCode = entity.IndustryCode,
        //        IndustryName = entity.IndustryName,
        //        IndustryDescription = entity.IndustryDescription,
        //        IsActive = entity.IsActive
        //    };
        //    return PartialView("_FormEdit", dto);
        //}

        [Authorize("CRM.Industries.Edit")]
        public async Task<IActionResult> Edit(Guid id)
        {
            var entity = await _service.GetByIdAsync(id);
            if (entity == null) return NotFound();

            var dto = new IndustryUpdateDto
            {
                Id = entity.Id,
                IndustryName = entity.IndustryName,
                IsActive = entity.IsActive
            };
            return PartialView("_FormEdit", dto);
        }




        // POST: /Industrys/Edit
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize("CRM.Industries.Edit")]
        public async Task<IActionResult> Edit(IndustryUpdateDto dto)
        {
            if (!ModelState.IsValid)
            {
                Response.StatusCode = 422;
                return PartialView("_FormEdit", dto);
            }

            var (success, error) = await _service.UpdateAsync(dto);

            if (!success)
            {
                ModelState.AddModelError("Code", error!);
                Response.StatusCode = 422;
                return PartialView("_FormEdit", dto);
            }

            HxTrigger(new
            {
                closeModal = new { },
                refreshIndustryList = new { },
                showToast = new { message = "Industry updated successfully.", type = "success" }
            });
            return Content("");
        }

        // GET: /Industrys/DeleteConfirm/5

        [HttpGet]
        [Authorize("CRM.Industries.Edit")]
        public async Task<IActionResult> Delete(Guid id)
        {
            var item = await _service.GetByIdAsync(id);
            if (item == null) return NotFound();
            return PartialView("_DeleteConfirm", item);
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize("CRM.Industries.Edit")]
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
                refreshIndustryList = new { },
                showToast = new { message = "Industry deleted successfully.", type = "success" }
            });

            return Content("");
        }



        // POST: /Industrys/ToggleActive/5
        //[HttpPost]
        //public async Task<IActionResult> ToggleActive(int id)
        //{
        //    await _service.ToggleActiveAsync(id);
        //    Response.Headers.Append("HX-Trigger", "refreshList");
        //    return Content("");
        //}

        [HttpPost]
        [Authorize("CRM.Industries.Edit")]
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
                refreshIndustryList = new { },
                showToast = new { message = "Status toggled.", type = "info" }
            });

            return Content("");
        }
    }
}
