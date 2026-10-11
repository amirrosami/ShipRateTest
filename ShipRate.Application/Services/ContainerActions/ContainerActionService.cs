using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using ShipRate.Application.DTOs;
using ShipRate.Application.Interfaces;
using ShipRate.Application.Interfaces.ContainerActions;
using ShipRate.Application.ViewModels;
using ShipRate.Common.Utilities;
using ShipRate.Domain.Entities.BookingOperation;
using ShipRate.Domain.Entities.ContainerOperation;
using ShipRate.Domain.Enums;
using ShipRate.Infrastructure.Data;
namespace ShipRate.Application.Services.ContainerActions
{
    public class ContainerActionService : IContainerActionService
    {
        private readonly ApplicationDbContext _context;
        private readonly ICurrentUserService _currentUserService;
        private readonly IContainerService _containerService;

        public ContainerActionService(ApplicationDbContext context, ICurrentUserService currentUserService, IContainerService containerService)
        {
            _context = context;
            _currentUserService = currentUserService;
            _containerService = containerService;
        }

        public async Task<ContainerActionIndexVm> SearchActionsAsync(
            ContainerActionSearchFormVm searchForm,
            int pageNumber,
            int pageSize)
        {
            searchForm ??= new ContainerActionSearchFormVm();

            pageNumber = pageNumber <= 0 ? 1 : pageNumber;
            pageSize = pageSize <= 0 ? 10 : pageSize;

            await PopulateSearchLookupsAsync(searchForm);

            var query = _context.ContainerActions
                .Include(x => x.Location)
                .AsNoTracking()
                .Where(x => !x.IsDeleted);

            if (!string.IsNullOrWhiteSpace(searchForm.ContainerNumber))
            {
                var containerNumber = searchForm.ContainerNumber.Trim();
                query = query.Where(x => x.ContainerNumber.Contains(containerNumber));
            }

            if (searchForm.CntActionTypeId.HasValue)
            {
                query = query.Where(x => x.CntActionTypeId == searchForm.CntActionTypeId.Value);
            }

            if (!string.IsNullOrWhiteSpace(searchForm.LocationName))
            {
                query = query.Where(x => x.Location.LocationName.Contains(searchForm.LocationName));
            }

            //if (searchForm.SubLocationId.HasValue)
            //{
            //    query = query.Where(x => x.SubLocationId == searchForm.SubLocationId.Value);
            //}

            if (searchForm.VoyageId.HasValue)
            {
                query = query.Where(x => x.VoyageId == searchForm.VoyageId.Value);
            }

            if (searchForm.VoyageTypeId.HasValue)
            {
                query = query.Where(x => x.VoyageTypeId == searchForm.VoyageTypeId.Value);
            }

            if (searchForm.CntOwnTypeId.HasValue)
            {
                query = query.Where(x => x.cntOwnTypeId == searchForm.CntOwnTypeId.Value);
            }

            if (searchForm.FullMTId.HasValue)
            {
                query = query.Where(x => x.FullMTId == searchForm.FullMTId.Value);
            }

            if (searchForm.EventActionSourceId.HasValue)
            {
                query = query.Where(x => x.EventActionSourceId == searchForm.EventActionSourceId.Value);
            }

            if (searchForm.FromDate.HasValue)
            {
                var fromDate = searchForm.FromDate.Value.Date;
                query = query.Where(x => x.ContainerActionDateTime >= fromDate);
            }

            if (searchForm.ToDate.HasValue)
            {
                var toDateExclusive = searchForm.ToDate.Value.Date.AddDays(1);
                query = query.Where(x => x.ContainerActionDateTime < toDateExclusive);
            }

            var totalCount = await query.CountAsync();

            var items = await query
                .OrderByDescending(x => x.ContainerActionDateTime)
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .Select(x => new ContainerActionListItemVm
                {
                    Id = x.Id,
                    CompanyId = x.CompanyId,
                    ContainerId = x.ContainerId,
                    ContainerNumber = x.ContainerNumber,

                    CntActionTypeId = x.CntActionTypeId,
                    ActionTypeName = x.CntActionType != null
                        ? x.CntActionType.CntActionTypeName
                        : x.CntActionTypeId.ToString(),

                    LocationId = x.LocationId,
                    LocationName = x.Location != null
                        ? x.Location.LocationName
                        : null,

                    SubLocationId = x.SubLocationId,
                    SubLocationName = x.SubLocation != null
                        ? x.SubLocation.LocationName
                        : null,

                    VoyageId = x.VoyageId,
                    VoyageNumber = x.Voyage != null
                        ? x.Voyage.VoyageNo
                        : null,

                    VoyageTypeId = x.VoyageTypeId,
                    VoyageTypeName = x.VoyageType != null
                        ? x.VoyageType.VoyageTypeName
                        : null,

                    ContainerActionStatusId = x.ContainerConditionStatusId,
                    ContainerActionStatusName = x.ContainerCondition != null
                        ? x.ContainerCondition.ContainerConditionStatusName
                        : null,


                    CntOwnTypeId = x.cntOwnTypeId,
                    //OwnershipTypeName = x.cntOwnTypeId != null
                    //    ? x.cntOwnTypeId
                    //    : x.cntOwnTypeId.ToString(),

                    FullMTId = x.FullMTId,
                    FullEmptyName = x.FullMT != null
                        ? x.FullMT.FullMTName
                        : x.FullMTId.ToString(),

                    ContainerActionDateTime = x.ContainerActionDateTime,

                    EventActionSourceId = x.EventActionSourceId,
                    EventSourceName = x.EventActionSource != null
                        ? x.EventActionSource.EventActionSourceName
                        : x.EventActionSourceId.ToString(),

                    ContainerActionSourceReference = x.ContainerActionSourceReference,
                    CreatedAt = x.CreatedAt
                })
                .ToListAsync();

            var result = new PagedResultDto<ContainerActionListItemVm>(
                items,
                totalCount,
                pageNumber,
                pageSize
            )
            {
                SearchTerm = searchForm.ContainerNumber,
                SortColumn = "ContainerActionDateTime",
                SortDirection = "desc"
            };

            return new ContainerActionIndexVm
            {
                SearchForm = searchForm,
                Result = result
            };
        }

