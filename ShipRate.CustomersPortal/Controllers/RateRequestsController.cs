using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using ShipRate.Application.DTOs;
using ShipRate.Application.Interfaces;
using ShipRate.Application.Interfaces.Cargos;
using ShipRate.Application.Interfaces.Jobs;
using ShipRate.Application.ViewModels;
using ShipRate.Domain.Entities.RateQuotation;
using ShipRate.Infrastructure.Data;
using ShipRate.WebAppCustomers.ViewModels;
using System.Data;

namespace ShipRate.WebAppCustomers.Controllers
{
    [Authorize]
    public class RateRequestsController : ApplicationController
    {
        private readonly ApplicationDbContext _context;
        private readonly IEmailService _emailService;
        private readonly ICurrentUserService _currentUserService;
        private readonly IJobLookupService _lookupService;
        private readonly IRazorViewRenderService _razorViewRenderService;
        private readonly IRateInquiryEmailService _rateInquiryEmailService;
        private readonly IJobService _jobService;
        private readonly ICargoDetailsService _cargoDetailsService;
        private readonly Application.Services.RateRequestService _rateRequestService;

        public RateRequestsController(ApplicationDbContext context, IEmailService emailService, ICurrentUserService currentUserService, IJobLookupService lookupService, IRazorViewRenderService razorViewRenderService, IRateInquiryEmailService rateInquiryEmailService, IJobService jobService, ICargoDetailsService cargoDetailsService,
            Application.Services.RateRequestService rateRequestService)
        {
            _context = context;
            _rateRequestService = rateRequestService;
            _emailService = emailService;
            _currentUserService = currentUserService;
            _lookupService = lookupService;
            _razorViewRenderService = razorViewRenderService;
            _rateInquiryEmailService = rateInquiryEmailService;
            _jobService = jobService;
            _cargoDetailsService = cargoDetailsService;
        }
        [HttpGet]
        public async Task<IActionResult> Index(RateRequestSearchDto search)
        {
            var result = await _rateRequestService.GetList(search);
            if (!result.Succeeded)
            {
                return ErrorPartialView(result.Errors);
            }

            if (Request.Headers["HX-Request"] == "true")
                return PartialView("_RateRequestTable", result.Data);
            PopulateDropDowns();
            return View(result.Data);
        }



        private void PopulateDropDowns()
        {
            ViewBag.Customers = new SelectList(_context.Customers.Include(x => x.Person), "Id", "Person.PersonName");
            ViewBag.Statuses = new SelectList(_context.RateRequestStatus, "Id", "RateRequestStatusName");
            ViewBag.Origins = new SelectList(_context.Locations.OrderBy(x => x.LocationName), "Id", "LocationName");
            ViewBag.Destinations = new SelectList(_context.Locations.OrderBy(x => x.LocationName), "Id", "LocationName");
        }



        [HttpGet]
        public async Task<IActionResult> Create()
        {


            var personId = await _currentUserService.GetPersonIdAsync();
            var customer = await _context.Customers.Where(x => x.Id == personId).Include(x => x.Person).FirstOrDefaultAsync();
            if (customer == null)
            {
                return NotFound();
            }
            var viewModel = new RateRequestFormDto
            {
                RequestDate = DateTime.Now,
                CargoReadinessDate = DateTime.Now.AddDays(7),
                CustomerId = customer.Id,
                CustomerName = customer.Person.PersonName,
                MobileNo = customer.Person.Mobile,
                EmailAddress = customer.Person.EmailAddress
            };
            PopulateDropdowns(viewModel);
            return PartialView("_RateRequestForm", viewModel);
        }
        [ValidateAntiForgeryToken]
        [HttpPost]
        public async Task<IActionResult> SaveStep1(RateRequestFormDto model)
        {

            if (!ModelState.IsValid)
            {
                PopulateDropdowns(model);
                return PartialView("_RateRequestForm", model);
            }
            var result = await _rateRequestService.CreateRateRequest(model);
            PopulateDropdowns(model);
            if (!result.Succeeded)
            {
                AddModelStateErrors(result.Errors);
                return PartialView("_RateRequestForm", model);
            }
            HxTrigger(
               new { toastSuccess = "Created Successfully", switchToCargoTab = true }
               );
            return PartialView("_RateRequestForm", model);
        }

