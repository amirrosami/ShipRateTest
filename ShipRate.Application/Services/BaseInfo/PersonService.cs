using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using ShipRate.Application.DTOs;
using ShipRate.Application.DTOs.Base_Info;
using ShipRate.Application.Interfaces;
using ShipRate.Application.Interfaces.BaseInfo;
using ShipRate.Application.Validations.Domain;
using ShipRate.Common.Exceptions;
using ShipRate.Common.Web;
using ShipRate.Domain.Entities.Agents;
using ShipRate.Domain.Entities.BaseInfo.Locations;
using ShipRate.Domain.Entities.BaseInfo.Persons;
using ShipRate.Domain.Entities.Finance;
using ShipRate.Domain.Entities.Lines;
using ShipRate.Domain.Entities.Operations;
using ShipRate.Domain.Interfaces;
using ShipRate.Infrastructure.Data;

namespace ShipRate.Application.Services.BaseInfo
{
    public class PersonService : IPersonService
    {
        private readonly IPersonRepository _repository;
        private readonly IRepository<Carrier> _carrierrepository;
        private readonly IRepository<Driver> _driverrepository;
        private readonly IRepository<PersonContact> _contactrepository;
        private readonly IRepository<PersonExtraInfo> _extrainforepository;
        private readonly IRepository<BankAccount> _bankaccountrepository;
        private readonly IRepository<Branch> _branchrepository;
        private readonly IRepository<Country> _countryrepository;
        private readonly ICurrentUserService _currentUserService;
        private readonly IRepository<Agent> _agentrepository;
        private readonly IRepository<GPSProvider> _gpsProviderRepo;
        private readonly IRepository<CourierCompany> _courierCompanyRepo;
        private readonly IRepository<PersonRole> _personRoleRepo;
        private readonly UserService _userService;
        private readonly ApplicationDbContext _db;

        public PersonService(IPersonRepository repository
            , IRepository<Country> countryrepository
            , IRepository<PersonContact> contactrepository
            , IRepository<Carrier> carrierrepository
            , IRepository<Driver> driverrepository
            , IRepository<PersonExtraInfo> extrainforepository
            , ICurrentUserService currentUserService,
IRepository<BankAccount> bankaccountrepository,
IRepository<Agent> agentrepository, IRepository<GPSProvider> gpsProviderRepo, IRepository<CourierCompany> courierCompanyRepo, IRepository<PersonRole> personRoleRepo, IRepository<Branch> branchrepository, ApplicationDbContext db)
        {
            _repository = repository;
            _countryrepository = countryrepository;
            _contactrepository = contactrepository;
            _carrierrepository = carrierrepository;
            _driverrepository = driverrepository;
            _extrainforepository = extrainforepository;
            _currentUserService = currentUserService;
            _bankaccountrepository = bankaccountrepository;
            _agentrepository = agentrepository;
            _gpsProviderRepo = gpsProviderRepo;
            _courierCompanyRepo = courierCompanyRepo;
            _personRoleRepo = personRoleRepo;
            _branchrepository = branchrepository;
            _db = db;
        }

        public async Task<PagedResultDto<PersonDto>> GetPagedAsync(PagedRequestDto request)
        {
            var query = await _repository.GetAllAsQueryableAsync();
            //query = ApplySecurityFilter(query);



            // Search
            if (!string.IsNullOrWhiteSpace(request.SearchTerm))
            {
                var term = request.SearchTerm.Trim().ToLower();
                query = query.Where(x =>
                    x.PersonName.ToLower().Contains(term) ||
                    x.PersonPersianName.ToLower().Contains(term)
                    || x.NationalID.ToLower().Contains(term));
            }

            var totalCount = await query.CountAsync();

            var items = await query
                .Skip((request.PageNumber - 1) * request.PageSize)
                .Take(request.PageSize)
                .Select(x => MapToDto(x))
                .ToListAsync();

            return new PagedResultDto<PersonDto>(items, totalCount, request.PageNumber, request.PageSize);
        }
        // متد کمکی برای اعمال امنیت
        private IQueryable<Person> ApplySecurityFilter(IQueryable<Person> query)
        {
            // اگر ادمین است، هیچ فیلتری اعمال نشود (همه چیز را ببیند)
            //if (_currentUserService.IsAdmin) return query;

            var userId = _currentUserService.UserId;

            // فیلتر:
            // یا اصلاً مشتری نیست (در جدول Customers نیست)
            // یا مشتری است و SalesUserId او برابر با کاربر فعلی است
            return query.Where(p =>
                !_db.Customers.Any(c => c.Id == p.Id) ||
                _db.Customers.Any(c => c.Id == p.Id && c.SalesUserId == userId)
            );
        }


        public async Task<PersonDto?> GetByIdAsync(Guid id)
        {
            var entity = await _repository.GetByIdWithForeignAsync(id);
            if (entity == null) return null;

            var personDto = MapToDto(entity);

            //personDto.IsCarrier = await _carrierrepository.AnyAsync(x => x.Id == id);
            //personDto.IsDriver = await _driverrepository.AnyAsync(x => x.Id == id);
            //personDto.IsAgent = await _agentrepository.AnyAsync(x => x.Id == id);

            return personDto;
        }
        public async Task<PersonContactDto?> GetContactByIdAsync(Guid id)
        {
            var entity = await _contactrepository.GetByGuidAsync(id);
            return entity == null ? null : MapToConatctDto(entity);
        }