        private async Task PopulateSearchLookupsAsync(ContainerActionSearchFormVm vm)
        {
            if (_context.CntActionTypes != null)
            {
                vm.ActionTypes = await _context.CntActionTypes
                    .AsNoTracking()
                    .OrderBy(x => x.CntActionTypeName)
                    .Select(x => new SelectListItem
                    {
                        Value = x.Id.ToString(),
                        Text = x.CntActionTypeName
                    })
                    .ToListAsync();
            }

            //if (_context.Locations != null)
            //{
            //    vm.Locations = await _context.Locations
            //        .AsNoTracking()
            //        .OrderBy(x => x.LocationName)
            //        .Select(x => new SelectListItem
            //        {
            //            Value = x.Id.ToString(),
            //            Text = x.LocationName
            //        })
            //        .ToListAsync();

            //    vm.SubLocations = await _context.Locations
            //        .AsNoTracking()
            //        .OrderBy(x => x.LocationName)
            //        .Select(x => new SelectListItem
            //        {
            //            Value = x.Id.ToString(),
            //            Text = x.LocationName
            //        })
            //        .ToListAsync();
            //}

            if (_context.Voyages != null)
            {
                vm.Voyages = await _context.Voyages
                    .AsNoTracking()
                    .OrderBy(x => x.VoyageNo)
                    .Select(x => new SelectListItem
                    {
                        Value = x.Id.ToString(),
                        Text = x.VoyageNo
                    })
                    .ToListAsync();
            }

            if (_context.VoyageTypes != null)
            {
                vm.VoyageTypes = await _context.VoyageTypes
                    .AsNoTracking()
                    .OrderBy(x => x.Id)
                    .Select(x => new SelectListItem
                    {
                        Value = x.Id.ToString(),
                        Text = x.VoyageTypeName
                    })
                    .ToListAsync();
            }

            if (_context.CntOwnTypes != null)
            {
                vm.OwnershipTypes = await _context.CntOwnTypes
                    .AsNoTracking()
                    .OrderBy(x => x.Id)
                    .Select(x => new SelectListItem
                    {
                        Value = x.Id.ToString(),
                        Text = x.CntOwnTypeName
                    })
                    .ToListAsync();
            }

            if (_context.FullMT != null)
            {
                vm.FullEmptyTypes = await _context.FullMT
                    .AsNoTracking()
                    .OrderBy(x => x.Id)
                    .Select(x => new SelectListItem
                    {
                        Value = x.Id.ToString(),
                        Text = x.FullMTName
                    })
                    .ToListAsync();
            }

            if (_context.EventActionSources != null)
            {
                vm.EventSources = await _context.EventActionSources
                    .AsNoTracking()
                    .OrderBy(x => x.Id)
                    .Select(x => new SelectListItem
                    {
                        Value = x.Id.ToString(),
                        Text = x.EventActionSourceName
                    })
                    .ToListAsync();
            }
        }



