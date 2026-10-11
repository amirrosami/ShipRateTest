using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ShipRate.Application.DTOs.BillOfLadings;
using ShipRate.Application.DTOs.Finance;
using ShipRate.Application.Interfaces;
using ShipRate.Application.Interfaces.Cargos;
using ShipRate.Application.Interfaces.Finance;
using ShipRate.Application.Interfaces.Jobs;
using ShipRate.Application.Interfaces.Reports;
using ShipRate.Application.Interfaces.SalesSpecial;
using ShipRate.Application.JobsViewModels;
using ShipRate.Application.Utilities;
using ShipRate.Application.ViewModels;
using System.Text.Json;


namespace ShipRate.WebApp.Controllers
{
    [Authorize]
    public class JobsController : Controller
    {
        private readonly IJobService _jobService;
        private readonly IJobLookupService _lookupService;
        private readonly ICargoDetailsService _cargoDetailsService;
        private readonly IPackagesService _packagesService;
        private readonly IJobPartyService _jobPartyService;
        private readonly IShipmentService _shipmentService;
        private readonly IInvoiceAppService _invoiceAppService;
        private readonly ITransportDocumentService _transportDocumentService;
        private readonly ISettlementService _settlementService;
        private readonly IActivityLogService _activityLogService;
        private readonly ICustomsClearanceService _customsClearanceService;
        private readonly IBookingService _bookingService;
        private readonly ISalesSpecialService _salesService;
        private const string JobSearchSessionKey = "Jobs_LastSearchCriteria";


        public JobsController(
            IJobService jobService,
            IJobLookupService lookupService,
            ICargoDetailsService cargoDetailsService,
            IPackagesService packagesService,
            IJobPartyService jobPartyService,
            IShipmentService shipmentService,
            IInvoiceAppService invoiceAppService,
            ITransportDocumentService transportDocumentService,
            ISettlementService settlementService,
            IActivityLogService activityLogService,
            ICustomsClearanceService customsClearanceService,
            IReportService reportService,
            IWebHostEnvironment env,
            IBookingService bookingService,
            ISalesSpecialService salesService)
        {
            _jobService = jobService;
            _lookupService = lookupService;
            _cargoDetailsService = cargoDetailsService;
            _packagesService = packagesService;
            _jobPartyService = jobPartyService;
            _shipmentService = shipmentService;
            _invoiceAppService = invoiceAppService;
            _transportDocumentService = transportDocumentService;
            _settlementService = settlementService;
            _activityLogService = activityLogService;
            _customsClearanceService = customsClearanceService;
            _bookingService = bookingService;
            _salesService = salesService;
        }
        private void HxTrigger(object triggers)
        {
            Response.Headers["HX-Trigger"] = JsonSerializer.Serialize(triggers);
        }
        [HttpGet]
        [Authorize("Operations.Jobs.View")]
        public async Task<IActionResult> Index(JobSearchVm search)
        {
            search ??= new JobSearchVm();

            bool isRequestEmpty = string.IsNullOrWhiteSpace(search.JobNumber) && search.Page <= 1;
            if (isRequestEmpty)
            {
                // تلاش برای بازیابی آخرین سرچ از Session
                var savedRequest = HttpContext.Session.GetObject<JobSearchVm>(JobSearchSessionKey);
                if (savedRequest != null)
                {
                    search = savedRequest;
                }
            }
            else
            {
                // در صورت ارسال پارامتر مستقیم در متد Get، ذخیره در سشن
                HttpContext.Session.SetObject(JobSearchSessionKey, search);
            }
            var result = await _jobService.SearchAsync(search);
            var vm = new JobIndexVm
            {
                Search = search,
                Jobs = result.Items.ToList(),
                TotalCount = result.TotalCount,
                TotalPages = (int)Math.Ceiling(result.TotalCount / (double)search.PageSize),
                Companies = await _lookupService.GetCompaniesAsync(),
                //Customers = await _lookupService.GetCustomersAsync(),
                TransportTypes = await _lookupService.GetTransportTypesAsync(),
                JobStatuses = await _lookupService.GetJobStatusesAsync(),
                TradeTypes = await _lookupService.GetTradeTypesAsync(),
                SalesUsers = await _salesService.GetSalesUsersAsync()
            };
            return View(vm);
        }

