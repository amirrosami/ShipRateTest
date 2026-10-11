using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using ShipRate.Application.DTOs;
using ShipRate.Application.DTOs.Base_Info;
using ShipRate.Application.Interfaces;
using ShipRate.Application.Interfaces.BaseInfo;
using ShipRate.Application.Interfaces.Location;
using ShipRate.Common.Web;
using ShipRate.Domain.Entities.BaseInfo.Persons;
using ShipRate.Infrastructure.Data;
using ShipRate.WebApp.ViewModels;
using System.Linq.Dynamic.Core;
using System.Text;
using System.Text.Json;

namespace ShipRate.WebApp.Controllers
{
    [Authorize]
    public class PersonsController : BaseController
    {
        private readonly IPersonService _service;
        private readonly IBaseDataService _basedataservice;
        private readonly ApplicationDbContext _context;
        private readonly ICurrentUserService _currentUserService;
        private readonly ILocationService _locationService;


        public PersonsController(UserService userService, IPersonService service, IBaseDataService basedataservice, ApplicationDbContext context, ICurrentUserService currentUserService, ILocationService locationService) : base(userService)
        {
            _service = service;
            _basedataservice = basedataservice;
            _context = context;
            _currentUserService = currentUserService;
            _locationService = locationService;
        }

        private void HxTrigger(object triggers)
        {
            Response.Headers["HX-Trigger"] = JsonSerializer.Serialize(triggers);
        }

        [Authorize(Policy = "BaseInfo.Persons.View")]
        [HttpGet]
        [IgnoreAntiforgeryToken] // برای GET نیازی نیست
        public async Task<IActionResult> Index()
        {
            var personalityTypes = await _basedataservice.GetPersonalityTypesAsync();

            ViewBag.PersonalityTypes = new SelectList(personalityTypes, "Id", "PersonalityTypeName");

            var roleTypes = await _context.RoleTypes.ToListAsync();
            ViewBag.RoleTypes = new SelectList(roleTypes, "Id", "RoleTypeName");
            var result = await getFilteredList(new PagedRequestDto(), 0, 0, false);
            return View(result);
        }
        [HttpPost]
        public async Task<IActionResult> List(PagedRequestDto request, int personalityType, int roleType, bool filterMyClients)
        {
            var result = await getFilteredList(request, personalityType, roleType, filterMyClients);
            var gridVm = new GridViewModel
            {
                Data = result.toObject(),
                ListUrl = "Persons/List",
                GridPartialName = "_List",
                columns = GetGridColumns()

            };
            return PartialView("_Grid", gridVm);
        }
        private List<GridColumn> GetGridColumns()
        {
            return new List<GridColumn> {
                new GridColumn { DisplayName = "Name", Name = "PersonName" }, new GridColumn { DisplayName = "Persian Name", Name = "PersonPersianName" }, new GridColumn { Name = "PersonalityTypeName", DisplayName = "Personality Type" }, new GridColumn { Name = "CountryName", DisplayName = "Country" } ,new GridColumn{ Name="NationalID" ,DisplayName="NationalID"}, new GridColumn{ Name="EmailAddress" , DisplayName="Email"} , new GridColumn("Phone","Phone") , new GridColumn("Mobile","Mobile") };
        }



