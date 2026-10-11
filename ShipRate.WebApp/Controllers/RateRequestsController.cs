using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using ShipRate.Application.Interfaces;
using ShipRate.Application.Interfaces.Cargos;
using ShipRate.Application.Interfaces.Jobs;
using ShipRate.Application.ViewModels;
using ShipRate.Common.Utilities;
using ShipRate.Domain.Entities.RateQuotation;
using ShipRate.Infrastructure.Data;
using ShipRate.WebApp.ViewModels;
using ShipRate.WebApp.ViewModels.CustomersVms;
using System.Data;
using System.Text.Json;

namespace ShipRate.WebApp.Controllers
{
    [Authorize]
    public class RateRequestsController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly IEmailService _emailService;
        private readonly ICurrentUserService _currentUserService;
        private readonly IJobLookupService _lookupService;
        private readonly IRazorViewRenderService _razorViewRenderService;
        private readonly IRateInquiryEmailService _rateInquiryEmailService;
        private readonly IJobService _jobService;
        private readonly ICargoDetailsService _cargoDetailsService;
        private readonly IActivityLogService activityLogService;
        private readonly Application.Services.RateRequestService _rateRequestService;
        public RateRequestsController(ApplicationDbContext context, IEmailService emailService, ICurrentUserService currentUserService, IJobLookupService lookupService, IRazorViewRenderService razorViewRenderService, IRateInquiryEmailService rateInquiryEmailService, IJobService jobService, ICargoDetailsService cargoDetailsService, Application.Services.RateRequestService rateRequestService, IActivityLogService activityLogService)
        {
            _context = context;
            _emailService = emailService;
            _currentUserService = currentUserService;
            _lookupService = lookupService;
            _razorViewRenderService = razorViewRenderService;
            _rateInquiryEmailService = rateInquiryEmailService;
            _jobService = jobService;
            _cargoDetailsService = cargoDetailsService;
            _rateRequestService = rateRequestService;
            this.activityLogService = activityLogService;
        }

        private void HxTrigger(object triggers)
        {
            Response.Headers["HX-Trigger"] = JsonSerializer.Serialize(triggers);
        }

        [HttpGet]
        [Authorize("Operations.RateRequests.View")]
        public async Task<IActionResult> Index(RateRequestSearchViewModel search)
        {
            var userId = _currentUserService.UserId;
            var query = _context.RateRequests
                .Include(x => x.Origin)
                .Include(x => x.Destination)
                .AsQueryable();

            if (_currentUserService.IsOnlySalesUser())
            {
                query = query.Where(p => p.AssignedSalesUserId == userId || p.CreateUserId == userId);
            }




            // Search filters
            if (!string.IsNullOrWhiteSpace(search.CustomerName))
                query = query.Where(r => r.CustomerName.Contains(search.CustomerName));

            if (!string.IsNullOrWhiteSpace(search.ContactName))
                query = query.Where(r => r.ContactName.Contains(search.ContactName));

            if (!string.IsNullOrWhiteSpace(search.MobileNo))
                query = query.Where(r => r.MobileNo.Contains(search.MobileNo));

            if (!string.IsNullOrWhiteSpace(search.EmailAddress))
                query = query.Where(r => r.EmailAddress.Contains(search.EmailAddress));

            if (search.StatusId.HasValue)
                query = query.Where(r => r.StatusId == search.StatusId.Value);

            //if (search.OriginId.HasValue)
            //    query = query.Where(r => r.OriginId == search.OriginId.Value);

            //if (search.DestinationId.HasValue)
            //    query = query.Where(r => r.DestinationId == search.DestinationId.Value);

            if (!string.IsNullOrWhiteSpace(search.OriginName))
                query = query.Where(r => r.Origin.LocationName.Contains(search.OriginName));

            if (!string.IsNullOrWhiteSpace(search.DestinationName))
                query = query.Where(r => r.Destination.LocationName.Contains(search.DestinationName));

            if (search.CargoReadinessDateFrom.HasValue)
                query = query.Where(r => r.CargoReadinessDate >= search.CargoReadinessDateFrom.Value);

            if (search.CargoReadinessDateTo.HasValue)
                query = query.Where(r => r.CargoReadinessDate <= search.CargoReadinessDateTo.Value);


            //if (search.CustomsClearanceRequired.HasValue)
            //{
            //    query = query.Where(x =>
            //        x.CustomsClearanceRequired == search.CustomsClearanceRequired.Value);
            //}

            //if (search.InsuranceRequired.HasValue)
            //{
            //    query = query.Where(x =>
            //        x.InsuranceRequired == search.InsuranceRequired.Value);
            //}

            //if (search.WarehousingRequired.HasValue)
            //{
            //    query = query.Where(x =>
            //        x.WarehousingRequired == search.WarehousingRequired.Value);
            //}

            //if (search.StuffingRequired.HasValue)
            //{
            //    query = query.Where(x =>
            //        x.StuffingRequired == search.StuffingRequired.Value);
            //}

            //if (search.ExternalTransitRequired.HasValue)
            //{
            //    query = query.Where(x =>
            //        x.ExternalTransitRequired == search.ExternalTransitRequired.Value);
            //}

            //if (search.InternalTransitRequired.HasValue)
            //{
            //    query = query.Where(x =>
            //        x.InternalTransitRequired == search.InternalTransitRequired.Value);
            //}


            query = query
                .WhereIf(search.CustomsClearanceRequired.HasValue,
                    x => x.CustomsClearanceRequired == search.CustomsClearanceRequired!.Value)
                .WhereIf(search.PickupRequired.HasValue,
                    x => x.PickupRequired == search.PickupRequired!.Value)
                .WhereIf(search.DeliveryRequired.HasValue,
                    x => x.DeliveryRequired == search.DeliveryRequired!.Value)
                .WhereIf(search.InsuranceRequired.HasValue,
                    x => x.InsuranceRequired == search.InsuranceRequired!.Value)

                .WhereIf(search.WarehousingRequired.HasValue,
                    x => x.WarehousingRequired == search.WarehousingRequired!.Value)
                .WhereIf(search.StuffingRequired.HasValue,
                    x => x.StuffingRequired == search.StuffingRequired!.Value)
                .WhereIf(search.ExternalTransitRequired.HasValue,
                    x => x.ExternalTransitRequired == search.ExternalTransitRequired!.Value)
                .WhereIf(search.InternalTransitRequired.HasValue,
                    x => x.InternalTransitRequired == search.InternalTransitRequired!.Value);

            // Total count for pagination
            var totalCount = await query.CountAsync();

            // Pagination
            var items = await query
                .OrderByDescending(r => r.CreatedAt)
                .Skip((search.PageNumber - 1) * search.PageSize)
                .Take(search.PageSize)
                .Select(r => new RateRequestItemViewModel
                {
                    Id = r.Id,
                    CustomerName = r.CustomerName,
                    ContactName = r.ContactName,
                    MobileNo = r.MobileNo,
                    EmailAddress = r.EmailAddress,
                    OriginName = r.Origin != null ? r.Origin.LocationName : null,
                    DestinationName = r.Destination != null ? r.Destination.LocationName : null,
                    CargoReadinessDate = r.CargoReadinessDate.Value,
                    StatusName = r.RateRequestStatus != null ? r.RateRequestStatus.RateRequestStatusName : null,
                    CreatedAt = r.CreatedAt,
                    RequestNo = r.RequestNumber,
                    RequestDate = r.RequestDate,
                    CreatedByUserName = r.CreatedByUser.FullName,
                    LastEditedByUserName = r.LastEditedByUser.FullName,
                    SalesUserName = r.AssignedSalesUser == null ? "-" : r.AssignedSalesUser.FullName
                })
                .ToListAsync();

            var viewModel = new RateRequestListViewModel
            {
                Items = items,
                Search = search,
                Pagination = new PaginationViewModel
                {
                    PageNumber = search.PageNumber,
                    PageSize = search.PageSize,
                    TotalCount = totalCount
                }
            };


            // Populate dropdowns for search form
            ViewBag.Customers = new SelectList(_context.Customers.Include(x => x.Person), "Id", "Person.PersonName");
            ViewBag.Statuses = new SelectList(_context.RateRequestStatus, "Id", "RateRequestStatusName");

            //ViewBag.Origins = new SelectList(_context.Locations.OrderBy(x => x.LocationName), "Id", "LocationName");
            //ViewBag.Destinations = new SelectList(_context.Locations.OrderBy(x => x.LocationName), "Id", "LocationName");


            // HTMX partial return
            if (Request.Headers["HX-Request"] == "true")
                return PartialView("_RateRequestTable", viewModel);

            return View(viewModel);
        }