        [HttpGet]
        public async Task<IActionResult> Edit(Guid id)
        {
            var rateRequest = await _context.RateRequests
                .Include(r => r.TransportTypes)
                .Include(r => r.Packages)
                .Include(r => r.Containers)
                .FirstOrDefaultAsync(r => r.Id == id);

            if (rateRequest == null)
                return NotFound();

            var model = new RateRequestFormDto
            {
                Id = rateRequest.Id,
                CustomerName = rateRequest.CustomerName,
                ContactName = rateRequest.ContactName,
                MobileNo = rateRequest.MobileNo,
                EmailAddress = rateRequest.EmailAddress,
                CargoHandlingTypeId = rateRequest.CargoHandlingTypeId,
                CargoNatureId = rateRequest.CargoNatureId,
                OriginId = rateRequest.OriginId,
                DestinationId = rateRequest.DestinationId,
                CargoReadinessDate = rateRequest.CargoReadinessDate.Value,
                StatusId = rateRequest.StatusId,
                Remarks = rateRequest.Remarks,
                CargoCommodityId = rateRequest.CargoCommodityId,
                CargoCommodityDescription = rateRequest.CargoCommodityDescription,
                CustomerId = rateRequest.CustomerId,
                SourceId = rateRequest.RateRequestSourceId,
                RequestDate = rateRequest.RequestDate,
                CompanyId = rateRequest.CompanyId,
                PaymentTypeId = rateRequest.PaymentTypeId,
                TradeTypeId = rateRequest.TradeTypeId,
                LoadTypeId = rateRequest.LoadTypeId,
                MovementTypeId = rateRequest.MovementTypeId,
                PickupRequired = rateRequest.PickupRequired,
                PickupAddress = rateRequest.PickupAddress,
                OriginZipCode = rateRequest.OriginZipCode,
                DeliveryRequired = rateRequest.DeliveryRequired,
                DeliveryAddress = rateRequest.DeliveryAddress,
                DestinationZipCode = rateRequest.DestinationZipCode,
                CustomsClearanceRequired = rateRequest.CustomsClearanceRequired,
                InsuranceRequired = rateRequest.InsuranceRequired,
                StuffingRequired = rateRequest.StuffingRequired,
                WarehousingRequired = rateRequest.WarehousingRequired,
                TransportTypes = rateRequest.TransportTypes.Select(t => new RateRequestTransportTypeDto
                {
                    Id = t.Id,
                    TransportTypeId = t.TransportTypeId
                }).ToList(),
                Packages = rateRequest.Packages.Select(p => new RateRequestPackageDto
                {
                    Id = p.Id,
                    PackageTypeId = p.PackageTypeId,
                    UnitCount = p.UnitCount,
                    UnitLength = p.UnitLength,
                    UnitWidth = p.UnitWidth,
                    UnitGrossWeight = p.UnitGrossWeight,
                    UnitHeight = p.UnitHeight,
                    UnitNetWeight = p.UnitNetWeight,
                    UnitVolume = p.UnitVolume
                }).ToList(),
                Containers = rateRequest.Containers.Select(c => new RateRequestContainerDto
                {
                    Id = c.Id,
                    ContainerSizeTypeId = c.ContainerSizeTypeId,
                    Counts = c.Counts
                }).ToList()
            };
            PopulateDropdowns(model);
            return PartialView("_RateRequestForm", model);
        }
        [ValidateAntiForgeryToken]
        [HttpPost]
        public async Task<IActionResult> Edit(RateRequestFormDto model)
        {
            if (!ModelState.IsValid)
            {
                PopulateDropdowns(model);
                return PartialView("_RateRequestForm", model);
            }
            var result = await _rateRequestService.EditRateRequest(model);
            if (!result.Succeeded)
            {
                AddModelStateErrors(result.Errors);
                return PartialView("_RateRequestForm", model);
            }
            HxTrigger(
                new { closeAppModal = true, toastSuccess = "Edited Successfully", rateRequestChanged = true }
                );
            return NoContent();
        }

