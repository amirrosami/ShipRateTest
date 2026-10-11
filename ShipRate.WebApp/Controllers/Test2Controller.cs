using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ShipRate.Application.DTOs;
using ShipRate.WebApp.ViewModels;
using System.Text.Json;

namespace ShipRate.WebApp.Controllers
{
    [Authorize]

    public class Test2Controller : Controller
    {

        public async Task<IActionResult> Detail(Guid masterId)
        {
            var list = new List<DtlTestCreateEditVM>();
            for (int i = 0; i < 5; i++)
            {
                list.Add(new DtlTestCreateEditVM { Id = Guid.CreateVersion7(), MasterId = masterId, DtlDescription = $"DSCP-{masterId}-{i}", DtlName = $"Name-{masterId}-{i}" });
            }
            var result = new DetailGridViewModel
            {
                Controller = "Test2",
                MasterTitle = "Test detail",
                MasterDisplayValue = $"Master Id : {masterId.ToString()}",
                MasterId = masterId,
                Columns = new List<GridColumn> { new GridColumn { DisplayName = "DtlName", Name = "DtlName" }, new GridColumn { DisplayName = "DtlDescription", Name = "DtlDescription" } }
            ,
                Data = list.Select(x => (object)x).ToList(),
                ListUrl = "Test2/Detail",
                DetailGridPartialName = "_DtlGrid"
            };
            return PartialView("_DetailGrid", result);
        }
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
                ListUrl = "Test2/List",
                GridPartialName = "_MainGrid",
                columns = new List<GridColumn> { new GridColumn { DisplayName = "Id", Name = "Id" }, new GridColumn { DisplayName = "Name", Name = "Name" }, new GridColumn { Name = "Description", DisplayName = "Description" } }

            };
            return PartialView("_Grid", gridVm);
        }


        private async Task<PagedResult<TestCreateEditVM>> getFilteredList(PagedRequestDto dto, string name)
        {
            var data = TestStore2.List;
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
            TestStore2.List.Add(vm);
            // save...

            Response.Headers.Add("HX-Trigger", JsonSerializer.Serialize(new
            {
                GridChanged = true,
                toastSuccess = "Created Sussefully"
            }));


            return Ok();
        }
    }


    public class TestCreateEditVM2
    {
        public Guid? Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
    }

    public class DtlTestCreateEditVM
    {
        public Guid? Id { get; set; }
        public Guid MasterId { get; set; }
        public string DtlName { get; set; }
        public string DtlDescription { get; set; }
    }

    public static class TestStore2
    {
        public static List<TestCreateEditVM> List { get; set; } = new List<TestCreateEditVM>();
        public static List<DtlTestCreateEditVM> DtlList { get; set; } = new List<DtlTestCreateEditVM>();

    }

}
