using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using ShipRate.Application.DTOs;
using ShipRate.Application.Interfaces;
using ShipRate.Application.Interfaces.Jobs;
using ShipRate.Application.Interfaces.SalesSpecial;
using ShipRate.Application.JobsViewModels;
using ShipRate.Application.Utilities;
using ShipRate.Application.ViewModels;
using ShipRate.Application.ViewModels.Dashboard;
using ShipRate.Common.Utilities;
using ShipRate.Domain.Entities.BillOfLading;
using ShipRate.Domain.Entities.BookingOperation;
using ShipRate.Domain.Entities.Finance;
using ShipRate.Domain.Entities.Operations;
using ShipRate.Domain.Entities.RateQuotation;
using ShipRate.Infrastructure.Data;
using System.Linq.Dynamic.Core;

namespace ShipRate.Application.Services.JobsServices
{
    public class JobService : IJobService
    {
        private readonly ApplicationDbContext _db;
        private readonly IJobLookupService _lookupService;
        private readonly ICurrentUserService _currentUserService;
        private readonly IActivityLogService _activityLogService;
        private readonly ISalesSpecialService _salesService;

        public JobService(
            ApplicationDbContext db,
            IJobLookupService lookupService,
            ICurrentUserService currentUserService,
            IActivityLogService activityLogService,
            ISalesSpecialService salesService)
        {
            _db = db;
            _lookupService = lookupService;
            _currentUserService = currentUserService;
            _activityLogService = activityLogService;
            _salesService = salesService;
        }

        public async Task<PagedResultDto<JobListItemVm>> SearchAsync(JobSearchVm search)
        {
            search ??= new JobSearchVm();

            if (search.Page <= 0)
                search.Page = 1;

            if (search.PageSize <= 0)
                search.PageSize = 10;

            var routingQuery =
                from jr in _db.JobRoutings
                join origin in _db.Locations on jr.OriginLocationId equals origin.Id
                join dest in _db.Locations on jr.DestinationLocationId equals dest.Id
                join mt in _db.MovementTypes on jr.MovementTypeId equals mt.Id
                where !jr.IsDeleted
                      && origin.IsActive && !origin.IsDeleted
                      && dest.IsActive && !dest.IsDeleted
                      && mt.IsActive && !mt.IsDeleted
                select new
                {
                    jr.JobId,
                    Origin = origin.LocationName,
                    Destination = dest.LocationName,
                    MovementType = mt.MovementTypeCode,
                    OriginId = origin.Id,
                    DestinationId = dest.Id
                };

            var query =
                from j in _db.Jobs
                join c in _db.Companies on j.CompanyId equals c.Id
                join cp in _db.Persons on c.PersonId equals cp.Id
                join tt in _db.TransportTypes on j.TransportTypeId equals tt.Id
                join tr in _db.TradeTypes on j.TradeTypeId equals tr.Id
                join lt in _db.LoadTypes on j.LoadTypeId equals lt.Id
                join js in _db.JobStatuses on j.JobStatusId equals js.Id
                join routing in routingQuery on j.Id equals routing.JobId into routingLeft
                from routing in routingLeft.DefaultIfEmpty()
                select new JobListItemVm
                {
                    Id = j.Id,
                    JobNumber = j.JobNumber,
                    JobFileNumber = j.JobFileNumber,
                    CompanyName = cp.PersonName,
                    SalesUserId = j.SalesUserId,
                    CustomerName = (
                        from jp in _db.JobParties
                        join p in _db.Persons on jp.PartyId equals p.Id
                        where jp.JobId == j.Id
                              && !jp.IsDeleted
                              && !p.IsDeleted
                              && jp.RoleTypeId == 2
                        select p.PersonName
                    ).FirstOrDefault() ?? "",
                    TransportType = tt.TransportTypeName,
                    TradeType = tr.TradeTypeName,
                    LoadType = lt.LoadTypeCode,
                    JobStatus = js.JobStatusName,
                    Origin = routing != null ? routing.Origin : "",
                    Destination = routing != null ? routing.Destination : "",
                    MovementType = routing != null ? routing.MovementType : "",
                    JobDate = j.JobDate,
                    CargoReadyDate = j.CargoReadyDate,
                    OriginId = routing.OriginId,
                    DestinationId = routing.DestinationId,
                    TradeTypeId = j.TradeTypeId,
                    SalesUserName = j.SalesPerson.FullName
                };

            //if (!string.IsNullOrWhiteSpace(search.JobNumber))
            //{
            //    var jobNumber = search.JobNumber.Trim();
            //    query = query.Where(x => x.JobNumber.Contains(jobNumber) || x.JobFileNumber.Contains(jobNumber
            //        ));
            //}


            if (!string.IsNullOrWhiteSpace(search.JobNumber))
            {
                var searchTerm = search.JobNumber.Trim();

                query = query.Where(x =>
                    x.JobNumber.Contains(searchTerm) ||
                    x.JobFileNumber.Contains(searchTerm) ||

                    _db.Bookings.Any(booking =>
                        booking.JobId == x.Id &&
                        booking.BookingNo.Contains(searchTerm)) ||

                    _db.BLs.Any(bl =>
                        bl.JobId == x.Id &&
                        bl.BLNo.Contains(searchTerm)));
            }

            var userId = _currentUserService.UserId;
            if (_currentUserService.IsSalesUser() && !_currentUserService.IsAdmin() && !_currentUserService.IsSalesManager() && !_currentUserService.IsManager())
            {
                query = query.Where(x => x.SalesUserId == userId);
            }
            if (search.OriginId != null && search.OriginId.HasValue && search.OriginId != Guid.Empty)
            {
                query = query.Where(x => x.OriginId == search.OriginId);
            }

            if (search.DestinationId != null && search.DestinationId.HasValue && search.DestinationId != Guid.Empty)
            {
                query = query.Where(x => x.DestinationId == search.DestinationId);
            }

            if (search.CompanyId.HasValue)
            {
                var companyId = search.CompanyId.Value;
                query = query.Where(x =>
                    _db.Jobs.Any(j => j.Id == x.Id && j.CompanyId == companyId));
            }
            if (search.SalesUserId.HasValue)
            {
                query = query.Where(x => x.SalesUserId == search.SalesUserId);
            }
            if (search.TradeTypeId.HasValue)
            {
                query = query.Where(x => x.TradeTypeId == search.TradeTypeId);
            }

            if (search.CustomerId.HasValue)
            {
                var customerId = search.CustomerId.Value;

                query = query.Where(x =>
                    _db.JobParties.Any(jp =>
                        jp.JobId == x.Id &&
                        jp.PartyId == customerId &&
                        !jp.IsDeleted));
            }

            if (search.TransportTypeId.HasValue)
            {
                var transportTypeId = search.TransportTypeId.Value;

                query = query.Where(x =>
                    _db.Jobs.Any(j => j.Id == x.Id && j.TransportTypeId == transportTypeId));
            }

            if (search.JobStatusId.HasValue)
            {
                var jobStatusId = search.JobStatusId.Value;

                query = query.Where(x =>
                    _db.Jobs.Any(j => j.Id == x.Id && j.JobStatusId == jobStatusId));
            }

            var totalCount = await query.CountAsync();

            var items = await query
                .OrderByDescending(x => x.JobNumber)
                .Skip((search.Page - 1) * search.PageSize)
                .Take(search.PageSize)
                .ToListAsync();

            return new PagedResultDto<JobListItemVm>
            {
                Items = items,
                TotalCount = totalCount
            }
            ;
        }

        public async Task<Guid> GetBookingJobIdAsync(Guid bookingId)
        {
            var booking = await _db.Bookings.Where(x => x.Id == bookingId).FirstOrDefaultAsync();
            return booking.JobId;
        }

        public string GenerateNewJobNumberAsync()
        {
            string result = DateHelper.getSolarDate8(DateTime.Now);
            int maxVal = 0;
            var max = _db.Jobs.Max(x => x.JobNumber);
            if (max == null) max = "0";
            if (max.StartsWith(result)) maxVal = Convert.ToInt32(max.Substring(8)) + 1;
            else maxVal = 1;
            result = result + maxVal.ToString().PadLeft(3, '0');
            return result;
        }
        public async Task<string> GenerateNewJobFileNumberAsync(Guid userId)
        {
            var p1 = await _salesService.GetSalesPrefixAsync(userId);
            var p2 = await _salesService.GetSalesUserNewFileNoAsync(userId);
            return p1 + p2.ToString();
        }
        public async Task<CreateJobVm> BuildCreateVmAsync()
        {
            var vm = new CreateJobVm
            {
                Companies = await _lookupService.GetCompaniesAsync(),
                TransportTypes = await _lookupService.GetTransportTypesAsync(),
                TradeTypes = await _lookupService.GetTradeTypesAsync(),
                LoadTypes = await _lookupService.GetLoadTypesAsync(),
                CargoHandlingTypes = await _lookupService.GetCargoHandlingTypesAsync(),
                CargoNatures = await _lookupService.GetCargoNaturesAsync(),
                Incoterms = await _lookupService.GetIncotermsAsync(),
                PaymentTypes = await _lookupService.GetPaymentTypesAsync(),
                JobTypes = await _lookupService.GetJobTypesAsync(),
                JobStatuses = await _lookupService.GetJobStatusesAsync(),
                MovementTypes = await _lookupService.GetMovementTypesAsync(),
                JobSourceTypes = await _lookupService.GetJobSourceTypesAsync(),
                SalesUsers = await _salesService.GetSalesUsersAsync(),
            };
            var userId = _currentUserService.UserId;
            if (_currentUserService.IsSalesUser())
            {
                vm.SalesUserId = userId;
                vm.JobFileNumber = (await _salesService.GetSalesPrefixAsync(userId)) + (await _salesService.GetSalesUserNewFileNoAsync(userId)).ToString();
            }
            return vm;
        }
        public async Task<Guid> CreateAsync(CreateJobVm vm)
        {
            var currentUserId = _currentUserService.UserId;
            var now = DateTime.Now;
            var jobId = Guid.CreateVersion7();

            var job = new Domain.Entities.Operations.Job
            {
                Id = jobId,
                JobSourceTypeId = vm.JobSourceTypeId,
                CompanyId = vm.CompanyId,
                JobNumber = GenerateNewJobNumberAsync(),
                //JobNumber = vm.JobNumber.Trim(),
                JobFileNumber = vm.JobFileNumber.Trim(),
                TransportTypeId = vm.TransportTypeId,
                TradeTypeId = vm.TradeTypeId,
                LoadTypeId = vm.LoadTypeId,
                CargoHandlingTypeId = vm.CargoHandlingTypeId,
                CargoNatureId = vm.CargoNatureId,
                IncotermId = vm.IncotermId,
                IncotermsLocationName = vm.IncotermsLocationName ?? "",
                FreightPaymentTypeId = vm.FreightPaymentTypeId,
                SalesUserId = vm.SalesUserId,
                //JobTypeId = vm.JobTypeId,
                JobStatusId = 1,
                JobDate = vm.JobDate,
                CargoReadyDate = vm.CargoReadyDate,

                //IsStackable = vm.IsStackable,
                //IsRotateable = vm.IsRotateable,
                //IsConsolMaster = vm.IsConsolMaster,
                CreatedAt = now,
                CreateUserId = currentUserId,
                //IsActive = true,
                IsDeleted = false
            };

            var routing = new Domain.Entities.Operations.JobRouting
            {
                Id = Guid.CreateVersion7(),
                JobId = jobId,
                OriginLocationId = vm.OriginLocationId,
                DestinationLocationId = vm.DestinationLocationId,
                MovementTypeId = vm.MovementTypeId,
                ETD = vm.ETD,
                ETA = vm.ETA,
                CreatedAt = now,
                CreateUserId = currentUserId,
                //IsActive = true,
                IsDeleted = false
            };

            //var party = new Domain.Entities.Operations.JobParty
            //{
            //    Id = Guid.CreateVersion7(),
            //    JobId = jobId,
            //    SequenceNo = 1,
            //    PartyId = vm.CustomerId,
            //    RoleTypeId = 2,
            //    IsPrimaryRole = true,
            //    CreatedAt = now,
            //    CreateUserId = currentUserId,
            //    IsActive = true,
            //    IsDeleted = false
            //};

            _db.Jobs.Add(job);
            _db.JobRoutings.Add(routing);

            int sequenceNo = 0;
            //foreach (var item in vm.JobParties)
            //{
            //    sequenceNo += 1;
            //    var party = new JobParty
            //    {
            //        Id = Guid.CreateVersion7(),
            //        JobId = jobId,
            //        SequenceNo = sequenceNo,
            //        PartyId = item.PartyId,
            //        RoleTypeId = item.RoleTypeId,
            //        IsPrimaryRole = item.IsPrimaryRole,
            //        ContactName = item.ContactName,
            //        ContactEmail = item.ContactEmail,
            //        ContactPhone = item.ContactPhone,
            //        CreatedAt = now,
            //        CreateUserId = currentUserId,
            //        IsActive = true,
            //        IsDeleted = false
            //    };
            //    _db.JobParties.Add(party);
            //}

            if (vm.CustomerId != null && vm.CustomerId != Guid.Empty)
            {
                sequenceNo += 1;
                var party = new JobParty
                {
                    Id = Guid.CreateVersion7(),
                    JobId = jobId,
                    SequenceNo = sequenceNo,
                    PartyId = vm.CustomerId.Value,
                    RoleTypeId = 2,
                    IsPrimaryRole = true,
                    CreatedAt = now,
                    CreateUserId = currentUserId,
                    //IsActive = true,
                    IsDeleted = false
                };
                _db.JobParties.Add(party);
            }

            if (vm.BookerClientId != null && vm.BookerClientId != Guid.Empty)
            {
                sequenceNo += 1;
                var party = new JobParty
                {
                    Id = Guid.CreateVersion7(),
                    JobId = jobId,
                    SequenceNo = sequenceNo,
                    PartyId = vm.BookerClientId.Value,
                    RoleTypeId = 31,
                    IsPrimaryRole = true,
                    CreatedAt = now,
                    CreateUserId = currentUserId,
                    //IsActive = true,
                    IsDeleted = false
                };
                _db.JobParties.Add(party);
            }


            foreach (var item in vm.JobCompanyRoles)
            {
                var crole = new JobCompanyRole
                {
                    JobId = jobId,
                    CompanyRoleTypeId = item.CompanyRoleTypeId,
                    ServiceTypeId = item.ServiceTypeId,
                    CompanyId = item.CompanyId,
                    CreateUserId = currentUserId,
                    CreatedAt = DateTime.Now

                };
                _db.JobCompanyRoles.Add(crole);
            }

            foreach (var item in vm.Containers)
            {
                var cnt = new RateRequestContainer
                {
                    JobId = jobId,
                    ContainerSizeTypeId = item.ContainerSizeTypeId,
                    Counts = item.Counts.Value,
                    Remarks = item.Remarks,
                    CntOwnTypeId = item.cntOwnTypeId,
                    CreateUserId = currentUserId,
                    CreatedAt = DateTime.Now,
                };
                _db.RateRequestContainers.Add(cnt);
            }

            //foreach (var item in vm.Packages)
            //{
            //    var package = new RateRequestPackage
            //    {
            //        JobId = jobId,
            //        PackageTypeId = item.PackageTypeId,
            //        UnitCount = item.UnitCounts.Value,
            //        UnitLength = item.UnitLength,
            //        UnitWidth = item.UnitWidth,
            //        UnitHeight = item.UnitHeight,
            //        UnitGrossWeight = item.UnitGrossWeight,
            //        UnitNetWeight = item.UnitNetWeight,
            //        UnitVolume = item.UnitVolume,
            //        TotalGrossWeight = item.TotalGrossWeight,
            //        TotalNetWeight = item.TotalNetWeight,
            //        TotalVolume = item.TotalVolume,
            //        TotalHeight = item.TotalHeight,
            //        TotalWidth = item.TotalWidth,
            //        TotalLength = item.TotalLength,
            //        LengthMeasurementUnitId = item.DimensionMeasurementUnitId,
            //        VolumeMeasurementUnitId = item.VolumeMeasurementUnitId,
            //        WeightMeasurementUnitId = item.WeightMeasurementUnitId,
            //        Remarks = item.Remarks,
            //        CreateUserId = currentUserId,
            //        CreatedAt = DateTime.Now
            //    };
            //    _db.RateRequestPackages.Add(package);
            //}
            var shipment = new Shipment
            {
                Id = Guid.CreateVersion7(),
                JobId = jobId,
                ShipmentTitle = "SH" + job.JobNumber,
                CreateUserId = _currentUserService.UserId,
                CreatedAt = DateTime.Now,
                ShipmentStatusId = 1
            };
            _db.Shipments.Add(shipment);

            //_db.JobStatusHistories.Add(new JobStatusHistory { JobId = jobId, CreatedAt = DateTime.Now, CreateUserId = currentUserId, OldStatusId = 1, NewStatusId = 1, ReasonRemarks = "Job Created" });
            await _activityLogService.LogHistoryAsync(
                new CreateActivityLogRequest
                {
                    ActivityCategoryId = 2,
                    ActivityDateTime = DateTime.Now,
                    Title = "Job Created ",
                    Description = "Job Created Manually!!!",
                    EntityTypeId = 1,
                    EntityId = jobId,
                    FromStatusEntityType = "JobStatus",
                    ToStatusId = 1,
                    PerformedByUserId = currentUserId,
                    IsSystemGenerated = true,
                }
                );

            await _db.SaveChangesAsync();
            return jobId;
        }
        public async Task<JobStatsViewModel> Stats()
        {
            var jobs = await _db.Jobs.ToListAsync();
            var vm = new JobStatsViewModel
            {
                TotalJobs = jobs.Count,
                Draft = jobs.Count(j => j.JobStatusId == 1),
                Confirmed = jobs.Count(j => j.JobStatusId == 2),
                InProgress = jobs.Count(j => j.JobStatusId == 3),
                Completed = jobs.Count(j => j.JobStatusId == 4),
                Closed = jobs.Count(j => j.JobStatusId == 5),
                Canceled = jobs.Count(j => j.JobStatusId == 6),
                OnHold = jobs.Count(j => j.JobStatusId == 7)
            };
            return vm;
        }


