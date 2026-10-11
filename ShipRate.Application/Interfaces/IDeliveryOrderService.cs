using FastReport;
using FastReport.Export.PdfSimple;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using ShipRate.Application.Interfaces.ContainerActions;
using ShipRate.Application.ViewModels;
using ShipRate.Domain.Entities.BillOfLading;
using ShipRate.Domain.Entities.DocumentStore;
using ShipRate.Infrastructure.Data;
using System.Data;
using DocumentAttachmentDto = ShipRate.Application.ViewModels.DocumentAttachmentDto;

namespace ShipRate.Application.Interfaces
{
    public interface IDeliveryOrderService
    {
        Task<CreateDeliveryOrderPageVm> GetCreatePageAsync(Guid? blId, CancellationToken cancellationToken = default);
        Task<CreateDeliveryOrderPageVm> GetEditPageVmAsync(Guid id, CancellationToken cancellationToken = default);
        Task FillCreatePageLookupsAsync(CreateDeliveryOrderPageVm vm, CancellationToken cancellationToken = default);
        Task<List<DeliveryOrderBLLookupItemVm>> SearchBLAsync(string? term, CancellationToken cancellationToken = default);
        Task<DeliveryOrderBLSummaryVm?> GetBLSummaryAsync(Guid blId, CancellationToken cancellationToken = default);
        Task<SaveDeliveryOrderDraftResultVm> SaveDraftAsync(SaveDeliveryOrderDraftRequestVm request, CancellationToken cancellationToken = default);
        Task SaveTIPUsageAsync(Guid tipId, Guid blId, CancellationToken cancellationToken = default);
        Task SyncAllContainersFromBlAsync(Guid deliveryOrderId, Guid blId, CancellationToken cancellationToken = default);
        Task<List<DeliveryOrderContainerVm>> GetDeliveryOrderContainersAsync(Guid deliveryOrderId, CancellationToken cancellationToken = default);
        Task<List<DeliveryOrderCargoVm>> GetDeliveryOrderCargoAsync(Guid deliveryOrderId, CancellationToken cancellationToken);
        Task<DeliveryOrderDetailsVm?> GetDetailsAsync(Guid deliveryOrderId, CancellationToken cancellationToken);
        Task<DeliveryOrderListWrapperVm> GetPagedListAsync(DeliveryOrderSearchFilter filter, CancellationToken cancellationToken);
        Task<FinalizeDeliveryOrderResultVm> FinalizeAsync(FinalizeDeliveryOrderRequestVm request, CancellationToken cancellationToken);
        Task<FinalizeDeliveryOrderResultVm> ApproveAsync(FinalizeDeliveryOrderRequestVm request, CancellationToken cancellationToken);
        Task<FinalizeDeliveryOrderResultVm> CollectedAsync(FinalizeDeliveryOrderRequestVm request, CancellationToken cancellationToken);
        Task<FinalizeDeliveryOrderResultVm> ReleaseAsync(FinalizeDeliveryOrderRequestVm request, CancellationToken cancellationToken);
        Task<bool> DeleteDraftAsync(Guid deliveryOrderId, CancellationToken cancellationToken);
        Task<bool> CancelDeliveryOrderAsync(Guid id, string reason, CancellationToken cancellationToken);
        Task<DeliveryOrderDetentionsVm?> GetDetentionsByBlAsync(Guid blId, CancellationToken cancellationToken);
        Task<DeliveryOrderDocumentPanelVm> BuildDocumentPanelAsync(Guid deliveryOrderId, CancellationToken cancellationToken = default);
        Task<DeliveryOrderDocumentCardVm> UploadDocumentFilesAsync(Guid deliveryOrderId, int documentTypeId, List<IFormFile> files, int currentCompanyId, Guid currentUserId, CancellationToken cancellationToken = default);
        Task<DocumentAttachmentDto?> GetAttachmentByIdAsync(Guid id, CancellationToken cancellationToken = default);
        Task<List<ChargeRowVm>> GetChargesByBlIdAsync(Guid blId);
        Task<List<InvoiceRowVm>> GetInvoicesByBlIdAsync(Guid blId);
        byte[] GenerateDeliveryOrderPdf(DeliveryOrderDetailsVm vm, string frxPath);
        Task<bool> ApproveAsync(Guid id, Guid userId, CancellationToken cancellationToken);
        Task<int> GetNewDeliveryOrderNoAsync(int CompanyId, int doOperationalModeId, CancellationToken cancellationToken = default);

        Task<FreightLetterUpsertVm> GetFreightLetterUpsertVmAsync(Guid doId, CancellationToken ct);
        Task<FreightLetterUpsertVm> UpsertFreightLetterAsync(FreightLetterUpsertVm vm, CancellationToken ct);
        Task<byte[]> GenerateFreightLetterPdfAsync(Guid doId, string reportTemplatePath, CancellationToken ct);


    }
    public class DeliveryOrderService : IDeliveryOrderService
    {
        private readonly ApplicationDbContext _db;
        private readonly ICurrentUserService _currentUserService;
        private readonly IContainerActionService _containerActionService;
        private readonly ITelexReleaseService _telexReleaseService;
        private readonly ITransportDocumentService _transportDocumentService;
        private readonly ITipService _tipService;

        public DeliveryOrderService(ApplicationDbContext db, ICurrentUserService currentUserService, IContainerActionService containerActionService, ITelexReleaseService telexReleaseService, ITransportDocumentService transportDocumentService, ITipService tipService)
        {
            _db = db;
            _currentUserService = currentUserService;
            _containerActionService = containerActionService;
            _telexReleaseService = telexReleaseService;
            _transportDocumentService = transportDocumentService;
            _tipService = tipService;
        }

        public async Task<DeliveryOrderListWrapperVm> GetPagedListAsync(
            DeliveryOrderSearchFilter filter,
            CancellationToken cancellationToken)
        {
            var query = _db.DeliveryOrders.AsNoTracking().AsQueryable();

            // فیلتر متنی (جستجو در شماره ترخیصیه یا شماره بارنامه از طریق ارتباط با جدول BL)
            if (!string.IsNullOrWhiteSpace(filter.SearchTerm))
            {
                var term = filter.SearchTerm.Trim().ToLower();
                query = query.Where(x =>
                    (x.DONo != null && x.DONo.ToLower().Contains(term)) ||
                    (x.BL != null && x.BL.BLNo != null && x.BL.BLNo.ToLower().Contains(term)));
            }

            // فیلتر وضعیت نهایی‌سازی
            if (filter.StatusId.HasValue)
            {
                query = query.Where(x => x.StatusId == filter.StatusId.Value);
            }

            if (filter.BLId != null && filter.BLId.HasValue && filter.BLId != Guid.Empty)
            {
                query = query.Where(x => x.BLId == filter.BLId.Value);
            }


            if (filter.ClearanceTypeId.HasValue)
            {
                query = query.Where(x => x.ClearanceTypeId == filter.ClearanceTypeId.Value);
            }

            if (filter.CarrierId.HasValue && filter.CarrierId != Guid.Empty)
            {
                query = query.Where(x => x.CarrierId == filter.CarrierId.Value);
            }

            // فیلتر تاریخ
            if (filter.FromDate.HasValue)
            {
                query = query.Where(x => x.DoDate >= filter.FromDate.Value);
            }
            if (filter.ToDate.HasValue)
            {
                query = query.Where(x => x.DoDate <= filter.ToDate.Value);
            }

            filter.Statuses = await GetDOStatusesAsync(cancellationToken);
            filter.ClearanceTypes = await GetClearanceTypesAsync(cancellationToken);
            filter.Carriers = await GetCarriersAsync(cancellationToken);
            var totalItems = await query.CountAsync(cancellationToken);
            var items = await query
                .OrderByDescending(x => x.CreatedAt) // یا بر اساس تاریخ ترخیصیه
                .Skip((filter.Page - 1) * filter.PageSize)
                .Take(filter.PageSize)
                .Select(x => new DeliveryOrderListItemVm
                {
                    DeliveryOrderId = x.Id,
                    DONo = x.DONo,
                    DoDate = x.DoDate,
                    ExpiryDate = x.ExpiryDate,
                    BLNo = x.BL != null ? x.BL.BLNo : null,
                    //ConsigneeName = x.BL != null && x.BL.Consignee != null ? x.BL.Consignee.Name : null,
                    ClearanceTypeName = x.ClearanceType != null ? x.ClearanceType.ClearanceTypeName : null,
                    //IsFinalized = x.IsFinalized,
                    //FinalizedAt = x.FinalizedAt,
                    ContainerCount = _db.DeliveryOrderContainers.Count(c => c.DeliveryOrderId == x.Id),
                    CarrierName = x.Carrier.Person.PersonName,
                    StatusName = x.Status.DOStatusName,
                    StatusId = x.StatusId,
                    DOOperationModeId = x.DOOperationalModeId,
                    BLId = x.BLId,
                    UpdateAT = x.UpdatedAt,
                    CreatedAT = x.CreatedAt,
                    CreateUserName = x.CreatedByUser.FullName,
                    UpdateUserName = x.EditedByUser == null ? null : x.EditedByUser.FullName,
                    Serial = x.Serial,
                    DOOperationModeName = x.DOOperationalMode.DOOperationalModeName,
                    DOOperationModeLocalName = x.DOOperationalMode.DOOperationalModeLocalName,
                    ClearanceTypeId = x.ClearanceTypeId.Value
                })
                .ToListAsync(cancellationToken);

            return new DeliveryOrderListWrapperVm
            {
                Items = items,
                Filter = filter,
                TotalItems = totalItems
            };
        }
        public async Task<CreateDeliveryOrderPageVm> GetCreatePageAsync(Guid? blId, CancellationToken cancellationToken = default)
        {
            var vm = new CreateDeliveryOrderPageVm
            {
                DoDate = DateTime.Today,
                OriginalBLCount = 0,
                BLId = blId
            };
            var bl = await _transportDocumentService.GetFinalVoyageForBLAsync(blId.Value);
            vm.ArrivalDate = bl.ArrivalDate;

            await FillCreatePageLookupsAsync(vm, cancellationToken);

            return vm;
        }

