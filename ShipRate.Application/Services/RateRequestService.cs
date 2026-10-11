using Microsoft.EntityFrameworkCore;
using ShipRate.Application.DTOs;
using ShipRate.Application.Interfaces;
using ShipRate.Application.Interfaces.Cargos;
using ShipRate.Application.Interfaces.Jobs;
using ShipRate.Application.Utilities;
using ShipRate.Application.ViewModels;
using ShipRate.Domain.Entities.RateQuotation;
using ShipRate.Infrastructure.Data;
using System.Data;

namespace ShipRate.Application.Services
{

    public class RateRequestService
    {
        private readonly ApplicationDbContext _context;
        private readonly IEmailService _emailService;
        private readonly ICurrentUserService _currentUserService;
        private readonly IJobLookupService _lookupService;
        private readonly IRazorViewRenderService _razorViewRenderService;
        private readonly IRateInquiryEmailService _rateInquiryEmailService;
        private readonly IJobService _jobService;
        private readonly ICargoDetailsService _cargoDetailsService;
        private readonly IActivityLogService _activityLogService;

        public RateRequestService(ApplicationDbContext context, IEmailService emailService, ICurrentUserService currentUserService, IJobLookupService lookupService, IRazorViewRenderService razorViewRenderService, IRateInquiryEmailService rateInquiryEmailService, IJobService jobService, ICargoDetailsService cargoDetailsService, IActivityLogService activityLogSerice)
        {
            _context = context;
            _emailService = emailService;
            _currentUserService = currentUserService;
            _lookupService = lookupService;
            _razorViewRenderService = razorViewRenderService;
            _rateInquiryEmailService = rateInquiryEmailService;
            _jobService = jobService;
            _cargoDetailsService = cargoDetailsService;
            _activityLogService = activityLogSerice;
        }

        public async Task<ServiceResult<RateRequestListDto>> GetList(RateRequestSearchDto search)
        {
            var currentCustomer = await _currentUserService.GetPersonIdAsync();
            var query = _context.RateRequests.Where(x => x.CustomerId != null && x.CustomerId == currentCustomer && !x.IsDeleted).AsQueryable();

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

            if (search.OriginId.HasValue)
                query = query.Where(r => r.OriginId == search.OriginId.Value);

            if (search.DestinationId.HasValue)
                query = query.Where(r => r.DestinationId == search.DestinationId.Value);

            if (search.CargoReadinessDateFrom.HasValue)
                query = query.Where(r => r.CargoReadinessDate >= search.CargoReadinessDateFrom.Value);

            if (search.CargoReadinessDateTo.HasValue)
                query = query.Where(r => r.CargoReadinessDate <= search.CargoReadinessDateTo.Value);

            // Total count for pagination
            var totalCount = await query.CountAsync();

            // Pagination
            var items = await query
                .OrderByDescending(r => r.CreatedAt)
                .Skip((search.PageNumber - 1) * search.PageSize)
                .Take(search.PageSize)
                .Select(r => new RateRequestItemDto
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
                })
                .ToListAsync();

            var viewModel = new RateRequestListDto
            {
                Items = items,
                Search = search,
                Pagination = new PaginationDto
                {
                    PageNumber = search.PageNumber,
                    PageSize = search.PageSize,
                    TotalCount = totalCount
                }
            };

            return ServiceResult<RateRequestListDto>.Success(viewModel);
        }



        public async Task<Utilities.ServiceResult> CreateRateRequest(RateRequestFormDto model)
        {
            var personId = await _currentUserService.GetPersonIdAsync();
            var customer = await _context.Customers.Where(x => x.Id == personId).Include(x => x.Person).FirstOrDefaultAsync();
            if (customer == null)
            {
                return Utilities.ServiceResult.Failed("Customer is not defined");
            }
            var requestNumber = await GenerateRateRequestNumberAsync();
            if (!requestNumber.Succeeded)
            {
                return Utilities.ServiceResult.Failed("Error in generating RequestNumber");
            }
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
                RequestNumber = requestNumber.Data,
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
            };
            rateRequest.StatusId = 2;
            if (customer.SalesUserId != null)
            {
                rateRequest.AssignedSalesUserId = customer.SalesUserId;
                rateRequest.AssignedSalesUserDate = DateTime.Now;
            }

