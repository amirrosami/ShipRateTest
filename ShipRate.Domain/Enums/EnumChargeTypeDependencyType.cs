namespace ShipRate.Domain.Enums
{
    public enum EnumChargeTypeDependencyType
    {
        None = 0,
        Conditional = 1,   // هزینه‌ی شرطی
        Composite = 2,     // هزینه‌ی ترکیبی
        Multiplier = 3,    // هزینه‌ی ضربی
        Alternative = 4,   // هزینه‌ی جایگزین
        ServiceBased = 5   // وابسته به سرویس/کانتینر
    }
}