        //public async Task<JobDetailsVm?> GetDetailsAsync(Guid id)
        //{
        //    var job = await _db.Jobs
        //                   .AsNoTracking()
        //                   .Include(x => x.Routing)
        //                   .ThenInclude(x => x.OriginLocation)
        //                   .Include(x => x.Routing)
        //                   .ThenInclude(x => x.DestinationLocation)
        //                   .Include(x => x.Routing)
        //                   .ThenInclude(x => x.MovementType)
        //                   .Include(x => x.Containers)
        //                   .Include(x => x.Packages)
        //                   .Include(x => x.JobStatus)
        //                   .Include(x => x.Incoterm)
        //                   .Include(x => x.TransportType)
        //                   .Include(x => x.CargoHandlingType)
        //                   .Include(x => x.CargoNature)
        //                   .Include(x => x.LoadType)
        //                   .Include(x => x.SalesPerson)
        //                   .Include(x => x.Company)
        //                   .ThenInclude(x => x.Person)
        //                   .Include(x => x.TradeType)
        //                   .Include(x => x.Parties)
        //                   .ThenInclude(x => x.Party)
        //                   .Include(x => x.JobSourceType)
        //                   .Include(x => x.CreatedByUser)
        //                   .Include(x => x.UpdatedByUser)
        //                   .Include(x => x.SellQuote)
        //                   .ThenInclude(x => x.SelectedBuyQuote)
        //                   .ThenInclude(x => x.RateRequestInquiry)
        //                   .ThenInclude(x => x.RateRequest)
        //                   .FirstOrDefaultAsync(x => x.Id == id);

        //    if (job == null) return null;

        //    var firstRoute = job.Routing.FirstOrDefault();
        //    var customer = job.Parties.Where(x => x.RoleTypeId == 2).FirstOrDefault();

        //    var vm = new JobDetailsVm
        //    {
        //        Id = job.Id,
        //        JobNumber = job.JobNumber,
        //        JobDate = job.JobDate,
        //        JobFileNumber = job.JobFileNumber,
        //        SalesUserName = job.SalesPerson?.UserName,
        //        OriginName = firstRoute?.OriginLocation?.LocationName,
        //        DestinationName = firstRoute?.DestinationLocation?.LocationName,
        //        Status = job.JobStatus?.JobStatusName,
        //        StatusId = job.JobStatusId,
        //        CompanyName = job.Company?.Person?.PersonName,
        //        Incoterm = job.Incoterm?.IncotermCode,
        //        CustomerName = customer?.Party?.PersonName,
        //        JobSourceTypeName = job.JobSourceType.JobSourceTypeName,
        //        CreatedByUserName = job.CreatedByUser?.FullName,
        //        UpdatedByUserName = job.UpdatedByUser?.FullName,

        //        TransportTypeName = job.TransportType.TransportTypeName,
        //        LoadTypeName = job.LoadType.LoadTypeCode,
        //        CargoNatureName = job.CargoNature.CargoNatureName,
        //        CargohandlingTypeName = job.CargoHandlingType.CargoHandlingTypeName,
        //        CargoReadyDate = job.CargoReadyDate is null ? null : job.CargoReadyDate.Value,
        //        MovementTypeName = firstRoute.MovementType is null ? null : firstRoute.MovementType.MovementTypeCode,
        //        TradeTypeName = job.TradeType.TradeTypeName,
        //        SellQuoteNo = job.SellQuote is null ? null : job.SellQuote.QuotationNo,
        //        BuyQuoteNo = job.SellQuote is null ? null : job.SellQuote.SelectedBuyQuote is null ? null : job.SellQuote.SelectedBuyQuote.QuotationNo,
        //        InquiryNo = job.SellQuote is null ? null : job.SellQuote.SelectedBuyQuote is null ? null : job.SellQuote.SelectedBuyQuote.RateRequestInquiry is null ? null : job.SellQuote.SelectedBuyQuote.RateRequestInquiry.InquiryNo,
        //        RateRequestNo = job.SellQuote is null ? null : job.SellQuote.SelectedBuyQuote is null ? null : job.SellQuote.SelectedBuyQuote.RateRequestInquiry is null ? null : job.SellQuote.SelectedBuyQuote.RateRequestInquiry.RateRequest is null ? null : job.SellQuote.SelectedBuyQuote.RateRequestInquiry.RateRequest.RequestNumber,
        //    };

        //    var rolesList = await _db.JobCompanyRoles
        //        .Include(x => x.Company)
        //        .ThenInclude(x => x.Person)
        //        .Include(x => x.CompanyRoleType)
        //        .Include(x => x.ServiceType)
        //        .Where(x => x.JobId == id)
        //        .Select(x => new JobCompanyRoleListItemVm { CompanyName = x.Company.Person.PersonName, CompanyRoleTypeName = x.CompanyRoleType.CompanyRoleTypeName, ServiceTypeName = x.ServiceType.ServiceTypeName, ServiceTypeLocalName = x.ServiceType.ServiceTypeLocalName })
        //        .ToListAsync();
        //    vm.JobCompanyRolesList = rolesList;
        //    vm.ShipmentNumber = await GetJobFirstShipmentTitleAsync(id);
        //    vm.BookingNumber = await GetJobFirstBookingAsync(id);
        //    return vm;
        //}


        public async Task<JobDetailsVm?> GetDetailsAsync(Guid id)
        {
            // کوئری مستقیم روی DTO بدون هیچ‌گونه Include اضافی
            var vm = await _db.Jobs
                .AsNoTracking()
                .Where(x => x.Id == id)
                .Select(job => new JobDetailsVm
                {
                    Id = job.Id,
                    JobNumber = job.JobNumber,
                    JobDate = job.JobDate,
                    JobFileNumber = job.JobFileNumber,
                    SalesUserName = job.SalesPerson != null ? job.SalesPerson.UserName : null,

                    // دسترسی مستقیم به اولین رکورد مسیرها در سطح دیتابیس
                    OriginName = job.Routing.Select(r => r.OriginLocation.LocationName).FirstOrDefault(),
                    DestinationName = job.Routing.Select(r => r.DestinationLocation.LocationName).FirstOrDefault(),
                    MovementTypeName = job.Routing.Select(r => r.MovementType.MovementTypeCode).FirstOrDefault(),

                    Status = job.JobStatus != null ? job.JobStatus.JobStatusName : null,
                    StatusId = job.JobStatusId,
                    CompanyName = job.Company != null && job.Company.Person != null ? job.Company.Person.PersonName : null,
                    Incoterm = job.Incoterm != null ? job.Incoterm.IncotermCode : null,

                    // دریافت نام مشتری مستقیم از SQL
                    CustomerName = job.Parties
                        .Where(p => p.RoleTypeId == 2)
                        .Select(p => p.Party.PersonName)
                        .FirstOrDefault(),

                    JobSourceTypeName = job.JobSourceType != null ? job.JobSourceType.JobSourceTypeName : null,
                    CreatedByUserName = job.CreatedByUser != null ? job.CreatedByUser.FullName : null,
                    UpdatedByUserName = job.UpdatedByUser != null ? job.UpdatedByUser.FullName : null,

                    TransportTypeName = job.TransportType != null ? job.TransportType.TransportTypeName : null,
                    LoadTypeName = job.LoadType != null ? job.LoadType.LoadTypeCode : null,
                    CargoNatureName = job.CargoNature != null ? job.CargoNature.CargoNatureName : null,
                    CargohandlingTypeName = job.CargoHandlingType != null ? job.CargoHandlingType.CargoHandlingTypeName : null,
                    CargoReadyDate = job.CargoReadyDate,
                    TradeTypeName = job.TradeType != null ? job.TradeType.TradeTypeName : null,

                    // مدیریت زنجیره Quotes به صورت بهینه در SQL
                    SellQuoteNo = job.SellQuote != null ? job.SellQuote.QuotationNo : null,
                    BuyQuoteNo = job.SellQuote != null && job.SellQuote.SelectedBuyQuote != null
                        ? job.SellQuote.SelectedBuyQuote.QuotationNo
                        : null,
                    InquiryNo = job.SellQuote != null && job.SellQuote.SelectedBuyQuote != null && job.SellQuote.SelectedBuyQuote.RateRequestInquiry != null
                        ? job.SellQuote.SelectedBuyQuote.RateRequestInquiry.InquiryNo
                        : null,
                    RateRequestNo = job.SellQuote != null && job.SellQuote.SelectedBuyQuote != null && job.SellQuote.SelectedBuyQuote.RateRequestInquiry != null && job.SellQuote.SelectedBuyQuote.RateRequestInquiry.RateRequest != null
                        ? job.SellQuote.SelectedBuyQuote.RateRequestInquiry.RateRequest.RequestNumber
                        : null
                })
                .FirstOrDefaultAsync();

            if (vm == null) return null;

            // دریافت نقش‌ها بدون Include (فقط ستون‌های ضروری خوانده می‌شوند)
            vm.JobCompanyRolesList = await _db.JobCompanyRoles
                .AsNoTracking()
                .Where(x => x.JobId == id)
                .Select(x => new JobCompanyRoleListItemVm
                {
                    CompanyName = x.Company.Person.PersonName,
                    CompanyRoleTypeName = x.CompanyRoleType.CompanyRoleTypeName,
                    ServiceTypeName = x.ServiceType.ServiceTypeName,
                    ServiceTypeLocalName = x.ServiceType.ServiceTypeLocalName
                })
                .ToListAsync();

            vm.ShipmentNumber = await GetJobFirstShipmentTitleAsync(id);
            vm.BookingNumber = await GetJobFirstBookingAsync(id);

            return vm;
        }