            await _context.Database.BeginTransactionAsync();
            _context.RateRequests.Add(rateRequest);
            await _context.SaveChangesAsync();
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
            try
            {
                await _context.SaveChangesAsync();
                await _context.Database.CommitTransactionAsync();
            }
            catch (Exception e)
            {
                await _context.Database.RollbackTransactionAsync();
                return Utilities.ServiceResult.Failed("Error in save rate request");
            }

            model.Id = rateRequest.Id;
            return Utilities.ServiceResult.Success();
        }

        public async Task<ServiceResult<string>> GenerateRateRequestNumberAsync(CancellationToken cancellationToken = default)
        {
            var connection = _context.Database.GetDbConnection();

            if (connection.State != ConnectionState.Open)
                await connection.OpenAsync(cancellationToken);

            await using var command = connection.CreateCommand();
            command.CommandText = "SELECT NEXT VALUE FOR [ops].[RateRequestNumberSeq]";

            var result = await command.ExecuteScalarAsync(cancellationToken);
            var nextNumber = Convert.ToInt32(result);

            return ServiceResult<string>.Success($"Req-{nextNumber:00000}");
        }


        public async Task<Utilities.ServiceResult> EditRateRequest(RateRequestFormDto model)
        {
            var rateRequest = await _context.RateRequests
               .Include(r => r.TransportTypes)
               .Include(r => r.Packages)
               .Include(r => r.Containers)
               .FirstOrDefaultAsync(r => r.Id == model.Id);

            if (rateRequest == null)
                return Utilities.ServiceResult.Failed("Rate request does not exist");

            await _context.Database.BeginTransactionAsync();
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
            rateRequest.TradeTypeId = model.PaymentTypeId;
            rateRequest.MovementTypeId = model.MovementTypeId;
            rateRequest.Remarks = model.Remarks;

            rateRequest.PickupRequired = model.PickupRequired;
            rateRequest.PickupAddress = model.PickupAddress;
            rateRequest.OriginZipCode = model.OriginZipCode;

            rateRequest.DeliveryRequired = model.DeliveryRequired;
            rateRequest.DeliveryAddress = model.DeliveryAddress;
            rateRequest.DestinationZipCode = model.DestinationZipCode;

            rateRequest.CustomsClearanceRequired = model.CustomsClearanceRequired;
            rateRequest.InsuranceRequired = model.InsuranceRequired;
            rateRequest.WarehousingRequired = model.WarehousingRequired;
            rateRequest.StuffingRequired = model.StuffingRequired;

            rateRequest.EditUserId = _currentUserService.UserId;
            rateRequest.UpdatedAt = DateTime.Now;

            // حذف TransportTypes قدیمی
            _context.RateRequestTransportTypes.RemoveRange(rateRequest.TransportTypes);

            // اضافه کردن TransportTypes جدید
            if (model.TransportTypes?.Any() == true)
            {
                foreach (var tt in model.TransportTypes)
                {
                    rateRequest.TransportTypes.Add(new RateRequestTransportType
                    {
                        TransportTypeId = tt.TransportTypeId
                    });
                }
            }

            // حذف Packages قدیمی
            _context.RateRequestPackages.RemoveRange(rateRequest.Packages);
            // اضافه کردن Packages جدید
            if (model.Packages?.Any() == true)
            {
                foreach (var p in model.Packages)
                {
                    rateRequest.Packages.Add(new RateRequestPackage
                    {
                        PackageTypeId = p.PackageTypeId,
                        Remarks = p.Remarks,
                        UpdatedAt = DateTime.Now,
                        //Quantity = p.Quantity,
                        //Length = p.Length,
                        //Width = p.Width,
                        //Height = p.Height,
                        //Weight = p.Weight
                    });
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
                        ContainerSizeTypeId = c.ContainerSizeTypeId,
                        Counts = c.Counts
                    });
                }
            }
            try
            {
                await _context.SaveChangesAsync();
                await _context.Database.CommitTransactionAsync();
            }
            catch (Exception)
            {

                await _context.Database.RollbackTransactionAsync();
                return Utilities.ServiceResult.Failed("error in save Rate request");
            }
            return Utilities.ServiceResult.Success();
        }


        public async Task<Utilities.ServiceResult> DeleteRateRequest(Guid id)
        {
            var rateRequest = await _context.RateRequests
               .Include(r => r.TransportTypes)
               .Include(r => r.Packages)
               .Include(r => r.Containers)
               .FirstOrDefaultAsync(r => r.Id == id);

            if (rateRequest == null)
                return Utilities.ServiceResult.Failed("Rate REquest doesnt exists");

            await _context.Database.BeginTransactionAsync();

            var tt = await _context.RateRequestTransportTypes.Where(x => x.RateRequestId == id).ToListAsync();



            foreach (var t in tt)
            {
                t.IsDeleted = true;
                t.UpdatedAt = DateTime.Now;
                t.DeleteUserId = _currentUserService.UserId;
            }
            _context.UpdateRange(tt);

            var packages = await _context.RateRequestPackages.Where(x => x.RateRequestId == id).ToListAsync();

            foreach (var p in packages)
            {
                p.IsDeleted = true;
                p.UpdatedAt = DateTime.Now;
                p.DeleteUserId = _currentUserService.UserId;
            }

            _context.UpdateRange(packages);

            var containers = await _context.RateRequestContainers.Where(x => x.RateRequestId == id).ToListAsync();

            foreach (var c in containers)
            {
                c.IsDeleted = true;
                c.UpdatedAt = DateTime.Now;
                c.DeleteUserId = _currentUserService.UserId;
            }
            _context.UpdateRange(containers);


            rateRequest.IsDeleted = true;
            rateRequest.UpdatedAt = DateTime.Now;
            rateRequest.DeleteUserId = _currentUserService.UserId;
            _context.Update(rateRequest);
            try
            {
                await _context.SaveChangesAsync();
                await _context.Database.CommitTransactionAsync();
                return Utilities.ServiceResult.Success();
            }
            catch (Exception)
            {
                await _context.Database.RollbackTransactionAsync();
                return Utilities.ServiceResult.Failed("Error in Save Changes");

            }
        }


        public async Task<ServiceResult<SellQuoteListIndexDto>> GetSellQuotes(Guid rateRequestId)
        {

            var bq = await _context.SellQuotes
                .Include(r => r.RateRequest)
                .Where(r => r.RateRequestId == rateRequestId)
                .Select(r => new SellQuoteListDto
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
                return ServiceResult<SellQuoteListIndexDto>.Success(new SellQuoteListIndexDto { SellQuotesList = new List<SellQuoteListDto>() });


            var model = new SellQuoteListIndexDto
            {
                RateRequestId = rateRequestId,
                SellQuotesList = bq
            };
            return ServiceResult<SellQuoteListIndexDto>.Success(model);
        }

        public async Task<ServiceResult<AssignSalesUserDto>> GetAssignedSalesUser(Guid rateRequestId)
        {
            var rr = await _context.RateRequests
               .Include(c => c.AssignedSalesUser)
               .FirstOrDefaultAsync(c => c.Id == rateRequestId);

            if (rr == null)
            {
                return ServiceResult<AssignSalesUserDto>.Failed("Rate request not found");
            }

            var vm = new AssignSalesUserDto
            {
                CustomerId = rr.Id,
                PersonName = rr.RequestNumber,
                CurrentSalesUserId = rr.AssignedSalesUserId,
                CurrentSalesUserName = rr.AssignedSalesUser?.FullName,
                SelectedSalesUserId = rr.AssignedSalesUserId
            };
            return ServiceResult<AssignSalesUserDto>.Success(vm);
        }


        public async Task<Utilities.ServiceResult> AssignSalesUser(AssignSalesUserDto vm)
        {
            var rr = await _context.RateRequests.FindAsync(vm.CustomerId);
            if (rr == null) throw new Exception("Rate request Not Found");
            rr.AssignedSalesUserId = vm.SelectedSalesUserId;
            rr.AssignedSalesUserDate = DateTime.Now;
            rr.UpdatedAt = DateTime.Now;
            await _context.SaveChangesAsync();
            return Utilities.ServiceResult.Success();
        }
        public async Task<Utilities.ServiceResult> SaveSellQuote(CreateSellQuoteDto model)
        {
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

                        if (sellQuote == null) return Utilities.ServiceResult.Failed("sellQuote does not exists");

                        quoteId = sellQuote.Id;
                        sellQuote.ServiceName = model.ServiceName;
                        sellQuote.ValidUntil = model.ValidUntil.Value;
                        sellQuote.VehicleNames = model.VehicleNames;
                        sellQuote.MovementNumbers = model.MovementNumbers;
                        sellQuote.Remarks = model.Remarks;
                        sellQuote.ETD = model.ETD.Value;
                        sellQuote.ETA = model.ETA.Value;
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
                    return Utilities.ServiceResult.Success();
                }
                catch (Exception ex)
                {
                    await transaction.RollbackAsync();
                    return Utilities.ServiceResult.Failed("error in saving sell quote");
                }
            }
        }

        public async Task<ServiceResult<SellQuoteListDto>> GetSellQuoteById(Guid id)
        {

            var sq = await _context.SellQuotes
                .Where(x => x.Id == id)
                .Select(r => new SellQuoteListDto
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
                .FirstOrDefaultAsync();

            if (sq == null)
            {
                return ServiceResult<SellQuoteListDto>.Failed("This sell quote does not exists");
            }
            return ServiceResult<SellQuoteListDto>.Success(sq);
        }

        public async Task<ServiceResult<List<SellQuoteItemListDto>>> GetSellQuoteItems(Guid? quoteId)
        {
            var bq = await _context.SellQuoteItems
                .Include(r => r.ChargeType)
                .Include(r => r.ContainerSizeType)
                .Include(r => r.BuyCurrency)
                .Include(r => r.SellCurrency)
                .Where(r => r.SellQuoteId == quoteId)
                .Select(r => new SellQuoteItemListDto
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
            return ServiceResult<List<SellQuoteItemListDto>>.Success(bq);
        }


        public async Task<ServiceResult<SellQuoteListDto>> GetSellQuoteDetails(Guid? quoteId)
        {

            var sq = await _context.SellQuotes
                    .Where(r => r.Id == quoteId)
                    .Select(r => new SellQuoteListDto
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
                return ServiceResult<SellQuoteListDto>.Failed("sell quote does not exists");

            var sqi = await _context.SellQuoteItems
                .Include(x => x.ChargeType)
                .Include(x => x.ContainerSizeType)
                .Include(x => x.SellCurrency)
                .Include(x => x.BuyCurrency)
                .Where(x => x.SellQuoteId == quoteId)
                .Select(x => new SellQuoteItemListDto
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

            return ServiceResult<SellQuoteListDto>.Success(sq);
        }


        public async Task<Utilities.ServiceResult> RejectSellQuote(RejectSellQuoteVm vm)
        {
            var sq = await _context.SellQuotes
                .AsNoTracking()
                .FirstOrDefaultAsync(x => x.Id == vm.SellQuoteId);

            if (sq == null)
                return Utilities.ServiceResult.Failed("this sell quote doesnt exists");


            sq.StatusId = 4;  //Rejected
            sq.CustomerRespondComments = vm.Comments;
            sq.CustomerRespondUserId = _currentUserService.UserId;
            sq.CustomerRespondDate = DateTime.Now;
            await _context.SaveChangesAsync();
            return Utilities.ServiceResult.Success();
        }

        public async Task<Utilities.ServiceResult> AcceptSellQuote(AcceptSellQuoteVm vm)
        {
            var sq = await _context.SellQuotes
                .AsNoTracking()
                .FirstOrDefaultAsync(x => x.Id == vm.SellQuoteId);

            if (sq == null)
                return Utilities.ServiceResult.Failed("this sell quote doesnt exists");

            sq.StatusId = 3;  //Accepted
            sq.CustomerRespondComments = vm.Comments;
            sq.CustomerRespondUserId = _currentUserService.UserId;
            sq.CustomerRespondDate = DateTime.Now;
            await _context.SaveChangesAsync();
            return Utilities.ServiceResult.Success();
        }
        public async Task<ServiceResult<List<PartyLookupItemVm>>> SearchPartiesForRateRequest(
          string? term,
          string targetHiddenInputId = "CPartyId",
          string targetTextInputId = "CPartySearchText",
          CancellationToken cancellationToken = default)
        {
            var items = await _jobService.SearchPartiesAsync(term);
            return ServiceResult<List<PartyLookupItemVm>>.Success(items);
        }

        public async Task<Utilities.ServiceResult> RejectRateRequest(RejectRateRequestVm vm)
        {
            var rr = await _context.RateRequests
                .FirstOrDefaultAsync(x => x.Id == vm.RateRequestId);

            if (rr == null)
                return Utilities.ServiceResult.Failed("this rate request doesnt exists");

            rr.StatusId = 7;  //درست شود
            rr.RejectComments = vm.Comments;
            rr.RejectDate = DateTime.Now;
            rr.RejectUserId = _currentUserService.UserId;

            await _context.SaveChangesAsync();

            await _activityLogService.LogHistoryAsync(
                new CreateActivityLogRequest
                {
                    ActivityCategoryId = 2,
                    ActivityDateTime = DateTime.Now,
                    Title = "Rate Request Rejected ",
                    Description = "Rate Request Rejected",
                    EntityTypeId = 8,
                    EntityId = vm.RateRequestId,
                    FromStatusEntityType = "RateRequestStatus",
                    ToStatusId = 7,
                    PerformedByUserId = _currentUserService.UserId,
                    IsSystemGenerated = true,
                }
                );
            return Utilities.ServiceResult.Success();
        }
    }

    //public class RateRequestService : IRateRequestService
    //{
    //    private readonly IUnitOfWork _unitOfWork;
    //    private readonly ILogger<RateRequestService> _logger;

    //    public RateRequestService(IUnitOfWork unitOfWork, ILogger<RateRequestService> logger)
    //    {
    //        _unitOfWork = unitOfWork;
    //        _logger = logger;
    //    }

    //    public async Task<RateRequestDto> GetByIdAsync(Guid id)
    //    {
    //        var rateRequest = await _unitOfWork.RateRequests.GetByIdWithDetailsAsync(id);

    //        if (rateRequest == null)
    //        {
    //            throw new KeyNotFoundException($"Rate request with ID {id} not found");
    //        }

    //        return MapToDto(rateRequest);
    //    }

    //    public async Task<PagedResultDto<RateRequestDto>> SearchAsync(RateRequestSearchDto searchDto)
    //    {
    //        try
    //        {
    //            var result = await _unitOfWork.RateRequests.SearchAsync(
    //                searchDto.SearchTerm,
    //                searchDto.CustomerId,
    //                searchDto.OriginId,
    //                searchDto.DestinationId,
    //                searchDto.ShipmentType,
    //                searchDto.TransportMode,
    //                searchDto.Status,
    //                searchDto.RequestDateFrom,
    //                searchDto.RequestDateTo,
    //                searchDto.PageNumber,
    //                searchDto.PageSize
    //            );

    //            return new PagedResult<RateRequestDto>
    //            {
    //                Items = result.Items.Select(MapToDto).ToList(),
    //                TotalCount = result.TotalCount,
    //                PageNumber = result.PageNumber,
    //                PageSize = result.PageSize
    //            };
    //        }
    //        catch (Exception ex)
    //        {
    //            _logger.LogError(ex, "Error searching rate requests");
    //            throw;
    //        }
    //    }

    //    public async Task<RateRequestDto> CreateAsync(RateRequestCreateUpdateDto dto, Guid createdBy)
    //    {
    //        try
    //        {
    //            var rateRequest = new RateRequest
    //            {
    //                //RequestNumber = await GenerateRequestNumberAsync(),
    //                //CustomerId = dto.CustomerId,
    //                //RequestDate = dto.RequestDate,
    //                //ValidityDate = dto.ValidityDate,
    //                //OriginId = dto.OriginId,
    //                //DestinationId = dto.DestinationId,
    //                //ShipmentType = dto.ShipmentType,
    //                //CargoType = dto.CargoType,
    //                //CargoTypeId = dto.CargoTypeId,
    //                //Incoterm = dto.Incoterm,
    //                //TransportMode = dto.TransportMode,
    //                //ServiceType = dto.ServiceType,
    //                //IsTemperatureControlled = dto.IsTemperatureControlled,
    //                //TemperatureRange = dto.TemperatureRange,
    //                //IsHazardous = dto.IsHazardous,
    //                //UNNumber = dto.UNNumber,
    //                //HazardClass = dto.HazardClass,
    //                //RequiresInsurance = dto.RequiresInsurance,
    //                //InsuranceValue = dto.InsuranceValue,
    //                //RequiresCustomsClearance = dto.RequiresCustomsClearance,
    //                //AdditionalServices = dto.AdditionalServices,
    //                //SpecialInstructions = dto.SpecialInstructions,
    //                //Status = RateRequestStatus.Pending,
    //                //CreatedBy = createdBy,
    //                CreatedAt = DateTime.Now
    //            };

    //            // Add Packages
    //            if (dto.Packages != null && dto.Packages.Any())
    //            {
    //                rateRequest.Packages = dto.Packages.Select(p => new Package
    //                {

    //                    //Quantity = p.Quantity,
    //                    //PackageType = p.PackageType,
    //                    //Length = p.Length,
    //                    //Width = p.Width,
    //                    //Height = p.Height,
    //                    //Weight = p.Weight,
    //                    //Volume = p.Volume
    //                }).ToList();
    //            }

    //            // Add FCL Details
    //            if (dto.FCLDetails != null)
    //            {
    //                rateRequest.FCLDetails = new FCLDetails
    //                {
    //                    ContainerType = dto.FCLDetails.ContainerType,
    //                    ContainerCount = dto.FCLDetails.ContainerCount,
    //                    TotalWeight = dto.FCLDetails.TotalWeight
    //                };
    //            }

    //            await _unitOfWork.RateRequests.AddAsync(rateRequest);
    //            await _unitOfWork.SaveChangesAsync();

    //            _logger.LogInformation("Rate request {RequestNumber} created by {User}",
    //                rateRequest.RequestNumber, createdBy);

    //            return await GetByIdAsync(rateRequest.Id);
    //        }
    //        catch (Exception ex)
    //        {
    //            _logger.LogError(ex, "Error creating rate request");
    //            throw;
    //        }
    //    }

    //    public async Task<RateRequestDto> UpdateAsync(Guid id, RateRequestCreateUpdateDto dto, Guid updatedBy)
    //    {
    //        try
    //        {
    //            var rateRequest = await _unitOfWork.RateRequests.GetByIdWithDetailsAsync(id);

    //            if (rateRequest == null)
    //            {
    //                throw new KeyNotFoundException($"Rate request with ID {id} not found");
    //            }

    //            // Update basic properties
    //            rateRequest.CustomerId = dto.CustomerId;
    //            rateRequest.RequestDate = dto.RequestDate;
    //            rateRequest.ValidityDate = dto.ValidityDate;
    //            rateRequest.OriginId = dto.OriginId;
    //            rateRequest.DestinationId = dto.DestinationId;
    //            rateRequest.ShipmentType = dto.ShipmentType;
    //            rateRequest.CargoType = dto.CargoType;
    //            rateRequest.CargoTypeId = dto.CargoTypeId;
    //            rateRequest.Incoterm = dto.Incoterm;
    //            rateRequest.TransportMode = dto.TransportMode;
    //            rateRequest.ServiceType = dto.ServiceType;
    //            rateRequest.IsTemperatureControlled = dto.IsTemperatureControlled;
    //            rateRequest.TemperatureRange = dto.TemperatureRange;
    //            rateRequest.IsHazardous = dto.IsHazardous;
    //            rateRequest.UNNumber = dto.UNNumber;
    //            rateRequest.HazardClass = dto.HazardClass;
    //            rateRequest.RequiresInsurance = dto.RequiresInsurance;
    //            rateRequest.InsuranceValue = dto.InsuranceValue;
    //            rateRequest.RequiresCustomsClearance = dto.RequiresCustomsClearance;
    //            rateRequest.AdditionalServices = dto.AdditionalServices;
    //            rateRequest.SpecialInstructions = dto.SpecialInstructions;
    //            rateRequest.UpdatedBy = updatedBy;
    //            rateRequest.UpdatedAt = DateTime.Now;

    //            // Update Packages
    //            if (rateRequest.Packages != null)
    //            {
    //                rateRequest.Packages.Clear();
    //            }

    //            if (dto.Packages != null && dto.Packages.Any())
    //            {
    //                rateRequest.Packages = dto.Packages.Select(p => new Package
    //                {
    //                    RateRequestId = id,
    //                    Quantity = p.Quantity,
    //                    PackageType = p.PackageType,
    //                    Length = p.Length,
    //                    Width = p.Width,
    //                    Height = p.Height,
    //                    Weight = p.Weight,
    //                    Volume = p.Volume
    //                }).ToList();
    //            }

    //            // Update FCL Details
    //            if (dto.FCLDetails != null)
    //            {
    //                if (rateRequest.FCLDetails == null)
    //                {
    //                    rateRequest.FCLDetails = new FCLDetails
    //                    {
    //                        RateRequestId = id
    //                    };
    //                }

    //                rateRequest.FCLDetails.ContainerType = dto.FCLDetails.ContainerType;
    //                rateRequest.FCLDetails.ContainerCount = dto.FCLDetails.ContainerCount;
    //                rateRequest.FCLDetails.TotalWeight = dto.FCLDetails.TotalWeight;
    //            }
    //            else
    //            {
    //                rateRequest.FCLDetails = null;
    //            }

    //            _unitOfWork.RateRequests.Update(rateRequest);
    //            await _unitOfWork.SaveChangesAsync();

    //            _logger.LogInformation("Rate request {RequestNumber} updated by {User}",
    //                rateRequest.RequestNumber, updatedBy);

    //            return await GetByIdAsync(id);
    //        }
    //        catch (Exception ex)
    //        {
    //            _logger.LogError(ex, "Error updating rate request {Id}", id);
    //            throw;
    //        }
    //    }

    //    public async Task DeleteAsync(int id)
    //    {
    //        try
    //        {
    //            var rateRequest = await _unitOfWork.RateRequests.GetByIdAsync(id);

    //            if (rateRequest == null)
    //            {
    //                throw new KeyNotFoundException($"Rate request with ID {id} not found");
    //            }

    //            _unitOfWork.RateRequests.Delete(rateRequest);
    //            await _unitOfWork.SaveChangesAsync();

    //            _logger.LogInformation("Rate request {RequestNumber} deleted", rateRequest.RequestNumber);
    //        }
    //        catch (Exception ex)
    //        {
    //            _logger.LogError(ex, "Error deleting rate request {Id}", id);
    //            throw;
    //        }
    //    }

    //    public async Task<string> GenerateRequestNumberAsync()
    //    {
    //        var year = DateTime.Now.Year;
    //        var lastRequest = await _unitOfWork.RateRequests.GetLastRequestByYearAsync(year);

    //        int sequence = 1;
    //        if (lastRequest != null && !string.IsNullOrEmpty(lastRequest.RequestNumber))
    //        {
    //            var parts = lastRequest.RequestNumber.Split('-');
    //            if (parts.Length == 3 && int.TryParse(parts[2], out int lastSequence))
    //            {
    //                sequence = lastSequence + 1;
    //            }
    //        }

    //        return $"RR-{year}-{sequence:D6}";
    //    }

    //    public async Task<List<RateRequestDto>> GetByCustomerIdAsync(int customerId)
    //    {
    //        var rateRequests = await _unitOfWork.RateRequests.GetByCustomerIdAsync(customerId);
    //        return rateRequests.Select(MapToDto).ToList();
    //    }

    //    public async Task<List<RateRequestDto>> GetPendingRequestsAsync()
    //    {
    //        var rateRequests = await _unitOfWork.RateRequests.GetByStatusAsync(RateRequestStatus.Pending);
    //        return rateRequests.Select(MapToDto).ToList();
    //    }

    //    public async Task<bool> ExistsAsync(int id)
    //    {
    //        var rateRequest = await _unitOfWork.RateRequests.GetByIdAsync(id);
    //        return rateRequest != null;
    //    }

    //    // Private mapping method
    //    private RateRequestDto MapToDto(RateRequest rateRequest)
    //    {
    //        return new RateRequestDto
    //        {
    //            Id = rateRequest.Id,
    //            RequestNumber = rateRequest.RequestNumber,
    //            CustomerId = rateRequest.CustomerId,
    //            CustomerName = rateRequest.Customer?.Name,
    //            RequestDate = rateRequest.RequestDate,
    //            ValidityDate = rateRequest.ValidityDate,
    //            OriginId = rateRequest.OriginId,
    //            OriginName = rateRequest.Origin?.Name,
    //            DestinationId = rateRequest.DestinationId,
    //            DestinationName = rateRequest.Destination?.Name,
    //            ShipmentType = rateRequest.ShipmentType,
    //            CargoType = rateRequest.CargoType,
    //            CargoTypeId = rateRequest.CargoTypeId,
    //            CargoTypeName = rateRequest.CargoTypeEntity?.Name,
    //            Incoterm = rateRequest.Incoterm,
    //            TransportMode = rateRequest.TransportMode,
    //            ServiceType = rateRequest.ServiceType,
    //            Packages = rateRequest.Packages?.Select(p => new PackageDto
    //            {
    //                Id = p.Id,
    //                Quantity = p.Quantity,
    //                PackageType = p.PackageType,
    //                Length = p.Length,
    //                Width = p.Width,
    //                Height = p.Height,
    //                Weight = p.Weight,
    //                Volume = p.Volume
    //            }).ToList(),
    //            FCLDetails = rateRequest.FCLDetails != null ? new FCLDetailsDto
    //            {
    //                Id = rateRequest.FCLDetails.Id,
    //                ContainerType = rateRequest.FCLDetails.ContainerType,
    //                ContainerCount = rateRequest.FCLDetails.ContainerCount,
    //                TotalWeight = rateRequest.FCLDetails.TotalWeight
    //            } : null,
    //            IsTemperatureControlled = rateRequest.IsTemperatureControlled,
    //            TemperatureRange = rateRequest.TemperatureRange,
    //            IsHazardous = rateRequest.IsHazardous,
    //            UNNumber = rateRequest.UNNumber,
    //            HazardClass = rateRequest.HazardClass,
    //            RequiresInsurance = rateRequest.RequiresInsurance,
    //            InsuranceValue = rateRequest.InsuranceValue,
    //            RequiresCustomsClearance = rateRequest.RequiresCustomsClearance,
    //            AdditionalServices = rateRequest.AdditionalServices,
    //            SpecialInstructions = rateRequest.SpecialInstructions,
    //            Status = rateRequest.Status,
    //            CreatedBy = rateRequest.CreatedBy,
    //            CreatedAt = rateRequest.CreatedAt,
    //            UpdatedBy = rateRequest.UpdatedBy,
    //            UpdatedAt = rateRequest.UpdatedAt
    //        };
    //    }
    //}
}