        public async Task<List<DeliveryOrderBLLookupItemVm>> SearchBLAsync(string? term, CancellationToken cancellationToken = default)
        {
            term = (term ?? string.Empty).Trim();

            var query = _db.BLs
                .AsNoTracking()
                .Where(x => !x.IsDeleted && !_db.DeliveryOrders.Any(d => d.BLId == x.Id && !d.IsDeleted && d.StatusId != 99));

            if (!string.IsNullOrWhiteSpace(term))
            {
                query = query.Where(x =>
                    x.BLNo.Contains(term));
                //
                //
                //||
                //(x.ReferenceNo != null && x.ReferenceNo.Contains(term)) ||
                //(x.BookingNo != null && x.BookingNo.Contains(term)));
            }

            return await query
                .OrderByDescending(x => x.CreatedAt)
                .Take(20)
                .Select(x => new DeliveryOrderBLLookupItemVm
                {
                    BLId = x.Id,
                    BLNo = x.BLNo,
                    //ReferenceNo = x.ReferenceNo,
                    //BookingNo = x.BookingNo,
                    CarrierName = x.Carrier != null ? x.Carrier.Person.PersonName : string.Empty,
                    ConsigneeName = x.BLParties
                        .Where(p => p.RoleTypeId == 52) // adjust if your consignee type id is different
                        .Select(p => p.Party.PersonName)
                        .FirstOrDefault() ?? string.Empty
                })
                .ToListAsync(cancellationToken);
        }

        public async Task<DeliveryOrderBLSummaryVm?> GetBLSummaryAsync(Guid blId, CancellationToken cancellationToken = default)
        {
            var vm = await _db.BLs
                .Include(x => x.BLContainers)
                .AsNoTracking()
                .Where(x => x.Id == blId && !x.IsDeleted)
                .Select(x => new DeliveryOrderBLSummaryVm
                {
                    BLId = x.Id,
                    BLNo = x.BLNo,
                    //ReferenceNo = x.ReferenceNo,
                    //BookingNo = x.BookingNo,
                    CarrierName = x.Carrier != null ? x.Carrier.Person.PersonName : string.Empty,
                    //VesselName = x.VesselName,
                    //VoyageNumber = x.VoyageNumber,
                    POL = x.POL != null ? x.POL.LocationName : string.Empty,
                    POD = x.POD != null ? x.POD.LocationName : string.Empty,
                    IssueDate = x.BLIssueDate,
                    GrossWeightKg = x.GrossWeightKg,
                    PackageCount = x.TotalPackages,
                    ContainerCount = x.BLContainers.Count(),
                    Containers = x.BLContainers
                                .OrderBy(x => x.ContainerNumber)
                                .Select(x => new DeliveryOrderContainerVm
                                {
                                    ContainerNo = x.ContainerNumber,
                                    ContainerType = x.ContainerSizeType != null ? x.ContainerSizeType.ShortName : null,
                                    SealNo = x.SealNumber,
                                    GrossWeightKg = x.TotalGrossWeight,
                                    PackageCount = x.TotalPackageCount,
                                    TareWeight = (int)x.TareWeight,
                                    MovementType = x.LoadType.LoadTypeCode,
                                })
                                .ToList(),
                    ConsigneeText = x.BLParties.Where(x => x.RoleTypeId == 52).Any() ? x.BLParties.Where(x => x.RoleTypeId == 52).FirstOrDefault().PartyTextInBL : "",
                    NotifyText = x.BLParties.Where(x => x.RoleTypeId == 53).Any() ? x.BLParties.Where(x => x.RoleTypeId == 53).FirstOrDefault().PartyTextInBL : "",
                    Cargos = x.CargoDetails
                            .Select(x => new DeliveryOrderCargoVm
                            {
                                PackageTypeName = x.PackageType != null ? x.PackageType.PackageTypeName : null,
                                PackageCount = x.PackageCount,
                                GrossWeight = x.GrossWeight,
                                NetWeight = x.NetWeight,
                                Measurement = x.WeightUnit.MeasurementUnitName,
                                MarksAndNumbers = x.MarksAndNumbers,
                                DescriptionOfGoods = x.CargoDescription
                            }).ToList()
                })
                .FirstOrDefaultAsync(cancellationToken);
            return vm;
        }

        public async Task<SaveDeliveryOrderDraftResultVm> SaveDraftAsync(SaveDeliveryOrderDraftRequestVm request, CancellationToken cancellationToken = default)
        {
            if (request.BLId == null || request.BLId == Guid.Empty)
            {
                return new SaveDeliveryOrderDraftResultVm
                {
                    Success = false,
                    Message = "BL is required."
                };
            }

            if (string.IsNullOrWhiteSpace(request.DONo))
            {
                return new SaveDeliveryOrderDraftResultVm
                {
                    Success = false,
                    Message = "DO No is required."
                };
            }

            if (request.ClearanceTypeId == null || request.ClearanceTypeId <= 0)
            {
                return new SaveDeliveryOrderDraftResultVm
                {
                    Success = false,
                    Message = "Clearance Type is required."
                };
            }

            if (request.BLReleaseTypeId == null || request.BLReleaseTypeId <= 0)
            {
                return new SaveDeliveryOrderDraftResultVm
                {
                    Success = false,
                    Message = "BL Release Type is required."
                };
            }

            if (request.BLReleaseTypeId == 2)
            {
                if (await _telexReleaseService.HasBLConfirmedTelexAsync(request.BLId.Value) == false)
                {
                    return new SaveDeliveryOrderDraftResultVm
                    {
                        Success = false,
                        Message = "No Any Confirmed Telex Release Found For This BL!!!!!!!"
                    };
                }
            }






            if (request.ExpiryDate.HasValue && request.ExpiryDate.Value.Date < request.DoDate.Date)
            {
                return new SaveDeliveryOrderDraftResultVm
                {
                    Success = false,
                    Message = "Expiry Date cannot be less than DO Date."
                };
            }

            var bl = await _db.BLs
                .AsNoTracking()
                .Where(x => x.Id == request.BLId.Value && !x.IsDeleted)
                .Select(x => new
                {
                    x.Id,
                    x.CompanyId,
                    x.CarrierId,
                    x.JobId,
                    x.ShipmentId,
                    x.BLReleaseTypeId,
                    //x.VoyageId,
                    //x.VesselName,
                    //x.VoyageNumber,
                    x.PODId,
                    //x.ArrivalDate
                })
                .FirstOrDefaultAsync(cancellationToken);

            if (bl == null)
            {
                return new SaveDeliveryOrderDraftResultVm
                {
                    Success = false,
                    Message = "Selected BL was not found."
                };
            }

            var normalizedDoNo = request.DONo.Trim();

            var duplicateQuery = _db.DeliveryOrders
                .AsNoTracking()
                .Where(x => !x.IsDeleted && x.DONo == normalizedDoNo);

            if (request.DeliveryOrderId.HasValue)
            {
                duplicateQuery = duplicateQuery.Where(x => x.Id != request.DeliveryOrderId.Value);
            }

            var hasDuplicateDoNo = await duplicateQuery.AnyAsync(cancellationToken);
            if (hasDuplicateDoNo)
            {
                return new SaveDeliveryOrderDraftResultVm
                {
                    Success = false,
                    Message = "Duplicate DO No found."
                };
            }

            DeliveryOrder entity;

            if (request.DeliveryOrderId.HasValue && request.DeliveryOrderId.Value != Guid.Empty)
            {
                entity = await _db.DeliveryOrders
                    .FirstOrDefaultAsync(x => x.Id == request.DeliveryOrderId.Value && !x.IsDeleted, cancellationToken);

                if (entity == null)
                {
                    return new SaveDeliveryOrderDraftResultVm
                    {
                        Success = false,
                        Message = "Delivery Order record not found."
                    };
                }
                entity.UpdatedAt = DateTime.Now;
                entity.EditUserId = _currentUserService.UserId;
                if (entity.DOOperationalModeId == 2) entity.DONo = request.DONo;
            }
            else
            {
                entity = new DeliveryOrder();
                _db.DeliveryOrders.Add(entity);
                entity.CompanyId = bl.CompanyId;
                entity.CarrierId = bl.CarrierId;
                entity.BLId = bl.Id;
                entity.JobId = bl.JobId;
                entity.DOOperationalModeId = request.DOOperationalModeId;
                entity.CompanyId = request.CompanyId;
                //entity.VoyageId = bl.VoyageId;
                //entity.VesselName = bl.VesselName;
                //entity.VoyageNumber = bl.VoyageNumber;
                entity.DischargePortId = bl.PODId;
                entity.StatusId = 10; // Draft
                entity.CreateUserId = _currentUserService.UserId;
                entity.CreatedAt = DateTime.Now;
                entity.Serial = await GetNewDeliveryOrderNoAsync(entity.CompanyId, entity.DOOperationalModeId);
                if (entity.DOOperationalModeId == 1 || entity.DOOperationalModeId == 3) entity.DONo = entity.Serial.ToString();
            }

            entity.DoDate = request.DoDate;
            entity.ExpiryDate = request.ExpiryDate;
            entity.ArrivalDate = request.ArrivalDate;
            entity.ClearanceTypeId = request.ClearanceTypeId.Value;
            entity.BLReleaseTypeId = request.BLReleaseTypeId.Value;
            entity.OriginalBLCount = request.OriginalBLCount;
            entity.Remarks = request.Remarks?.Trim();
            entity.ExternalIssuerId = request.ExternalIssuerId;
            entity.ExternalIssuerText = request.ExternalIssuerText;
            entity.CarrierId = request.CarrierId;
            entity.CarrierAgentId = request.CarrierAgentId;
            entity.ReceivedAtDateTime = request.ReceivedAtDateTime;
            entity.ReceivedByUserId = request.ReceivedByUserId;
            entity.ReceiveMethodId = request.ReceiveMethodId;
            entity.TIPId = request.TIPId;

            await _db.SaveChangesAsync(cancellationToken);
            if (entity.TIPId.HasValue && entity.TIPId != null) await SaveTIPUsageAsync(entity.TIPId.Value, entity.BLId, cancellationToken);
            await SyncAllContainersFromBlAsync(entity.Id, entity.BLId, cancellationToken);
            await SyncAllCargoFromBlAsync(entity.Id, entity.BLId, cancellationToken);
            return new SaveDeliveryOrderDraftResultVm
            {
                Success = true,
                Message = "Draft saved successfully.",
                DeliveryOrderId = entity.Id
            };
        }
        private async Task<List<SelectListItem>> GetClearanceTypesAsync(CancellationToken cancellationToken)
        {
            return await _db.ClearanceTypes
                .AsNoTracking()
                .OrderBy(x => x.Id)
                .Select(x => new SelectListItem
                {
                    Value = x.Id.ToString(),
                    Text = x.ClearanceTypeName
                })
                .ToListAsync(cancellationToken);
        }
        private async Task<List<SelectListItem>> GetCarriersAsync(CancellationToken cancellationToken)
        {
            return await _db.Carriers
                .Include(x => x.Person)
                .AsNoTracking()
                .OrderBy(x => x.Person.PersonName)
                .Select(x => new SelectListItem
                {
                    Value = x.Id.ToString(),
                    Text = x.Person.PersonName
                })
                .ToListAsync(cancellationToken);
        }

