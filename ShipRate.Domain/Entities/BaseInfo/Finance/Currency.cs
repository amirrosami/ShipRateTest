using ShipRate.Domain.Entities.Base;

namespace ShipRate.Domain.Entities.BaseInfo.Finance
{
    public class Currency : SystemEntity
    {
        public int? Sort { get; set; }
        public string CurrencyCode { get; set; } = string.Empty;
        public string CurrencyName { get; set; } = string.Empty;
        public string? LocalName { get; set; } = string.Empty;
        public string? Symbol { get; set; } = string.Empty;
        public int? DecimalPlaces { get; set; }

        //Relations
        //public List<BankAccount>? BankAccounts { get; set; }
    }
}



/*بحث : آی دی کشور حذف شد . دلیل : توی کشورها دارم 
 * همچنین ارزی مثل یورو متعلق به اتحادیه اروپاست نه کشور خاص
 * 
 * 
 */