        private async Task<ShipRate.WebApp.ViewModels.PagedResult<PersonDto>> getFilteredList(PagedRequestDto dto, int personalityType, int roleType, bool filterMyClients)
        {
            var data = _context.Persons
                .Include(x => x.Country)
                .Include(X => X.PersonalityType)
                .Where(x => !x.IsDeleted)
                .AsQueryable();

            var userId = _currentUserService.UserId;
            if (_currentUserService.IsSalesUser() && !_currentUserService.IsAdmin() && !_currentUserService.IsSalesManager() && !_currentUserService.IsManager())
            {
                data = data.Where(p =>
                    !_context.Customers.Any(c => c.Id == p.Id) ||
                    _context.Customers.Any(c => c.Id == p.Id && c.SalesUserId == userId)
                );
            }

            if (filterMyClients)
            {
                if (_currentUserService.IsSalesUser())
                {
                    data = data.Where(p => _context.Customers.Any(c => c.Id == p.Id && c.SalesUserId == userId)
                    );
                }
            }



            if (personalityType != null && personalityType > 0)
            {
                data = data.Where(x => x.PersonalityTypeId == personalityType);
            }







            if (!string.IsNullOrEmpty(dto.SearchTerm))
            {

                var searchPersian = dto.SearchTerm.Replace('ي', 'ی').Replace('ك', 'ک');
                var searchArabic = dto.SearchTerm.Replace('ی', 'ي').Replace('ک', 'ك');


                data = data.Where(x => x.PersonName.Contains(dto.SearchTerm)
                    || x.PersonPersianName.Contains(dto.SearchTerm) || x.PersonPersianName.Contains(searchPersian) || x.PersonPersianName.Contains(searchArabic)
                    || x.Country.CountryName.Contains(dto.SearchTerm) || x.Country.CountryName.Contains(searchPersian) || x.Country.CountryName.Contains(searchArabic)
                    || x.NationalID.Contains(dto.SearchTerm) || x.Phone.Contains(dto.SearchTerm) || x.EmailAddress.Contains(dto.SearchTerm) || x.Mobile.Contains(dto.SearchTerm)
                    || _context.PersonContacts.Where(c => c.PersonId == x.Id && (c.FullName.Contains(dto.SearchTerm) || c.FullName.Contains(searchPersian) || c.FullName.Contains(searchPersian) || c.Mobile.Contains(dto.SearchTerm) || c.Phone.Contains(dto.SearchTerm))).Any()
                    || _context.PersonExtraInfos.Where(c => c.PersonId == x.Id && (c.PersonExtraInfoValue.Contains(dto.SearchTerm) || c.PersonExtraInfoValue.Contains(searchPersian) || c.PersonExtraInfoValue.Contains(searchArabic))).Any()
                );
            }
            if (roleType > 6)
            {
                data = data.Where(x => _context.PersonRoles.Where(r => r.PersonId == x.Id && r.RoleTypeId == roleType).Any());
            }
            if (roleType == 1)
            {
                data = data.Where(x => _context.Carriers.Where(d => d.Id == x.Id).Any());
            }
            if (roleType == 3)
            {
                data = data.Where(x => _context.Agents.Where(d => d.Id == x.Id).Any());
            }
            if (roleType == 4)
            {
                data = data.Where(x => _context.CourierCompanies.Where(d => d.Id == x.Id).Any());
            }
            if (roleType == 5)
            {
                data = data.Where(x => _context.Drivers.Where(d => d.Id == x.Id).Any());
            }

            if (roleType == 6)
            {
                data = data.Where(x => _context.GPSProviders.Where(d => d.Id == x.Id).Any());
            }


            var selectedData = data.Select(x => new PersonDto
            {
                Id = x.Id,
                PersonalityTypeName = x.PersonalityType.PersonalityTypeName,
                CountryName = x.Country.CountryName,
                NationalID = x.NationalID,
                PersonName = x.PersonName,
                PersonPersianName = x.PersonPersianName,
                EmailAddress = x.EmailAddress,
                Phone = x.Phone,
                Mobile = x.Mobile,
                UpdatedAt = x.UpdatedAt
            });

            if (!string.IsNullOrEmpty(dto.SortColumn) && !string.IsNullOrEmpty(dto.SortDirection))
            {
                selectedData = selectedData.OrderBy($"{dto.SortColumn}  {dto.SortDirection}");
            }


            int total = await selectedData.CountAsync();

            selectedData = selectedData.Skip((dto.Page - 1) * dto.PageSize).Take(dto.PageSize);

            var items = await selectedData.ToListAsync();

            return new ShipRate.WebApp.ViewModels.PagedResult<PersonDto>()
            {
                Items = items,
                Page = dto.Page,
                PageSize = dto.PageSize,
                TotalCount = total,
                SearchTerm = dto.SearchTerm,
                CustomFilterValues = new { },
                SortColumn = dto.SortColumn,
                SortDirection = dto.SortDirection

            };

        }


        private async Task PopulateDropDowns()
        {
            var personalityTypes = await _basedataservice.GetPersonalityTypesAsync();
            var countries = await _basedataservice.GetCountriesAsync();
            ViewBag.PersonalityTypes = new SelectList(personalityTypes, "Id", "PersonalityTypeName");
            ViewBag.Countries = new SelectList(countries, "Id", "CountryName");
            var roleTypes = await _context.RoleTypes.Where(x => x.Id >= 21).ToListAsync();
            ViewBag.RoleTypes = roleTypes;
            var companies = await _basedataservice.GetCompanies();
            var companiesmap = companies.Select(x => new { Id = x.Id, CompanyName = x.Person.PersonName });
            ViewBag.Companies = new SelectList(companiesmap, "Id", "CompanyName");
            var agents = await _basedataservice.GetAgents();
            var agentsmap = agents.Select(x => new { Id = x.Id, Name = x.Person.PersonName });
            ViewBag.Agents = new SelectList(agentsmap, "Id", "Name");

            var carrierTypes = await _basedataservice.GetCarrierTypesAsync();
            ViewBag.CarrierTypes = new SelectList(carrierTypes, "Id", "CarrierTypeName");

            var carriers = await _context.Carriers.Include(x => x.Person).Select(x => new { Id = x.Id, Name = x.Person.PersonName }).ToListAsync();
            ViewBag.Carriers = new SelectList(carriers, "Id", "Name");
        }

        [Authorize(Policy = "BaseInfo.Persons.Edit")]
        [HttpGet]
        public async Task<IActionResult> Create()
        {
            await PopulateDropDowns();
            ViewBag.CreateEdit = "Create";
            return PartialView("_CreateEdit", new PersonCreateUpdateDto { });
        }

        [Authorize(Policy = "BaseInfo.Persons.Edit")]
        [HttpGet]
        public async Task<IActionResult> CreateAgent()
        {
            await PopulateDropDowns();
            ViewBag.CreateEdit = "Create";
            return PartialView("_CreateEdit", new PersonCreateUpdateDto { IsAgent = true });
        }
        [HttpGet]
        public async Task<IActionResult> ContactList(Guid personId)
        {
            var result = await _context.PersonContacts
                .Where(x => x.PersonId == personId)
                .Select(x => new PersonContactDto
                {
                    Id = x.Id,
                    PersonId = x.PersonId,
                    FullName = x.FullName,
                    Email = x.Email,
                    IsMain = x.IsMain,
                    Phone = x.Phone
                }).ToListAsync();
            return PartialView("_ContactsList", result);
        }