        [HttpGet]
        public async Task<IActionResult> DeleteConfirm(Guid id)
        {
            return PartialView("_DeleteRateRequestConfirm", id);
        }

        [ValidateAntiForgeryToken]
        [HttpPost]
        public async Task<IActionResult> Delete(Guid id)
        {
            var result = await _rateRequestService.DeleteRateRequest(id);
            if (!result.Succeeded)
            {
                AddModelStateErrors(result.Errors);
                return PartialView("_DeleteRateRequestConfirm");
            }
            HxTrigger(new { rateRequestChanged = true, closeAppModal = true, toastSuccess = "Deleted Successfully" });
            return Ok();
        }


        private void PopulateDropdowns(RateRequestFormDto model)
        {
            model.Customers = new SelectList(_context.Customers.Include(x => x.Person).ToList(), "Id", "Person.PersonName");
            model.Origins = new SelectList(_context.Locations.ToList().OrderBy(x => x.LocationName), "Id", "LocationName");
            model.Destinations = new SelectList(_context.Locations.ToList().OrderBy(x => x.LocationName), "Id", "LocationName");
            model.CargoHandlingTypes = new SelectList(_context.CargoHandlingTypes.ToList(), "Id", "CargoHandlingTypeName");
            model.CargoNatures = new SelectList(_context.CargoNatures.ToList(), "Id", "CargoNatureName");
            model.LoadTypes = new SelectList(_context.LoadTypes.ToList(), "Id", "LoadTypeCode");
            model.Sources = new SelectList(_context.RateRequestSources.ToList(), "Id", "RateRequestSourceName");
            model.TradeTypes = new SelectList(_context.TradeTypes.ToList(), "Id", "TradeTypeName");
            model.PaymentTypes = new SelectList(_context.PaymentTypes.ToList(), "Id", "PaymentTypeName");
            model.MovementTypes = new SelectList(_context.MovementTypes.ToList(), "Id", "MovementTypeCode");
            model.Companies = new SelectList(_context.Companies.Include(x => x.Person).ToList(), "Id", "Person.PersonName");
            model.CargoCommodities = new SelectList(_context.CargoCommodities.ToList().OrderBy(x => x.CargoCommodityName), "Id", "CargoCommodityName");

            //model.Statuses = new SelectList(_context.RateRequestStatus.ToList(), "Id", "RateRequestStatusName");
            model.ContainerTypesSelectList = _context.ContainerSizeTypes.ToList();
            model.TransportTypesSelectList = _context.TransportTypes.ToList();
            //model.TransportTypesSelectList = _context.TransportTypes
            //        .Select(t => new SelectListItem
            //        {
            //            Value = t.Id.ToString(),
            //            Text = t.TransportTypeName
            //        });
            model.PackageTypesSelectList = _context.PackageTypes.OrderBy(x => x.PackageTypeName).ToList();
        }

