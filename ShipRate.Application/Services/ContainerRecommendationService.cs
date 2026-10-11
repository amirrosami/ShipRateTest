using ShipRate.Application.Interfaces.BaseInfo;
using ShipRate.Domain.Entities.BaseInfo.ContainerSpec;

namespace ShipRate.Application.Services
{
    public class CargoDetailsInput
    {
        public decimal TotalWeightKg { get; set; } // وزن کل بار
        public decimal TotalVolumeCbm { get; set; } // حجم کل بار (اگر مستقیماً وارد شود)

        // یا بر اساس ابعاد بسته‌ها
        public decimal? PackageLengthCm { get; set; }
        public decimal? PackageWidthCm { get; set; }
        public decimal? PackageHeightCm { get; set; }
        public int? PackageQuantity { get; set; }

        public bool RequiresTemperatureControl { get; set; } // نیاز به یخچال دارد؟
        public string CargoType { get; set; } // نوع بار (عمومی، خطرناک، فله و...)
    }
    public class ContainerRecommendationResult
    {
        public ContainerSizeType Container { get; set; }
        public int RequiredQuantity { get; set; }
        public decimal VolumeUtilizationPercentage { get; set; }
        public decimal WeightUtilizationPercentage { get; set; }
        public string RecommendationNotes { get; set; }
        public string? ErrorMessage { get; set; }
    }
    public interface IContainerRecommendationService
    {
        Task<ContainerRecommendationResult> RecommendContainer(CargoDetailsInput cargo);
    }


    public class ContainerRecommendationService : IContainerRecommendationService
    {
        private readonly IBaseDataService _baseDataService;
        public ContainerRecommendationService(IBaseDataService baseDataService)
        {
            _baseDataService = baseDataService;
        }

        private IEnumerable<ContainerSizeType> _standardContainers;

        public async Task<ContainerRecommendationResult> RecommendContainer(CargoDetailsInput cargo)
        {
            _standardContainers = await _baseDataService.GetContainerSizeTypesAsync();

            // ۱. محاسبه حجم کل در صورت وارد کردن ابعاد بسته
            decimal volume = (decimal?)cargo.TotalVolumeCbm ?? 0m;

            if (cargo.PackageLengthCm.HasValue && cargo.PackageWidthCm.HasValue && cargo.PackageHeightCm.HasValue && cargo.PackageQuantity.HasValue)
            {
                // استفاده از 100m برای سازگاری با decimal
                decimal singlePackageVol = (cargo.PackageLengthCm.Value / 100m) *
                                           (cargo.PackageWidthCm.Value / 100m) *
                                           (cargo.PackageHeightCm.Value / 100m);

                volume = singlePackageVol * cargo.PackageQuantity.Value;
            }

            // ۲. فیلتر کردن بر اساس نیاز سرمایشی (IsReefer در مدل شما get-only است)
            var eligibleContainers = _standardContainers
                .Where(c => c.IsReefer == cargo.RequiresTemperatureControl)
                .ToList();

            if (!eligibleContainers.Any()) return null;

            ContainerRecommendationResult bestFit = null;
            decimal bestEfficiency = 0;

            foreach (var container in eligibleContainers)
            {
                // فرض ۱۵٪ فضای مرده در کانتینر
                decimal effectiveVolumeCbm = (container.MaxVolumeCBM ?? 0m) * 0.85m;
                decimal maxPayload = (decimal?)container.MaxPayLoadKg ?? 0m;

                if (effectiveVolumeCbm <= 0 || maxPayload <= 0) continue;

                // محاسبه تعداد مورد نیاز
                // Math.Ceiling خروجی double می‌دهد، پس ابتدا تقسیم decimal را انجام داده و سپس به double کست می‌کنیم
                int qtyNeededByVolume = (int)Math.Ceiling((double)(volume / effectiveVolumeCbm));
                int qtyNeededByWeight = (int)Math.Ceiling((double)(cargo.TotalWeightKg / maxPayload));

                int totalContainersNeeded = Math.Max(qtyNeededByVolume, qtyNeededByWeight);
                if (totalContainersNeeded <= 0) totalContainersNeeded = 1;

                // محاسبه بهره‌وری
                decimal totalVolCapacity = (container.MaxVolumeCBM ?? 0m) * totalContainersNeeded;
                decimal totalWeightCapacity = maxPayload * totalContainersNeeded;

                decimal volUtil = (volume / totalVolCapacity) * 100m;
                decimal weightUtil = (cargo.TotalWeightKg / totalWeightCapacity) * 100m;

                decimal avgUtilization = (volUtil + weightUtil) / 2m;

                if (volUtil <= 100m && weightUtil <= 100m)
                {
                    if (bestFit == null || avgUtilization > bestEfficiency)
                    {
                        bestEfficiency = avgUtilization;
                        bestFit = new ContainerRecommendationResult
                        {
                            Container = container,
                            RequiredQuantity = totalContainersNeeded,
                            VolumeUtilizationPercentage = Math.Round(volUtil, 2),
                            WeightUtilizationPercentage = Math.Round(weightUtil, 2),
                            RecommendationNotes = totalContainersNeeded > 1
                                                ? $"Your cargo requires {totalContainersNeeded} containers due to its high volume/weight."
                                                : "Your cargo fits well into a single container."
                        };
                    }
                }
            }
            // ۳. اگر در حالت بهینه پیدا نشد (بار بسیار بزرگ)، بزرگترین کانتینر را پیشنهاد بده
            if (bestFit == null)
            {
                var largestContainer = eligibleContainers.OrderByDescending(c => c.MaxVolumeCBM).First();
                decimal maxVol = largestContainer.MaxVolumeCBM ?? 1m;
                decimal maxWeight = (decimal?)largestContainer.MaxPayLoadKg ?? 1m;

                int qtyByVol = (int)Math.Ceiling((double)(volume / (maxVol * 0.85m)));
                int qtyByWeight = (int)Math.Ceiling((double)(cargo.TotalWeightKg / maxWeight));
                int totalNeeded = Math.Max(qtyByVol, qtyByWeight);

                bestFit = new ContainerRecommendationResult
                {
                    Container = largestContainer,
                    RequiredQuantity = totalNeeded,
                    VolumeUtilizationPercentage = Math.Round((volume / (maxVol * totalNeeded)) * 100m, 2),
                    WeightUtilizationPercentage = Math.Round((cargo.TotalWeightKg / (maxWeight * totalNeeded)) * 100m, 2),
                    RecommendationNotes = $"Cargo size exceeds standard limits. {totalNeeded} containers are required."
                };
            }
            return bestFit;
        }
    }


}