        public async Task<ReleaseToShipperVm> PrepareReleaseToShipperVmAsync(
            ReleaseToShipperVm? model = null)
        {
            model ??= new ReleaseToShipperVm();

            model.Locations = await _context.Locations
                .AsNoTracking()
                .OrderBy(x => x.LocationName)
                .Select(x => new SelectListItem
                {
                    Value = x.Id.ToString(),
                    Text = x.LocationName
                })
                .ToListAsync();

            model.SubLocations = await _context.Locations
                .AsNoTracking()
                .OrderBy(x => x.LocationName)
                .Select(x => new SelectListItem
                {
                    Value = x.Id.ToString(),
                    Text = x.LocationName
                })
                .ToListAsync();

            model.Voyages = await _context.Voyages
                .AsNoTracking()
                .OrderByDescending(x => x.CreatedAt)
                .Select(x => new SelectListItem
                {
                    Value = x.Id.ToString(),
                    Text = x.VoyageNo
                })
                .ToListAsync();

            model.OwnershipTypes = await _context.CntOwnTypes
                .AsNoTracking()
                .OrderBy(x => x.CntOwnTypeName)
                .Select(x => new SelectListItem
                {
                    Value = x.Id.ToString(),
                    Text = x.CntOwnTypeName
                })
                .ToListAsync();

            model.FullEmptyTypes = await _context.FullMT
                .AsNoTracking()
                .OrderBy(x => x.FullMTName)
                .Select(x => new SelectListItem
                {
                    Value = x.Id.ToString(),
                    Text = x.FullMTName
                })
                .ToListAsync();

            return model;
        }




        public async Task<LoadOnBoardVm> PrepareLoadOnBoardVmAsync(LoadOnBoardVm? model = null)
        {
            model ??= new LoadOnBoardVm();
            model.Voyages = await _context.Voyages
                .AsNoTracking()
                .OrderByDescending(x => x.CreatedAt)
                .Select(x => new SelectListItem
                {
                    Value = x.Id.ToString(),
                    Text = x.VoyageNo
                })
                .ToListAsync();

            model.OwnershipTypes = await _context.CntOwnTypes
                .AsNoTracking()
                .OrderBy(x => x.CntOwnTypeName)
                .Select(x => new SelectListItem
                {
                    Value = x.Id.ToString(),
                    Text = x.CntOwnTypeName
                })
                .ToListAsync();

            model.FullEmptyTypes = await _context.FullMT
                .AsNoTracking()
                .OrderBy(x => x.FullMTName)
                .Select(x => new SelectListItem
                {
                    Value = x.Id.ToString(),
                    Text = x.FullMTName
                })
                .ToListAsync();

            return model;
        }


        public async Task<DischargeVm> PrepareDischargeVmAsync(DischargeVm? model = null)
        {
            model ??= new DischargeVm();

            model.Voyages = await _context.Voyages
                .AsNoTracking()
                .OrderByDescending(x => x.CreatedAt)
                .Select(x => new SelectListItem
                {
                    Value = x.Id.ToString(),
                    Text = x.VoyageNo
                })
                .ToListAsync();

            model.OwnershipTypes = await _context.CntOwnTypes
                .AsNoTracking()
                .OrderBy(x => x.CntOwnTypeName)
                .Select(x => new SelectListItem
                {
                    Value = x.Id.ToString(),
                    Text = x.CntOwnTypeName
                })
                .ToListAsync();

            model.FullEmptyTypes = await _context.FullMT
                .AsNoTracking()
                .OrderBy(x => x.FullMTName)
                .Select(x => new SelectListItem
                {
                    Value = x.Id.ToString(),
                    Text = x.FullMTName
                })
                .ToListAsync();

            return model;
        }


