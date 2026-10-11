using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ShipRate.Application.DTOs.ApiIr;
using ShipRate.Application.Interfaces.ExternalServices;

namespace ShipRate.WebApp.Controllers
{
    [Authorize]
    public class ApiIrController : Controller
    {
        private readonly IApiIrService _apiIrService;
        private readonly ILogger<ApiIrController> _logger;

        public ApiIrController(IApiIrService apiIrService, ILogger<ApiIrController> logger)
        {
            _apiIrService = apiIrService;
            _logger = logger;
        }

        [Authorize("BaseInfo.Persons.ApiGetPersonInfo")]
        [HttpGet]
        public IActionResult PersonInfo()
        {
            return PartialView("_PersonInfoForm", new PersonInfoLookupVm());
        }

        [Authorize("BaseInfo.Persons.ApiGetPersonInfo")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> PersonInfo(PersonInfoLookupVm vm, CancellationToken ct)
        {
            if (!ModelState.IsValid)
                return PartialView("_PersonInfoForm", vm);

            try
            {
                var request = new PersonInfoRequest
                {
                    NationalCode = vm.NationalCode,
                    BirthDate = vm.BirthDate
                };

                var result = await _apiIrService.PersonInfoAsync(request, ct);
                vm.Result = result;

                return PartialView("_PersonInfoForm", vm);
            }
            catch (Exception ex)
            {
                vm.ErrorMessage = ex.Message;
                return PartialView("_PersonInfoForm", vm);
            }
        }

        [Authorize("BaseInfo.Persons.ApiGetPersonInfo")]
        [HttpGet]
        public IActionResult CompanyInfo()
        {
            return PartialView("_CompanyInfoForm", new CompanyInfoLookupVm());
        }

        [Authorize("BaseInfo.Persons.ApiGetPersonInfo")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CompanyInfo(
            CompanyInfoLookupVm vm,
            CancellationToken ct)
        {
            if (!ModelState.IsValid)
                return PartialView("_CompanyInfoForm", vm);

            try
            {
                var request = new CompanyInfoRequest
                {
                    NationalId = vm.NationalId
                };

                var result = await _apiIrService.CompanyInfoAsync(request, ct);

                /*
                 * برای این endpoint صرفاً Success را ملاک نمی‌گیریم؛
                 * چون در نمونه پاسخ API.ir، با وجود data معتبر، success=false آمده است.
                 */
                if (result.Data is null)
                {
                    vm.ErrorMessage = !string.IsNullOrWhiteSpace(result.Message)
                        ? result.Message
                        : "اطلاعاتی برای این شناسه ملی شرکت یافت نشد.";

                    vm.Result = result;

                    return PartialView("_CompanyInfoForm", vm);
                }

                vm.Result = result;

                return PartialView("_CompanyInfoForm", vm);
            }
            catch (Exception ex)
            {
                _logger.LogError(
                    ex,
                    "Error while calling API.ir CompanyInfo for NationalId: {NationalId}",
                    vm.NationalId);

                // در محیط Production جزئیات Exception را به کاربر نشان ندهید.
                vm.ErrorMessage = "خطا در برقراری ارتباط با سرویس استعلام شرکت‌ها.";

                return PartialView("_CompanyInfoForm", vm);
            }
        }



        [Authorize("BaseInfo.Persons.ApiGetReturnChequesInfo")]
        [HttpGet]
        public IActionResult ChequeInfo()
        {
            return PartialView("_ChequeInfoForm", new ChequeInfoLookupVm());
        }

        [Authorize("BaseInfo.Persons.ApiGetReturnChequesInfo")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ChequeInfo(ChequeInfoLookupVm vm, CancellationToken ct)
        {
            if (!ModelState.IsValid)
                return PartialView("_ChequeInfoForm", vm);

            try
            {
                var request = new ChequeInfoRequest { NationalCode = vm.NationalCode };
                var result = await _apiIrService.ChequeInfoAsync(request, ct);

                vm.Result = result;
                return PartialView("_ChequeInfoForm", vm);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Cheque inquiry failed for {Id}", vm.NationalCode);
                vm.ErrorMessage = "خطا در استعلام چک‌های برگشتی.";
                return PartialView("_ChequeInfoForm", vm);
            }
        }
    }
}
