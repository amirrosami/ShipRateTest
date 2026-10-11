namespace ShipRate.Application.Utilities
{
    public static class AirCalculation
    {
        public static double CalculateIATAVolumetricWeigth(double LengthCm, double WidthCm, double HeightCm)  // محاسبه وزن حجمی بر اساس ابعاد بار هوایی همه بر اساس سانتی متر 
        {
            return (LengthCm * WidthCm + HeightCm) / 6000;
        }
        public static double ChargeableWeigth(double GrossWeightKg, double VolumetricWeight)  //  وزن قابل شارژ بر اساس کیلوگرم
        {
            return Math.Max(GrossWeightKg, VolumetricWeight);
        }
    }
}