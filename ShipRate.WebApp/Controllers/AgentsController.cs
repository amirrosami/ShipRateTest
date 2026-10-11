using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using ShipRate.Application.DTOs;
using ShipRate.Application.Interfaces;
using ShipRate.Domain.Entities.Agents;
using ShipRate.Infrastructure.Data;
using ShipRate.WebApp.ViewModels.AgentVms;
using System.Text.Json;

namespace ShipRate.WebApp.Controllers
{
    [Authorize]

    public class AgentsController : Controller
    {
        private readonly ApplicationDbContext _db;
        public ICurrentUserService _currentUserService { get; set; }

        public AgentsController(ApplicationDbContext db, ICurrentUserService currentUserService)
        {
            _db = db;
            _currentUserService = currentUserService;
        }

        private bool IsHtmx => Request.Headers.ContainsKey("HX-Request");
        private void HxTrigger(object triggers)
        {
            Response.Headers["HX-Trigger"] = JsonSerializer.Serialize(triggers);
        }

        [Authorize("BaseOperations.AgentsManagement.View")]
        [HttpGet]
        public async Task<IActionResult> Index()
        {
            var vm = new AgentSearchViewModel();
            return View(vm);
        }

        [Authorize("BaseOperations.AgentsManagement.View")]
        [HttpGet]
        public async Task<IActionResult> List(AgentSearchViewModel search)
        {
            if (search.Page < 1) search.Page = 1;
            if (search.PageSize < 5) search.PageSize = 10;

            var query = _db.Agents
                .Include(c => c.Person)
                .Include(c => c.AgentActScopes.Where(ct => !ct.IsDeleted))
                    .ThenInclude(ct => ct.AgentScope)
                .Where(c => !c.IsDeleted)
                .AsQueryable();

            // ── Filters ──
            if (!string.IsNullOrWhiteSpace(search.PersonName))
                query = query.Where(c => c.Person.PersonName.Contains(search.PersonName));

            if (search.Status == "true")
                query = query.Where(c => c.IsActive);
            else if (search.Status == "false")
                query = query.Where(c => !c.IsActive);


            // ── Sort + Paginate ──
            var sorted = query.OrderByDescending(c => c.CreatedAt)
                .Select(c => new AgentListViewModel
                {
                    Id = c.Id,
                    PersonName = c.Person.PersonName,
                    IsActive = c.IsActive,
                    ScopeNames = c.AgentActScopes
                        .Where(ct => !ct.IsDeleted)
                        .OrderBy(x => x.Id)
                        .Select(ct => ct.AgentScope.AgentScopeName)
                        .ToList()
                });

            var paginatedList = await PaginatedList<AgentListViewModel>
                .CreateAsync(sorted, search.Page, search.PageSize);

            // ── اطلاعات جستجو رو برای ساخت لینک‌های صفحه‌بندی پاس بده ──
            ViewBag.Search = search;

            return PartialView("_List", paginatedList);
        }
        [Authorize("BaseOperations.AgentsManagement.Edit")]
        [HttpGet]
        public async Task<IActionResult> Edit(Guid id)
        {



            var agent = await _db.Agents
                .Include(c => c.Person)
                .FirstOrDefaultAsync(c => c.Id == id);

            if (agent == null) return NotFound();

            // فعلی‌های انتخاب شده
            var selectedScopeIds = await _db.AgentActScopes
                .Where(ct => ct.AgentId == id && !ct.IsDeleted)
                .Select(ct => ct.AgentScopeId)
                .ToListAsync();

            var vm = new AgentEditViewModel
            {
                Id = agent.Id,
                PersonName = agent.Person.PersonName,
                IsActive = agent.IsActive,
                SelectedScopeIds = selectedScopeIds,

                Scopes = await _db.AgentScopes
                    .OrderBy(t => t.Id)
                    .Select(t => new ScopeDropdownItem { Id = t.Id, Name = t.AgentScopeName })
                    .ToListAsync()
            };

            return PartialView("_EditForm", vm);
        }