        public async Task ReleaseToShipperAsync(ReleaseToShipperVm model)
        {
            var containerNumber = model.ContainerNumber.Trim();

            var container = await _context.Containers
                .AsNoTracking()
                .FirstOrDefaultAsync(x => x.ContainerNumber == containerNumber);

            if (container == null)
            {
                throw new InvalidOperationException("Container number was not found.");
            }

            var releaseToShipperActionType = await _context.CntActionTypes
                .AsNoTracking()
                .FirstOrDefaultAsync(x => x.CntActionTypeName == "Release To Shipper");

            if (releaseToShipperActionType == null)
            {
                throw new InvalidOperationException("Action type 'Release To Shipper' was not found.");
            }

            var manualEventSource = await _context.EventActionSources
                .AsNoTracking()
                .FirstOrDefaultAsync(x => x.EventActionSourceName == "Manual" || x.EventActionSourceName == "User");

            if (manualEventSource == null)
            {
                throw new InvalidOperationException("Manual event source was not found.");
            }

            var entity = new ContainerAction
            {
                Id = Guid.CreateVersion7(),

                CompanyId = 1,
                ContainerId = container.Id,
                ContainerNumber = container.ContainerNumber,

                CntActionTypeId = releaseToShipperActionType.Id,

                LocationId = model.LocationId,
                SubLocationId = model.SubLocationId,
                VoyageId = model.VoyageId,

                cntOwnTypeId = model.CntOwnTypeId!.Value,
                FullMTId = model.FullMTId!.Value,

                ContainerActionDateTime = model.ContainerActionDateTime,

                EventActionSourceId = manualEventSource.Id,
                ContainerActionSourceReference = model.ContainerActionSourceReference,

                CreatedAt = DateTime.Now,
                CreateUserId = _currentUserService.UserId,
                IsDeleted = false
            };

            _context.ContainerActions.Add(entity);

            await _context.SaveChangesAsync();
        }




        public async Task LoadOnBoardAsync(LoadOnBoardVm model)
        {
            var containerNumber = model.ContainerNumber.Trim();
            var container = await _context.Containers
                .AsNoTracking()
                .FirstOrDefaultAsync(x => x.ContainerNumber == containerNumber);

            if (container == null)
            {
                throw new InvalidOperationException("Container number was not found.");
            }


            var entity = new ContainerAction
            {
                Id = Guid.CreateVersion7(),

                CompanyId = 1,
                ContainerId = container.Id,
                ContainerNumber = container.ContainerNumber,
                CntActionTypeId = 3,  //Load On Board

                LocationId = model.LocationId,
                SubLocationId = model.SubLocationId,
                VoyageId = model.VoyageId,

                cntOwnTypeId = model.CntOwnTypeId!.Value,
                FullMTId = model.FullMTId!.Value,

                ContainerActionDateTime = model.ContainerActionDateTime,

                EventActionSourceId = 1, //Manual
                ContainerActionSourceReference = model.ContainerActionSourceReference,

                CreatedAt = DateTime.Now,
                CreateUserId = _currentUserService.UserId,
                IsDeleted = false
            };

            _context.ContainerActions.Add(entity);

            await _context.SaveChangesAsync();
        }


        public async Task DischargeAsync(DischargeVm model)
        {
            var containerNumber = model.ContainerNumber.Trim();
            var container = await _context.Containers
                .AsNoTracking()
                .FirstOrDefaultAsync(x => x.ContainerNumber == containerNumber);

            if (container == null)
            {
                throw new InvalidOperationException("Container number was not found.");
            }


            var entity = new ContainerAction
            {
                Id = Guid.CreateVersion7(),

                CompanyId = 1,
                ContainerId = container.Id,
                ContainerNumber = container.ContainerNumber,
                CntActionTypeId = 4,  //Discharge

                LocationId = model.LocationId,
                SubLocationId = model.SubLocationId,
                VoyageId = model.VoyageId,

                cntOwnTypeId = model.CntOwnTypeId!.Value,
                FullMTId = model.FullMTId!.Value,

                ContainerActionDateTime = model.ContainerActionDateTime,

                EventActionSourceId = 1, //Manual
                ContainerActionSourceReference = model.ContainerActionSourceReference,

                CreatedAt = DateTime.Now,
                CreateUserId = _currentUserService.UserId,
                IsDeleted = false
            };

            _context.ContainerActions.Add(entity);

            await _context.SaveChangesAsync();
        }
        public async Task<RtsDeliverySummaryVm> GetRtsDeliverySummaryAsync(string rtsNo)
        {
            if (string.IsNullOrWhiteSpace(rtsNo)) return null;

            rtsNo = rtsNo.Trim().ToUpperInvariant();

            var rts = await _context.RTSInstructions
                .Include(r => r.Shipper)
                .Include(r => r.Depot)
                .FirstOrDefaultAsync(r => r.RTSInstructionNo == rtsNo && !r.IsDeleted);

            if (rts == null) return null;

            var summary = new RtsDeliverySummaryVm
            {
                RTSInstructionId = rts.Id,
                BookingId = rts.BookingId,
                RTSInstructionNo = rts.RTSInstructionNo,
                ShipperName = rts.Shipper?.PersonName ?? "N/A",
                DepotName = rts.Depot?.LocationName ?? "N/A",
                ValidDateTo = rts.ValidDateTo
            };

            // واکشی رکوردهای مجاز بر اساس نوع کانتینر در RTS
            var allowedTypes = await _context.RTSInstructionContainerTypes
                .Include(ct => ct.ContainerSizeType)
                .Where(ct => ct.RTSInstructionId == rts.Id && !ct.IsDeleted)
                .ToListAsync();

            // واکشی کانتینرهایی که قبلاً برای این RTS خارج شده‌اند (Gate Out Empty - CntActionTypeId = 1)
            var deliveredActions = await _context.ContainerActions
                .Include(a => a.ContainerUse)
                .ThenInclude(u => u.ContainerSizeType)
                .Where(a => a.ContainerActionSourceReference == rtsNo
                            && a.CntActionTypeId == 1
                            && !a.IsDeleted)
                .OrderByDescending(a => a.ContainerActionDateTime)
                .ToListAsync();

            foreach (var allowed in allowedTypes)
            {
                var deliveredCount = deliveredActions.Count(a =>
                    a.ContainerUse?.ContainerSizeTypeId == allowed.ContainerSizeTypeId);

                summary.AllowedContainers.Add(new RtsDeliveryItemStatusVm
                {
                    ContainerSizeTypeId = allowed.ContainerSizeTypeId,
                    ContainerSizeTypeName = allowed.ContainerSizeType?.ShortName ?? "N/A",
                    AllowedCount = allowed.ContainerCounts,
                    DeliveredCount = deliveredCount
                });
            }

            summary.AlreadyDelivered = deliveredActions.Select(a => new DeliveredContainerVm
            {
                ContainerNumber = a.ContainerNumber,
                SealNumber = a.ContainerUse?.SealNumber ?? "-",
                ContainerSizeTypeName = a.ContainerUse?.ContainerSizeType?.ShortName ?? "N/A",
                ActionDateTime = a.ContainerActionDateTime
            }).ToList();

            return summary;
        }