        public async Task<(bool Success, string? Error)> CreateAsync(PersonCreateUpdateDto dto)
        {
            dto.Id = Guid.CreateVersion7();
            var entity = new Person
            {
                Id = dto.Id,
                FirstName = dto.FirstName,
                LastName = dto.LastName,
                PersonPersianName = dto.PersonPersianName,
                PersonName = dto.PersonName,
                Address = dto.Address,
                PersianAddress = dto.PersianAddress,
                BirthDate = dto.BirthDate,
                CityId = dto.CityId,
                CountryId = dto.CountryId,
                EcoCode = dto.EcoCode ?? "",
                FaxNo = dto.FaxNo ?? "",
                EmailAddress = dto.EmailAddress,
                Mobile = dto.Mobile,
                NationalID = dto.NationalID,
                PersonalityTypeId = dto.PersonalityTypeId,
                PersonRegDate = dto.PersonRegDate,
                Phone = dto.Phone,
                RegNo = dto.RegNo,
                ZipCode = dto.ZipCode,
                IsDeleted = false,
                CreateUserId = _currentUserService.UserId,
                CreatedAt = DateTime.Now
            };

            var validator = new PersonValidator();
            var validationresult = validator.Validate(entity);
            if (!validationresult.IsValid)
            {
                string errors = "";
                foreach (var error in validationresult.Errors)
                {
                    errors = errors + error + "-";
                }
                return (false, errors);
            }

            if (await _repository.AnyAsync(x => x.NationalID == entity.NationalID && x.CountryId == 1))
            {
                return (false, "This National ID exists!");
            }
            await _repository.BeginTransactionAsync();
            try
            {


                await _repository.AddAsync(entity);
                await _repository.SaveChangesAsync();


                dto.Agent.PersonId = dto.Id;
                var res = await SaveAgentAsync(dto.Agent, dto.IsAgent);
                if (!res.Success)
                {
                    await _repository.RollBackTransactionAsync();
                    return (false, res.Error);
                }
                dto.Carrier.PersonId = dto.Id;

                res = await SaveCarrierAsync(dto.Carrier, dto.IsCarrier);
                if (!res.Success)
                {
                    await _repository.RollBackTransactionAsync();
                    return (false, res.Error);
                }
                dto.Driver.PersonId = dto.Id;

                res = await SaveDriverAsync(dto.Driver, dto.IsDriver);
                if (!res.Success)
                {
                    await _repository.RollBackTransactionAsync();
                    return (false, res.Error);
                }
                dto.GPSProviderDto.PersonId = dto.Id;
                res = await SaveGPSProviderAsync(dto.GPSProviderDto, dto.IsGPSProvider);
                if (!res.Success)
                {
                    await _repository.RollBackTransactionAsync();
                    return (false, res.Error);
                }
                dto.CourierCompany.PersonId = dto.Id;
                res = await SaveCourierCompanyAsync(dto.CourierCompany, dto.IsCourierCompany);
                if (!res.Success)
                {
                    await _repository.RollBackTransactionAsync();
                    return (false, res.Error);
                }
                res = await SavePersonRoleTypes(dto.SelectedRoleTypes, dto.Id);
                if (!res.Success)
                {
                    await _repository.RollBackTransactionAsync();
                    return (false, res.Error);
                }
            }
            catch (Exception e)
            {
                await _repository.RollBackTransactionAsync();
                throw new GeneralAppException("error in create person");
            }
            await _repository.CommitTransactionAsync();
            return (true, null);
        }




