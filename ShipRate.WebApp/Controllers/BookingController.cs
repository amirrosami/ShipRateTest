using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ShipRate.Application.DTOs;
using ShipRate.Application.Interfaces.Jobs;
using ShipRate.Application.Interfaces.Reports;
using ShipRate.Application.ViewModels;
using ShipRate.Application.ViewModels.BookingConsoleVms;

namespace ShipRate.WebApp.Controllers
{
    [Authorize]

    public class BookingController : Controller
    {
        private readonly IBookingService _bookingService;
        private readonly IJobLookupService _lookupService;
        private readonly IReportService _reportService;


        public BookingController(IBookingService bookingService, IJobLookupService lookupService, IReportService reportService)
        {
            _bookingService = bookingService;
            _lookupService = lookupService;
            _reportService = reportService;
        }

        [HttpGet]
        public async Task<IActionResult> Create(Guid? jobId)
        {
            var model = new BookingFormVm
            {
                JobId = jobId,
                TransportTypeId = 1 // default sea
            };

            await FillLookupsAsync(model);
            return PartialView("~/Views/Booking/_BookingFormModal.cshtml", model);
        }

        [HttpGet]
        public async Task<IActionResult> Edit(Guid id)
        {
            var model = await _bookingService.GetBookingForEditAsync(id);
            await FillLookupsAsync(model);
            await PopulateContainerLookupsAsync();
            await PopulateChargeLookupsAsync();
            ViewBag.Filter = new CargoDetailFilterVm { BookingId = id };
            return PartialView("~/Views/Booking/_BookingFormModal.cshtml", model);
        }


        [HttpGet]
        public async Task<IActionResult> AddChargeRow()
        {
            var index = Guid.NewGuid().ToString("N");
            ViewData["Index"] = index;
            await PopulateChargeLookupsAsync();
            return PartialView("~/Views/Booking/_BookingChargeRow.cshtml", new BookingChargeVm());
        }


        [HttpGet]
        public async Task<IActionResult> AddLegRow(int index)
        {
            var model = new BookingLegVm
            {
                Id = Guid.CreateVersion7(),
                SequenceNo = index + 1
            };
            await PopulateLegLookupsAsync(model);
            ViewData["LegIndex"] = index;
            return PartialView("~/Views/Booking/_LegRowTemplate.cshtml", model);
        }


        [HttpGet]
        public async Task<IActionResult> AddContainerRow()
        {
            var model = new BookingContainerTypeVm { Id = Guid.CreateVersion7() };
            await PopulateContainerLookupsAsync();
            return PartialView("~/Views/Booking/_ContainerRowTemplate.cshtml", model);
        }

        [HttpGet]
        public async Task<IActionResult> AddPartyRow()
        {
            // تولید شناسه Guid نسخه 7 برای ردیف جدید
            var model = new BookingPartyVm { Id = Guid.CreateVersion7() };

            // مطمئن شوید که علاوه بر Roleها، لیست Parties هم در این متد پر می‌شود:
            await PopulatePartyLookupsAsync();

            return PartialView("~/Views/Booking/_PartyRowTemplate.cshtml", model);
        }

        //[HttpGet]
        //public async Task<IActionResult> GetTransportDetailsForm(int transportTypeId)
        //{
        //    var model = new BookingFormVm { TransportTypeId = transportTypeId };
        //    await FillLookupsAsync(model);
        //    return PartialView("~/Views/Jobs/Tabs/_TransportDetailForm.cshtml", model);
        //}