        public async Task<(bool Success, string Message)> IssueContainerAsync(IssueContainerCommand command, int companyId)
        {
            if (command == null) return (false, "Invalid request.");

            command.ContainerNumber = command.ContainerNumber?.Trim().ToUpperInvariant();
            if (string.IsNullOrWhiteSpace(command.ContainerNumber)) return (false, "Container number is required.");

            if (!Iso6346.ValidateContainerNumber(command.ContainerNumber))
            {
                return (false, $"Container number '{command.ContainerNumber}' is not a valid ISO 6346 number.");
            }

            await using var transaction = await _context.Database.BeginTransactionAsync();
            try
            {
                var rts = await _context.RTSInstructions
                    .FirstOrDefaultAsync(r => r.Id == command.RTSInstructionId && !r.IsDeleted);

                if (rts == null) return (false, "RTS instruction not found.");

                if (rts.ValidDateTo.HasValue && rts.ValidDateTo.Value.Date < DateTime.Today)
                {
                    return (false, "The RTS instruction validity period has expired.");
                }

                var allowedType = await _context.RTSInstructionContainerTypes
                    .FirstOrDefaultAsync(x => x.RTSInstructionId == command.RTSInstructionId
                                              && x.ContainerSizeTypeId == command.ContainerSizeTypeId
                                              && !x.IsDeleted);

                if (allowedType == null)
                {
                    return (false, "Selected container type is not allowed for this RTS.");
                }

                var currentDeliveredCount = await _context.ContainerActions
                    .Include(x => x.ContainerUse)
                    .CountAsync(x => x.ContainerActionSourceReference == rts.RTSInstructionNo
                                     && x.CntActionTypeId == 1
                                     && x.ContainerUse.ContainerSizeTypeId == command.ContainerSizeTypeId
                                     && !x.IsDeleted);

                if (currentDeliveredCount >= allowedType.ContainerCounts)
                {
                    return (false, "Delivery quantity limit reached for this container type.");
                }

                // بررسی جلوگیری از خروج مجدد کانتینر فعال با چرخه باز
                var container = await _context.Containers
                    .FirstOrDefaultAsync(x => x.ContainerNumber == command.ContainerNumber && !x.IsDeleted);

                if (container != null)
                {
                    var activeCycleExists = await _context.ContainerUses
                        .AnyAsync(x => x.ContainerId == container.Id && !x.CycleClosed && !x.IsDeleted);

                    if (activeCycleExists)
                    {
                        return (false, $"Container '{command.ContainerNumber}' already has an active open cycle.");
                    }
                }
                else
                {
                    container = new Container
                    {
                        Id = Guid.CreateVersion7(),
                        ContainerNumber = command.ContainerNumber,
                        ContainerSizeTypeId = command.ContainerSizeTypeId,
                        IsDeleted = false
                    };
                    _context.Containers.Add(container);
                }

                var now = DateTime.Now;

                var containerUse = new ContainerUse
                {
                    Id = Guid.CreateVersion7(),
                    ContainerId = container.Id,
                    CompanyId = companyId,
                    ContainerNumber = command.ContainerNumber,
                    SealNumber = command.SealNumber?.Trim().ToUpperInvariant(),
                    ContainerSizeTypeId = command.ContainerSizeTypeId,
                    TareWeight = (long)command.TareWeight,
                    GrossWeight = (long)command.GrossWeight,
                    CycleClosed = false,
                    CntOwnTypeId = 1,
                    FullMTId = 4, // MT (Empty)
                    IsDeleted = false,
                    CreatedAt = DateTime.Now,
                    CreateUserId = _currentUserService.UserId,
                    RTSInstructionId = command.RTSInstructionId,
                    BookingId = command.BookingId
                };
                _context.ContainerUses.Add(containerUse);

                var containerAction = new ContainerAction
                {
                    Id = Guid.CreateVersion7(),
                    CompanyId = companyId,
                    ContainerId = container.Id,
                    ContainerUseId = containerUse.Id,
                    ContainerNumber = command.ContainerNumber,
                    CntActionTypeId = 1, // Gate Out Empty
                    LocationId = rts.DepotId,
                    FullMTId = 4,
                    ContainerActionDateTime = command.ActionDateTime.Value,
                    ContainerActionSourceReference = rts.RTSInstructionNo,
                    IsDeleted = false,
                    EventActionSourceId = 1, //Manual
                    CreatedAt = DateTime.Now,
                    CreateUserId = _currentUserService.UserId,
                    RTSInstructionId = command.RTSInstructionId,
                    BookingId = command.BookingId,
                };
                _context.ContainerActions.Add(containerAction);
                if (!string.IsNullOrWhiteSpace(command.DriverName))
                {
                    _context.ContainerActionAttributes.Add(new ContainerActionAttribute
                    {
                        Id = Guid.CreateVersion7(),
                        ContainerActionId = containerAction.Id,
                        AttributeKey = "DriverName",
                        AttributeValue = command.DriverName.Trim(),
                        AttributeValueType = "string",
                        CreatedAt = DateTime.Now,
                        CreateUserId = _currentUserService.UserId,
                    });
                }
                if (!string.IsNullOrWhiteSpace(command.DriverPlate))
                {
                    _context.ContainerActionAttributes.Add(new ContainerActionAttribute
                    {
                        Id = Guid.CreateVersion7(),
                        ContainerActionId = containerAction.Id,
                        AttributeKey = "DriverPlate",
                        AttributeValue = command.DriverPlate.Trim().ToUpperInvariant(),
                        AttributeValueType = "string",
                        CreatedAt = DateTime.Now,
                        CreateUserId = _currentUserService.UserId,
                    });
                }
                await _context.SaveChangesAsync();
                await transaction.CommitAsync();

                return (true, "Container issued successfully.");
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                return (false, $"An error occurred during save: {ex.Message}");
            }
        }
        public async Task DTCAsync(Guid doId)
        {
            var data = await _context.DeliveryOrders
                .Include(x => x.BL)
                .ThenInclude(x => x.BLContainers)
                .Where(x => x.Id == doId)
                .Select(x => new
                {
                    x.DONo,
                    x.DoDate,
                    x.CompanyId,
                    Containers = x.BL.BLContainers.Select(c => new
                    {
                        c.ContainerNumber,
                        c.ContainerUseId,
                        c.FullMTId
                    }).ToList()
                }).FirstOrDefaultAsync();
            if (data != null)
            {
                foreach (var cnt in data.Containers)
                {
                    var containerAction = new ContainerAction
                    {
                        Id = Guid.CreateVersion7(),
                        CompanyId = data.CompanyId,  //درست شود
                        ContainerId = GetContainerId(cnt.ContainerNumber),
                        ContainerUseId = cnt.ContainerUseId,
                        ContainerNumber = cnt.ContainerNumber,
                        CntActionTypeId = 5, // Gate Out Empty
                        FullMTId = cnt.FullMTId.Value,
                        ContainerActionDateTime = data.DoDate.Value,
                        ContainerActionSourceReference = data.DONo,
                        IsDeleted = false,
                        EventActionSourceId = 3, //Manual
                        CreatedAt = DateTime.Now,
                        CreateUserId = _currentUserService.UserId,
                    };
                    _context.ContainerActions.Add(containerAction);
                }
                await _context.SaveChangesAsync();
            }
        }
        private Guid GetContainerId(string ContainerNumber)
        {
            return _context.Containers.Where(x => x.ContainerNumber == ContainerNumber).FirstOrDefault().Id;
        }
        public async Task<RBCVm> PrepareRBCVmAsync(RBCVm? model = null)
        {
            model ??= new RBCVm();

            model.ContainerConditions = await _context.ContainerConditionStatuses
                .AsNoTracking()
                .OrderBy(x => x.Id)
                .Select(x => new SelectListItem
                {
                    Value = x.Id.ToString(),
                    Text = x.ContainerConditionStatusName
                })
                .ToListAsync();
            return model;
        }