        public async Task<(bool success, string error)> SavePersonRoleTypes(List<int> selectedRoleTypes, Guid personId)
        {
            var currentRoles = _personRoleRepo.Query().Where(x => x.PersonId == personId).ToList();
            var removedRoles = currentRoles.Where(x => !selectedRoleTypes.Contains(x.RoleTypeId)).ToList();
            var newRoles = selectedRoleTypes.Where(x => !currentRoles.Select(d => d.RoleTypeId).Contains(x));
            //await _personRoleRepo.BeginTransactionAsync();
            try
            {
                foreach (var item in removedRoles)
                {
                    await _personRoleRepo.DeleteAsync(item);
                }
                foreach (var item in newRoles)
                {
                    await _personRoleRepo.AddAsync(new PersonRole
                    {
                        Id = Guid.CreateVersion7(),
                        PersonId = personId,
                        RoleTypeId = item
                    });
                }
                return (true, null);
            }
            catch (Exception e)
            {
                // await _personRoleRepo.RollBackTransactionAsync();
                return (false, "error in saving Person Role Types");
            }
            //await _personRoleRepo.CommitTransactionAsync();
        }
        public async Task<(bool Success, string? Error)> UpdateAsync(PersonCreateUpdateDto dto)
        {
            var entity = await _repository.GetByGuidAsync(dto.Id);
            if (entity == null || entity.IsDeleted)
                return (false, "Person not found.");


            entity.PersonName = dto.PersonName;
            entity.PersonPersianName = dto.PersonPersianName;
            entity.FaxNo = dto.FaxNo;
            entity.ZipCode = dto.FaxNo;
            entity.PersonalityTypeId = dto.PersonalityTypeId;
            entity.CountryId = dto.CountryId;
            entity.CityId = dto.CityId;
            entity.BirthDate = dto.BirthDate;
            entity.NationalID = dto.NationalID;
            entity.Mobile = dto.Mobile;
            entity.Phone = dto.Phone;
            entity.Address = dto.Address;
            entity.PersianAddress = dto.PersianAddress;
            entity.RegNo = dto.RegNo;
            entity.FirstName = dto.FirstName;
            entity.LastName = dto.LastName;
            entity.EcoCode = dto.EcoCode;
            entity.EditUserId = _currentUserService.UserId;
            entity.EmailAddress = dto.EmailAddress;

            var validator = new PersonValidator();
            var validationresult = validator.Validate(entity);
            if (!validationresult.IsValid)
            {
                string errors = "";
                foreach (var error in validationresult.Errors)
                {
                    errors += error + "-";
                }
                return (false, errors);
            }
            await _repository.BeginTransactionAsync();

            try
            {
                await _repository.UpdateAsync(entity);
                await _repository.SaveChangesAsync();


                dto.Agent.PersonId = dto.Id;
                var res = await SaveAgentAsync(dto.Agent, dto.IsAgent);
                if (!res.Success)
                {
                    await _repository.RollBackTransactionAsync();
                    return (false, res.Error);
                }
                dto.Carrier.PersonId = dto.Id;
                if (!res.Success)
                {
                    await _repository.RollBackTransactionAsync();
                    return (false, res.Error);
                }
                res = await SaveCarrierAsync(dto.Carrier, dto.IsCarrier);
                if (!res.Success)
                {
                    await _repository.RollBackTransactionAsync();
                    return (false, res.Error);
                }
                dto.Driver.PersonId = dto.Id;

                res = await SaveDriverAsync(dto.Driver, dto.IsDriver);
                if (!res.Success)
                {
                    await _repository.RollBackTransactionAsync();
                    return (false, res.Error);
                }
                dto.GPSProviderDto.PersonId = dto.Id;
                res = await SaveGPSProviderAsync(dto.GPSProviderDto, dto.IsGPSProvider);
                if (!res.Success)
                {
                    await _repository.RollBackTransactionAsync();
                    return (false, res.Error);
                }
                dto.CourierCompany.PersonId = dto.Id;
                res = await SaveCourierCompanyAsync(dto.CourierCompany, dto.IsCourierCompany);
                if (!res.Success)
                {
                    await _repository.RollBackTransactionAsync();
                    return (false, res.Error);
                }
                res = await SavePersonRoleTypes(dto.SelectedRoleTypes, dto.Id);
                if (!res.Success)
                {
                    await _repository.RollBackTransactionAsync();
                    return (false, res.Error);
                }
            }
            catch (Exception e)
            {
                await _repository.RollBackTransactionAsync();
                throw e;
            }
            await _repository.CommitTransactionAsync();
            return (true, null);
        }


        public async Task<(bool Success, string? Error)> UpdateContactAsync(PersonContactCreateUpdateDto dto)
        {
            var entity = await _contactrepository.GetByGuidAsync(dto.Id.Value);
            if (entity == null || entity.IsDeleted)
                return (false, "Contact not found.");


            entity.IsMain = dto.IsMain;
            entity.Role = dto.Role;
            entity.Email = dto.Email;
            entity.Phone = dto.Phone;
            entity.UpdatedAt = DateTime.Now;
            entity.EditUserId = _currentUserService.UserId;

            await _contactrepository.UpdateAsync(entity);
            await _contactrepository.SaveChangesAsync();

            //return MapToDto(entity);
            return (true, null);
        }




        public async Task<bool> SoftDeleteAsync(Guid id)
        {
            var entity = await _repository.GetByGuidAsync(id);
            if (entity == null) return false;

            var canDelete = await _db.CanSoftDelete<Person>(id);
            if (canDelete)
            {

                entity.IsDeleted = true;
                entity.UpdatedAt = DateTime.Now;
                entity.EditUserId = _currentUserService.UserId;

                await _repository.UpdateAsync(entity);
                await _repository.SaveChangesAsync();
                return true;
            }
            return false;
        }

        //public async Task<bool> CodeExistsAsync(string code, int? excludeId = null)
        //{
        //    return await _repository.AnyAsync(x =>
        //        x.PackageTypeCode.ToLower() == code.Trim().ToLower() &&
        //        (!excludeId.HasValue || x.Id != excludeId.Value));
        //}


        private static List<PersonExtraInfoDto> MapToDto3(List<PersonExtraInfo> list)
        {
            var infolist = new List<PersonExtraInfoDto>();
            if (list is null) return infolist;

            foreach (var item in list)
            {
                infolist.Add(
                    new PersonExtraInfoDto
                    {
                        Id = item.Id,
                        PersonId = item.PersonId,
                        ExtraInfoTypeId = item.InfoTypeId,
                        ExtraInfoTypeName = item.InfoType.InfoTypeName,
                        PersonExtraInfoValue = item.PersonExtraInfoValue,
                    }
                    );
            }
            return infolist;
        }


        private static List<BankAccountDto> MapToDto4(List<BankAccount> list)
        {
            var infolist = new List<BankAccountDto>();
            if (list is null) return infolist;

            foreach (var item in list)
            {
                infolist.Add(
                    new BankAccountDto
                    {
                        Id = item.Id,
                        PersonId = item.PersonId,
                        AccountNumber = item.AccountNumber,
                        BankAccountPurposeId = item.BankAccountPurposeId,
                        BankAccountPurposeName = item.BankAccountPurpose.BankAccountPurposeName,
                        BankAccountTitle = item.BankAccountTitle,
                        BankAccountTitlePersian = item.BankAccountTitle,
                        BankId = item.BankId,
                        BankName = item.Bank.BankName,
                        BranchAddress = item.BranchAddress,
                        BranchName = item.BranchName,
                        BranchNo = item.BranchNo,
                        CompanyId = item.CompanyId,
                        CurrencyId = item.CurrencyId,
                        CurrencyName = item.Currency.CurrencyName,
                        IBAN = item.IBAN,
                        Remarks = item.Remarks,
                    }
                    );
            }
            return infolist;
        }



