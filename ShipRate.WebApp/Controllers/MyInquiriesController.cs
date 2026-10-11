using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using ShipRate.Application.Interfaces;
using ShipRate.Application.ViewModels;
using ShipRate.Domain.Entities.RateQuotation;
using ShipRate.Infrastructure.Data;
using ShipRate.WebApp.ViewModels.MyInquiriesIndexVMs.cs;

namespace ShipRate.WebApp.Controllers
{
    [Authorize]

    public class MyInquiriesController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly ICurrentUserService _currentUserService;
        private readonly IRateInquiryEmailService _rateInquiryEmailService;

        public MyInquiriesController(ApplicationDbContext context, ICurrentUserService currentUserService, IRateInquiryEmailService rateInquiryEmailService)
        {
            _context = context;
            _currentUserService = currentUserService;
            _rateInquiryEmailService = rateInquiryEmailService;
        }
        public async Task<IActionResult> Index(VendorInquirySearchVm search)
        {
            var vendorId = await _currentUserService.GetPersonIdAsync();
            if (vendorId == null || vendorId == Guid.Empty) { return BadRequest(); }
            var inquiries = await GetVendorInquiriesQuery(vendorId.Value, search)
                .ToListAsync();
            var selectedInquiryId = inquiries.FirstOrDefault()?.Id;
            var vm = new VendorInquiryDashboardVm
            {
                Search = search,
                Inquiries = inquiries,
                SelectedInquiryId = selectedInquiryId,
                Responses = new List<VendorQuoteResponseRowVm>(),
                Statuses = await _context.InquiryStatuses
                .OrderBy(x => x.Id)
                .Select(x => new SelectListItem
                {
                    Value = x.Id.ToString(),
                    Text = x.StatusName
                })
                .ToListAsync()
            };
            return View(vm);
        }
        public async Task<IActionResult> QuoteResponses(Guid inquiryId)
        {
            var vendorId = await _currentUserService.GetPersonIdAsync();
            if (vendorId == null || vendorId == Guid.Empty) { return BadRequest(); }
            var responses = await GetVendorResponsesQuery(vendorId.Value, inquiryId).ToListAsync();
            return PartialView("_VendorQuoteResponseGrid", responses);
        }

        public async Task<IActionResult> InquiryList(VendorInquirySearchVm search)
        {
            var vendorId = await _currentUserService.GetPersonIdAsync();
            if (vendorId == null || vendorId == Guid.Empty) { return BadRequest(); }

            var inquiries = await GetVendorInquiriesQuery(vendorId.Value, search)
                .ToListAsync();

            return PartialView("_VendorInquiryGrid", inquiries);
        }