        private async Task FillLookupsAsync(BookingFormVm model)
        {
            ViewBag.TransportTypes = await _lookupService.GetTransportTypesAsync();
            ViewBag.LoadTypes = await _lookupService.GetLoadTypesAsync();
            ViewBag.ServiceTypes = await _lookupService.GetServiceTypesAsync();
            ViewBag.BookingStatuses = await _lookupService.GetBookingStatusesAsync();
            ViewBag.Companies = await _lookupService.GetCompaniesAsync();
            ViewBag.CompanyRoleTypes = await _lookupService.GetCompanyRoleTypesAsync();

            //ViewBag.Locations = await _lookupService.GetLocationsAsync();

            //ViewBag.Parties = await _lookupService.GetPartiesAsync();

            ViewBag.ContainerSizeTypes = await _lookupService.GetContainerSizeTypesAsync();
            ViewBag.ContainerOwnTypes = await _lookupService.GetContainerOwnTypesAsync();
            ViewBag.PartyRoleTypes = await _lookupService.GetRoleTypesAsync();
            ViewBag.TruckTypes = await _lookupService.GetTruckTypesAsync();

            ViewBag.Depots = await _lookupService.GetDepotsAsync();
            ViewBag.Terminals = await _lookupService.GetTerminalsAsync();

            ViewBag.Airports = await _lookupService.GetAirportsAsync(); // یا لیست Locations فیلتر شده بر اساس فرودگاه
            ViewBag.Airlines = await _lookupService.GetAirlinesAsync();  // یا فیلتر شده برای خطوط هوایی

            ViewBag.Carriers = await _lookupService.GetCarriersAsync(null);
            //ViewBag.Vendors = await _lookupService.GetPartiesAsync();
            //ViewBag.BookingAgents = await _lookupService.GetPartiesAsync();
        }

        private async Task PopulateLegLookupsAsync(BookingLegVm model)
        {
            //ViewBag.Locations = await _lookupService.GetLocationsAsync();
            ViewBag.TransportTypes = await _lookupService.GetTransportTypesAsync();
        }

        private async Task PopulateContainerLookupsAsync()
        {
            ViewBag.ContainerSizes = await _lookupService.GetContainerSizeTypesAsync();
            ViewBag.OwnershipTypes = await _lookupService.GetCntOwnTypesAsync();
        }

        private async Task PopulateChargeLookupsAsync()
        {
            ViewBag.ChargeTypes = await _lookupService.GetChargeTypesAsync();
            ViewBag.ChargeSides = await _lookupService.GetFinanceDirectionsForInvoiceContextAsync();
            ViewBag.Currencies = await _lookupService.GetCurrenciesAsync();
            ViewBag.Parties = await _lookupService.GetPartiesAsync();
        }


        private async Task PopulatePartyLookupsAsync()
        {
            ViewBag.PartyRoleTypes = await _lookupService.GetRoleTypesAsync();
            ViewBag.Parties = await _lookupService.GetPartiesAsync();
        }
        [HttpGet]
        public async Task<IActionResult> OpenBookingModal(Guid? id, Guid? jobId)
        {
            BookingFormVm model;

            if (id.HasValue && id.Value != Guid.Empty)
            {
                model = await _bookingService.GetBookingForEditAsync(id.Value);
            }
            else
            {
                model = new BookingFormVm
                {
                    JobId = jobId,
                    TransportTypeId = 1 // پیش‌فرض دریایی
                };
            }
            await FillLookupsAsync(model);
            return PartialView("~/Views/Booking/_BookingFormModal.cshtml", model);
        }

        // GET: لود پویای جزئیات فیلدهای ترانسپورت بر اساس نوع حمل
        [HttpGet]
        public async Task<IActionResult> GetTransportDetailsForm(int transportTypeId, Guid? id)
        {
            BookingFormVm model;

            if (id.HasValue && id.Value != Guid.Empty)
            {
                model = await _bookingService.GetBookingForEditAsync(id.Value);
                model.TransportTypeId = transportTypeId;
            }
            else
            {
                model = new BookingFormVm
                {
                    TransportTypeId = transportTypeId
                };
            }

            await FillLookupsAsync(model);
            return PartialView("~/Views/Booking/_TransportDetailForm.cshtml", model);
        }