        private static List<PersonContactDto> MapToDto2(List<PersonContact> list)
        {
            var contactlist = new List<PersonContactDto>();
            if (list is null) return contactlist;

            foreach (var item in list)
            {
                contactlist.Add(
                    new PersonContactDto
                    {
                        Id = item.Id,
                        FullName = item.FullName,
                        Email = item.Email,
                        IsMain = item.IsMain,
                        Phone = item.Phone,
                        Role = item.Role,
                        CreatedAt = item.CreatedAt,
                        IsDeleted = item.IsDeleted,
                        UpdatedAt = item.UpdatedAt,
                        PersonId = item.PersonId,
                        PersonName = item.Person.PersonName,
                    }
                    );

            }

            return contactlist;
        }

        private static PersonDto MapToDto(Person entity)
        {
            return new PersonDto
            {
                Id = entity.Id,
                PersonPersianName = entity.PersonPersianName,
                PersonName = entity.PersonName,
                ZipCode = entity.ZipCode,
                Address = entity.Address,
                PersianAddress = entity.PersianAddress,
                BirthDate = entity.BirthDate,
                CityId = entity.CityId,
                CountryId = entity.CountryId,
                EcoCode = entity.EcoCode,
                EmailAddress = entity.EmailAddress,
                FirstName = entity.FirstName,
                FaxNo = entity.FaxNo,
                LastName = entity.LastName,
                Mobile = entity.Mobile,
                NationalID = entity.NationalID,
                PersonalityTypeId = entity.PersonalityTypeId,
                PersonalityTypeName = entity.PersonalityType.PersonalityTypeName,
                CountryName = entity.Country.CountryName,
                CreatedAt = entity.CreatedAt,
                UpdatedAt = entity.UpdatedAt,
                RegNo = entity.RegNo,
                PersonRegDate = entity.PersonRegDate,
                Phone = entity.Phone,
                ContactsList = MapToDto2(entity.PersonContacts),
                PersonExtraInfoList = MapToDto3(entity.PersonExtraInfos),
                BankAccountsList = MapToDto4(entity.BankAccounts),
                IsActive = true
            };
        }

        private static PersonContactDto MapToConatctDto(PersonContact entity)
        {
            return new PersonContactDto
            {
                Id = entity.Id,
                CreatedAt = entity.CreatedAt,
                Email = entity.Email,
                FullName = entity.FullName,
                IsDeleted = entity.IsDeleted,
                IsMain = entity.IsMain,
                PersonId = entity.PersonId,
                Phone = entity.Phone,
                Role = entity.Role,
                UpdatedAt = entity.UpdatedAt
            };
        }


        private static DTOs.Base_Info.CarrierDto MapToCarrierDto(Carrier entity)
        {
            return new DTOs.Base_Info.CarrierDto
            {
                Id = entity.Id,
                TrackingUrl = entity.TrackingUrl,
                AWBPrefix = entity.AWBPrefix,
                BookingEmail = entity.BookingEmail,
                CarrierCode = entity.CarrierCode,
                CarrierTypeId = entity.CarrierTypeId,
                IsVesselOwner = entity.IsVesselOwner,
                OperatingRegion = entity.OperatingRegion,
                PersonId = entity.Id
            };
        }

        private static AgentDto MapToAgentDto(Agent entity)
        {
            return new AgentDto
            {
                Id = entity.Id,
                CompanyId = entity.CompanyId.Value,
                ParentId = entity.ParentId is null ? null : entity.ParentId.Value,
                PersonId = entity.Id
            };
        }



        private static PersonExtraInfoDto MapToExtraInfoDto(PersonExtraInfo entity)
        {
            return new PersonExtraInfoDto
            {
                Id = entity.Id,
                ExtraInfoTypeId = entity.InfoTypeId,
                PersonExtraInfoValue = entity.PersonExtraInfoValue,
                //ExtraInfoTypeName = entity.InfoType.InfoTypeName,
                PersonId = entity.PersonId
            };
        }

        private static BankAccountDto MapToBankAccountDto(BankAccount entity)
        {
            return new BankAccountDto
            {
                Id = entity.Id,
                PersonId = entity.PersonId,
                Remarks = entity.Remarks,
                AccountNumber = entity.AccountNumber,
                BankAccountPurposeId = entity.BankAccountPurposeId,
                BankAccountPurposeName = entity.BankAccountPurpose?.BankAccountPurposeName,
                BankAccountTitle = entity.BankAccountTitle,
                BankAccountTitlePersian = entity.BankAccountTitle,
                BankId = entity.BankId,
                BankName = entity.Bank?.BankName,
                BranchAddress = entity.BranchAddress,
                BranchName = entity.BranchName,
                BranchNo = entity.BranchNo,
                CompanyId = entity.CompanyId,
                CurrencyId = entity.CurrencyId,
                CompanyName = entity.Currency?.CurrencyName,
                IBAN = entity.IBAN,
            };
        }
        private static BranchDto MapToBranchDto(Branch entity)
        {
            return new BranchDto
            {
                Id = entity.Id,
                PersonId = entity.ParentId,
                BranchName = entity.BranchName,
                CountryId = entity.CountryId,
                Address = entity.Address,
                BranchCode = entity.BranchCode,
                BranchTypeId = entity.BranchTypeId,
                BranchTypeName = entity.BranchType.BranchTypeName,
                Country = entity.Country.CountryName,
                CreatedAt = entity.CreatedAt,
                Email = entity.Email,
                Location = entity.Location.LocationName,
                LocationId = entity.LocationId,
                ManagerName = entity.ManagerName,
                Phone = entity.Phone,
                UpdatedAt = entity.UpdatedAt
            };
        }