        private async Task<List<SelectListItem>> GetDOStatusesAsync(CancellationToken cancellationToken)
        {
            return await _db.DOStatuses
                .AsNoTracking()
                .OrderBy(x => x.Id)
                .Select(x => new SelectListItem
                {
                    Value = x.Id.ToString(),
                    Text = x.DOStatusName
                })
                .ToListAsync(cancellationToken);
        }



        private async Task<List<SelectListItem>> GetClearanceTypeSelectListAsync(CancellationToken cancellationToken)
        {
            return await _db.ClearanceTypes
                .AsNoTracking()
                .OrderBy(x => x.Id)
                .Select(x => new SelectListItem
                {
                    Value = x.Id.ToString(),
                    Text = x.ClearanceTypeName
                })
                .ToListAsync(cancellationToken);
        }
        //GetDOOperationalModeSelectListAsync
        private async Task<List<SelectListItem>> GetBLReleaseTypeSelectListAsync(CancellationToken cancellationToken)
        {
            return await _db.BLReleaseTypes
                .AsNoTracking()
                .OrderBy(x => x.Id)
                .Select(x => new SelectListItem
                {
                    Value = x.Id.ToString(),
                    Text = x.BLReleaseTypeName
                })
                .ToListAsync(cancellationToken);
        }
        private async Task<List<SelectListItem>> GetDOOperationalModeSelectListAsync(CancellationToken cancellationToken)
        {
            return await _db.DOOperationalModes
                .AsNoTracking()
                .OrderBy(x => x.Id)
                .Select(x => new SelectListItem
                {
                    Value = x.Id.ToString(),
                    Text = x.DOOperationalModeName + "-" + x.DOOperationalModeLocalName
                })
                .ToListAsync(cancellationToken);
        }

        private async Task<List<SelectListItem>> GetUsersSelectListAsync(CancellationToken cancellationToken)
        {
            return await _db.Users
                .AsNoTracking()
                .OrderBy(x => x.UserName)
                .Select(x => new SelectListItem
                {
                    Value = x.Id.ToString(),
                    Text = x.UserName
                })
                .ToListAsync(cancellationToken);
        }
        private async Task<List<SelectListItem>> GetExternalIssuersSelectListAsync(CreateDeliveryOrderPageVm vm, CancellationToken cancellationToken)
        {
            var query = _db.Persons
                .AsNoTracking();

            if (vm.BLId != null && vm.BLId.HasValue && vm.BLId != Guid.Empty)
            {
                var jobId = await _db.BLs
                       .AsNoTracking()
                       .Where(bl => bl.Id == vm.BLId.Value)
                       .Select(bl => bl.JobId)
                       .FirstOrDefaultAsync(cancellationToken);
                query = query.Where(person =>
                    _db.JobParties.Any(jobParty =>
                        jobParty.JobId == jobId &&
                        jobParty.PartyId == person.Id));
            }

            return await query
                .OrderBy(x => x.PersonName)
                .Select(x => new SelectListItem
                {
                    Value = x.Id.ToString(),
                    Text = x.PersonName
                })
                .ToListAsync(cancellationToken);
        }




        private async Task<List<SelectListItem>> GetCarriersSelectListAsync(CreateDeliveryOrderPageVm vm, CancellationToken cancellationToken)
        {
            var query = _db.Carriers
                .Include(x => x.Person)
                .AsNoTracking();

            if (vm.BLId != null && vm.BLId.HasValue && vm.BLId != Guid.Empty)
            {
                var jobId = await _db.BLs
                       .AsNoTracking()
                       .Where(bl => bl.Id == vm.BLId.Value)
                       .Select(bl => bl.JobId)
                       .FirstOrDefaultAsync(cancellationToken);
                query = query.Where(person =>
                    _db.JobParties.Any(jobParty =>
                        jobParty.JobId == jobId &&
                        jobParty.PartyId == person.Id));
            }

            return await query
                .OrderBy(x => x.Person.PersonName)
                .Select(x => new SelectListItem
                {
                    Value = x.Id.ToString(),
                    Text = x.Person.PersonName
                })
                .ToListAsync(cancellationToken);
        }

        private async Task<List<SelectListItem>> GetAgentsSelectListAsync(CreateDeliveryOrderPageVm vm, CancellationToken cancellationToken)
        {
            var query = _db.Agents
                .Include(x => x.Person)
                .AsNoTracking();

            if (vm.BLId != null && vm.BLId.HasValue && vm.BLId != Guid.Empty)
            {
                var jobId = await _db.BLs
                       .AsNoTracking()
                       .Where(bl => bl.Id == vm.BLId.Value)
                       .Select(bl => bl.JobId)
                       .FirstOrDefaultAsync(cancellationToken);
                query = query.Where(person =>
                    _db.JobParties.Any(jobParty =>
                        jobParty.JobId == jobId &&
                        jobParty.PartyId == person.Id));
            }
            return await query
                .OrderBy(x => x.Person.PersonName)
                .Select(x => new SelectListItem
                {
                    Value = x.Id.ToString(),
                    Text = x.Person.PersonName
                })
                .ToListAsync(cancellationToken);
        }
        private async Task<List<SelectListItem>> GetTIPsSelectListAsync(CreateDeliveryOrderPageVm vm, CancellationToken cancellationToken)
        {
            var query = _db.TIPs
                            .Include(x => x.VoyagePort)
                            .ThenInclude(x => x.InboundVoyage)
                            .AsNoTracking();

            if (vm.BLId != null && vm.BLId.HasValue && vm.BLId != Guid.Empty)
            {
                var blId = vm.BLId.Value;
                query = query.Where(tip =>
                    _db.BLVoyages.Any(blVoyage =>
                        blVoyage.BLId == blId &&
                        blVoyage.VoyageId ==
                            tip.VoyagePort.InboundVoyageId));
            }
            return await query
                .OrderBy(x => x.VoyagePort.InboundVoyage.VoyageNo)
                .Select(x => new SelectListItem
                {
                    Value = x.Id.ToString(),
                    Text = x.VoyagePort.InboundVoyage.VoyageNo
                })
                .ToListAsync(cancellationToken);
        }

        private async Task<List<SelectListItem>> GetReceiveMethodsSelectListAsync(CancellationToken cancellationToken)
        {
            return await _db.DOReceiveMethods
                .AsNoTracking()
                .OrderBy(x => x.DOReceiveMethodName)
                .Select(x => new SelectListItem
                {
                    Value = x.Id.ToString(),
                    Text = x.DOReceiveMethodName
                })
                .ToListAsync(cancellationToken);
        }