        private async Task<string?> GetJobFirstBookingAsync(Guid id)
        {
            var booking = await _db.Bookings.Where(x => x.JobId == id).OrderBy(x => x.CreatedAt).FirstOrDefaultAsync();
            if (booking != null) return booking.BookingNo + " L:" + booking.LineBookingNo;
            return "-----";
        }
        public async Task<CreateJobVm> BuildEditVmAsync(Guid id)
        {
            var job = await _db.Jobs
                      .Include(x => x.Parties)
                      .ThenInclude(x => x.Party)
                      .Include(x => x.JobCompanyRoles)
                      .ThenInclude(x => x.Company)
                      .ThenInclude(x => x.Person)
                      .Include(x => x.Containers)
                      .Include(x => x.Packages)
                      .Include(x => x.Routing)
                      .ThenInclude(x => x.OriginLocation)
                      .Include(x => x.Routing)
                      .ThenInclude(x => x.DestinationLocation)
                      .Where(x => x.Id == id)
                      .FirstOrDefaultAsync();
            var vm = new CreateJobVm
            {
                JobId = id,
                JobNumber = job.JobNumber,
                CargoNatureId = job.CargoNatureId,
                CargoReadyDate = job.CargoReadyDate,
                CompanyId = job.CompanyId,
                OriginLocationId = job.Routing.FirstOrDefault().OriginLocationId,
                OriginName = job.Routing.First().OriginLocation.LocationName,
                DestinationLocationId = job.Routing.FirstOrDefault().DestinationLocationId,
                DestinationName = job.Routing.First().DestinationLocation.LocationName,
                ETA = job.Routing.FirstOrDefault().ETA,
                ETD = job.Routing.FirstOrDefault().ETD,
                CargoHandlingTypeId = job.CargoHandlingTypeId,
                FreightPaymentTypeId = job.FreightPaymentTypeId,
                IncotermId = job.IncotermId,
                IncotermsLocationName = job.IncotermsLocationName,
                JobDate = job.JobDate,
                JobFileNumber = job.JobFileNumber,
                JobSourceTypeId = job.JobSourceTypeId,
                JobStatusId = job.JobStatusId,
                LoadTypeId = job.LoadTypeId,
                TradeTypeId = job.TradeTypeId,
                MovementTypeId = job.Routing.FirstOrDefault().MovementTypeId,
                SalesUserId = job.SalesUserId,
                TransportTypeId = job.TransportTypeId,
                Companies = await _lookupService.GetCompaniesAsync(),
                TransportTypes = await _lookupService.GetTransportTypesAsync(),
                TradeTypes = await _lookupService.GetTradeTypesAsync(),
                LoadTypes = await _lookupService.GetLoadTypesAsync(),
                CargoHandlingTypes = await _lookupService.GetCargoHandlingTypesAsync(),
                CargoNatures = await _lookupService.GetCargoNaturesAsync(),
                Incoterms = await _lookupService.GetIncotermsAsync(),
                PaymentTypes = await _lookupService.GetPaymentTypesAsync(),
                JobTypes = await _lookupService.GetJobTypesAsync(),
                JobStatuses = await _lookupService.GetJobStatusesAsync(),
                MovementTypes = await _lookupService.GetMovementTypesAsync(),
                JobSourceTypes = await _lookupService.GetJobSourceTypesAsync(),
                SalesUsers = await _lookupService.GetSalesUsersAsync(),
                Containers = job.Containers.Select(x => new JobContainerCreateVm { ContainerSizeTypeId = x.ContainerSizeTypeId, Counts = (int)x.Counts, Remarks = x.Remarks, cntOwnTypeId = x.CntOwnTypeId }).ToList(),
                JobCompanyRoles = job.JobCompanyRoles.Select(x => new JobCompanyRoleCreateVm { CompanyId = x.CompanyId, CompanyRoleTypeId = x.CompanyRoleTypeId, ServiceTypeId = x.ServiceTypeId.Value }).ToList()
            };

            if (vm.Containers.Any())
            {
                foreach (var container in vm.Containers)
                {
                    container.SizeTypes = await _lookupService.GetContainerSizeTypesAsync();
                    container.CntOwnTypes = await _lookupService.GetCntOwnTypesAsync();
                }
            }
            if (vm.Packages.Any())
            {
                foreach (var p in vm.Packages)
                {
                    p.PackageTypes = await _lookupService.GetPackageTypesAsync();
                    p.DimensionMeasurementUnits = await _lookupService.GetDimUnitsAsync();
                    p.WeightMeasurementUnits = await _lookupService.GetWeightUnitsAsync();
                    p.VolumeMeasurementUnits = await _lookupService.GetVolumeUnitsAsync();
                }
            }
            if (vm.JobCompanyRoles.Any())
            {
                foreach (var c in vm.JobCompanyRoles)
                {
                    c.CompanyRoleTypes = await _lookupService.GetCompanyRoleTypesAsync();
                    c.Companies = await _lookupService.GetCompaniesAsync();
                    c.ServiceTypes = await _lookupService.GetServiceTypesAsync();
                }
            }
            var customer = job.Parties.Where(x => x.JobId == id && x.RoleTypeId == 2).FirstOrDefault();
            if (customer != null)
            {
                vm.CustomerId = customer.PartyId;
                vm.CustomerName = customer.Party.PersonName;
            }
            var bookerClient = job.Parties.Where(x => x.JobId == id && x.RoleTypeId == 31).FirstOrDefault();
            if (customer != null)
            {
                vm.BookerClientId = customer.PartyId;
                vm.BookerClientName = customer.Party.PersonName;
            }
            return vm;
        }
        public async Task<bool> UpdateAsync(CreateJobVm vm)
        {
            var currentUserId = _currentUserService.UserId;
            var now = DateTime.Now;
            var jobId = vm.JobId.Value;

            var job = await _db.Jobs.Where(x => x.Id == jobId).FirstOrDefaultAsync();
            if (job == null) { return false; }

            job.JobSourceTypeId = vm.JobSourceTypeId;
            job.CompanyId = vm.CompanyId;
            job.JobNumber = vm.JobNumber;
            job.JobFileNumber = vm.JobFileNumber.Trim();
            job.TransportTypeId = vm.TransportTypeId;
            job.TradeTypeId = vm.TradeTypeId;
            job.LoadTypeId = vm.LoadTypeId;
            job.CargoHandlingTypeId = vm.CargoHandlingTypeId;
            job.CargoNatureId = vm.CargoNatureId;
            job.IncotermId = vm.IncotermId;
            job.IncotermsLocationName = vm.IncotermsLocationName ?? "";
            job.FreightPaymentTypeId = vm.FreightPaymentTypeId;
            job.SalesUserId = vm.SalesUserId;
            job.UpdatedAt = now;
            job.EditUserId = currentUserId;
            job.CargoReadyDate = vm.CargoReadyDate;


            var containers = _db.RateRequestContainers.Where(x => x.JobId == jobId).ToList();
            foreach (var container in containers)
            {
                container.IsDeleted = true;
                container.DeleteUserId = currentUserId;
                container.DeletedAt = now;
            }

            foreach (var container in vm.Containers)
            {
                var cnt = new RateRequestContainer
                {
                    JobId = jobId,
                    ContainerSizeTypeId = container.ContainerSizeTypeId,
                    Counts = container.Counts.Value,
                    Remarks = container.Remarks,
                    CreateUserId = currentUserId,
                    CreatedAt = now,
                    CntOwnTypeId = container.cntOwnTypeId
                };
                _db.RateRequestContainers.Add(cnt);
            }
            var jobcompanyroles = _db.JobCompanyRoles.Where(x => x.JobId == jobId).ToList();
            foreach (var j in jobcompanyroles)
            {
                j.IsDeleted = true;
                j.DeleteUserId = currentUserId;
                j.DeletedAt = now;
            }

            foreach (var jr in vm.JobCompanyRoles)
            {
                var jrole = new JobCompanyRole
                {
                    JobId = jobId,
                    CompanyId = jr.CompanyId,
                    CompanyRoleTypeId = jr.CompanyRoleTypeId,
                    ServiceTypeId = jr.ServiceTypeId,
                    CreateUserId = currentUserId,
                    CreatedAt = now,
                };
                _db.JobCompanyRoles.Add(jrole);
            }

            var routings = _db.JobRoutings.Where(x => x.JobId == jobId).ToList();
            foreach (var j in routings)
            {
                j.IsDeleted = true;
                j.DeleteUserId = currentUserId;
                j.DeletedAt = now;
            }

            var routing = new Domain.Entities.Operations.JobRouting
            {
                Id = Guid.CreateVersion7(),
                JobId = jobId,
                OriginLocationId = vm.OriginLocationId,
                DestinationLocationId = vm.DestinationLocationId,
                MovementTypeId = vm.MovementTypeId,
                ETD = vm.ETD,
                ETA = vm.ETA,
                CreatedAt = now,
                CreateUserId = currentUserId,
                //IsActive = true,
                IsDeleted = false
            };
            _db.JobRoutings.Add(routing);

            if (job.Parties.Where(x => x.RoleTypeId == 2).FirstOrDefault() == null)
            {
                if (vm.CustomerId != null && vm.CustomerId != Guid.Empty)
                {
                    var party = new JobParty
                    {
                        Id = Guid.CreateVersion7(),
                        JobId = jobId,
                        SequenceNo = 1,
                        PartyId = vm.CustomerId.Value,
                        RoleTypeId = 2,
                        IsPrimaryRole = true,
                        CreatedAt = now,
                        CreateUserId = currentUserId,
                        //IsActive = true,
                        IsDeleted = false
                    };
                    _db.JobParties.Add(party);
                }
            }
            if (job.Parties.Where(x => x.RoleTypeId == 31).FirstOrDefault() == null)
            {
                if (vm.BookerClientId != null && vm.BookerClientId != Guid.Empty)
                {
                    var party = new JobParty
                    {
                        Id = Guid.CreateVersion7(),
                        JobId = jobId,
                        SequenceNo = 2,
                        PartyId = vm.CustomerId.Value,
                        RoleTypeId = 31,
                        IsPrimaryRole = true,
                        CreatedAt = now,
                        CreateUserId = currentUserId,
                        //IsActive = true,
                        IsDeleted = false
                    };
                    _db.JobParties.Add(party);
                }
            }












            await _db.SaveChangesAsync();
            await _activityLogService.LogHistoryAsync(
                new CreateActivityLogRequest
                {
                    ActivityCategoryId = 2,
                    ActivityDateTime = DateTime.Now,
                    Title = "Job Updated ",
                    Description = "Edited!!!",
                    EntityTypeId = 1,
                    EntityId = jobId,
                    PerformedByUserId = currentUserId,
                    IsSystemGenerated = true,
                }
                );
            return true;
        }
        public async Task<BookingTabVm> GetBookingTabAsync(Guid id)
        {
            var vm = new BookingTabVm { JobId = id };
            var bookingList = await _db.Bookings
                .Include(x => x.BookingStatus)
                .Include(x => x.Company)
                .ThenInclude(x => x.Person)
                .Include(x => x.CompanyRoleType)
                .Include(x => x.TransportType)
                .Include(x => x.Carrier)
                .ThenInclude(x => x.Person)
                .Include(x => x.VendorParty)
                .Where(x => x.JobId == id)
                .OrderByDescending(x => x.Id)
                .Select(x => new BookingListItemVm
                {
                    Id = x.Id,
                    BookingNo = x.BookingNo,
                    LineBookingNo = x.BookingNo,
                    BookingDate = x.CreatedAt,
                    CompanyName = x.Company.Person.PersonName,
                    JobId = x.JobId,
                    CompanyRoleTypeName = x.CompanyRoleType.CompanyRoleTypeName,
                    CarrierName = x.Carrier.Person.PersonName,
                    VendorName = x.VendorParty.PersonName,
                    Origin = x.Origin.LocationName,
                    Destination = x.Dest.LocationName,
                    LoadType = x.LoadType.LoadTypeCode,
                    TransportType = x.TransportType.TransportTypeName,
                    BookingStatusId = x.BookingStatusId,
                    BookingStatusName = x.BookingStatus.BookingStatusName
                })
                .ToListAsync();
            vm.Bookings = bookingList;
            return vm;
        }

