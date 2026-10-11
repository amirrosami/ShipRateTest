using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using ShipRate.Application.DTOs;
using ShipRate.Application.Interfaces;
using ShipRate.Application.ViewModels;
using ShipRate.Domain.Entities.ServiceRequests;
using ShipRate.Infrastructure.Data;

namespace ShipRate.Application.Services
{
    public class CustomerServiceRequestService
    {
        private readonly ApplicationDbContext _db;
        private readonly ICurrentUserService _currentUserService;

        public CustomerServiceRequestService(ApplicationDbContext db, ICurrentUserService currentUserService)
        {
            _db = db;
            _currentUserService = currentUserService;
        }

        public async Task<ServiceRequestIndexDto> GetIndexDataAsync(ServiceRequestSearchDto input)
        {
            NormalizeSearch(input);

            var query = _db.ServiceRequests
                .Include(x => x.Company)
                .ThenInclude(x => x.Person)
                .Include(x => x.InOut)
                .Include(x => x.Party)
                .Include(x => x.CreateUser)
                .Include(x => x.UpdateUser)
                .AsNoTracking()
                .Where(x => !x.IsDeleted);


            if (input.PartyId != Guid.Empty)
            {
                query = query.Where(x => x.PartyId == input.PartyId);
            }
            if (!string.IsNullOrWhiteSpace(input.RequestNo))
                query = query.Where(x => x.RequestNo.Contains(input.RequestNo));

            if (!string.IsNullOrWhiteSpace(input.Subject))
                query = query.Where(x => x.Subject.Contains(input.Subject));

            if (!string.IsNullOrWhiteSpace(input.RequesterName))
            {
                query = query.Where(x =>
                    (x.Party != null && x.Party.PersonName.Contains(input.RequesterName)));
            }

            if (input.RequestTypeId.HasValue)
                query = query.Where(x => x.RequestTypeId == input.RequestTypeId.Value);

            if (input.StatusId.HasValue)
                query = query.Where(x => x.StatusId == input.StatusId.Value);

            if (input.PriorityId.HasValue)
                query = query.Where(x => x.PriorityId == input.PriorityId.Value);

            var totalCount = await query.CountAsync();

            var items = await query
                .OrderByDescending(x => x.CreatedAt)
                .Skip((input.PageNumber - 1) * input.PageSize)
                .Take(input.PageSize)
                .Select(x => new ServiceRequestListItemDto
                {
                    Id = x.Id,
                    RequestNo = x.RequestNo,
                    RequestTypeName = x.RequestType.Name,
                    StatusName = x.RequestStatus.Name,
                    PriorityName = x.RequestPriority.Name,
                    Subject = x.Subject,
                    PartyName = x.Party.PersonName,
                    PartyNameDetail = x.PartyDetailName,
                    RequestDate = x.RequestDate,
                    DueDate = x.DueDate,
                    IsChargeable = x.IsChargeable,
                    CompanyName = x.Company.Person.PersonName,
                    InOutName = x.InOut.InOutName,
                    CreatedAt = x.CreatedAt,
                    UpdatedAt = x.UpdatedAt,
                    CreateUserName = x.CreateUser.UserName,
                    UpdateUserName = x.UpdateUser.UserName,
                    RequestTypeId = x.RequestTypeId,
                    StatusId = x.StatusId
                })
                .ToListAsync();

            return new ServiceRequestIndexDto
            {
                Search = input,
                Result = new PagedResultDto<ServiceRequestListItemDto>(items, totalCount, 1, 10),
                RequestTypes = await GetRequestTypesAsync(),
                Statuses = await GetStatusesAsync(),
                Priorities = await GetPrioritiesAsync()
            };
        }

        public async Task<ServiceRequestFormDto> GetCreateFormAsync()
        {
            return new ServiceRequestFormDto
            {
                Model = new ServiceRequestCreateUpdateDto
                {
                    RequestDate = DateTime.Now
                },
                RequestTypes = await GetRequestTypesAsync(),
                Statuses = await GetStatusesAsync(),
                Priorities = await GetPrioritiesAsync(),
                RoleTypes = await GetRoleTypesAsync(),
                Currencies = await GetCurrenciesAsync(),
                Companies = await GetCompaniesAsync(),
                InOuts = await GetInOutsAsync(),
            };
        }


        public async Task<ServiceRequestFormDto> GetCreateFormForCustomerAsync(int requestTypeId)
        {

            var currentParty = await _currentUserService.GetPersonIdAsync();
            var currentPerson = await _db.Persons.Where(x => x.Id == currentParty).FirstOrDefaultAsync();


            return new ServiceRequestFormDto
            {
                Model = new ServiceRequestCreateUpdateDto
                {
                    RequestTypeId = requestTypeId,
                    PartyDetailName = currentPerson.PersonName,
                    PartyId = currentParty,
                    RequestDate = DateTime.Now,
                    InOutId = 1, //in
                },
                Statuses = await GetStatusesAsync(),
                Priorities = await GetPrioritiesAsync(),
                RoleTypes = await GetRoleTypesAsync(),
                Currencies = await GetCurrenciesAsync(),
                Companies = await GetCompaniesAsync(),
            };
        }


        public async Task<ServiceRequestFormDto> GetEditFormAsync(Guid id)
        {
            var entity = await _db.ServiceRequests
                .AsNoTracking()
                .Include(x => x.Party)
                .FirstOrDefaultAsync(x => x.Id == id && !x.IsDeleted);

            if (entity == null)
                throw new Exception("Service request not found.");

            return new ServiceRequestFormDto
            {
                Model = new ServiceRequestCreateUpdateDto
                {
                    Id = entity.Id,
                    RequestNo = entity.RequestNo,
                    RequestTypeId = entity.RequestTypeId,
                    StatusId = entity.StatusId,
                    PriorityId = entity.PriorityId,
                    Subject = entity.Subject,
                    Description = entity.Description,
                    PartyId = entity.PartyId,
                    PartyDetailName = entity.PartyDetailName,
                    ExternalReferenceNo = entity.ExternalReferenceNo,
                    ParentServiceRequestId = entity.ParentServiceRequestId,
                    RequestDate = entity.RequestDate,
                    DueDate = entity.DueDate,
                    ClosedDate = entity.ClosedDate,
                    IsChargeable = entity.IsChargeable,
                    EstimatedCostAmount = entity.EstimatedCostAmount,
                    FinalCostAmount = entity.FinalCostAmount,
                    CostCurrencyId = entity.CostCurrencyId,
                    Remarks = entity.Remarks,
                    CompanyId = entity.CompanyId,
                    InOutId = entity.InOutId,
                },
                RequestTypes = await GetRequestTypesAsync(),
                Statuses = await GetStatusesAsync(),
                Priorities = await GetPrioritiesAsync(),
                RoleTypes = await GetRoleTypesAsync(),
                Currencies = await GetCurrenciesAsync(),
                PartyDisplayName = entity.Party?.PersonName,
                Companies = await GetCompaniesAsync(),
                InOuts = await GetInOutsAsync()
            };
        }