        public async Task RBCAsync(RBCVm model)
        {
            var containerNumber = model.ContainerNumber.Trim();
            var container = await _context.Containers
                .AsNoTracking()
                .FirstOrDefaultAsync(x => x.ContainerNumber == containerNumber);

            if (container == null)
            {
                throw new InvalidOperationException("Container number was not found.");
            }


            var entity = new ContainerAction
            {
                Id = Guid.CreateVersion7(),

                CompanyId = 1,
                ContainerId = container.Id,
                ContainerNumber = container.ContainerNumber,
                CntActionTypeId = 6,  //RBC

                LocationId = model.LocationId,
                SubLocationId = model.SubLocationId,
                ContainerConditionStatusId = model.ContainerConditionId,
                ContainerActionDateTime = model.ContainerActionDateTime,
                FullMTId = 4,
                EventActionSourceId = 1, //Manual
                ContainerActionSourceReference = model.ContainerActionSourceReference,
                CreatedAt = DateTime.Now,
                CreateUserId = _currentUserService.UserId,
                IsDeleted = false
            };
            _context.ContainerActions.Add(entity);
            await _context.SaveChangesAsync();
        }

        private async Task<Guid> GetFinalPortIdForBLAsync(Guid blId)
        {
            var bl = await _context.BLs
                .Include(x => x.PODel)
                .Include(x => x.POD)
                .Where(x => x.Id == blId).FirstOrDefaultAsync();

            if (bl.PODel != null)
            {
                if (bl.PODel.CityTypeId == (int)EnumCityType.Port || bl.PODel.CityTypeId == (int)EnumCityType.CityAndPort)
                {
                    return bl.PODelId.Value;
                }
            }
            if (bl.POD != null)
            {
                if (bl.POD.CityTypeId == (int)EnumCityType.Port || bl.POD.CityTypeId == (int)EnumCityType.CityAndPort)
                {
                    return bl.PODId.Value;
                }
            }
            return Guid.Empty;
        }