        [HttpGet]
        [Authorize("Operations.Jobs.View")]
        public async Task<IActionResult> List(JobSearchVm search)
        {
            search ??= new JobSearchVm();
            HttpContext.Session.SetObject(JobSearchSessionKey, search);
            var result = await _jobService.SearchAsync(search);
            var vm = new JobIndexVm
            {
                Search = search,
                Jobs = result.Items.ToList(),
                TotalCount = result.TotalCount,
                TotalPages = (int)Math.Ceiling(result.TotalCount / (double)search.PageSize)
            };

            return PartialView("_JobTable", vm);
        }

        [HttpGet]
        [Authorize("Operations.Jobs.Edit")]
        public async Task<IActionResult> Create()
        {
            var vm = await _jobService.BuildCreateVmAsync();
            vm.JobSourceTypeId = 1; //Manual
            vm.JobId = Guid.Empty;
            vm.JobNumber = _jobService.GenerateNewJobNumberAsync();


            var sizeTypes = await _lookupService.GetContainerSizeTypesAsync();
            var ownTypes = await _lookupService.GetCntOwnTypesAsync();

            //var packageTypes = await _lookupService.GetPackageTypesAsync();
            //var dimunits = await _lookupService.GetDimUnitsAsync();
            //var weightunits = await _lookupService.GetWeightUnitsAsync();
            //var volumeunits = await _lookupService.GetVolumeUnitsAsync();

            var companies = await _lookupService.GetCompaniesAsync();
            var servicetypes = await _lookupService.GetServiceTypesAsync();
            var companyroletypes = await _lookupService.GetCompanyRoleTypesAsync();
            var roletypes = await _lookupService.GetRoleTypesAsync();

            vm.JobParties.Add(new JobPartyCreateVm
            {
                Index = 0,
                RoleTypes = roletypes,
            });

            vm.Containers.Add(new JobContainerCreateVm
            {
                Index = 0,
                SizeTypes = sizeTypes,
                CntOwnTypes = ownTypes
            });
            vm.JobCompanyRoles.Add(new JobCompanyRoleCreateVm
            {
                Index = 0,
                Companies = companies,
                CompanyRoleTypes = companyroletypes,
                ServiceTypes = servicetypes
            });
            ViewBag.DialogTitle = "Create Job";
            return PartialView("_CreateEdit", vm);
        }
        [HttpGet]
        [Authorize("Operations.Jobs.Edit")]
        public async Task<IActionResult> Edit(Guid id)
        {
            var vm = await _jobService.BuildEditVmAsync(id);
            var sizeTypes = await _lookupService.GetContainerSizeTypesAsync();
            var ownTypes = await _lookupService.GetCntOwnTypesAsync();
            var servicetypes = await _lookupService.GetServiceTypesAsync();
            var companyroletypes = await _lookupService.GetCompanyRoleTypesAsync();
            if (vm.Containers == null || vm.Containers.Count == 0)
            {
                vm.Containers.Add(new JobContainerCreateVm
                {
                    Index = 0,
                    SizeTypes = sizeTypes,
                    CntOwnTypes = ownTypes
                });
            }
            if (vm.JobCompanyRoles == null || vm.JobCompanyRoles.Count == 0)
            {
                vm.JobCompanyRoles.Add(new JobCompanyRoleCreateVm
                {
                    Index = 0,
                    Companies = vm.Companies,
                    CompanyRoleTypes = companyroletypes,
                    ServiceTypes = servicetypes
                });
            }
            ViewBag.DialogTitle = "Edit Job";
            return PartialView("_CreateEdit", vm);
        }

        [HttpGet]
        [Authorize("Operations.Jobs.Edit")]
        public async Task<IActionResult> AddPackageRow(int index)
        {
            var packageTypes = await _lookupService.GetPackageTypesAsync();
            var dimunits = await _lookupService.GetDimUnitsAsync();
            var weightunits = await _lookupService.GetWeightUnitsAsync();
            var volumeunits = await _lookupService.GetVolumeUnitsAsync();
            var vm = new JobPackageCreateVm
            {
                Index = index,
                PackageTypes = packageTypes,
            };

            return PartialView("~/Views/Jobs/_JobPackageRow.cshtml", vm);
        }