        public async Task FillCreatePageLookupsAsync(CreateDeliveryOrderPageVm vm, CancellationToken cancellationToken = default)
        {
            vm.ClearanceTypes = await GetClearanceTypeSelectListAsync(cancellationToken);
            vm.BLReleaseTypes = await GetBLReleaseTypeSelectListAsync(cancellationToken);
            vm.DOOperationalModes = await GetDOOperationalModeSelectListAsync(cancellationToken);

            vm.Users = await GetUsersSelectListAsync(cancellationToken);
            vm.ReceiveMethods = await GetReceiveMethodsSelectListAsync(cancellationToken);

            vm.ExternalIssuers = await GetExternalIssuersSelectListAsync(vm, cancellationToken);
            vm.Carriers = await GetCarriersSelectListAsync(vm, cancellationToken);
            vm.Agents = await GetAgentsSelectListAsync(vm, cancellationToken);

            vm.TIPs = await GetTIPsSelectListAsync(vm, cancellationToken);

        }
        public async Task SyncAllCargoFromBlAsync(
            Guid deliveryOrderId,
            Guid blId,
            CancellationToken cancellationToken)
        {
            //var blCargoItems = await _db.BLCargos
            //    .Where(x => x.BLId == blId)
            //    .ToListAsync(cancellationToken);

            //var existingCargoBlIds = await _db.DeliveryOrderCargos
            //    .Where(x => x.DeliveryOrderId == deliveryOrderId)
            //    .Select(x => x.BLCargoId)
            //    .ToListAsync(cancellationToken);

            //var newItems = blCargoItems
            //    .Where(x => !existingCargoBlIds.Contains(x.Id))
            //    .Select(x => new DeliveryOrderCargo
            //    {
            //        Id = Guid.CreateVersion7(),
            //        DeliveryOrderId = deliveryOrderId,
            //        BLCargoId = x.Id,

            //        // snapshot fields
            //        PackageTypeId = x.PackageTypeId,
            //        PackageCount = x.PackageCount,
            //        GrossWeight = x.GrossWeight,
            //        NetWeight = x.NetWeight,
            //        Measurement = x.Measurement,
            //        MarksAndNumbers = x.MarksAndNumbers,
            //        DescriptionOfGoods = x.DescriptionOfGoods
            //    })
            //    .ToList();

            //if (newItems.Count == 0)
            //{
            //    return;
            //}

            //_db.DeliveryOrderCargos.AddRange(newItems);
            //await _db.SaveChangesAsync(cancellationToken);
        }

        public async Task<List<DeliveryOrderCargoVm>> GetDeliveryOrderCargoAsync(
            Guid deliveryOrderId,
            CancellationToken cancellationToken)
        {
            return new List<DeliveryOrderCargoVm>();
            //return await _db.DeliveryOrderCargos
            //    .Where(x => x.DeliveryOrderId == deliveryOrderId)
            //    .Select(x => new DeliveryOrderCargoVm
            //    {
            //        Id = x.Id,
            //        DeliveryOrderId = x.DeliveryOrderId,
            //        BLCargoId = x.BLCargoId,
            //        PackageTypeName = x.PackageType != null ? x.PackageType.Name : null,
            //        PackageCount = x.PackageCount,
            //        GrossWeight = x.GrossWeight,
            //        NetWeight = x.NetWeight,
            //        Measurement = x.Measurement,
            //        MarksAndNumbers = x.MarksAndNumbers,
            //        DescriptionOfGoods = x.DescriptionOfGoods
            //    })
            //    .ToListAsync(cancellationToken);
        }



        public async Task SyncAllContainersFromBlAsync(Guid deliveryOrderId, Guid blId, CancellationToken ct)
        {
            var blContainers = await _db.BLContainers
                .AsNoTracking()
                .Where(x => x.BLId == blId)
                .ToListAsync(ct);

            var existing = await _db.DeliveryOrderContainers
                .Where(x => x.DeliveryOrderId == deliveryOrderId)
                .ToListAsync(ct);

            var existingBlContainerIds = existing
                .Select(x => x.BLContainerId)
                .ToHashSet();

            foreach (var blContainer in blContainers)
            {
                if (existingBlContainerIds.Contains(blContainer.Id))
                    continue;
                _db.DeliveryOrderContainers.Add(new DeliveryOrderContainer
                {
                    Id = Guid.CreateVersion7(),
                    DeliveryOrderId = deliveryOrderId,
                    BLContainerId = blContainer.Id,
                    ContainerNumber = blContainer.ContainerNumber,
                    SealNumber = blContainer.SealNumber,
                    ContainerSizeTypeId = blContainer.ContainerSizeTypeId.Value,
                    IsDeleted = false,
                    CreatedAt = DateTime.Now,
                    CreateUserId = _currentUserService.UserId,
                    cntOwnTypeId = 1 //درست شود
                });
            }
            await _db.SaveChangesAsync(ct);
        }

        public async Task<List<DeliveryOrderContainerVm>> GetDeliveryOrderContainersAsync(
            Guid deliveryOrderId,
            CancellationToken cancellationToken = default)
        {
            return await _db.DeliveryOrderContainers
                .AsNoTracking()
                .Where(x => x.DeliveryOrderId == deliveryOrderId && !x.IsDeleted)
                .Select(x => new DeliveryOrderContainerVm
                {
                    DeliveryOrderContainerId = x.Id,
                    BLContainerId = x.BLContainerId,
                    ContainerNo = x.ContainerNumber,
                    SealNo = x.SealNumber,
                    ContainerSize = x.ContainerSizeType.ContainerSize.ContainerSizeName != null ? x.ContainerSizeType.ContainerSize.ContainerSizeName : null,
                    ContainerType = x.ContainerSizeType.ContainerType.ContainerTypeAbb != null ? x.ContainerSizeType.ContainerType.ContainerTypeAbb : null,
                    GrossWeightKg = x.GrossWeightKG
                })
                .OrderBy(x => x.ContainerNo)
                .ToListAsync(cancellationToken);
        }


        public async Task<FinalizeDeliveryOrderResultVm> FinalizeAsync(FinalizeDeliveryOrderRequestVm request, CancellationToken cancellationToken)
        {
            var result = new FinalizeDeliveryOrderResultVm
            {
                DeliveryOrderId = request.DeliveryOrderId
            };

            if (request.DeliveryOrderId == Guid.Empty)
            {
                result.Message = "Invalid delivery order id.";
                return result;
            }

            var entity = await _db.DeliveryOrders
                .FirstOrDefaultAsync(x => x.Id == request.DeliveryOrderId, cancellationToken);

            if (entity == null)
            {
                result.Message = "Delivery order not found.";
                return result;
            }

            if (entity.StatusId != 10)
            {
                result.Success = true;
                result.Message = "Delivery order is already finalized.";
                result.StatusName = "Finalized";
                return result;
            }

            if (entity.BLId == Guid.Empty)
            {
                result.Message = "Bill of lading is required.";
                return result;
            }

            if (string.IsNullOrWhiteSpace(entity.DONo))
            {
                result.Message = "DO No is required.";
                return result;
            }

            if (!entity.DoDate.HasValue)
            {
                result.Message = "DO Date is required.";
                return result;
            }

            if (!entity.ExpiryDate.HasValue)
            {
                result.Message = "Expiry Date is required.";
                return result;
            }

            if (!entity.ClearanceTypeId.HasValue)
            {
                result.Message = "Clearance Type is required.";
                return result;
            }

            if (!entity.BLReleaseTypeId.HasValue)
            {
                result.Message = "BL Release Type is required.";
                return result;
            }

            var hasContainers = await _db.DeliveryOrderContainers
                .AnyAsync(x => x.DeliveryOrderId == entity.Id, cancellationToken);

            if (!hasContainers)
            {
                result.Message = "No container snapshot found for this delivery order.";
                return result;
            }

            entity.StatusId = 20;
            entity.UpdatedAt = DateTime.Now;
            await _db.SaveChangesAsync(cancellationToken);





            result.Success = true;
            result.Message = "Delivery order finalized successfully.";
            result.StatusName = "Finalized";
            return result;
        }

        public async Task<FinalizeDeliveryOrderResultVm> ApproveAsync(FinalizeDeliveryOrderRequestVm request, CancellationToken cancellationToken)
        {
            var result = new FinalizeDeliveryOrderResultVm
            {
                DeliveryOrderId = request.DeliveryOrderId
            };

            if (request.DeliveryOrderId == Guid.Empty)
            {
                result.Message = "Invalid delivery order id.";
                return result;
            }

            var entity = await _db.DeliveryOrders
                .FirstOrDefaultAsync(x => x.Id == request.DeliveryOrderId, cancellationToken);

            if (entity == null)
            {
                result.Message = "Delivery order not found.";
                return result;
            }

            if (entity.StatusId != 20)
            {
                result.Success = true;
                result.Message = "Delivery order is already finalized.";
                result.StatusName = "Finalized";
                return result;
            }

            entity.StatusId = 30;
            entity.UpdatedAt = DateTime.Now;
            await _db.SaveChangesAsync(cancellationToken);

            result.Success = true;
            result.Message = "Delivery order Approved successfully.";
            result.StatusName = "Approved";
            return result;
        }