        public async Task<CreateBookingVm> BuildCreateBookingVmAsync()
        {
            return new CreateBookingVm
            {
                Companies = await _lookupService.GetCompaniesAsync(),
                //Customers = await _lookupService.GetCustomersAsync(),
                TransportTypes = await _lookupService.GetTransportTypesAsync(),
                TradeTypes = await _lookupService.GetTradeTypesAsync(),
                LoadTypes = await _lookupService.GetLoadTypesAsync(),
                CargoHandlingTypes = await _lookupService.GetCargoHandlingTypesAsync(),
                CargoNatures = await _lookupService.GetCargoNaturesAsync(),
                //Incoterms = await _lookupService.GetIncotermsAsync(),
                PaymentTypes = await _lookupService.GetPaymentTypesAsync(),
                MovementTypes = await _lookupService.GetMovementTypesAsync(),
                ServiceTypes = await _lookupService.GetServiceTypesAsync(),
                Carriers = await _lookupService.GetCarriersAsync(null),
                CompanyRoleTypes = await _lookupService.GetCompanyRoleTypesAsync(),
            };
        }

        public async Task CreateBookingAsync(CreateBookingVm vm)
        {
            var currentUserId = _currentUserService.UserId;
            var now = DateTime.Now;
            var bookingId = Guid.CreateVersion7();

            var booking = new Booking
            {
                JobId = vm.JobId,
                Id = bookingId,
                CompanyId = vm.CompanyId,
                TransportTypeId = vm.TransportTypeId,
                LoadTypeId = vm.LoadTypeId,

                BookingAgentPartyId = vm.BookingAgentId,
                BookingNo = vm.BookingNo,
                LineBookingNo = vm.LineBookingNo,
                BookingStatusId = 1,
                CargoReadyDate = vm.CargoReadyDate,
                CarrierPartyId = vm.CarrierId,
                CompanyRoleTypeId = vm.CompanyRoleTypeId,
                CutOffDate = vm.CutOffDate,
                DocumentCutOffDate = vm.DocumentCutOffDate,
                OriginId = vm.OriginLocationId,
                DestId = vm.DestinationLocationId,
                POLId = vm.POLId,
                PODId = vm.PODId,
                VendorPartyId = vm.VendorId,
                SICutOffDate = vm.SICutOffDate,
                ServiceTypeId = vm.ServiceTypeId,
                Remarks = vm.Remarks,
                VGMCutOffDate = vm.VGMCutOffDate,
                CreatedAt = now,
                CreateUserId = currentUserId,
                IsDeleted = false
            };

            _db.Bookings.Add(booking);

            //int sequenceNo = 0;
            //foreach (var item in vm.JobParties)
            //{
            //    sequenceNo += 1;
            //    var party = new JobParty
            //    {
            //        Id = Guid.CreateVersion7(),
            //        JobId = jobId,
            //        SequenceNo = sequenceNo,
            //        PartyId = item.PartyId,
            //        RoleTypeId = item.RoleTypeId,
            //        IsPrimaryRole = item.IsPrimaryRole,
            //        ContactName = item.ContactName,
            //        ContactEmail = item.ContactEmail,
            //        ContactPhone = item.ContactPhone,
            //        CreatedAt = now,
            //        CreateUserId = currentUserId,
            //        IsActive = true,
            //        IsDeleted = false
            //    };
            //    _db.JobParties.Add(party);
            //}

            foreach (var item in vm.Containers)
            {
                var cnt = new BookingContainerType
                {
                    ContainerSizeTypeId = item.ContainerSizeTypeId,
                    ContainerCount = item.Counts.Value,
                    CntOwnTypeId = item.cntOwnTypeId.Value,
                    BookingId = bookingId,
                    CreateUserId = currentUserId,
                    CreatedAt = DateTime.Now
                };
                _db.BookingContainerTypes.Add(cnt);
            }

            //foreach (var item in vm.Packages)
            //{
            //    var package = new RateRequestPackage
            //    {
            //        JobId = jobId,
            //        PackageTypeId = item.PackageTypeId,
            //        UnitCount = item.UnitCounts.Value,
            //        UnitLength = item.UnitLength,
            //        UnitWidth = item.UnitWidth,
            //        UnitHeight = item.UnitHeight,
            //        UnitGrossWeight = item.UnitGrossWeight,
            //        UnitNetWeight = item.UnitNetWeight,
            //        UnitVolume = item.UnitVolume,
            //        TotalGrossWeight = item.TotalGrossWeight,
            //        TotalNetWeight = item.TotalNetWeight,
            //        TotalVolume = item.TotalVolume,
            //        TotalHeight = item.TotalHeight,
            //        TotalWidth = item.TotalWidth,
            //        TotalLength = item.TotalLength,
            //        LengthMeasurementUnitId = item.DimensionMeasurementUnitId,
            //        VolumeMeasurementUnitId = item.VolumeMeasurementUnitId,
            //        WeightMeasurementUnitId = item.WeightMeasurementUnitId,
            //        Remarks = item.Remarks,
            //        CreateUserId = currentUserId,
            //        CreatedAt = DateTime.Now
            //    };
            //    _db.RateRequestPackages.Add(package);
            //}
            _db.BookingLegs.Add(new BookingLeg { BookingId = bookingId, ShipmentLegId = vm.ShipmentLegId.Value, CreateUserId = currentUserId, CreatedAt = DateTime.Now });
            //_db.JobStatusHistories.Add(new JobStatusHistory { JobId = vm.JobId, CreatedAt = DateTime.Now, CreateUserId = currentUserId, OldStatusId = 1, NewStatusId = 1, ReasonRemarks = "Booking Created" });
            await _db.SaveChangesAsync();
        }

        public async Task UpdateBookingAsync(CreateBookingVm vm)
        {
            throw new NotImplementedException();
        }

        public async Task<List<BLListItemVm>> GetBLTabAsync(Guid id)
        {
            var blList = await _db.BLs
               .Include(x => x.Company)
               .ThenInclude(x => x.Person)
               .Where(x => x.JobId == id)
               .Select(x => new BLListItemVm { BookingNo = x.BLNo, BookingDate = x.CreatedAt, CompanyName = x.Company.Person.PersonPersianName })
               .ToListAsync();
            return blList;
        }

        public async Task<CreateBLVm> BuildCreateBLVmAsync()
        {
            return new CreateBLVm
            {
                Companies = await _lookupService.GetCompaniesAsync(),
                Customers = await _lookupService.GetCustomersAsync(),
                Locations = await _lookupService.GetLocationsAsync(),
                TransportTypes = await _lookupService.GetTransportTypesAsync(),
                TradeTypes = await _lookupService.GetTradeTypesAsync(),
                LoadTypes = await _lookupService.GetLoadTypesAsync(),
                CargoHandlingTypes = await _lookupService.GetCargoHandlingTypesAsync(),
                CargoNatures = await _lookupService.GetCargoNaturesAsync(),
                //Incoterms = await _lookupService.GetIncotermsAsync(),
                PaymentTypes = await _lookupService.GetPaymentTypesAsync(),
                MovementTypes = await _lookupService.GetMovementTypesAsync()
            };
        }

        public async Task<CreateBLVm> BuildEditBLVmAsync(Guid id)
        {
            throw new NotImplementedException();
        }

        public async Task CreateBLAsync(CreateBLVm vm)
        {
            throw new NotImplementedException();
        }

        public async Task UpdateBLAsync(CreateBLVm vm)
        {
            throw new NotImplementedException();
        }

        public async Task<JobChargesVm> GetChargesVmAsync(Guid id)
        {
            var charges = await _db.Charges
                     .Include(x => x.Currency)
                     .Include(x => x.ChargeType)
                     .Include(x => x.Party)
                     .Include(x => x.Company)
                    .Where(x => x.JobId == id && !x.IsDeleted)
                    .Select(x => new
                    {
                        x.Id,
                        x.DirectionId,
                        x.ChargeType.ChargeTypeName,
                        x.ChargeDescription,
                        Qty = x.Quantity,
                        x.UnitRate,
                        x.Amount,
                        Currency = x.Currency.CurrencyCode,
                        Vendor = x.PartyId != null ? x.Party.PersonName : "",
                        PartyName = x.Party.PersonName,
                        x.IsTaxable,
                        x.TaxPercent,
                        x.GrossAmount,
                        x.Remarks,
                        x.CompanyId,
                        CompanyName = x.Company.CompanyAbb
                    })
                    .ToListAsync();

            var vm = new JobChargesVm
            {
                JobId = id,
                SellCharges = charges.Where(x => x.DirectionId == 1)
                                     .Select(x => new ChargeItemVm
                                     {
                                         Id = x.Id,
                                         Charge = x.ChargeTypeName,
                                         Currency = x.Currency,
                                         Qty = x.Qty,
                                         Amount = x.UnitRate,
                                         CompanyId = x.CompanyId,
                                         Remarks = x.Remarks,
                                         CompanyName = x.CompanyName,
                                         GrossAmount = x.GrossAmount is null ? 0 : x.GrossAmount.Value,
                                         IsTaxable = x.IsTaxable,
                                         PartyName = x.PartyName,
                                         TaxPercent = x.TaxPercent is null ? 0 : x.TaxPercent.Value,
                                         UnitRate = x.UnitRate,
                                         ChargeDscription = x.ChargeDescription
                                     }).ToList(),

                BuyCharges = charges.Where(x => x.DirectionId == 2)
                                     .Select(x => new ChargeItemVm
                                     {
                                         Id = x.Id,
                                         Charge = x.ChargeTypeName,
                                         Currency = x.Currency,
                                         Qty = x.Qty,
                                         Amount = x.UnitRate,
                                         CompanyId = x.CompanyId,
                                         Remarks = x.Remarks,
                                         CompanyName = x.CompanyName,
                                         GrossAmount = x.GrossAmount is null ? 0 : x.GrossAmount.Value,
                                         IsTaxable = x.IsTaxable,
                                         PartyName = x.PartyName,
                                         TaxPercent = x.TaxPercent is null ? 0 : x.TaxPercent.Value,
                                         UnitRate = x.UnitRate,
                                         ChargeDscription = x.ChargeDescription
                                     }).ToList(),
            };
            return vm;
        }
        public async Task CreateChargeAsync(CreateChargeVm vm)
        {
            if (vm.Id == Guid.Empty)
            {
                var charge = new Charge
                {
                    Id = Guid.CreateVersion7(),
                    JobId = vm.JobId,
                    CompanyId = vm.CompanyId,
                    ChargeTypeId = vm.ChargeTypeId,
                    DirectionId = vm.DirectionId,
                    ChargeDescription = vm.ChargeDescription,
                    Quantity = vm.Quantity,
                    UnitRate = vm.UnitRate,
                    Amount = vm.Amount,
                    CurrencyId = vm.CurrencyId,
                    PartyId = vm.PartyId,
                    Remarks = vm.Remarks,
                    IsTaxable = vm.Taxable,
                    TaxPercent = vm.TaxPercent,
                    TaxAmount = vm.TaxAmount,
                    GrossAmount = vm.GrossAmount,
                    //IsActive = true,
                    IsDeleted = false,
                    CreatedAt = DateTime.Now,
                    CreateUserId = _currentUserService.UserId

                };
                await _db.Charges.AddAsync(charge);
                await _db.SaveChangesAsync();
            }
            else
            {
                var charge = await _db.Charges.Where(x => x.Id == vm.Id).FirstOrDefaultAsync();
                charge.IsTaxable = vm.Taxable;
                charge.CompanyId = vm.CompanyId;
                charge.ChargeTypeId = vm.ChargeTypeId;
                charge.DirectionId = vm.DirectionId;
                charge.ChargeDescription = vm.ChargeDescription;
                charge.Quantity = vm.Quantity;
                charge.UnitRate = vm.UnitRate;
                charge.Amount = vm.Amount;
                charge.CurrencyId = vm.CurrencyId;
                charge.PartyId = vm.PartyId;
                charge.Remarks = vm.Remarks;
                charge.IsTaxable = vm.Taxable;
                charge.TaxPercent = vm.TaxPercent;
                charge.TaxAmount = vm.TaxAmount;
                charge.GrossAmount = vm.GrossAmount;
                charge.UpdatedAt = DateTime.Now;
                charge.EditUserId = _currentUserService.UserId;
                await _db.SaveChangesAsync();
            }
        }
        public async Task<bool> DeleteBookingAsync(Guid id)
        {
            var booking = await _db.Bookings.Where(x => x.Id == id).FirstOrDefaultAsync();
            if (booking == null) return false;
            booking.IsDeleted = true;
            booking.DeletedAt = DateTime.Now;
            booking.DeleteUserId = _currentUserService.UserId;
            await _db.SaveChangesAsync();
            return true;
        }

