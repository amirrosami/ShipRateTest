using ShipRate.Common.Notifications.Models;
using ShipRate.Domain.Entities.Finance;

namespace ShipRate.WebAppCustomers.ViewModels
{
    public class DashboardViewModel
    {
        public List<RateRequestItemViewModel> RateRequests { get; set; }
        public List<Invoice> Invoices{ get; set; }

        public List<Notification> Notifications { get; set; }
    }

}