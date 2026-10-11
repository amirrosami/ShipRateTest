using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using ShipRate.Application.DTOs;
using ShipRate.Application.DTOs.Base_Info;
using ShipRate.Application.Interfaces.BaseInfo;
using ShipRate.Application.Services.SMS;
using ShipRate.Infrastructure.Data;
using ShipRate.WebApp.ViewModels;
using System.Text.Json;

namespace ShipRate.WebApp.Controllers
{
    [Authorize]
    public class BanksController : Controller
    {
        private readonly IBankService _service;
        private readonly IBaseDataService _basedataservice;
        private readonly ApplicationDbContext _context;
        private readonly ISmsQueueService _smsQueue;

        public BanksController(IBankService service, IBaseDataService basedataservice, ApplicationDbContext context, ISmsQueueService smsQueue)
        {
            _service = service;
            _basedataservice = basedataservice;
            _context = context;
            _smsQueue = smsQueue;
        }
        private async Task<PagedResult<BankDto>> getFilteredList(PagedRequestDto dto)
        {

            //await _smsQueue.EnqueueSingleAsync(
            //        mobile: "989028402502",
            //        body: $"بارنامه شماره 1 با موفقیت صادر گردید.ساعی ترابر آرشام",
            //        tag: "BL_ISSUED"
            //    );

            var data = _context.Banks
                .Where(x => !x.IsDeleted)
                .Include(x => x.Country)
                .AsQueryable();

            if (!string.IsNullOrEmpty(dto.SearchTerm))
            {
                data = data.Where(x => x.BankCode.Contains(dto.SearchTerm) || x.Country.CountryName.Contains(dto.SearchTerm) || x.BankName.Contains(dto.SearchTerm) || x.BankLocalName.Contains(dto.SearchTerm));
            }

            int total = await data.CountAsync();

            data = data.Skip((dto.Page - 1) * dto.PageSize).Take(dto.PageSize);

            var items = await data.Select(x => new BankDto
            {
                Id = x.Id,
                BankLocalName = x.BankLocalName,
                BankName = x.BankName,
                BankCode = x.BankCode,
                CountryName = x.Country.CountryName
            }).ToListAsync();

            return new PagedResult<BankDto>()
            {
                Items = items,
                Page = dto.Page,
                PageSize = dto.PageSize,
                SearchTerm = dto.SearchTerm,
                CustomFilterValues = new { },
                SortColumn = dto.SortColumn,
                SortDirection = dto.SortDirection,
                TotalCount = total,

            };

        }


        private bool IsHtmx => Request.Headers.ContainsKey("HX-Request");
        private void HxTrigger(object triggers)
        {
            Response.Headers.Append("HX-Trigger", JsonSerializer.Serialize(triggers));
        }

        [HttpGet]
        public async Task<IActionResult> Index()
        {
            var result = await getFilteredList(new PagedRequestDto());
            return View(result);
        }


        [HttpPost]
        public async Task<IActionResult> List(PagedRequestDto request)
        {
            var result = await getFilteredList(request);
            var gridVm = new GridViewModel
            {
                Data = result.toObject(),
                ListUrl = "Banks/List",
                GridPartialName = "_List",
                columns = GetGridColumns()

            };
            return PartialView("_Grid", gridVm);
        }

        private List<GridColumn> GetGridColumns()
        {
            return new List<GridColumn> {
                new GridColumn { DisplayName = "Bank Code", Name = "BankCode" }, new GridColumn { DisplayName = "Bank Name", Name = "BankName" } ,new GridColumn("BankLocalName","Bank Local Name") ,new GridColumn("CountryName","Country")};
        }




        [HttpGet]
        public async Task<IActionResult> Create()
        {
            var countries = await _basedataservice.GetCountriesAsync();
            ViewBag.Countries = new SelectList(countries, "Id", "CountryName");
            ViewBag.CreateEdit = "Create";
            return PartialView("_CreateEdit", new BankCreateEditDto { });
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(BankCreateEditDto dto)
        {
            ModelState.Clear();



            var (success, error) = await _service.CreateAsync(dto);



            if (!success)
            {
                var countries = await _basedataservice.GetCountriesAsync();
                ViewBag.Countries = new SelectList(countries, "Id", "CountryName");
                ModelState.AddModelError("Code", error!);
                Response.StatusCode = 422;
                return PartialView("_CreateEdit", dto);
            }


            HxTrigger(new
            {
                GridChanged = true,
                toastSuccess = "Created Sussefully"
            });
            return Content("");
        }

        public async Task<IActionResult> Edit(Guid id)
        {
            var entity = await _service.GetByIdAsync(id);
            if (entity == null) return NotFound();

            var countries = await _basedataservice.GetCountriesAsync();
            ViewBag.Countries = new SelectList(countries, "Id", "CountryName");

            var dto = new BankCreateEditDto
            {
                Id = entity.Id,
                CountryId = entity.CountryId,
                BankCode = entity.BankCode,
                BankName = entity.BankName,
                BankLocalName = entity.BankLocalName,
                Remarks = entity.Remarks,
                SWIFTCode = entity.SWIFTCode
            };
            ViewBag.CreateEdit = "Edit";
            return PartialView("_CreateEdit", dto);
        }
        // POST: /PackageTypes/Edit
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(BankCreateEditDto dto)
        {


            //if (!ModelState.IsValid)
            //{
            //    var countries = await _basedataservice.GetCountriesAsync();
            //    ViewBag.Countries = new SelectList(countries, "Id", "CountryName");
            //    Response.StatusCode = 422;
            //    return PartialView("_CreateEdit", dto);
            //}

            var (success, error) = await _service.UpdateAsync(dto);

            if (!success)
            {
                var countries = await _basedataservice.GetCountriesAsync();
                ViewBag.Countries = new SelectList(countries, "Id", "CountryName");
                ModelState.AddModelError("Code", error!);
                Response.StatusCode = 422;
                return PartialView("_CreateEdit", dto);
            }
            HxTrigger(new
            {
                GridChanged = true,
                toastSuccess = "Edited Sussefully"
            });
            return Content("");
        }

        // GET: /PackageTypes/DeleteConfirm/5

        [HttpGet]
        public async Task<IActionResult> Delete(Guid id)
        {
            //var item = await _service.GetByIdAsync(id);
            //if (item == null) return NotFound();
            return PartialView("_DeleteConfirm", id);
        }



        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(Guid id)
        {
            var (success, error) = await _service.DeleteAsync(id);

            if (!success)
            {
                HxTrigger(new
                {
                    gridChanged = false,
                    toastError = error
                });
                return Content("");
            }

            HxTrigger(new
            {
                GridChanged = true,
                toastSuccess = "Deleted Sussefully"
            });

            return Content("");
        }
    }
}