        [HttpGet]
        [Authorize("Operations.Jobs.Edit")]
        public async Task<IActionResult> AddContainerRow(int index)
        {
            var sizeTypes = await _lookupService.GetContainerSizeTypesAsync();
            var cntOwnTypes = await _lookupService.GetCntOwnTypesAsync();
            var vm = new JobContainerCreateVm
            {
                Index = index,
                SizeTypes = sizeTypes,
                CntOwnTypes = cntOwnTypes
            };

            return PartialView("~/Views/Jobs/_JobContainerRow.cshtml", vm);
        }

        [HttpGet]
        [Authorize("Operations.Jobs.Edit")]
        public async Task<IActionResult> AddPartyRow(int index)
        {
            var roletypes = await _lookupService.GetRoleTypesAsync();
            var vm = new JobPartyCreateVm
            {
                Index = index,
                RoleTypes = roletypes
            };
            return PartialView("~/Views/Jobs/_JobPartyRow.cshtml", vm);
        }

        [HttpGet]
        [Authorize("Operations.Jobs.Bookings.Edit")]
        public async Task<IActionResult> AddBookingPartyRow(int index)
        {
            var roletypes = await _lookupService.GetRoleTypesAsync();
            var vm = new BookingPartyCreateVm
            {
                Index = index,
                RoleTypes = roletypes
            };
            return PartialView("~/Views/Booking/_BookingPartyRow.cshtml", vm);
        }
        //AddCompanyRoleTypeRow
        [HttpGet]
        [Authorize("Operations.Jobs.Edit")]
        public async Task<IActionResult> AddCompanyRoleTypeRow(int index)
        {
            var companies = await _lookupService.GetCompaniesAsync();
            var servicetypes = await _lookupService.GetServiceTypesAsync();
            var companyroletypes = await _lookupService.GetCompanyRoleTypesAsync();
            var vm = new JobCompanyRoleCreateVm
            {
                Index = index,
                Companies = companies,
                CompanyRoleTypes = companyroletypes,
                ServiceTypes = servicetypes
            };
            return PartialView("~/Views/Jobs/_JobCompanyRoleRow.cshtml", vm);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize("Operations.Jobs.Edit")]
        public async Task<IActionResult> SaveJob(CreateJobVm vm)
        {
            if (vm.OriginLocationId == Guid.Empty || vm.DestinationLocationId == Guid.Empty)
            {
                ModelState.AddModelError("Locations", "Origin-Destination is Required");
            }
            if (!ModelState.IsValid)
            {
                await FillCreateLookups(vm);
                return PartialView("_CreateEdit", vm);
            }
            if (vm.JobId is null || vm.JobId == Guid.Empty)
                vm.JobId = await _jobService.CreateAsync(vm);
            else
                await _jobService.UpdateAsync(vm);

            //Response.Headers["HX-Trigger"] = "jobCreated";
            //return NoContent();

            HxTrigger(new
            {
                toastSuccess = "Job Saved Successfully"
            });




            Response.Headers["HX-Redirect"] = Url.Action(nameof(Details), new { id = vm.JobId });
            return new EmptyResult();
        }

        [HttpDelete]
        [Authorize("Operations.Jobs.Edit")]
        public async Task<IActionResult> Delete(Guid id)
        {
            var b = await _jobService.CanBeDeletedAsync(id);
            if (!b.CanDelete)
            {
                HxTrigger(new
                {
                    toastFail = b.Message
                });
                return NoContent();
            }
            await _jobService.DeleteAsync(id);
            HxTrigger(new
            {
                jobDeleted = true,
                successToast = "Job Deleted Successfully!!!"
            });
            return NoContent();
        }
        [HttpDelete]
        [Authorize("Operations.Jobs.Bookings.Edit")]
        public async Task<IActionResult> DeleteBooking(Guid id)
        {
            bool b = await _jobService.DeleteBookingAsync(id);
            if (b)
            {
                Response.Headers["HX-Trigger"] = "bookingDeleted";
                return NoContent();
            }
            return NoContent();
        }

        [HttpDelete]
        [Authorize("Operations.Jobs.Bookings.Edit")]
        public async Task<IActionResult> DeleteRTSInstruction(Guid id)
        {
            bool b = await _jobService.DeleteRTSInstructionAsync(id);
            if (b)
            {

                HxTrigger(new
                {
                    rtsSaved = true,
                    toastSuccess = "RTS Instruction Deleted Successfuly!!!!"
                }
                    );

                return NoContent();
            }
            return NoContent();
        }