        public async Task<Guid> CreateAsync(ServiceRequestCreateUpdateDto input)
        {
            ValidateInput(input);
            var entity = new ServiceRequest
            {
                Id = Guid.CreateVersion7(),
                CompanyId = input.CompanyId,
                InOutId = input.InOutId,
                RequestNo = input.RequestNo.Trim(),
                RequestTypeId = input.RequestTypeId,
                StatusId = 1,
                PriorityId = input.PriorityId,
                Subject = input.Subject.Trim(),
                Description = input.Description,
                PartyId = input.PartyId,
                PartyDetailName = input.PartyDetailName,
                ExternalReferenceNo = input.ExternalReferenceNo,
                ParentServiceRequestId = input.ParentServiceRequestId,
                RequestDate = input.RequestDate,
                DueDate = input.DueDate,
                ClosedDate = input.ClosedDate,
                IsChargeable = input.IsChargeable,
                EstimatedCostAmount = input.EstimatedCostAmount,
                FinalCostAmount = input.FinalCostAmount,
                CostCurrencyId = input.CostCurrencyId,
                Remarks = input.Remarks,
                //IsActive = true,
                IsDeleted = false,
                CreatedAt = DateTime.Now,
                CreateUserId = _currentUserService.UserId
            };
            _db.ServiceRequests.Add(entity);

            //if (input.RequestTypeId == 3)
            //{
            //    var exists = await _db.SplitRequests
            //        .AnyAsync(x => x.ServiceRequestId == entity.Id);

            //    if (!exists)
            //    {
            //        var splitRequest = new SplitRequest
            //        {
            //            Id = Guid.CreateVersion7(),
            //            ServiceRequestId = entity.Id,
            //            IsFinalized = false,
            //            CreatedAt = DateTime.Now,
            //            CreateUserId = _currentUserService.UserId
            //        };
            //        _db.SplitRequests.Add(splitRequest);
            //    }
            //}

            await _db.SaveChangesAsync();
            return entity.Id;
        }

        public async Task UpdateAsync(ServiceRequestCreateUpdateDto input)
        {
            ValidateInput(input);

            if (!input.Id.HasValue)
                throw new Exception("Id is required.");

            var entity = await _db.ServiceRequests
                .FirstOrDefaultAsync(x => x.Id == input.Id.Value && !x.IsDeleted);

            if (entity == null)
                throw new Exception("Service request not found.");

            entity.RequestNo = input.RequestNo.Trim();
            entity.CompanyId = input.CompanyId;
            entity.InOutId = input.InOutId;
            entity.RequestTypeId = input.RequestTypeId;
            entity.PriorityId = input.PriorityId;
            entity.Subject = input.Subject.Trim();
            entity.Description = input.Description;
            entity.PartyId = input.PartyId;
            entity.PartyDetailName = input.PartyDetailName;
            entity.ExternalReferenceNo = input.ExternalReferenceNo;
            entity.ParentServiceRequestId = input.ParentServiceRequestId;
            entity.RequestDate = input.RequestDate;
            entity.DueDate = input.DueDate;
            entity.ClosedDate = input.ClosedDate;
            entity.IsChargeable = input.IsChargeable;
            entity.EstimatedCostAmount = input.EstimatedCostAmount;
            entity.FinalCostAmount = input.FinalCostAmount;
            entity.CostCurrencyId = input.CostCurrencyId;
            entity.Remarks = input.Remarks;
            entity.UpdatedAt = DateTime.Now;
            entity.EditUserId = _currentUserService.UserId;
            await _db.SaveChangesAsync();
        }

        public async Task DeleteAsync(Guid id)
        {
            var entity = await _db.ServiceRequests
                .FirstOrDefaultAsync(x => x.Id == id && !x.IsDeleted);

            if (entity == null)
                throw new Exception("Service request not found.");

            entity.IsDeleted = true;
            //entity.IsActive = false;
            entity.DeletedAt = DateTime.Now;
            entity.DeleteUserId = _currentUserService.UserId;

            await _db.SaveChangesAsync();
        }

        public async Task<List<SPersonLookupDto>> SearchPersonsAsync(string? term, int take = 10)
        {
            var query = _db.Persons
                .AsNoTracking()
                .Where(x => !x.IsDeleted);

            if (!string.IsNullOrWhiteSpace(term))
            {
                term = term.Trim();
                query = query.Where(x =>
                    x.PersonName.Contains(term) ||
                    x.FirstName.Contains(term) ||
                    x.LastName.Contains(term) ||
                    x.PersonPersianName.Contains(term));
            }

            return await query
                .OrderBy(x => x.PersonName)
                .Take(take)
                .Select(x => new SPersonLookupDto
                {
                    Id = x.Id,
                    Name = x.PersonName
                })
                .ToListAsync();
        }

        private void NormalizeSearch(ServiceRequestSearchDto input)
        {
            if (input.PageNumber <= 0)
                input.PageNumber = 1;

            if (input.PageSize != 10 && input.PageSize != 20 && input.PageSize != 50)
                input.PageSize = 10;
        }

        private void ValidateInput(ServiceRequestCreateUpdateDto input)
        {
            if (string.IsNullOrWhiteSpace(input.RequestNo))
                throw new Exception("Request No is required.");

            if (string.IsNullOrWhiteSpace(input.Subject))
                throw new Exception("Subject is required.");

            if (input.IsChargeable && !input.CostCurrencyId.HasValue)
                throw new Exception("Currency is required when request is chargeable.");
        }
        public async Task<List<SLookupDto>> GetRequestTypesAsync()
        {
            return await _db.ServiceRequestTypes
                .AsNoTracking()
                .Where(x => x.IsActive && !x.IsDeleted)
                .OrderBy(x => x.Id)
                .Select(x => new SLookupDto
                {
                    Id = x.Id,
                    Name = x.Name + "-" + x.LocalName
                })
                .ToListAsync();
        }

        private async Task<List<SLookupDto>> GetStatusesAsync()
        {
            return await _db.ServiceRequestStatuses
                .AsNoTracking()
                .Where(x => x.IsActive && !x.IsDeleted)
                .OrderBy(x => x.Id)
                .Select(x => new SLookupDto
                {
                    Id = x.Id,
                    Name = x.Name
                })
                .ToListAsync();
        }

        private async Task<List<SLookupDto>> GetPrioritiesAsync()
        {
            return await _db.ServiceRequestPriorities
                .AsNoTracking()
                .Where(x => x.IsActive && !x.IsDeleted)
                .OrderBy(x => x.Id)
                .Select(x => new SLookupDto
                {
                    Id = x.Id,
                    Name = x.Name
                })
                .ToListAsync();
        }

        private async Task<List<SLookupDto>> GetRoleTypesAsync()
        {
            return await _db.RoleTypes
                .AsNoTracking()
                .Where(x => x.IsActive && !x.IsDeleted)
                .OrderBy(x => x.Id)
                .Select(x => new SLookupDto
                {
                    Id = x.Id,
                    Name = x.RoleTypeName
                })
                .ToListAsync();
        }

        private async Task<List<SLookupDto>> GetCurrenciesAsync()
        {
            return await _db.Currencies
                .AsNoTracking()
                .Where(x => x.IsActive && !x.IsDeleted)
                .OrderBy(x => x.Id)
                .Select(x => new SLookupDto
                {
                    Id = x.Id,
                    Name = x.CurrencyCode
                })
                .ToListAsync();
        }
        private async Task<List<SLookupDto>> GetCompaniesAsync()
        {
            return await _db.Companies
                .Include(x => x.Person)
                .AsNoTracking()
                .Where(x => x.IsActive && !x.IsDeleted)
                .OrderBy(x => x.Id)
                .Select(x => new SLookupDto
                {
                    Id = x.Id,
                    Name = x.Person.PersonName
                })
                .ToListAsync();
        }

        private async Task<List<SLookupDto>> GetInOutsAsync()
        {
            return await _db.InOut
                .AsNoTracking()
                .Where(x => x.IsActive && !x.IsDeleted)
                .OrderBy(x => x.Id)
                .Select(x => new SLookupDto
                {
                    Id = x.Id,
                    Name = x.InOutName
                })
                .ToListAsync();
        }