        // POST: ذخیره‌سازی هم‌زمان در یک تراکنش
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Save(BookingFormVm model)
        {
            if (!ModelState.IsValid)
            {
                await FillLookupsAsync(model);
                // در صورت وجود خطا، کل مودال مجدداً رندر و جایگزین می‌شود
                return PartialView("~/Views/Booking/_BookingFormModal.cshtml", model);
            }

            try
            {

                bool isNew = !model.Id.HasValue || model.Id == Guid.Empty;
                var bookingId = await _bookingService.SaveBookingAsync(model);

                // با استفاده از Triggerهای HTMX، لیست اصلی در پشت‌زمینه بدون رفرش کل صفحه بروز می‌شود.
                //Response.Headers.Add("HX-Trigger", "refreshBookingGrid");

                //if (isNew)
                //{
                //    Response.Headers["HX-Trigger"] =
                //                $"{{\"bookingCreated\":{{\"id\":\"{bookingId}\"}}}}";

                //    return new EmptyResult();
                //}
                //else
                //{
                //    Response.Headers["HX-Trigger"] = "bookingUpdated";
                //}


                Response.Headers.Append("HX-Trigger", System.Text.Json.JsonSerializer.Serialize(new
                {
                    bookingUpdated = new { id = bookingId },
                    bookingSaved = true
                }));

                return Ok();



                // بستن مودال بوت‌استرپ به صورت کلاینت ساید
                //return Content("<script>$('#booking-modal-container').closest('.modal').modal('hide');</script>");
            }
            catch (Exception ex)
            {
                ModelState.AddModelError(string.Empty, $"Transactional Save Failed: {ex.Message}");
                await FillLookupsAsync(model);
                return PartialView("~/Views/Booking/_BookingFormModal.cshtml", model);
            }
        }
        // متد کمکی برای پر کردن دراپ‌دان‌های تب حمل دریایی
        private async Task PopulateSeaTransportLookupsAsync()
        {
            //var locations = await _lookupService.GetLocationsAsync(); // بنادر و ترمینال‌ها
            var parties = await _lookupService.GetPartiesAsync();     // لاین‌های کشتیرانی (Shipping Lines)
            var vessels = await _lookupService.GetVesselsAsync();     // کشتی‌ها
            var voyages = await _lookupService.GetVoyagesAsync();     // سفرها

            //ViewBag.Ports = locations; // بنادر بارگیری و تخلیه

            ViewBag.ShippingLines = parties; // کشتیرانی‌ها

            ViewBag.Depots = await _lookupService.GetDepotsAsync();
            ViewBag.Terminals = await _lookupService.GetTerminalsAsync();

            ViewBag.Vessels = vessels;
            ViewBag.Voyages = voyages;
        }

        // اکشن HTMX برای اضافه کردن سطر جدید Voyage
        [HttpGet]
        public async Task<IActionResult> AddVoyageRow(int index)
        {
            await PopulateSeaTransportLookupsAsync();

            var model = new BookingSeaVoyageVm
            {
                Id = Guid.CreateVersion7(),
                SequenceNo = index + 1,
                IsTransshipment = index > 0 // معمولاً اولین سطر ترانشیپمنت نیست، سطرهای بعدی هستند




            };

            ViewData["VoyageIndex"] = index;
            // استفاده از تمپلت سطر تکی برای Voyage
            return PartialView("~/Views/Booking/_VoyageRowTemplate.cshtml", model);
        }
        [HttpGet]
        public async Task<IActionResult> AddRoadRow(int index)
        {
            // مقداردهی لیست لوکیشن‌ها و انواع کامیون‌ها در ViewBag برای استفاده در دراپ‌داون‌ها
            //ViewBag.Locations = await _lookupService.GetLocationsAsync();
            ViewBag.TruckTypes = await _lookupService.GetTruckTypesAsync(); // مثلاً ترانزیت، چادری، کفی و غیره

            var model = new BookingRoadDetailVm
            {
                Id = Guid.CreateVersion7(),
                TruckCount = 1,
                Index = index

            };
            return PartialView("~/Views/Booking/_RoadRowTemplate.cshtml", model);
        }

