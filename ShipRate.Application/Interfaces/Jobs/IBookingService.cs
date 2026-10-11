using ShipRate.Application.DTOs;
using ShipRate.Application.ViewModels;
using ShipRate.Application.ViewModels.BookingConsoleVms;

namespace ShipRate.Application.Interfaces.Jobs
{
    public interface IBookingService
    {
        Task<BookingFormVm> GetBookingForEditAsync(Guid bookingId);
        Task<byte[]> PrintRTSInstructionLetterPdfAsync(Guid id);
        Task<Guid> SaveBookingAsync(BookingFormVm model);


        Task<BookingConsoleBookingSummaryVm?> GetBookingSummaryAsync(Guid bookingId, CancellationToken cancellationToken = default);
        Task<BookingConsoleVm> GetBookingConsoleAsync(Guid id);
        Task<List<InvoiceRowVm>> GetInvoicesByBookingIdAsync(Guid bookingId, string filterType);
        Task<BookingConsoleDetentionsVm?> GetDetentionsByBookingAsync(Guid bookingId, CancellationToken cancellationToken);

    }
}
