using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ShipRate.Application.Interfaces;
using ShipRate.Application.ViewModels;

namespace ShipRate.WebApp.Controllers
{
    [Authorize]

    public class TariffBooksController : Controller
    {
        private readonly ITariffService _tariffService;
        private readonly ICurrentUserService _currentUserService;

        public TariffBooksController(ITariffService tariffService, ICurrentUserService currentUserService)
        {
            _tariffService = tariffService;
            _currentUserService = currentUserService;
        }

        [HttpGet]
        [Authorize("BaseOperations.Tariffs.View")]
        public async Task<IActionResult> Index(
            [FromQuery] TariffBookSearchVm search,
            CancellationToken cancellationToken)
        {
            var model = await _tariffService.GetIndexVmAsync(search, cancellationToken);
            return View(model);
        }

        [HttpGet]
        [Authorize("BaseOperations.Tariffs.View")]
        public async Task<IActionResult> Grid(
            [FromQuery] TariffBookSearchVm search,
            CancellationToken cancellationToken)
        {

            if (search.TariffTypeId <= 0)
                search.TariffTypeId = null;

            var result = await _tariffService.GetTariffBooksAsync(search, cancellationToken);
            return PartialView("_TariffBookGrid", result);
        }

        [HttpGet]
        [Authorize("BaseOperations.Tariffs.View")]
        public async Task<IActionResult> RulesGrid(
            Guid tariffBookId,
            int page = 1,
            int pageSize = 1000,
            CancellationToken cancellationToken = default)
        {
            if (tariffBookId == Guid.Empty)
            {
                return PartialView("_TariffRuleGrid", null);
            }

            var result = await _tariffService.GetTariffRulesAsync(
                tariffBookId,
                page,
                pageSize,
                cancellationToken);

            ViewBag.TariffBookId = tariffBookId;
            ViewBag.TariffTypeId = await _tariffService.GetTariffTypeIdAsync(tariffBookId, cancellationToken);
            ViewBag.TariffTitle = await _tariffService.GetTariffTitleAsync(tariffBookId, cancellationToken);
            return PartialView("_TariffRuleGrid", result);
        }

        [HttpGet]
        [Authorize("BaseOperations.Tariffs.View")]
        public async Task<IActionResult> CoveragesGrid(
            Guid tariffRuleId,
            CancellationToken cancellationToken)
        {
            if (tariffRuleId == Guid.Empty)
            {
                return PartialView("_TariffRuleCoverageGrid", new List<TariffRuleCoverageListItemVm>());
            }

            var canShow = await _tariffService.RuleCanShowCoveragesAsync(
                tariffRuleId,
                cancellationToken);

            if (!canShow)
            {
                return PartialView("_NoCoverageGrid");
            }

            var result = await _tariffService.GetTariffRuleCoveragesAsync(
                tariffRuleId,
                cancellationToken);

            ViewBag.TariffRuleId = tariffRuleId;

            return PartialView("_TariffRuleCoverageGrid", result);
        }

        [HttpGet]
        [Authorize("BaseOperations.Tariffs.Edit")]
        public async Task<IActionResult> Create(int SelectedTariffTypeId,
            CancellationToken cancellationToken)
        {
            var model = await _tariffService.GetCreateTariffBookVmAsync(cancellationToken);
            model.TariffTypeId = SelectedTariffTypeId;

            return PartialView("_TariffBookForm", model);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize("BaseOperations.Tariffs.Edit")]
        public async Task<IActionResult> Create(
            TariffBookFormVm model,
            CancellationToken cancellationToken)
        {
            if (!ModelState.IsValid)
            {
                var vm = await _tariffService.GetCreateTariffBookVmAsync(cancellationToken);

                model.TariffTypes = vm.TariffTypes;
                model.Statuses = vm.Statuses;
                model.Currencies = vm.Currencies;

                return PartialView("_TariffBookForm", model);
            }

            model.StatusId = -1;
            await _tariffService.CreateTariffBookAsync(model, cancellationToken);

            return PartialView("_HtmxModalSuccess", new HtmxModalSuccessVm
            {
                RefreshTarget = "#tariffBooksGrid",
                RefreshUrl = Url.Action(nameof(Grid), "TariffBooks")!
            });
        }