        [Authorize("BaseOperations.AgentsManagement.Edit")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(AgentEditViewModel vm)
        {
            if (!ModelState.IsValid)
            {

                vm.Scopes = await _db.AgentScopes
                    .OrderBy(t => t.AgentScopeName)
                    .Select(t => new ScopeDropdownItem { Id = t.Id, Name = t.AgentScopeName })
                    .ToListAsync();

                var person = await _db.Persons.FindAsync(vm.Id);
                vm.PersonName = person?.PersonName ?? "";

                Response.StatusCode = 422;
                return PartialView("_EditForm", vm);
            }


            var agent = await _db.Agents.FindAsync(vm.Id);
            if (agent == null) return NotFound();

            // ── Update Agent fields ──
            agent.IsActive = vm.IsActive;
            agent.UpdatedAt = DateTime.Now;
            agent.EditUserId = _currentUserService.UserId;

            // ══════════════════════════════════════════════
            //  Sync AgentTradeTypes
            // ══════════════════════════════════════════════
            var existingScopes = await _db.AgentActScopes
                .Where(ct => ct.AgentId == vm.Id)
                .ToListAsync();

            var selectedIds = vm.SelectedScopeIds ?? new List<int>();

            // 1) آیتم‌هایی که قبلاً بوده ولی الان انتخاب نشده → Soft Delete
            foreach (var existing in existingScopes)
            {
                if (!selectedIds.Contains(existing.AgentScopeId))
                {
                    existing.IsDeleted = true;
                    existing.UpdatedAt = DateTime.Now;
                    // existing.EditUserId = currentUserId;
                }
                else
                {
                    // 2) آیتم‌هایی که قبلاً بوده و هنوز انتخابه → Restore if deleted
                    existing.IsDeleted = false;
                    existing.UpdatedAt = DateTime.Now;
                }
            }

            // 3) آیتم‌های جدیدی که قبلاً وجود نداشته → Add
            var existingScopeIds = existingScopes.Select(e => e.AgentScopeId).ToList();
            foreach (var newScopeId in selectedIds.Where(id => !existingScopeIds.Contains(id)))
            {
                _db.AgentActScopes.Add(new AgentActScope
                {
                    Id = Guid.CreateVersion7(),
                    AgentId = vm.Id,
                    AgentScopeId = newScopeId,


                    IsDeleted = false,
                    CreatedAt = DateTime.Now,
                    // CreateUserId = currentUserId
                });
            }

            await _db.SaveChangesAsync();

            //Response.Headers["HX-Trigger"] = """{"closeModal":"","showToast":{"message":"Agent updated successfully","type":"success"},"refreshAgentsList":""}""";

            HxTrigger(new
            {
                //showToast = new { message = "Agent updated successfully.", type = "success" },
                closeModalRefreshAgents = new { message = "Agent updated successfully.", type = "success" }
            });



            return Content("");
        }
        [Authorize("BaseOperations.AgentsManagement.Edit")]
        [HttpGet]
        public async Task<IActionResult> Delete(Guid id)
        {
            var agent = await _db.Agents
                .Include(c => c.Person)
                .FirstOrDefaultAsync(c => c.Id == id);

            if (agent == null) return NotFound();

            return PartialView("_DeleteConfirm", id);
        }

        [Authorize("BaseOperations.AgentsManagement.Edit")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(Guid id)
        {
            var agent = await _db.Agents.FindAsync(id);
            if (agent == null) return NotFound();

            agent.IsDeleted = true;
            agent.UpdatedAt = DateTime.Now;

            await _db.SaveChangesAsync();

            Response.Headers["HX-Trigger"] = "closeSecondModalRefreshAgents";
            return Content("");
        }

        [Authorize("BaseOperations.AgentsManagement.View")]
        public IActionResult RolesList(Guid agentId)
        {
            var roles = _db.AgentRoles
                .Include(x => x.Carrier)
                .ThenInclude(x => x.Person)
                .Where(x => x.AgentId == agentId && !x.IsDeleted)
                .Select(x => new AgentRoleRowVm
                {
                    Id = x.Id,
                    CarrierName = x.Carrier.Person.PersonName,
                    AgentRoleTypeName = x.AgentRoleType.AgentRoleTypeName,
                    CommissionRatePercent = x.CommissionRatePercent,
                }).ToList();

            var vm = new AgentRolesListViewModel
            {
                AgentId = agentId,
                AgentName = _db.Agents
                    .Include(x => x.Person)
                    .Where(x => x.Id == agentId)
                    .Select(x => x.Person.PersonName)
                    .FirstOrDefault(),
                Items = roles
            };

            return PartialView("_AgentRolesList", vm);
        }


        [Authorize("BaseOperations.AgentsManagement.View")]
        public IActionResult TerritoriesList(Guid agentId)
        {
            var terrs = _db.AgentTerritories
                .Include(x => x.Territory)
                .Where(x => x.AgentId == agentId && !x.IsDeleted)
                .Select(x => new AgentTerritoryRowVm
                {
                    Id = x.Id,
                    TerritoryName = x.Territory.TerritoryName,
                }).ToList();

            var vm = new AgentTerritoriesListViewModel
            {
                AgentId = agentId,
                AgentName = _db.Agents
                    .Include(x => x.Person)
                    .Where(x => x.Id == agentId)
                    .Select(x => x.Person.PersonName)
                    .FirstOrDefault(),
                Items = terrs
            };

            return PartialView("_AgentTerritoriesList", vm);
        }






        [Authorize("BaseOperations.AgentsManagement.Edit")]
        [HttpGet]
        public IActionResult CreateRole(Guid agentId)
        {
            var model = new AgentRoleFormViewModel
            {
                AgentId = agentId,
                IsActive = true,
                Carriers = new SelectList(_db.Carriers.Include(x => x.Person).Where(c => !c.IsDeleted), "Id", "Person.PersonName"),
                RoleTypes = new SelectList(_db.AgentRoleTypes.Where(rt => rt.IsActive && !rt.IsDeleted), "Id", "AgentRoleTypeName")
            };

            return PartialView("_AgentRoleForm", model);
        }

        [Authorize("BaseOperations.AgentsManagement.Edit")]
        [HttpGet]
        public IActionResult CreateTerritory(Guid agentId)
        {
            var model = new AgentTerritoryFormViewModel
            {
                AgentId = agentId,
                IsActive = true,
                Territories = new SelectList(_db.Territories.Where(c => !c.IsDeleted), "Id", "TerritoryName"),
            };

            return PartialView("_AgentTerritoryForm", model);
        }





        [HttpPost]
        [Authorize("BaseOperations.AgentsManagement.Edit")]
        public IActionResult CreateRole(AgentRoleFormViewModel vm)
        {
            if (!ModelState.IsValid) return PartialView("_AgentRoleForm", vm);

            var entity = new AgentRole
            {
                AgentId = vm.AgentId,
                CarrierId = vm.CarrierId.Value,
                AgentRoleTypeId = vm.AgentRoleTypeId.Value,
                CommissionRatePercent = vm.CommissionRatePercent.Value,
                IsDeleted = false
            };

            _db.AgentRoles.Add(entity);
            _db.SaveChanges();

            return Content("");
        }

        [HttpPost]
        [Authorize("BaseOperations.AgentsManagement.Edit")]
        public IActionResult CreateTerritory(AgentTerritoryFormViewModel vm)
        {
            if (!ModelState.IsValid) return PartialView("_AgentTerritoryForm", vm);

            var entity = new AgentTerritory
            {
                AgentId = vm.AgentId,
                TerritoryId = vm.TerritoryId,
                IsDeleted = false
            };

            _db.AgentTerritories.Add(entity);
            _db.SaveChanges();

            return Content("");
        }
        [HttpGet]
        [Authorize("BaseOperations.AgentsManagement.Edit")]
        public IActionResult EditRole(Guid id)
        {
            var entity = _db.AgentRoles.FirstOrDefault(x => x.Id == id && !x.IsDeleted);
            if (entity == null) return NotFound();

            var vm = new AgentRoleFormViewModel
            {
                Id = entity.Id,
                AgentId = entity.AgentId,
                CarrierId = entity.CarrierId,
                AgentRoleTypeId = entity.AgentRoleTypeId,
                CommissionRatePercent = entity.CommissionRatePercent,
                Carriers = new SelectList(_db.Carriers.Include(x => x.Person).Where(c => !c.IsDeleted), "Id", "Person.PersonName"),
                RoleTypes = new SelectList(_db.AgentRoleTypes.Where(rt => rt.IsActive && !rt.IsDeleted), "Id", "AgentRoleTypeName")
            };

            return PartialView("_AgentRoleForm", vm);
        }

        [HttpGet]
        [Authorize("BaseOperations.AgentsManagement.Edit")]
        public IActionResult EditTerritory(Guid id)
        {
            var entity = _db.AgentTerritories.FirstOrDefault(x => x.Id == id && !x.IsDeleted);
            if (entity == null) return NotFound();

            var vm = new AgentTerritoryFormViewModel
            {
                Id = entity.Id,
                AgentId = entity.AgentId,
                TerritoryId = entity.TerritoryId,
                Territories = new SelectList(_db.Territories.Where(rt => !rt.IsDeleted), "Id", "TerritoryName")
            };

            return PartialView("_AgentTerritoryForm", vm);
        }





        [Authorize("BaseOperations.AgentsManagement.Edit")]
        [HttpPost]
        public IActionResult EditRole(AgentRoleFormViewModel vm)
        {
            if (!ModelState.IsValid) return PartialView("_AgentRoleForm", vm);

            var entity = _db.AgentRoles.FirstOrDefault(x => x.Id == vm.Id);
            if (entity == null) return NotFound();

            entity.CarrierId = vm.CarrierId.Value;
            entity.AgentRoleTypeId = vm.AgentRoleTypeId.Value;
            entity.CommissionRatePercent = vm.CommissionRatePercent.Value;

            _db.SaveChanges();

            return Content("");
        }


        [HttpPost]
        [Authorize("BaseOperations.AgentsManagement.Edit")]
        public IActionResult EditTerritory(AgentTerritoryFormViewModel vm)
        {
            if (!ModelState.IsValid) return PartialView("_AgentTerritoryForm", vm);

            var entity = _db.AgentTerritories.FirstOrDefault(x => x.Id == vm.Id);
            if (entity == null) return NotFound();
            entity.TerritoryId = vm.TerritoryId;
            _db.SaveChanges();
            return Content("");
        }

        [HttpDelete]
        public IActionResult DeleteRole(Guid id)
        {
            var entity = _db.AgentRoles.FirstOrDefault(x => x.Id == id);
            if (entity == null) return NotFound();

            entity.IsDeleted = true;
            _db.SaveChanges();

            return Content("");
        }

        [HttpDelete]
        [Authorize("BaseOperations.AgentsManagement.Edit")]
        public IActionResult DeleteTerritory(Guid id)
        {
            var entity = _db.AgentTerritories.FirstOrDefault(x => x.Id == id);
            if (entity == null) return NotFound();

            entity.IsDeleted = true;
            _db.SaveChanges();
            return Content("");
        }

        [HttpGet]
        [Authorize("BaseOperations.AgentsManagement.Edit")]
        public async Task<IActionResult> ManageChargeTypes(Guid agentId)
        {
            var agent = await _db.Agents.Include(x => x.Person).FirstOrDefaultAsync(x => x.Id == agentId);
            if (agent == null)
                return NotFound();

            var allChargeTypes = await _db.ChargeTypes
                .Include(x => x.ChargeTypeCategory)
                .Where(ct => !ct.IsDeleted)
                .OrderBy(ct => ct.ChargeTypeCategoryId)
                .ThenBy(ct => ct.Id)
                .ToListAsync();

            var selectedChargeTypeIds = await _db.AgentChargeTypes
                .Where(act => act.AgentId == agentId && !act.IsDeleted /*&& act.IsActive*/)
                .Select(act => act.ChargeTypeId)
                .ToListAsync();

            var groupedChargeTypes = allChargeTypes
                .GroupBy(ct => ct.ChargeTypeCategoryId)
                .ToDictionary(
                    g => g.Key.ToString(),
                    g => g.Select(ct => new ChargeTypeItem
                    {
                        ChargeTypeId = ct.Id,
                        ChargeTypeName = ct.ChargeTypeName,
                        IsSelected = selectedChargeTypeIds.Contains(ct.Id)
                    }).ToList()
                );
            var vm = new AgentChargeTypeViewModel
            {
                AgentId = agentId,
                AgentName = agent.Person?.PersonName,
                ChargeTypesByCategory = groupedChargeTypes
            };

            return PartialView("_ManageChargeTypesModal", vm);
        }

        [HttpPost]
        [Authorize("BaseOperations.AgentsManagement.Edit")]
        public async Task<IActionResult> SaveChargeTypes([FromForm] SaveAgentChargeTypesRequest request)
        {
            var existing = await _db.AgentChargeTypes
                .Where(x => x.AgentId == request.AgentId && !x.IsDeleted)
                .ToListAsync();

            var existingIds = existing.Select(x => x.ChargeTypeId).ToList();
            var toRemove = existingIds;


            if (request.SelectedChargeTypeIds is not null && request.SelectedChargeTypeIds.Count > 0)
            {
                var toAdd = request.SelectedChargeTypeIds.Except(existingIds).ToList();
                toRemove = existingIds.Except(request.SelectedChargeTypeIds).ToList();
                foreach (var chargeTypeId in toAdd)
                {
                    _db.AgentChargeTypes.Add(new AgentChargeType
                    {
                        Id = Guid.CreateVersion7(),
                        AgentId = request.AgentId,
                        ChargeTypeId = chargeTypeId,
                        IsDeleted = false,
                        CreatedAt = DateTime.Now,
                        CreateUserId = Guid.Empty
                    });
                }
            }

            foreach (var chargeTypeId in toRemove)
            {
                var item = existing.First(x => x.ChargeTypeId == chargeTypeId);
                item.IsDeleted = true;
                item.UpdatedAt = DateTime.Now;
            }

            await _db.SaveChangesAsync();

            return Content("<script>$('.modal-content').modal('hide');</script>", "text/html");
        }

        [Authorize("BaseOperations.AgentsManagement.Activation")]
        [HttpPost]
        public async Task<IActionResult> ToggleActive(Guid id)
        {
            var agent = await _db.Agents.FindAsync(id);
            if (agent == null || agent.IsDeleted)
                return NotFound();
            // سوئیچ وضعیت
            agent.IsActive = !agent.IsActive;
            agent.UpdatedAt = DateTime.Now;
            agent.EditUserId = _currentUserService.UserId;
            await _db.SaveChangesAsync();
            // تریگر کردن ایونت برای رفرش لیست ایجنت‌ها
            // (از همان ایونتی که در صفحه اصلی برای رفرش لیست استفاده می‌کنید مثل refreshAgentsList استفاده کنید)
            HxTrigger(new
            {
                refreshAgentsList = "",
                showToast = new
                {
                    message = $"Agent status changed to {(agent.IsActive ? "Active" : "Inactive")}.",
                    type = "success"
                }
            });
            return Ok();
        }
    }
}