        private IQueryable<VendorInquiryRowVm> GetVendorInquiriesQuery(Guid vendorId, VendorInquirySearchVm search)
        {
            var query = _context.RateRequestInquiries
                .Include(x => x.RateRequest)
                .ThenInclude(x => x.Origin)
                .Include(x => x.RateRequest)
                .ThenInclude(x => x.Destination)
                .AsNoTracking()
                .Where(x => x.VendorId == vendorId && x.StatusId != 1);

            //if (!string.IsNullOrWhiteSpace(search.Status))
            //    query = query.Where(x => x.Status == search.Status);

            if (!string.IsNullOrWhiteSpace(search.InquiryNo))
                query = query.Where(x => x.InquiryNo.Contains(search.InquiryNo));

            if (search.From.HasValue)
                query = query.Where(x => x.InquiryDate >= search.From.Value);

            if (search.StatusId.HasValue)
                query = query.Where(x => x.StatusId == search.StatusId);

            if (search.To.HasValue)
                query = query.Where(x => x.InquiryDate <= search.To.Value);

            return query
                .OrderByDescending(x => x.InquiryDate)
                .Select(x => new VendorInquiryRowVm
                {
                    Id = x.Id,
                    InquiryNo = x.InquiryNo,
                    InquiryDate = x.InquiryDate,
                    Origin = x.Origin.LocationName,
                    Destination = x.Destination.LocationName,
                    //CargoType = x.CargoTypeName,
                    //Weight = x.Weight,
                    Status = x.Status.StatusName,
                    ResponseCount = _context.BuyQuotes.Count(q =>
                        q.RateRequestInquiryId == x.Id &&
                        q.VendorId == vendorId)
                });
        }
        private IQueryable<VendorQuoteResponseRowVm> GetVendorResponsesQuery(Guid vendorId, Guid inquiryId)
        {
            return _context.BuyQuotes
                .AsNoTracking()
                .Where(x =>
                    x.VendorId == vendorId &&
                    x.RateRequestInquiryId == inquiryId)
                .OrderByDescending(x => x.CreatedAt)
                .Select(x => new VendorQuoteResponseRowVm
                {
                    Id = x.Id,
                    QuoteNo = x.QuotationNo,
                    QuoteDate = x.CreatedAt,

                    ServiceName = x.OptionsServiceName,
                    TransitTime = x.TransitDays.Value.ToString(),
                    CarrierName = x.CarrierName,
                    VendorName = x.VendorName,
                    TransportBy = x.VehicleName + "-" + x.TransportMovementNo,
                    StatusId = x.StatusId,
                    Status = x.Status.QuotationStatusName,
                    CreatedByUserName = x.CreatedByUser.FullName,
                    LastEditedByUserName = x.EditedByUser != null ? x.EditedByUser.FullName : "",
                    LastActionDateTime = x.UpdatedAt == null ? "" : x.UpdatedAt.Value.ToString("yyyy-MM-dd HH:mm")
                    //TotalAmount = x.TotalAmount,
                    //Currency = x.CurrencyCode,
                    //Status = x.Status
                });
        }


        // GET: MyInquiries/Details/5
        [HttpGet]
        public async Task<IActionResult> Details(Guid id)
        {
            var inquiry = await _context.RateRequestInquiries
                .Include(i => i.RateRequest)
                    .ThenInclude(rr => rr.TransportTypes)
                .Include(i => i.RateRequest)
                    .ThenInclude(rr => rr.Packages)
                        .ThenInclude(x => x.PackageType)
                .Include(i => i.RateRequest)
                    .ThenInclude(i => i.Origin)
                .Include(i => i.RateRequest)
                    .ThenInclude(i => i.Destination)
                .Include(i => i.RateRequest)
                    .ThenInclude(rr => rr.Containers)
                        .ThenInclude(rr => rr.ContainerSizeType)
                .FirstOrDefaultAsync(i => i.Id == id);

            if (inquiry == null)
            {
                return NotFound();
            }

            // بررسی دسترسی
            //var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            //if (string.IsNullOrEmpty(userIdClaim))
            //{
            //    return Unauthorized();
            //}

            //int userId = int.Parse(userIdClaim);

            //var carrier = await _context.Carriers
            //    .FirstOrDefaultAsync(c => c.PersonId == userId);

            //var agent = await _context.Agents
            //    .FirstOrDefaultAsync(a => a.PersonId == userId);

            //bool hasAccess = (carrier != null && inquiry.CarrierId == carrier.Id) ||
            //                 (agent != null && inquiry.AgentId == agent.Id);

            //if (!hasAccess)
            //{
            //    return Forbid();
            //}

            // بررسی وجود Quote قبلی
            var existingQuote = await _context.BuyQuotes
                .Include(q => q.BuyQuoteItems)
                .FirstOrDefaultAsync(q => q.RateRequestInquiryId == id);

            var viewModel = new MyInquiryDetailsVM
            {
                InquiryId = inquiry.Id,
                RateRequestId = inquiry.RateRequestId,
                Origin = inquiry.RateRequest.Origin.LocationName ?? "N/A",
                Destination = inquiry.RateRequest.Destination.LocationName ?? "N/A",
                //CargoType = inquiry.RateRequest.CargoType ?? "N/A",
                //Weight = inquiry.RateRequest.TotalWeight,
                RequestDate = inquiry.RateRequest.CreatedAt,
                //Status = inquiry.Status,
                //SentDate = inquiry.SentDate ?? DateTime.Now,

                //TransportTypes = inquiry.RateRequest.TransportTypes
                //.Select(t => new TransportTypeDto
                //{
                //    Type = t.TransportType.TransportTypeName
                //}).ToList(),

                Packages = inquiry.RateRequest.Packages
                .Select(p => new PackageDto
                {
                    Type = p.PackageType.PackageTypeName,
                    Quantity = p.UnitCount,
                    //Length = p.Length,
                    //Width = p.Width,
                    //Height = p.Height,
                    //Weight = p.Weight
                }).ToList(),

                Containers = inquiry.RateRequest.Containers
                .Select(c => new ContainerDto
                {
                    Type = c.ContainerSizeType.ShortName,
                    Quantity = c.Counts
                }).ToList()
            };

            if (existingQuote != null)
            {
                viewModel.ExistingQuote = new BuyQuoteDto
                {
                    Id = existingQuote.Id,
                    ServiceName = existingQuote.OptionsServiceName,
                    VesselName = existingQuote.VehicleName,
                    VoyageNumber = existingQuote.TransportMovementNo,
                    ETD = existingQuote.ETD.Value,
                    ETA = existingQuote.ETA.Value,
                    TransitDays = existingQuote.TransitDays.Value,
                    IsDirect = existingQuote.IsDirect,
                    ValidUntil = existingQuote.ValidUntil,
                    Remarks = existingQuote.Remarks,

                    Items = existingQuote.BuyQuoteItems.Select(i => new BuyQuoteItemDto
                    {
                        ChargeType = i.ChargeType.ChargeTypeName,
                        Currency = i.Currency.CurrencyCode,
                        ContainerType = i.ContainerSizeType.ShortName,
                        UnitRate = i.UnitRate,
                        TotalAmount = i.TotalAmount,
                        Remarks = i.Remarks
                    }).ToList()
                };
            }

            return View(viewModel);
        }