        private static DriverDto MapToDriverDto(Driver entity)
        {
            return new DriverDto
            {
                Id = entity.Id,
                PersonId = entity.Id,
                CarrierId = entity.CarrierId,
                LicenseNumber = entity.LicenseNumber,
                PassportNo = entity.PassportNo,
                SmartCardNo = entity.SmartCardNo
            };
        }


        private static GPSProviderDto MapToGPSProviderDto(GPSProvider entity)
        {
            return new GPSProviderDto
            {
                Id = entity.Id,
                PersonId = entity.Id,
                Remarks = entity.Remarks,
                SiteAddress = entity.SiteAddress
            };
        }

        private static CourierCompanyDto MapToCourierCompanyDto(CourierCompany entity)
        {
            return new CourierCompanyDto
            {
                Id = entity.Id,
                PersonId = entity.Id,
                Remarks = entity.Remarks,
            };
        }


        //public async Task<bool> IsCodeUniqueAsync(string code, int? excludeId = null)
        //{
        //    var trimmed = code.Trim().ToLower();
        //    if (excludeId.HasValue)
        //    {
        //        return !await _repository.AnyAsync(x => x.PackageTypeCode.ToLower() == trimmed && x.Id != excludeId.Value);
        //    }
        //    return !await _repository.AnyAsync(x => x.PackageTypeCode.ToLower() == trimmed);
        //}

        public async Task<(bool Success, string? Error)> DeleteAsync(Guid id)
        {
            var entity = await _repository.GetByGuidAsync(id);
            if (entity == null || entity.IsDeleted)
                return (false, "Person not found.");


            var canDelete = await _db.CanSoftDelete<Person>(id);
            if (canDelete)
            {
                entity.IsDeleted = true;
                entity.DeletedAt = DateTime.Now;
                entity.DeleteUserId = _currentUserService.UserId;

                //entity. = DateTime.Now;
                await _repository.UpdateAsync(entity);
                await _repository.SaveChangesAsync();
                return (true, null);
            }
            return (false, "This record has been used!!!!");
        }

        public async Task<(bool Success, string? Error)> ToggleActiveAsync(int id)
        {
            var entity = await _repository.GetByIdAsync(id);
            if (entity == null || entity.IsDeleted)
                return (false, "Person not found.");

            entity.UpdatedAt = DateTime.Now;
            entity.EditUserId = _currentUserService.UserId;
            await _repository.UpdateAsync(entity);
            await _repository.SaveChangesAsync();
            return (true, null);
        }



        public Task<(bool Success, string? Error)> ToggleActiveAsync(Guid id)
        {
            throw new NotImplementedException();
        }
        public async Task<(bool Success, string? Error)> CreateContactAsync(PersonContactCreateUpdateDto dto)
        {
            var entity = new PersonContact
            {
                PersonId = dto.PersonId,
                Email = dto.Email,
                FullName = dto.FullName,
                IsMain = dto.IsMain,
                Phone = dto.Phone,
                Role = dto.Role,
                IsDeleted = false,
                CreateUserId = _currentUserService.UserId,
                CreatedAt = DateTime.Now,

            };
            await _contactrepository.AddAsync(entity);
            await _contactrepository.SaveChangesAsync();
            return (true, null);
        }

        public async Task<(bool Success, string? Error)> DeleteContactAsync(Guid id)
        {
            var entity = await _contactrepository.GetByGuidAsync(id);
            if (entity == null || entity.IsDeleted)
                return (false, "Contact not found.");

            entity.IsDeleted = true;
            entity.UpdatedAt = DateTime.Now;
            entity.EditUserId = _currentUserService.UserId;

            await _contactrepository.UpdateAsync(entity);
            await _contactrepository.SaveChangesAsync();
            return (true, null);
        }

        public async Task<(bool Success, string? Error)> SaveCarrierAsync(DTOs.Base_Info.CarrierDto dto, bool isCarrier)
        {
            var existingEntity = await _carrierrepository.GetByGuidAsync(dto.PersonId);
            if (existingEntity == null && isCarrier)
            {
                var carrier = new Carrier
                {
                    Id = dto.PersonId,
                    AWBPrefix = dto.AWBPrefix,
                    BookingEmail = dto.BookingEmail,
                    CarrierCode = dto.CarrierCode,
                    CarrierTypeId = dto.CarrierTypeId.Value,
                    IsVesselOwner = dto.IsVesselOwner,
                    OperatingRegion = dto.OperatingRegion,
                    TrackingUrl = dto.TrackingUrl,
                    CreateUserId = _currentUserService.UserId
                };
                await _carrierrepository.AddAsync(carrier);
                await _carrierrepository.SaveChangesAsync();
                return (true, null);
            }
            else
            {
                if (!isCarrier && existingEntity != null)
                {
                    await _carrierrepository.DeleteAsync(existingEntity);
                    await _carrierrepository.SaveChangesAsync();
                }
                if (isCarrier && existingEntity != null)
                {
                    existingEntity.AWBPrefix = dto.AWBPrefix;
                    existingEntity.BookingEmail = dto.BookingEmail;
                    existingEntity.CarrierCode = dto.CarrierCode;
                    existingEntity.CarrierTypeId = dto.CarrierTypeId.Value;
                    existingEntity.IsVesselOwner = dto.IsVesselOwner;
                    existingEntity.OperatingRegion = dto.OperatingRegion;
                    existingEntity.TrackingUrl = dto.TrackingUrl;

                    existingEntity.UpdatedAt = DateTime.Now;
                    existingEntity.EditUserId = _currentUserService.UserId;

                    await _carrierrepository.UpdateAsync(existingEntity);
                    await _carrierrepository.SaveChangesAsync();
                }

                return (true, null);
            }
        }







