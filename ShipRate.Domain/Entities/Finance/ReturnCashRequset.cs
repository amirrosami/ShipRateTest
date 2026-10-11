using ShipRate.Domain.Entities.Base;
using ShipRate.Domain.Entities.BaseInfo.Generals;
using ShipRate.Domain.Entities.BaseInfo.Persons;
using ShipRate.Domain.Entities.Settings;

namespace ShipRate.Domain.Entities.Finance
{
    public class ReturnCashRequest : BusinessEntity
    {
        public int CompanyId { get; set; }
        public int InOutId { get; set; }    //درخواست از مشتری برابر 1 و درخواست شرکت از بیرون 2 میباشد

        public Guid PersonId { get; set; }    //طرق مقابل شرکت 

        public string ReqNo { get; set; }
        public DateTime ReqDate { get; set; }

        public Guid PaymentDocumentId { get; set; }
        public decimal CollectValue { get; set; }
        public int ReturnCashStatusId { get; set; }

        //Relations
        public PaymentDocument PaymentDocument { get; set; }
        public Company Company { get; set; }
        public InOut InOut { get; set; }
        public ReturnCashStatus ReturnCashStatus { get; set; }
        public Person Person { get; set; }
    }
}

/* تاریخچه وضعیت مونده هنوز و نحوه پرداخت
 * 
 * 
 * 
 * 
 */