        public async Task<ServiceRequestApprovalsPanelDto> GetApprovalsPanelAsync(Guid serviceRequestId)
        {
            var request = await _db.ServiceRequests
                .AsNoTracking()
                .Where(x => x.Id == serviceRequestId)
                .Select(x => new
                {
                    x.Id,
                    ServiceRequestNo = x.RequestNo // نام ستون را با مدل خودتان هماهنگ کنید
                })
                .FirstOrDefaultAsync();

            if (request == null)
                throw new Exception("Service request not found.");

            var approvals = await _db.ServiceRequestApprovals
                .AsNoTracking()
                .Where(x => x.ServiceRequestId == serviceRequestId)
                .OrderByDescending(x => x.Id)
                .Select(x => new ServiceRequestApprovalListItemDto
                {
                    Id = x.Id,
                    ServiceRequestId = x.ServiceRequestId,
                    ApprovalTypeName = x.ApprovalType.Name,
                    StatusName = x.ApprovalStatus.Name,
                    ApproverUserName = x.ApproverUser != null ? x.ApproverUser.UserName : null,
                    ApprovedAt = x.DecisionDate,
                    Remarks = x.Remarks
                })
                .ToListAsync();

            return new ServiceRequestApprovalsPanelDto
            {
                ServiceRequestId = request.Id,
                ServiceRequestNo = request.ServiceRequestNo,
                Items = approvals
            };
        }


        public async Task<ServiceRequestApprovalFormDto> GetApprovalCreateFormAsync(Guid serviceRequestId)
        {
            var exists = await _db.ServiceRequests.AnyAsync(x => x.Id == serviceRequestId);

            if (!exists)
                throw new Exception("Service request not found.");

            var dto = new ServiceRequestApprovalFormDto
            {
                ServiceRequestId = serviceRequestId,
                ApprovedAt = DateTime.Now
            };

            await FillApprovalFormLookupsAsync(dto);

            return dto;
        }

        public async Task<ServiceRequestApprovalFormDto> GetApprovalEditFormAsync(Guid id)
        {
            var entity = await _db.ServiceRequestApprovals
                .AsNoTracking()
                .FirstOrDefaultAsync(x => x.Id == id);

            if (entity == null)
                throw new Exception("Approval not found.");

            var dto = new ServiceRequestApprovalFormDto
            {
                Id = entity.Id,
                ServiceRequestId = entity.ServiceRequestId,
                ApprovalTypeId = entity.ApprovalTypeId,
                ApprovalStatusId = entity.ApprovalStatusId,
                ApproverUserId = entity.ApproverUserId,
                ApprovedAt = entity.DecisionDate,
                Remarks = entity.Remarks
            };

            await FillApprovalFormLookupsAsync(dto);

            return dto;
        }


        public async Task<Guid> SaveApprovalAsync(ServiceRequestApprovalFormDto dto)
        {
            if (dto.Id == null || dto.Id == Guid.Empty)
            {
                var entity = new ServiceRequestApproval
                {
                    ServiceRequestId = dto.ServiceRequestId,
                    ApprovalTypeId = dto.ApprovalTypeId,
                    ApprovalStatusId = dto.ApprovalStatusId,
                    ApproverUserId = dto.ApproverUserId,
                    DecisionDate = dto.ApprovedAt,
                    Remarks = dto.Remarks
                };

                _db.ServiceRequestApprovals.Add(entity);
                await _db.SaveChangesAsync();

                return entity.Id;
            }
            else
            {
                var entity = await _db.ServiceRequestApprovals
                    .FirstOrDefaultAsync(x => x.Id == dto.Id.Value);

                if (entity == null)
                    throw new Exception("Approval not found.");

                entity.ApprovalTypeId = dto.ApprovalTypeId;
                entity.ApprovalStatusId = dto.ApprovalStatusId;
                entity.ApproverUserId = dto.ApproverUserId;
                entity.DecisionDate = dto.ApprovedAt;
                entity.Remarks = dto.Remarks;

                await _db.SaveChangesAsync();
                return entity.Id;
            }
        }

        public async Task DeleteApprovalAsync(Guid id)
        {
            var entity = await _db.ServiceRequestApprovals
                .FirstOrDefaultAsync(x => x.Id == id);

            if (entity == null)
                return;

            _db.ServiceRequestApprovals.Remove(entity);

            await _db.SaveChangesAsync();
        }


        private async Task FillApprovalFormLookupsAsync(ServiceRequestApprovalFormDto dto)
        {
            dto.ApprovalTypes = await _db.ServiceRequestApprovalTypes
                .AsNoTracking()
                .OrderBy(x => x.Name)
                .Select(x => new SelectListItem
                {
                    Value = x.Id.ToString(),
                    Text = x.Name,
                    Selected = x.Id == dto.ApprovalTypeId
                })
                .ToListAsync();

            dto.ApprovalStatuses = await _db.ServiceRequestApprovalStatuses
                .AsNoTracking()
                .OrderBy(x => x.Name)
                .Select(x => new SelectListItem
                {
                    Value = x.Id.ToString(),
                    Text = x.Name,
                    Selected = x.Id == dto.ApprovalStatusId
                })
                .ToListAsync();

            dto.ApproverUsers = await _db.Users
                .AsNoTracking()
                .OrderBy(x => x.UserName)
                .Select(x => new SelectListItem
                {
                    Value = x.Id.ToString(),
                    Text = x.UserName,
                    Selected = dto.ApproverUserId.HasValue && x.Id == dto.ApproverUserId.Value
                })
                .ToListAsync();
        }



        public async Task<ServiceRequestChargesPanelDto> GetChargesPanelAsync(Guid serviceRequestId)
        {

            var request = await _db.ServiceRequests
                .AsNoTracking()
                .Where(x => x.Id == serviceRequestId)
                .Select(x => new
                {
                    x.Id,
                    ServiceRequestNo = x.RequestNo // نام ستون را با مدل خودتان هماهنگ کنید
                })
                .FirstOrDefaultAsync();

            if (request == null)
                throw new Exception("Service request not found.");




            var charges = await _db.ServiceRequestCharges
                .Where(x => x.ServiceRequestId == serviceRequestId && !x.IsDeleted)
                .Select(x => new ServiceRequestChargeRowDto
                {
                    Id = x.Id,
                    ServiceRequestId = x.ServiceRequestId,
                    ChargeTypeId = x.ChargeTypeId,
                    ChargeTypeName = x.ChargeType.ChargeTypeName,
                    CostTitle = x.CostTitle,
                    Amount = x.Amount,
                    CurrencyId = x.CurrencyId,
                    CurrencyName = x.Currency.CurrencyCode,
                    IsPayable = x.IsPayable,
                    IsReceivable = x.IsReceivable,
                    IsApproved = x.IsApproved,
                    Remarks = x.Remarks
                })
                .ToListAsync();

            return new ServiceRequestChargesPanelDto
            {
                ServiceRequestId = serviceRequestId,
                ServiceRequestNo = request.ServiceRequestNo,
                Charges = charges
            };
        }

        public async Task<ServiceRequestChargeFormDto> GetCreateChargeFormAsync(Guid serviceRequestId)
        {
            var model = new ServiceRequestChargeFormDto
            {
                ServiceRequestId = serviceRequestId,
                IsPayable = false,
                IsReceivable = false,
                IsApproved = false,
                ChargeTypes = await GetChargeTypeSelectListAsync(),
                Currencies = await GetCurrencySelectListAsync()
            };

            return model;
        }

        public async Task<ServiceRequestChargeFormDto?> GetEditChargeFormAsync(Guid chargeId)
        {
            var model = await _db.ServiceRequestCharges
                .Where(x => x.Id == chargeId && !x.IsDeleted)
                .Select(x => new ServiceRequestChargeFormDto
                {
                    Id = x.Id,
                    ServiceRequestId = x.ServiceRequestId,
                    ChargeTypeId = x.ChargeTypeId,
                    CostTitle = x.CostTitle,
                    Amount = x.Amount,
                    CurrencyId = x.CurrencyId,
                    IsPayable = x.IsPayable,
                    IsReceivable = x.IsReceivable,
                    IsApproved = x.IsApproved,
                    Remarks = x.Remarks
                })
                .FirstOrDefaultAsync();

            if (model == null)
                return null;

            model.ChargeTypes = await GetChargeTypeSelectListAsync();
            model.Currencies = await GetCurrencySelectListAsync();

            return model;
        }