        [HttpGet]
        [Authorize("Operations.Jobs.Bookings.View")]
        public async Task<IActionResult> GetDynamicBookingFields(Guid jobId, Guid? bookingId, int transportTypeId, int companyRoleTypeId)
        {
            // دریافت اطلاعات پایه و کلاینت‌ها برای لود شدن لیست‌های دراپ‌داون
            // (این متد در سرویس شما ViewModel را مجدداً با لیست‌ها پر می‌کند)
            CreateBookingVm model = await _jobService.GetBookingViewModelAsync(jobId, bookingId, transportTypeId, companyRoleTypeId);
            // ارسال به ویو واسط که تصمیم‌گیری را انجام دهد
            return PartialView("_DynamicBookingFields", model);
        }




        private async Task FillCreateLookups(CreateJobVm vm)
        {
            vm.Companies = await _lookupService.GetCompaniesAsync();
            vm.Customers = await _lookupService.GetCustomersAsync();
            vm.Locations = await _lookupService.GetLocationsAsync();
            vm.TransportTypes = await _lookupService.GetTransportTypesAsync();
            vm.TradeTypes = await _lookupService.GetTradeTypesAsync();
            vm.LoadTypes = await _lookupService.GetLoadTypesAsync();
            vm.CargoHandlingTypes = await _lookupService.GetCargoHandlingTypesAsync();
            vm.CargoNatures = await _lookupService.GetCargoNaturesAsync();
            vm.Incoterms = await _lookupService.GetIncotermsAsync();
            vm.PaymentTypes = await _lookupService.GetPaymentTypesAsync();
            vm.JobTypes = await _lookupService.GetJobTypesAsync();
            vm.JobStatuses = await _lookupService.GetJobStatusesAsync();
            vm.MovementTypes = await _lookupService.GetMovementTypesAsync();
        }
        [Authorize("Operations.Jobs.View")]
        public async Task<PartialViewResult> Stats()
        {
            var vm = await _jobService.Stats();
            return PartialView("_JobStats", vm);
        }
        [HttpGet]
        [Authorize("Operations.Jobs.View")]
        public async Task<IActionResult> Details(Guid id)
        {
            var model = await _jobService.GetDetailsAsync(id);
            if (model.JobCompanyRolesList is null || model.JobCompanyRolesList.Count == 0)
            {
                TempData["ToastFail"] = "Please Complete Company Roles!";
                return RedirectToAction(nameof(Index));
            }
            var legs = await _shipmentService.GetShipmentLegListAsync(new ShipmentLegFilterVm { ShipmentId = await _jobService.GetFirstShipmentIdAsync(id) });
            model.Legs = legs.Items;
            if (model == null)
                return NotFound();
            return View(model);
        }