        [HttpGet]
        [Authorize("Operations.RateRequests.Edit")]
        public async Task<IActionResult> Create()
        {
            var viewModel = new RateRequestFormViewModel
            {
                RequestDate = DateTime.Now,
                CargoReadinessDate = DateTime.Now.AddDays(7)
            };
            PopulateDropdowns(viewModel);
            return PartialView("_RateRequestForm", viewModel);
        }
        [ValidateAntiForgeryToken]
        [HttpPost]
        [Authorize("Operations.RateRequests.Edit")]
        public async Task<IActionResult> SaveStep1(RateRequestFormViewModel model)
        {
            if (!ModelState.IsValid)
            {
                PopulateDropdowns(model);
                return PartialView("_RateRequestForm", model);
            }
            var requestNumber = await GenerateRateRequestNumberAsync();
            var rateRequest = new RateRequest
            {
                Id = Guid.CreateVersion7(),
                CompanyId = model.CompanyId.Value,
                CustomerId = model.CustomerId,
                LeadId = model.LeadId,
                CustomerContactId = model.CustomerContactId,
                CustomerName = model.CustomerName,
                ContactName = model.ContactName,
                MobileNo = model.MobileNo,
                EmailAddress = model.EmailAddress,
                CargoHandlingTypeId = model.CargoHandlingTypeId,
                CargoNatureId = model.CargoNatureId,
                CargoCommodityId = model.CargoCommodityId,
                CargoCommodityDescription = model.CargoCommodityDescription,
                IsHazardous = model.IsHazardous,
                DGId = model.DGId,
                DGDescription = model.DGDescription,
                OriginHSCode = model.OriginHSCode,
                DestinationHSCode = model.DestinationHSCode,
                LoadTypeId = model.LoadTypeId,
                OriginId = model.OriginId,
                DestinationId = model.DestinationId,
                TradeTypeId = model.TradeTypeId,
                CargoReadinessDate = model.CargoReadinessDate,
                QuotationPriorityId = model.QuotationPriorityId,
                HasEx1 = model.HasEx1,
                TransitHasTranshipment = model.TransitHasTranshipment,
                SellToCountryId = model.SellToCountryId,
                PickupRequired = model.PickupRequired,
                PickupAddress = model.PickupAddress,
                ExternalTransitRequired = model.ExternalTransitRequired,
                InternalTransitRequired = model.InternalTransitRequired,
                OriginZipCode = model.OriginZipCode,
                DeliveryRequired = model.DeliveryRequired,
                DeliveryAddress = model.DeliveryAddress,
                DestinationZipCode = model.DestinationZipCode,
                CargoValue = model.CargoValue,
                CurrencyId = model.CurrencyId,
                SalesManagerId = model.SalesManagerId,
                AssignedSalesUserId = model.SalesmanId,
                StatusId = 1,
                Remarks = model.Remarks,
                RequestNumber = requestNumber,
                RequestDate = model.RequestDate,
                RateRequestSourceId = model.SourceId,
                PaymentTypeId = model.PaymentTypeId,
                MovementTypeId = model.MovementTypeId,
                CustomsClearanceRequired = model.CustomsClearanceRequired,
                StuffingRequired = model.StuffingRequired,
                WarehousingRequired = model.WarehousingRequired,
                InsuranceRequired = model.InsuranceRequired,
                CreateUserId = _currentUserService.UserId,
                CreatedAt = DateTime.Now,
                Introducer = model.Introducer,

            };
            rateRequest.StatusId = 3;
            if (model.CustomerId != null)
            {
                var customer = _context.Customers.Where(x => x.Id == model.CustomerId).FirstOrDefault();
                if (customer != null)
                {
                    if (customer.SalesUserId != null)
                    {
                        rateRequest.StatusId = 4;
                        rateRequest.AssignedSalesUserId = customer.SalesUserId;
                        rateRequest.AssignedSalesUserDate = DateTime.Now;
                    }
                }
            }
            _context.RateRequests.Add(rateRequest);
            // TransportTypes
            foreach (var t in model.TransportTypes)
            {
                _context.RateRequestTransportTypes.Add(new RateRequestTransportType
                {
                    Id = Guid.CreateVersion7(),
                    RateRequestId = rateRequest.Id,
                    Sequence = t.Sequence,
                    TransportTypeId = t.TransportTypeId,
                    CreateUserId = _currentUserService.UserId,
                    CreatedAt = DateTime.Now,
                });
            }

            // Packages
            foreach (var p in model.Packages)
            {
                _context.RateRequestPackages.Add(new RateRequestPackage
                {
                    Id = Guid.CreateVersion7(),
                    RateRequestId = rateRequest.Id,
                    PackageTypeId = p.PackageTypeId,
                    UnitLength = p.UnitLength,
                    UnitWidth = p.UnitWidth,
                    UnitHeight = p.UnitHeight,
                    LengthMeasurementUnitId = p.LengthMeasurementUnitId,
                    UnitVolume = p.UnitVolume,
                    VolumeMeasurementUnitId = p.VolumeMeasurementUnitId,
                    UnitGrossWeight = p.UnitGrossWeight,
                    UnitNetWeight = p.UnitNetWeight,
                    WeightMeasurementUnitId = p.WeightMeasurementUnitId,
                    UnitCount = p.UnitCount,
                    TotalGrossWeight = p.TotalGrossWeight,
                    TotalNetWeight = p.TotalNetWeight,
                    TotalVolume = p.TotalVolume,
                    TotalLength = p.TotalLength,
                    TotalWidth = p.TotalWidth,
                    TotalHeight = p.TotalHeight,
                    Remarks = p.Remarks,
                    CreateUserId = _currentUserService.UserId,
                    CreatedAt = DateTime.Now,
                });
            }

            // Containers
            foreach (var c in model.Containers)
            {
                _context.RateRequestContainers.Add(new RateRequestContainer
                {
                    Id = Guid.CreateVersion7(),
                    RateRequestId = rateRequest.Id,
                    ContainerSizeTypeId = c.ContainerSizeTypeId,
                    Counts = c.Counts,
                    Remarks = c.Remarks,
                    CreateUserId = _currentUserService.UserId,
                    CreatedAt = DateTime.Now,
                });
            }

            await _context.SaveChangesAsync();
            await activityLogService.LogHistoryAsync(
                new CreateActivityLogRequest
                {
                    ActivityCategoryId = 2,
                    ActivityDateTime = DateTime.Now,
                    Title = "Rate Request Created",
                    Description = "Rate Request Registered",
                    EntityTypeId = 8,
                    EntityId = rateRequest.Id,
                    FromStatusEntityType = "RateRequestStatus",
                    ToStatusId = 3,
                    PerformedByUserId = _currentUserService.UserId,
                    IsSystemGenerated = true,
                }
                );
            //var items = await _cargoDetailsService.GetListAsync(new Application.ViewModels.CargoDetailFilterVm { RateRequestId = rateRequest.Id });
            //ViewBag.Filter = new Application.ViewModels.CargoDetailFilterVm { RateRequestId = rateRequest.Id };
            //return PartialView("~/Views/CargoDetails/_CargoDetailsPanel.cshtml", items);
            model.Id = rateRequest.Id;
            PopulateDropdowns(model);

            //Response.Headers["HX-Trigger"] = "switchToCargoTab";
            //Response.Headers["HX-Trigger"] = "{\"rateRequestChanged\": true, \"switchToCargoTab\": true}";

            HxTrigger(new
            {
                Step1Saved = true,
                switchToCargoTab = true,
                toastSuccess = "Rate Request Created Successfully!!!!",
            });





            return PartialView("_RateRequestForm", model);
        }
        [HttpGet]
        [Authorize("Operations.RateRequests.Edit")]
        public async Task<IActionResult> Edit(Guid id)
        {
            var rateRequest = await _context.RateRequests
                .Include(r => r.TransportTypes)
                .Include(r => r.Packages)
                .Include(r => r.Containers)
                .Include(r => r.Origin)
                .Include(r => r.Destination)
                .FirstOrDefaultAsync(r => r.Id == id);

            if (rateRequest == null)
                return NotFound();

            var model = new RateRequestFormViewModel
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
                OriginName = rateRequest.Origin.LocationName,
                DestinationName = rateRequest.Destination.LocationName,
                CargoReadinessDate = rateRequest.CargoReadinessDate.Value,
                StatusId = rateRequest.StatusId,
                Remarks = rateRequest.Remarks,
                CargoCommodityId = rateRequest.CargoCommodityId,
                CargoCommodityDescription = rateRequest.CargoCommodityDescription,
                CustomerId = rateRequest.CustomerId,
                SourceId = rateRequest.RateRequestSourceId,
                CargoValue = rateRequest.CargoValue,
                CurrencyId = rateRequest.CurrencyId,
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
                InternalTransitRequired = rateRequest.InternalTransitRequired == null ? false : rateRequest.InternalTransitRequired.Value,
                ExternalTransitRequired = rateRequest.ExternalTransitRequired == null ? false : rateRequest.ExternalTransitRequired.Value,
                Introducer = rateRequest.Introducer,
                TransportTypes = rateRequest.TransportTypes.Select(t => new RateRequestTransportTypeViewModel
                {
                    Id = t.Id,
                    TransportTypeId = t.TransportTypeId
                }).ToList(),
                Packages = rateRequest.Packages.Select(p => new RateRequestPackageViewModel
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
                Containers = rateRequest.Containers.Select(c => new RateRequestContainerViewModel
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
        [Authorize("Operations.RateRequests.Edit")]
        public async Task<IActionResult> Edit(RateRequestFormViewModel model)
        {
            if (!ModelState.IsValid)
            {
                PopulateDropdowns(model);
                return PartialView("_RateRequestForm", model);
            }

            var rateRequest = await _context.RateRequests
                .Include(r => r.TransportTypes)
                .Include(r => r.Packages)
                .Include(r => r.Containers)
                .FirstOrDefaultAsync(r => r.Id == model.Id);

            if (rateRequest == null)
                return NotFound();

            rateRequest.CustomerName = model.CustomerName;
            rateRequest.ContactName = model.ContactName;
            rateRequest.MobileNo = model.MobileNo;
            rateRequest.EmailAddress = model.EmailAddress;
            rateRequest.CargoHandlingTypeId = model.CargoHandlingTypeId;
            rateRequest.CargoNatureId = model.CargoNatureId;
            rateRequest.OriginId = model.OriginId;
            rateRequest.DestinationId = model.DestinationId;
            rateRequest.CargoReadinessDate = model.CargoReadinessDate;
            rateRequest.PaymentTypeId = model.PaymentTypeId;
            rateRequest.TradeTypeId = model.TradeTypeId;
            rateRequest.MovementTypeId = model.MovementTypeId;
            rateRequest.Remarks = model.Remarks;

            rateRequest.PickupRequired = model.PickupRequired;
            rateRequest.PickupAddress = model.PickupAddress;
            rateRequest.OriginZipCode = model.OriginZipCode;

            rateRequest.DeliveryRequired = model.DeliveryRequired;
            rateRequest.DeliveryAddress = model.DeliveryAddress;
            rateRequest.DestinationZipCode = model.DestinationZipCode;
            rateRequest.InternalTransitRequired = model.InternalTransitRequired;
            rateRequest.ExternalTransitRequired = model.ExternalTransitRequired;
            rateRequest.CustomsClearanceRequired = model.CustomsClearanceRequired;
            rateRequest.InsuranceRequired = model.InsuranceRequired;
            rateRequest.WarehousingRequired = model.WarehousingRequired;
            rateRequest.StuffingRequired = model.StuffingRequired;
            rateRequest.Introducer = model.Introducer;
            rateRequest.CargoValue = model.CargoValue;
            rateRequest.CurrencyId = model.CurrencyId;

            rateRequest.EditUserId = _currentUserService.UserId;
            rateRequest.UpdatedAt = DateTime.Now;
            // حذف TransportTypes قدیمی
            _context.RateRequestTransportTypes.RemoveRange(rateRequest.TransportTypes);

            // اضافه کردن TransportTypes جدید
            if (model.TransportTypes?.Any() == true)
            {
                var seq = 1;
                foreach (var tt in model.TransportTypes)
                {
                    rateRequest.TransportTypes.Add(new RateRequestTransportType
                    {
                        RateRequestId = rateRequest.Id,
                        TransportTypeId = tt.TransportTypeId,
                        CreatedAt = DateTime.Now,
                        CreateUserId = _currentUserService.UserId,
                        Sequence = seq,
                        IsDeleted = false
                    });
                    seq++;
                }
            }
            _context.RateRequestContainers.RemoveRange(rateRequest.Containers);
            // اضافه کردن Containers جدید
            if (model.Containers?.Any() == true)
            {
                foreach (var c in model.Containers)
                {
                    rateRequest.Containers.Add(new RateRequestContainer
                    {
                        RateRequestId = rateRequest.Id,
                        ContainerSizeTypeId = c.ContainerSizeTypeId,
                        Counts = c.Counts,
                        IsDeleted = false,
                        CreatedAt = DateTime.Now,
                        CreateUserId = _currentUserService.UserId,
                    });
                }
            }
            await _context.SaveChangesAsync();
            Response.Headers["HX-Trigger"] = "rateRequestChanged";
            return NoContent();
        }

        [ValidateAntiForgeryToken]
        [HttpPost]
        [Authorize("Operations.RateRequests.Edit")]
        public async Task<IActionResult> Delete(Guid id)
        {
            var rateRequest = await _context.RateRequests
                .Include(r => r.TransportTypes)
                .Include(r => r.Packages)
                .Include(r => r.Containers)
                .FirstOrDefaultAsync(r => r.Id == id);

            if (rateRequest == null)
                return NotFound();

            _context.RateRequestTransportTypes.RemoveRange(rateRequest.TransportTypes);
            _context.RateRequestPackages.RemoveRange(rateRequest.Packages);
            _context.RateRequestContainers.RemoveRange(rateRequest.Containers);
            _context.RateRequests.Remove(rateRequest);

            await _context.SaveChangesAsync();

            Response.Headers["HX-Trigger"] = "rateRequestChanged";
            return Ok();
        }
        private void PopulateDropdowns(RateRequestFormViewModel model)
        {
            model.CargoHandlingTypes = new SelectList(_context.CargoHandlingTypes.ToList(), "Id", "CargoHandlingTypeName");
            model.CargoNatures = new SelectList(_context.CargoNatures.ToList(), "Id", "CargoNatureName");
            model.LoadTypes = new SelectList(_context.LoadTypes.ToList(), "Id", "LoadTypeCode");
            model.Sources = new SelectList(_context.RateRequestSources.ToList(), "Id", "RateRequestSourceName");
            model.TradeTypes = new SelectList(_context.TradeTypes.ToList(), "Id", "TradeTypeName");
            model.PaymentTypes = new SelectList(_context.PaymentTypes.ToList(), "Id", "PaymentTypeName");
            model.MovementTypes = new SelectList(_context.MovementTypes.ToList(), "Id", "MovementTypeCode");
            model.Companies = new SelectList(_context.Companies.Include(x => x.Person).ToList(), "Id", "Person.PersonName");
            model.CargoCommodities = new SelectList(_context.CargoCommodities.ToList().OrderBy(x => x.CargoCommodityName), "Id", "CargoCommodityName");
            model.Currencies = new SelectList(_context.Currencies.ToList().OrderBy(x => x.Id), "Id", "CurrencyCode");
            model.ContainerTypesSelectList = _context.ContainerSizeTypes.ToList();
            model.TransportTypesSelectList = _context.TransportTypes.ToList();
            model.PackageTypesSelectList = _context.PackageTypes.OrderBy(x => x.PackageTypeName).ToList();
        }

        [HttpGet]
        [Authorize("Operations.RateRequests.Edit")]
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
                //Locations = await _lookupService.GetLocationsAsync(),
                TransportTypes = await _lookupService.GetTransportTypesAsync(),
                Locations = await _lookupService.GetLocationsAsync()
            };
            return PartialView("_RateRequestInquiry", model);
        }
        [ValidateAntiForgeryToken]
        [HttpPost]
        [Authorize("Operations.RateRequests.Edit")]
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
        [Authorize("Operations.RateRequests.View")]
        public async Task<IActionResult> GetResponds(Guid? rateRequestId, Guid? InquiryId)
        {
            var rr = await _context.RateRequests.Where(x => x.Id == rateRequestId).FirstOrDefaultAsync();
            ViewBag.RequestNo = rr.RequestNumber;
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
        [Authorize("Operations.RateRequests.View")]
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
        [Authorize("Operations.RateRequests.View")]
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
        [Authorize("Operations.RateRequests.View")]
        public async Task<IActionResult> GetSellQuotes(Guid rateRequestId)
        {

            var rr = await _context.RateRequests.Where(x => x.Id == rateRequestId).FirstOrDefaultAsync();
            ViewBag.RequestNo = rr.RequestNumber;

            var bq = await _context.SellQuotes
                .Include(r => r.RateRequest)
                .Where(r => r.RateRequestId == rateRequestId)
                .Select(r => new SellQuoteListViewModel
                {
                    QuotationNo = r.QuotationNo,
                    QuotationDate = r.QuotationDate,
                    ETA = r.ETA,
                    ETD = r.ETD,
                    TransportMovementNo = r.MovementNumbers,
                    VehicleName = r.VehicleNames,
                    ValidUntil = r.ValidUntil,
                    Id = r.Id,
                    CreateUserName = r.CreatedByUser.FullName,
                    UpdateUserName = r.LastEditUser != null ? r.LastEditUser.FullName : "",
                    StatusId = r.StatusId,
                    Status = r.Status.QuotationStatusName
                })
                .ToListAsync();

            if (bq == null)
                return NotFound();

            var model = new SellQuoteListIndex
            {
                RateRequestId = rateRequestId,
                SellQuotesList = bq
            };
            return PartialView("_SellQuoteList", model);
        }
        [HttpGet]
        [Authorize("Operations.RateRequests.View")]
        public async Task<IActionResult> GetSellQuoteItems(Guid? quoteId)
        {
            var bq = await _context.SellQuoteItems
                .Include(r => r.ChargeType)
                .Include(r => r.ContainerSizeType)
                .Include(r => r.BuyCurrency)
                .Include(r => r.SellCurrency)
                .Where(r => r.SellQuoteId == quoteId)
                .Select(r => new SellQuoteItemListViewModel
                {
                    ChargeTypeCode = r.ChargeType.ChargeTypeCode,
                    ChargeTypeName = r.ChargeType.ChargeTypeName,
                    Quantity = r.SellQuantity,
                    UnitRate = r.SellUnitRate,
                    TotalAmount = r.SellTotalAmount,
                    CurrencyCode = r.SellCurrency.CurrencyCode,
                    CurrencyName = r.SellCurrency.CurrencyName,
                    SizeTypeShortName = r.ContainerSizeType.ShortName,
                    Remarks = r.RemarksToCustomer,
                })
                .ToListAsync();
            if (bq == null)
                return NotFound();

            return PartialView("_SellQuoteItemsList", bq);
        }


        [HttpGet]
        [Authorize("Operations.RateRequests.View")]
        public async Task<IActionResult> GetSellQuoteDetails(Guid? quoteId)
        {
            var sq = await _context.SellQuotes
                .Where(r => r.Id == quoteId)
                .Select(r => new SellQuoteListViewModel
                {
                    QuotationNo = r.QuotationNo,
                    QuotationDate = r.QuotationDate,
                    ETA = r.ETA,
                    ETD = r.ETD,
                    VehicleName = r.VehicleNames,
                    TransportMovementNo = r.MovementNumbers,
                    ValidUntil = r.ValidUntil,
                    Id = r.Id,
                    CreateUserName = r.CreatedByUser.FullName,
                    UpdateUserName = r.LastEditUser != null ? r.LastEditUser.FullName : "",
                    Remarks = r.Remarks,
                    OptionsServiceName = r.ServiceName
                })
                .FirstOrDefaultAsync();
            if (sq == null)
                return NotFound();

            var sqi = await _context.SellQuoteItems
                .Include(x => x.ChargeType)
                .Include(x => x.ContainerSizeType)
                .Include(x => x.SellCurrency)
                .Include(x => x.BuyCurrency)
                .Where(x => x.SellQuoteId == quoteId)
                .Select(x => new SellQuoteItemListViewModel
                {
                    SellQuoteId = quoteId.Value,
                    ChargeTypeCode = x.ChargeType.ChargeTypeCode,
                    ChargeTypeName = x.ChargeType.ChargeTypeName,
                    CurrencyCode = x.SellCurrency.CurrencyCode,
                    CurrencyName = x.SellCurrency.CurrencyName,
                    Quantity = x.SellQuantity,
                    Remarks = x.RemarksToCustomer,
                    SizeTypeShortName = x.ContainerSizeType.ShortName,
                    TotalAmount = x.SellTotalAmount,
                    UnitRate = x.SellUnitRate,
                    Id = x.Id
                }).ToListAsync();

            sq.Items = sqi;
            return PartialView("~/Views/RateRequests/_SellQuoteDetails.cshtml", sq);
        }
        [HttpGet]
        [Authorize("Operations.RateRequests.View")]
        public async Task<IActionResult> GetInquiries(Guid id)
        {
            var rateRequest = await _context.RateRequests
                .Include(r => r.Origin)
                .Include(r => r.Destination)
                .Include(r => r.Inquiries)
                        .ThenInclude(a => a.Vendor)
                .Include(r => r.Inquiries)
                        .ThenInclude(a => a.Origin)
                .Include(r => r.Inquiries)
                        .ThenInclude(a => a.Destination)
                .Include(r => r.Inquiries)
                .ThenInclude(x => x.Status)
                .Include(r => r.Inquiries)
                .ThenInclude(x => x.SentByUser)
                .FirstOrDefaultAsync(r => r.Id == id);

            if (rateRequest == null)
                return NotFound();

            var viewModel = new RateRequestInquiryListViewModel
            {
                RateRequestId = rateRequest.Id,
                RequestNo = rateRequest.RequestNumber.ToString(),
                Inquiries = rateRequest.Inquiries.Select(i => new InquiryDto
                {
                    Id = i.Id,
                    CarrierName = i.Vendor?.PersonName ?? "N/A",
                    Origin = i.Origin.LocationName,
                    Destination = i.Destination.LocationName,
                    Status = i.Status.StatusName,
                    RequestNo = i.InquiryNo,
                    SentByUserName = i.SentByUser != null ? i.SentByUser.FullName : "",
                    SentDate = i.SentDateTime,
                    VendorEmailAddress = i.Vendor.EmailAddress
                }).ToList()
            };
            return PartialView("_InquiryList", viewModel);
        }

        [HttpDelete]
        [Authorize("Operations.RateRequests.Edit")]
        public async Task<IActionResult> DeleteInquiry(Guid id)
        {
            var inquiry = await _context.RateRequestInquiries.FindAsync(id);
            if (inquiry == null)
            {
                return NotFound();
            }
            if (inquiry.StatusId != 1)
            {
                HxTrigger(new
                {
                    toastFail = "Status is illegal"
                });
                return NoContent();
            }
            var rateRequestId = inquiry.RateRequestId;
            _context.RateRequestInquiries.Remove(inquiry);
            await _context.SaveChangesAsync();
            Response.Headers["HX-Trigger"] = $"{{\"inquirySaved\":{{\"rateRequestId\":\"{inquiry.RateRequestId}\"}}}}";
            return Ok();
        }

        // در RateRequestController


        //[HttpPost]
        //public async Task<IActionResult> SendAllInquiryEmails(Guid id)
        //{
        //    var rateRequest = await _context.RateRequests
        //        .Include(r => r.Inquiries)
        //            .ThenInclude(i => i.Vendor)
        //        .FirstOrDefaultAsync(r => r.Id == id);

        //    if (rateRequest == null)
        //        return Json(new { success = false, message = "Rate request not found." });

        //    var inquiries = rateRequest.Inquiries
        //        //.Where(i => i.Status == InquiryStatus.Draft || i.Status == InquiryStatus.Sent)
        //        .ToList();

        //    if (!inquiries.Any())
        //        return Json(new { success = false, message = "No inquiries available to send." });

        //    int sentCount = 0;
        //    var errors = new List<string>();

        //    foreach (var inquiry in inquiries)
        //    {
        //        try
        //        {
        //            var carrierEmail = inquiry.Vendor?.EmailAddress;

        //            if (string.IsNullOrWhiteSpace(carrierEmail))
        //            {
        //                errors.Add($"Inquiry #{inquiry.Id}: No email address found.");
        //                continue;
        //            }

        //            var subject = $"Rate Inquiry #{inquiry.Id} - Request #{rateRequest.Id}";

        //            var vm = new InquiryEmailViewModel { };
        //            var body = BuildEmailBody(vm);

        //            var recipients = new List<string>();

        //            if (!string.IsNullOrWhiteSpace(carrierEmail))
        //                recipients.Add(carrierEmail);

        //            await _emailService.SendEmailAsync(recipients, subject, body, true);

        //            inquiry.StatusId = 2; //Sent=ارسال شده
        //            inquiry.SentDateTime = DateTime.Now;
        //            inquiry.SentByUserId = _currentUserService.UserId;
        //            sentCount++;
        //        }
        //        catch (Exception ex)
        //        {
        //            errors.Add($"Inquiry #{inquiry.Id}: {ex.Message}");
        //        }
        //    }

        //    await _context.SaveChangesAsync();

        //    var message = $"{sentCount} email(s) sent successfully.";
        //    if (errors.Any())
        //        message += $" {errors.Count} error(s) occurred.";

        //    return Json(new
        //    {
        //        success = sentCount > 0,
        //        message
        //    });
        //}

        //private RateInquiryEmailVm BuildRateInquiryEmailVm()
        //{
        //    var model = new RateInquiryEmailVm
        //    {
        //        InquiryNo = "INQ-2026-000148",
        //        InquiryDate = DateTime.Now,
        //        ResponseDueDate = DateTime.Now.AddDays(2),

        //        VendorName = "Blue Ocean Shipping LLC",

        //        Origin = "Tehran, Iran",
        //        Destination = "Frankfurt, Germany",

        //        TransportType = "Air Freight",
        //        ServiceType = "Airport to Airport",
        //        CargoType = "General Cargo",
        //        Incoterm = "FOB",

        //        GrossWeight = 1250,
        //        ChargeableWeight = 1380,
        //        Volume = 8.25m,

        //        PackageCount = 24,
        //        PackageSummary = "24 Cartons",

        //        CargoReadyDate = DateTime.Now.AddDays(6),
        //        Commodity = "Electronic Components",

        //        Remarks = "Shipment is expected to be ready next week. Please quote based on airport-to-airport service. If there are any restrictions, kindly mention them clearly.",

        //        CompanyName = "Arman Logistics Co.",
        //        DepartmentName = "Freight Procurement Department",
        //        CompanyEmail = "procurement@arman-logistics.com",
        //        CompanyTel = "+98 21 0000 0000",
        //        CompanyWebsite = "www.arman-logistics.com",

        //        Packages = new List<RateInquiryPackageVm>
        //{
        //    new RateInquiryPackageVm
        //    {
        //        RowNo = 1,
        //        PackageType = "Carton",
        //        Quantity = 12,
        //        Length = 60,
        //        Width = 45,
        //        Height = 50,
        //        Weight = 620
        //    },
        //    new RateInquiryPackageVm
        //    {
        //        RowNo = 2,
        //        PackageType = "Carton",
        //        Quantity = 12,
        //        Length = 70,
        //        Width = 50,
        //        Height = 55,
        //        Weight = 630
        //    }
        //}
        //    };

        //    return model;
        //}

        //public async Task<IActionResult> PreviewInquiryEmail()
        //{
        //    var model = BuildRateInquiryEmailVm();
        //    model.canSendEmail = true;

        //    var html = await _razorViewRenderService.RenderToStringAsync(
        //        "~/Views/Shared/EmailTemplates/_RateInquiryEmailTemplate.cshtml",
        //        model
        //    );

        //    return Content(html, "text/html");
        //}


        [HttpGet]
        [Authorize("Operations.RateRequests.Edit")]
        public async Task<IActionResult> PreviewInquiryEmailFromService(Guid inquiryId, bool canSendEmail = false)
        {
            var html = await _rateInquiryEmailService.BuildInquiryEmailHtmlAsync(inquiryId, canSendEmail, true);
            return Content(html, "text/html");
        }
        [HttpGet]
        [Authorize("Operations.RateRequests.Edit")]
        public async Task<IActionResult> PreviewSellQuoteEmailFromService(Guid quoteId, bool canSendEmail = false)
        {
            var html = await _rateInquiryEmailService.BuildSellQuoteEmailHtmlAsync(quoteId, canSendEmail, true);
            return Content(html, "text/html");
        }


        [HttpPost]
        [Authorize("Operations.RateRequests.Edit")]
        public async Task<IActionResult> SendInquiryEmail(Guid inquiryId)
        {
            try
            {
                var inq = await _context.RateRequestInquiries.Where(x => x.Id == inquiryId).FirstOrDefaultAsync();
                await _rateInquiryEmailService.SendInquiryEmailAsync(inquiryId);
                Response.Headers["HX-Trigger"] = $"{{\"inquirySaved\":{{\"rateRequestId\":\"{inq.RateRequestId}\"}}}}";
                return Json(new { success = true, message = "Email sent successfully." });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message });
            }
        }

