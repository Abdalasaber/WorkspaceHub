using AutoMapper;
using WorkspaceHub.Application.DTOs.Booking;
using WorkspaceHub.Application.Exceptions;
using WorkspaceHub.Application.Interfaces;
using WorkspaceHub.Application.Interfaces.GenericRepository;
using WorkspaceHub.Application.Interfaces.Persistence;
using WorkspaceHub.Application.Interfaces.Repository;
using WorkspaceHub.Application.Interfaces.Services;
using WorkspaceHub.Domain.Entities;
using WorkspaceHub.Domain.Enums;

namespace WorkspaceHub.Application.Services;

public class BookingService : IBookingService
{
    private readonly IBookingRepository _bookingRepository;
    private readonly IGenericRepository<Tenant> _tenantRepository;
    private readonly IGenericRepository<Payment> _paymentRepository;
    private readonly IGenericRepository<Customer> _customerRepository;
    private readonly IGenericRepository<Workspace> _workspaceRepository;
    private readonly IGenericRepository<WorkspacePricing> _pricingRepository;

    private readonly IMapper _mapper;
    private readonly IUnitOfWork _unitOfWork;
    private readonly ICurrentUserService _currentUserService;

    public BookingService(
        IBookingRepository bookingRepository,
        IGenericRepository<Tenant> tenantRepository,
        IGenericRepository<Payment> paymentRepository,
        IGenericRepository<Customer> customerRepository,
        IGenericRepository<Workspace> workspaceRepository,
        IGenericRepository<WorkspacePricing> pricingRepository,
        IMapper mapper,
        IUnitOfWork unitOfWork,
        ICurrentUserService currentUserService
        )
    {
        _bookingRepository = bookingRepository;
        _tenantRepository = tenantRepository;
        _paymentRepository = paymentRepository;
        _customerRepository = customerRepository;
        _workspaceRepository = workspaceRepository;
        _pricingRepository = pricingRepository;
        _mapper = mapper;
        _unitOfWork = unitOfWork;
        _currentUserService = currentUserService;
    }

    public async Task<BookingResponseDto> CreateAsync(
        CreateBookingRequest request,
        CancellationToken cancellationToken = default)
    {
        var tenantId = _currentUserService.GetRequiredTenantId();

        var tenant = await _tenantRepository.GetByIdAsync(
            tenantId,
            cancellationToken: cancellationToken);

        if (tenant is null)
            throw new NotFoundException("Tenant not found.");

        if (!tenant.IsActive)
            throw new ConflictException(
                "Cannot create a booking for an inactive tenant.");

        var customer = await _customerRepository.GetByIdAsync(
            request.CustomerId,
            x => x.TenantId == tenantId,
            cancellationToken);

        if (customer is null)
            throw new NotFoundException("Customer not found.");

        if (!customer.IsActive)
            throw new ConflictException("Customer is inactive.");

        var workspace = await _workspaceRepository.GetByIdAsync(
            request.WorkspaceId,
            x => x.TenantId == tenantId,
            cancellationToken);

        if (workspace is null)
            throw new NotFoundException("Workspace not found.");

        if (!workspace.IsActive)
            throw new ConflictException("Workspace is inactive.");

        if (!workspace.IsAvailable)
            throw new ConflictException(
                "Workspace is currently unavailable.");

        if (request.EndAt <= request.StartAt)
            throw new ConflictException(
                "End time must be greater than start time.");
        if (request.StartAt < DateTime.UtcNow)
            throw new ConflictException(
                "Booking start time cannot be in the past.");

        if (request.Discount < 0)
            throw new ConflictException(
                "Discount cannot be negative.");

        if (request.Tax < 0)
            throw new ConflictException(
                "Tax cannot be negative.");

        var pricing = await _pricingRepository.GetAllAsync(
            x => x.TenantId == tenantId &&
                 x.WorkspaceId == request.WorkspaceId &&
                 x.PricingType == request.PricingType &&
                 x.IsActive,
            cancellationToken);

        var workspacePricing = pricing.FirstOrDefault();

        if (workspacePricing is null)
            throw new ConflictException(
                "Pricing for this pricing type is not available.");

        var hasConflict = await _bookingRepository.AnyAsync(
            x => x.TenantId == tenantId &&
                 x.WorkspaceId == request.WorkspaceId &&
                 x.Status != BookingStatus.Cancelled &&
                 x.StartAt < request.EndAt &&
                 x.EndAt > request.StartAt,
            cancellationToken);

        if (hasConflict)
            throw new ConflictException(
                "Workspace is already booked during this time.");

        var duration = request.EndAt - request.StartAt;

        decimal bookingAmount;

        switch (request.PricingType)
        {
            case PricingType.Hour:
                {
                    var hours = (decimal)duration.TotalHours;

                    bookingAmount = hours * workspacePricing.Price;
                    break;
                }

            case PricingType.Day:
                {
                    var days = (decimal)Math.Ceiling(
                        duration.TotalDays);

                    bookingAmount = days * workspacePricing.Price;
                    break;
                }

            case PricingType.Week:
                {
                    var weeks = (decimal)Math.Ceiling(
                        duration.TotalDays / 7);

                    bookingAmount = weeks * workspacePricing.Price;
                    break;
                }

            case PricingType.Month:
                {
                    var months = (decimal)Math.Ceiling(
                        duration.TotalDays / 30);

                    bookingAmount = months * workspacePricing.Price;
                    break;
                }

            default:
                throw new ConflictException(
                    "Invalid pricing type.");
        }

        if (request.Discount > bookingAmount)
            throw new ConflictException(
                "Discount cannot exceed the booking amount.");

        var subtotal = bookingAmount - request.Discount;

        var finalAmount = subtotal + request.Tax;

        var booking = _mapper.Map<Booking>(request);

        booking.TenantId = tenantId;
        booking.BookingDate = DateTime.UtcNow;
        booking.UnitPrice = workspacePricing.Price;
        booking.TotalAmount = finalAmount;
        booking.Status = BookingStatus.Pending;
        booking.PaymentStatus = PaymentStatus.Pending;

        await _bookingRepository.AddAsync(
            booking,
            cancellationToken);

        await _unitOfWork.SaveChangesAsync(
            cancellationToken);

        return _mapper.Map<BookingResponseDto>(booking);
    }