        public async Task<List<RTSInstructionListItemVm>> GetRTSInstructionsByBookingIdAsync(Guid bookingId)
        {
            var rtsList = await _db.RTSInstructions
                .Include(x => x.Carrier)
                .ThenInclude(x => x.Person)
                .Include(x => x.Shipper)
                .Include(x => x.Status)
                .Include(x => x.Depot)
                .Include(x => x.CreateUser)
                .Include(x => x.UpdateUser)
                .Include(x => x.RTSInstructionContainerTypes)
                .ThenInclude(x => x.ContainerSizeType)
                .Where(x => x.BookingId == bookingId)
                .Select(x => new RTSInstructionListItemVm
                {
                    Id = x.Id,
                    JobId = x.JobId,
                    BookingId = bookingId,
                    RTSInstructionNo = x.RTSInstructionNo,
                    RTSInstructionDate = x.RTSInstructionDate,
                    Remarks = x.Remarks,
                    ValidDateFrom = x.ValidDateFrom,
                    ValidDateTo = x.ValidDateTo,
                    ShipperName = x.Shipper.PersonName,
                    CarrierName = x.Carrier.Person.PersonName,
                    DepotName = x.Depot.LocationName,
                    StatusName = x.Status.StatusName,
                    CreateUserName = x.CreateUser.FullName,
                    UpdateUserName = x.UpdateUser == null ? "-" : x.UpdateUser.FullName,
                    CreatedAt = x.CreatedAt,
                    UpdatedAt = x.UpdatedAt,
                    ContainerTypes = x.RTSInstructionContainerTypes.Select(x => new RTSInstructionContainerTypeVm
                    {
                        ContainerCounts = x.ContainerCounts,
                        ContainerSizeTypeName = x.ContainerSizeType.ShortName,
                        ContainerSizeTypeId = x.ContainerSizeTypeId,
                        Remarks = x.Remarks,
                        Id = x.Id,
                        RTSInstructionId = x.RTSInstructionId
                    }).ToList()
                })
                .ToListAsync();
            return rtsList;
        }

        public async Task<List<RTSContainerInputVm>> GetBookingContainerTypesToCreateRTSIAsync(Guid bookingId)
        {
            var list = await _db.BookingContainerTypes
                .Include(x => x.ContainerSizeType)
                .Where(b => b.BookingId == bookingId && !b.IsDeleted)
                .Select(b => new RTSContainerInputVm
                {
                    ContainerSizeTypeId = b.ContainerSizeTypeId,
                    ContainerSizeTypeName = b.ContainerSizeType.ShortName,
                    MaxAvailableCount = b.ContainerCount
                }).ToListAsync();
            return list;
        }
        public async Task<List<RTSContainerInputVm>> GetBookingContainerTypesForEditRTSIAsync(Guid rtsiId, Guid bookingId)
        {
            // ۱. ابتدا rtsitems را به یک Dictionary تبدیل می‌کنیم تا دسترسی به آن فوق‌العاده سریع باشد (O(1))
            var rtsDict = await _db.RTSInstructionContainerTypes
                .Where(x => x.RTSInstructionId == rtsiId)
                .ToDictionaryAsync(x => x.ContainerSizeTypeId);

            // ۲. حالا کوئری اصلی را اجرا می‌کنیم
            var list = await _db.BookingContainerTypes
                .Include(x => x.ContainerSizeType)
                .Where(b => b.BookingId == bookingId && !b.IsDeleted)
                .Select(b => new RTSContainerInputVm
                {
                    ContainerSizeTypeId = b.ContainerSizeTypeId,
                    ContainerSizeTypeName = b.ContainerSizeType != null ? b.ContainerSizeType.ShortName : "N/A",
                    MaxAvailableCount = b.ContainerCount,
                    // چک می‌کنیم اگر در دیکشنری وجود داشت، مقادیر را بردارد، وگرنه null یا 0 برگرداند
                    RequestedCount = rtsDict.ContainsKey(b.ContainerSizeTypeId)
                                     ? rtsDict[b.ContainerSizeTypeId].ContainerCounts
                                     : 0,
                    Remarks = rtsDict.ContainsKey(b.ContainerSizeTypeId)
                              ? rtsDict[b.ContainerSizeTypeId].Remarks
                              : string.Empty
                }).ToListAsync();

            return list;

        }
        public async Task AddRTSInstructionAsync(CreateRTSInstructionVm model)
        {
            // 1. نگاشت (Mapping) ViewModel به Entity

            using var transaction = await _db.Database.BeginTransactionAsync();
            try
            {
                var rts = new RTSInstruction
                {
                    Id = Guid.CreateVersion7(),
                    BookingId = model.BookingId,
                    JobId = model.JobId,
                    CompanyId = model.CompanyId,
                    RTSInstructionNo = model.RTSInstructionNo,
                    RTSInstructionDate = model.RTSInstructionDate ?? DateTime.Now,
                    ShipperId = model.ShipperId.Value,
                    CarrierId = model.CarrierId.Value,
                    DepotId = model.DepotId.Value,
                    StatusId = 1,  //draft
                    ValidDateFrom = model.ValidDateFrom,
                    ValidDateTo = model.ValidDateTo,
                    Remarks = model.Remarks,
                    CancelationReason = "--",

                    CreatedAt = DateTime.Now,
                    CreateUserId = _currentUserService.UserId // یا هر سرویسی که کاربر جاری را می‌دهد
                };
                _db.RTSInstructions.Add(rts);

                foreach (var item in model.Containers.Where(c => c.RequestedCount > 0))
                {
                    _db.RTSInstructionContainerTypes.Add(new RTSInstructionContainerType
                    {
                        Id = Guid.CreateVersion7(),
                        RTSInstructionId = rts.Id,
                        ContainerSizeTypeId = item.ContainerSizeTypeId,
                        ContainerCounts = item.RequestedCount,
                        Remarks = item.Remarks ?? "",
                        //IsActive = true,
                        CreatedAt = DateTime.Now,
                        CreateUserId = _currentUserService.UserId
                    });
                }
                await _db.SaveChangesAsync();
                await transaction.CommitAsync();
            }
            catch (Exception)
            {
                await transaction.RollbackAsync();
                throw;
            }
        }
        public async Task UpdateRTSInstructionAsync(CreateRTSInstructionVm model)
        {
            var vm = await _db.RTSInstructions.Include(x => x.RTSInstructionContainerTypes).Where(x => x.Id == model.Id).FirstOrDefaultAsync();
            using var transaction = await _db.Database.BeginTransactionAsync();
            try
            {
                vm.RTSInstructionNo = model.RTSInstructionNo;
                vm.RTSInstructionDate = model.RTSInstructionDate ?? DateTime.Now;
                vm.ShipperId = model.ShipperId.Value;
                vm.CarrierId = model.CarrierId.Value;
                vm.DepotId = model.DepotId.Value;
                vm.ValidDateFrom = model.ValidDateFrom;
                vm.ValidDateTo = model.ValidDateTo;
                vm.Remarks = model.Remarks;
                vm.CancelationReason = "--";
                vm.UpdatedAt = DateTime.Now;
                vm.EditUserId = _currentUserService.UserId;

                var existsingCnts = await _db.RTSInstructionContainerTypes.Where(x => x.RTSInstructionId == model.Id).ToListAsync();
                foreach (var cnt in existsingCnts)
                {
                    cnt.IsDeleted = true;
                    cnt.DeleteUserId = _currentUserService.UserId;
                    cnt.DeletedAt = DateTime.Now;
                }

                foreach (var item in model.Containers.Where(c => c.RequestedCount > 0))
                {
                    _db.RTSInstructionContainerTypes.Add(new RTSInstructionContainerType
                    {
                        Id = Guid.CreateVersion7(),
                        RTSInstructionId = vm.Id,
                        ContainerSizeTypeId = item.ContainerSizeTypeId,
                        ContainerCounts = item.RequestedCount,
                        Remarks = item.Remarks ?? "",
                        //IsActive = true,
                        CreatedAt = DateTime.Now,
                        CreateUserId = _currentUserService.UserId
                    });
                }
                await _db.SaveChangesAsync();
                await transaction.CommitAsync();
            }
            catch (Exception)
            {
                await transaction.RollbackAsync();
                throw;
            }
        }








        public async Task<RTSInstructionDetailsVm> GetRTSInstructionDetailsAsync(Guid id)
        {
            var vm = await _db.RTSInstructions
                    .Include(x => x.Shipper)
                    .Include(x => x.Carrier)
                    .ThenInclude(x => x.Person)
                    .Include(x => x.Depot)
                    .Include(x => x.Status)
                    .Include(x => x.RTSInstructionContainerTypes)
                    .ThenInclude(x => x.ContainerSizeType)
                    .Where(x => x.Id == id && !x.IsDeleted)
                    .Select(x => new RTSInstructionDetailsVm
                    {
                        Id = x.Id,
                        RTSInstructionNo = x.RTSInstructionNo,
                        RTSInstructionDate = x.RTSInstructionDate,

                        ShipperName = x.Shipper.PersonName,
                        CarrierName = x.Carrier.Person.PersonName,
                        DepotName = x.Depot.LocationName,
                        StatusName = x.Status.StatusName,

                        ValidDateFrom = x.ValidDateFrom,
                        ValidDateTo = x.ValidDateTo,
                        Remarks = x.Remarks,

                        Containers = x.RTSInstructionContainerTypes
                            .Where(c => !c.IsDeleted)
                            .Select(c => new RTSInstructionDetailsContainerVm
                            {
                                ContainerSizeTypeName = c.ContainerSizeType.ShortName,
                                ContainerCounts = c.ContainerCounts,
                                Remarks = c.Remarks
                            }).ToList()
                    })
                    .FirstOrDefaultAsync();
            return vm;
        }

        public async Task<List<PartyLookupItemVm>> SearchPartiesAsync(string? term)
        {
            term = term?.Trim();
            var query = _db.Persons
                .Include(x => x.PersonContacts)
                .AsNoTracking()
                .Where(x => !x.IsDeleted)
                .AsQueryable();

            var userId = _currentUserService.UserId;
            if (_currentUserService.IsSalesUser() && !_currentUserService.IsAdmin() && !_currentUserService.IsSalesManager() && !_currentUserService.IsManager())
            {
                query = query.Where(p =>
                    !_db.Customers.Any(c => c.Id == p.Id) ||
                    _db.Customers.Any(c => c.Id == p.Id && c.SalesUserId == userId)
                );
            }

            //if (_currentUserService.IsInRole("Sales User") && !_currentUserService.IsInRole("Sales Manager") && !_currentUserService.IsInRole("Admin"))
            //{
            //    query = query.Join(
            //        _db.Customers.AsNoTracking().Where(x => x.SalesUserId == _currentUserService.UserId),
            //        person => person.Id,
            //        customer => customer.Id,
            //        (person, customer) => person);
            //}


            if (!string.IsNullOrWhiteSpace(term))
            {
                query = query.Where(x =>
                    x.PersonName.Contains(term) ||
                    x.PersonPersianName.Contains(term));
            }

            return await query
                .OrderBy(x => x.PersonName)
                .Take(20)
                .Select(x => new PartyLookupItemVm
                {
                    Id = x.Id,
                    DisplayName = x.PersonName + " - " + x.PersonPersianName,
                    ContactName = x.PersonContacts.Any() ? x.PersonContacts.FirstOrDefault().FullName : "",
                    EmailAddress = x.EmailAddress,
                    Mobile = x.Mobile
                })
                .ToListAsync();
        }


        public async Task<List<LocationLookupItemVm>> SearchLocationsAsync(string? term)
        {
            term = term?.Trim();
            var query = _db.Locations
                .AsNoTracking()
                .Where(x => !x.IsDeleted && x.IsActive);


            if (!string.IsNullOrWhiteSpace(term))
            {
                query = query.Where(x =>
                    x.LocationName.Contains(term) ||
                    x.LocationLocalName.Contains(term));
            }

            return await query
                .OrderBy(x => x.LocationName)
                .Take(20)
                .Select(x => new LocationLookupItemVm
                {
                    Id = x.Id,
                    DisplayName = x.LocationName + " (" + x.LocationCode + ")",
                })
                .ToListAsync();
        }




        public async Task<List<RateRequestContainerListItemVm?>> GetContainersListAsync(Guid id)
        {
            return await _db.RateRequestContainers
                  .Include(x => x.ContainerSizeType)
                  .Include(x => x.CntOwnType)
                  .Where(x => x.JobId == id)
                  .Select(x => new RateRequestContainerListItemVm
                  {
                      Id = x.Id,
                      ShortName = x.ContainerSizeType.ShortName + "(" + x.ContainerSizeType.FullName + ")",
                      Counts = (int)x.Counts,
                      Remarks = x.Remarks,
                      SizeTypeId = x.ContainerSizeTypeId,
                      CntOwnTypeName = x.CntOwnType.CntOwnTypeName
                  }
                  ).ToListAsync();
        }