        public async Task<FinalizeDeliveryOrderResultVm> ReleaseAsync(FinalizeDeliveryOrderRequestVm request, CancellationToken cancellationToken)
        {
            var result = new FinalizeDeliveryOrderResultVm
            {
                DeliveryOrderId = request.DeliveryOrderId
            };

            if (request.DeliveryOrderId == Guid.Empty)
            {
                result.Message = "Invalid delivery order id.";
                return result;
            }

            var entity = await _db.DeliveryOrders
                .FirstOrDefaultAsync(x => x.Id == request.DeliveryOrderId, cancellationToken);

            if (entity == null)
            {
                result.Message = "Delivery order not found.";
                return result;
            }

            if (entity.StatusId != 30)
            {
                result.Success = true;
                result.Message = "Delivery order is already finalized.";
                result.StatusName = "Finalized";
                return result;
            }

            entity.StatusId = 40;
            entity.UpdatedAt = DateTime.Now;
            entity.ReceivedAtDateTime = DateTime.Now;
            entity.ReleasedByUserId = _currentUserService.UserId;

            await _db.SaveChangesAsync(cancellationToken);

            result.Success = true;
            result.Message = "Delivery order Releaseed successfully.";
            result.StatusName = "Released";
            return result;
        }

        public async Task<FinalizeDeliveryOrderResultVm> CollectedAsync(FinalizeDeliveryOrderRequestVm request, CancellationToken cancellationToken)
        {
            var result = new FinalizeDeliveryOrderResultVm
            {
                DeliveryOrderId = request.DeliveryOrderId
            };

            if (request.DeliveryOrderId == Guid.Empty)
            {
                result.Message = "Invalid delivery order id.";
                return result;
            }

            var entity = await _db.DeliveryOrders
                .FirstOrDefaultAsync(x => x.Id == request.DeliveryOrderId, cancellationToken);

            if (entity == null)
            {
                result.Message = "Delivery order not found.";
                return result;
            }

            if (entity.StatusId != 30)
            {
                result.Success = true;
                result.Message = "Delivery order is already finalized.";
                result.StatusName = "Finalized";
                return result;
            }
            entity.StatusId = 50;
            entity.UpdatedAt = DateTime.Now;
            entity.CollectedDate = DateTime.Now;
            entity.EditUserId = _currentUserService.UserId;
            await _db.SaveChangesAsync(cancellationToken);
            await _containerActionService.DTCAsync(entity.Id);
            result.Success = true;
            result.Message = "Delivery order Collected/Delivered successfully.";
            result.StatusName = "Collected";
            return result;
        }
        public async Task<DeliveryOrderDetailsVm?> GetDetailsAsync(
            Guid deliveryOrderId,
            CancellationToken cancellationToken)
        {
            var entity = await _db.DeliveryOrders
                .AsNoTracking()
                .Where(x => x.Id == deliveryOrderId && !x.IsDeleted)
                .Select(x => new
                {
                    x.Id,
                    x.DONo,
                    x.DoDate,
                    x.ExpiryDate,
                    x.ArrivalDate,
                    x.OriginalBLCount,
                    x.Remarks,
                    x.BLId,
                    //x.IsFinalized,
                    //x.FinalizedAt,
                    ClearanceTypeName = x.ClearanceType != null ? x.ClearanceType.ClearanceTypeName : null,
                    BLReleaseTypeName = x.BLReleaseType != null ? x.BLReleaseType.BLReleaseTypeName : null,
                    GoodsDescription = x.BL.GoodsDescription
                })
                .FirstOrDefaultAsync(cancellationToken);

            if (entity == null)
            {
                return null;
            }

            var result = new DeliveryOrderDetailsVm
            {
                DeliveryOrderId = entity.Id,
                DONo = entity.DONo,
                DoDate = entity.DoDate,
                ExpiryDate = entity.ExpiryDate,
                ArrivalDate = entity.ArrivalDate,
                OriginalBLCount = entity.OriginalBLCount,
                Remarks = entity.Remarks,
                IsFinalized = true,
                //FinalizedAt = entity.,
                ClearanceTypeName = entity.ClearanceTypeName,
                BLReleaseTypeName = entity.BLReleaseTypeName,
                GoodsDescription = entity.GoodsDescription

            };

            result.BLSummary = await GetBLSummaryAsync(entity.BLId, cancellationToken);
            result.Containers = await GetDeliveryOrderContainersAsync(entity.Id, cancellationToken);
            result.CargoItems = await GetDeliveryOrderCargoAsync(entity.Id, cancellationToken);

            return result;
        }

        public async Task<bool> DeleteDraftAsync(Guid deliveryOrderId, CancellationToken cancellationToken)
        {
            var deliveryOrder = await _db.DeliveryOrders
                .FirstOrDefaultAsync(x => x.Id == deliveryOrderId, cancellationToken);

            if (deliveryOrder == null)
            {
                return false;
            }

            // بررسی امنیتی: فقط پیش‌نویس‌ها قابل حذف هستند
            if (deliveryOrder.StatusId != 10)
            {
                throw new InvalidOperationException("Cannot delete a non-Draft Delivery Order.");
            }

            // حذف کانتینرهای کپی شده در این ترخیصیه
            var containers = _db.DeliveryOrderContainers
                .Where(c => c.DeliveryOrderId == deliveryOrderId);
            _db.DeliveryOrderContainers.RemoveRange(containers);

            // حذف خود ترخیصیه
            _db.DeliveryOrders.Remove(deliveryOrder);

            await _db.SaveChangesAsync(cancellationToken);
            return true;
        }

        public async Task<bool> CancelDeliveryOrderAsync(Guid id, string reason, CancellationToken cancellationToken)
        {
            var doEntity = await _db.DeliveryOrders.FirstOrDefaultAsync(x => x.Id == id, cancellationToken);
            if (doEntity == null)
            {
                return false;
            }

            // فقط ترخیصیه‌های نهایی شده (یا حتی پیش‌نویس فعال) قابل ابطال به وضعیت ۹۹ هستند.
            // اما معمولاً ابطال برای اسناد نهایی‌شده معنی دارد.

            doEntity.StatusId = 99; // وضعیت ابطال شده
            doEntity.CancellationReason = reason;
            doEntity.CancelledAt = DateTime.Now;
            doEntity.CancelUserId = _currentUserService.UserId;


            //// در صورت نیاز، پرچم نهایی را تغییر نمی‌دهیم یا قفل نگه می‌داریم تا ویرایش نشود
            //doEntity.IsFinalized = true;

            _db.DeliveryOrders.Update(doEntity);
            await _db.SaveChangesAsync(cancellationToken);
            return true;
        }
        public async Task<DeliveryOrderDetentionsVm?> GetDetentionsByBlAsync(Guid blId, CancellationToken cancellationToken)
        {
            var items = await _db.Detentions
                .AsNoTracking()
                .Include(x => x.Party)
                .Include(x => x.DetentionStatus)
                .Where(x => x.BLId == blId)
                .OrderByDescending(x => x.DetentionToDate)
                .Select(x => new DetentionRowVm
                {
                    Id = x.Id,
                    PartyName = x.Party != null ? x.Party.PersonName : null,
                    DirectionName = x.FinanceDirection.InvoiceContextName,
                    CompanyName = x.Company.CompanyAbb,
                    IssueDate = x.CreatedAt,
                    TariffTitle = x.TariffBook.Name,
                    StatusTitle = x.DetentionStatus != null ? x.DetentionStatus.DetentionStatusName : null,
                    GrossAmount = x.GrossAmount,
                    VATAmount = x.VATAmount,
                    Amount = x.Amount,
                    DetentionToDate = x.DetentionToDate,
                    IsFinal = x.IsFinal,
                    Remarks = x.Remarks
                })
                .ToListAsync(cancellationToken);
            return new DeliveryOrderDetentionsVm
            {
                BLId = blId,
                BLNo = "-",
                Items = items
            };
        }

        public async Task<DeliveryOrderDocumentPanelVm> BuildDocumentPanelAsync(
            Guid deliveryOrderId,
            CancellationToken cancellationToken = default)
        {
            const int objectTypeId = 15;

            var documentTypes = await _db.DocumentTypes
                .AsNoTracking()
                .Where(x => !x.IsDeleted)
                .OrderBy(x => x.Id)
                .Select(x => new
                {
                    x.Id,
                    x.DocumentTypeName
                })
                .ToListAsync(cancellationToken);

            var docs = await _db.Documents
                .AsNoTracking()
                .Where(x => !x.IsDeleted
                    && x.ObjectTypeId == objectTypeId
                    && x.ObjectId == deliveryOrderId)
                .Include(x => x.Attachments)
                .ToListAsync(cancellationToken);

            var vm = new DeliveryOrderDocumentPanelVm
            {
                DeliveryOrderId = deliveryOrderId,
                Cards = documentTypes.Select(dt =>
                {
                    var doc = docs.FirstOrDefault(d => d.DocumentTypeId == dt.Id);

                    return new DeliveryOrderDocumentCardVm
                    {
                        DeliveryOrderId = deliveryOrderId,
                        DocumentTypeId = dt.Id,
                        DocumentTypeName = dt.DocumentTypeName,
                        DocumentId = doc?.Id,
                        Attachments = doc?.Attachments
                            .OrderByDescending(a => a.CreatedAt)
                            .Select(a => new DeliveryOrderDocumentAttachmentVm
                            {
                                Id = a.Id,
                                FileName = a.DocumentAttachmentFileName,
                                FileType = a.DocumentAttachmentFileExtension,
                                FileSizeText = FormatFileSize(a.DocumentAttachmentFileSize)
                            })
                            .ToList() ?? new List<DeliveryOrderDocumentAttachmentVm>()
                    };
                }).ToList()
            };

            return vm;
        }