        [HttpGet]
        [Authorize("Operations.Jobs.View")]
        public async Task<IActionResult> DetailsTab(Guid id, string tab)
        {
            if (string.IsNullOrWhiteSpace(tab))
                return BadRequest();

            switch (tab.ToLower())
            {
                case "overview":
                    var overview = await _jobService.GetDetailsAsync(id);
                    return PartialView("~/Views/Jobs/_OverviewTab.cshtml", overview);
                case "shipment":
                    var shipmentlegfiltervm = new ShipmentLegFilterVm { ShipmentId = await _jobService.GetFirstShipmentIdAsync(id) };
                    return PartialView("~/Views/Shipments/ShipmentLegs/_ShipmentLegPanel.cshtml", shipmentlegfiltervm);
                case "booking":
                    var bookingTab = await _jobService.GetBookingTabAsync(id);
                    return PartialView("~/Views/Booking/_BookingTab.cshtml", bookingTab);

                case "bls":
                    //var bls = await _jobService.GetBLTabAsync(id);
                    //return PartialView("~/Views/Jobs/Tabs/_BLTab.cshtml", bls);
                    var context = new BLContextDto
                    {
                        JobId = id,
                    };
                    var bls = await _transportDocumentService.GetManagePageAsync(context);
                    return PartialView("~/Views/TransportDocuments/_BLPanel.cshtml", bls);
                case "cargodetails":

                    var items = await _cargoDetailsService.GetListAsync(new Application.ViewModels.CargoDetailFilterVm { JobId = id });
                    ViewBag.Filter = new Application.ViewModels.CargoDetailFilterVm { JobId = id };

                    return PartialView("~/Views/CargoDetails/_CargoDetailsPanel.cshtml", items);

                case "containers":
                    var containers = await _jobService.GetContainersListAsync(id);
                    ViewBag.Filter = new RateRequestContainerFilterVm { JobId = id };
                    return PartialView("~/Views/Jobs/_ContainersPanel.cshtml", containers);
                case "packages":
                    var packages = await _packagesService.GetListAsync(new Application.ViewModels.RateRequestPackageFilterVm { JobId = id });
                    ViewBag.Filter = new RateRequestPackageFilterVm { JobId = id };
                    return PartialView("~/Views/Packages/_PackagesPanel.cshtml", packages);

                case "parties":
                    //var parties = await _jobPartyService.GetListAsync(new Application.ViewModels.JobPartyFilterVm { JobId = id });
                    //ViewBag.Filter = new JobPartyFilterVm { JobId = id };
                    var jobpartyfiltervm = new JobPartyFilterVm { JobId = id };
                    return PartialView("~/Views/JobParty/_JobPartyPanel.cshtml", jobpartyfiltervm);

                case "charges":
                    var charges = await _jobService.GetChargesVmAsync(id);
                    return PartialView("~/Views/Charges/_ChargesTab.cshtml", charges);
                case "invoices":
                    var filter = new InvoiceListFilterVm { JobId = id };
                    var vm = await _invoiceAppService.GetListPageAsync(filter);
                    return PartialView("~/Views/Invoices/_InvoicesPanel.cshtml", vm);
                case "settles":
                    var model = await _settlementService.GetJobSettlementsAsync(id);
                    return PartialView("~/Views/Settlements/_JobSettlementList.cshtml", model);
                case "customs":
                    var cmodel = await _customsClearanceService.GetJobTabAsync(id)
                                ?? new CustomsClearanceJobTabVm
                                {
                                    JobId = id
                                };
                    return PartialView("~/Views/CustomsClearances/_JobTab.cshtml", cmodel);
                case "documents":
                    return RedirectToAction("Index", "DocumentManager", new { objectId = id, objectTypeId = 300 });
                case "history":
                    var history = await _activityLogService.GetEntityTimelineAsync(1, id);
                    return PartialView("~/Views/ActivityLog/_Timeline.cshtml", history);
                default:
                    return NotFound();
            }
        }






        [HttpGet]
        public async Task<IActionResult> GetLoadTypes(int? transportTypeId)
        {
            var vm = new CreateJobVm();

            vm.LoadTypes = await _lookupService.GetLoadTypesByTransportTypeAsync(transportTypeId);
            return PartialView("_LoadTypeSelect", vm);
        }
        [HttpGet]
        public async Task<IActionResult> GetLoadTypesForBooking(int? transportTypeId)
        {
            var vm = new CreateBookingVm();
            vm.LoadTypes = await _lookupService.GetLoadTypesByTransportTypeAsync(transportTypeId);
            vm.TransportTypeId = transportTypeId.Value;
            return PartialView("~/Views/Booking/_LoadTypeSelectForBooking.cshtml", vm);
        }
        [Authorize("Operations.Jobs.Bookings.View")]
        public async Task<IActionResult> BookingsList(Guid jobId)
        {
            var bookings = await _jobService.GetBookingTabAsync(jobId);
            return PartialView("~/Views/Booking/_BookingTable.cshtml", bookings.Bookings);
        }

