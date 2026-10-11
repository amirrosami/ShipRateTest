using Microsoft.AspNetCore.Mvc;
using ShipRate.Application.Interfaces;
using ShipRate.WebApp.ViewModels.CompaniesViewModels;

namespace ShipRate.WebApp.ViewComponents
{
    public class CompanySelectorViewComponent : ViewComponent
    {
        private readonly ICurrentCompanyService _currentCompanyService;

        public CompanySelectorViewComponent(ICurrentCompanyService currentCompanyService)
        {
            _currentCompanyService = currentCompanyService;
        }

        public async Task<IViewComponentResult> InvokeAsync()
        {
            var companies = await _currentCompanyService.GetAccessibleCompaniesAsync();
            var currentCompanyId = await _currentCompanyService.GetCompanyIdAsync();

            var model = new CompanySelectorViewModel
            {
                CurrentCompanyId = currentCompanyId,
                Companies = companies.Select(x => new CompanyItemViewModel { Id = x.Id, Name = x.Name }).ToList()
            };
            return View(model);
        }
    }
}