        public async Task<DeliveryOrderDocumentCardVm> BuildDocumentCardAsync(
            Guid deliveryOrderId,
            int documentTypeId,
            CancellationToken cancellationToken = default)
        {
            const int objectTypeId = 15;

            var doc = await _db.Documents
                .AsNoTracking()
                .Where(x => !x.IsDeleted
                    && x.ObjectTypeId == objectTypeId
                    && x.ObjectId == deliveryOrderId
                    && x.DocumentTypeId == documentTypeId)
                .Include(x => x.Attachments)
                .FirstOrDefaultAsync(cancellationToken);

            var docTypeName = await _db.DocumentTypes
                .AsNoTracking()
                .Where(x => x.Id == documentTypeId)
                .Select(x => x.DocumentTypeName)
                .FirstOrDefaultAsync(cancellationToken) ?? string.Empty;

            return new DeliveryOrderDocumentCardVm
            {
                DeliveryOrderId = deliveryOrderId,
                DocumentTypeId = documentTypeId,
                DocumentTypeName = docTypeName,
                DocumentId = doc?.Id,
                Attachments = doc?.Attachments
                    .OrderByDescending(a => a.CreatedAt)
                    .Select(a => new DeliveryOrderDocumentAttachmentVm
                    {
                        Id = a.Id,
                        FileName = a.DocumentAttachmentFileName,
                        FileType = a.DocumentAttachmentFileExtension,
                        FileSizeText = FormatFileSize(a.DocumentAttachmentFileSize)
                    })
                    .ToList() ?? new List<DeliveryOrderDocumentAttachmentVm>()
            };
        }


        private static string FormatFileSize(decimal bytes)
        {
            string[] suffixes = { "B", "KB", "MB", "GB", "TB" };
            int counter = 0;
            decimal number = bytes;

            while (Math.Round(number / 1024) >= 1)
            {
                number /= 1024;
                counter++;
            }

            return $"{number:n1} {suffixes[counter]}";
        }


        public async Task<DeliveryOrderDocumentCardVm> UploadDocumentFilesAsync(
            Guid deliveryOrderId,
            int documentTypeId,
            List<IFormFile> files,
            int currentCompanyId,
            Guid currentUserId,
            CancellationToken cancellationToken = default)
        {
            const int objectTypeId = 15;

            // ۱. بررسی یا ایجاد سند اصلی (Document) برای این نوع مستند
            var document = await _db.Documents
                .FirstOrDefaultAsync(x => !x.IsDeleted
                    && x.ObjectTypeId == objectTypeId
                    && x.ObjectId == deliveryOrderId
                    && x.DocumentTypeId == documentTypeId, cancellationToken);

            if (document == null)
            {
                var docTitle = await _db.DocumentTypes
                    .Where(x => x.Id == documentTypeId)
                    .Select(x => x.DocumentTypeName)
                    .FirstOrDefaultAsync(cancellationToken) ?? string.Empty;

                document = new Document
                {
                    Id = Guid.CreateVersion7(),
                    CompanyId = 1,
                    DocumentTypeId = documentTypeId,
                    DocumentTitle = "-",
                    DocumentNo = "-",
                    DocumentDate = DateTime.Now,
                    DocumentDescription = "-",
                    ObjectTypeId = objectTypeId,
                    ObjectId = deliveryOrderId,
                    CreatedAt = DateTime.Now,
                    CreateUserId = currentUserId
                };

                _db.Documents.Add(document);
                // ذخیره اولیه برای دریافت شناسه سند جهت انتساب به ضمیمه‌ها
                await _db.SaveChangesAsync(cancellationToken);
            }

            // ۲. افزودن فایل‌ها به عنوان ضمیمه (DocumentAttachment)
            foreach (var file in files)
            {
                if (file.Length == 0) continue;

                using var ms = new MemoryStream();
                await file.CopyToAsync(ms, cancellationToken);

                var attachment = new DocumentAttachment
                {
                    Id = Guid.CreateVersion7(),
                    DocumentId = document.Id,
                    DocumentAttachmentFileName = Path.GetFileName(file.FileName),
                    DocumentAttachmentFileType = file.ContentType ?? string.Empty,
                    DocumentAttachmentFileExtension = Path.GetExtension(file.FileName) ?? string.Empty,
                    DocumentAttachmentFileSize = file.Length,
                    DocumentAttachmentFileData = ms.ToArray(),
                    DocumentAttachmentFilePath = string.Empty,
                    CreatedAt = DateTime.Now,
                    CreateUserId = currentUserId
                };

                _db.DocumentAttachments.Add(attachment);
            }

            await _db.SaveChangesAsync(cancellationToken);

            // ۳. بازگرداندن مدل به‌روزرسانی شده کارت جهت رندر جزئی (Partial Render) با HTMX
            return await BuildDocumentCardAsync(deliveryOrderId, documentTypeId, cancellationToken);
        }

        public async Task<CreateDeliveryOrderPageVm> GetEditPageVmAsync(Guid id, CancellationToken cancellationToken = default)
        {
            var doEntity = await _db.DeliveryOrders
                .AsNoTracking()
                .FirstOrDefaultAsync(x => x.Id == id && !x.IsDeleted, cancellationToken);

            if (doEntity == null)
            {
                return null!;
            }

            var blNo = string.Empty;
            if (doEntity.BLId != Guid.Empty)
            {
                blNo = await _db.BLs
                    .AsNoTracking()
                    .Where(x => x.Id == doEntity.BLId && !x.IsDeleted)
                    .Select(x => x.BLNo)
                    .FirstOrDefaultAsync(cancellationToken) ?? string.Empty;
            }

            var statusText = "Draft";
            if (doEntity.StatusId == 99)
            {
                statusText = "Cancelled";
            }
            else if (doEntity.StatusId == 2) // فرض بر این است که ۲ یا مقدار متناظر در سیستم شما وضعیت Finalized است
            {
                statusText = "Finalized";
            }
            var vm = new CreateDeliveryOrderPageVm
            {
                DeliveryOrderId = doEntity.Id,
                BLId = doEntity.BLId,
                BLNo = blNo,
                DONo = doEntity.DONo ?? string.Empty,
                DoDate = doEntity.DoDate.Value,
                ExpiryDate = doEntity.ExpiryDate,
                ArrivalDate = doEntity.ArrivalDate,
                ClearanceTypeId = doEntity.ClearanceTypeId,
                DOOperationalModeId = doEntity.DOOperationalModeId,
                BLReleaseTypeId = doEntity.BLReleaseTypeId,
                OriginalBLCount = doEntity.OriginalBLCount ?? 0,
                Remarks = doEntity.Remarks,
                StatusText = statusText,
                ExternalIssuerId = doEntity.ExternalIssuerId,
                CarrierAgentId = doEntity.CarrierAgentId,
                CarrierId = doEntity.CarrierId,
                ReceivedByUserId = doEntity.ReceivedByUserId,
                ReceiveMethodId = doEntity.ReceiveMethodId,
                ReceivedAtDateTime = doEntity.ReceivedAtDateTime,
                ExternalIssuerText = doEntity.ExternalIssuerText,
                TIPId = doEntity.TIPId,
                StatusId = doEntity.StatusId

            };
            await FillCreatePageLookupsAsync(vm, cancellationToken);
            return vm;
        }
        public async Task<DocumentAttachmentDto?> GetAttachmentByIdAsync(Guid id, CancellationToken cancellationToken = default)
        {
            // بازیابی فایل بر اساس شناسه از جدول پیوست‌ها
            var attachment = await _db.DocumentAttachments
                .AsNoTracking()
                .FirstOrDefaultAsync(x => x.Id == id && !x.IsDeleted, cancellationToken);

            if (attachment == null)
            {
                return null;
            }
            return new DocumentAttachmentDto
            {
                Id = attachment.Id,
                FileName = attachment.DocumentAttachmentFileName,
                ContentType = attachment.DocumentAttachmentFileType,
                Content = attachment.DocumentAttachmentFileData // آرایه بایت‌های فایل ذخیره شده
            };
        }