        [HttpGet]
        public async Task<IActionResult> AddRailRow(int index)
        {
            // ایستگاه‌ها معمولاً در جدول Locations یا یک جدول اختصاصی Stations هستند
            ViewBag.Stations = await _lookupService.GetLocationsAsync();
            ViewBag.RailOperators = await _lookupService.GetCarriersAsync(null); // یا فیلتر شده برای اپراتورهای ریلی

            var model = new BookingRailDetailVm
            {
                Id = Guid.CreateVersion7()
            };

            return PartialView("~/Views/Booking/_RailRowTemplate.cshtml", model);
        }
        [HttpGet]
        public async Task<IActionResult> AddAirFlightRow(int index)
        {
            // بارگذاری فرودگاه‌ها و شرکت‌های هواپیمایی
            ViewBag.Airports = await _lookupService.GetAirportsAsync(); // یا لیست Locations فیلتر شده بر اساس فرودگاه
            ViewBag.Airlines = await _lookupService.GetAirlinesAsync();  // یا فیلتر شده برای خطوط هوایی
            var model = new BookingAirFlightVm
            {
                Id = Guid.CreateVersion7(),
                SequenceNo = index + 1
            };
            return PartialView("~/Views/Booking/_AirFlightRowTemplate.cshtml", model);
        }
        public async Task<IActionResult> PrintRTSInstructionLetter(Guid id)
        {
            var pdfBytes = await _bookingService.PrintRTSInstructionLetterPdfAsync(id);
            if (pdfBytes == null || pdfBytes.Length == 0)
                return NotFound();

            var fileName = $"RTSInstruction-{id:N}.pdf";
            return File(pdfBytes, "application/pdf");
        }



        [HttpGet]
        public async Task<IActionResult> BookingConsole(Guid id, Guid jobId)
        {
            var model = await _bookingService.GetBookingConsoleAsync(id);
            if (model is null)
                return NotFound();
            return View("~/Views/Booking/BookingManagement/BookingConsole.cshtml", model);
        }
        public async Task<IActionResult> BookingSummary(Guid bookingId, CancellationToken cancellationToken)
        {
            var vm = await _bookingService.GetBookingSummaryAsync(bookingId, cancellationToken);
            if (vm == null)
                return Content("<div class='alert alert-danger mb-0'>BL not found.</div>", "text/html");
            return PartialView("~/Views/Booking/BookingManagement/_BookingSummaryCard.cshtml", vm);
        }
        public async Task<IActionResult> GetBookingInvoices(Guid bookingId, Guid jobId, string filterType)
        {
            var vm = new BookingInvoicesVm();
            var invoices = await _bookingService.GetInvoicesByBookingIdAsync(bookingId, filterType);
            vm.Invoices = invoices;
            vm.BookingId = bookingId;
            vm.JobId = jobId;
            vm.FilterType = filterType;
            return PartialView("~/Views/Booking/BookingManagement/_BookingInvoicesGrid.cshtml", vm);
        }
        public async Task<IActionResult> GetDetentionsGrid(Guid id, CancellationToken cancellationToken, bool showInactive = false)
        {
            var vm = await _bookingService.GetDetentionsByBookingAsync(id, cancellationToken);
            if (!showInactive)
                vm.Items = vm.Items?.Where(x => x.StatusId != -1).ToList();
            vm.ShowInActive = showInactive;
            if (vm == null)
            {
                return PartialView("~/Views/Booking/BookingManagement/_DetentionsGrid.cshtml", new BookingConsoleDetentionsVm
                {
                    BookingId = id,
                });
            }
            return PartialView("~/Views/Booking/BookingManagement/_DetentionsGrid.cshtml", vm);
        }
    }
}