        public async Task<bool> RegisterDischargeActionsForFinalPortVoyageAsync(Guid voyagePortId)
        {
            var discompletedEvent = await _context.VoyagePortEvents.Where(x => x.VoyagePortId == voyagePortId && x.EventTypeId == 121).FirstOrDefaultAsync();
            if (discompletedEvent == null) return false;
            var voyagePort = await _context.VoyagePorts.FindAsync(voyagePortId);
            if (voyagePort == null) return false;
            var voyagePorDischargeCompleteEvent = await _context.VoyagePortEvents.Where(x => x.VoyagePortId == voyagePortId && x.EventTypeId == 121).FirstOrDefaultAsync();
            if (voyagePorDischargeCompleteEvent == null) return false;
            var voyageId = voyagePort.InboundVoyageId;
            var cnts = await _context.BLContainers
                     .Include(x => x.BL)
                     .ThenInclude(x => x.BLVoyages)
                     .Where(x => x.BL.BLVoyages.Any(x => x.IsFinal == true && x.VoyageId == voyageId)).ToListAsync();
            foreach (var item in cnts)
            {
                if (!(await GetFinalPortIdForBLAsync(item.BL.Id) == voyagePort.PortId)) continue;
                var existingRecs = await _context
                    .ContainerActions
                    .Where(x => x.ContainerUseId == item.ContainerUseId && x.ContainerNumber == item.ContainerNumber && x.CntActionTypeId == 4 && x.LocationId == voyagePort.PortId)
                    .ToListAsync();
                if (existingRecs.Any()) continue;

                var entity = new ContainerAction
                {
                    Id = Guid.CreateVersion7(),
                    CompanyId = item.BL.CompanyId,
                    ContainerUseId = item.ContainerUseId,
                    ContainerId = await _containerService.GetContainerIdAsync(item.ContainerNumber),
                    ContainerNumber = item.ContainerNumber,
                    CntActionTypeId = 4,  //Discharge
                    LocationId = voyagePort.PortId,
                    VoyageId = voyageId,
                    cntOwnTypeId = item.CntOwnTypeId,
                    FullMTId = item.FullMTId!.Value,

                    ContainerActionDateTime = voyagePorDischargeCompleteEvent.VoyagePortEventDateTime,
                    EventActionSourceId = 3, //System
                    ContainerActionSourceReference = "Voyage DisCompelte",
                    CreatedAt = DateTime.Now,
                    CreateUserId = _currentUserService.UserId,
                    IsDeleted = false
                };
                _context.ContainerActions.Add(entity);
                await _context.SaveChangesAsync();
            }
            return true;
        }