        public async Task<(bool Success, string? Error)> SaveAgentAsync(AgentDto dto, bool isAgent)
        {
            var existingEntity = await _agentrepository.GetByGuidAsync(dto.PersonId);
            if (existingEntity == null && isAgent)
            {
                var agent = new Agent
                {
                    Id = dto.PersonId,
                    ParentId = dto.ParentId,
                    CompanyId = dto.CompanyId,
                    CreateUserId = _currentUserService.UserId,
                    CreatedAt = DateTime.Now,
                    IsActive = false
                };
                var validator = new AgentValidator();
                var validationresult = validator.Validate(agent);
                if (!validationresult.IsValid)
                {
                    string errors = "";
                    foreach (var error in validationresult.Errors)
                    {
                        errors += error + "-";
                    }
                    return (false, errors);
                }
                await _agentrepository.AddAsync(agent);
                await _agentrepository.SaveChangesAsync();
                return (true, null);
            }
            else
            {
                if (!isAgent && existingEntity != null)
                {
                    //remove
                    await _agentrepository.DeleteAsync(existingEntity);
                    await _agentrepository.SaveChangesAsync();
                }
                if (isAgent && existingEntity != null)
                {
                    existingEntity.CompanyId = dto.CompanyId;
                    existingEntity.ParentId = dto.ParentId;
                    existingEntity.UpdatedAt = DateTime.Now;
                    existingEntity.EditUserId = _currentUserService.UserId;

                    await _agentrepository.UpdateAsync(existingEntity);
                    await _agentrepository.SaveChangesAsync();
                }

                return (true, null);
            }
        }

        public async Task<DTOs.Base_Info.CarrierDto?> GetCarrierByIdAsync(Guid id)
        {
            var entity = await _carrierrepository.GetByGuidAsync(id);
            return entity == null ? null : MapToCarrierDto(entity);
        }

        public async Task<AgentDto?> GetAgentByIdAsync(Guid id)
        {
            var entity = await _agentrepository.GetByGuidAsync(id);
            return entity == null ? null : MapToAgentDto(entity);
        }



        public async Task<PersonExtraInfoDto?> GetExtraInfoByIdAsync(Guid id)
        {
            var entity = await _extrainforepository.GetByGuidAsync(id);
            return entity == null ? null : MapToExtraInfoDto(entity);
        }

        public async Task<BankAccountDto?> GetBankAccountByIdAsync(Guid id)
        {
            var entity = await _bankaccountrepository.GetByGuidAsync(id);
            return entity == null ? null : MapToBankAccountDto(entity);
        }



        public async Task<DriverDto?> GetDriverByIdAsync(Guid id)
        {
            var entity = await _driverrepository.GetByGuidAsync(id);
            return entity == null ? null : MapToDriverDto(entity);
        }

        public async Task<GPSProviderDto?> GetGPSProviderByIdAsync(Guid id)
        {
            var entity = await _gpsProviderRepo.GetByGuidAsync(id);
            return entity == null ? null : MapToGPSProviderDto(entity);
        }

        public async Task<CourierCompanyDto?> GetCourierCompanyByIdAsync(Guid id)
        {
            var entity = await _courierCompanyRepo.GetByGuidAsync(id);
            return entity == null ? null : MapToCourierCompanyDto(entity);
        }

        public async Task<(bool Success, string? Error)> SaveDriverAsync(DriverDto dto, bool isDriver)
        {
            var existingEntity = await _driverrepository.GetByGuidAsync(dto.PersonId);
            if (existingEntity == null && isDriver)
            {
                var driver = new Driver
                {
                    Id = dto.PersonId,
                    CarrierId = dto.CarrierId,
                    PassportNo = dto.PassportNo,
                    LicenseNumber = dto.PassportNo,
                    SmartCardNo = dto.SmartCardNo,
                    CreatedAt = DateTime.Now,
                    CreateUserId = _currentUserService.UserId
                };
                await _driverrepository.AddAsync(driver);
                await _driverrepository.SaveChangesAsync();
                return (true, null);
            }
            else
            {
                if (!isDriver && existingEntity != null)
                {
                    await _driverrepository.DeleteAsync(existingEntity);
                    await _driverrepository.SaveChangesAsync();
                }
                if (isDriver && existingEntity != null)
                {
                    existingEntity.LicenseNumber = dto.LicenseNumber;
                    existingEntity.PassportNo = dto.PassportNo;
                    existingEntity.SmartCardNo = dto.SmartCardNo;
                    await _driverrepository.UpdateAsync(existingEntity);
                    await _driverrepository.SaveChangesAsync();
                }

                return (true, null);
            }
        }

        public async Task<(bool Success, string? Error)> SaveGPSProviderAsync(GPSProviderDto dto, bool isGPS)
        {
            var existingEntity = await _gpsProviderRepo.GetByGuidAsync(dto.PersonId);
            if (existingEntity == null && isGPS)
            {
                var gps = new GPSProvider
                {
                    Id = dto.PersonId,
                    SiteAddress = dto.SiteAddress,
                    Remarks = dto.Remarks,
                    CreatedAt = DateTime.Now,
                    CreateUserId = _currentUserService.UserId
                };
                await _gpsProviderRepo.AddAsync(gps);
                await _driverrepository.SaveChangesAsync();
                return (true, null);
            }
            else
            {
                if (!isGPS && existingEntity != null)
                {
                    await _gpsProviderRepo.DeleteAsync(existingEntity);
                    await _gpsProviderRepo.SaveChangesAsync();
                }
                if (isGPS && existingEntity != null)
                {
                    existingEntity.SiteAddress = dto.SiteAddress;
                    existingEntity.Remarks = dto.Remarks;
                    await _gpsProviderRepo.UpdateAsync(existingEntity);
                    await _gpsProviderRepo.SaveChangesAsync();
                }

                return (true, null);
            }
        }