        public async Task SaveChargeAsync(ServiceRequestChargeFormDto model)
        {
            if (model.Id.HasValue)
            {
                var entity = await _db.ServiceRequestCharges
                    .FirstAsync(x => x.Id == model.Id.Value && !x.IsDeleted);

                entity.ChargeTypeId = model.ChargeTypeId!.Value;
                entity.CostTitle = model.CostTitle;
                entity.Amount = model.Amount!.Value;
                entity.CurrencyId = model.CurrencyId!.Value;
                entity.IsPayable = model.IsPayable;
                entity.IsReceivable = model.IsReceivable;
                entity.IsApproved = model.IsApproved;
                entity.Remarks = model.Remarks;
                entity.UpdatedAt = DateTime.Now;
                // entity.EditUserId = currentUserId;
            }
            else
            {
                var entity = new ServiceRequestCharge
                {
                    Id = Guid.CreateVersion7(),
                    ServiceRequestId = model.ServiceRequestId,
                    ChargeTypeId = model.ChargeTypeId!.Value,
                    CostTitle = model.CostTitle,
                    Amount = model.Amount!.Value,
                    CurrencyId = model.CurrencyId!.Value,
                    IsPayable = model.IsPayable,
                    IsReceivable = model.IsReceivable,
                    IsApproved = model.IsApproved,
                    Remarks = model.Remarks,
                    //IsActive = true,
                    IsDeleted = false,
                    CreatedAt = DateTime.Now,
                    // CreateUserId = currentUserId
                };

                await _db.ServiceRequestCharges.AddAsync(entity);
            }

            await _db.SaveChangesAsync();
        }

        public async Task DeleteChargeAsync(Guid id)
        {
            var entity = await _db.ServiceRequestCharges
                .FirstAsync(x => x.Id == id && !x.IsDeleted);

            entity.IsDeleted = true;
            entity.DeletedAt = DateTime.Now;
            entity.DeleteUserId = _currentUserService.UserId;

            await _db.SaveChangesAsync();
        }

        public async Task<List<SelectListItem>> GetChargeTypeSelectListAsync()
        {
            return await _db.ChargeTypes
                .Where(x => x.IsActive && !x.IsDeleted)
                .OrderBy(x => x.ChargeTypeName)
                .Select(x => new SelectListItem
                {
                    Value = x.Id.ToString(),
                    Text = x.ChargeTypeName
                })
                .ToListAsync();
        }

        public async Task<List<SelectListItem>> GetCurrencySelectListAsync()
        {
            return await _db.Currencies
                .Where(x => x.IsActive && !x.IsDeleted)
                .OrderBy(x => x.Id)
                .Select(x => new SelectListItem
                {
                    Value = x.Id.ToString(),
                    Text = x.CurrencyCode
                })
                .ToListAsync();
        }

        public async Task<BLAmendmentsPanelDto> GetBLAmendmentsPanelAsync(Guid serviceRequestId)
        {
            var request = await _db.ServiceRequests
                .AsNoTracking()
                .Where(x => x.Id == serviceRequestId)
                .Select(x => new
                {
                    x.Id,
                    ServiceRequestNo = x.RequestNo // نام ستون را با مدل خودتان هماهنگ کنید
                })
                .FirstOrDefaultAsync();

            if (request == null)
                throw new Exception("Service request not found.");






            var requests = await _db.BLAmendmentRequests
                .Where(x => x.ServiceRequestId == serviceRequestId && !x.IsDeleted)
                .OrderByDescending(x => x.CreatedAt)
                .Select(x => new BLAmendmentRequestRowDto
                {
                    Id = x.Id,
                    ServiceRequestId = x.ServiceRequestId,
                    BLId = x.BLId,

                    // این Property را با نام واقعی Entity BL هماهنگ کن
                    BLNumber = x.BL.BLNo,

                    Reason = x.Reason,
                    RequiresCarrierApproval = x.RequiresCarrierApproval,
                    RequiresCustomerApproval = x.RequiresCustomerApproval,

                    Lines = x.Lines
                        .Where(l => !l.IsDeleted)
                        .OrderBy(l => l.CreatedAt)
                        .Select(l => new BLAmendmentRequestLineRowDto
                        {
                            Id = l.Id,
                            BLAmendmentRequestId = l.BLAmendmentRequestId,
                            FieldTypeId = l.FieldTypeId,
                            FieldTypeName = l.FieldType.Name,
                            CurrentValue = l.CurrentValue,
                            RequestedValue = l.RequestedValue,
                            Remarks = l.Remarks
                        })
                        .ToList()
                })
                .ToListAsync();

            return new BLAmendmentsPanelDto
            {
                ServiceRequestId = serviceRequestId,
                ServiceRequestNo = request.ServiceRequestNo,
                Requests = requests
            };
        }

        public async Task<BLAmendmentRequestFormDto> GetCreateBLAmendmentRequestFormAsync(Guid serviceRequestId)
        {
            return new BLAmendmentRequestFormDto
            {
                ServiceRequestId = serviceRequestId,
                RequiresCarrierApproval = false,
                RequiresCustomerApproval = false,
                BLs = await GetBLSelectListAsync(serviceRequestId)
            };
        }


        public async Task<BLAmendmentRequestFormDto> GetCreateBLAmendmentRequestFormforCustomerAsync(Guid serviceRequestId)
        {
            return new BLAmendmentRequestFormDto
            {
                ServiceRequestId = serviceRequestId,
                RequiresCarrierApproval = false,
                RequiresCustomerApproval = false,
                BLs = await GetBLSelectListAsync(serviceRequestId)
            };
        }



        public async Task<ConsolidationRequestDto> GetCreateConsolidationAsync()
        {

            return new ConsolidationRequestDto();
        }


        public async Task<SplitRequestDto> GetCreateSplitAsync()
        {

            return new SplitRequestDto();
        }



        public async Task<BLAmendmentRequestFormDto?> GetEditBLAmendmentRequestFormAsync(Guid id)
        {
            var model = await _db.BLAmendmentRequests
                .Where(x => x.Id == id && !x.IsDeleted)
                .Select(x => new BLAmendmentRequestFormDto
                {
                    Id = x.Id,
                    ServiceRequestId = x.ServiceRequestId,
                    BLId = x.BLId,
                    Reason = x.Reason,
                    RequiresCarrierApproval = x.RequiresCarrierApproval,
                    RequiresCustomerApproval = x.RequiresCustomerApproval
                })
                .FirstOrDefaultAsync();

            if (model == null)
                return null;

            model.BLs = await GetBLSelectListAsync(model.ServiceRequestId);

            return model;
        }


