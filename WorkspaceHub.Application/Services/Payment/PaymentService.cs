using AutoMapper;
using WorkspaceHub.Application.DTOs.Payment;
using WorkspaceHub.Application.Exceptions;
using WorkspaceHub.Application.Interfaces;
using WorkspaceHub.Application.Interfaces.GenericRepository;
using WorkspaceHub.Application.Interfaces.Persistence;
using WorkspaceHub.Application.Interfaces.Services.Payment;
using WorkspaceHub.Domain.Entities;
using WorkspaceHub.Domain.Enums;

namespace WorkspaceHub.Application.Services
{
    public class PaymentService : IPaymentService
    {
        private readonly IGenericRepository<Payment> _paymentRepository;
        private readonly IGenericRepository<Booking> _bookingRepository;
        private readonly IMapper _mapper;
        private readonly IUnitOfWork _unitOfWork;
        private readonly ICurrentUserService _currentUserService;

        public PaymentService(
            IGenericRepository<Payment> paymentRepository,
            IGenericRepository<Booking> bookingRepository,
            IMapper mapper,
            IUnitOfWork unitOfWork,
            ICurrentUserService currentUserService)
        {
            _paymentRepository = paymentRepository;
            _bookingRepository = bookingRepository;
            _mapper = mapper;
            _unitOfWork = unitOfWork;
            _currentUserService = currentUserService;
        }

        public async Task<PaymentResponseDto> GetByBookingAsync(
            int bookingId,
            CancellationToken cancellationToken = default)
        {
            var tenantId = _currentUserService.GetRequiredTenantId();

            var booking = await _bookingRepository.GetByIdAsync(
                bookingId,
                x => x.TenantId == tenantId,
                cancellationToken);

            if (booking is null)
                throw new NotFoundException("Booking not found.");

            return await BuildPaymentSummaryAsync(
                booking,
                tenantId,
                cancellationToken);
        }

        public async Task<PaymentResponseDto> CreateAsync(
            CreatePaymentRequest request,
            CancellationToken cancellationToken = default)
        {
            var tenantId = _currentUserService.GetRequiredTenantId();

            if (!Enum.IsDefined(request.PaymentMethod))
            {
                throw new ConflictException(
                    "Invalid payment method.");
            }


            if (request.Amount <= 0)
            {
                throw new ConflictException(
                    "Payment amount must be greater than zero.");
            }

            var booking = await _bookingRepository.GetByIdAsync(
                request.BookingId,
                x => x.TenantId == tenantId,
                cancellationToken);

            if (booking is null)
                throw new NotFoundException("Booking not found.");

            if (booking.Status == BookingStatus.Cancelled)
            {
                throw new ConflictException(
                    "Cannot add payment to a cancelled booking.");
            }

            var payments = await _paymentRepository.GetAllAsync(
                x => x.BookingId == booking.Id &&
                     x.TenantId == tenantId,
                cancellationToken);

            var totalPaid = payments.Sum(x => x.AppliedAmount);

            var remainingAmount =
                Math.Max(booking.TotalAmount - totalPaid, 0);

            if (remainingAmount <= 0)
            {
                throw new ConflictException(
                    "Booking is already fully paid.");
            }

            var appliedAmount =
                Math.Min(request.Amount, remainingAmount);

            var overpaymentAmount =
                request.Amount - appliedAmount;

            var payment = new Payment
            {
                TenantId = tenantId,
                BookingId = request.BookingId,

                Amount = request.Amount,

                AppliedAmount = appliedAmount,

                PaymentMethod = request.PaymentMethod,
                PaymentDate = DateTime.UtcNow,
                TransactionId = request.TransactionId,
                Notes = request.Notes
            };

            await _paymentRepository.AddAsync(
                payment,
                cancellationToken);

            var newTotalPaid =
                totalPaid + appliedAmount;

            booking.PaymentStatus =
                CalculatePaymentStatus(
                    newTotalPaid,
                    booking.TotalAmount);

            _bookingRepository.Update(booking);

            await _unitOfWork.SaveChangesAsync(
                cancellationToken);

            return await BuildPaymentSummaryAsync(
                booking,
                tenantId,
                cancellationToken);
        }

        public async Task<PaymentResponseDto> DeleteAsync(
            int id,
            CancellationToken cancellationToken = default)
        {
            var tenantId =
                _currentUserService.GetRequiredTenantId();

            var payment = await _paymentRepository.GetByIdAsync(
                id,
                x => x.TenantId == tenantId,
                cancellationToken);

            if (payment is null)
                throw new NotFoundException(
                    "Payment not found.");

            var booking = await _bookingRepository.GetByIdAsync(
                payment.BookingId,
                x => x.TenantId == tenantId,
                cancellationToken);

            if (booking is null)
                throw new NotFoundException(
                    "Booking not found.");

            _paymentRepository.Delete(payment);

            var payments = await _paymentRepository.GetAllAsync(
                x => x.BookingId == booking.Id &&
                     x.TenantId == tenantId &&
                     x.Id != id,
                cancellationToken);

            var totalPaid =
                payments.Sum(x => x.AppliedAmount);

            booking.PaymentStatus =
                CalculatePaymentStatus(
                    totalPaid,
                    booking.TotalAmount);

            _bookingRepository.Update(booking);

            await _unitOfWork.SaveChangesAsync(
                cancellationToken);

            return await BuildPaymentSummaryAsync(
                booking,
                tenantId,
                cancellationToken);
        }

        private async Task<PaymentResponseDto> BuildPaymentSummaryAsync(
            Booking booking,
            int tenantId,
            CancellationToken cancellationToken)
        {
            var payments = await _paymentRepository.GetAllAsync(
                x => x.BookingId == booking.Id &&
                     x.TenantId == tenantId,
                cancellationToken);

            var totalReceived =
                payments.Sum(x => x.Amount);

            var totalPaid =
                payments.Sum(x => x.AppliedAmount);

            var remainingAmount =
                Math.Max(
                    booking.TotalAmount - totalPaid,
                    0);

            var totalOverpayment =
                Math.Max(
                    totalReceived - totalPaid,
                    0);

            var paymentItems =
                payments.Select(payment =>
                    new PaymentItemResponseDto
                    {
                        Id = payment.Id,
                        Amount = payment.Amount,
                        AppliedAmount = payment.AppliedAmount,

                        OverpaymentAmount =
                            Math.Max(
                                payment.Amount -
                                payment.AppliedAmount,
                                0),

                        PaymentMethod =
                            payment.PaymentMethod,

                        PaymentDate =
                            payment.PaymentDate,

                        TransactionId =
                            payment.TransactionId,

                        Notes =
                            payment.Notes,

                        BookingId =
                            payment.BookingId
                    })
                    .ToList();

            return new PaymentResponseDto
            {
                BookingId = booking.Id,

                TotalAmount =
                    booking.TotalAmount,

                TotalPaid =
                    totalPaid,

                TotalReceived =
                    totalReceived,

                RemainingAmount =
                    remainingAmount,

                TotalOverpayment =
                    totalOverpayment,

                PaymentStatus =
                    booking.PaymentStatus,

                Payments =
                    paymentItems
            };
        }

        private static PaymentStatus CalculatePaymentStatus(
            decimal totalPaid,
            decimal totalAmount)
        {
            if (totalPaid <= 0)
                return PaymentStatus.Pending;

            if (totalPaid >= totalAmount)
                return PaymentStatus.Paid;

            return PaymentStatus.PartiallyPaid;
        }
    }
}