    public async Task UpdateAsync(
        int id,
        UpdateBookingRequest request,
        CancellationToken cancellationToken = default)
    {
        var tenantId = _currentUserService.GetRequiredTenantId();

        var booking = await _bookingRepository.GetByIdAsync(
            id,
            x => x.TenantId == tenantId,
            cancellationToken);

        if (booking is null)
            throw new NotFoundException("Booking not found.");

        if (booking.Status == BookingStatus.Cancelled)
            throw new ConflictException(
                "Cancelled booking cannot be updated.");

        var existingPayments = await _paymentRepository.GetAllAsync(
            x => x.BookingId == booking.Id && x.TenantId == tenantId,
            cancellationToken);

        if (existingPayments.Count > 0)
        {
            if (request.CustomerId != booking.CustomerId ||
                request.WorkspaceId != booking.WorkspaceId ||
                request.PricingType != booking.PricingType ||
                request.StartAt != booking.StartAt ||
                request.EndAt != booking.EndAt ||
                request.Discount != booking.Discount ||
                request.Tax != booking.Tax)
            {
                throw new ConflictException(
                    "Booking details cannot be changed after payments have been recorded.");
            }
        }

        var tenant = await _tenantRepository.GetByIdAsync(
            tenantId,
            cancellationToken: cancellationToken);

        if (tenant is null)
            throw new NotFoundException("Tenant not found.");

        if (!tenant.IsActive)
            throw new ConflictException(
                "Cannot update a booking for an inactive tenant.");

        var customer = await _customerRepository.GetByIdAsync(
            request.CustomerId,
            x => x.TenantId == tenantId,
            cancellationToken);

        if (customer is null)
            throw new NotFoundException("Customer not found.");

        if (!customer.IsActive)
            throw new ConflictException(
                "Customer is inactive.");

        var workspace = await _workspaceRepository.GetByIdAsync(
            request.WorkspaceId,
            x => x.TenantId == tenantId,
            cancellationToken);

        if (workspace is null)
            throw new NotFoundException("Workspace not found.");

        if (!workspace.IsActive)
            throw new ConflictException(
                "Workspace is inactive.");

        if (!workspace.IsAvailable)
            throw new ConflictException(
                "Workspace is currently unavailable.");

        if (request.EndAt <= request.StartAt)
            throw new ConflictException(
                "End time must be greater than start time.");

        if (request.StartAt < DateTime.UtcNow)
            throw new ConflictException(
                "Booking start time cannot be in the past.");

        if (request.Discount < 0)
            throw new ConflictException(
                "Discount cannot be negative.");

        if (request.Tax < 0)
            throw new ConflictException(
                "Tax cannot be negative.");

        var pricingList = await _pricingRepository.GetAllAsync(
            x => x.TenantId == tenantId &&
                 x.WorkspaceId == request.WorkspaceId &&
                 x.PricingType == request.PricingType &&
                 x.IsActive,
            cancellationToken);

        var pricing = pricingList.FirstOrDefault();

        if (pricing is null)
            throw new ConflictException(
                "Pricing for this pricing type is not available.");

        var hasConflict = await _bookingRepository.AnyAsync(
            x => x.TenantId == tenantId &&
                 x.WorkspaceId == request.WorkspaceId &&
                 x.Id != id &&
                 x.Status != BookingStatus.Cancelled &&
                 x.StartAt < request.EndAt &&
                 x.EndAt > request.StartAt,
            cancellationToken);

        if (hasConflict)
            throw new ConflictException(
                "Workspace is already booked during this time.");

        var duration = request.EndAt - request.StartAt;

        decimal bookingAmount;

        switch (request.PricingType)
        {
            case PricingType.Hour:
                {
                    var hours = (decimal)duration.TotalHours;

                    bookingAmount = hours * pricing.Price;
                    break;
                }

            case PricingType.Day:
                {
                    var days = (decimal)Math.Ceiling(
                        duration.TotalDays);

                    bookingAmount = days * pricing.Price;
                    break;
                }

            case PricingType.Week:
                {
                    var weeks = (decimal)Math.Ceiling(
                        duration.TotalDays / 7);

                    bookingAmount = weeks * pricing.Price;
                    break;
                }

            case PricingType.Month:
                {
                    var months = (decimal)Math.Ceiling(
                        duration.TotalDays / 30);

                    bookingAmount = months * pricing.Price;
                    break;
                }

            default:
                throw new ConflictException(
                    "Invalid pricing type.");
        }

        if (request.Discount > bookingAmount)
            throw new ConflictException(
                "Discount cannot exceed the booking amount.");

        var subtotal = bookingAmount - request.Discount;

        var finalAmount = subtotal + request.Tax;

        _mapper.Map(request, booking);

        booking.UnitPrice = pricing.Price;
        booking.TotalAmount = finalAmount;

        _bookingRepository.Update(booking);

        await _unitOfWork.SaveChangesAsync(
            cancellationToken);
    }