        [HttpGet]
        [Authorize("Operations.Jobs.Bookings.Create")]
        public async Task<IActionResult> CreateBooking(Guid jobId)
        {
            var vm = await _jobService.BuildCreateBookingVmAsync();
            vm.JobId = jobId;
            var sizeTypes = await _lookupService.GetContainerSizeTypesAsync();
            var roletypes = await _lookupService.GetRoleTypesAsync();
            var cntowntypes = await _lookupService.GetCntOwnTypesAsync();
            var legs = await _lookupService.GetShipmentLegsAsync(jobId);
            vm.Legs = legs;
            //vm.JobParties.Add(new JobPartyCreateVm
            //{
            //    Index = 0,
            //    RoleTypes = roletypes,
            //});

            vm.Containers.Add(new JobContainerCreateVm
            {
                Index = 0,
                SizeTypes = sizeTypes,
                CntOwnTypes = cntowntypes
            });
            //vm.Packages.Add(new JobPackageCreateVm
            //{
            //    Index = 0,
            //    DimensionMeasurementUnits = dimunits,
            //    VolumeMeasurementUnits = volumeunits,
            //    WeightMeasurementUnits = weightunits,
            //    PackageTypes = packageTypes
            //});

            //vm.JobCompanyRoles.Add(new JobCompanyRoleCreateVm
            //{
            //    Index = 0,
            //    Companies = companies,
            //    CompanyRoleTypes = companyroletypes,
            //    ServiceTypes = servicetypes
            //});

            return PartialView("~/Views/Booking/_CreateEditBooking.cshtml", vm);
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize("Operations.Jobs.Bookings.Edit")]
        public async Task<IActionResult> SaveBooking(CreateBookingVm vm)
        {
            if (!ModelState.IsValid)
            {
                //await FillCreateLookups(vm);
                return PartialView("_Create", vm);
            }

            if (vm.BookingId is null || vm.BookingId == Guid.Empty)
                await _jobService.CreateBookingAsync(vm);
            else
                await _jobService.UpdateBookingAsync(vm);

            Response.Headers["HX-Trigger"] = "bookingCreated";
            return NoContent();
        }



        [HttpGet]
        [Authorize("Operations.Jobs.BLs.Edit")]
        public async Task<IActionResult> CreateBL()
        {
            var vm = await _jobService.BuildCreateBLVmAsync();

            vm.JobId = Guid.Empty;
            var sizeTypes = await _lookupService.GetContainerSizeTypesAsync();
            var packageTypes = await _lookupService.GetPackageTypesAsync();
            var dimunits = await _lookupService.GetDimUnitsAsync();
            var weightunits = await _lookupService.GetWeightUnitsAsync();
            var volumeunits = await _lookupService.GetVolumeUnitsAsync();

            var companies = await _lookupService.GetCompaniesAsync();
            var servicetypes = await _lookupService.GetServiceTypesAsync();
            var companyroletypes = await _lookupService.GetCompanyRoleTypesAsync();
            var roletypes = await _lookupService.GetRoleTypesAsync();
            return PartialView("~/Views/TransportDocuments/_CreateEditBL.cshtml", vm);
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize("Operations.Jobs.BLs.Edit")]
        public async Task<IActionResult> SaveBL(CreateBLVm vm)
        {
            if (!ModelState.IsValid)
            {
                //await FillCreateLookups(vm);
                return PartialView("_CreateEditBL", vm);
            }

            if (vm.JobId == Guid.Empty)
                await _jobService.CreateBLAsync(vm);
            else
                await _jobService.UpdateBLAsync(vm);

            Response.Headers["HX-Trigger"] = "blCreated";
            return NoContent();
        }

        [HttpGet]
        [Authorize("Operations.Jobs.Charges.Edit")]
        public async Task<IActionResult> NewCharge(Guid jobId)
        {
            var vm = new CreateChargeVm { JobId = jobId };
            vm.Companies = await _lookupService.GetCompaniesAsync();
            vm.Currencies = await _lookupService.GetCurrenciesAsync();
            vm.ChargeTypes = await _lookupService.GetChargeTypesAsync();
            return PartialView("~/Views/Charges/_ChargeNewModal.cshtml", vm);
        }
        [HttpGet]
        [Authorize("Operations.Jobs.Charges.Edit")]
        public async Task<IActionResult> EditCharge(Guid chargeId)
        {
            var vm = await _jobService.GetChargeDetailsAsync(chargeId);
            vm.Companies = await _lookupService.GetCompaniesAsync();
            vm.Currencies = await _lookupService.GetCurrenciesAsync();
            vm.ChargeTypes = await _lookupService.GetChargeTypesAsync();
            return PartialView("~/Views/Charges/_ChargeNewModal.cshtml", vm);
        }
        [HttpPost]
        [Authorize("Operations.Jobs.Charges.Edit")]
        public async Task<IActionResult> CreateCharge(CreateChargeVm vm)
        {
            await _jobService.CreateChargeAsync(vm);
            var charges = await _jobService.GetChargesVmAsync(vm.JobId);
            return PartialView("~/Views/Charges/_ChargesTab.cshtml", charges);
        }