        [HttpGet]
        [Authorize("BaseOperations.Tariffs.Edit")]
        public async Task<IActionResult> Edit(
            Guid id,
            CancellationToken cancellationToken)
        {
            var model = await _tariffService.GetEditTariffBookVmAsync(id, cancellationToken);

            if (model == null)
                return NotFound();

            return PartialView("_TariffBookForm", model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize("BaseOperations.Tariffs.Edit")]
        public async Task<IActionResult> Edit(
            TariffBookFormVm model,
            CancellationToken cancellationToken)
        {
            if (!ModelState.IsValid)
            {
                if (model.Id.HasValue)
                {
                    var vm = await _tariffService.GetEditTariffBookVmAsync(
                        model.Id.Value,
                        cancellationToken);

                    if (vm != null)
                    {
                        model.TariffTypes = vm.TariffTypes;
                        model.Statuses = vm.Statuses;
                        model.Currencies = vm.Currencies;
                    }
                }

                return PartialView("_TariffBookForm", model);
            }

            var updated = await _tariffService.UpdateTariffBookAsync(model, cancellationToken);

            if (!updated)
                return NotFound();

            return PartialView("_HtmxModalSuccess", new HtmxModalSuccessVm
            {
                RefreshTarget = "#tariffBooksGrid",
                RefreshUrl = Url.Action(nameof(Grid), "TariffBooks")!
            });
        }

        [HttpGet]
        [Authorize("BaseOperations.Tariffs.Edit")]
        public IActionResult Delete(Guid id)
        {
            var model = new DeleteConfirmVm
            {
                Id = id,
                Title = "Delete Tariff Book",
                Message = "Are you sure you want to delete this tariff book?",
                PostUrl = Url.Action(nameof(DeleteConfirmed), "TariffBooks", new { id })!,
                RefreshTarget = "#tariffBooksGrid",
                RefreshUrl = Url.Action(nameof(Grid), "TariffBooks")!
            };

            return PartialView("_DeleteConfirm", model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize("BaseOperations.Tariffs.Edit")]
        public async Task<IActionResult> DeleteConfirmed(
            Guid id,
            CancellationToken cancellationToken)
        {
            var deleted = await _tariffService.SoftDeleteTariffBookAsync(id, cancellationToken);

            if (!deleted)
                return NotFound();

            return PartialView("_HtmxModalSuccess", new HtmxModalSuccessVm
            {
                RefreshTarget = "#tariffBooksGrid",
                RefreshUrl = Url.Action(nameof(Grid), "TariffBooks")!,
                ClearTargets = new[]
                {
                "#tariffRulesGrid",
                "#tariffRuleCoveragesGrid"
            }
            });
        }

        [HttpGet]
        [Authorize("BaseOperations.Tariffs.Edit")]
        public async Task<IActionResult> CreateRule(
            Guid tariffBookId,
            int TariffTypeId,
            CancellationToken cancellationToken)
        {
            if (tariffBookId == Guid.Empty)
                return BadRequest();

            var model = await _tariffService.GetCreateTariffRuleVmAsync(
                TariffTypeId,
                tariffBookId,
                cancellationToken);

            return PartialView("_TariffRuleForm", model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize("BaseOperations.Tariffs.Edit")]
        public async Task<IActionResult> CreateRule(int tariffTypeId,
            TariffRuleFormVm model,
            CancellationToken cancellationToken)
        {
            if (model.CalculationById == 1)
            {
                if (!model.ContainerSizeTypeId.HasValue)
                {
                    ModelState.AddModelError(nameof(model.ContainerSizeId), "Container Size Type is required.");
                }
                model.ContainerSizeId = null;
            }
            else if (model.CalculationById == 2)
            {
                if (!model.ContainerSizeId.HasValue)
                {
                    ModelState.AddModelError(nameof(model.ContainerSizeId), "Container Size is required.");
                }
                model.ContainerSizeTypeId = null;
            }
            else
            {
                model.ContainerSizeId = null;
                model.ContainerSizeTypeId = null;
            }

            if (!ModelState.IsValid)
            {
                var vm = await _tariffService.GetCreateTariffRuleVmAsync(
                    tariffTypeId,
                    model.TariffBookId,
                    cancellationToken);

                model.ChargeTypes = vm.ChargeTypes;
                model.CalculationByItems = vm.CalculationByItems;
                model.CalculationTypes = vm.CalculationTypes;
                model.Currencies = vm.Currencies;

                return PartialView("_TariffRuleForm", model);
            }




            await _tariffService.CreateTariffRuleAsync(model, cancellationToken);
            return NoContent();

            //return PartialView("_HtmxModalSuccess", new HtmxModalSuccessVm
            //{
            //    RefreshTarget = "#tariffRulesGrid",
            //    RefreshUrl = Url.Action(
            //        nameof(RulesGrid),
            //        "TariffBooks",
            //        new
            //        {
            //            tariffBookId = model.TariffBookId
            //        })!,
            //    ClearTargets = new[]
            //    {
            //    "#tariffRuleCoveragesGrid"
            //}
            //});

        }

        // ============================================================
        // TariffRule Edit
        // ============================================================

        [HttpGet]
        [Authorize("BaseOperations.Tariffs.Edit")]
        public async Task<IActionResult> EditRule(int tariffTypeId,
            Guid id,
            CancellationToken cancellationToken)
        {
            var model = await _tariffService.GetEditTariffRuleVmAsync(tariffTypeId, id, cancellationToken);

            if (model == null)
                return NotFound();

            return PartialView("_TariffRuleForm", model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize("BaseOperations.Tariffs.Edit")]
        public async Task<IActionResult> EditRule(int tariffTypeId,
            TariffRuleFormVm model,
            CancellationToken cancellationToken)
        {
            if (model.CalculationById == 1)
            {
                if (!model.ContainerSizeTypeId.HasValue)
                {
                    ModelState.AddModelError(nameof(model.ContainerSizeId), "Container Size Type is required.");
                }
                model.ContainerSizeId = null;
            }
            else if (model.CalculationById == 2)
            {
                if (!model.ContainerSizeId.HasValue)
                {
                    ModelState.AddModelError(nameof(model.ContainerSizeId), "Container Size is required.");
                }
                model.ContainerSizeTypeId = null;
            }
            else
            {
                model.ContainerSizeId = null;
                model.ContainerSizeTypeId = null;
            }





            if (!ModelState.IsValid)
            {
                if (model.Id.HasValue)
                {
                    var vm = await _tariffService.GetEditTariffRuleVmAsync(
                        tariffTypeId,
                        model.Id.Value,
                        cancellationToken);

                    if (vm != null)
                    {
                        model.ChargeTypes = vm.ChargeTypes;
                        model.CalculationByItems = vm.CalculationByItems;
                        model.CalculationTypes = vm.CalculationTypes;
                        model.Currencies = vm.Currencies;

                    }
                }

                return PartialView("_TariffRuleForm", model);
            }

            var updated = await _tariffService.UpdateTariffRuleAsync(model, cancellationToken);

            if (!updated)
                return NotFound();

            return PartialView("_HtmxModalSuccess", new HtmxModalSuccessVm
            {
                RefreshTarget = "#tariffRulesGrid",
                RefreshUrl = Url.Action(
                    nameof(RulesGrid),
                    "TariffBooks",
                    new
                    {
                        tariffBookId = model.TariffBookId
                    })!,
                ClearTargets = new[]
                {
                "#tariffRuleCoveragesGrid"
            }
            });
        }

        [Authorize("BaseOperations.Tariffs.Edit")]
        [HttpGet]
        public IActionResult DeleteRule(
            Guid id,
            Guid tariffBookId)
        {
            var model = new DeleteConfirmVm
            {
                Id = id,
                Title = "Delete Tariff Rule",
                Message = "Are you sure you want to delete this tariff rule?",
                PostUrl = Url.Action(nameof(DeleteRuleConfirmed), "TariffBooks", new
                {
                    id,
                    tariffBookId
                })!,
                RefreshTarget = "#tariffRulesGrid",
                RefreshUrl = Url.Action(nameof(RulesGrid), "TariffBooks", new
                {
                    tariffBookId
                })!
            };

            return PartialView("_DeleteConfirm", model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize("BaseOperations.Tariffs.Edit")]
        public async Task<IActionResult> DeleteRuleConfirmed(
            Guid id,
            Guid tariffBookId,
            CancellationToken cancellationToken)
        {
            var deleted = await _tariffService.SoftDeleteTariffRuleAsync(id, cancellationToken);

            if (!deleted)
                return NotFound();

            return PartialView("_HtmxModalSuccess", new HtmxModalSuccessVm
            {
                RefreshTarget = "#tariffRulesGrid",
                RefreshUrl = Url.Action(nameof(RulesGrid), "TariffBooks", new
                {
                    tariffBookId
                })!,
                ClearTargets = new[]
                {
                "#tariffRuleCoveragesGrid"
            }
            });
        }

        [HttpGet]
        [Authorize("BaseOperations.Tariffs.Edit")]
        public async Task<IActionResult> CreateCoverage(
            Guid tariffRuleId,
            CancellationToken cancellationToken)
        {
            if (tariffRuleId == Guid.Empty)
                return BadRequest();

            var model = await _tariffService.GetCreateCoverageVmAsync(
                tariffRuleId,
                cancellationToken);

            return PartialView("_TariffRuleCoverageForm", model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize("BaseOperations.Tariffs.Edit")]
        public async Task<IActionResult> CreateCoverage(
            TariffRuleCoverageFormVm model,
            CancellationToken cancellationToken)
        {
            if (!ModelState.IsValid)
            {
                var vm = await _tariffService.GetCreateCoverageVmAsync(
                    model.TariffRuleId,
                    cancellationToken);

                model.Countries = vm.Countries;
                model.Locations = vm.Locations;

                return PartialView("_TariffRuleCoverageForm", model);
            }

            await _tariffService.CreateCoverageAsync(model, cancellationToken);

            return PartialView("_HtmxModalSuccess", new HtmxModalSuccessVm
            {
                RefreshTarget = "#tariffRuleCoveragesGrid",
                RefreshUrl = Url.Action(nameof(CoveragesGrid), "TariffBooks", new
                {
                    tariffRuleId = model.TariffRuleId
                })!
            });
        }

        [HttpGet]
        [Authorize("BaseOperations.Tariffs.Edit")]
        public async Task<IActionResult> EditCoverage(
            Guid id,
            CancellationToken cancellationToken)
        {
            var model = await _tariffService.GetEditCoverageVmAsync(id, cancellationToken);

            if (model == null)
                return NotFound();

            return PartialView("_TariffRuleCoverageForm", model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize("BaseOperations.Tariffs.Edit")]
        public async Task<IActionResult> EditCoverage(
            TariffRuleCoverageFormVm model,
            CancellationToken cancellationToken)
        {
            if (!ModelState.IsValid)
            {
                if (model.Id.HasValue)
                {
                    var vm = await _tariffService.GetEditCoverageVmAsync(
                        model.Id.Value,
                        cancellationToken);

                    if (vm != null)
                    {
                        model.Countries = vm.Countries;
                        model.Locations = vm.Locations;
                    }
                }

                return PartialView("_TariffRuleCoverageForm", model);
            }

            var updated = await _tariffService.UpdateCoverageAsync(model, cancellationToken);

            if (!updated)
                return NotFound();

            return PartialView("_HtmxModalSuccess", new HtmxModalSuccessVm
            {
                RefreshTarget = "#tariffRuleCoveragesGrid",
                RefreshUrl = Url.Action(nameof(CoveragesGrid), "TariffBooks", new
                {
                    tariffRuleId = model.TariffRuleId
                })!
            });
        }

        [HttpGet]
        [Authorize("BaseOperations.Tariffs.Edit")]
        public IActionResult DeleteCoverage(
            Guid id,
            Guid tariffRuleId)
        {
            var model = new DeleteConfirmVm
            {
                Id = id,
                Title = "Delete Coverage",
                Message = "Are you sure you want to delete this coverage?",
                PostUrl = Url.Action(nameof(DeleteCoverageConfirmed), "TariffBooks", new
                {
                    id,
                    tariffRuleId
                })!,
                RefreshTarget = "#tariffRuleCoveragesGrid",
                RefreshUrl = Url.Action(nameof(CoveragesGrid), "TariffBooks", new
                {
                    tariffRuleId
                })!
            };

            return PartialView("_DeleteConfirm", model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize("BaseOperations.Tariffs.Edit")]
        public async Task<IActionResult> DeleteCoverageConfirmed(
            Guid id,
            Guid tariffRuleId,
            CancellationToken cancellationToken)
        {
            var deleted = await _tariffService.SoftDeleteCoverageAsync(id, cancellationToken);

            if (!deleted)
                return NotFound();

            return PartialView("_HtmxModalSuccess", new HtmxModalSuccessVm
            {
                RefreshTarget = "#tariffRuleCoveragesGrid",
                RefreshUrl = Url.Action(nameof(CoveragesGrid), "TariffBooks", new
                {
                    tariffRuleId
                })!
            });
        }

        [HttpGet]
        public async Task<IActionResult> SearchParties(
            string? term,
            string targetHiddenInputId = "PartyId",
            string targetTextInputId = "PartySearchText",
            CancellationToken cancellationToken = default)
        {
            var items = await _tariffService.SearchPartiesAsync(term, cancellationToken);

            ViewBag.TargetHiddenInputId = targetHiddenInputId;
            ViewBag.TargetTextInputId = targetTextInputId;

            return PartialView("_PartySearchResults", items);
        }

        [HttpGet]
        [Authorize("BaseOperations.Tariffs.Edit")]
        public async Task<IActionResult> CreateDemurrageRules(Guid tariffBookId)
        {
            var vm = await _tariffService.BuildDemurrageRuleBatchFormAsync(tariffBookId);
            return PartialView("_DemurrageRuleBatchForm", vm);
        }

        [HttpGet]
        [Authorize("BaseOperations.Tariffs.Edit")]
        public async Task<IActionResult> DemurrageRulesGrid(Guid tariffBookId, int containerSizeTypeId)
        {
            var vm = await _tariffService.BuildDemurrageRuleBatchFormAsync(tariffBookId, containerSizeTypeId);

            if (!vm.Steps.Any())
            {
                vm.Steps.Add(new DemurrageRuleStepVm
                {
                    From = 1,
                    To = 9999,
                    Amount = 0
                });
            }

            return PartialView("_DemurrageRuleStepsGrid", vm);
        }

        [HttpPost]
        [Authorize("BaseOperations.Tariffs.Edit")]
        public async Task<IActionResult> AddDemurrageRuleRow(DemurrageRuleBatchFormVm vm)
        {
            vm.Steps ??= new();

            var nextFrom = vm.Steps.Any()
                ? vm.Steps.Max(x => x.To) + 1
                : 1;

            vm.Steps.Add(new DemurrageRuleStepVm
            {
                From = nextFrom,
                To = 9999,
                Amount = 0
            });

            return PartialView("_DemurrageRuleStepsGrid", vm);
        }

        [HttpPost]
        [Authorize("BaseOperations.Tariffs.Edit")]
        public IActionResult RemoveDemurrageRuleRow(DemurrageRuleBatchFormVm vm, int index)
        {
            vm.Steps ??= new();

            if (index >= 0 && index < vm.Steps.Count)
                vm.Steps.RemoveAt(index);

            return PartialView("_DemurrageRuleStepsGrid", vm);
        }

        [HttpPost]
        [Authorize("BaseOperations.Tariffs.Edit")]
        public async Task<IActionResult> SaveDemurrageRules(DemurrageRuleBatchFormVm vm)
        {
            var validationErrors = ValidateDemurrageSteps(vm);

            foreach (var error in validationErrors)
                ModelState.AddModelError(string.Empty, error);

            if (!ModelState.IsValid)
            {
                vm.ContainerSizeTypes = await _tariffService.GetContainerSizeTypesListAsync();
                return PartialView("_DemurrageRuleBatchForm", vm);
            }

            await _tariffService.ReplaceDemurrageRulesAsync(vm);

            return PartialView("_HtmxModalSuccess", new HtmxModalSuccessVm
            {
                RefreshTarget = "#tariffRulesGrid"
            });
        }

        private static List<string> ValidateDemurrageSteps(DemurrageRuleBatchFormVm vm)
        {
            var errors = new List<string>();

            if (!vm.ContainerSizeTypeId.HasValue)
                errors.Add("Container size type is required.");

            if (vm.Steps == null || !vm.Steps.Any())
            {
                errors.Add("At least one step is required.");
                return errors;
            }

            var ordered = vm.Steps.OrderBy(x => x.From).ToList();

            if (ordered[0].From != 1)
                errors.Add("The first step must start from day 1.");

            for (var i = 0; i < ordered.Count; i++)
            {
                var step = ordered[i];

                if (step.From < 1)
                    errors.Add("From day must be greater than or equal to 1.");

                if (step.To < step.From)
                    errors.Add("To day must be greater than or equal to From day.");

                if (i == 0)
                {
                    if (step.Amount < 0)
                        errors.Add("Amount in the first row cannot be negative.");
                }
                else
                {
                    if (step.Amount <= 0)
                        errors.Add("Amount after the first row must be greater than zero.");

                    var previous = ordered[i - 1];

                    if (step.From != previous.To + 1)
                        errors.Add("Steps must be continuous and non-overlapping.");
                }
            }

            if (ordered[^1].To != 9999)
                errors.Add("The last step must end at day 9999.");

            return errors.Distinct().ToList();
        }

        [HttpPost]
        [Authorize("BaseOperations.Tariffs.Edit")]
        public async Task<IActionResult> PasteDemurrageRulesGrid(DemurrageRuleBatchFormVm model, string? pastedStepsText)
        {
            var vm = await _tariffService.BuildDemurrageRuleBatchFormAsync(
                   model.TariffBookId,
                   model.ContainerSizeTypeId
               );
            vm.Steps = ParseDemurrageSteps(pastedStepsText);

            return PartialView("_DemurrageRuleStepsGrid", vm);
        }

        private static List<DemurrageRuleStepVm> ParseDemurrageSteps(string? text)
        {
            var result = new List<DemurrageRuleStepVm>();

            if (string.IsNullOrWhiteSpace(text))
                return result;

            var lines = text
                .Split(new[] { "\r\n", "\n", "\r" }, StringSplitOptions.RemoveEmptyEntries)
                .Select(x => x.Trim())
                .Where(x => !string.IsNullOrWhiteSpace(x))
                .ToList();

            foreach (var line in lines)
            {
                var parts = line
                    .Split(new[] { '\t', ',', ';', ' ' }, StringSplitOptions.RemoveEmptyEntries)
                    .Select(x => x.Trim())
                    .ToArray();

                if (parts.Length < 3)
                    continue;

                if (!decimal.TryParse(parts[0], out var fromDayDecimal))
                    continue;

                if (!decimal.TryParse(parts[1], out var toDayDecimal))
                    continue;

                if (!decimal.TryParse(parts[2], out var amount))
                    continue;

                result.Add(new DemurrageRuleStepVm
                {
                    From = (int)fromDayDecimal,
                    To = (int)toDayDecimal,
                    Amount = amount
                });
            }
            return result;
        }



        [HttpGet]
        [Authorize("BaseOperations.Tariffs.Edit")]
        public async Task<IActionResult> CreateOceanFreightRules(Guid tariffBookId)
        {
            var model = await _tariffService.BuildOceanFreightRuleBatchFormAsync(tariffBookId);

            return PartialView("_OceanFreightRuleBatchForm", model);
        }

        [HttpPost]
        [Authorize("BaseOperations.Tariffs.View")]
        public async Task<IActionResult> OceanFreightRulesGrid(OceanFreightRuleBatchFormVm model)
        {
            model = await _tariffService.BuildOceanFreightRuleBatchFormAsync(
                model.TariffBookId,
                model.OriginId,
                model.DestinationId,
                model.PartyId,
                model);

            return PartialView("_OceanFreightRateRowsGrid", model);
        }

        [HttpPost]
        [Authorize("BaseOperations.Tariffs.Edit")]
        public async Task<IActionResult> AddOceanFreightRuleRow(OceanFreightRuleBatchFormVm model)
        {
            model = await _tariffService.AddOceanFreightRuleRowAsync(model);

            return PartialView("_OceanFreightRateRowsGrid", model);
        }


        [HttpPost]
        [Authorize("BaseOperations.Tariffs.Edit")]
        public async Task<IActionResult> RemoveOceanFreightRuleRow(
            OceanFreightRuleBatchFormVm model,
            int rowIndex)
        {
            model = await _tariffService.RemoveOceanFreightRuleRowAsync(model, rowIndex);

            return PartialView("_OceanFreightRateRowsGrid", model);
        }

        [HttpPost]
        [Authorize("BaseOperations.Tariffs.Edit")]
        public async Task<IActionResult> PasteOceanFreightRuleRows(OceanFreightRuleBatchFormVm model)
        {
            model = await _tariffService.PasteOceanFreightRuleRowsAsync(model);

            return PartialView("_OceanFreightRateRowsGrid", model);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize("BaseOperations.Tariffs.Edit")]
        public async Task<IActionResult> SaveOceanFreightRules(OceanFreightRuleBatchFormVm model)
        {
            var currentUserId = _currentUserService.UserId;

            var result = await _tariffService.SaveOceanFreightRulesAsync(model, currentUserId);

            if (!result.Succeeded)
            {
                foreach (var error in result.Errors)
                {
                    foreach (var message in error.Value)
                    {
                        ModelState.AddModelError(error.Key, message);
                    }
                }

                model = await _tariffService.BuildOceanFreightRuleBatchFormAsync(
                    model.TariffBookId,
                    model.OriginId,
                    model.DestinationId,
                    model.PartyId,
                    model);

                return PartialView("_OceanFreightRuleBatchForm", model);
            }

            return Json(new
            {
                success = true,
                message = result.Message ?? "Ocean freight rates saved successfully."
            });
        }
        [HttpPost]
        [Authorize("BaseOperations.Tariffs.View")]
        public async Task<IActionResult> LoadOceanFreightRulesGrid(OceanFreightRuleBatchFormVm model)
        {
            model = await _tariffService.LoadOceanFreightRulesForHeaderAsync(model);

            return PartialView("_OceanFreightRateRowsGrid", model);
        }
        [HttpPost]
        [Authorize("BaseOperations.Tariffs.Edit")]
        public async Task<IActionResult> Activate(Guid tariffBookId, TariffBookSearchVm search)
        {
            var result = await _tariffService.ActivateTariffBookAsync(tariffBookId);
            if (!result)
            {
                Response.StatusCode = 400;
                return Content("Errors occured!!!!!!");
            }
            var model = await _tariffService.GetTariffBooksAsync(search);
            return PartialView("_TariffBookGrid", model);
        }

        //Local
        [HttpGet]
        [Authorize("BaseOperations.Tariffs.Edit")]
        public async Task<IActionResult> CreateLocalCntRules(Guid tariffBookId, CancellationToken cancellationToken)
        {
            if (tariffBookId == Guid.Empty)
                return BadRequest();

            var vm = await _tariffService.BuildLocalCntSaoiFormAsync(tariffBookId, cancellationToken);
            return PartialView("_TariffRuleCntForSaoi", vm);
        }

        [HttpPost]
        [Authorize("BaseOperations.Tariffs.Edit")]
        public async Task<IActionResult> SaveTariffRuleForSaoi(TariffRuleForSaoiVM tariffRule, CancellationToken cancellationToken)
        {
            if (tariffRule == null)
                return BadRequest();

            var vm = await _tariffService.SaveTariffRuleForSaoiAsync(tariffRule, cancellationToken);
            return PartialView("_HtmxModalSuccess", new HtmxModalSuccessVm
            {
                RefreshTarget = "#tariffRulesGrid",
                RefreshUrl = Url.Action(nameof(RulesGrid), "TariffBooks", new { tariffBookId = tariffRule.TariffBookId })!,
                ClearTargets = new[] { "#tariffRuleCoveragesGrid" }
            });
        }

        // ============================================================
        // THC Rules (TariffTypeId = 4)
        // ============================================================
        [HttpGet]
        [Authorize("BaseOperations.Tariffs.Edit")]
        public async Task<IActionResult> CreateThcRules(Guid tariffBookId, CancellationToken cancellationToken)
        {
            if (tariffBookId == Guid.Empty)
                return BadRequest();

            var vm = await _tariffService.BuildThcRuleBatchFormAsync(tariffBookId, cancellationToken);
            return PartialView("_ThcRuleBatchForm", vm);
        }

        [HttpPost]
        [Authorize("BaseOperations.Tariffs.Edit")]
        public async Task<IActionResult> AddThcRuleRow(TariffThcRuleBatchFormVm model, CancellationToken cancellationToken)
        {
            model.Rows ??= new();
            model.Rows.Add(new TariffThcRuleRowVm());

            // هر دو دراپ‌داون باید مجدداً لود شوند تا پارشال ویو رندر شود
            model.TradeTypes = await _tariffService.GetTradeTypesSelectListAsync(cancellationToken);
            model.ContainerSizeTypes = await _tariffService.GetContainerSizeTypesListAsync(cancellationToken);
            model.FullMTs = await _tariffService.GetFullMTsListAsync(cancellationToken);

            return PartialView("_ThcRateRowsGrid", model);
        }

        [HttpPost]
        [Authorize("BaseOperations.Tariffs.Edit")]
        public async Task<IActionResult> RemoveThcRuleRow(TariffThcRuleBatchFormVm model, int rowIndex, CancellationToken cancellationToken)
        {
            model.Rows ??= new();
            if (rowIndex >= 0 && rowIndex < model.Rows.Count)
            {
                model.Rows.RemoveAt(rowIndex);
            }

            // هر دو دراپ‌داون باید مجدداً لود شوند
            model.TradeTypes = await _tariffService.GetTradeTypesSelectListAsync(cancellationToken);
            model.ContainerSizeTypes = await _tariffService.GetContainerSizeTypesListAsync(cancellationToken);
            model.FullMTs = await _tariffService.GetFullMTsListAsync(cancellationToken);

            return PartialView("_ThcRateRowsGrid", model);
        }

        [HttpPost]
        [Authorize("BaseOperations.Tariffs.Edit")]
        public async Task<IActionResult> PasteThcRuleRows(TariffThcRuleBatchFormVm model, string? pastedText, CancellationToken cancellationToken)
        {
            var parsedRows = _tariffService.ParseThcRulePastedText(pastedText);
            model.Rows = parsedRows;

            // هر دو دراپ‌داون باید مجدداً لود شوند
            model.TradeTypes = await _tariffService.GetTradeTypesSelectListAsync(cancellationToken);
            model.ContainerSizeTypes = await _tariffService.GetContainerSizeTypesListAsync(cancellationToken);
            model.FullMTs = await _tariffService.GetFullMTsListAsync(cancellationToken);

            return PartialView("_ThcRateRowsGrid", model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize("BaseOperations.Tariffs.Edit")]
        public async Task<IActionResult> SaveThcRules(TariffThcRuleBatchFormVm model, CancellationToken cancellationToken)
        {
            // اعتبارسنجی کلی ردیف‌ها
            if (model.Rows == null || !model.Rows.Any())
            {
                ModelState.AddModelError(string.Empty, "At least one THC rate row is required.");
            }
            else
            {
                // اعتبارسنجی ردیف‌ها به جای هدر
                for (int i = 0; i < model.Rows.Count; i++)
                {
                    if (!model.Rows[i].TradeTypeId.HasValue)
                    {
                        ModelState.AddModelError($"Rows[{i}].TradeTypeId", "Trade Type is required.");
                    }

                    if (!model.Rows[i].ContainerSizeTypeId.HasValue)
                    {
                        ModelState.AddModelError($"Rows[{i}].ContainerSizeTypeId", "Container Size is required.");
                    }

                    if (!model.Rows[i].FullMTId.HasValue)
                    {
                        ModelState.AddModelError($"Rows[{i}].FullMTId", "Full/MT is required.");
                    }

                }
            }

            if (!ModelState.IsValid)
            {
                model = await _tariffService.RebuildThcRuleBatchFormAsync(model, cancellationToken);
                return PartialView("_ThcRuleBatchForm", model);
            }

            await _tariffService.SaveThcRulesAsync(model, cancellationToken);

            return PartialView("_HtmxModalSuccess", new HtmxModalSuccessVm
            {
                RefreshTarget = "#tariffRulesGrid",
                RefreshUrl = Url.Action(nameof(RulesGrid), "TariffBooks", new { tariffBookId = model.TariffBookId })!,
                ClearTargets = new[] { "#tariffRuleCoveragesGrid" }
            });
        }
        [HttpGet]
        [Authorize("BaseOperations.Tariffs.Edit")]
        public async Task<IActionResult> SearchLocations(string? term, CancellationToken cancellationToken)
        {
            var items = await _tariffService.SearchLocationsAsync(term, cancellationToken);
            return PartialView("_LocationSearchResults", items);
        }
    }
}