        [HttpGet]
        public async Task<IActionResult> Inquiry(Guid id)
        {
            var rateRequest = await _context.RateRequests
                .Include(r => r.Inquiries)
                .Include(r => r.Origin)
                .Include(r => r.Destination)
                .FirstOrDefaultAsync(r => r.Id == id);

            if (rateRequest == null)
                return NotFound();

            var existingCarrierIds = rateRequest.Inquiries.Select(i => i.VendorId).Distinct().ToList();
            var carriers = await _context.Carriers
                .Include(x => x.Person)
                .Select(c => new CarrierItemVM
                {
                    Id = c.Id,
                    Name = c.Person.PersonName,
                    IsSelected = existingCarrierIds.Contains(c.Id)
                })
                .ToListAsync();

            var model = new RateRequestInquiryVM
            {
                RateRequestId = id,
                OriginId = rateRequest.OriginId,
                DestinationId = rateRequest.DestinationId,
                AvailableCarriers = carriers,
                Locations = await _lookupService.GetLocationsAsync(),
                TransportTypes = await _lookupService.GetTransportTypesAsync()
            };
            return PartialView("_RateRequestInquiry", model);
        }
        [ValidateAntiForgeryToken]
        [HttpPost]
        public async Task<IActionResult> SaveInquiry(RateRequestInquiryVM vm)
        {
            if (vm.SelectedCarrierIds == null || !vm.SelectedCarrierIds.Any())
                return BadRequest("Please select at least one carrier.");

            var now = DateTime.Now;
            var userId = _currentUserService.UserId; // متد دریافت کاربر جاری

            // لیست جدید برای ذخیره
            var inquiries = new List<RateRequestInquiry>();

            foreach (var vendorId in vm.SelectedCarrierIds)
            {
                // تولید شماره درخواست منحصر به فرد
                //var inquiryNo = await _numberGeneratorService.GenerateAsync("INQ");
                var inquiryNo = "INQ-001";
                inquiries.Add(new RateRequestInquiry
                {
                    Id = Guid.CreateVersion7(),
                    RateRequestId = vm.RateRequestId,
                    InquiryNo = inquiryNo,
                    InquiryDate = now,
                    VendorId = vendorId,
                    OriginId = vm.OriginId,
                    DestinationId = vm.DestinationId,
                    TransportTypeId = vm.TransportTypeId ?? 1,
                    RemarksInInquiry = vm.Remarks,

                    // مقادیر پیش‌فرض
                    StatusId = 1, // یا ID وضعیت "Pending"
                    //IsActive = true,
                    IsDeleted = false,
                    CreatedAt = now,
                    CreateUserId = userId
                });
            }

            _context.RateRequestInquiries.AddRange(inquiries);
            await _context.SaveChangesAsync();

            Response.Headers["HX-Trigger"] = $"{{\"inquirySaved\":{{\"rateRequestId\":\"{vm.RateRequestId}\"}}}}";
            return Ok();
        }


        [HttpGet]
        public async Task<IActionResult> GetResponds(Guid? rateRequestId, Guid? InquiryId)
        {
            var bq = await _context.BuyQuotes
                .Include(r => r.RateRequestInquiry)
                .ThenInclude(r => r.Vendor)
                .Include(r => r.RateRequestInquiry)
                .ThenInclude(r => r.RateRequest)
                .Include(r => r.Carrier)
                .Include(r => r.Vendor)
                .Where(r => r.RateRequestInquiry.RateRequestId == rateRequestId && r.StatusId != 1)
                .Select(r => new BuyQuoteListViewModel
                {
                    QuotationNo = r.QuotationNo,
                    QuotationDate = r.QuotationDate,
                    CarrierName = r.Carrier.PersonName,
                    ETA = r.ETA,
                    ETD = r.ETD,
                    VendorName = r.Vendor.PersonName,
                    VehicleName = r.VehicleName,
                    TransportMovementNo = r.TransportMovementNo,
                    ValidUntil = r.ValidUntil,
                    Id = r.Id,
                    CreateUserName = r.CreatedByUser.FullName,
                    UpdateUserName = r.EditedByUser != null ? r.EditedByUser.FullName : ""
                })
                .ToListAsync();

            if (bq == null)
                return NotFound();

            return PartialView("_BuyQuoteList", bq);
        }
        [HttpGet]
        public async Task<IActionResult> GetRespondItems(Guid? quoteId)
        {
            var bq = await _context.BuyQuoteItems
                .Include(r => r.ChargeType)
                .Include(r => r.ContainerSizeType)
                .Include(r => r.Currency)
                .Where(r => r.BuyQuoteId == quoteId)
                .Select(r => new BuyQuoteItemListViewModel
                {
                    ChargeTypeCode = r.ChargeType.ChargeTypeCode,
                    ChargeTypeName = r.ChargeType.ChargeTypeName,
                    Quantity = r.Quantity,
                    UnitRate = r.UnitRate,
                    TotalAmount = r.TotalAmount,
                    CurrencyCode = r.Currency.CurrencyCode,
                    CurrencyName = r.Currency.CurrencyName,
                    SizeTypeShortName = r.ContainerSizeType.ShortName,
                    Remarks = r.Remarks,
                })
                .ToListAsync();
            if (bq == null)
                return NotFound();

            return PartialView("_BuyQuoteItemsList", bq);
        }