        public async Task<Guid> GetFirstShipmentIdAsync(Guid jobId)
        {
            var shipment = await _db.Shipments
                  .Where(x => x.JobId == jobId)
                  .FirstOrDefaultAsync();
            if (shipment == null) return Guid.Empty;
            return shipment.Id;
        }
        public async Task<string> GetJobFirstShipmentTitleAsync(Guid jobid)
        {
            var shipment = await _db.Shipments.Where(x => x.JobId == jobid).FirstOrDefaultAsync();
            if (shipment != null) return shipment.ShipmentTitle;
            return "Not Defined";
        }
        public async Task<Guid> GenerateJobFromAcceptedSellQuoteAsync(Guid quoteId)
        {
            var sq = await _db.SellQuotes
                .Include(x => x.RateRequest)
                    .ThenInclude(x => x.TransportTypes)
                .Include(x => x.RateRequest)
                    .ThenInclude(x => x.Containers)
                .Include(x => x.RateRequest)
                    .ThenInclude(x => x.CargoDetails)
                .Include(x => x.Items)
                .Include(x => x.SelectedBuyQuote)
                .Where(x => x.Id == quoteId).FirstOrDefaultAsync();

            if (sq == null) return Guid.Empty;

            var existingJob = await _db.Jobs.Where(x => x.SellQuoteId == quoteId).FirstOrDefaultAsync();
            if (existingJob != null) return Guid.Empty;


            var currentUserId = _currentUserService.UserId;
            var now = DateTime.Now;
            var jobId = Guid.CreateVersion7();

            var job = new Domain.Entities.Operations.Job
            {
                Id = jobId,
                JobSourceTypeId = 2,
                SellQuoteId = sq.Id,
                CompanyId = sq.CompanyId,
                JobNumber = GenerateNewJobNumberAsync(),
                JobFileNumber = await GenerateNewJobFileNumberAsync(sq.RateRequest.AssignedSalesUserId.Value),
                TransportTypeId = sq.RateRequest.TransportTypes.FirstOrDefault().TransportTypeId,
                TradeTypeId = sq.RateRequest.TradeTypeId.Value,
                LoadTypeId = sq.RateRequest.LoadTypeId,
                CargoHandlingTypeId = sq.RateRequest.CargoHandlingTypeId,
                CargoNatureId = sq.RateRequest.CargoNatureId,
                //IncotermId = sq.RateRequest.IncotermId,
                IncotermsLocationName = "-",
                FreightPaymentTypeId = sq.RateRequest.PaymentTypeId.Value,
                SalesUserId = sq.RateRequest.AssignedSalesUserId,
                //JobTypeId = vm.JobTypeId,
                JobStatusId = 1,
                JobDate = DateTime.Now,
                CargoReadyDate = sq.RateRequest.CargoReadinessDate,
                CreatedAt = now,
                CreateUserId = currentUserId,
                //IsActive = true,
                IsDeleted = false
            };

            var routing = new Domain.Entities.Operations.JobRouting
            {
                Id = Guid.CreateVersion7(),
                JobId = jobId,
                OriginLocationId = sq.RateRequest.OriginId.Value,
                DestinationLocationId = sq.RateRequest.DestinationId.Value,
                MovementTypeId = sq.RateRequest.MovementTypeId.Value,
                ETD = sq.ETD,
                ETA = sq.ETA,
                CreatedAt = now,
                CreateUserId = currentUserId,
                //IsActive = true,
                IsDeleted = false
            };
            _db.Jobs.Add(job);
            _db.JobRoutings.Add(routing);
            int sequenceNo = 0;
            if (sq.RateRequest.CustomerId != null && sq.RateRequest.CustomerId != Guid.Empty)
            {
                sequenceNo += 1;
                var party = new JobParty
                {
                    Id = Guid.CreateVersion7(),
                    JobId = jobId,
                    SequenceNo = sequenceNo,
                    PartyId = sq.RateRequest.CustomerId.Value,
                    RoleTypeId = 2,
                    IsPrimaryRole = true,
                    ContactName = sq.RateRequest.ContactName,
                    ContactEmail = sq.RateRequest.EmailAddress,
                    ContactPhone = sq.RateRequest.MobileNo,
                    CreatedAt = now,
                    CreateUserId = currentUserId,
                    //IsActive = true,
                    IsDeleted = false
                };
                _db.JobParties.Add(party);
            }
            foreach (var item in sq.RateRequest.Containers)
            {
                var cnt = new RateRequestContainer
                {
                    JobId = jobId,
                    ContainerSizeTypeId = item.ContainerSizeTypeId,
                    Counts = item.Counts,
                    Remarks = item.Remarks,
                    CreateUserId = currentUserId,
                    CreatedAt = DateTime.Now
                };
                _db.RateRequestContainers.Add(cnt);
            }
            //Charges
            foreach (var item in sq.Items)
            {
                var charge = new Charge
                {
                    Id = Guid.CreateVersion7(),
                    JobId = job.Id,
                    ChargeTypeId = item.ChargeTypeId,
                    Amount = item.SellTotalAmount,
                    TaxAmount = item.SellTaxAmount,
                    TaxPercent = item.SellTaxPercent,
                    CalculationBy = item.CalculationBy,
                    CompanyId = item.ResponsibleCompanyId,
                    UnitRate = item.SellUnitRate,
                    Quantity = item.SellQuantity,
                    CreateUserId = currentUserId,
                    CreatedAt = DateTime.Now,
                    CurrencyId = item.SellCurrencyId,
                    IsPostedToAcc = false,
                    PartyId = sq.RateRequest.Customer is not null ? sq.RateRequest.CustomerId : null,
                    DirectionId = 1  //sell
                };
                _db.Charges.Add(charge);

                var bcharge = new Charge
                {
                    Id = Guid.CreateVersion7(),
                    JobId = job.Id,
                    ChargeTypeId = item.ChargeTypeId,
                    Amount = item.BuyTotalAmount,
                    TaxAmount = item.BuyTaxAmount,
                    TaxPercent = item.BuyTaxPercent,
                    CalculationBy = item.CalculationBy,
                    CompanyId = item.ResponsibleCompanyId,
                    UnitRate = item.BuyUnitRate,
                    Quantity = item.BuyQuantity,
                    CreateUserId = currentUserId,
                    CreatedAt = DateTime.Now,
                    CurrencyId = item.BuyCurrencyId,
                    IsPostedToAcc = false,
                    PartyId = sq.SelectedBuyQuote is null ? null : sq.SelectedBuyQuote.VendorId is not null ? sq.SelectedBuyQuote.VendorId : null,
                    DirectionId = 2   //buy
                };
                _db.Charges.Add(bcharge);
            }
            var shipment = new Shipment
            {
                Id = Guid.CreateVersion7(),
                JobId = jobId,
                ShipmentTitle = "SH" + job.JobNumber,
                CreateUserId = _currentUserService.UserId,
                CreatedAt = DateTime.Now,
                ShipmentStatusId = 1
            };
            _db.Shipments.Add(shipment);
            await _db.SaveChangesAsync();
            foreach (var cargo in sq.RateRequest.CargoDetails)
            {
                var cargoDetail = new CargoDetail
                {
                    Id = Guid.CreateVersion7(),
                    JobId = jobId,
                    CargoCommodityId = cargo.CargoCommodityId,
                    CargoDescription = cargo.CargoDescription,
                    ChargeableWeight = cargo.ChargeableWeight,
                    CountryHSCodeId = cargo.CountryHSCodeId,
                    CreatedAt = DateTime.Now,
                    CreateUserId = _currentUserService.UserId,
                    DGClassId = cargo.DGClassId,
                    DGFlashPoint = cargo.DGFlashPoint,
                    DGPackingGroupId = cargo.DGPackingGroupId,
                    DimensionUnitId = cargo.DimensionUnitId,
                    GrossWeight = cargo.GrossWeight,
                    HSCodeToSave = cargo.HSCodeToSave,
                    Height = cargo.Height,
                    //IsActive = true,
                    InnerPackageCount = cargo.InnerPackageCount,
                    InnerPackageTypeId = cargo.InnerPackageTypeId,
                    IsDeleted = false,
                    IsDG = cargo.IsDG,
                    IsOverSized = cargo.IsOverSized,
                    IsFragile = cargo.IsFragile,
                    IsPerishable = cargo.IsPerishable,
                    IsRotateable = cargo.IsRotateable,
                    IsStackable = cargo.IsStackable,
                    IsTemperatureControlled = cargo.IsTemperatureControlled,
                    Length = cargo.Length,
                    Width = cargo.Width,
                    WeightUnitId = cargo.WeightUnitId,
                    VolumeUnitId = cargo.VolumeUnitId,
                    Volume = cargo.Volume,
                    UNNumber = cargo.UNNumber,
                    TransitShipmentAssignmentId = cargo.TransitShipmentAssignmentId,
                    TempUnitId = cargo.TempUnitId,
                    Remarks = cargo.Remarks,
                    PackageTypeId = cargo.PackageTypeId,
                    Sequence = cargo.Sequence,
                    PackageCount = cargo.PackageCount,
                    NormalTemp = cargo.NormalTemp,
                    NetWeight = cargo.NetWeight,
                    MarksAndNumbers = cargo.MarksAndNumbers,
                    CargoCommodity = cargo.CargoCommodity,
                    MinTemp = cargo.MinTemp,
                    MaxTemp = cargo.MaxTemp,
                };
                _db.CargoDetails.Add(cargoDetail);
            }
            await _db.SaveChangesAsync();
            sq.StatusId = 7;
            await _db.SaveChangesAsync();
            //_db.JobStatusHistories.Add(new JobStatusHistory { JobId = jobId, CreatedAt = DateTime.Now, CreateUserId = currentUserId, OldStatusId = 1, NewStatusId = 1, ReasonRemarks = "Job Created" });
            await _activityLogService.LogHistoryAsync(
                new CreateActivityLogRequest
                {
                    ActivityCategoryId = 2,
                    ActivityDateTime = DateTime.Now,
                    Title = "Job Created ",
                    Description = "Job Created From Sell Quoptation Acceptance!!!",
                    EntityTypeId = 1,
                    EntityId = jobId,
                    FromStatusEntityType = "JobStatus",
                    ToStatusId = 1,
                    PerformedByUserId = currentUserId,
                    IsSystemGenerated = true,
                }
                );
            return job.Id;
        }
        public async Task<bool> JobExistsToConfirmAsync(Guid myJobId)
        {
            return await _db.Jobs.Where(x => x.Id == myJobId && !x.IsDeleted && x.JobStatusId == 1).AnyAsync();
        }
        public async Task<bool> OperationConfirmAsync(Guid myJobId)
        {
            var currentUserId = _currentUserService.UserId;
            var job = await _db.Jobs.Where(x => x.Id == myJobId).FirstOrDefaultAsync();
            var oldValue = job.JobStatusId;
            job?.JobStatusId = 2;
            await _db.SaveChangesAsync();
            await _activityLogService.LogHistoryAsync(
                new CreateActivityLogRequest
                {
                    ActivityCategoryId = 3, //status
                    ActivityDateTime = DateTime.Now,
                    Title = "Job Confirmed",
                    Description = "Jon Status Changed to confirmed",
                    EntityTypeId = 1,
                    EntityId = myJobId,
                    FromStatusEntityType = "JobStatus",
                    ToStatusId = 1,
                    PerformedByUserId = currentUserId,
                    IsSystemGenerated = true,
                    OldValue = oldValue.ToString(),
                    FromStatusId = oldValue,
                    NewValue = job?.JobStatusId.ToString(),
                    ToStatusEntityType = "base.JobStatus"
                }
                );
            return true;
        }

