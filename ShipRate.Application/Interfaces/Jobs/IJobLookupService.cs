using Microsoft.AspNetCore.Mvc.Rendering;
using System.Collections;

namespace ShipRate.Application.Interfaces.Jobs
{
    public interface IJobLookupService
    {
        Task<List<SelectListItem>> GetCustomOfficesAsync();
        Task<List<SelectListItem>> GetCompaniesAsync();
        Task<List<SelectListItem>> GetCustomersAsync();
        Task<List<SelectListItem>> GetLocationsAsync();
        Task<List<SelectListItem>> GetTransportTypesAsync();
        Task<List<SelectListItem>> GetTradeTypesAsync();
        Task<List<SelectListItem>> GetLoadTypesAsync();
        Task<List<SelectListItem>> GetCargoHandlingTypesAsync();
        Task<List<SelectListItem>> GetCargoNaturesAsync();
        Task<List<SelectListItem>> GetIncotermsAsync();
        Task<List<SelectListItem>> GetPaymentTypesAsync();
        Task<List<SelectListItem>> GetJobTypesAsync();
        Task<List<SelectListItem>> GetJobStatusesAsync();
        Task<List<SelectListItem>> GetMovementTypesAsync();
        Task<List<SelectListItem>> GetContainerSizeTypesAsync();
        Task<List<SelectListItem>> GetPackageTypesAsync();
        Task<List<SelectListItem>> GetDimUnitsAsync();
        Task<List<SelectListItem>> GetWeightUnitsAsync();
        Task<List<SelectListItem>> GetVolumeUnitsAsync();
        Task<List<SelectListItem>> GetTempUnitsAsync();
        Task<List<SelectListItem>> GetLoadTypesByTransportTypeAsync(int? transportTypeId);
        Task<List<SelectListItem>> GetServiceTypesAsync();
        Task<List<SelectListItem>> GetCompanyRoleTypesAsync();
        Task<List<SelectListItem>> GetRoleTypesAsync();
        Task<List<SelectListItem>> GetChargeTypesAsync();
        Task<List<SelectListItem>> GetCurrenciesAsync();
        Task<List<SelectListItem>> GetJobSpecificPartiesAsync(Guid jobId);

        Task<List<SelectListItem>> GetShippersAsync(Guid? bookingId);
        Task<List<SelectListItem>> GetCarriersAsync(Guid? bookingId);
        Task<List<SelectListItem>> GetDepotsAsync();
        Task<List<SelectListItem>> GetTerminalsAsync();
        Task<List<SelectListItem>> GetRTSStatusesAsync();
        Task<List<SelectListItem>> GetCntOwnTypesAsync();

        Task<List<SelectListItem>> GetTruckTypesAsync();


        Task<List<SelectListItem>> GetShipmentLegsAsync(Guid jobId);
        Task<List<SelectListItem>> GetJobSourceTypesAsync();
        Task<List<SelectListItem>> GetSalesUsersAsync();
        Task<List<SelectListItem>> GetCargoCommoditiesAsync();
        Task<List<SelectListItem>> GetFlightsAsync();
        Task<List<SelectListItem>> GetFlightDatesAsync();
        Task<List<SelectListItem>> GetPersonsAsync();
        Task<IEnumerable> GetBookingStatusesAsync();
        Task<IEnumerable> GetPartiesAsync();
        Task<IEnumerable> GetContainerOwnTypesAsync();
        Task<List<SelectListItem>> GetVesselsAsync();
        Task<List<SelectListItem>> GetVoyagesAsync();
        Task<List<SelectListItem>> GetAirportsAsync();
        Task<List<SelectListItem>> GetAirlinesAsync();
        Task<List<SelectListItem>> GetBLTypesByTransportTypeIdAsync(int? transportTypeId);
        Task<IEnumerable<SelectListItem>> GetDeclarationTypesAsync();
        Task<List<SelectListItem>> GetFinanceDirectionsForInvoiceContextAsync();
        Task<List<SelectListItem>> GetFullMTsAsync();
    }
}
