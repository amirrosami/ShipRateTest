using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using ShipRate.Application.DTOs;
using ShipRate.Application.DTOs.Finance;
using ShipRate.Application.Interfaces.BaseInfo;
using ShipRate.Application.Interfaces.Finance;
using ShipRate.Application.Services.External;
using System.Text.Json;

namespace ShipRate.WebApp.Controllers
{
    [Authorize]
    public class ExchangeRatesController : Controller
    {
        private readonly SaoiCurrencyFetchService _saoicurrencyFetchService;
        private readonly IExchangeRateService _service;
        private readonly IBaseDataService _baseDataService;

        public ExchangeRatesController(IExchangeRateService service, SaoiCurrencyFetchService saoicurrencyFetchService, IBaseDataService baseDataService)
        {
            _service = service;
            _saoicurrencyFetchService = saoicurrencyFetchService;
            _baseDataService = baseDataService;
        }

        private bool IsHtmx => Request.Headers.ContainsKey("HX-Request");
        private void HxTrigger(object triggers)
        {
            Response.Headers["HX-Trigger"] = JsonSerializer.Serialize(triggers);
        }

        [HttpGet]
        [Authorize("Finance.ExchangeRates.View")]
        public async Task<IActionResult> Index()
        {
            var result = await _service.GetPagedAsync(new PagedRequestDto());
            return View(result);
        }


        [HttpGet]
        [Authorize("Finance.ExchangeRates.View")]
        public async Task<IActionResult> List(PagedRequestDto request)
        {
            var result = await _service.GetPagedAsync(request);
            return PartialView("_List", result);
        }


        private async Task FillSelects()
        {

            var exchangeratebases = await _baseDataService.GetExchangeRateBasesAsync();
            ViewBag.ExchangeRateBases = new SelectList(exchangeratebases, "Id", "ExchangeRateBaseName");


            var currencies = await _baseDataService.GetCurrenciesAsync();
            ViewBag.Currencies = new SelectList(currencies, "Id", "CurrencyName");



        }

        [HttpGet]
        [Authorize("Finance.ExchangeRates.Edit")]
        public async Task<IActionResult> Create()
        {
            await FillSelects();
            return PartialView("_FormCreate", new ExchangeRateCreateDto
            {
                ExchangeRateDate = DateTime.Now,
                FromCurrencyId = 1,
                ToCurrencyId = 2,
                ExchangeRateBaseId = 20
            });
        }


        // POST: /PackageTypes/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize("Finance.ExchangeRates.Edit")]
        public async Task<IActionResult> Create(ExchangeRateCreateDto dto)
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
                await FillSelects();
                return PartialView("_FormCreate", dto);
            }


            var (success, error) = await _service.CreateAsync(dto);

            if (!success)
            {
                ModelState.AddModelError("Code", error!);
                Response.StatusCode = 422;
                await FillSelects();
                return PartialView("_FormCreate", dto);
            }

            HxTrigger(new
            {
                closeModal = new { },
                refreshExchangeRatesList = new { },
                showToast = new { message = "ExchangeRate created successfully.", type = "success" }
            });

            //Response.Headers.Append("HX-Trigger", "closeModal, refreshList, showToast");
            return Content("");
        }

        [Authorize("Finance.ExchangeRates.Edit")]
        public async Task<IActionResult> Edit(Guid id)
        {
            var entity = await _service.GetByIdAsync(id);
            if (entity == null) return NotFound();

            await FillSelects();

            var dto = new ExchangeRateUpdateDto
            {
                Id = entity.Id,
                ExchangeRateBaseId = entity.ExchangeRateBaseId,
                ExchangeRateDate = entity.ExchangeRateDate,
                ExchangeRateSource = entity.ExchangeRateSource,
                ExchangeRateValue = entity.ExchangeRateValue,
                FromCurrencyId = entity.FromCurrencyId,
                ToCurrencyId = entity.ToCurrencyId
            };
            return PartialView("_FormEdit", dto);
        }
        // POST: /PackageTypes/Edit
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize("Finance.ExchangeRates.Edit")]
        public async Task<IActionResult> Edit(ExchangeRateUpdateDto dto)
        {
            //if (!await _service.IsCodeUniqueAsync(dto.PackageTypeCode, dto.Id))
            //{
            //    ModelState.AddModelError("Code", "This code already exists.");
            //}

            if (!ModelState.IsValid)
            {
                Response.StatusCode = 422;
                await FillSelects();
                return PartialView("_FormEdit", dto);
            }

            var (success, error) = await _service.UpdateAsync(dto);

            if (!success)
            {
                ModelState.AddModelError("Code", error!);
                Response.StatusCode = 422;
                await FillSelects();
                return PartialView("_FormEdit", dto);
            }

            HxTrigger(new
            {
                closeModal = new { },
                refreshExchangeRatesList = new { },
                showToast = new { message = "Exchange Rate updated successfully.", type = "success" }
            });
            return Content("");
        }

        // GET: /PackageTypes/DeleteConfirm/5

        [HttpGet]
        [Authorize("Finance.ExchangeRates.Edit")]
        public async Task<IActionResult> Delete(Guid id)
        {
            var item = await _service.GetByIdAsync(id);
            if (item == null) return NotFound();
            return PartialView("_DeleteConfirm", item);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize("Finance.ExchangeRates.Edit")]
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
                refreshExchangeRatesList = new { },
                showToast = new { message = "ExchangeRate deleted successfully.", type = "success" }
            });

            return Content("");
        }





        [HttpPost]
        [Authorize("Finance.ExchangeRates.Edit")]
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
                refreshExchangeRatesList = new { },
                showToast = new { message = "Status toggled.", type = "info" }
            });

            return Content("");
        }

        [Authorize("Finance.ExchangeRates.Edit")]
        public async Task<IActionResult> GetSaoiData()
        {
            await _saoicurrencyFetchService.FetchAndSaveAsync();
            HxTrigger(new
            {
                closeModal = new { },
                refreshExchangeRatesList = new { },
                showToast = new { message = "Exchange Rates refreshed successfully.", type = "success" }
            });
            return Content("");
        }
    }
}