        public async Task<(bool Success, string? Error)> SaveCourierCompanyAsync(CourierCompanyDto dto, bool isCourier)
        {
            var existingEntity = await _courierCompanyRepo.GetByGuidAsync(dto.PersonId);
            if (existingEntity == null && isCourier)
            {
                var gps = new CourierCompany
                {
                    Id = dto.PersonId,
                    Remarks = dto.Remarks,
                    CreatedAt = DateTime.Now,
                    CreateUserId = _currentUserService.UserId
                };
                await _courierCompanyRepo.AddAsync(gps);
                await _courierCompanyRepo.SaveChangesAsync();
                return (true, null);
            }
            else
            {
                if (!isCourier && existingEntity != null)
                {
                    await _courierCompanyRepo.DeleteAsync(existingEntity);
                    await _courierCompanyRepo.SaveChangesAsync();
                }
                if (isCourier && existingEntity != null)
                {
                    existingEntity.Remarks = dto.Remarks;

                    await _courierCompanyRepo.UpdateAsync(existingEntity);
                    await _courierCompanyRepo.SaveChangesAsync();
                }
                return (true, null);
            }
        }

        public async Task<(bool Success, string? Error)> DeleteExtraInfoAsync(Guid id)
        {
            var entity = await _extrainforepository.GetByGuidAsync(id);
            if (entity == null || entity.IsDeleted)
                return (false, "Info not found.");

            entity.IsDeleted = true;
            entity.UpdatedAt = DateTime.Now;
            entity.EditUserId = _currentUserService.UserId;

            await _extrainforepository.UpdateAsync(entity);
            await _extrainforepository.SaveChangesAsync();
            return (true, null);
        }

        public async Task<(bool Success, string? Error)> UpdateExtraInfoAsync(PersonExtraInfoDto dto)
        {
            var entity = await _extrainforepository.GetByGuidAsync(dto.Id.Value);
            if (entity == null || entity.IsDeleted)
                return (false, "Info not found.");


            entity.InfoTypeId = dto.ExtraInfoTypeId.Value;
            entity.PersonExtraInfoValue = dto.PersonExtraInfoValue;

            await _extrainforepository.UpdateAsync(entity);
            await _extrainforepository.SaveChangesAsync();

            //return MapToDto(entity);
            return (true, null);
        }
        public async Task<(bool Success, string? Error)> CreateExtraInfoAsync(PersonExtraInfoDto dto)
        {
            var entity = new PersonExtraInfo
            {
                PersonId = dto.PersonId,
                InfoTypeId = dto.ExtraInfoTypeId.Value,
                PersonExtraInfoValue = dto.PersonExtraInfoValue,
                IsDeleted = false,
                CreateUserId = _currentUserService.UserId,
                CreatedAt = DateTime.Now
            };
            await _extrainforepository.AddAsync(entity);
            await _extrainforepository.SaveChangesAsync();
            return (true, null);
        }

        public async Task<(bool Success, string? Error)> DeleteBankAccountAsync(Guid id)
        {
            var entity = await _bankaccountrepository.GetByGuidAsync(id);
            if (entity == null || entity.IsDeleted)
                return (false, "Bank Account not found.");

            entity.IsDeleted = true;
            entity.UpdatedAt = DateTime.Now;
            entity.EditUserId = _currentUserService.UserId;

            await _bankaccountrepository.UpdateAsync(entity);
            await _bankaccountrepository.SaveChangesAsync();
            return (true, null);

        }


        public async Task<(bool Success, string? Error)> DeleteBranchAsync(Guid id)
        {
            var entity = await _branchrepository.GetByGuidAsync(id);
            if (entity == null || entity.IsDeleted)
                return (false, "Branch not found.");

            entity.IsDeleted = true;
            entity.DeletedAt = DateTime.Now;
            entity.DeleteUserId = _currentUserService.UserId;

            await _branchrepository.UpdateAsync(entity);
            await _branchrepository.SaveChangesAsync();
            return (true, null);

        }





