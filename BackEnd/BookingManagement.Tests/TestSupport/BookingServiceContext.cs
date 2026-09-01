using BookingManagement.BL.Implementation;
using BookingManagement.Domain.Entities;

namespace BookingManagement.Tests.TestSupport
{
    internal class BookingServiceContext
    {
        private readonly InMemoryBookingRepository _bookingRepository;
        private readonly InMemoryAuditLogRepository _auditLogRepository;
        private readonly FakeUnitOfWork _unitOfWork;

        public BookingServiceContext(params Booking[] bookings)
        {
            _bookingRepository = new InMemoryBookingRepository(bookings);
            _auditLogRepository = new InMemoryAuditLogRepository();
            _unitOfWork = new FakeUnitOfWork();

            Service = new BookingService(_bookingRepository, _auditLogRepository, _unitOfWork);
        }

        public BookingService Service { get; }

        public IReadOnlyList<Booking> Bookings => _bookingRepository.Bookings;

        public IReadOnlyList<AuditLog> AuditLogs => _auditLogRepository.AuditLogs;

        public int SaveChangesCount => _unitOfWork.SaveChangesCount;

        public BookingQueryArguments? LastResourceQuery => _bookingRepository.LastResourceQuery;
    }
}