        public async Task<CreateChargeVm> GetChargeDetailsAsync(Guid chargeId)
        {
            var charge = await _db.Charges
                     .Include(x => x.Currency)
                     .Include(x => x.ChargeType)
                     .Include(x => x.Party)
                     .Include(x => x.Company)
                     .ThenInclude(x => x.Person)
                    .Where(x => x.Id == chargeId && !x.IsDeleted)
                    .Select(x => new
                    {
                        x.JobId,
                        x.Id,
                        x.ChargeTypeId,
                        x.DirectionId,
                        x.ChargeType.ChargeTypeName,
                        x.ChargeDescription,
                        Qty = x.Quantity,
                        x.UnitRate,
                        x.Amount,
                        Currency = x.Currency.CurrencyCode,
                        Vendor = x.PartyId != null ? x.Party.PersonName : "",
                        PartyName = x.Party.PersonName,
                        x.IsTaxable,
                        x.TaxPercent,
                        x.GrossAmount,
                        x.Remarks,
                        x.CompanyId,
                        CompanyName = x.Company.Person.PersonName,
                        x.CurrencyId,
                        x.PartyId,
                        x.TaxAmount
                    })
                    .FirstOrDefaultAsync();
            var vm = new CreateChargeVm
            {
                JobId = charge.JobId,
                Id = charge.Id,
                Quantity = charge.Qty,
                Amount = charge.UnitRate,
                CompanyId = charge.CompanyId,
                Remarks = charge.Remarks,
                GrossAmount = charge.GrossAmount is null ? 0 : charge.GrossAmount.Value,
                Taxable = charge.IsTaxable,
                PartyName = charge.PartyName,
                TaxPercent = charge.TaxPercent is null ? 0 : charge.TaxPercent.Value,
                UnitRate = charge.UnitRate,
                ChargeDescription = charge.ChargeDescription,
                ChargeTypeId = charge.ChargeTypeId,
                CurrencyId = charge.CurrencyId,
                PartyId = charge.PartyId,
                DirectionId = charge.DirectionId,
                TaxAmount = charge.TaxAmount is null ? 0 : charge.TaxAmount.Value
            };
            return vm;
        }
        public async Task<ChargeDeleteVm?> BuildChargeDeleteModelAsync(Guid id)
        {
            var entity = await _db.Charges
                .Include(x => x.ChargeType)
                .AsNoTracking()
                .FirstOrDefaultAsync(x => x.Id == id && x.IsDeleted == false);
            if (entity == null)
                return null;
            return new ChargeDeleteVm
            {
                Id = entity.Id,
                JobId = entity.JobId,
                ChargeDescription = entity.ChargeType.ChargeTypeName,
            };
        }
        public async Task<bool> DeleteChargeAsync(Guid id)
        {
            var charge = await _db.Charges.FindAsync(id);
            _db.Charges.Remove(charge);
            await _db.SaveChangesAsync();
            return true;
        }
        public async Task<CreateBookingVm> GetBookingViewModelAsync(
            Guid jobId,
            Guid? bookingId = null,
            int? transportTypeId = null,
            int? companyRoleTypeId = null)
        {
            // ۱. بارگذاری تمامی لیست‌های عمومی (دراپ‌داون‌ها) با استفاده از متد از پیش تعریف شده شما
            var vm = await BuildCreateBookingVmAsync();
            vm.JobId = jobId;

            // ۲. بارگذاری اطلاعات پیش‌فرض مرتبط با Job اصلی (جهت به ارث‌بری فیلدها در زمان ایجاد جدید)
            var job = await _db.Jobs
                .Include(x => x.Routing)
                .AsNoTracking()
                .FirstOrDefaultAsync(j => j.Id == jobId);

            if (job == null)
            {
                throw new KeyNotFoundException($"Job with ID {jobId} not found.");
            }

            // لود کردن گزینه‌های Shipment Legs مربوط به این جاب خاص
            // (با توجه به جدول ops.ShipmentLegs که به ops.Shipments و ops.Jobs متصل است)
            vm.Legs = await _db.ShipmentLegs
                .AsNoTracking()
                .Where(sl => _db.Shipments.Where(s => s.JobId == jobId).Select(s => s.Id).Contains(sl.ShipmentId))
                .Select(sl => new SelectListItem
                {
                    Value = sl.Id.ToString(),
                    Text = $"{sl.SequenceNo} - {sl.ReceiptPlaceText} to {sl.DeliveryPlaceText}"
                })
                .ToListAsync();

            // ۳. سناریوی ویرایش (Edit Mode): اطلاعات بوکینگ موجود را به همراه جزئیات بارگذاری می‌کنیم
            if (bookingId.HasValue && bookingId.Value != Guid.Empty)
            {
                var booking = await _db.Bookings
                    .Include(b => b.BookingLegs)
                    .FirstOrDefaultAsync(b => b.Id == bookingId.Value);

                if (booking != null)
                {
                    vm.BookingId = booking.Id;
                    vm.CompanyId = booking.CompanyId;
                    vm.CompanyRoleTypeId = booking.CompanyRoleTypeId;
                    vm.LoadTypeId = booking.LoadTypeId;
                    vm.ServiceTypeId = booking.ServiceTypeId;
                    vm.TransportTypeId = booking.TransportTypeId;
                    vm.BookingNo = booking.BookingNo;
                    vm.LineBookingNo = booking.LineBookingNo;

                    // تخصیص پارت‌های مختلف
                    vm.CarrierId = booking.CarrierPartyId;
                    vm.VendorId = booking.VendorPartyId;
                    vm.BookingAgentId = booking.BookingAgentPartyId;

                    // بنادر و فرودگاه‌ها (Locations)
                    vm.POLId = booking.POLId;
                    vm.PODId = booking.PODId;
                    vm.OriginLocationId = booking.OriginId.Value;
                    vm.DestinationLocationId = booking.DestId.Value;

                    // ددلاین‌ها
                    vm.CargoReadyDate = booking.CargoReadyDate;
                    vm.CutOffDate = booking.CutOffDate;
                    vm.SICutOffDate = booking.SICutOffDate;
                    vm.VGMCutOffDate = booking.VGMCutOffDate;
                    vm.DocumentCutOffDate = booking.DocumentCutOffDate;
                    vm.Remarks = booking.Remarks;
                    //vm.BookingStatusI = booking.BookingStatusId;

                    // دریافت اولین ShipmentLegId تخصیص یافته به بوکینگ (بر اساس ops.BookingLegs)
                    vm.ShipmentLegId = booking.BookingLegs.FirstOrDefault()?.ShipmentLegId;

                    // بارگذاری کانتینرها از جدول ops.BookingContainerTypes
                    var dbContainers = await _db.BookingContainerTypes
                        .Where(c => c.BookingId == booking.Id && !c.IsDeleted)
                        .AsNoTracking()
                        .ToListAsync();

                    vm.Containers = dbContainers.Select(c => new JobContainerCreateVm
                    {
                        Id = c.Id,
                        ContainerSizeTypeId = c.ContainerSizeTypeId,
                        cntOwnTypeId = c.CntOwnTypeId,
                        Counts = c.ContainerCount,
                        //DGId = c.DGId,
                        //OverLengthCM = c.OverLengthCM,
                        //OverWidthCM = c.OverWidthCM,
                        //OverHeightCM = c.OverHeightCM
                    }).ToList();

                    // بارگذاری سایر بخش‌ها (پارتی‌ها و...) از ops.BookingParty
                    var dbParties = await _db.BookingParties
                        .Where(p => p.BookingId == booking.Id && !p.IsDeleted)
                        .AsNoTracking()
                        .ToListAsync();

                    vm.Parties = dbParties.Select(p => new BookingPartyCreateVm
                    {
                        Id = p.Id,
                        PartyId = p.PartyId,
                        RoleTypeId = p.PartyRoleTypeId
                    }).ToList();
                }
            }
            // ۴. سناریوی ثبت جدید (Create Mode): ارث‌بری مقادیر پایه از Job اصلی
            else
            {
                vm.TransportTypeId = job.TransportTypeId;
                vm.LoadTypeId = job.LoadTypeId;
                vm.OriginLocationId = job.Routing.FirstOrDefault().OriginLocationId; // یا تخصیص از جدول Routings
                vm.CargoReadyDate = job.CargoReadyDate;
                vm.Remarks = job.OperationalNotes;

                vm.Containers = new List<JobContainerCreateVm>();
                vm.Parties = new List<BookingPartyCreateVm>();
                vm.Packages = new List<JobPackageCreateVm>();
            }

            // ۵. اعمال Overrides ناشی از درخواست‌های داینامیک HTMX (تغییر Transport Type یا Role روی کلاینت)
            if (transportTypeId.HasValue && transportTypeId.Value > 0)
            {
                vm.TransportTypeId = transportTypeId.Value;
            }

            if (companyRoleTypeId.HasValue && companyRoleTypeId.Value > 0)
            {
                vm.CompanyRoleTypeId = companyRoleTypeId.Value;
            }

            return vm;
        }
        public async Task<CreateRTSInstructionVm> GetRTSInstructionForEditAsync(Guid id)
        {
            var vm = await _db.RTSInstructions
                .Include(x => x.Booking)
                .Include(x => x.RTSInstructionContainerTypes)
                .Where(x => x.Id == id)
                .Select(x => new CreateRTSInstructionVm
                {
                    Id = x.Id,
                    JobId = x.JobId,
                    BookingId = x.BookingId,
                    RTSInstructionNo = x.RTSInstructionNo,
                    RTSInstructionDate = x.RTSInstructionDate,
                    Remarks = x.Remarks,
                    ValidDateFrom = x.ValidDateFrom,
                    ValidDateTo = x.ValidDateTo,
                    ShipperId = x.ShipperId,
                    CarrierId = x.CarrierId,
                    DepotId = x.DepotId,
                    StatusId = x.StatusId,
                    CompanyId = x.CompanyId,
                    //ContainerSizeTypes = x.RTSInstructionContainerTypes.Select(x => new RTSInstructionContainerTypeVm
                    //{
                    //    ContainerCounts = x.ContainerCounts,
                    //    ContainerSizeTypeName = x.ContainerSizeType.ShortName,
                    //    ContainerSizeTypeId = x.ContainerSizeTypeId,
                    //    Remarks = x.Remarks,
                    //    Id = x.Id,
                    //    RTSInstructionId = x.RTSInstructionId
                    //}
                    //).ToList()
                }).FirstOrDefaultAsync();
            return vm;
        }
        public async Task<bool> DeleteRTSInstructionAsync(Guid id)
        {
            var rtsi = await _db.RTSInstructions.Where(x => x.Id == id).FirstOrDefaultAsync();
            if (rtsi == null) return false;
            rtsi.IsDeleted = true;
            rtsi.DeletedAt = DateTime.Now;
            rtsi.DeleteUserId = _currentUserService.UserId;
            await _db.SaveChangesAsync();
            return true;
        }
        public async Task<bool> ConfirmAndIssueRTSInstructionAsync(Guid id)
        {
            var rtsi = await _db.RTSInstructions.Where(x => x.Id == id).FirstOrDefaultAsync();
            if (rtsi == null) return false;
            if (rtsi.StatusId != 1) return false;
            rtsi.StatusId = 2;
            rtsi.IssuedByUserId = _currentUserService.UserId;
            rtsi.ApprovedByUserId = _currentUserService.UserId;
            rtsi.ApprovedAT = DateTime.Now;
            await _db.SaveChangesAsync();
            return true;
        }

        public async Task<Guid> GetJobIdForQuickSearchTermAsync(string searchTerm)
        {
            var jobs = await _db.Jobs
                       .AsNoTracking()
                       .Where(x => x.JobFileNumber == searchTerm).ToListAsync();
            if (jobs.Count == 1)
            {
                return jobs.First().Id;
            }
            else if (jobs.Count > 1)
            {
                return Guid.Empty;
            }


            var bls = await _db.BLs
                       .AsNoTracking()
                       .Where(x => x.BLNo.Contains(searchTerm) || x.MasterBLNo.Contains(searchTerm)).ToListAsync();
            if (bls.Count == 1)
            {
                return bls.First().JobId;
            }
            else if (bls.Count > 1)
            {
                return Guid.Empty;
            }

            var brs = await _db.Bookings
                       .AsNoTracking()
                       .Where(x => x.BookingNo.Contains(searchTerm) || x.LineBookingNo.Contains(searchTerm)).ToListAsync();
            if (brs.Count == 1)
            {
                return brs.First().JobId;
            }
            else if (brs.Count > 1)
            {
                return Guid.Empty;
            }
            return Guid.Empty;
        }
        public async Task<List<HSCodeLookupItemVm>> SearchHSCodesAsync(string? term)
        {
            term = term?.Trim();
            var query = _db.CountryHSCodes
                .Include(x => x.Country)
                .AsNoTracking()
                .Where(x => !x.IsDeleted && x.IsActive);
            if (!string.IsNullOrWhiteSpace(term))
            {
                query = query.Where(x =>
                    x.CHSCode.Contains(term) ||
                    x.CHSName.Contains(term));
            }
            return await query
                .OrderBy(x => x.CHSCode)
                .Take(20)
                .Select(x => new HSCodeLookupItemVm
                {
                    HSCode = x.CHSCode,
                    DisplayName = x.CHSCode + "-" + x.Country.CountryIsoCode2 + " (" + x.CHSName.Substring(0, 50) + ")"
                })
                .ToListAsync();
        }
        public async Task<List<PackageTypeLookupItemVm>> SearchPackageTypesAsync(string? term)
        {
            term = term?.Trim();
            var query = _db.PackageTypes
                .AsNoTracking()
                .Where(x => !x.IsDeleted && x.IsActive);
            if (!string.IsNullOrWhiteSpace(term))
            {
                query = query.Where(x =>
                    x.PackageTypeCode.Contains(term) ||
                    x.PackageTypeName.Contains(term));
            }
            return await query
                .OrderBy(x => x.PackageTypeName)
                .Take(20)
                .Select(x => new PackageTypeLookupItemVm
                {
                    Id = x.Id,
                    DisplayName = x.PackageTypeName
                })
                .ToListAsync();
        }

        public async Task<int> GetActiveShipmentsAsync()
        {
            return await _db.Jobs.Where(x => x.JobStatusId == 2 || x.JobStatusId == 3 || x.JobStatusId == 4).CountAsync();
        }

        public async Task<List<TransportationModeChartItemVm>> GetTransportationModeChartAsync(
            int companyId,
            CancellationToken cancellationToken = default)
        {
            var today = DateTime.Now.Date;
            var currentMonthStart = new DateTime(today.Year, today.Month, 1);
            var nextMonthStart = currentMonthStart.AddMonths(1);

            var rawItems = await _db.Jobs
                .AsNoTracking()
                .Where(x =>
                    !x.IsDeleted &&
                    x.CompanyId == companyId &&
                    x.JobStatusId != 6 &&

                    // اگر می‌خواهید فقط Jobهای ماه جاری باشند:
                    x.CreatedAt >= currentMonthStart &&
                    x.CreatedAt < nextMonthStart)
                .GroupBy(x => new
                {
                    x.TransportTypeId,
                    Title = x.TransportType.TransportTypeName
                })
                .Select(x => new
                {
                    TransportationModeId = x.Key.TransportTypeId,
                    Title = x.Key.Title,
                    Count = x.Count()
                })
                .OrderByDescending(x => x.Count)
                .ToListAsync(cancellationToken);

            var totalCount = rawItems.Sum(x => x.Count);

            return rawItems
                .Select(x => new TransportationModeChartItemVm
                {
                    TransportationModeId = x.TransportationModeId,
                    Title = string.IsNullOrWhiteSpace(x.Title)
                        ? "Unknown"
                        : x.Title,

                    Count = x.Count,

                    Percentage = totalCount == 0
                        ? 0
                        : Math.Round((decimal)x.Count / totalCount * 100m, 1),

                    Color = GetTransportationModeColor(x.TransportationModeId)
                })
                .ToList();
        }