        public async Task<RtsDeliverySummaryVm> GetRtsDeliverySummaryByInstructionIdAsync(Guid rtsInstructionId)
        {
            var rts = await _context.RTSInstructions
                .Include(r => r.Shipper)
                .Include(r => r.Depot)
                .FirstOrDefaultAsync(r => r.Id == rtsInstructionId && !r.IsDeleted);

            if (rts == null) return null;

            var summary = new RtsDeliverySummaryVm
            {
                RTSInstructionId = rts.Id,
                BookingId = rts.BookingId,
                RTSInstructionNo = rts.RTSInstructionNo,
                ShipperName = rts.Shipper?.PersonName ?? "N/A",
                DepotName = rts.Depot?.LocationName ?? "N/A",
                ValidDateTo = rts.ValidDateTo
            };

            // واکشی رکوردهای مجاز بر اساس نوع کانتینر در RTS
            var allowedTypes = await _context.RTSInstructionContainerTypes
                .Include(ct => ct.ContainerSizeType)
                .Where(ct => ct.RTSInstructionId == rts.Id && !ct.IsDeleted)
                .ToListAsync();

            // واکشی کانتینرهایی که قبلاً برای این RTS خارج شده‌اند (Gate Out Empty - CntActionTypeId = 1)
            var deliveredActions = await _context.ContainerActions
                .Include(a => a.ContainerUse)
                .ThenInclude(u => u.ContainerSizeType)
                .Where(a => a.RTSInstructionId == rtsInstructionId
                            && a.CntActionTypeId == 1
                            && !a.IsDeleted)
                .OrderByDescending(a => a.ContainerActionDateTime)
                .ToListAsync();

            foreach (var allowed in allowedTypes)
            {
                var deliveredCount = deliveredActions.Count(a =>
                    a.ContainerUse?.ContainerSizeTypeId == allowed.ContainerSizeTypeId);

                summary.AllowedContainers.Add(new RtsDeliveryItemStatusVm
                {
                    ContainerSizeTypeId = allowed.ContainerSizeTypeId,
                    ContainerSizeTypeName = allowed.ContainerSizeType?.ShortName ?? "N/A",
                    AllowedCount = allowed.ContainerCounts,
                    DeliveredCount = deliveredCount
                });
            }

            summary.AlreadyDelivered = deliveredActions.Select(a => new DeliveredContainerVm
            {
                ContainerNumber = a.ContainerNumber,
                SealNumber = a.ContainerUse?.SealNumber ?? "-",
                ContainerSizeTypeName = a.ContainerUse?.ContainerSizeType?.ShortName ?? "N/A",
                ActionDateTime = a.ContainerActionDateTime
            }).ToList();

            return summary;
        }
        public async Task<bool> RBSReceiptAsync(ExportDocsReceipt receipt)
        {
            foreach (var item in receipt.Containers)
            {
                var cnt = await _containerService.GetContainerAsync(item.ContainerId.Value);
                var openUseId = await _containerService.GetLastOpenCycleContainerUseIdForContainer(item.ContainerId.Value);
                if (openUseId == Guid.Empty) return false;

                var entity = new ContainerAction
                {
                    Id = Guid.CreateVersion7(),
                    CompanyId = receipt.CompanyId,
                    ContainerId = item.ContainerId.Value,
                    ContainerNumber = cnt.ContainerNumber,
                    CntActionTypeId = 2,  //RBS
                    cntOwnTypeId = cnt.CntOwnTypeId,
                    //LocationId = model.LocationId,
                    //SubLocationId = model.SubLocationId,
                    //VoyageId = model.VoyageId,
                    FullMTId = 5,
                    ContainerActionDateTime = item.RBSDate.Value,
                    ContainerUseId = openUseId,
                    EventActionSourceId = 3,
                    ContainerActionSourceReference = $"Receipt No:{receipt.ReceiptNo}",
                    CreatedAt = DateTime.Now,
                    CreateUserId = _currentUserService.UserId,
                    IsDeleted = false
                };
                _context.ContainerActions.Add(entity);
            }
            return true;
        }
    }
}