        public async Task<BLAmendmentRequestFormDto?> GetEditBLAmendmentRequestFormByServiceRequestAsync(Guid serviceRequestId)
        {
            var fieldTypes = await GetBLAmendmentFieldTypeSelectListAsync();
            var bls = await GetBLSelectListAsync(serviceRequestId);
            var model = await _db.BLAmendmentRequests
                .Include(x => x.Lines)
                .Where(x => x.ServiceRequestId == serviceRequestId && !x.IsDeleted)
                .Select(x => new BLAmendmentRequestFormDto
                {
                    Id = x.Id,
                    ServiceRequestId = x.ServiceRequestId,
                    BLId = x.BLId,
                    Reason = x.Reason,
                    RequiresCarrierApproval = x.RequiresCarrierApproval,
                    RequiresCustomerApproval = x.RequiresCustomerApproval,
                    Lines = x.Lines.Select(l => new BLAmendmentRequestLineFormDto
                    {
                        Id = l.Id,
                        FieldTypeId = l.FieldTypeId,
                        BLAmendmentRequestId = l.BLAmendmentRequestId,
                        CurrentValue = l.CurrentValue,
                        Remarks = l.Remarks,
                        RequestedValue = l.RequestedValue,
                        FieldTypes = fieldTypes
                    }).ToList()
                }).FirstOrDefaultAsync();

            if (model == null)
                return new BLAmendmentRequestFormDto { BLs = bls, Lines = new List<BLAmendmentRequestLineFormDto>() };

            model.BLs = bls;

            return model;
        }
        public async Task<Utilities.ServiceResult<BLAmendmentRequestFormDto>> SaveBLAmendmentRequestForCustomerAsync(BLAmendmentRequestFormDto model)
        {


            if (model.Id.HasValue)
            {
                var entity = await _db.BLAmendmentRequests
                    .Include(x => x.Lines)
                    .FirstAsync(x => x.Id == model.Id.Value && !x.IsDeleted);

                entity.BLId = model.BLId!.Value;
                entity.Reason = model.Reason;
                entity.RequiresCarrierApproval = model.RequiresCarrierApproval;
                entity.RequiresCustomerApproval = model.RequiresCustomerApproval;
                entity.UpdatedAt = DateTime.Now;

                var updatedLines = model.Lines.Where(x => x.Id.HasValue);
                var newLines = model.Lines.Where(x => !x.Id.HasValue);
                var removedLines = entity.Lines.Where(x => !model.Lines.Select(l => l.Id).Contains(x.Id));

                AddNewRequestLines(entity, newLines.ToList());
                UpdateRequestLines(entity, updatedLines.ToList());
                RemoveRequestLines(removedLines.ToList());

            }
            else
            {
                var entity = new BLAmendmentRequest
                {
                    Id = Guid.CreateVersion7(),
                    ServiceRequestId = model.ServiceRequestId,
                    BLId = model.BLId!.Value,
                    Reason = model.Reason,
                    RequiresCarrierApproval = false,
                    RequiresCustomerApproval = false,
                    IsDeleted = false,
                    CreatedAt = DateTime.Now,
                    CreateUserId = _currentUserService.UserId // جایگزین با currentUserId
                };

                await _db.BLAmendmentRequests.AddAsync(entity);
                AddNewRequestLines(entity, model.Lines);
            }

            try
            {
                await _db.SaveChangesAsync();
                return Utilities.ServiceResult<BLAmendmentRequestFormDto>.Success(model);
            }
            catch (Exception)
            {
                return Utilities.ServiceResult<BLAmendmentRequestFormDto>.Failed("Error in Save Changes in Database");
            }

        }



        private void RemoveRequestLines(List<BLAmendmentRequestLine> lines)
        {
            _db.RemoveRange(lines);
        }


        private void UpdateRequestLines(BLAmendmentRequest request, List<BLAmendmentRequestLineFormDto> lines)
        {
            foreach (var model in lines)
            {
                var line = request.Lines.Where(x => x.Id == model.Id).FirstOrDefault();
                line.CurrentValue = model.CurrentValue;
                line.UpdatedAt = DateTime.Now;
                line.FieldTypeId = (int)model.FieldTypeId;
                line.RequestedValue = model.RequestedValue;
                line.Remarks = model.Remarks;
            }
        }

        private void AddNewRequestLines(BLAmendmentRequest request, List<BLAmendmentRequestLineFormDto> lines)
        {
            foreach (var model in lines)
            {
                _db.BLAmendmentRequestLines.Add(new BLAmendmentRequestLine
                {
                    Id = Guid.CreateVersion7(),
                    BLAmendmentRequestId = request.Id,
                    FieldTypeId = model.FieldTypeId!.Value,
                    CurrentValue = model.CurrentValue,
                    RequestedValue = model.RequestedValue,
                    Remarks = model.Remarks,
                    IsDeleted = false,
                    CreatedAt = DateTime.Now,
                    CreateUserId = Guid.Empty // جایگزین با currentUserId
                });
            }
        }
        public async Task SaveBLAmendmentRequestAsync(BLAmendmentRequestFormDto model)
        {


            if (model.Id.HasValue)
            {
                var entity = await _db.BLAmendmentRequests
                    .FirstAsync(x => x.Id == model.Id.Value && !x.IsDeleted);

                entity.BLId = model.BLId!.Value;
                entity.Reason = model.Reason;
                entity.RequiresCarrierApproval = model.RequiresCarrierApproval;
                entity.RequiresCustomerApproval = model.RequiresCustomerApproval;
                entity.UpdatedAt = DateTime.Now;
                // entity.EditUserId = currentUserId;
            }
            else
            {
                var entity = new BLAmendmentRequest
                {
                    Id = Guid.CreateVersion7(),
                    ServiceRequestId = model.ServiceRequestId,
                    BLId = model.BLId!.Value,
                    Reason = model.Reason,
                    RequiresCarrierApproval = model.RequiresCarrierApproval,
                    RequiresCustomerApproval = model.RequiresCustomerApproval,
                    IsDeleted = false,
                    CreatedAt = DateTime.Now,
                    CreateUserId = Guid.Empty // جایگزین با currentUserId
                };

                await _db.BLAmendmentRequests.AddAsync(entity);
            }

            await _db.SaveChangesAsync();
        }


        public async Task DeleteBLAmendmentRequestAsync(Guid id)
        {
            var entity = await _db.BLAmendmentRequests
                .Include(x => x.Lines)
                .FirstAsync(x => x.Id == id && !x.IsDeleted);

            entity.IsDeleted = true;
            entity.DeletedAt = DateTime.Now;
            entity.DeleteUserId = _currentUserService.UserId;

            foreach (var line in entity.Lines.Where(x => !x.IsDeleted))
            {
                line.IsDeleted = true;
                line.DeletedAt = DateTime.Now;
                line.DeleteUserId = _currentUserService.UserId;
            }


            await _db.SaveChangesAsync();
        }

        public async Task<Utilities.ServiceResult> RegisterRequest(Guid id)
        {
            var request = await _db.ServiceRequests.Where(x => x.Id == id).FirstOrDefaultAsync();
            if (request == null)
            {
                return Utilities.ServiceResult.Failed("Request Not Found!");
            }
            if (request.StatusId != 1)
            {
                return Utilities.ServiceResult.Failed("Cant Submit this Request!");
            }
            request.StatusId = 2;
            try
            {
                await _db.SaveChangesAsync();
                return Utilities.ServiceResult.Success();
            }
            catch (Exception)
            {
                return Utilities.ServiceResult.Failed("Error in Update Request Status!");
            }
        }




        public async Task<BLAmendmentRequestLineFormDto> GetCreateBLAmendmentRequestLineFormAsync(Guid blAmendmentRequestId)
        {
            var serviceRequestId = await _db.BLAmendmentRequests
                .Where(x => x.Id == blAmendmentRequestId && !x.IsDeleted)
                .Select(x => x.ServiceRequestId)
                .FirstAsync();

            return new BLAmendmentRequestLineFormDto
            {
                BLAmendmentRequestId = blAmendmentRequestId,
                ServiceRequestId = serviceRequestId,
                FieldTypes = await GetBLAmendmentFieldTypeSelectListAsync()
            };
        }

        public async Task<BLAmendmentRequestLineFormDto?> GetEditBLAmendmentRequestLineFormAsync(Guid id)
        {
            var model = await _db.BLAmendmentRequestLines
                .Where(x => x.Id == id && !x.IsDeleted)
                .Select(x => new BLAmendmentRequestLineFormDto
                {
                    Id = x.Id,
                    BLAmendmentRequestId = x.BLAmendmentRequestId,
                    ServiceRequestId = x.BLAmendmentRequest.ServiceRequestId,
                    FieldTypeId = x.FieldTypeId,
                    CurrentValue = x.CurrentValue,
                    RequestedValue = x.RequestedValue,
                    Remarks = x.Remarks
                })
                .FirstOrDefaultAsync();

            if (model == null)
                return null;

            model.FieldTypes = await GetBLAmendmentFieldTypeSelectListAsync();

            return model;
        }

