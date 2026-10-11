using Microsoft.AspNetCore.Mvc.Rendering;
using ShipRate.Application.DTOs;
using ShipRate.Application.DTOs.Base_Info;
using ShipRate.Domain.Entities.BaseInfo.Persons;

namespace ShipRate.Application.Interfaces.BaseInfo
{
    public interface IPersonService
    {
        Task<PagedResultDto<PersonDto>> GetPagedAsync(PagedRequestDto request);
        //Task<PagedResultDto<PersonDto>> GetPagedAsync(int page, int pageSize, string? search, string? sortBy, string? sortDir);
        Task<PersonDto?> GetByIdAsync(Guid id);
        Task<PersonContactDto?> GetContactByIdAsync(Guid id);
        Task<DTOs.Base_Info.CarrierDto?> GetCarrierByIdAsync(Guid id);
        Task<AgentDto?> GetAgentByIdAsync(Guid id);
        Task<GPSProviderDto?> GetGPSProviderByIdAsync(Guid id);
        Task<CourierCompanyDto?> GetCourierCompanyByIdAsync(Guid id);
        Task<DriverDto?> GetDriverByIdAsync(Guid id);
        Task<PersonExtraInfoDto?> GetExtraInfoByIdAsync(Guid id);
        Task<BankAccountDto?> GetBankAccountByIdAsync(Guid id);
        Task<BranchDto?> GetBranchByIdAsync(Guid id);
        Task<List<int>?> GetSelectedRoleTypes(Guid id);

        //Task<bool> SoftDeleteAsync(int id);
        //Task<bool> CodeExistsAsync(string code, int? excludeId = null);
        //Task<bool> IsCodeUniqueAsync(string code, int? excludeId = null);

        Task<(bool Success, string? Error)> DeleteAsync(Guid id);
        Task<(bool Success, string? Error)> CreateAsync(PersonCreateUpdateDto dto);
        Task<(bool Success, string? Error)> UpdateAsync(PersonCreateUpdateDto dto);
        Task<(bool Success, string? Error)> ToggleActiveAsync(Guid id);


        Task<(bool Success, string? Error)> DeleteContactAsync(Guid id);
        Task<(bool Success, string? Error)> UpdateContactAsync(PersonContactCreateUpdateDto dto);
        Task<(bool Success, string? Error)> CreateContactAsync(PersonContactCreateUpdateDto dto);


        Task<(bool Success, string? Error)> DeleteExtraInfoAsync(Guid id);
        Task<(bool Success, string? Error)> UpdateExtraInfoAsync(PersonExtraInfoDto dto);
        Task<(bool Success, string? Error)> CreateExtraInfoAsync(PersonExtraInfoDto dto);

        Task<(bool Success, string? Error)> DeleteBankAccountAsync(Guid id);
        Task<(bool Success, string? Error)> UpdateBankAccountAsync(BankAccountDto dto);
        Task<(bool Success, string? Error)> CreateBankAccountAsync(BankAccountDto dto);

        Task<(bool Success, string? Error)> DeleteBranchAsync(Guid id);
        Task<(bool Success, string? Error)> UpdateBranchAsync(BranchDto dto);
        Task<(bool Success, string? Error)> CreateBranchAsync(BranchDto dto);




        Task<(bool Success, string? Error)> SaveAgentAsync(AgentDto dto, bool isAgent);
        Task<(bool Success, string? Error)> SaveCarrierAsync(DTOs.Base_Info.CarrierDto dto, bool isCarrier);
        Task<(bool Success, string? Error)> SaveDriverAsync(DriverDto dto, bool isDriver);
        Task<IEnumerable<SelectListItem>> SearchNameAsync(string partyDisplayName);
        Task<IQueryable<Person>> GetPersonsForCurrentUserAsync();
        Task<string> GetPersonNameAsync(Guid Id);
        Task<IEnumerable<NLookupDto>> SearchPersonsAsync(string term, int take);

    }
}
