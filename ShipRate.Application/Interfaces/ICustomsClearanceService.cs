using ShipRate.Application.ViewModels;

namespace ShipRate.Application.Interfaces
{
    public interface ICustomsClearanceService
    {
        Task<CustomsClearanceJobTabVm> GetJobTabAsync(Guid jobId);
        Task<CustomsClearanceCreateEditVm> BuildCreateVmAsync(Guid jobId, int companyId);
        Task<CustomsClearanceCreateEditVm?> BuildEditVmAsync(Guid id, int companyId);
        Task CreateAsync(CustomsClearanceCreateEditVm model);
        Task UpdateAsync(CustomsClearanceCreateEditVm model);
        Task DeleteAsync(Guid id, int companyId);


        Task<CustomsClearanceDetailsVm?> GetDetailsAsync(Guid id, int companyId);
        Task<CustomsClearanceCreateEditVm?> GetGeneralTabVmAsync(Guid id, int companyId);
        Task UpdateGeneralInfoAsync(CustomsClearanceCreateEditVm model, int companyId);

    }

    public interface ICustomsClearanceItemService
    {
        Task<CustomsClearanceItemsTabVm> GetItemsTabAsync(Guid customsClearanceId, int companyId);
        Task<CustomsClearanceItemCreateEditVm> BuildCreateVmAsync(Guid customsClearanceId, int companyId);
        Task<CustomsClearanceItemCreateEditVm?> BuildEditVmAsync(Guid id, int companyId);
        Task PopulateLookupsAsync(CustomsClearanceItemCreateEditVm vm, int companyId);
        Task CreateAsync(CustomsClearanceItemCreateEditVm model, int companyId);
        Task UpdateAsync(CustomsClearanceItemCreateEditVm model, int companyId);
        Task DeleteAsync(Guid id, int companyId);
    }


    public interface ICustomsClearanceContainerService
    {
        Task<CustomsClearanceContainersTabVm> GetContainersTabAsync(Guid customsClearanceId, int companyId);
        Task<CustomsClearanceContainerCreateEditVm> BuildCreateVmAsync(Guid customsClearanceId, int companyId);
        Task<CustomsClearanceContainerCreateEditVm?> BuildEditVmAsync(Guid id, int companyId);
        Task PopulateLookupsAsync(CustomsClearanceContainerCreateEditVm vm, int companyId);
        Task CreateAsync(CustomsClearanceContainerCreateEditVm model, int companyId);
        Task UpdateAsync(CustomsClearanceContainerCreateEditVm model, int companyId);
        Task DeleteAsync(Guid id, int companyId);
    }



    public interface ICustomsClearanceChargeService
    {
        Task<CustomsClearanceChargesTabVm> GetChargesTabAsync(Guid customsClearanceId, int companyId);
        Task<CustomsClearanceChargeCreateEditVm> BuildCreateVmAsync(Guid customsClearanceId, int companyId);
        Task<CustomsClearanceChargeCreateEditVm?> BuildEditVmAsync(Guid id, int companyId);
        Task PopulateLookupsAsync(CustomsClearanceChargeCreateEditVm vm, int companyId);
        Task CreateAsync(CustomsClearanceChargeCreateEditVm model, int companyId);
        Task UpdateAsync(CustomsClearanceChargeCreateEditVm model, int companyId);
        Task DeleteAsync(Guid id, int companyId);
    }

    public interface ICustomsClearanceTimelineService
    {
        Task<CustomsClearanceTimelineTabVm> GetTimelineTabAsync(Guid customsClearanceId, int companyId);
    }


    public interface ICustomsClearanceLicenseService
    {
        Task<CustomsClearanceLicensesTabVm> GetLicensesTabAsync(Guid customsClearanceId, int companyId);
        Task<CustomsClearanceLicenseCreateEditVm> BuildCreateVmAsync(Guid customsClearanceId, int companyId);
        Task<CustomsClearanceLicenseCreateEditVm> BuildEditVmAsync(Guid id, int companyId);

        Task CreateAsync(CustomsClearanceLicenseCreateEditVm vm, int companyId);
        Task UpdateAsync(CustomsClearanceLicenseCreateEditVm vm, int companyId);
        Task DeleteAsync(Guid id, int companyId);
    }




    public interface ICustomsInspectionService
    {
        Task<CustomsInspectionsTabVm> GetInspectionsTabAsync(Guid customsClearanceId, int companyId);
        Task<CustomsInspectionCreateEditVm> BuildCreateVmAsync(Guid customsClearanceId, int companyId);
        Task<CustomsInspectionCreateEditVm> BuildEditVmAsync(Guid id, int companyId);
        Task CreateAsync(CustomsInspectionCreateEditVm vm, int companyId);
        Task UpdateAsync(CustomsInspectionCreateEditVm vm, int companyId);

        // متد حذف اکنون آی‌دی والد را برمی‌گرداند
        Task<Guid> DeleteAsync(Guid id, int companyId);

        Task PopulateLookupsAsync(CustomsInspectionCreateEditVm vm, int companyId);

        // متد عمومی برای چک کردن وضعیت ویرایش‌پذیری از بیرون سرویس در صورت نیاز
        Task VerifyClearanceIsEditableAsync(Guid customsClearanceId, int companyId);
    }

}