        public async Task SaveBLAmendmentRequestLineAsync(BLAmendmentRequestLineFormDto model)
        {
            if (model.Id.HasValue)
            {
                var entity = await _db.BLAmendmentRequestLines
                    .FirstAsync(x => x.Id == model.Id.Value && !x.IsDeleted);

                entity.FieldTypeId = model.FieldTypeId!.Value;
                entity.CurrentValue = model.CurrentValue;
                entity.RequestedValue = model.RequestedValue;
                entity.Remarks = model.Remarks;
                entity.UpdatedAt = DateTime.Now;
                // entity.EditUserId = currentUserId;
            }
            else
            {
                var entity = new BLAmendmentRequestLine
                {
                    Id = Guid.CreateVersion7(),
                    BLAmendmentRequestId = model.BLAmendmentRequestId,
                    FieldTypeId = model.FieldTypeId!.Value,
                    CurrentValue = model.CurrentValue,
                    RequestedValue = model.RequestedValue,
                    Remarks = model.Remarks,
                    IsDeleted = false,
                    CreatedAt = DateTime.Now,
                    CreateUserId = Guid.Empty // جایگزین با currentUserId
                };

                await _db.BLAmendmentRequestLines.AddAsync(entity);
            }

            await _db.SaveChangesAsync();
        }

        public async Task DeleteBLAmendmentRequestLineAsync(Guid id)
        {
            var entity = await _db.BLAmendmentRequestLines
                .FirstAsync(x => x.Id == id && !x.IsDeleted);

            entity.IsDeleted = true;
            entity.DeletedAt = DateTime.Now;
            entity.DeleteUserId = _currentUserService.UserId;

            await _db.SaveChangesAsync();
        }

        public async Task<List<SelectListItem>> GetBLAmendmentFieldTypeSelectListAsync()
        {
            return await _db.BLAmendmentFieldTypes
                .Where(x => x.IsActive && !x.IsDeleted)
                .OrderBy(x => x.Name)
                .Select(x => new SelectListItem
                {
                    Value = x.Id.ToString(),
                    Text = x.LocalName != null
                        ? x.Name + " - " + x.LocalName
                        : x.Name
                })
                .ToListAsync();
        }

        public async Task<List<SelectListItem>> GetBLSelectListAsync(Guid serviceRequestId)
        {
            return await _db.BLs
                .Where(x => !x.IsDeleted)
                .OrderByDescending(x => x.CreatedAt)
                .Select(x => new SelectListItem
                {
                    Value = x.Id.ToString(),

                    // این Property را با نام واقعی BL هماهنگ کن
                    Text = x.BLNo
                })
                .ToListAsync();
        }

        private async Task<List<SelectListItem>> GetBLSelectListAsync()
        {
            return await _db.BLs
                .AsNoTracking()
                .Where(x => !x.IsDeleted)
                .OrderByDescending(x => x.CreatedAt)
                .Select(x => new SelectListItem
                {
                    Value = x.Id.ToString(),
                    Text = x.BLNo
                })
                .ToListAsync();
        }
















        public async Task<List<SelectListItem>> GetJobSelectListAsync(Guid serviceRequestId)
        {
            return await _db.Jobs
                .AsNoTracking()
                .Where(x => !x.IsDeleted)
                .OrderBy(x => x.JobNumber)
                .Select(x => new SelectListItem
                {
                    Value = x.Id.ToString(),
                    Text = x.JobNumber
                })
                .ToListAsync();
        }


        public async Task<List<SelectListItem>> GetBookingSelectListAsync(Guid serviceRequestId)
        {
            return await _db.Bookings
                .AsNoTracking()
                .Where(x => !x.IsDeleted)
                .OrderBy(x => x.BookingNo)
                .Select(x => new SelectListItem
                {
                    Value = x.Id.ToString(),
                    Text = x.BookingNo
                })
                .ToListAsync();
        }

        public async Task<List<SelectListItem>> GetBLContainerSelectListAsync(Guid serviceRequestId)
        {
            return await _db.BLContainers
                .AsNoTracking()
                .Where(x => !x.IsDeleted)
                .OrderBy(x => x.ContainerNumber)
                .Select(x => new SelectListItem
                {
                    Value = x.Id.ToString(),
                    Text = x.ContainerNumber
                })
                .ToListAsync();
        }



        public async Task<ServiceResult> SaveBLAmendmentFullRequestAsync(
            BLAmendmentFullRequestFormDto model)
        {
            if (model.Lines == null || !model.Lines.Any())
            {
                return ServiceResult.Failure("01", "At least one amendment line is required.");
            }
            await using var transaction = await _db.Database.BeginTransactionAsync();
            try
            {
                var serviceRequest = new ServiceRequest
                {
                    Id = Guid.CreateVersion7(),
                    CompanyId = model.Request.Model.CompanyId,
                    InOutId = model.Request.Model.InOutId,
                    PartyId = model.Request.Model.PartyId,
                    PartyDetailName = model.Request.Model.PartyDetailName,
                    RequestNo = model.Request.Model.RequestNo,
                    RequestTypeId = 1,
                    PriorityId = model.Request.Model.PriorityId,
                    Subject = model.Request.Model.Subject,
                    Description = model.Request.Model.Description,
                    RequestDate = model.Request.Model.RequestDate,
                    DueDate = model.Request.Model.DueDate,
                    IsChargeable = model.Request.Model.IsChargeable,
                    ExternalReferenceNo = model.Request.Model.ExternalReferenceNo,
                    Remarks = model.Request.Model.Remarks,
                    StatusId = 1,
                    //IsActive = true,
                    IsDeleted = false,
                    CreatedAt = DateTime.Now,
                    CreateUserId = _currentUserService.UserId
                };
                _db.ServiceRequests.Add(serviceRequest);
                var amendmentRequest = new BLAmendmentRequest
                {
                    Id = Guid.CreateVersion7(),
                    ServiceRequestId = serviceRequest.Id,
                    BLId = model.Amendment.BLId.Value,
                    Reason = model.Amendment.Reason,
                    RequiresCarrierApproval = model.Amendment.RequiresCarrierApproval,
                    RequiresCustomerApproval = model.Amendment.RequiresCustomerApproval,
                    IsDeleted = false,
                    CreatedAt = DateTime.Now,
                    CreateUserId = _currentUserService.UserId
                };
                _db.BLAmendmentRequests.Add(amendmentRequest);
                foreach (var line in model.Lines.Where(x => x.FieldTypeId.HasValue))
                {
                    var entity = new BLAmendmentRequestLine
                    {
                        Id = Guid.CreateVersion7(),
                        BLAmendmentRequestId = amendmentRequest.Id,
                        FieldTypeId = line.FieldTypeId.Value,
                        CurrentValue = line.CurrentValue,
                        RequestedValue = line.RequestedValue,
                        Remarks = line.Remarks,
                        IsDeleted = false,
                        CreatedAt = DateTime.Now,
                        CreateUserId = _currentUserService.UserId
                    };

                    _db.BLAmendmentRequestLines.Add(entity);
                }
                await _db.SaveChangesAsync();
                await transaction.CommitAsync();
                return ServiceResult.Success();
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                return ServiceResult.Failure("02", ex.Message);
            }
        }

        public async Task<BLAmendmentLineItemViewDto> GetBlankBLAmendmentLineAsync(int index)
        {
            var fieldTypes = await GetBLAmendmentFieldTypeSelectListAsync();
            return new BLAmendmentLineItemViewDto
            {
                Index = index,
                Line = new BLAmendmentRequestLineFormDto
                {
                    FieldTypes = fieldTypes
                }
            };
        }

        public async Task<BLAmendmentFullRequestFormDto> GetCreateBLAmendmentFullRequestFormAsync()
        {
            var model = new BLAmendmentFullRequestFormDto();

            model.Request = new ServiceRequestFormDto
            {
                Model = new ServiceRequestCreateUpdateDto
                {
                    Id = Guid.Empty,
                    RequestTypeId = 1, // B/L Amendment
                    RequestDate = DateTime.Now,
                    IsChargeable = false,
                    Subject = "B/L Amendment Request",
                },

                Companies = await GetCompaniesAsync(),
                InOuts = await GetInOutsAsync(),
                Priorities = await GetPrioritiesAsync(),
                RequestTypes = await GetRequestTypesAsync()
            };

            model.Amendment = new BLAmendmentRequestFormDto
            {
                Id = Guid.Empty,
                ServiceRequestId = Guid.Empty,
                BLs = await GetBLSelectListAsync(),
                RequiresCarrierApproval = false,
                RequiresCustomerApproval = false
            };

            var fieldTypes = await GetBLAmendmentFieldTypeSelectListAsync();

            model.Lines = new List<BLAmendmentRequestLineFormDto>
    {
        new BLAmendmentRequestLineFormDto
        {
            Id = Guid.Empty,
            BLAmendmentRequestId = Guid.Empty,
            ServiceRequestId = Guid.Empty,
            FieldTypes = fieldTypes
        }
    };

            return model;
        }