        [HttpGet]
        public async Task<IActionResult> GetRespondDetails(Guid? quoteId)
        {
            var bq = await _context.BuyQuotes
                .Include(r => r.RateRequestInquiry)
                .ThenInclude(r => r.Vendor)
                .Include(r => r.RateRequestInquiry)
                .ThenInclude(r => r.RateRequest)
                .Include(r => r.Carrier)
                .Include(r => r.Vendor)
                .Where(r => r.Id == quoteId)
                .Select(r => new BuyQuoteListViewModel
                {
                    QuotationNo = r.QuotationNo,
                    QuotationDate = r.QuotationDate,
                    CarrierName = r.Carrier.PersonName,
                    ETA = r.ETA,
                    ETD = r.ETD,
                    VendorName = r.Vendor.PersonName,
                    VehicleName = r.VehicleName,
                    TransportMovementNo = r.TransportMovementNo,
                    ValidUntil = r.ValidUntil,
                    Id = r.Id,
                    CreateUserName = r.CreatedByUser.FullName,
                    UpdateUserName = r.EditedByUser != null ? r.EditedByUser.FullName : "",
                    InqNo = r.RateRequestInquiry.InquiryNo,
                    InqDate = r.RateRequestInquiry.InquiryDate,
                    Remarks = r.Remarks,
                    TransitDays = r.TransitDays,
                    OptionsServiceName = r.OptionsServiceName,
                    IsDirect = r.IsDirect,
                })
                .FirstOrDefaultAsync();
            if (bq == null)
                return NotFound();

            var bqi = await _context.BuyQuoteItems
                .Include(x => x.ChargeType)
                .Include(x => x.ContainerSizeType)
                .Include(x => x.Currency)
                .Where(x => x.BuyQuoteId == quoteId)
                .Select(x => new BuyQuoteItemListViewModel
                {
                    BuyQuoteId = quoteId.Value,
                    ChargeTypeCode = x.ChargeType.ChargeTypeCode,
                    ChargeTypeName = x.ChargeType.ChargeTypeName,
                    CurrencyCode = x.Currency.CurrencyCode,
                    CurrencyName = x.Currency.CurrencyName,
                    Quantity = x.Quantity,
                    Remarks = x.Remarks,
                    SizeTypeShortName = x.ContainerSizeType.ShortName,
                    TotalAmount = x.TotalAmount,
                    UnitRate = x.UnitRate,
                    Id = x.Id
                }).ToListAsync();

            bq.Items = bqi;
            return PartialView("~/Views/RateRequests/_BuyQuoteDetails.cshtml", bq);
        }

        [HttpGet]
        public async Task<IActionResult> GetSellQuotes(Guid rateRequestId)
        {


            if (rateRequestId == Guid.Empty)
            {
                return Content("");
            }
            var result = await _rateRequestService.GetSellQuotes(rateRequestId);
            if (!result.Succeeded)
            {
                AddModelStateErrors(result.Errors);
            }

            return PartialView("_SellQuoteList", result.Data);
        }
        [HttpGet]
        public async Task<IActionResult> GetSellQuoteItems(Guid? quoteId)
        {
            var result = await _rateRequestService.GetSellQuoteItems(quoteId);
            if (!result.Succeeded)
            {
                AddModelStateErrors(result.Errors);
            }

            return PartialView("_SellQuoteItemsList", result.Data);
        }


        [HttpGet]
        public async Task<IActionResult> GetSellQuoteDetails(Guid? quoteId)
        {
            var result = await _rateRequestService.GetSellQuoteDetails(quoteId);
            if (!result.Succeeded)
            {
                AddModelStateErrors(result.Errors);
            }
            return PartialView("~/Views/RateRequests/_SellQuoteDetails.cshtml", result.Data);
        }