        public async Task<IActionResult> LoadVendors(int directionId, Guid jobId)
        {
            if (directionId != 2)
                return Content("<option value=''>-- Select Vendor --</option>");

            var vendors = await _lookupService.GetJobSpecificPartiesAsync(jobId);

            string html = "<option value=''>-- Select --</option>";
            foreach (var v in vendors)
                html += $"<option value='{v.Value}'>{v.Text}</option>";
            return Content(html, "text/html");
        }
        [HttpGet]
        [Authorize("Operations.Jobs.Bookings.View")]
        public async Task<IActionResult> RTSInstructionsByBooking(Guid bookingId, string bookingNo)
        {
            var containers = await _jobService.GetBookingContainerTypesToCreateRTSIAsync(bookingId);
            if (containers is null || containers.Count == 0) return NoContent();

            RTSInstructionsGridVm vm = new RTSInstructionsGridVm
            {
                BookingId = bookingId
            };
            var items = await _jobService.GetRTSInstructionsByBookingIdAsync(bookingId);
            vm.Items = items;
            ViewBag.BookingNo = bookingNo;
            return PartialView("~/Views/Booking/RTS/_RTSInstructionsGrid.cshtml", vm);
        }

        [HttpGet]
        [Authorize("Operations.Jobs.Bookings.Edit")]
        public async Task<IActionResult> AddRTSInstruction(Guid bookingId)
        {
            var model = new CreateRTSInstructionVm
            {
                BookingId = bookingId,
                RTSInstructionDate = DateTime.Now,
                ValidDateFrom = DateTime.Now,
                ValidDateTo = DateTime.Now.AddDays(7),
                CompanyId = 1
            };
            model.JobId = await _jobService.GetBookingJobIdAsync(bookingId);
            model.Containers = await _jobService.GetBookingContainerTypesToCreateRTSIAsync(model.BookingId);
            model.Shippers = await _lookupService.GetShippersAsync(bookingId);
            model.Statuses = await _lookupService.GetRTSStatusesAsync();
            model.Depots = await _lookupService.GetDepotsAsync();
            model.Carriers = await _lookupService.GetCarriersAsync(null);
            return PartialView("~/Views/Booking/RTS/_RTSNewModal.cshtml", model);
        }
        [HttpGet]
        [Authorize("Operations.Jobs.Bookings.Edit")]
        public async Task<IActionResult> EditRTSInstruction(Guid id, Guid bookingId)
        {
            //var model = new CreateRTSInstructionVm
            //{
            //    Id = id,
            //    BookingId = bookingId
            //};
            var model = await _jobService.GetRTSInstructionForEditAsync(id);
            model.JobId = await _jobService.GetBookingJobIdAsync(bookingId);
            model.Containers = await _jobService.GetBookingContainerTypesForEditRTSIAsync(id, bookingId);
            model.Shippers = await _lookupService.GetShippersAsync(bookingId);
            model.Statuses = await _lookupService.GetRTSStatusesAsync();
            model.Depots = await _lookupService.GetDepotsAsync();
            model.Carriers = await _lookupService.GetCarriersAsync(bookingId);
            return PartialView("~/Views/Booking/RTS/_RTSNewModal.cshtml", model);
        }

        [ValidateAntiForgeryToken]
        [HttpPost]
        [Authorize("Operations.Jobs.Bookings.Edit")]
        public async Task<IActionResult> SaveRTSInstruction(CreateRTSInstructionVm model)
        {
            if (!ModelState.IsValid)
            {
                model.Shippers = await _lookupService.GetShippersAsync(model.BookingId);
                model.Carriers = await _lookupService.GetCarriersAsync(model.BookingId);
                model.Depots = await _lookupService.GetDepotsAsync();
                model.Statuses = await _lookupService.GetRTSStatusesAsync();
                model.ContainerSizeTypes = await _lookupService.GetContainerSizeTypesAsync();

                return PartialView("~/Views/Booking/RTS/_RTSNewModal.cshtml", model);
            }
            model.CompanyId = 1;

            if (model.Id is null || model.Id == Guid.Empty)
                await _jobService.AddRTSInstructionAsync(model);
            else
                await _jobService.UpdateRTSInstructionAsync(model);

            Response.Headers.Append("HX-Trigger", "rtsSaved, refreshRTSGrid");

            return NoContent();
        }