        public async Task<BLAmendmentFullRequestFormDto> RebuildBLAmendmentFullRequestFormAsync(
            BLAmendmentFullRequestFormDto model)
        {
            model ??= new BLAmendmentFullRequestFormDto();

            model.Request ??= new ServiceRequestFormDto();
            model.Request.Model ??= new ServiceRequestCreateUpdateDto();

            model.Amendment ??= new BLAmendmentRequestFormDto();

            model.Lines ??= new List<BLAmendmentRequestLineFormDto>();

            model.Request.Model.RequestTypeId = 1;

            model.Request.Companies = await GetCompaniesAsync();
            model.Request.InOuts = await GetInOutsAsync();
            model.Request.Priorities = await GetPrioritiesAsync();
            model.Request.RequestTypes = await GetRequestTypesAsync();

            model.Amendment.BLs = await GetBLSelectListAsync();

            var fieldTypes = await GetBLAmendmentFieldTypeSelectListAsync();

            if (!model.Lines.Any())
            {
                model.Lines.Add(new BLAmendmentRequestLineFormDto
                {
                    Id = Guid.Empty,
                    BLAmendmentRequestId = Guid.Empty,
                    ServiceRequestId = Guid.Empty,
                    FieldTypes = fieldTypes
                });
            }
            else
            {
                foreach (var line in model.Lines)
                {
                    line.FieldTypes = fieldTypes;
                }
            }

            return model;
        }
        public async Task<SplitRequestPanelDto?> GetSplitRequestPanelAsync(Guid serviceRequestId)
        {
            var serviceRequest = await _db.ServiceRequests
                .AsNoTracking()
                .FirstOrDefaultAsync(x => x.Id == serviceRequestId);

            if (serviceRequest == null)
            {
                return null;
            }
            if (serviceRequest.RequestTypeId != 3)
                return null;


            var splitRequest = await _db.SplitRequests
                .AsNoTracking()
                .Include(x => x.Targets.OrderBy(t => t.SortOrder))
                    .ThenInclude(t => t.Allocations)
                .FirstOrDefaultAsync(x => x.ServiceRequestId == serviceRequestId);


            var availableBls = await _db.BLs   ////درست شود
           .AsNoTracking()
           .Select(x => new SplitBillOfLadingOptionDto
           {
               Id = x.Id,
               BLNo = x.BLNo
           })
           .ToListAsync();




            if (splitRequest == null)
            {
                return new SplitRequestPanelDto
                {
                    ServiceRequestId = serviceRequest.Id,
                    RequestNo = serviceRequest.RequestNo ?? string.Empty,
                    RequestTypeId = serviceRequest.RequestTypeId,
                    Targets = new List<SplitRequestTargetDto>(),
                    AvailableBillsOfLading = availableBls
                };
            }

            string? sourceBlNo = null;

            if (splitRequest.SourceBLId.HasValue)
            {
                sourceBlNo = await _db.BLs
                    .Where(x => x.Id == splitRequest.SourceBLId.Value)
                    .Select(x => x.BLNo)
                    .FirstOrDefaultAsync();
            }
            var sourceBl = await _db.BLs
                    .AsNoTracking()
                    .FirstOrDefaultAsync(x => x.Id == splitRequest.SourceBLId);




            var dto = new SplitRequestPanelDto
            {
                ServiceRequestId = serviceRequest.Id,
                SplitRequestId = splitRequest.Id,
                RequestNo = serviceRequest.RequestNo ?? string.Empty,
                RequestTypeId = serviceRequest.RequestTypeId,
                SourceJobId = splitRequest.SourceJobId,
                SourceBookingId = splitRequest.SourceBookingId,
                SourceBLId = splitRequest.SourceBLId,
                SourceBLNo = sourceBlNo,
                Reason = splitRequest.Reason,
                Notes = splitRequest.Notes,
                IsFinalized = splitRequest.IsFinalized,
                AvailableBillsOfLading = availableBls,
                Targets = splitRequest.Targets
                    .OrderBy(x => x.SortOrder)
                    .Select(target => new SplitRequestTargetDto
                    {
                        Id = target.Id,
                        TargetKey = target.TargetKey,
                        Title = target.Title,
                        TargetBLNo = target.TargetBLNo,
                        Remarks = target.Remarks,
                        SortOrder = target.SortOrder,
                        Allocations = target.Allocations
                            .OrderBy(x => x.CreatedAt)
                            .Select(allocation => new SplitRequestAllocationDto
                            {
                                Id = allocation.Id,
                                SourceLineKey = allocation.SourceLineKey,
                                SourceLineType = (int)allocation.SourceLineType,
                                SourceAllocationId = allocation.SourceAllocationId,
                                SourceCargoDetailId = allocation.SourceCargoDetailId,
                                ContainerNo = allocation.ContainerNo,
                                CargoNo = allocation.CargoNo,
                                Description = allocation.Description,
                                Packages = allocation.Packages,
                                Weight = allocation.Weight,
                                Volume = allocation.Volume,
                                Remarks = allocation.Remarks
                            })
                            .ToList()
                    })
                    .ToList()
            };

            return dto;
        }


        public async Task<AdvancedBLSplitVm?> GetAdvancedSplitModelAsync(
            Guid splitRequestId)
        {
            var splitRequest = await _db.SplitRequests
                .AsNoTracking()
                .Include(x => x.SourceBL)
                    .ThenInclude(x => x.CargoDetails)
                .Include(x => x.SourceBL)
                    .ThenInclude(x => x.Job)
                .Include(x => x.Targets)
                    .ThenInclude(x => x.Allocations)
                .FirstOrDefaultAsync(x => x.Id == splitRequestId);

            if (splitRequest == null)
            {
                return null;
            }

            var model = new AdvancedBLSplitVm
            {
                SplitRequestId = splitRequest.Id,
                ServiceRequestId = splitRequest.ServiceRequestId,
                BlId = splitRequest.SourceBLId.Value,
                BlNo = splitRequest.SourceBL.BLNo,
                JobId = splitRequest.SourceBL.JobId,
                JobNo = splitRequest.SourceBL.Job.JobNumber
            };

            // تبدیل خطوط بارنامه مبدأ به SourceLines
            model.SourceLines = splitRequest.SourceBL
                .CargoDetails
                .Select(x => new BLSplitSourceLineVm
                {
                    SourceLineKey = x.Id.ToString(),
                    SourceCargoDetailId = x.Id,
                    Description = x.CargoDescription,
                    AvailablePackages = x.PackageCount.Value,
                    AvailableWeight = x.GrossWeight.Value,
                    AvailableVolume = x.Volume.Value
                })
                .ToList();

            // تبدیل پیش‌نویس‌های ذخیره‌شده به Targetها
            model.Targets = splitRequest.Targets
                .OrderBy(x => x.SortOrder)
                .Select(x => new BLSplitTargetVm
                {
                    TargetKey = x.TargetKey,
                    Title = x.Title,
                    Assignments = x.Allocations
                        .Select(a => new BLSplitAssignmentVm
                        {
                            SourceLineKey = a.SourceLineKey,
                            Packages = a.Packages,
                            Weight = a.Weight,
                            Volume = a.Volume
                        })
                        .ToList()
                })
                .ToList();

            return model;
        }