        [HttpGet]
        public async Task<IActionResult> AssignSalesUser(Guid id)
        {
            var rr = await _context.RateRequests
                .Include(c => c.AssignedSalesUser)
                .FirstOrDefaultAsync(c => c.Id == id);

            if (rr == null) return NotFound();

            var vm = new AssignSalesUserViewModel
            {
                CustomerId = rr.Id,
                PersonName = rr.RequestNumber,
                CurrentSalesUserId = rr.AssignedSalesUserId,
                CurrentSalesUserName = rr.AssignedSalesUser?.FullName,
                SelectedSalesUserId = rr.AssignedSalesUserId,
                SalesUsers = await _context.Users
                    .Where(u => u.IsActive)
                    .OrderBy(u => u.FullName)
                    .Select(u => new SalesUserDropdownItem { Id = u.Id, FullName = u.FullName })
                    .ToListAsync()
            };

            ViewBag.SalesUsers = await _context.Users
                .Where(u => u.IsActive)
                .OrderBy(u => u.FullName)
                .ToListAsync();
            return PartialView("_AssignSalesUser", vm);
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AssignSalesUser(AssignSalesUserDto vm)
        {
            var result = await _rateRequestService.AssignSalesUser(vm);
            if (!result.Succeeded)
            {
                AddModelStateErrors(result.Errors);
                return PartialView("_AssignSalesUser", vm);
            }
            return Content("");
        }
        [HttpGet]
        public async Task<IActionResult> RejectSellQuoteInApp(Guid quoteId)
        {
            var sq = await _context.SellQuotes
                .AsNoTracking()
                .FirstOrDefaultAsync(x => x.Id == quoteId);

            if (sq == null)
                return NotFound();

            var vm = new RejectSellQuoteVm
            {
                SellQuoteId = sq.Id
            };

            return PartialView("_RejectSellQuote", vm);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> RejectSellQuoteInApp(RejectSellQuoteVm vm)
        {
            if (!ModelState.IsValid)
                return PartialView("_RejectSellQuote", vm);
            var result = await _rateRequestService.RejectSellQuote(vm);
            if (!result.Succeeded)
            {
                AddModelStateErrors(result.Errors);
                return PartialView("_RejectSellQuote", vm);
            }
            HxTrigger(new { sellQuoteUpdated = true, toastSuccess = "Rejected successfully", closeAppModal = true });
            return NoContent();
        }






        [HttpGet]
        public async Task<IActionResult> AcceptSellQuoteInApp(Guid quoteId)
        {
            var sq = await _context.SellQuotes
                .AsNoTracking()
                .FirstOrDefaultAsync(x => x.Id == quoteId);

            if (sq == null)
                return NotFound();

            var vm = new AcceptSellQuoteVm
            {
                SellQuoteId = sq.Id
            };

            return PartialView("_AcceptSellQuote", vm);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AcceptSellQuoteInApp(AcceptSellQuoteVm vm)
        {
            if (!ModelState.IsValid)
                return PartialView("_AcceptSellQuote", vm);

            var result = await _rateRequestService.AcceptSellQuote(vm);
            if (!result.Succeeded)
            {
                AddModelStateErrors(result.Errors);
                return PartialView("_AcceptSellQuote", vm);
            }
            HxTrigger(new { sellQuoteUpdated = true, closeAppModal = true, toastSuccess = "Quotation Accepted by Customer" });
            return NoContent();
        }
        //[HttpGet]
        //public async Task<IActionResult> GenerateJobFromQuote(Guid quoteId)
        //{
        //    var result = await _jobService.GenerateJobFromAcceptedSellQuoteAsync(quoteId);
        //    return NoContent();
        //}


        // [HttpGet]
        // public async Task<IActionResult> SearchParties(
        //   string? term,
        //   string targetHiddenInputId = "CPartyId",
        //   string targetTextInputId = "CPartySearchText",
        //   CancellationToken cancellationToken = default)
        //{
        //     var items = await _jobService.SearchPartiesAsync(term);

        //     ViewBag.TargetHiddenInputId = targetHiddenInputId;
        //     ViewBag.TargetTextInputId = targetTextInputId;

        //     return PartialView("_PartySearchResult", items);
        // }
    }
}