        [HttpGet]
        public async Task<IActionResult> ExtraInfoList(Guid personId)
        {
            var result = await _context.PersonExtraInfos
                .Include(x => x.InfoType)
                .Where(x => x.PersonId == personId)
                .Select(x => new PersonExtraInfoDto
                {
                    Id = x.Id,
                    PersonId = x.PersonId,
                    ExtraInfoTypeName = x.InfoType.InfoTypeName,
                    PersonExtraInfoValue = x.PersonExtraInfoValue
                }).ToListAsync();
            return PartialView("_ExtraInfoList", result);
        }

        [HttpGet]
        public async Task<IActionResult> BankAccountList(Guid personId)
        {
            var result = await _context.BankAccounts
                .Where(x => x.PersonId == personId)
                .Select(x => new BankAccountDto
                {
                    Id = x.Id,
                    PersonId = x.PersonId,
                    BankName = x.Bank.BankName,
                    BankAccountPurposeName = x.BankAccountPurpose.BankAccountPurposeName,
                    IBAN = x.IBAN,
                    AccountNumber = x.AccountNumber
                }).ToListAsync();
            return PartialView("_BankAccountsList", result);
        }


        [HttpGet]
        public async Task<IActionResult> BranchList(Guid personId)
        {
            var result = await _context.Branches
                .Where(x => x.ParentId == personId)
                .Select(x => new BranchDto
                {
                    Id = x.Id,
                    PersonId = x.ParentId,
                    Address = x.Address,
                    BranchCode = x.BranchCode,
                    BranchName = x.BranchName,
                    BranchTypeId = x.BranchTypeId,
                    BranchTypeName = x.BranchType.BranchTypeName,
                    CountryId = x.CountryId,
                    Country = x.Country.CountryName,
                    Location = x.Location.LocationName,
                    CreatedAt = x.CreatedAt,
                    Email = x.Email,
                    ManagerName = x.ManagerName,
                    LocationId = x.LocationId,
                    PersonName = x.Parent.PersonName,
                    Phone = x.Phone,
                    UpdatedAt = x.UpdatedAt
                }).ToListAsync();
            return PartialView("_BranchesList", result);
        }

        [Authorize(Policy = "BaseInfo.Persons.Edit")]
        [HttpGet]
        public async Task<IActionResult> CreateContact(Guid personId)
        {
            ViewBag.CreateEditContact = "Create";
            return PartialView("_CreateEditContact", new PersonContactCreateUpdateDto { PersonId = personId });
        }

        [Authorize(Policy = "BaseInfo.Persons.Edit")]
        [HttpGet]
        public async Task<IActionResult> CreateExtraInfo(Guid personId)
        {
            ViewBag.CreateEditExtraInfo = "Create";
            var infoTypes = await _basedataservice.GetInfoTypesAsync(1);
            ViewBag.InfoTypes = new SelectList(infoTypes, "Id", "InfoTypeName");

            return PartialView("_CreateEditExtraInfo", new PersonExtraInfoDto { PersonId = personId });
        }

        [Authorize(Policy = "BaseInfo.Persons.Edit")]
        [HttpGet]
        public async Task<IActionResult> CreateBankAccount(Guid personId)
        {
            ViewBag.CreateEditBankAccount = "Create";

            var banks = await _basedataservice.GetBanksAsync();
            ViewBag.Banks = new SelectList(banks, "Id", "BankName");

            var purposes = await _basedataservice.GetBankAccountPurposesAsync();
            ViewBag.BankAccountPurposes = new SelectList(purposes, "Id", "BankAccountPurposeName");


            var currencies = await _basedataservice.GetCurrenciesAsync();
            ViewBag.Currencies = new SelectList(currencies, "Id", "CurrencyName");

            return PartialView("_CreateEditBankAccount", new BankAccountDto { PersonId = personId });
        }

        [Authorize(Policy = "BaseInfo.Persons.Edit")]
        [HttpGet]
        public async Task<IActionResult> CreateBranch(Guid personId)
        {
            ViewBag.CreateEditBranch = "Create";

            var branchtypes = await _basedataservice.GetBranchTypesAsync();
            ViewBag.BranchTypes = new SelectList(branchtypes, "Id", "BranchTypeName");

            var countries = await _basedataservice.GetCountriesAsync();
            ViewBag.Countries = new SelectList(countries, "Id", "CountryName");

            var locations = await _locationService.GetLocationsByCountryIdAsync(1);
            ViewBag.Locations = new SelectList(locations, "Id", "LocationName");
            return PartialView("_CreateEditBranch", new BranchDto { PersonId = personId, CountryId = 1 });
        }


        [Authorize(Policy = "BaseInfo.Persons.Edit")]
        [HttpGet]
        public async Task<IActionResult> EditBranch(Guid id)
        {
            ViewBag.CreateEditBranch = "Edit";
            var branch = await _service.GetBranchByIdAsync(id);


            var branchtypes = await _basedataservice.GetBranchTypesAsync();
            ViewBag.BranchTypes = new SelectList(branchtypes, "Id", "BranchTypeName");

            var countries = await _basedataservice.GetCountriesAsync();
            ViewBag.Countries = new SelectList(countries, "Id", "CountryName");

            var locations = await _locationService.GetLocationsByCountryIdAsync(branch.CountryId);
            ViewBag.Locations = new SelectList(locations, "Id", "LocationName");

            return PartialView("_CreateEditBranch", branch);
        }