        public async Task<(bool Success, string? Error)> UpdateBankAccountAsync(BankAccountDto dto)
        {
            var entity = await _bankaccountrepository.GetByGuidAsync(dto.Id.Value);
            if (entity == null || entity.IsDeleted)
                return (false, "Info not found.");


            entity.BankAccountPurposeId = dto.BankAccountPurposeId;
            entity.BankId = dto.BankId;
            entity.Remarks = dto.Remarks;
            entity.AccountNumber = dto.AccountNumber;
            entity.Remarks = dto.Remarks;
            entity.BranchName = dto.BranchName;
            entity.BranchAddress = dto.BranchAddress;
            entity.Remarks = dto.Remarks;
            entity.BankAccountTitle = dto.BankAccountTitle;
            entity.BankAccountTitlePersian = dto.BankAccountTitle;
            entity.CurrencyId = dto.CurrencyId;

            await _bankaccountrepository.UpdateAsync(entity);
            await _bankaccountrepository.SaveChangesAsync();

            //return MapToDto(entity);
            return (true, null);

        }
        public async Task<(bool Success, string? Error)> UpdateBranchAsync(BranchDto dto)
        {
            var entity = await _branchrepository.GetByGuidAsync(dto.Id);
            if (entity == null || entity.IsDeleted)
                return (false, "Branch not found.");
            entity.LocationId = dto.LocationId;
            entity.CountryId = dto.CountryId;
            entity.BranchCode = dto.BranchCode;
            entity.BranchName = dto.BranchName;
            entity.ManagerName = dto.ManagerName;
            entity.Address = dto.Address;
            entity.Remarks = "";
            entity.Phone = dto.Phone;
            entity.Email = dto.Email;
            entity.BranchTypeId = dto.BranchTypeId;
            entity.EditUserId = _currentUserService.UserId;
            entity.UpdatedAt = DateTime.Now;
            await _branchrepository.UpdateAsync(entity);
            await _branchrepository.SaveChangesAsync();
            //return MapToDto(entity);
            return (true, null);
        }





        public async Task<(bool Success, string? Error)> CreateBankAccountAsync(BankAccountDto dto)
        {
            var entity = new BankAccount
            {
                PersonId = dto.PersonId,
                CurrencyId = dto.CurrencyId,
                BankAccountPurposeId = dto.BankAccountPurposeId,
                BankId = dto.BankId,
                AccountNumber = dto.AccountNumber,
                BankAccountTitle = dto.BankAccountTitle,
                BankAccountTitlePersian = dto.BankAccountTitle,
                BranchAddress = dto.BranchAddress,
                BranchName = dto.BranchName,
                BranchNo = dto.BranchNo,
                CompanyId = 1,
                IBAN = dto.IBAN,
                Remarks = dto.Remarks,
                IsDeleted = false,
                CreateUserId = _currentUserService.UserId,
                CreatedAt = DateTime.Now
            };
            await _bankaccountrepository.AddAsync(entity);
            await _bankaccountrepository.SaveChangesAsync();
            return (true, null);
        }

        public async Task<(bool Success, string? Error)> CreateBranchAsync(BranchDto dto)
        {
            var entity = new Branch
            {
                LocationId = dto.LocationId,
                ManagerName = dto.ManagerName,
                Email = dto.Email,
                ParentId = dto.PersonId,
                BranchCode = dto.BranchCode,
                BranchName = dto.BranchName,
                BranchTypeId = dto.BranchTypeId,
                CountryId = dto.CountryId,
                Address = dto.Address,
                Phone = dto.Phone,
                IsDeleted = false,
                CreateUserId = _currentUserService.UserId,
                CreatedAt = DateTime.Now
            };
            await _branchrepository.AddAsync(entity);
            await _branchrepository.SaveChangesAsync();
            return (true, null);
        }




        public async Task<IEnumerable<SelectListItem>> SearchNameAsync(string partyDisplayName)
        {
            return await _repository.GetQueryable()
                .Where(x => x.PersonName.Contains(partyDisplayName))
                .OrderBy(x => x.Id)
                .Select(x => new SelectListItem
                {
                    Value = x.Id.ToString(),
                    Text = x.PersonName
                })
                .ToListAsync();
        }

        public async Task<List<int>?> GetSelectedRoleTypes(Guid id)
        {
            return await _personRoleRepo.GetAllAsQueryable().Where(x => x.PersonId == id).Select(x => x.RoleTypeId).ToListAsync();
        }

        public async Task<BranchDto?> GetBranchByIdAsync(Guid id)
        {
            var entity = await _db.Branches
                .Include(x => x.Country)
                .Include(x => x.Location)
                .Include(x => x.BranchType)
                .Where(x => x.Id == id)
                .FirstOrDefaultAsync();
            return entity == null ? null : MapToBranchDto(entity);
        }

        public Task<bool> SoftDeleteAsync(int id)
        {
            throw new NotImplementedException();
        }

        public async Task<IQueryable<Person>> GetPersonsForCurrentUserAsync()
        {
            var userId = _currentUserService.UserId;
            var isAdmin = _currentUserService.IsInRole("Admin");
            if (isAdmin) return _db.Persons;

            return _db.Persons.Where(p =>
                !_db.Customers.Any(c => c.Id == p.Id) ||
                _db.Customers.Any(c => c.Id == p.Id && c.SalesUserId == userId)
            );
        }

        public async Task<string> GetPersonNameAsync(Guid Id)
        {
            var person = await _db.Persons.FindAsync(Id);
            if (person != null) return person.PersonName;
            return String.Empty;
        }
        public async Task<IEnumerable<NLookupDto>> SearchPersonsAsync(string? term, int take = 20)
        {
            if (take <= 0) take = 20;
            if (take > 100) take = 100;
            var query = _db.Persons
                .AsNoTracking()
                .AsQueryable();

            // فیلتر کردن بر اساس عبارت جستجو شده
            if (!string.IsNullOrWhiteSpace(term))
            {
                var trimmedTerm = term.Trim();

                // جستجو همزمان روی PortCode (مانند CNSHA یا IRBND) و PortName
                query = query.Where(p =>
                    p.PersonName.Contains(trimmedTerm) ||
                    (p.NationalID != null && p.NationalID.Contains(trimmedTerm)));
            }

            return await query
                .OrderBy(p => p.PersonName)
                .Take(take)
                .Select(p => new NLookupDto
                {
                    Value = p.Id.ToString(), // تبدیل Id (چه Guid چه int) به string
                    Text = p.PersonName
                })
                .ToListAsync();
        }



    }
}