        [HttpPost]
        [Authorize("Operations.RateRequests.Edit")]
        public async Task<IActionResult> SendSellQuoteEmail(Guid quoteId)
        {
            try
            {
                await _rateInquiryEmailService.SendSellQuoteEmailAsync(quoteId);
                return Json(new { success = true, message = "Email sent successfully." });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message });
            }
        }




        [ValidateAntiForgeryToken]
        [HttpPost]
        [Authorize("Operations.RateRequests.Edit")]
        public async Task<IActionResult> SendInquiryInApp(Guid inquiryId)
        {
            var inq = await _context.RateRequestInquiries.Where(x => x.Id == inquiryId).FirstOrDefaultAsync();
            if (inq == null) return NotFound();
            try
            {
                await _rateInquiryEmailService.SendInquiryInAppAsync(inquiryId);
                Response.Headers["HX-Trigger"] = $"{{\"inquirySaved\":{{\"rateRequestId\":\"{inq.RateRequestId}\"}}}}";
                return Ok();
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message });
            }
        }

        [HttpPost]
        [Authorize("Operations.RateRequests.Edit")]
        public async Task<IActionResult> SendSellQuoteInApp(Guid quoteId)
        {
            try
            {
                await _rateInquiryEmailService.SendSellQuoteInAppAsync(quoteId);
                Response.Headers["HX-Trigger"] = "sellQuoteSaved";
                return NoContent();
            }
            catch (Exception ex)
            {
                return BadRequest();
            }
        }
        [HttpPost]
        [Authorize("Operations.RateRequests.Edit")]
        public async Task<IActionResult> SendAllDraftInquiryEmails(Guid rateRequestId)
        {
            try
            {
                var count = await _rateInquiryEmailService.SendAllDraftInquiryEmailsAsync(rateRequestId);

                return Json(new
                {
                    success = true,
                    message = $"{count} draft inquiry email(s) sent successfully."
                });
            }
            catch (Exception ex)
            {
                return Json(new
                {
                    success = false,
                    message = ex.Message
                });
            }
        }


        public async Task<string> GenerateRateRequestNumberAsync(CancellationToken cancellationToken = default)
        {
            var connection = _context.Database.GetDbConnection();

            if (connection.State != ConnectionState.Open)
                await connection.OpenAsync(cancellationToken);

            await using var command = connection.CreateCommand();
            command.CommandText = "SELECT NEXT VALUE FOR [ops].[RateRequestNumberSeq]";

            var result = await command.ExecuteScalarAsync(cancellationToken);
            var nextNumber = Convert.ToInt32(result);

            return $"Req-{nextNumber:00000}";
        }



        [HttpGet]
        [Authorize(Roles = "Sales Manager")]
        public async Task<IActionResult> AssignSalesUser(Guid id)
        {
            if (!_currentUserService.IsInRole("Sales Manager") && !_currentUserService.IsInRole("Manager") && !_currentUserService.IsInRole("Admin")) return NoContent();
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

        [HttpGet]
        [Authorize("Operations.RateRequests.Edit")]
        public async Task<IActionResult> CreateSellQuote(Guid rateRequestId)
        {
            if (User.IsInRole("External Party") || User.IsInRole("Customer")) return BadRequest();
            var viewModel = new CreateSellQuoteViewModel
            {
                RateRequestId = rateRequestId,
                Carriers = await _lookupService.GetCarriersAsync(null),
                Items = new List<SellQuoteItemEditModel>()
            };
            ViewBag.ContainerSizeTypes = await _context.ContainerSizeTypes.ToListAsync();
            ViewBag.ChargeTypes = await _context.ChargeTypes.ToListAsync();
            ViewBag.Currencies = await _context.Currencies
                   .Where(x => x.IsDeleted == false)
                   .ToListAsync();
            return PartialView("_CreateSellQuoteModal", viewModel);
        }
        [HttpGet]
        [Authorize("Operations.RateRequests.Edit")]
        public async Task<IActionResult> EditSellQuote(Guid Id)
        {
            if (User.IsInRole("External Party") || User.IsInRole("Customer")) return BadRequest();
            var sq = await _context
                .SellQuotes
                .Include(x => x.Items)
                .ThenInclude(x => x.ChargeType)
                .Include(x => x.Items)
                .ThenInclude(x => x.ContainerSizeType)
                .Include(x => x.Items)
                .ThenInclude(x => x.SellCurrency)
                .Where(x => x.Id == Id)
                .FirstOrDefaultAsync();
            if (sq == null) return NotFound();

            var viewModel = new CreateSellQuoteViewModel
            {
                Id = sq.Id,
                RateRequestId = sq.RateRequestId,
                ETA = sq.ETA,
                ETD = sq.ETD,
                MovementNumbers = sq.MovementNumbers,
                QuotationNo = sq.QuotationNo,
                Remarks = sq.Remarks,
                ServiceName = sq.ServiceName,
                ValidUntil = sq.ValidUntil,
                VehicleNames = sq.VehicleNames,
                CarrierId = sq.CarrierId,
                Carriers = await _lookupService.GetCarriersAsync(null),
                Items = sq.Items.OrderBy(x => x.LineNumber).Select(i => new SellQuoteItemEditModel
                {
                    ChargeTypeId = i.ChargeTypeId,
                    ChargeTypeName = i.ChargeType.ChargeTypeName,
                    ContainerSizeTypeId = i.ContainerSizeTypeId,
                    SizeTypeName = i.ContainerSizeType != null
                            ? i.ContainerSizeType.ShortName
                            : null,
                    BuyUnitRate = i.BuyUnitRate,
                    BuyTotal = i.BuyTotalAmount,

                    CurrencyId = i.SellCurrencyId,
                    CurrencyCode = i.SellCurrency?.CurrencyCode,

                    Quantity = i.SellQuantity,
                    // به صورت پیش‌فرض همان نرخ خرید را برای فروش قرار می‌دهیم
                    SellUnitRate = i.SellUnitRate,
                    SellTotal = i.SellTotalAmount
                }).ToList()
            };
            ViewBag.ContainerSizeTypes = await _context.ContainerSizeTypes.ToListAsync();
            ViewBag.ChargeTypes = await _context.ChargeTypes.ToListAsync();
            ViewBag.Currencies = await _context.Currencies
                   .Where(x => x.IsDeleted == false)
                   .ToListAsync();
            return PartialView("_CreateSellQuoteModal", viewModel);
        }

        [HttpGet]
        [Authorize("Operations.RateRequests.Edit")]
        public async Task<IActionResult> CreateFromBuy(Guid buyQuoteId)
        {
            if (User.IsInRole("External Party") || User.IsInRole("Customer")) return BadRequest();
            var buyQuote = await _context.BuyQuotes
                .Include(x => x.RateRequestInquiry)
                .Include(x => x.BuyQuoteItems)
                .ThenInclude(x => x.ChargeType)
                .Include(x => x.BuyQuoteItems)
                .ThenInclude(x => x.Currency)
                .Include(x => x.BuyQuoteItems)
                .ThenInclude(x => x.ContainerSizeType)
                .Where(x => x.Id == buyQuoteId).FirstOrDefaultAsync();
            var viewModel = new CreateSellQuoteViewModel
            {
                BuyQuoteId = buyQuote.Id,
                RateRequestId = buyQuote.RateRequestInquiry.RateRequestId,
                QuotationNo = "SQ-" + DateTime.Now.Ticks.ToString().Substring(10), // تولید موقت شماره
                ValidUntil = buyQuote.ValidUntil ?? DateTime.Now.AddDays(7),
                ServiceName = buyQuote.OptionsServiceName,
                VehicleNames = buyQuote.VehicleName,
                MovementNumbers = buyQuote.TransportMovementNo,
                ETD = buyQuote.ETD,
                ETA = buyQuote.ETA,
                Remarks = buyQuote.Remarks,
                CarrierId = buyQuote.CarrierId,
                Carriers = await _lookupService.GetCarriersAsync(null),
                Items = buyQuote.BuyQuoteItems.Select(i => new SellQuoteItemEditModel
                {
                    ChargeTypeId = i.ChargeTypeId,
                    ChargeTypeName = i.ChargeType.ChargeTypeName,
                    ContainerSizeTypeId = i.ContainerSizeTypeId,
                    SizeTypeName = i.ContainerSizeType?.ShortName,
                    BuyUnitRate = i.UnitRate,
                    BuyTotal = i.TotalAmount,
                    CurrencyCode = i.Currency.CurrencyCode,
                    Quantity = i.Quantity,
                    CurrencyId = i.CurrencyId,
                    SellUnitRate = i.UnitRate // به صورت پیش‌فرض همان نرخ خرید را می‌گذاریم
                }).ToList()
            };
            ViewBag.ContainerSizeTypes = await _context.ContainerSizeTypes.ToListAsync();
            ViewBag.ChargeTypes = await _context.ChargeTypes.ToListAsync();
            ViewBag.Currencies = await _context.Currencies
                   .Where(x => x.IsDeleted == false)
                   .ToListAsync();
            return PartialView("_CreateSellQuoteModal", viewModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Sales Manager")]
        public async Task<IActionResult> AssignSalesUser(AssignSalesUserViewModel vm)
        {
            var rr = await _context.RateRequests.FindAsync(vm.CustomerId);
            if (rr == null) return NotFound();
            rr.AssignedSalesUserId = vm.SelectedSalesUserId;
            rr.AssignedSalesUserDate = DateTime.Now;
            rr.UpdatedAt = DateTime.Now;
            rr.StatusId = 4;    //Sent To Sales User
            await _context.SaveChangesAsync();

            await activityLogService.LogHistoryAsync(
                new CreateActivityLogRequest
                {
                    ActivityCategoryId = 2,
                    ActivityDateTime = DateTime.Now,
                    Title = "Assigned Sales User",
                    Description = "Sales user Assigned",
                    EntityTypeId = 8,
                    EntityId = vm.CustomerId,
                    FromStatusEntityType = "RateRequestStatus",
                    ToStatusId = 4,
                    PerformedByUserId = _currentUserService.UserId,
                    IsSystemGenerated = true,
                }
                );
            HxTrigger(new
            {
                rateRequestChanged = new { },
                showToast = new { message = "Sales user Assigned Successfuly!!!!.", type = "success" },
                closeAssignSalesUserModal = new { }
            });
            return NoContent();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize("Operations.RateRequests.Edit")]
        public async Task<IActionResult> SaveSellQuote(CreateSellQuoteViewModel model)
        {
            if (!ModelState.IsValid)
            {



                var viewModel = new CreateSellQuoteViewModel
                {
                    RateRequestId = model.RateRequestId,
                    Carriers = await _lookupService.GetCarriersAsync(null),
                    Items = new List<SellQuoteItemEditModel>()
                };
                ViewBag.ContainerSizeTypes = await _context.ContainerSizeTypes.ToListAsync();
                ViewBag.ChargeTypes = await _context.ChargeTypes.ToListAsync();
                ViewBag.Currencies = await _context.Currencies
                       .Where(x => x.IsDeleted == false)
                       .ToListAsync();
                return PartialView("_CreateSellQuoteModal", model);
            }
            using (var transaction = await _context.Database.BeginTransactionAsync())
            {
                try
                {
                    Guid quoteId;
                    int companyId = 1;
                    if (model.Id == Guid.Empty)
                    {
                        var sellQuote = new SellQuote
                        {
                            Id = Guid.CreateVersion7(),
                            CompanyId = 1, // این باید از Identity یا تنظیمات جاری خوانده شود
                            QuotationNo = "SQ-" + DateTime.Now.Ticks.ToString().Substring(10), // تولید موقت شماره,
                            QuotationDate = DateTime.Now,
                            SelectedBuyQuoteId = model.BuyQuoteId == Guid.Empty ? null : model.BuyQuoteId,
                            RateRequestId = model.RateRequestId,
                            ServiceName = model.ServiceName,
                            ValidUntil = model.ValidUntil.Value,
                            VehicleNames = model.VehicleNames,
                            MovementNumbers = model.MovementNumbers,
                            Remarks = model.Remarks,
                            ETD = model.ETD.Value,
                            ETA = model.ETA.Value,
                            CarrierId = model.CarrierId,
                            StatusId = 1, // Draft
                            //IsActive = true,
                            IsDeleted = false,
                            CreatedAt = DateTime.Now,
                            CreateUserId = _currentUserService.UserId,
                        };
                        _context.SellQuotes.Add(sellQuote);
                        quoteId = sellQuote.Id;
                    }
                    else
                    {
                        var sellQuote = await _context.SellQuotes.Where(x => x.Id == model.Id).FirstOrDefaultAsync();
                        if (sellQuote == null) return NotFound();
                        quoteId = sellQuote.Id;
                        sellQuote.ServiceName = model.ServiceName;
                        sellQuote.ValidUntil = model.ValidUntil.Value;
                        sellQuote.VehicleNames = model.VehicleNames;
                        sellQuote.MovementNumbers = model.MovementNumbers;
                        sellQuote.Remarks = model.Remarks;
                        sellQuote.ETD = model.ETD.Value;
                        sellQuote.ETA = model.ETA.Value;
                        sellQuote.CarrierId = model.CarrierId;
                        sellQuote.UpdatedAt = DateTime.Now;
                        sellQuote.EditUserId = _currentUserService.UserId;
                        var items = await _context.SellQuoteItems.Where(x => x.SellQuoteId == quoteId).ToListAsync();
                        foreach (var item in items)
                        {
                            item.DeletedAt = DateTime.Now;
                            item.DeleteUserId = _currentUserService.UserId;
                            item.IsDeleted = true;
                        }
                    }
                    // ۲. ایجاد آیتم‌ها SellQuoteItems
                    int lineNumber = 1;
                    foreach (var item in model.Items)
                    {
                        // واکشی اطلاعات خرید مربوط به این آیتم برای ذخیره در رکورد فروش (Audit)
                        // در دنیای واقعی بهتر است اطلاعات خرید را از دیتابیس مجدد چک کنید

                        var sellItem = new SellQuoteItem
                        {
                            Id = Guid.CreateVersion7(),
                            SellQuoteId = quoteId,
                            ResponsibleCompanyId = companyId,
                            LineNumber = lineNumber++,
                            ChargeTypeId = item.ChargeTypeId,
                            ContainerSizeTypeId = item.ContainerSizeTypeId,

                            // مقادیر فروش
                            SellQuantity = item.Quantity.Value,
                            SellUnitRate = item.SellUnitRate.Value,
                            SellTotalAmount = item.Quantity.Value * item.SellUnitRate.Value,
                            SellCurrencyId = item.CurrencyId.Value,

                            BuyQuantity = item.Quantity.Value,
                            BuyUnitRate = item.BuyUnitRate.Value,
                            BuyTotalAmount = item.Quantity.Value * item.BuyUnitRate.Value,
                            BuyCurrencyId = item.CurrencyId.Value,
                            // فیلدهای اجباری دیتابیس
                            CalculationById = 1, // Per Unit
                            CalculationTypeId = 1, // Fixed
                            IsTaxable = false,
                            SellTaxPercent = 0,
                            SellTaxAmount = 0,
                            BuyTaxPercent = 0,
                            BuyTaxAmount = 0,

                            //IsActive = true,
                            IsDeleted = false,
                            CreatedAt = DateTime.Now,
                            CreateUserId = _currentUserService.UserId
                        };
                        _context.SellQuoteItems.Add(sellItem);
                    }
                    await _context.SaveChangesAsync();
                    await transaction.CommitAsync();
                    Response.Headers["HX-Trigger"] = "sellQuoteSaved";
                    return Content("Success! Quotation saved as Draft.");
                }
                catch (Exception ex)
                {
                    await transaction.RollbackAsync();
                    ModelState.AddModelError("", "Error saving data: " + ex.Message);
                    return PartialView("_CreateSellQuoteModal", model);
                }
            }
        }
        [HttpGet]
        [Authorize("Operations.RateRequests.Edit")]




        [HttpGet]
        [Authorize(Roles = "Sales Manager")]
        public async Task<IActionResult> RejectRateRequest(Guid id)
        {
            var rq = await _context.RateRequests
                .AsNoTracking()
                .FirstOrDefaultAsync(x => x.Id == id);

            if (rq == null)
                return NotFound();

            var vm = new RejectRateRequestVm
            {
                RateRequestId = rq.Id
            };

            return PartialView("_RejectRateRequest", vm);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Sales Manager")]
        public async Task<IActionResult> RejectRateRequest(RejectRateRequestVm vm)
        {
            if (!ModelState.IsValid)
                return PartialView("_RejectRateRequest", vm);
            var result = await _rateRequestService.RejectRateRequest(vm);
            if (!result.Succeeded)
            {
                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError("", error);
                }

                return PartialView("_RejectRateRequest", vm);
            }
            HxTrigger(new { rateRequestChanged = true, toastSuccess = "Rejected successfully", closeAppModal = true });
            Response.Headers["HX-Trigger"] = "rateRequestChanged";
            return NoContent();
        }


        [Authorize("Operations.RateRequests.Edit")]
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
        [Authorize("Operations.RateRequests.Edit")]

        [Authorize("Operations.RateRequests.Edit")]
        public async Task<IActionResult> RejectSellQuoteInApp(RejectSellQuoteVm vm)
        {
            if (!ModelState.IsValid)
                return PartialView("_RejectSellQuote", vm);
            var sq = await _context.SellQuotes
                .FirstOrDefaultAsync(x => x.Id == vm.SellQuoteId);
            if (sq == null)
                return NotFound();
            sq.StatusId = 4;  //Rejected
            sq.CustomerRespondComments = vm.Comments;
            sq.CustomerRespondUserId = _currentUserService.UserId;
            sq.CustomerRespondDate = DateTime.Now;
            await _context.SaveChangesAsync();
            Response.Headers["HX-Trigger"] = "sellQuoteSaved";
            return NoContent();
        }






        [HttpGet]
        [Authorize("Operations.RateRequests.Edit")]
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
        [Authorize("Operations.RateRequests.Edit")]

        public async Task<IActionResult> AcceptSellQuoteInApp(AcceptSellQuoteVm vm)
        {
            if (!ModelState.IsValid)
                return PartialView("_AcceptSellQuote", vm);
            var sq = await _context.SellQuotes
                .FirstOrDefaultAsync(x => x.Id == vm.SellQuoteId);
            if (sq == null)
                return NotFound();
            sq.StatusId = 3;  //Accepted
            sq.CustomerRespondComments = vm.Comments;
            sq.CustomerRespondUserId = _currentUserService.UserId;
            sq.CustomerRespondDate = DateTime.Now;
            await _context.SaveChangesAsync();

            var triggerPayload = new
            {
                sellQuoteSaved = new
                {
                    rateRequestId = sq.RateRequestId
                }
            };
            Response.Headers["HX-Trigger"] = System.Text.Json.JsonSerializer.Serialize(triggerPayload);
            //Response.Headers["HX-Trigger"] = "sellQuoteSaved";
            return NoContent();
        }

        [HttpGet]
        [Authorize("Operations.RateRequests.GenerateJob")]
        public async Task<IActionResult> GenerateJobFromQuote(Guid quoteId)
        {
            var sq = await _context.SellQuotes
                            .Include(x => x.RateRequest)
                            .ThenInclude(x => x.TransportTypes)
                            .Where(x => x.Id == quoteId).FirstOrDefaultAsync();
            if (sq == null) return BadRequest();
            if (!sq.RateRequest.TransportTypes.Any())
            {
                HxTrigger(new
                {
                    toastFail = "No Transport Types defined!!!"
                });
                return NoContent();
            }
            var result = await _jobService.GenerateJobFromAcceptedSellQuoteAsync(quoteId);
            if (result != Guid.Empty)
            {
                var job = await _jobService.GetJobNoFileNumberAsync(result);
                var toastSuccessMessage = $"Job No: {job.JobNumber} With Job File No: {job.JobFileNumber} Created Successfully";

                HxTrigger(new
                {
                    sellQuoteSaved = new
                    {
                        rateRequestId = sq.RateRequestId
                    },
                    toastSuccess = toastSuccessMessage
                });
            }
            else
            {
                HxTrigger(new
                {
                    toastFail = "Job can not be created . already exists or some other erros occured!!!!"
                });
            }
            return NoContent();
        }
        [HttpGet]
        [Authorize("Operations.RateRequests.View")]
        public async Task<IActionResult> Details(Guid id)
        {
            var model = await _context.RateRequests
                .Include(x => x.RateRequestSource)
                .Include(x => x.Company)
                    .ThenInclude(x => x.Person)
                .Include(x => x.TransportTypes)
                    .ThenInclude(y => y.TransportType)
                .Include(x => x.Containers)
                .Include(x => x.Origin)
                .Include(x => x.Destination)
                .Include(x => x.LoadType)
                .Include(x => x.TradeType)
                .Where(x => x.Id == id)
                .Select(x => new RRDetailsVM
                {
                    CargoReadyDate = x.CargoReadinessDate,
                    Commodity = x.CargoCommodityDescription,
                    Origin = x.Origin.LocationName,
                    Destination = x.Destination.LocationName,
                    CompanyName = x.Company.Person.PersonName,
                    EmailAddress = x.EmailAddress,
                    ContactName = x.ContactName,
                    MobileNo = x.MobileNo,
                    Remarks = x.Remarks,
                    RequestDate = x.RequestDate,
                    RateRequestNumber = x.RequestNumber,

                    TransportTypes = x.TransportTypes
                        .OrderBy(x => x.Sequence)
                        .ThenBy(x => x.Id)
                        .Select(y => new RRTransportTypeVM
                        {
                            Sequence = y.Sequence,
                            TransportTypeName = y.TransportType.TransportTypeName
                        })
                        .ToList(),
                    Containers = x.Containers
                        .OrderBy(x => x.Id)
                        .Select(y => new RRContainerVM
                        {
                            Counts = (int)y.Counts,
                            SizeTypeName = y.ContainerSizeType.ShortName
                        })
                        .ToList(),
                })
                .FirstOrDefaultAsync();
            var cargoDetails = await _context.CargoDetails.Where(x => x.RateRequestId == id).OrderBy(x => x.Sequence).ToListAsync();
            foreach (var item in cargoDetails)
            {
                model.CargoDetails.Add(await _cargoDetailsService.GetDetailsAsync(item.Id));
            }
            return View("Details", model);
        }
    }
}