        public async Task<List<ChargeRowVm>> GetChargesByBlIdAsync(Guid blId)
        {
            return await _db.Charges
                .AsNoTracking()
                .Where(c => c.BLId == blId)
                .Select(c => new ChargeRowVm
                {
                    ChargeId = c.Id,
                    ChargeTypeName = c.ChargeType.ChargeTypeName, // فرض بر اینکه فیلد Name در ChargeType وجود دارد
                    ChargeDescription = c.ChargeDescription ?? "",
                    Direction = c.FinanceDirection.InvoiceContextName ?? "",
                    Quantity = c.Quantity,
                    UnitRate = c.UnitRate,
                    Amount = c.Amount,
                    CurrencyCode = c.Currency.CurrencyCode, // کد سه حرفی ارز (مثل USD، IRR)
                    TaxAmount = c.TaxAmount,
                    GrossAmount = c.GrossAmount ?? c.Amount,
                    PartyName = c.Party != null ? (c.Party.FirstName + " " + c.Party.LastName) : "-",
                    IsApproved = c.IsApproved
                })
                .OrderBy(c => c.ChargeTypeName)
                .ToListAsync();
        }
        public async Task<List<InvoiceRowVm>> GetInvoicesByBlIdAsync(Guid blId)
        {
            return await _db.Invoices
                .AsNoTracking()
                .Where(i => i.BLId == blId && i.InvoiceStatusId != 8) // حذف ابطال شده‌ها
                .Select(i => new InvoiceRowVm
                {
                    InvoiceId = i.Id,
                    InvoiceNo = i.InvoiceNo,
                    InvoiceDate = i.InvoiceDate.Value,
                    PartyName = i.Party != null ? (i.Party.FirstName + " " + i.Party.LastName) : "Unknown",
                    TypeName = i.InvoiceType.InvoiceTypeName,
                    TotalAmount = i.TotalAmount,
                    CurrencyCode = i.Currency.CurrencyCode,
                    StatusName = i.InvoiceStatus.InvoiceStatusName,
                    StatusId = i.InvoiceStatusId,
                    IsPosted = i.IsPostedToAcc
                })
                .OrderByDescending(i => i.InvoiceDate)
                .ToListAsync();
        }
        public async Task SaveTIPUsageAsync(Guid tipId, Guid blId, CancellationToken cancellationToken = default)
        {
            if (await _db.TIPUsages.Where(x => x.TIPId == tipId && x.BLId == blId).AnyAsync()) return;
            var bl = await _db.BLs
                .Include(x => x.BLContainers)
                    .ThenInclude(x => x.ContainerSizeType)
                .Where(x => x.Id == blId).FirstOrDefaultAsync();
            var serial = await _tipService.GetNewSerialNoAsync(bl.CompanyId, default);
            var tipusageEntity = new TIPUsage
            {
                Id = Guid.CreateVersion7(),
                BLId = blId,
                BLNo = bl.BLNo ?? bl.MasterBLNo,
                TIPId = tipId,
                CompanyId = bl.CompanyId,
                CreatedAt = DateTime.Now,
                CreateUserId = _currentUserService.UserId,
                F20 = bl.BLContainers.Count(x => x.ContainerSizeType.ContainerSizeId == 20),
                F40 = bl.BLContainers.Count(x => x.ContainerSizeType.ContainerSizeId == 40),
                IsDeleted = false,
                Serial = serial,
                LetterNo = $"TIP-{bl.CompanyId}-{serial.ToString().PadLeft(5, '0')}",
                LetterDate = DateTime.Now,

            };
            _db.TIPUsages.Add(tipusageEntity);

            foreach (var cnt in bl.BLContainers)
            {
                var cntEntity = new TIPUsageContainer
                {
                    Id = Guid.CreateVersion7(),
                    ContainerNumber = cnt.ContainerNumber,
                    CreatedAt = DateTime.Now,
                    CreateUserId = _currentUserService.UserId,
                    IsDeleted = false,
                    SizeTypeId = cnt.ContainerSizeTypeId.Value,
                    TIPUsageId = tipusageEntity.Id
                };

                if (_db.Containers.Where(x => x.ContainerNumber == cnt.ContainerNumber).Any())
                {
                    cntEntity.ContainerId = _db.Containers.Where(x => x.ContainerNumber == cnt.ContainerNumber).FirstOrDefault().Id;
                }
                _db.TIPUsageContainers.Add(cntEntity);
            }
            await _db.SaveChangesAsync();
        }


        public byte[] GenerateDeliveryOrderPdf(DeliveryOrderDetailsVm vm, string frxPath)
        {
            var ds = new DataSet();

            // 1. جدول Header
            var dtHeader = new DataTable("DOHeader");
            dtHeader.Columns.Add("DeliveryOrderId", typeof(Guid));
            dtHeader.Columns.Add("DONo", typeof(string));
            dtHeader.Columns.Add("DoDate", typeof(string));
            dtHeader.Columns.Add("ExpiryDate", typeof(string));
            dtHeader.Columns.Add("ArrivalDate", typeof(string));
            dtHeader.Columns.Add("ClearanceTypeName", typeof(string));
            dtHeader.Columns.Add("BLReleaseTypeName", typeof(string));
            dtHeader.Columns.Add("OriginalBLCount", typeof(string));
            dtHeader.Columns.Add("Status", typeof(string));
            dtHeader.Columns.Add("FinalizedAt", typeof(string));
            dtHeader.Columns.Add("Remarks", typeof(string));
            // فیلدهای BL
            dtHeader.Columns.Add("BLNo", typeof(string));
            dtHeader.Columns.Add("BLDate", typeof(string));
            dtHeader.Columns.Add("ConsigneeName", typeof(string));
            dtHeader.Columns.Add("ShipperName", typeof(string));
            dtHeader.Columns.Add("VesselVoyage", typeof(string));
            dtHeader.Columns.Add("PortOfDischargeName", typeof(string));
            dtHeader.Columns.Add("PlaceOfDeliveryName", typeof(string));
            dtHeader.Columns.Add("ContainerCount", typeof(int));

            dtHeader.Columns.Add("VesselName", typeof(string));
            dtHeader.Columns.Add("VoyageNo", typeof(string));
            dtHeader.Columns.Add("FinalDestination", typeof(string));
            dtHeader.Columns.Add("GoodsDescription", typeof(string));


            dtHeader.Rows.Add(
                vm.DeliveryOrderId,
                vm.DONo ?? "-",
                vm.DoDate?.ToString("yyyy-MM-dd") ?? "-",
                vm.ExpiryDate?.ToString("yyyy-MM-dd") ?? "-",
                vm.ArrivalDate?.ToString("yyyy-MM-dd") ?? "-",
                vm.ClearanceTypeName ?? "-",
                vm.BLReleaseTypeName ?? "-",
                vm.OriginalBLCount?.ToString() ?? "-",
                vm.IsFinalized ? "FINALIZED" : "DRAFT",
                vm.FinalizedAt?.ToString("yyyy-MM-dd HH:mm") ?? "-",
                vm.Remarks ?? "-",
                vm.BLSummary?.BLNo ?? "-",
                vm.BLSummary?.ArrivalDate?.ToString("yyyy-MM-dd") ?? "-",
                vm.BLSummary?.ConsigneeText ?? "-",
                vm.BLSummary?.ConsigneeText ?? "-",
                vm.BLSummary?.VoyageNumber ?? "-",
                vm.BLSummary?.POD ?? "-",
                vm.BLSummary?.POD ?? "-",
                vm.Containers?.Count ?? 0,
                "-",
                "-",
                "-",
                vm.GoodsDescription
            );
            ds.Tables.Add(dtHeader);

            // 2. جدول Containers
            var dtContainers = new DataTable("Containers");
            dtContainers.Columns.Add("RowIndex", typeof(int));
            dtContainers.Columns.Add("ContainerNo", typeof(string));
            dtContainers.Columns.Add("SizeAndType", typeof(string));
            dtContainers.Columns.Add("SealNo", typeof(string));
            dtContainers.Columns.Add("PackageCount", typeof(string));
            dtContainers.Columns.Add("GrossWeight", typeof(string));
            dtContainers.Columns.Add("NetWeight", typeof(string));
            dtContainers.Columns.Add("Measurement", typeof(string));
            dtContainers.Columns.Add("CntOwnType", typeof(string));
            dtContainers.Columns.Add("MovementType", typeof(string));
            dtContainers.Columns.Add("PackageType", typeof(string));
            dtContainers.Columns.Add("TareWeight", typeof(string));

            if (vm.Containers != null)
            {
                for (int i = 0; i < vm.Containers.Count; i++)
                {
                    var c = vm.Containers[i];
                    dtContainers.Rows.Add(
                        i + 1,
                        c.ContainerNo ?? "-",
                        $"{c.ContainerSize}-{c.ContainerType}",
                        c.SealNo ?? "-",
                        c.PackageCount,
                        c.GrossWeightKg,
                        "-", // c.NetWeight?.ToString() ?? "-"
                        "-",  // c.Measurement?.ToString() ?? "-"
                        "-",
                        "-",  // c.MovementType?.ToString() ?? "-"
                        "-",  // c.PackageType?.ToString() ?? "-"
                        0  // c.TareWeight?.ToString() ?? "-"
                    );
                }
            }
            ds.Tables.Add(dtContainers);

            //// 3. جدول CargoItems
            //var dtCargo = new DataTable("CargoItems");
            //dtCargo.Columns.Add("RowIndex", typeof(int));
            //dtCargo.Columns.Add("PackageTypeName", typeof(string));
            //dtCargo.Columns.Add("PackageCount", typeof(string));
            //dtCargo.Columns.Add("GrossWeight", typeof(string));
            //dtCargo.Columns.Add("NetWeight", typeof(string));
            //dtCargo.Columns.Add("Measurement", typeof(string));
            //dtCargo.Columns.Add("MarksAndNumbers", typeof(string));
            //dtCargo.Columns.Add("DescriptionOfGoods", typeof(string));

            //if (vm.CargoItems != null)
            //{
            //    for (int i = 0; i < vm.CargoItems.Count; i++)
            //    {
            //        var g = vm.CargoItems[i];
            //        dtCargo.Rows.Add(
            //            i + 1,
            //            g.PackageTypeName ?? "-",
            //            g.PackageCount?.ToString() ?? "-",
            //            g.GrossWeight?.ToString() ?? "-",
            //            g.NetWeight?.ToString() ?? "-",
            //            g.Measurement?.ToString() ?? "-",
            //            g.MarksAndNumbers ?? "-",
            //            g.DescriptionOfGoods ?? "-"
            //        );
            //    }
            //}
            //ds.Tables.Add(dtCargo);
            // 4. اجرای گزارش FastReport و خروجی PDF
            using var report = new Report();
            report.Load(frxPath);

            report.RegisterData(dtHeader, "DOHeader");
            report.RegisterData(dtContainers, "Containers");

            // فعال‌سازی تمام دیتاسورس‌های داخل دیکشنری گزارش بدون وابستگی به نام کانکشن والد
            if (report.GetDataSource("DOHeader") != null)
                report.GetDataSource("DOHeader").Enabled = true;

            if (report.GetDataSource("Containers") != null)
                report.GetDataSource("Containers").Enabled = true;



            var groupBand = report.FindObject("GroupHeaderContainers") as FastReport.GroupHeaderBand;
            if (groupBand != null)
            {
                groupBand.Condition = "1"; // شرط ثابت برای رد شدن از خطای اعتبارسنجی
            }
            report.Prepare();
            using var ms = new MemoryStream();
            var pdfExport = new PDFSimpleExport();
            report.Export(pdfExport, ms);
            return ms.ToArray();
        }
        public async Task<bool> ApproveAsync(Guid id, Guid userId, CancellationToken cancellationToken)
        {
            var _do = await _db.DeliveryOrders.Where(x => x.Id == id && x.StatusId == 10).FirstOrDefaultAsync();
            if (_do == null) return false;
            _do.VerifiedAtDateTime = DateTime.Now;
            _do.VerifiedByUserId = userId;
            _do.StatusId = 30;
            await _db.SaveChangesAsync();



            return true;
        }