        [Authorize(Policy = "BaseInfo.Persons.Edit")]
        [HttpGet]
        public async Task<IActionResult> EditBankAccount(Guid id)
        {
            ViewBag.CreateEditBankAccount = "Edit";

            var banks = await _basedataservice.GetBanksAsync();
            ViewBag.Banks = new SelectList(banks, "Id", "BankName");

            var purposes = await _basedataservice.GetBankAccountPurposesAsync();
            ViewBag.BankAccountPurposes = new SelectList(purposes, "Id", "BankAccountPurposeName");


            var currencies = await _basedataservice.GetCurrenciesAsync();
            ViewBag.Currencies = new SelectList(currencies, "Id", "CurrencyName");

            var banlAccount = await _service.GetBankAccountByIdAsync(id);
            return PartialView("_CreateEditBankAccount", banlAccount);
        }



        [Authorize(Policy = "BaseInfo.Persons.Edit")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(PersonCreateUpdateDto dto)
        {
            //if (!ModelState.IsValid)
            //{
            //    Response.StatusCode = 422;
            //    await PopulateDropDowns();
            //    ViewBag.CreateEdit = "Create";
            //    return PartialView("_CreateEdit", dto);
            //}

            ModelState.Clear();
            var (success, error) = await _service.CreateAsync(dto);

            if (!success)
            {
                ModelState.AddModelError("", error!);
                Response.StatusCode = 422;
                await PopulateDropDowns();
                ViewBag.CreateEdit = "Create";
                return PartialView("_CreateEdit", dto);
            }


            HxTrigger(new
            {
                GridChanged = true,
                toastSuccess = "Created Sussefully"
            });
            return Ok();
        }




        [Authorize(Policy = "BaseInfo.Persons.Edit")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateContact(PersonContactCreateUpdateDto dto)
        {
            if (!ModelState.IsValid)
            {
                Response.StatusCode = 422;
                ViewBag.CreateEditContact = "Create";
                return PartialView("_CreateEditContact", new PersonContactCreateUpdateDto { PersonId = dto.PersonId });
            }


            var (success, error) = await _service.CreateContactAsync(dto);

            if (!success)
            {
                ModelState.AddModelError("Code", error!);
                Response.StatusCode = 422;
                ViewBag.CreateEditContact = "Create";
                return PartialView("_CreateEditContact", dto);
            }

            HxTrigger(new
            {
                GridChangedOnAppModal = true,
                toastSuccess = "Created Sussefully"
            });

            //Response.Headers.Append("HX-Trigger", "closeModal, refreshList, showToast");
            return Content("");
        }


        [Authorize(Policy = "BaseInfo.Persons.Edit")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateExtraInfo(PersonExtraInfoDto dto)
        {
            if (!ModelState.IsValid)
            {
                Response.StatusCode = 422;
                ViewBag.CreateEditExtraInfo = "Create";
                return PartialView("_CreateEditExtraInfo", new PersonExtraInfoDto { PersonId = dto.PersonId });
            }


            var (success, error) = await _service.CreateExtraInfoAsync(dto);

            if (!success)
            {
                ModelState.AddModelError("Code", error!);
                Response.StatusCode = 422;
                ViewBag.CreateEditExtraInfo = "Create";
                return PartialView("_CreateEditExtraInfo", new PersonExtraInfoDto { PersonId = dto.PersonId });
            }

            HxTrigger(new
            {
                GridChangedOnAppModal = true,
                toastSuccess = $"Created Successfully"
            });

            //Response.Headers.Append("HX-Trigger", "closeModal, refreshList, showToast");
            return Content("");
        }


        [Authorize(Policy = "BaseInfo.Persons.Edit")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateBankAccount(BankAccountDto dto)
        {
            if (!ModelState.IsValid)
            {
                Response.StatusCode = 422;
                ViewBag.CreateEditBankAccount = "Create";
                return PartialView("_CreateEditBankAccount", new BankAccountDto { PersonId = dto.PersonId });
            }


            var (success, error) = await _service.CreateBankAccountAsync(dto);

            if (!success)
            {
                ModelState.AddModelError("Code", error!);
                Response.StatusCode = 422;
                ViewBag.CreateEditBankAccount = "Create";
                return PartialView("_CreateEditBankAccount", new BankAccountDto { PersonId = dto.PersonId });
            }

            HxTrigger(new
            {
                GridChangedOnAppModal = true,
                toastSuccess = $"Created Successfully"
            });

            //Response.Headers.Append("HX-Trigger", "closeModal, refreshList, showToast");
            return Content("");
        }

        [Authorize(Policy = "BaseInfo.Persons.Edit")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateBranch(BranchDto dto)
        {
            if (!ModelState.IsValid)
            {
                Response.StatusCode = 422;
                ViewBag.CreateEditBranch = "Create";
                return PartialView("_CreateEditBranch", new BranchDto { PersonId = dto.PersonId });
            }


            var (success, error) = await _service.CreateBranchAsync(dto);

            if (!success)
            {
                ModelState.AddModelError("Code", error!);
                Response.StatusCode = 422;
                ViewBag.CreateEditBankAccount = "Create";
                return PartialView("_CreateBranch", new BranchDto { PersonId = dto.PersonId });
            }

            HxTrigger(new
            {
                GridChangedOnAppModal = true,
                toastSuccess = $"Created Successfully"
            });

            //Response.Headers.Append("HX-Trigger", "closeModal, refreshList, showToast");
            return Content("");
        }

        [HttpGet]
        public async Task<IActionResult> Details(Guid id)
        {
            var person = await _service.GetByIdAsync(id);
            var generalRoles = await _context.PersonRoles.Include(x => x.RoleType).Where(x => x.PersonId == id).ToListAsync();
            if (generalRoles == null)
            {
                generalRoles = new();
            }
            var agent = await _service.GetAgentByIdAsync(id);
            var driver = await _service.GetDriverByIdAsync(id);
            var courier = await _service.GetCourierCompanyByIdAsync(id);
            var carrier = await _service.GetCarrierByIdAsync(id);
            var gps = await _service.GetGPSProviderByIdAsync(id);

            person.Agent = agent;
            person.Driver = driver;
            person.GPSProviderDto = gps;
            person.Carrier = carrier;
            person.CourierCompany = courier;
            person.RoleTypes = generalRoles.Select(x => x.RoleType).ToList();
            person.lastPictureId = GetLastPictureId(id);
            return PartialView("_Details", person);
        }

        [Authorize(Policy = "BaseInfo.Persons.Edit")]
        public async Task<IActionResult> Edit(Guid id)
        {
            var entity = await _service.GetByIdAsync(id);
            if (entity == null) return NotFound();

            var agent = await _service.GetAgentByIdAsync(id);
            var carrier = await _service.GetCarrierByIdAsync(id);
            var driver = await _service.GetDriverByIdAsync(id);
            var gps = await _service.GetGPSProviderByIdAsync(id);
            var couriercompany = await _service.GetCourierCompanyByIdAsync(id);
            var selectedRoles = await _service.GetSelectedRoleTypes(id);
            var dto = new PersonCreateUpdateDto
            {
                Id = entity.Id,
                FirstName = entity.FirstName,
                LastName = entity.LastName,
                PersonalityTypeId = entity.PersonalityTypeId,
                NationalID = entity.NationalID,
                Mobile = entity.Mobile,
                FaxNo = entity.FaxNo,
                EmailAddress = entity.EmailAddress,
                Address = entity.Address,
                BirthDate = entity.BirthDate,
                CityId = entity.CityId,
                CountryId = entity.CountryId,
                EcoCode = entity.EcoCode,
                PersonName = entity.PersonName,
                PersonPersianName = entity.PersonPersianName,
                PersonRegDate = entity.PersonRegDate,
                ZipCode = entity.ZipCode,
                Phone = entity.Phone,
                RegNo = entity.RegNo,
                PersianAddress = entity.PersianAddress,
                Agent = agent ?? new(),
                Driver = driver ?? new(),
                CourierCompany = couriercompany ?? new(),
                GPSProviderDto = gps ?? new(),
                Carrier = carrier ?? new(),
                IsAgent = agent != null,
                IsDriver = driver != null,
                IsGPSProvider = gps != null,
                IsCourierCompany = couriercompany != null,
                IsCarrier = carrier != null,
                SelectedRoleTypes = selectedRoles ?? new(),
            };

            await PopulateDropDowns();
            ViewBag.CreateEdit = "Edit";
            return PartialView("_CreateEdit", dto);
        }


        private async Task FillAgentViewBags()
        {
            var companies = await _basedataservice.GetCompanies();
            ViewBag.Companies = new SelectList(companies, "Id", "Person.PersonName");


            var agents = await _basedataservice.GetAgents();
            ViewBag.Agents = new SelectList(agents, "Id", "Person.PersonName");

        }

        public async Task<IActionResult> SetAgentInfo(Guid id)
        {
            var entity = await _service.GetByIdAsync(id);
            if (entity == null) return NotFound();

            await FillAgentViewBags();

            var dto = new AgentDto
            {
                PersonId = id
            };

            var agent = await _service.GetAgentByIdAsync(id);

            if (agent != null)
            {
                dto.Id = id;
                dto.PersonId = id;
                dto.CompanyId = agent.CompanyId;
                dto.ParentId = agent.ParentId;
            }
            return PartialView("_CreateEditAgent", dto);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> SetAgentInfo(AgentDto dto)
        {
            if (!ModelState.IsValid)
            {
                Response.StatusCode = 422;
                await FillAgentViewBags();
                return PartialView("_CreateEditAgent", dto);
            }

            var (success, error) = await _service.SaveAgentAsync(dto, true);

            if (!success)
            {
                ModelState.AddModelError("Code", error!);
                Response.StatusCode = 422;
                await FillAgentViewBags();
                return PartialView("_CreateEditAgent", dto);
            }
            HxTrigger(new
            {
                GridChangedOnAppModal = true,
                toastSuccess = $"Saved Successfully"
            });
            return Content("");
        }

        public async Task<IActionResult> SetCarrierInfo(Guid id)
        {
            var entity = await _service.GetByIdAsync(id);
            if (entity == null) return NotFound();

            var carrierTypes = await _basedataservice.GetCarrierTypesAsync();
            ViewBag.CarrierTypes = new SelectList(carrierTypes, "Id", "CarrierTypeName");

            var dto = new Application.DTOs.Base_Info.CarrierDto
            {
                PersonId = id
            };

            var carrier = await _service.GetCarrierByIdAsync(id);

            if (carrier != null)
            {
                dto.CarrierCode = carrier.CarrierCode;
                dto.TrackingUrl = carrier.TrackingUrl;
                dto.AWBPrefix = "1";
                dto.CarrierTypeId = carrier.CarrierTypeId;
                dto.BookingEmail = carrier.BookingEmail;
                dto.IsVesselOwner = carrier.IsVesselOwner;
                dto.OperatingRegion = carrier.OperatingRegion;
                dto.Id = id;
            }
            return PartialView("_CreateEditCarrier", dto);
        }

        [Authorize(Policy = "BaseInfo.Persons.Edit")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> SetCarrierInfo(Application.DTOs.Base_Info.CarrierDto dto)
        {
            if (!ModelState.IsValid)
            {
                Response.StatusCode = 422;
                return PartialView("_CreateEditCarrier", dto);
            }

            var (success, error) = await _service.SaveCarrierAsync(dto, true);

            if (!success)
            {
                ModelState.AddModelError("Code", error!);
                Response.StatusCode = 422;
                return PartialView("_CreateEditCarrier", dto);
            }
            HxTrigger(new
            {
                GridChangedOnAppModal = true,
                toastSuccess = $"Saved Successfully"
            });
            return Content("");
        }

        [Authorize(Policy = "BaseInfo.Persons.Edit")]
        public async Task<IActionResult> SetDriverInfo(Guid id)
        {
            var entity = await _service.GetByIdAsync(id);
            if (entity == null) return NotFound();

            var carriers = await _basedataservice.GetCarriers(4);
            ViewBag.Carriers = new SelectList(carriers, "Id", "Person.PersonName");

            var dto = new DriverDto
            {
                PersonId = id
            };
            var driver = await _service.GetDriverByIdAsync(id);
            if (driver != null)
            {
                dto.Id = id;
                dto.PersonId = id;
                dto.LicenseNumber = driver.LicenseNumber;
                dto.SmartCardNo = driver.SmartCardNo;
                dto.PassportNo = driver.PassportNo;
                dto.CarrierId = driver.CarrierId;
            }
            return PartialView("_CreateEditDriver", dto);
        }



        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> SetDriverInfo(DriverDto dto)
        {
            if (!ModelState.IsValid)
            {
                Response.StatusCode = 422;
                return PartialView("_CreateEditDriver", dto);
            }

            var (success, error) = await _service.SaveDriverAsync(dto, true);

            if (!success)
            {
                ModelState.AddModelError("Code", error!);
                Response.StatusCode = 422;
                return PartialView("_CreateEditDriver", dto);
            }
            HxTrigger(new
            {
                closeModal = new { },
                refreshPersonsList = new { },
                showToast = new { message = "Driver info saved successfully.", type = "success" }
            });
            return Content("");
        }

        [Authorize(Policy = "BaseInfo.Persons.Edit")]
        public async Task<IActionResult> EditContact(Guid id)
        {
            var entity = await _service.GetContactByIdAsync(id);
            if (entity == null) return NotFound();

            //var categories = await _service.GetCategoriesAsync();
            //ViewBag.Categories = new SelectList(categories, "Id", "PersonCategoryName");

            var dto = new PersonContactCreateUpdateDto
            {
                Id = entity.Id,
                Role = entity.Role,
                Phone = entity.Phone,
                PersonId = entity.PersonId,
                Email = entity.Email,
                IsMain = entity.IsMain,
                FullName = entity.FullName
            };

            ViewBag.CreateEditContact = "Edit";
            return PartialView("_CreateEditContact", dto);
        }

        [Authorize(Policy = "BaseInfo.Persons.Edit")]
        public async Task<IActionResult> EditExtraInfo(Guid id)
        {
            var entity = await _service.GetExtraInfoByIdAsync(id);
            if (entity == null) return NotFound();

            //var categories = await _service.GetCategoriesAsync();
            //ViewBag.Categories = new SelectList(categories, "Id", "PersonCategoryName");



            var dto = new PersonExtraInfoDto
            {
                Id = entity.Id,
                PersonId = entity.PersonId,
                ExtraInfoTypeName = entity.ExtraInfoTypeName,
                ExtraInfoTypeId = entity.ExtraInfoTypeId,
                PersonExtraInfoValue = entity.PersonExtraInfoValue
            };
            var infoTypes = await _basedataservice.GetInfoTypesAsync(1);
            ViewBag.InfoTypes = new SelectList(infoTypes, "Id", "InfoTypeName");
            ViewBag.CreateEditExtraInfo = "Edit";
            return PartialView("_CreateEditExtraInfo", dto);
        }



        [Authorize(Policy = "BaseInfo.Persons.Edit")]

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(PersonCreateUpdateDto dto)
        {
            var (success, error) = await _service.UpdateAsync(dto);

            if (!success)
            {
                ModelState.AddModelError("Code", error!);
                Response.StatusCode = 422;
                await PopulateDropDowns();
                return PartialView("_CreateEdit", dto);
            }

            HxTrigger(new
            {
                GridChanged = true,
                toastSuccess = "Edited Sussefully"
            });
            return Ok();
        }

        [Authorize(Policy = "BaseInfo.Persons.Edit")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditContact(PersonContactCreateUpdateDto dto)
        {
            if (!ModelState.IsValid)
            {
                Response.StatusCode = 422;
                return PartialView("_CreateEditContact", dto);
            }

            var (success, error) = await _service.UpdateContactAsync(dto);

            if (!success)
            {
                ModelState.AddModelError("Code", error!);
                Response.StatusCode = 422;
                return PartialView("_CreateEditContact", dto);
            }

            HxTrigger(new
            {
                GridChangedOnAppModal = true,
                toastSuccess = $"Edited Successfully"
            });

            return Content("");
        }


        [Authorize(Policy = "BaseInfo.Persons.Edit")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditBankAccount(BankAccountDto dto)
        {
            if (!ModelState.IsValid)
            {
                Response.StatusCode = 422;
                return PartialView("_CreateEditBankAccount", dto);
            }

            var (success, error) = await _service.UpdateBankAccountAsync(dto);

            if (!success)
            {
                ModelState.AddModelError("Code", error!);
                Response.StatusCode = 422;
                return PartialView("_CreateEditBankAccount", dto);
            }

            HxTrigger(new
            {
                GridChangedOnAppModal = true,
                toastSuccess = $"Edited Successfully"
            });

            return Content("");
        }


        [Authorize(Policy = "BaseInfo.Persons.Edit")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditBranch(BranchDto dto)
        {
            if (!ModelState.IsValid)
            {
                Response.StatusCode = 422;
                return PartialView("_CreateEditBranch", dto);
            }

            var (success, error) = await _service.UpdateBranchAsync(dto);

            if (!success)
            {
                ModelState.AddModelError("Code", error!);
                Response.StatusCode = 422;
                return PartialView("_CreateEditBranch", dto);
            }

            HxTrigger(new
            {
                GridChangedOnAppModal = true,
                toastSuccess = $"Updated Successfully"
            });

            return Content("");
        }





        [Authorize(Policy = "BaseInfo.Persons.Edit")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditExtraInfo(PersonExtraInfoDto dto)
        {
            if (!ModelState.IsValid)
            {
                Response.StatusCode = 422;
                return PartialView("_CreateEditExtraInfo", dto);
            }

            var (success, error) = await _service.UpdateExtraInfoAsync(dto);

            if (!success)
            {
                ModelState.AddModelError("Code", error!);
                Response.StatusCode = 422;
                return PartialView("_CreateEditExtraInfo", dto);
            }

            HxTrigger(new
            {
                GridChangedOnAppModal = true,
                toastSuccess = $"Edited Successfully"
            });
            return Content("");
        }





        [Authorize(Policy = "BaseInfo.Persons.Edit")]
        [HttpGet]
        public async Task<IActionResult> Delete(Guid id)
        {
            //var item = await _service.GetByIdAsync(id);
            //if (item == null) return NotFound();
            return PartialView("_DeleteConfirm", id);
        }

        [Authorize(Policy = "BaseInfo.Persons.Edit")]
        [HttpGet]
        public async Task<IActionResult> DeleteContact(Guid id)
        {
            var entity = await _service.GetContactByIdAsync(id);
            if (entity == null) return NotFound();
            return PartialView("_DeleteContactConfirm", entity);
        }

        [Authorize(Policy = "BaseInfo.Persons.Edit")]
        [HttpGet]
        public async Task<IActionResult> DeleteExtraInfo(Guid id)
        {
            var entity = await _service.GetExtraInfoByIdAsync(id);
            if (entity == null) return NotFound();
            return PartialView("_DeleteExtraInfoConfirm", entity);
        }

        [Authorize(Policy = "BaseInfo.Persons.Edit")]
        [HttpGet]
        public async Task<IActionResult> DeleteBankAccount(Guid id)
        {
            var entity = await _service.GetBankAccountByIdAsync(id);
            if (entity == null) return NotFound();
            return PartialView("_DeleteBankAccountConfirm", entity);
        }

        [Authorize(Policy = "BaseInfo.Persons.Edit")]
        [HttpGet]
        public async Task<IActionResult> DeleteBranch(Guid id)
        {
            var entity = await _service.GetBranchByIdAsync(id);
            if (entity == null) return NotFound();
            return PartialView("_DeleteBranchConfirm", entity);
        }



        [Authorize(Policy = "BaseInfo.Persons.Edit")]
        [HttpPost]
        public async Task<IActionResult> Delete2(Guid id)
        {
            await _service.DeleteAsync(id);
            Response.Headers.Append("HX-Trigger", "closeModal, refreshList, showToast");
            return Content("");
        }

        [Authorize(Policy = "BaseInfo.Persons.Edit")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(Guid id)
        {
            var (success, error) = await _service.DeleteAsync(id);

            if (!success)
            {
                HxTrigger(new
                {
                    GridChanged = false,
                    toastError = $"{error}"
                });
                return Content("");
            }

            HxTrigger(new
            {
                GridChanged = false,
                toastSuccess = $"Deleted Successfully"
            });

            return Content("");
        }

        [Authorize(Policy = "BaseInfo.Persons.Edit")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteContactConfirmed(Guid id)
        {
            var (success, error) = await _service.DeleteContactAsync(id);

            if (!success)
            {
                HxTrigger(new
                {
                    GridChangedOnAppModal = true,
                    showToast = new { message = error, type = "danger" }
                });
                return Content("");
            }

            HxTrigger(new
            {
                GridChangedOnAppModal = true,
                toastSuccess = $"Deleted Successfully"
            });

            return Content("");
        }

        [Authorize(Policy = "BaseInfo.Persons.Edit")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteBankAccountConfirmed(Guid id)
        {
            var (success, error) = await _service.DeleteBankAccountAsync(id);

            if (!success)
            {
                HxTrigger(new
                {
                    GridChangedOnAppModal = true,
                    showToast = new { message = error, type = "danger" }
                });
                return Content("");
            }

            HxTrigger(new
            {
                GridChangedOnAppModal = true,
                toastSuccess = $"Deleted Successfully"
            });

            return Content("");
        }
        [Authorize(Policy = "BaseInfo.Persons.Edit")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteBranchConfirmed(Guid id)
        {
            var (success, error) = await _service.DeleteBranchAsync(id);

            if (!success)
            {
                HxTrigger(new
                {
                    GridChangedOnAppModal = true,
                    showToast = new { message = error, type = "danger" }
                });
                return Content("");
            }

            HxTrigger(new
            {
                GridChangedOnAppModal = true,
                toastSuccess = $"Deleted Successfully"
            });

            return Content("");
        }


        [Authorize(Policy = "BaseInfo.Persons.Edit")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteExtraInfoConfirmed(Guid id)
        {
            var (success, error) = await _service.DeleteExtraInfoAsync(id);

            if (!success)
            {
                HxTrigger(new
                {
                    GridChangedOnAppModal = true,
                    showToast = new { message = error, type = "danger" }
                });
                return Content("");
            }

            HxTrigger(new
            {
                GridChangedOnAppModal = true,
                toastSuccess = $"Deleted Successfully"
            });

            return Content("");
        }

        public async Task<IActionResult> CheckCode(string code, int? id)
        {
            if (string.IsNullOrWhiteSpace(code))
                return Content("");

            //var isUnique = await _service.IsCodeUniqueAsync(code.Trim().ToUpper(), id);
            //if (isUnique)
            //    return Content("<span class=\"text-success small\"><i class=\"bi bi-check-circle\"></i> Available</span>");
            else
                return Content("<span class=\"text-danger small\"><i class=\"bi bi-x-circle\"></i>  Should not be empty</span>");
        }

        [Authorize(Policy = "BaseInfo.Persons.Edit")]
        [HttpPost]
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
                refreshPersonsList = new { },
                showToast = new { message = "Status toggled.", type = "info" }
            });

            return Content("");
        }

        // نمایش عکس
        [HttpGet]
        public async Task<IActionResult> GetPicturePartial(Guid personId, PersonPictureType pictureType)
        {
            var latestPic = await _context.PersonPics
                .Where(p => p.PersonId == personId)
                .OrderByDescending(p => p.CreatedAt)
                .FirstOrDefaultAsync();

            var model = new PersonPictureViewModel
            {
                PersonId = personId,
                HasPicture = latestPic != null,
                PictureType = pictureType
                //PictureId = latestPic?.Id ?? Guid.Empty
            };

            return PartialView("_PersonPicture", model);
        }

        // آپلود عکس
        [HttpPost]
        public async Task<IActionResult> UploadPicture(Guid personId, IFormFile file)
        {
            if (file == null || file.Length == 0)
                return BadRequest("فایلی انتخاب نشده");

            using var ms = new MemoryStream();
            await file.CopyToAsync(ms);



            var pic = new PersonPic
            {
                Id = Guid.CreateVersion7(),
                PersonId = personId,
                Pic = ms.ToArray(),
                CreatedAt = DateTime.Now,
                UpdatedAt = DateTime.Now
            };

            _context.PersonPics.Add(pic);
            await _context.SaveChangesAsync();

            return Ok(new { success = true, id = pic.Id });
        }
        [HttpGet]
        public async Task<IActionResult> GetPicture(Guid personId, PersonPictureType pictureType)
        {
            var personPicture = await _context.PersonPics
                .AsNoTracking()
                .SingleOrDefaultAsync(item => item.PersonId == personId);

            if (personPicture is null)
                return NotFound();

            var image = pictureType == PersonPictureType.Latin
                ? personPicture.Pic
                : personPicture.LocalPic;

            return image is { Length: > 0 }
                ? File(image, "image/*")
                : NotFound();
        }
        public Guid GetLastPictureId(Guid personid)
        {
            if (_context.PersonPics.Where(x => x.PersonId == personid).Any())
            {
                return _context.PersonPics.Where(x => x.PersonId == personid).FirstOrDefault().Id;
            }
            return Guid.Empty;
        }

        [HttpGet]
        public async Task<IActionResult> GetLocationsByCountry(int? countryId)
        {
            if (!countryId.HasValue || countryId == 0)
            {
                return Content("<option value=\"\">-- Select Location --</option>", "text/html");
            }

            var locations = await _locationService.GetLocationsByCountryIdAsync(countryId.Value);

            var options = new StringBuilder();
            options.Append("<option value=\"\">-- Select Location --</option>");

            foreach (var loc in locations)
            {
                options.Append($"<option value=\"{loc.Id}\">{loc.LocationName} ({loc.LocationCode})</option>");
            }
            return Content(options.ToString(), "text/html");
        }


        [HttpGet]
        public async Task<IActionResult> GetPersonsJson(string? q, int take = 20)
        {
            var persons = await _service.SearchPersonsAsync(q, take);
            var results = persons.Select(p => new
            {
                id = p.Value,
                text = p.Text
            });
            return Json(results);
        }








    }
}