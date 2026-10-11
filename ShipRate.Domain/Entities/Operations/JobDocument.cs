using ShipRate.Domain.Entities.Base;
using ShipRate.Domain.Entities.DocumentStore;

namespace ShipRate.Domain.Entities.Operations
{
    public class JobDocument : BusinessEntity
    {
        public Guid JobId { get; set; }
        public Guid DocumentId { get; set; }
        public string DocumentNumber { get; set; }
        public string DocumentTitle { get; set; }
        public DateTime DocumentDate { get; set; }
        public int VersionNumber { get; set; }
        public string IssuedBy { get; set; }
        public string Remarks { get; set; }
        //Relations
        public Job Job { get; set; }
        public Document Document { get; set; }
    }
}