        // POST: Reject Inquiry
        //[HttpPost]
        //public async Task<IActionResult> RejectInquiry(RejectInquiryVM model)
        //{
        //    if (!ModelState.IsValid)
        //    {
        //        return BadRequest();
        //    }
        //    var inquiry = await _context.RateRequestInquiries
        //        .FirstOrDefaultAsync(i => i.Id == model.InquiryId);

        //    if (inquiry == null)
        //    {
        //        return NotFound();
        //    }

        //    //inquiry.Status = "Rejected";

        //    inquiry.RejectReason = model.RejectionReason;
        //    inquiry.RejectedByUserId = _currentUserService.UserId;
        //    inquiry.RejectDateTime = DateTime.Now;
        //    await _context.SaveChangesAsync();
        //    return RedirectToAction(nameof(Index));
        //}


        [HttpGet]
        public async Task<IActionResult> EditQuote(Guid quoteId)
        {
            var quote = await _context.BuyQuotes
                .Include(x => x.BuyQuoteItems)
                .AsNoTracking()
                .FirstOrDefaultAsync(x => x.Id == quoteId);

            if (quote == null) return NotFound();
            var vm = new SubmitQuoteVm
            {
                Id = quoteId,
                RateRequestInquiryId = quote.RateRequestInquiryId.Value,
                QuotationDate = DateTime.Now,
                VendorId = quote.VendorId,
                CarrierId = quote.CarrierId,
                CarrierName = quote.CarrierName,
                VendorName = quote.VendorName,
                ETA = quote.ETA,
                ETD = quote.ETD,
                IsDirect = quote.IsDirect,
                QuotationNo = quote.QuotationNo,
                Remarks = quote.Remarks,
                ValidUntil = quote.ValidUntil,
                VehicleName = quote.VehicleName,
                TransitDays = quote.TransitDays,
                TransportMovementNo = quote.TransportMovementNo,
                OptionsServiceName = quote.OptionsServiceName,
                Items = new List<SubmitQuoteItemVm>
                {
                }
            };
            foreach (var item in quote.BuyQuoteItems)
            {
                vm.Items.Add(new SubmitQuoteItemVm
                {
                    ChargeTypeId = item.ChargeTypeId,
                    ContainerSizeTypeId = item.ContainerSizeTypeId,
                    CurrencyId = item.CurrencyId,
                    Quantity = item.Quantity,
                    Remarks = item.Remarks,
                    UnitRate = item.UnitRate
                });
            }
            await PopulateSubmitQuoteLookupsAsync(vm);
            return PartialView("_SubmitQuote", vm);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> SubmitQuote(SubmitQuoteVm vm)
        {
            var vendorId = await _currentUserService.GetPersonIdAsync();
            if (vendorId == null || vendorId == Guid.Empty) { return BadRequest(); }

            var inquiry = await _context.RateRequestInquiries
                .FirstOrDefaultAsync(x =>
                    x.Id == vm.RateRequestInquiryId &&
                    x.VendorId == vendorId);

            if (inquiry == null)
                return NotFound();

            if (inquiry.StatusId == 5) //Rejected
            {
                ModelState.AddModelError("", "This inquiry has already been rejected.");
                return PartialView("_SubmitQuote", vm);
            }

            if (!ModelState.IsValid)
            {
                await PopulateSubmitQuoteLookupsAsync(vm);
                return PartialView("_SubmitQuote", vm);
            }
            if (vm.Id == Guid.Empty)
            {
                var quote = new BuyQuote
                {
                    Id = Guid.CreateVersion7(),
                    RateRequestInquiryId = inquiry.Id,
                    VendorId = vendorId,
                    QuotationNo = vm.QuotationNo,
                    OptionsServiceName = vm.OptionsServiceName,
                    TransitDays = vm.TransitDays,
                    CreatedAt = DateTime.Now,
                    StatusId = 1,
                    CreateUserId = _currentUserService.UserId,
                    CarrierName = vm.CarrierName,
                    VendorName = vm.VendorName,
                    CarrierId = vm.CarrierId,
                    VehicleName = vm.VehicleName,
                    TransportMovementNo = vm.TransportMovementNo,
                    ETA = vm.ETA,
                    ETD = vm.ETD,
                    ValidUntil = vm.ValidUntil,
                    QuotationDate = vm.QuotationDate,
                    IsDirect = vm.IsDirect,
                    Remarks = vm.Remarks
                };

                foreach (var item in vm.Items)
                {
                    quote.BuyQuoteItems.Add(new BuyQuoteItem
                    {
                        Id = Guid.CreateVersion7(),
                        BuyQuoteId = quote.Id,
                        ChargeTypeId = item.ChargeTypeId.Value,
                        Quantity = item.Quantity.Value,
                        UnitRate = item.UnitRate.Value,
                        CurrencyId = item.CurrencyId.Value,
                        TotalAmount = item.Quantity.Value * item.UnitRate.Value,
                        CreateUserId = _currentUserService.UserId,
                        CreatedAt = DateTime.Now,
                        ContainerSizeTypeId = item.ContainerSizeTypeId,
                        Remarks = item.Remarks
                    });
                }
                _context.BuyQuotes.Add(quote);
            }
            else
            {
                var quote = await _context.BuyQuotes.Include(x => x.BuyQuoteItems).Where(x => x.Id == vm.Id).FirstOrDefaultAsync();
                if (quote == null) return NotFound();
                quote.VendorId = vendorId;
                quote.QuotationNo = vm.QuotationNo;
                quote.OptionsServiceName = vm.OptionsServiceName;
                quote.TransitDays = vm.TransitDays;
                quote.CreatedAt = DateTime.Now;
                quote.StatusId = 1;
                quote.CreateUserId = _currentUserService.UserId;
                quote.CarrierName = vm.CarrierName;
                quote.VendorName = vm.VendorName;
                quote.CarrierId = vm.CarrierId;
                quote.VehicleName = vm.VehicleName;
                quote.TransportMovementNo = vm.TransportMovementNo;
                quote.ETA = vm.ETA;
                quote.ETD = vm.ETD;
                quote.ValidUntil = vm.ValidUntil;
                quote.QuotationDate = vm.QuotationDate;
                quote.IsDirect = vm.IsDirect;
                quote.Remarks = vm.Remarks;

                foreach (var item in quote.BuyQuoteItems)
                {
                    item.IsDeleted = true;
                    item.DeleteUserId = _currentUserService.UserId;
                    item.DeletedAt = DateTime.Now;
                }
                foreach (var item in vm.Items)
                {
                    _context.BuyQuoteItems.Add(new BuyQuoteItem
                    {
                        Id = Guid.CreateVersion7(),
                        BuyQuoteId = quote.Id,
                        ChargeTypeId = item.ChargeTypeId.Value,
                        Quantity = item.Quantity.Value,
                        UnitRate = item.UnitRate.Value,
                        CurrencyId = item.CurrencyId.Value,
                        TotalAmount = item.Quantity.Value * item.UnitRate.Value,
                        CreateUserId = _currentUserService.UserId,
                        CreatedAt = DateTime.Now,
                        ContainerSizeTypeId = item.ContainerSizeTypeId,
                        Remarks = item.Remarks
                    });
                }
            }
            await _context.SaveChangesAsync();
            Response.Headers["HX-Trigger"] = "quoteSaved";
            return NoContent();
        }
        // GET: Containers for Inquiry (optional helper)
        [HttpGet]
        public async Task<IActionResult> GetContainers(Guid inquiryId)
        {
            var containers = await _context.RateRequestInquiries
                .Where(i => i.Id == inquiryId)
                .SelectMany(i => i.RateRequest.Containers)
                .Select(c => new
                {
                    id = c.Id,
                    name = c.ContainerSizeType.ShortName
                })
                .ToListAsync();

            return Json(containers);
        }

        [HttpGet]
        public async Task<IActionResult> SubmitQuote(Guid inquiryId)
        {
            var inquiry = await _context.RateRequestInquiries
                .AsNoTracking()
                .FirstOrDefaultAsync(x => x.Id == inquiryId);

            if (inquiry == null)
                return NotFound();

            var vm = new SubmitQuoteVm
            {
                RateRequestInquiryId = inquiry.Id,
                QuotationDate = DateTime.Now,
                VendorId = inquiry.VendorId,
                Items = new List<SubmitQuoteItemVm>
                        {
                            new SubmitQuoteItemVm()
                        }
            };
            await PopulateSubmitQuoteLookupsAsync(vm);
            return PartialView("_SubmitQuote", vm);
        }


        //[HttpPost]
        //[ValidateAntiForgeryToken]
        //public async Task<IActionResult> SubmitQuote(SubmitQuoteVm vm)
        //{
        //    var inquiry = await _context.RateRequestInquiries
        //        .FirstOrDefaultAsync(x => x.Id == vm.RateRequestInquiryId && !x.IsDeleted);

        //    if (inquiry == null)
        //        return NotFound();

        //    var alreadyResponded = await _context.BuyQuotes
        //        .AnyAsync(x => x.RateRequestInquiryId == vm.RateRequestInquiryId && !x.IsDeleted);

        //    if (alreadyResponded)
        //    {
        //        ModelState.AddModelError(string.Empty, "A quote has already been submitted for this inquiry.");
        //    }

        //    vm.Items = vm.Items?
        //        .Where(x => x.ChargeTypeId.HasValue || x.CurrencyId.HasValue || x.Quantity.HasValue || x.UnitRate.HasValue)
        //        .ToList() ?? new List<SubmitQuoteItemVm>();

        //    if (!vm.Items.Any())
        //    {
        //        ModelState.AddModelError(string.Empty, "At least one quote item is required.");
        //    }

        //    for (int i = 0; i < vm.Items.Count; i++)
        //    {
        //        var item = vm.Items[i];

        //        if (!item.ChargeTypeId.HasValue)
        //            ModelState.AddModelError($"Items[{i}].ChargeTypeId", "Charge type is required.");

        //        if (!item.CurrencyId.HasValue)
        //            ModelState.AddModelError($"Items[{i}].CurrencyId", "Currency is required.");

        //        if (!item.Quantity.HasValue || item.Quantity <= 0)
        //            ModelState.AddModelError($"Items[{i}].Quantity", "Quantity must be greater than zero.");

        //        if (!item.UnitRate.HasValue || item.UnitRate <= 0)
        //            ModelState.AddModelError($"Items[{i}].UnitRate", "Unit rate must be greater than zero.");
        //    }

        //    if (!ModelState.IsValid)
        //    {
        //        await PopulateSubmitQuoteLookupsAsync(vm);
        //        return PartialView("_SubmitQuote", vm);
        //    }

        //    using var tx = await _context.Database.BeginTransactionAsync();

        //    try
        //    {
        //        var buyQuote = new BuyQuote
        //        {
        //            Id = Guid.NewGuid(),
        //            RateRequestInquiryId = vm.RateRequestInquiryId,
        //            CarrierId = vm.CarrierId,
        //            VendorId = vm.VendorId ?? inquiry.VendorId,
        //            QuotationNo = vm.QuotationNo.Trim(),
        //            QuotationDate = vm.QuotationDate,
        //            OptionsServiceName = vm.OptionsServiceName?.Trim(),
        //            VehicleName = vm.VehicleName?.Trim(),
        //            TransportMovementNo = vm.TransportMovementNo?.Trim(),
        //            ETD = vm.ETD,
        //            ETA = vm.ETA,
        //            TransitDays = vm.TransitDays,
        //            IsDirect = vm.IsDirect,
        //            ValidUntil = vm.ValidUntil,
        //            Remarks = vm.Remarks?.Trim(),
        //            StatusId = 2,
        //            CarrierName = null,
        //            VendorName = null,
        //            IsActive = true,
        //            IsDeleted = false,
        //            CreatedAt = DateTime.Now,
        //            CreateUserId = _currentUserService.UserId,
        //        };

        //        _context.BuyQuotes.Add(buyQuote);
        //        await _context.SaveChangesAsync();

        //        var items = vm.Items.Select(item => new BuyQuoteItem
        //        {
        //            Id = Guid.NewGuid(),
        //            BuyQuoteId = buyQuote.Id,
        //            ChargeTypeId = item.ChargeTypeId!.Value,
        //            ContainerSizeTypeId = item.ContainerSizeTypeId,
        //            Quantity = item.Quantity!.Value,
        //            UnitRate = item.UnitRate!.Value,
        //            TotalAmount = item.Quantity.Value * item.UnitRate.Value,
        //            CurrencyId = item.CurrencyId!.Value,
        //            Remarks = item.Remarks?.Trim(),
        //            IsActive = true,
        //            IsDeleted = false,
        //            CreatedAt = DateTime.Now,
        //            CreateUserId = _currentUserService.UserId
        //        }).ToList();

        //        _context.BuyQuoteItems.AddRange(items);

        //        inquiry.StatusId = 4;
        //        inquiry.UpdatedAt = DateTime.Now;
        //        inquiry.EditUserId = _currentUserService.UserId;

        //        await _context.SaveChangesAsync();
        //        await tx.CommitAsync();

        //        Response.Headers["HX-Trigger"] = "quoteSaved";
        //        return NoContent();
        //    }
        //    catch
        //    {
        //        await tx.RollbackAsync();
        //        ModelState.AddModelError(string.Empty, "An error occurred while saving the quote.");
        //        await PopulateSubmitQuoteLookupsAsync(vm);
        //        return PartialView("_SubmitQuote", vm);
        //    }
        //}

        [HttpGet]
        public async Task<IActionResult> RejectInquiry(Guid inquiryId)
        {
            var inquiry = await _context.RateRequestInquiries
                .AsNoTracking()
                .FirstOrDefaultAsync(x => x.Id == inquiryId && x.StatusId == 2);

            if (inquiry == null)
                return NotFound();

            var vm = new RejectInquiryVm
            {
                RateRequestInquiryId = inquiry.Id
            };

            return PartialView("_RejectInquiry", vm);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> RejectInquiry(RejectInquiryVm vm)
        {
            if (!ModelState.IsValid)
                return PartialView("_RejectInquiry", vm);

            var inquiry = await _context.RateRequestInquiries
                .FirstOrDefaultAsync(x => x.Id == vm.RateRequestInquiryId);

            if (inquiry == null)
                return NotFound();

            inquiry.StatusId = 5;  //Rejected
            inquiry.RejectReason = vm.Reason;
            inquiry.RejectedByUserId = _currentUserService.UserId;
            inquiry.RejectDateTime = DateTime.Now;
            await _context.SaveChangesAsync();
            return NoContent();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> SendQuoteResponseInApp(Guid quoteId)
        {
            var quote = await _context.BuyQuotes
                .FirstOrDefaultAsync(x => x.Id == quoteId);
            if (quote == null) return NotFound();
            quote.StatusId = 2;  //Sent
            quote.QuotationDate = DateTime.Now;
            quote.UpdatedAt = DateTime.Now;
            quote.EditUserId = _currentUserService.UserId;

            var inq = await _context.RateRequestInquiries.Where(x => x.Id == quote.RateRequestInquiryId).FirstOrDefaultAsync();
            inq.StatusId = 4;
            await _context.SaveChangesAsync();
            Response.Headers["HX-Trigger"] = "quoteSaved";
            return NoContent();
        }
        public async Task<IActionResult> DeleteQuote(Guid quoteId)
        {
            var quote = await _context.BuyQuotes
                .FirstOrDefaultAsync(x => x.Id == quoteId);
            if (quote == null) return NotFound();
            quote.DeletedAt = DateTime.Now;
            quote.DeleteUserId = _currentUserService.UserId;
            quote.IsDeleted = true;
            await _context.SaveChangesAsync();
            Response.Headers["HX-Trigger"] = "quoteSaved";
            return NoContent();
        }
        private async Task PopulateSubmitQuoteLookupsAsync(SubmitQuoteVm vm)
        {
            vm.CarrierOptions = await _context.Carriers
                .Include(X => X.Person)
                .Where(x => !x.IsDeleted)
                .OrderBy(x => x.Person.PersonName)
                .Select(x => new SelectListItem
                {
                    Value = x.Id.ToString(),
                    Text = x.Person.PersonName
                })
                .ToListAsync();


            vm.VendorOptions = await _context.Carriers
                .Include(X => X.Person)
                .Where(x => !x.IsDeleted)
                .OrderBy(x => x.Person.PersonName)
                .Select(x => new SelectListItem
                {
                    Value = x.Id.ToString(),
                    Text = x.Person.PersonName
                })
                .ToListAsync();




            vm.ChargeTypeOptions = await _context.ChargeTypes
                .OrderBy(x => x.ChargeTypeName)
                .Select(x => new SelectListItem
                {
                    Value = x.Id.ToString(),
                    Text = x.ChargeTypeName
                })
                .ToListAsync();

            vm.ContainerSizeTypeOptions = await _context.ContainerSizeTypes
                .OrderBy(x => x.ShortName)
                .Select(x => new SelectListItem
                {
                    Value = x.Id.ToString(),
                    Text = x.ShortName
                })
                .ToListAsync();

            vm.CurrencyOptions = await _context.Currencies
                .OrderBy(x => x.CurrencyCode)
                .Select(x => new SelectListItem
                {
                    Value = x.Id.ToString(),
                    Text = x.CurrencyCode
                })
                .ToListAsync();
            vm.Items ??= new List<SubmitQuoteItemVm>();
            if (!vm.Items.Any())
                vm.Items.Add(new SubmitQuoteItemVm());
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AddQuoteItem(SubmitQuoteVm vm)
        {
            vm.Items ??= new List<SubmitQuoteItemVm>();
            vm.Items.Add(new SubmitQuoteItemVm());
            await PopulateSubmitQuoteLookupsAsync(vm);
            return PartialView("_QuoteItemsEditor", vm);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> RemoveQuoteItem(SubmitQuoteVm vm, int index)
        {
            vm.Items ??= new List<SubmitQuoteItemVm>();
            if (index >= 0 && index < vm.Items.Count)
                vm.Items.RemoveAt(index);
            await PopulateSubmitQuoteLookupsAsync(vm);
            return PartialView("_QuoteItemsEditor", vm);
        }
        [HttpGet]
        public async Task<IActionResult> PreviewInquiryEmailFromService(Guid inquiryId, bool canSendEmail = false)
        {
            var html = await _rateInquiryEmailService.BuildInquiryEmailHtmlAsync(inquiryId, canSendEmail, false);
            return Content(html, "text/html");
        }
    }
}