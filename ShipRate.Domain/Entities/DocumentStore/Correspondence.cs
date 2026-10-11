using ShipRate.Domain.Entities.Base;
using ShipRate.Domain.Entities.BaseInfo.Generals;
using ShipRate.Domain.Entities.BaseInfo.Persons;
using ShipRate.Domain.Entities.Settings;

namespace ShipRate.Domain.Entities.DocumentStore
{
    public class Correspondence : BusinessEntity
    {
        public int CompanyId { get; set; }
        public int InOutId { get; set; }
        public Guid? SenderReceiverId { get; set; }
        public string? SenderReceiverTitle { get; set; }
        public string CorrespondenceNo { get; set; }
        public DateTime CorrespondenceDate { get; set; }
        public int? SubjectId { get; set; }
        public string SubjectFreeText { get; set; }
        public int? SystemObjectTypeId { get; set; }
        public Guid? ObjectId { get; set; }
        public string Keywords { get; set; }

        //Relations
        public Company Company { get; set; }
        public InOut InOut { get; set; }
        public Person SenderReceiver { get; set; }
        public CorrespondenceSubject Subject { get; set; }
        public SystemObjectType SystemObjectType { get; set; }
    }
}
/*مکاتبات
 * 
 * 
 * 
 * 
 */