    public async Task DeleteAsync(
        int id,
        CancellationToken cancellationToken = default)
    {
        var tenantId = _currentUserService.GetRequiredTenantId();

        var booking = await _bookingRepository.GetByIdAsync(
            id,
            x => x.TenantId == tenantId,
            cancellationToken);

        if (booking is null)
            throw new NotFoundException(
                "Booking not found.");

        _bookingRepository.Delete(booking);

        await _unitOfWork.SaveChangesAsync(
            cancellationToken);
    }

    public async Task<IReadOnlyList<BookingResponseDto>> GetAllWithRelationAsync(
        CancellationToken cancellationToken = default)
    {
        var tenantId = _currentUserService.GetRequiredTenantId();

        var bookings =
            await _bookingRepository.GetAllWithRelationAsync(
                tenantId,
                cancellationToken);

        return _mapper.Map<IReadOnlyList<BookingResponseDto>>(
            bookings);
    }

    public async Task<BookingResponseDto> GetByIdWithRelationAsync(
        int id,
        CancellationToken cancellationToken = default)
    {
        var tenantId = _currentUserService.GetRequiredTenantId();

        var booking =
            await _bookingRepository.GetByIdWithRelationAsync(
                id,
                tenantId,
                cancellationToken);

        if (booking is null)
            throw new NotFoundException(
                "Booking not found.");

        return _mapper.Map<BookingResponseDto>(
            booking);
    }
}