        [HttpGet]
        [Authorize("Operations.Jobs.Bookings.View")]
        public async Task<IActionResult> RTSInstructionDetails(Guid id)
        {
            var vm = await _jobService.GetRTSInstructionDetailsAsync(id);

            if (vm == null)
                return NotFound();

            return PartialView("~/Views/Booking/RTS/_RTSInstructionDetailsModal.cshtml", vm);
        }
        [HttpGet]
        [Authorize("Operations.Jobs.Bookings.Edit")]
        public async Task<IActionResult> PrintRTSInstructionLetter(Guid id)
        {
            var vm = await _jobService.GetRTSInstructionDetailsAsync(id);
            if (vm == null)
                return NotFound();
            return PartialView("~/Views/Booking/RTS/_RTSInstructionDetailsModal.cshtml", vm);
        }
        [HttpGet]
        public async Task<IActionResult> SearchParties(
            string? term,
            string targetHiddenInputId = "PartyId",
            string targetTextInputId = "PartySearchText",
            CancellationToken cancellationToken = default)
        {
            var items = await _jobService.SearchPartiesAsync(term);

            ViewBag.TargetHiddenInputId = targetHiddenInputId;
            ViewBag.TargetTextInputId = targetTextInputId;

            return PartialView("_PartySearchResults", items);
        }

        [HttpGet]
        public async Task<IActionResult> ContainersList(Guid jobId)
        {
            var items = await _jobService.GetContainersListAsync(jobId);
            return PartialView("~/Views/Jobs/_ContainersList.cshtml", items);
        }


        [HttpGet]
        [Authorize("Operations.Jobs.Settlements.View")]
        public async Task<IActionResult> JobSettlementList(Guid jobId)
        {
            var model = await _settlementService.GetJobSettlementsAsync(jobId);
            return PartialView("~/Views/Settlements/_JobSettlementList.cshtml", model);
        }

        [HttpGet]
        public async Task<string> LoadChargeDescription(int ChargeTypeId)
        {
            var chargeTypeList = await _lookupService.GetChargeTypesAsync();
            return chargeTypeList.Where(x => x.Value == ChargeTypeId.ToString()).FirstOrDefault().Text;
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize("Operations.Jobs.Edit")]
        public async Task<IActionResult> OperationConfirm(Guid myJobId)
        {
            bool b = await _jobService.JobExistsToConfirmAsync(myJobId);
            if (!b) return NotFound();
            bool result = await _jobService.OperationConfirmAsync(myJobId);
            if (result) return NoContent(); else return BadRequest();
        }

        [HttpGet]
        [Authorize("Operations.Jobs.Charges.Edit")]
        public async Task<IActionResult> DeleteCharge(Guid chargeId)
        {
            var model = await _jobService.BuildChargeDeleteModelAsync(chargeId);
            if (model == null)
                return NotFound();
            return PartialView("~/Views/Charges/_DeleteCharge.cshtml", model);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize("Operations.Jobs.Charges.Edit")]
        public async Task<IActionResult> DeleteChargeConfirmed(Guid chargeId, Guid jobId)
        {
            var b = await _jobService.DeleteChargeAsync(chargeId);
            if (b)
            {
                Response.Headers["HX-Trigger"] = "chargeDeleted";
                return NoContent();
            }
            return NoContent();
        }
        [HttpGet]
        [Authorize("Operations.Jobs.Bookings.Edit")]
        public async Task<IActionResult> ConfirmAndIssueRTSInstruction(Guid id)
        {
            var b = await _jobService.ConfirmAndIssueRTSInstructionAsync(id);
            if (b)
            {
                HxTrigger(new
                {
                    rtsSaved = true,
                    toastSuccess = "RTS Instruction Confirmed Successfuly!!!"
                });
                return NoContent();
            }
            return NoContent();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> QuickSearch(string? searchTerm, CancellationToken cancellationToken)
        {
            searchTerm = searchTerm?.Trim();
            if (string.IsNullOrWhiteSpace(searchTerm))
            {
                return RedirectToAction("Index", "Home");
            }
            var jobId = await _jobService.GetJobIdForQuickSearchTermAsync(searchTerm);

            if (jobId != Guid.Empty)
            {
                return RedirectToAction(nameof(Details), new { id = jobId });
            }
            return NoContent();
        }
    }
}