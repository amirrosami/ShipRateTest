using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ShipRate.Application.DTOs;
using ShipRate.WebApp.ViewModels;
using System.Text.Json;

namespace ShipRate.WebApp.Controllers
{
    [Authorize]

    public class TestController : Controller
    {

        public async Task<IActionResult> Index()
        {
            var result = await getFilteredList(new PagedRequestDto(), null);
            return View(result);
        }

        [HttpPost]
        public async Task<IActionResult> List(PagedRequestDto dto, string name)
        {
            var res = await getFilteredList(dto, name);
            var gridVm = new GridViewModel
            {
                Data = res.toObject(),
                ListUrl = "Test/List",
                GridPartialName = "_MainGrid",
                columns = new List<GridColumn> { new GridColumn { DisplayName = "Id", Name = "Id" }, new GridColumn { DisplayName = "Name", Name = "Name" }, new GridColumn { Name = "Description", DisplayName = "Description" } }

            };
            return PartialView("_Grid", gridVm);
        }


        private async Task<PagedResult<TestCreateEditVM>> getFilteredList(PagedRequestDto dto, string name)
        {
            var data = TestStore.List;
            if (!string.IsNullOrEmpty(name))
            {
                data = data.Where(x => x.Name.Contains(name)).ToList();
            }
            if (!string.IsNullOrEmpty(dto.SearchTerm))
            {
                data = data.Where(x => x.Name.Contains(dto.SearchTerm) || x.Description.Contains(dto.SearchTerm)).ToList();

            }

            data = data.Take(dto.PageSize)
                .Skip((dto.PageNumber - 1) * dto.PageSize).ToList();

            return new PagedResult<TestCreateEditVM>()
            {
                Items = data,
                Page = dto.Page,
                PageSize = dto.PageSize,
                SearchTerm = dto.SearchTerm,
                CustomFilterValues = new { Name = name },
                SortColumn = dto.SortColumn,
                SortDirection = dto.SortDirection
            };

        }

        [HttpGet]
        public async Task<IActionResult> Create()
        {

            return PartialView("_TestForm", new TestCreateEditVM());
        }


        [HttpPost]
        public async Task<IActionResult> Create(TestCreateEditVM vm)
        {
            if (!ModelState.IsValid)
            {
                return PartialView("_TestForm", vm);
            }

            vm.Id = Guid.CreateVersion7();
            TestStore.List.Add(vm);
            // save...

            Response.Headers.Add("HX-Trigger", JsonSerializer.Serialize(new
            {
                GridChanged = true,
                toastSuccess = "Created Sussefully"
            }));


            return Ok();
        }
    }


    public class TestCreateEditVM
    {
        public Guid? Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
    }


    public static class TestStore
    {
        public static List<TestCreateEditVM> List { get; set; } = new List<TestCreateEditVM>();

    }
}