        public async Task<CreateSplitRequestResult> CreateSplitRequestAsync(CreateSplitRequestVm model)
        {
            var serviceRequest = await _db.ServiceRequests
                .FirstOrDefaultAsync(x => x.Id == model.ServiceRequestId);

            if (serviceRequest == null)
            {
                return CreateSplitRequestResult.Fail(404, "Service request not found.");
            }

            if (serviceRequest.RequestTypeId != 3)
            {
                return CreateSplitRequestResult.Fail(400, "This service request is not a split request.");
            }

            var billOfLadingExists = await _db.BLs
                .AnyAsync(x => x.Id == model.SourceBLId);

            if (!billOfLadingExists)
            {
                return CreateSplitRequestResult.Fail(400, "The selected bill of lading was not found.");
            }

            var splitRequest = await _db.SplitRequests
                .FirstOrDefaultAsync(x => x.ServiceRequestId == model.ServiceRequestId);

            if (splitRequest == null)
            {
                splitRequest = new SplitRequest
                {
                    Id = Guid.CreateVersion7(),
                    ServiceRequestId = model.ServiceRequestId,
                    SourceBLId = model.SourceBLId,
                    Reason = model.Reason,
                    Notes = model.Notes,
                    IsFinalized = false
                };

                _db.SplitRequests.Add(splitRequest);
            }
            else
            {
                if (splitRequest.IsFinalized)
                {
                    return CreateSplitRequestResult.Fail(
                        400,
                        "The source bill of lading cannot be changed after finalization.");
                }

                splitRequest.SourceBLId = model.SourceBLId;
                splitRequest.Reason = model.Reason;
                splitRequest.Notes = model.Notes;
            }
            await _db.SaveChangesAsync();
            return CreateSplitRequestResult.Ok(splitRequest.Id);
        }

        public async Task<SubmitAdvancedSplitResult> SubmitAdvancedSplitAsync(SubmitAdvancedSplitVm model)
        {
            var splitRequest = await _db.SplitRequests
                .Include(x => x.ServiceRequest)
                .FirstOrDefaultAsync(x => x.Id == model.SplitRequestId);

            if (splitRequest == null)
            {
                return SubmitAdvancedSplitResult.Fail("Split request not found.");
            }

            if (splitRequest.IsFinalized)
            {
                return SubmitAdvancedSplitResult.Fail("This split request is already finalized.");
            }

            if (splitRequest.SourceBLId != model.SourceBlId)
            {
                return SubmitAdvancedSplitResult.Fail("Source BL does not match the split request.");
            }

            if (model.Targets == null || !model.Targets.Any())
            {
                return SubmitAdvancedSplitResult.Fail("At least one target is required.");
            }

            var allAssignments = model.Targets
                .Where(t => t.Assignments != null)
                .SelectMany(t => t.Assignments)
                .ToList();

            if (!allAssignments.Any())
            {
                return SubmitAdvancedSplitResult.Fail("No split assignments found.");
            }

            if (allAssignments.Any(x => x.Packages < 0 || x.Weight < 0 || x.Volume < 0))
            {
                return SubmitAdvancedSplitResult.Fail("Split values cannot be negative.");
            }

            var sourceAllocationIds = allAssignments
                .Where(x => x.SourceAllocationId.HasValue)
                .Select(x => x.SourceAllocationId!.Value)
                .Distinct()
                .ToList();

            var sourceCargoDetailIds = allAssignments
                .Where(x => x.SourceCargoDetailId.HasValue)
                .Select(x => x.SourceCargoDetailId!.Value)
                .Distinct()
                .ToList();

            var sourceAllocations = await _db.SplitRequestAllocations
                .Where(x => sourceAllocationIds.Contains(x.Id))
                .ToListAsync();

            var sourceCargoDetails = await _db.CargoDetails
                .Where(x => sourceCargoDetailIds.Contains(x.Id))
                .ToListAsync();

            foreach (var assignment in allAssignments)
            {
                if (assignment.SourceAllocationId == null && assignment.SourceCargoDetailId == null)
                {
                    return SubmitAdvancedSplitResult.Fail("Invalid source line reference.");
                }
            }

            var groupedBySource = allAssignments.GroupBy(x => x.SourceLineKey);
            foreach (var group in groupedBySource)
            {
                var totalPackages = group.Sum(x => x.Packages);
                var totalWeight = group.Sum(x => x.Weight);
                var totalVolume = group.Sum(x => x.Volume);

                var sample = group.First();

                decimal availablePackages;
                decimal availableWeight;
                decimal availableVolume;

                if (sample.SourceAllocationId.HasValue)
                {
                    var source = sourceAllocations.FirstOrDefault(x => x.Id == sample.SourceAllocationId.Value);
                    if (source == null)
                        return SubmitAdvancedSplitResult.Fail($"Source allocation not found: {group.Key}");

                    availablePackages = source.Packages;
                    availableWeight = source.Weight;
                    availableVolume = source.Volume;
                }
                else
                {
                    var source = sourceCargoDetails.FirstOrDefault(x => x.Id == sample.SourceCargoDetailId!.Value);
                    if (source == null)
                        return SubmitAdvancedSplitResult.Fail($"Source cargo detail not found: {group.Key}");

                    availablePackages = source.PackageCount.Value;
                    availableWeight = source.GrossWeight.Value;
                    availableVolume = source.Volume.Value;
                }

                if (totalPackages > availablePackages || totalWeight > availableWeight || totalVolume > availableVolume)
                {
                    return SubmitAdvancedSplitResult.Fail($"Over-split detected for source line {group.Key}.");
                }
            }

            var sortOrder = 0;
            foreach (var target in model.Targets.Where(x => x.Assignments.Any()))
            {
                ////var targetBl = new BL
                ////{
                ////    Id = Guid.CreateVersion7(),
                ////    JobId = model.JobId,
                ////    BLNo = string.IsNullOrWhiteSpace(target.TargetBlNo) ? await GenerateBlNoAsync(model.JobId) : target.TargetBlNo.Trim()
                ////};

                //_db.BLs.Add(targetBl);
                sortOrder += 1;
                var rTarget = new SplitRequestTarget
                {
                    Id = Guid.CreateVersion7(),
                    SplitRequestId = splitRequest.Id,
                    CreatedAt = DateTime.Now,
                    CreateUserId = _currentUserService.UserId,
                    TargetKey = target.TargetKey,
                    TargetBLNo = string.IsNullOrWhiteSpace(target.TargetBlNo) ? sortOrder.ToString() : target.TargetBlNo.Trim(),
                    //IsActive = true,
                    IsDeleted = false
                };
                _db.SplitRequestTargets.Add(rTarget);

                foreach (var assignment in target.Assignments)
                {
                    if (assignment.SourceAllocationId.HasValue)
                    {
                        _db.SplitRequestAllocations.Add(new SplitRequestAllocation
                        {
                            Id = Guid.CreateVersion7(),
                            //SplitRequestId = splitRequest.Id,
                            //TargetBillOfLadingId = targetBl.Id,
                            SourceAllocationId = assignment.SourceAllocationId,
                            Packages = assignment.Packages,
                            Weight = assignment.Weight,
                            Volume = assignment.Volume,
                        });
                    }
                    else if (assignment.SourceCargoDetailId.HasValue)
                    {
                        _db.SplitRequestAllocations.Add(new SplitRequestAllocation
                        {
                            Id = Guid.CreateVersion7(),
                            //SplitRequestId = splitRequest.Id,
                            //TargetBillOfLadingId = targetBl.Id,
                            SourceCargoDetailId = assignment.SourceCargoDetailId,
                            Packages = assignment.Packages,
                            Weight = assignment.Weight,
                            Volume = assignment.Volume
                        });
                    }
                }
            }

            splitRequest.IsFinalized = true;

            await _db.SaveChangesAsync();

            return SubmitAdvancedSplitResult.Ok(
                splitRequest.ServiceRequestId,
                splitRequest.Id,
                "BL split submitted successfully.");
        }

















    }
}