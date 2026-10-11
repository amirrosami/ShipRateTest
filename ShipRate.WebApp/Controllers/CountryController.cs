using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;
using ShipRate.Application.DTOs;
using ShipRate.Application.Interfaces;
using ShipRate.Web.Authorization;

namespace ShipRate.Web.Controllers
{

    [Authorize]
    [AutoValidateAntiforgeryToken]
    [EnableRateLimiting("api")]

    public class CountryController : Controller
    {
        private readonly ICountryService _countryService;
        private readonly ILogger<CountryController> _logger;

        public CountryController(ICountryService countryService, ILogger<CountryController> logger)
        {
            _countryService = countryService;
            _logger = logger;
        }

        public IActionResult Index()
        {
            return View();
        }

        [HttpGet]
        public async Task<IActionResult> GetList(
            int pageNumber = 1,
            int pageSize = 10,
            string? searchTerm = null,
            string? sortColumn = null,
            string? sortDirection = null)
        {
            var result = await _countryService.GetPagedCountriesAsync(
                pageNumber, pageSize, searchTerm, sortColumn, sortDirection);

            return Json(result);
        }

        // ✅ دریافت یک کشور (JSON)
        public async Task<IActionResult> GetById(int id)
        {
            var country = await _countryService.GetCountryByIdAsync(id);
            if (country == null)
                return NotFound();

            return Json(country);
        }

        [HttpPost]
        [ClaimRequirement("Permission", "Country.Create")]
        public async Task<IActionResult> Create([FromBody] CountryDto model)
        {
            try
            {
                // ✅ Validation
                if (!ModelState.IsValid)
                {
                    var errors = ModelState
                        .Where(x => x.Value.Errors.Any())
                        .ToDictionary(
                            x => x.Key,
                            x => x.Value.Errors.Select(e => e.ErrorMessage).ToArray()
                        );
                    return BadRequest(new { errors });
                }

                // ✅ Sanitization
                model.Name = model.Name.Trim();
                model.Code = model.Code.Trim().ToUpper();

                // ✅ Business Validation
                if (!await _countryService.IsCodeUniqueAsync(model.Code))
                {
                    return BadRequest(new
                    {
                        errors = new { Code = new[] { "این کد قبلاً ثبت شده است" } }
                    });
                }

                var result = await _countryService.CreateCountryAsync(model);

                // ✅ Logging (برای Audit)
                _logger.LogInformation(
                    "Country created: {CountryName} by {UserName}",
                    result.Name,
                    User.Identity?.Name
                );

                return Ok(new { success = true, message = "کشور با موفقیت ایجاد شد" });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating country");
                return StatusCode(500, "خطای داخلی سرور");
            }
        }

        [HttpDelete]
        [ClaimRequirement("Permission", "Country.Delete")]
        public async Task<IActionResult> Delete(int id)
        {
            try
            {
                // ✅ بررسی وجود
                var country = await _countryService.GetCountryByIdAsync(id);
                if (country == null)
                    return NotFound();

                // ✅ بررسی امکان حذف
                if (country.CitiesCount > 0)
                    return BadRequest("کشور دارای شهر است و قابل حذف نیست");

                var success = await _countryService.DeleteCountryAsync(id);

                // ✅ Logging
                _logger.LogWarning(
                    "Country deleted: {CountryId} by {UserName}",
                    id,
                    User.Identity?.Name
                );

                return Ok();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting country");
                return StatusCode(500, "خطای داخلی سرور");
            }
        }
        // ✅ ویرایش (JSON)
        [HttpPut]
        public async Task<IActionResult> Update([FromBody] CountryDto model)
        {
            if (!ModelState.IsValid)
            {
                var errors = ModelState
                    .Where(x => x.Value.Errors.Count > 0)
                    .ToDictionary(
                        kvp => kvp.Key,
                        kvp => kvp.Value.Errors.Select(e => e.ErrorMessage).ToArray()
                    );
                return BadRequest(new { errors });
            }

            // بررسی یکتایی کد (به جز خودش)
            if (!await _countryService.IsCodeUniqueAsync(model.Code, model.Id))
            {
                return BadRequest(new
                {
                    errors = new { Code = new[] { "این کد قبلاً ثبت شده است" } }
                });
            }

            var success = await _countryService.UpdateCountryAsync(model);
            if (!success)
                return NotFound();

            return Ok(new { success = true, message = "کشور با موفقیت تغییر داده شد" });
        }


        // ✅ تغییر وضعیت (JSON)
        [HttpPost]
        public async Task<IActionResult> ToggleStatus(int id)
        {
            var country = await _countryService.GetCountryByIdAsync(id);
            if (country == null)
                return NotFound();

            country.IsActive = !country.IsActive;
            await _countryService.UpdateCountryAsync(country);
            return Ok(new { isActive = country.IsActive });
        }


        [HttpGet]
        public async Task<IActionResult> SearchCountries(string term = "", int page = 1, int pageSize = 10)
        {
            try
            {
                // اگر term خالی باشد، همه کشورهای فعال را برگردان
                var result = await _countryService.GetPagedCountriesAsync(
                    pageNumber: page,
                    pageSize: pageSize,
                    searchTerm: term,
                    sortColumn: "Name",
                    sortDirection: "asc"
                );

                // فرمت ویژه Select2
                var select2Result = new
                {
                    results = result.Items.Select(c => new
                    {
                        id = c.Id,
                        text = c.Name
                    }),
                    pagination = new
                    {
                        more = result.HasNextPage
                    }
                };

                return Json(select2Result);
            }
            catch (Exception ex)
            {
                return Json(new
                {
                    results = new List<object>(),
                    pagination = new { more = false }
                });
            }
        }




    }
}