        public async Task<int> GetNewDeliveryOrderNoAsync(int companyId, int doOperationalModeId, CancellationToken cancellationToken = default)
        {

            int oMode1 = doOperationalModeId;
            int oMode2 = doOperationalModeId;

            if (oMode1 == 1 || oMode1 == 3)
            {
                oMode1 = 1;
                oMode2 = 3;
            }
            var maxSerial = await _db.DeliveryOrders
                .AsNoTracking()
                .Where(x => x.CompanyId == companyId && (x.DOOperationalModeId == oMode1 || x.DOOperationalModeId == 3))
                .Where(x => !x.IsDeleted)
                .MaxAsync(x => (int?)x.Serial, cancellationToken);
            return (maxSerial ?? 0) + 1;
        }


        public async Task<FreightLetterUpsertVm> GetFreightLetterUpsertVmAsync(Guid doId, CancellationToken ct)
        {
            var doEntity = await _db.DeliveryOrders
                .AsNoTracking()
                .Include(x => x.BL)
                .FirstOrDefaultAsync(x => x.Id == doId, ct);

            if (doEntity == null)
                throw new InvalidOperationException("Delivery Order not found.");

            // 2) FreightLetter موجود را پیدا کن
            var letter = await _db.FreightLetters
                .AsNoTracking()
                .FirstOrDefaultAsync(x => x.DOId == doId && !x.IsDeleted, ct);

            // 3) VM را بساز
            var vm = new FreightLetterUpsertVm
            {
                DOId = doId,
                LetterDate = DateTime.Today
            };

            if (letter != null)
            {
                // edit mode
                vm.Id = letter.Id;
                vm.BLId = letter.BLId;
                vm.MBLNo = letter.MBLNo;
                vm.HBLNo = letter.HBLNo;
                vm.CustomsOfficeId = letter.CustomsOfficeId;
                vm.FreightValue = letter.FreightValue;
                vm.CurrencyId = letter.CurrencyId;
                vm.FromLocationName = letter.FromLocationName;
                vm.ToLocationName = letter.ToLocationName;
                vm.CustomerName = letter.CustomerName;
                vm.LetterNo = letter.LetterNo;
                vm.LetterDate = letter.LetterDate;
                vm.IsSaved = true;
            }
            else
            {
                // create mode - مقداردهی پیش‌فرض از DO/BL (فیلدها را با مدل خودتان تطبیق دهید)
                vm.BLId = doEntity.BLId; // اگر DO.BLId دارد
                vm.MBLNo = doEntity.BL?.MasterBLNo; // اگر BL دارید
                vm.HBLNo = doEntity.BL?.BLNo;

                // اینها را مطابق ساختار خودتان پر کنید
                //vm.CustomerName = doEntity.Consignee.PersonPersianName;
                vm.FromLocationName = doEntity.BL?.POLText;
                vm.ToLocationName = doEntity.BL?.PODText;

                // Currency/CustomsOffice پیش‌فرض اگر دارید
                // vm.CurrencyId = ...
                // vm.CustomsOfficeId = ...
                vm.IsSaved = false;
            }

            // 4) Dropdownها
            vm.Currencies = await _db.Currencies
                .AsNoTracking()
                .OrderBy(x => x.Id)
                .Select(x => new SelectListItem
                {
                    Value = x.Id.ToString(),
                    Text = x.CurrencyName
                })
                .ToListAsync(ct);

            vm.CustomsOffices = await _db.Locations
                .AsNoTracking()
                .Where(x => x.LocationTypeId == 5)
                .OrderBy(x => x.LocationLocalName)
                .Select(x => new SelectListItem
                {
                    Value = x.Id.ToString(),
                    Text = x.LocationLocalName
                })
                .ToListAsync(ct);

            // اگر CurrencyId/CustomsOfficeId هنوز مقدار ندارد، می‌توانید اولین گزینه را انتخاب نکنید
            // و در View یک گزینه "-- Select --" اضافه کنید (یا همینجا Insert کنید)

            return vm;
        }

        public async Task<FreightLetterUpsertVm> UpsertFreightLetterAsync(FreightLetterUpsertVm vm, CancellationToken ct)
        {
            // اعتبارسنجی ساده
            if (vm.DOId == Guid.Empty)
                throw new InvalidOperationException("DOId is required.");

            // DO باید وجود داشته باشد
            var doExists = await _db.DeliveryOrders
                .AsNoTracking()
                .AnyAsync(x => x.Id == vm.DOId, ct);

            if (!doExists)
                throw new InvalidOperationException("Delivery Order not found.");

            // FreightLetter موجود برای DOId (Upsert)
            var entity = await _db.FreightLetters
                .FirstOrDefaultAsync(x => x.DOId == vm.DOId && !x.IsDeleted, ct);

            if (entity == null)
            {
                entity = new FreightLetter
                {
                    DOId = vm.DOId
                };

                await _db.FreightLetters.AddAsync(entity, ct);
            }

            // Map VM -> Entity
            entity.BLId = vm.BLId;
            entity.MBLNo = vm.MBLNo;
            entity.HBLNo = vm.HBLNo;
            entity.CustomsOfficeId = vm.CustomsOfficeId;
            entity.FreightValue = vm.FreightValue;
            entity.CurrencyId = vm.CurrencyId;
            entity.FromLocationName = vm.FromLocationName ?? "";
            entity.ToLocationName = vm.ToLocationName ?? "";
            entity.CustomerName = vm.CustomerName ?? "";

            // اگر LetterNo در UI nullable است ولی در Entity int است:
            // اینجا تعیین تکلیف کنید: یا Required کنید، یا اگر null بود 0
            entity.LetterNo = vm.LetterNo ?? entity.LetterNo;
            entity.LetterDate = vm.LetterDate;
            await _db.SaveChangesAsync(ct);
            // برگشت VM تازه‌سازی‌شده (برای فعال شدن Print)
            var result = await GetFreightLetterUpsertVmAsync(vm.DOId, ct);
            result.IsSaved = true;
            return result;
        }

        public async Task<byte[]> GenerateFreightLetterPdfAsync(Guid doId, string reportTemplatePath, CancellationToken ct)
        {
            // ۱. واکشی اطلاعات نامه به همراه Relationها
            var letter = await _db.FreightLetters
                .AsNoTracking()
                .Include(x => x.CustomsOffice)
                .Include(x => x.Currency)
                .FirstOrDefaultAsync(x => x.DOId == doId && !x.IsDeleted, ct);

            if (letter == null)
                throw new InvalidOperationException("Freight letter not found for this delivery order.");

            // ۲. ساخت TableDataSource دستی (DataTable)
            using var dt = new DataTable("FreightLetterData");
            dt.Columns.Add("LetterNo", typeof(int));
            dt.Columns.Add("LetterDate", typeof(string));
            dt.Columns.Add("CustomsOfficeName", typeof(string));
            dt.Columns.Add("MBLNo", typeof(string));
            dt.Columns.Add("HBLNo", typeof(string));
            dt.Columns.Add("FreightValue", typeof(string));
            dt.Columns.Add("CurrencyName", typeof(string));
            dt.Columns.Add("FromLocationName", typeof(string));
            dt.Columns.Add("ToLocationName", typeof(string));
            dt.Columns.Add("CustomerName", typeof(string));

            // تبدیل تاریخ (می‌توانید از متد تبدیل شمسی پروژه خودتان هم استفاده کنید)
            string formattedDate = letter.LetterDate.ToString("yyyy/MM/dd");

            dt.Rows.Add(
                letter.LetterNo,
                formattedDate,
                letter.CustomsOffice?.LocationLocalName ?? "",
                letter.MBLNo ?? "-",
                letter.HBLNo ?? "-",
                letter.FreightValue.ToString("N2"),
                letter.Currency?.LocalName ?? "",
                letter.FromLocationName ?? "",
                letter.ToLocationName ?? "",
                letter.CustomerName ?? ""
            );

            // ۳. بارگذاری و اجرای FastReport
            using var report = new Report();
            report.Load(reportTemplatePath);
            report.RegisterData(dt, "FreightLetterData");

            // آماده‌سازی گزارش
            report.Prepare();

            // ۴. خروجی PDF درون MemoryStream
            using var pdfStream = new MemoryStream();
            var pdfExport = new PDFSimpleExport(); // یا new PDFExport();
            report.Export(pdfExport, pdfStream);

            return pdfStream.ToArray();
        }
    }
}
