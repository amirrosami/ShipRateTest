using ShipRate.Application.DTOs;
using ShipRate.Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ShipRate.Web.Controllers
{
    [Authorize]
    public class CityController : Controller
    {
        private readonly ICityService _cityService;
        private readonly ICountryService _countryService;
        private readonly ILogger<CityController> _logger;

        public CityController(
            ICityService cityService,
            ICountryService countryService,
            ILogger<CityController> logger)
        {
            _cityService = cityService;
            _countryService = countryService;
            _logger = logger;
        }

        // ========== View Action ==========
        public IActionResult Index()
        {
            return View();
        }

        // ========== API Endpoints (JSON-First) ==========

        /// <summary>
        /// دریافت لیست صفحه‌بندی‌شده شهرها
        /// </summary>
        [HttpGet]
        public async Task<IActionResult> GetPaged(
            int pageNumber = 1,
            int pageSize = 10,
            int? cityId = null,
            string? searchTerm = null,
            int? countryId = null)
        {
            try
            {
                // جستجوی دقیق با cityId
                if (cityId.HasValue)
                {
                    var city = await _cityService.GetCityByIdAsync(cityId.Value);

                    if (city == null)
                    {
                        return Json(new
                        {
                            success = true,
                            data = new PagedResultDto<CityDto>
                            {
                                Items = new List<CityDto>(),
                                TotalCount = 0,
                                PageNumber = pageNumber,
                                PageSize = pageSize
                            }
                        });
                    }

                    return Json(new
                    {
                        success = true,
                        data = new PagedResultDto<CityDto>
                        {
                            Items = new List<CityDto> { city },
                            TotalCount = 1,
                            PageNumber = 1,
                            PageSize = 1
                        }
                    });
                }

                // جستجوی عادی
                var result = await _cityService.GetPagedCitiesAsync(
                    pageNumber,
                    pageSize,
                    searchTerm,
                    countryId);

                return Json(new
                {
                    success = true,
                    data = result
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "خطا در دریافت لیست شهرها");
                return Json(new
                {
                    success = false,
                    message = "خطا در دریافت اطلاعات شهرها"
                });
            }
        }

        /// <summary>
        /// دریافت اطلاعات یک شهر با ID
        /// </summary>
        [HttpGet]
        public async Task<IActionResult> GetById(int id)
        {
            try
            {
                var city = await _cityService.GetCityByIdAsync(id);

                if (city == null)
                {
                    return Json(new
                    {
                        success = false,
                        message = "شهر مورد نظر یافت نشد"
                    });
                }

                return Json(new
                {
                    success = true,
                    data = city
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "خطا در دریافت اطلاعات شهر {CityId}", id);
                return Json(new
                {
                    success = false,
                    message = "خطا در دریافت اطلاعات شهر"
                });
            }
        }

        /// <summary>
        /// ایجاد شهر جدید
        /// </summary>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([FromBody] CityDto cityDto)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    var errors = ModelState
                        .Where(x => x.Value?.Errors.Count > 0)
                        .ToDictionary(
                            kvp => kvp.Key,
                            kvp => kvp.Value?.Errors.Select(e => e.ErrorMessage).ToArray() ?? Array.Empty<string>()
                        );

                    return Json(new
                    {
                        success = false,
                        message = "اطلاعات ورودی نامعتبر است",
                        errors
                    });
                }

                var createdCity = await _cityService.CreateCityAsync(cityDto);

                return Json(new
                {
                    success = true,
                    message = "شهر با موفقیت ایجاد شد",
                    data = createdCity
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "خطا در ایجاد شهر جدید");
                return Json(new
                {
                    success = false,
                    message = "خطا در ایجاد شهر"
                });
            }
        }

        /// <summary>
        /// بروزرسانی شهر
        /// </summary>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Update([FromBody] CityDto cityDto)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    var errors = ModelState
                        .Where(x => x.Value?.Errors.Count > 0)
                        .ToDictionary(
                            kvp => kvp.Key,
                            kvp => kvp.Value?.Errors.Select(e => e.ErrorMessage).ToArray() ?? Array.Empty<string>()
                        );

                    return Json(new
                    {
                        success = false,
                        message = "اطلاعات ورودی نامعتبر است",
                        errors
                    });
                }

                var result = await _cityService.UpdateCityAsync(cityDto);

                if (!result)
                {
                    return Json(new
                    {
                        success = false,
                        message = "شهر مورد نظر یافت نشد"
                    });
                }

                return Json(new
                {
                    success = true,
                    message = "شهر با موفقیت بروزرسانی شد"
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "خطا در بروزرسانی شهر {CityId}", cityDto.Id);
                return Json(new
                {
                    success = false,
                    message = "خطا در بروزرسانی شهر"
                });
            }
        }

        /// <summary>
        /// حذف شهر
        /// </summary>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int id)
        {
            try
            {
                var result = await _cityService.DeleteCityAsync(id);

                if (!result)
                {
                    return Json(new
                    {
                        success = false,
                        message = "شهر مورد نظر یافت نشد"
                    });
                }

                return Json(new
                {
                    success = true,
                    message = "شهر با موفقیت حذف شد"
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "خطا در حذف شهر {CityId}", id);
                return Json(new
                {
                    success = false,
                    message = "خطا در حذف شهر"
                });
            }
        }

        /// <summary>
        /// جستجوی کشورها (برای AutoComplete)
        /// </summary>
        [HttpGet]
        public async Task<IActionResult> SearchCountries(string? query = null)
        {
            try
            {
                var countries = await _countryService.GetAllCountriesAsync();

                if (!string.IsNullOrWhiteSpace(query))
                {
                    countries = countries.Where(c =>
                        c.Name.Contains(query, StringComparison.OrdinalIgnoreCase));
                }

                var result = countries
                    .OrderBy(c => c.Name)
                    .Take(50)
                    .Select(c => new { id = c.Id, name = c.Name });

                return Json(new
                {
                    success = true,
                    data = result
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "خطا در جستجوی کشورها");
                return Json(new
                {
                    success = false,
                    message = "خطا در دریافت لیست کشورها"
                });
            }
        }
    }
}
