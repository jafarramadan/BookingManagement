using BookingManagement.BL.Interfaces;
using BookingManagement.BL.Validation;
using BookingManagement.Common.DTOs.V1;
using BookingManagement.Common.Exceptions;
using BookingManagement.Common.Models;
using BookingManagement.DAL.Interfaces;
using BookingManagement.Domain.Entities;
using BookingManagement.Common.Enums;
using BookingManagement.Domain.ValueObjects;

namespace BookingManagement.BL.Implementation
{
    public class BookingService : IBookingService
    {
        private readonly IBookingRepository _bookingRepository;
        private readonly IAuditLogRepository _auditLogRepository;
        private readonly IUnitOfWork _unitOfWork;

        public BookingService(
            IBookingRepository bookingRepository,
            IAuditLogRepository auditLogRepository,
            IUnitOfWork unitOfWork)
        {
            _bookingRepository = bookingRepository;
            _auditLogRepository = auditLogRepository;
            _unitOfWork = unitOfWork;
        }

        public async Task<BookingDto> CreateAsync(CreateBookingRequest request)
        {
            BookingValidator.ValidateCreateRequest(request);

            var resourceId = request.ResourceId!.Trim();
            var slot = new TimeRange(request.StartDateTime, request.EndDateTime);

            if (await _bookingRepository.HasActiveOverlapAsync(resourceId, slot))
            {
                throw new ConflictException(
                    $"Resource '{resourceId}' is already booked between {slot.Start:O} and {slot.End:O}.");
            }

            var booking = Booking.Create(resourceId, request.UserId!.Trim(), slot, DateTime.UtcNow);

            _bookingRepository.Add(booking);
            _auditLogRepository.Add(AuditLog.For(booking, AuditEventType.BookingCreated, booking.CreatedAt));

            await _unitOfWork.SaveChangesAsync();

            return ToDto(booking);
        }

        public async Task<BookingDto> GetByIdAsync(Guid bookingId)
        {
            var booking = await GetExistingBookingAsync(bookingId);

            return ToDto(booking);
        }

        public async Task<BookingDto> CancelAsync(Guid bookingId)
        {
            var booking = await GetExistingBookingAsync(bookingId);

            if (!booking.IsActive)
            {
                throw new ConflictException($"Booking '{bookingId}' is already cancelled.");
            }

            booking.Cancel(DateTime.UtcNow);
            _auditLogRepository.Add(AuditLog.For(booking, AuditEventType.BookingCancelled, booking.CancelledAt!.Value));

            await _unitOfWork.SaveChangesAsync();

            return ToDto(booking);
        }

        public async Task<PagedResult<BookingDto>> GetForResourceAsync(
            string resourceId,
            BookingQueryRequest query)
        {
            BookingValidator.ValidateResourceId(resourceId);
            BookingValidator.ValidateBookingQuery(query);

            var bookings = await _bookingRepository.GetForResourceAsync(
                resourceId.Trim(),
                query.From,
                query.To,
                query.IncludeCancelled,
                query.Page,
                query.PageSize);

            return new PagedResult<BookingDto>(
                bookings.Items.Select(ToDto).ToList(),
                bookings.Page,
                bookings.PageSize,
                bookings.TotalCount);
        }

        private async Task<Booking> GetExistingBookingAsync(Guid bookingId)
        {
            return await _bookingRepository.GetByIdAsync(bookingId)
                   ?? throw new NotFoundException($"Booking '{bookingId}' was not found.");
        }

        private static BookingDto ToDto(Booking booking)
        {
            return new BookingDto
            {
                Id = booking.Id,
                ResourceId = booking.ResourceId,
                UserId = booking.UserId,
                StartDateTime = booking.StartDateTime,
                EndDateTime = booking.EndDateTime,
                Status = booking.Status.ToString(),
                CreatedAt = booking.CreatedAt,
                CancelledAt = booking.CancelledAt
            };
        }
    }
}
