using ShipRate.Application.DTOs;
using ShipRate.Application.DTOs.Locations;
using ShipRate.Domain.Enums;

namespace ShipRate.Application.Interfaces.Location
{
    /// <summary>
    /// سرویس مدیریت Location Hub
    /// هم‌الگو کامل با ICityService و ICountryService
    /// </summary>
    public interface ILocationService
    {
        // ═══════════════════════════════════════════════════════════════════
        //  Paging & Search (Index Page)
        // ═══════════════════════════════════════════════════════════════════

        /// <summary>
        /// دریافت صفحه‌بندی شده Location ها با قابلیت جستجو و فیلتر
        /// </summary>
        Task<PagedResultDto<LocationListItemDto>> GetPagedLocationsAsync(
            int pageNumber = 1,
            int pageSize = 10,
            string? searchTerm = null,
            int? countryId = null,
            EnumLocationType? locationType = null,
            string? sortColumn = null,
            string? sortDirection = null);

        // ═══════════════════════════════════════════════════════════════════
        //  Read Operations
        // ═══════════════════════════════════════════════════════════════════

        /// <summary>
        /// دریافت یک Location با تمام جزئیات (برای Edit/Detail)
        /// </summary>
        Task<LocationDto?> GetLocationByIdAsync(Guid id);

        /// <summary>
        /// دریافت تمام Locationها (بدون صفحه‌بندی)
        /// </summary>
        Task<IEnumerable<LocationListItemDto>> GetAllLocationsAsync();

        /// <summary>
        /// دریافت Locationهای یک کشور خاص
        /// </summary>
        Task<IEnumerable<LocationListItemDto>> GetLocationsByCountryIdAsync(int countryId);

        /// <summary>
        /// دریافت Locationها بر اساس نوع
        /// </summary>
        Task<IEnumerable<LocationListItemDto>> GetLocationsByTypeAsync(EnumLocationType locationType);

        // ═══════════════════════════════════════════════════════════════════
        //  CRUD Operations
        // ═══════════════════════════════════════════════════════════════════

        /// <summary>
        /// ایجاد Location جدید (از Step 2 Wizard فراخوانی می‌شود)
        /// </summary>
        Task<LocationDto> CreateLocationAsync(LocationDto locationDto);

        /// <summary>
        /// ویرایش Location موجود
        /// </summary>
        Task<bool> UpdateLocationAsync(LocationDto locationDto);

        /// <summary>
        /// حذف منطقی Location (IsActive = false)
        /// </summary>
        Task<bool> DeleteLocationAsync(Guid id);

        /// <summary>
        /// حذف فیزیکی Location (استفاده محتاطانه)
        /// </summary>
        Task<bool> PermanentDeleteLocationAsync(Guid id);

        // ═══════════════════════════════════════════════════════════════════
        //  Validation & Business Logic
        // ═══════════════════════════════════════════════════════════════════

        /// <summary>
        /// بررسی یکتا بودن کد Location در یک کشور
        /// </summary>
        Task<bool> IsCodeUniqueAsync(string locationCode, int countryId, Guid? excludeId = null);


        /// <summary>
        /// تغییر وضعیت فعال/غیرفعال
        /// </summary>
        Task<bool> ToggleActiveStatusAsync(Guid id);
        Task<IEnumerable<NLookupDto>> SearchPortsAsync(string term, int take);

        Task<IEnumerable<NLookupDto>> SearchPortsUsingCacheAsync(string term, int take, CancellationToken ct = default);
        void InvalidateLocationCache(); // برای زمان‌هایی که لوکیشن جدید ثبت یا ویرایش می‌شود


    }
}