        private static string GetTransportationModeColor(int transportationModeId)
        {
            // شناسه‌ها را مطابق اطلاعات واقعی جدول TransportationMode تنظیم کنید.
            return transportationModeId switch
            {
                1 => "#696cff", // Sea
                2 => "#36b9cc", // Air
                3 => "#f6c23e", // Road
                4 => "#1cc88a", // Rail
                5 => "#8592a3", // Multimodal
                _ => "#8592a3"
            };
        }

        public async Task<ShipmentVolumeChartVm> GetShipmentVolumeChartAsync(
            int companyId,
            string range,
            CancellationToken cancellationToken = default)
        {
            range = string.IsNullOrWhiteSpace(range)
                ? "weekly"
                : range.Trim().ToLowerInvariant();

            return range switch
            {
                "monthly" => await GetMonthlyShipmentVolumeAsync(
                    companyId,
                    cancellationToken),

                "yearly" => await GetYearlyShipmentVolumeAsync(
                    companyId,
                    cancellationToken),

                _ => await GetWeeklyShipmentVolumeAsync(
                    companyId,
                    cancellationToken)
            };
        }


        private async Task<ShipmentVolumeChartVm> GetWeeklyShipmentVolumeAsync(
    int companyId,
    CancellationToken cancellationToken)
        {
            var today = DateTime.Now.Date;

            // Monday = شروع هفته
            var daysSinceMonday = ((int)today.DayOfWeek + 6) % 7;
            var currentWeekStart = today.AddDays(-daysSinceMonday);

            var nextWeekStart = currentWeekStart.AddDays(7);
            var previousWeekStart = currentWeekStart.AddDays(-7);

            var jobs = await _db.Jobs
                .AsNoTracking()
                .Where(x =>
                    !x.IsDeleted &&
                    x.CompanyId == companyId &&
                    x.JobStatusId != 7 &&
                    x.CreatedAt >= previousWeekStart &&
                    x.CreatedAt < nextWeekStart)
                .Select(x => x.CreatedAt)
                .ToListAsync(cancellationToken);

            var labels = new List<string>
    {
        "Mon", "Tue", "Wed", "Thu", "Fri", "Sat", "Sun"
    };

            var currentPeriodData = Enumerable.Range(0, 7)
                .Select(dayIndex =>
                {
                    var dayStart = currentWeekStart.AddDays(dayIndex);
                    var dayEnd = dayStart.AddDays(1);

                    return jobs.Count(jobDate =>
                        jobDate >= dayStart &&
                        jobDate < dayEnd);
                })
                .ToList();

            var previousPeriodData = Enumerable.Range(0, 7)
                .Select(dayIndex =>
                {
                    var dayStart = previousWeekStart.AddDays(dayIndex);
                    var dayEnd = dayStart.AddDays(1);

                    return jobs.Count(jobDate =>
                        jobDate >= dayStart &&
                        jobDate < dayEnd);
                })
                .ToList();

            return new ShipmentVolumeChartVm
            {
                Range = "weekly",
                Labels = labels,
                CurrentPeriodData = currentPeriodData,
                PreviousPeriodData = previousPeriodData,
                CurrentPeriodLabel = "This Week",
                PreviousPeriodLabel = "Last Week"
            };
        }

        private async Task<ShipmentVolumeChartVm> GetMonthlyShipmentVolumeAsync(
    int companyId,
    CancellationToken cancellationToken)
        {
            var today = DateTime.Now.Date;

            var currentMonthStart = new DateTime(today.Year, today.Month, 1);
            var nextMonthStart = currentMonthStart.AddMonths(1);
            var previousMonthStart = currentMonthStart.AddMonths(-1);

            var currentMonthDays = DateTime.DaysInMonth(
                currentMonthStart.Year,
                currentMonthStart.Month);

            var previousMonthDays = DateTime.DaysInMonth(
                previousMonthStart.Year,
                previousMonthStart.Month);

            var jobs = await _db.Jobs
                .AsNoTracking()
                .Where(x =>
                    !x.IsDeleted &&
                    x.CompanyId == companyId &&
                    x.JobStatusId != 7 &&
                    x.CreatedAt >= previousMonthStart &&
                    x.CreatedAt < nextMonthStart)
                .Select(x => x.CreatedAt)
                .ToListAsync(cancellationToken);

            // برای مقایسه صحیح، بیشترین تعداد روز دو ماه را نمایش می‌دهیم.
            var maxDays = Math.Max(currentMonthDays, previousMonthDays);

            var labels = Enumerable.Range(1, maxDays)
                .Select(day => day.ToString())
                .ToList();

            var currentPeriodData = Enumerable.Range(1, maxDays)
                .Select(day =>
                {
                    if (day > currentMonthDays)
                        return 0;

                    var dayStart = currentMonthStart.AddDays(day - 1);
                    var dayEnd = dayStart.AddDays(1);

                    return jobs.Count(jobDate =>
                        jobDate >= dayStart &&
                        jobDate < dayEnd);
                })
                .ToList();

            var previousPeriodData = Enumerable.Range(1, maxDays)
                .Select(day =>
                {
                    if (day > previousMonthDays)
                        return 0;

                    var dayStart = previousMonthStart.AddDays(day - 1);
                    var dayEnd = dayStart.AddDays(1);

                    return jobs.Count(jobDate =>
                        jobDate >= dayStart &&
                        jobDate < dayEnd);
                })
                .ToList();

            return new ShipmentVolumeChartVm
            {
                Range = "monthly",
                Labels = labels,
                CurrentPeriodData = currentPeriodData,
                PreviousPeriodData = previousPeriodData,
                CurrentPeriodLabel = "This Month",
                PreviousPeriodLabel = "Last Month"
            };
        }


        private async Task<ShipmentVolumeChartVm> GetYearlyShipmentVolumeAsync(
            int companyId,
            CancellationToken cancellationToken)
        {
            var today = DateTime.Now.Date;

            var currentYearStart = new DateTime(today.Year, 1, 1);
            var nextYearStart = currentYearStart.AddYears(1);
            var previousYearStart = currentYearStart.AddYears(-1);

            var jobs = await _db.Jobs
                .AsNoTracking()
                .Where(x =>
                    !x.IsDeleted &&
                    x.CompanyId == companyId &&
                    x.JobStatusId != 7 &&
                    x.CreatedAt >= previousYearStart &&
                    x.CreatedAt < nextYearStart)
                .Select(x => x.CreatedAt)
                .ToListAsync(cancellationToken);

            var labels = new List<string>
    {
        "Jan", "Feb", "Mar", "Apr", "May", "Jun",
        "Jul", "Aug", "Sep", "Oct", "Nov", "Dec"
    };

            var currentPeriodData = Enumerable.Range(1, 12)
                .Select(month =>
                {
                    var monthStart = new DateTime(today.Year, month, 1);
                    var monthEnd = monthStart.AddMonths(1);

                    return jobs.Count(jobDate =>
                        jobDate >= monthStart &&
                        jobDate < monthEnd);
                })
                .ToList();

            var previousPeriodData = Enumerable.Range(1, 12)
                .Select(month =>
                {
                    var monthStart = new DateTime(today.Year - 1, month, 1);
                    var monthEnd = monthStart.AddMonths(1);

                    return jobs.Count(jobDate =>
                        jobDate >= monthStart &&
                        jobDate < monthEnd);
                })
                .ToList();

            return new ShipmentVolumeChartVm
            {
                Range = "yearly",
                Labels = labels,
                CurrentPeriodData = currentPeriodData,
                PreviousPeriodData = previousPeriodData,
                CurrentPeriodLabel = "This Year",
                PreviousPeriodLabel = "Last Year"
            };
        }
        public async Task<List<TopShippingRouteVm>> GetTopShippingRoutesAsync(
            int companyId,
            int take = 5,
            CancellationToken cancellationToken = default)
        {
            take = take <= 0 ? 5 : Math.Min(take, 20);

            var routeGroups = await _db.Jobs
                .AsNoTracking()
                .Where(x =>
                    !x.IsDeleted &&
                    x.CompanyId == companyId &&
                    x.JobStatusId != 7)
                .GroupBy(x => new
                {
                    Origin = x.Routing.First().OriginLocation!.LocationName,
                    Destination = x.Routing.First().DestinationLocation!.LocationName
                })
                .Select(group => new
                {
                    Route = group.Key.Origin + " → " + group.Key.Destination,
                    ShipmentCount = group.Count()
                })
                .OrderByDescending(x => x.ShipmentCount)
                .Take(take)
                .ToListAsync(cancellationToken);

            return routeGroups
                .Select(x => new TopShippingRouteVm
                {
                    Route = x.Route,
                    ShipmentCount = x.ShipmentCount
                })
                .ToList();
        }

        public async Task<JobNoFileNoVm> GetJobNoFileNumberAsync(Guid id)
        {
            var job = await _db.Jobs.Where(x => x.Id == id).FirstOrDefaultAsync();
            return new JobNoFileNoVm { JobNumber = job.JobNumber, JobFileNumber = job.JobFileNumber };
        }

        public async Task<List<SelectListItem>> GetJobPartiesSelectListByBLIdAsync(Guid blId)
        {
            var jobParties = await _db.BLs
                .Where(bl => bl.Id == blId && !bl.IsDeleted)
                .SelectMany(bl => bl.Job.Parties)
                .Where(jp => !jp.IsDeleted)
                .Select(jp => new
                {
                    Id = jp.PartyId,
                    PersonName = jp.Party != null
                        ? jp.Party.PersonName
                        : (jp.Party != null && jp.Party.PersonName != null ? jp.Party.PersonName : string.Empty)
                })
                .Distinct()
                .ToListAsync();
            return jobParties.ToSelectListItems(
                x => x.Id.ToString(),
                x => x.PersonName
            );
        }
        public async Task<bool> DeleteAsync(Guid id)
        {
            var job = await _db.Jobs
                .Include(x => x.Routing)
                .Include(x => x.JobCompanyRoles)
                .Include(x => x.Containers)
                .Include(x => x.CargoDetails)
                .Include(x => x.Parties)
                .Where(x => x.Id == id && !x.IsDeleted).FirstOrDefaultAsync();
            if (job == null) return false;

            var b = await CanBeDeletedAsync(id);
            if (b.CanDelete)
            {

                foreach (var item in job.Parties)
                {
                    item.IsDeleted = true;
                    item.DeletedAt = DateTime.Now;
                    item.DeleteUserId = _currentUserService.UserId;
                }
                foreach (var item in job.Routing)
                {
                    item.IsDeleted = true;
                    item.DeletedAt = DateTime.Now;
                    item.DeleteUserId = _currentUserService.UserId;
                }
                foreach (var item in job.JobCompanyRoles)
                {
                    item.IsDeleted = true;
                    item.DeletedAt = DateTime.Now;
                    item.DeleteUserId = _currentUserService.UserId;
                }
                foreach (var item in job.CargoDetails)
                {
                    item.IsDeleted = true;
                    item.DeletedAt = DateTime.Now;
                    item.DeleteUserId = _currentUserService.UserId;
                }
                foreach (var item in job.Containers)
                {
                    item.IsDeleted = true;
                    item.DeletedAt = DateTime.Now;
                    item.DeleteUserId = _currentUserService.UserId;
                }
                job.IsDeleted = true;
                job.DeletedAt = DateTime.Now;
                job.DeleteUserId = _currentUserService.UserId;
                await _db.SaveChangesAsync();
                return true;
            }
            return false;
        }
        public async Task<(bool CanDelete, string Message)> CanBeDeletedAsync(Guid id)
        {
            var hasShipmentLegs = await _db.ShipmentLegs.AnyAsync(x => x.Shipment.JobId == id && !x.IsDeleted);
            if (hasShipmentLegs) return (false, "Shipment Legs Exists");

            var hasInvoices = await _db.Invoices.AnyAsync(x => x.JobId == id && !x.IsDeleted);
            if (hasInvoices) return (false, "Invoice Exists");

            var hasBookings = await _db.Bookings.AnyAsync(x => x.JobId == id && !x.IsDeleted);
            if (hasBookings) return (false, "Booking Exists");

            var hasBL = await _db.BLs.AnyAsync(x => x.JobId == id && !x.IsDeleted);
            if (hasBL) return (false, "BL Exists");
            return (true, string.Empty);
        }
        public async Task<List<SelectListItem>> GetJobPartiesSelectListByBookingIdAsync(Guid bookingId)
        {
            var jobParties = await _db.Bookings
               .Where(x => x.Id == bookingId && !x.IsDeleted)
               .SelectMany(x => x.Job.Parties)
               .Where(jp => !jp.IsDeleted)
               .Select(jp => new
               {
                   Id = jp.PartyId,
                   PersonName = jp.Party != null
                       ? jp.Party.PersonName
                       : (jp.Party != null && jp.Party.PersonName != null ? jp.Party.PersonName : string.Empty)
               })
               .Distinct()
               .ToListAsync();
            return jobParties.ToSelectListItems(
                x => x.Id.ToString(),
                x => x.PersonName
            );
        }
    }
}