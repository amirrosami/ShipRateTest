using ShipRate.Application.DTOs.ApiIr;

namespace ShipRate.Application.Interfaces.ExternalServices
{
    public interface IApiIrService
    {
        Task<PersonInfoResponse> PersonInfoAsync(PersonInfoRequest request, CancellationToken ct = default);
        Task<CompanyInfoResponse> CompanyInfoAsync(CompanyInfoRequest request, CancellationToken ct = default);
        Task<ChequeInfoResponse> ChequeInfoAsync(ChequeInfoRequest request, CancellationToken ct = default);

    }
}
