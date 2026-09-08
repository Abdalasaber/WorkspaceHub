using WorkspaceHub.Application.DTOs.Dashboard.Staff;
using WorkspaceHub.Application.Interfaces;
using WorkspaceHub.Application.Interfaces.GenericRepository;
using WorkspaceHub.Application.Interfaces.Services;
using WorkspaceHub.Domain.Entities;
using WorkspaceHub.Domain.Enums;

namespace WorkspaceHub.Application.Services
{
    public class StaffDashboardService : IStaffDashboardService
    {
        private readonly IGenericRepository<Booking> _bookingRepository;
        private readonly IGenericRepository<Workspace> _workspaceRepository;
        private readonly ICurrentUserService _currentUserService;

        public StaffDashboardService(
            IGenericRepository<Booking> bookingRepository,
            IGenericRepository<Workspace> workspaceRepository,
            ICurrentUserService currentUserService)
        {
            _bookingRepository = bookingRepository;
            _workspaceRepository = workspaceRepository;
            _currentUserService = currentUserService;
        }

        public async Task<StaffDashboardDto> GetAsync(
            CancellationToken cancellationToken = default)
        {
            var tenantId =
                _currentUserService.GetRequiredTenantId();

            var bookings =
                await _bookingRepository.GetAllAsync(
                    x => x.TenantId == tenantId,
                    cancellationToken);

            var workspaces =
                await _workspaceRepository.GetAllAsync(
                    x => x.TenantId == tenantId,
                    cancellationToken);

            var now = DateTime.UtcNow;
            var today = now.Date;
            var tomorrow = today.AddDays(1);

            return new StaffDashboardDto
            {
                TodayBookings = bookings.Count(
                    x => x.StartAt >= today &&
                         x.StartAt < tomorrow),

                UpcomingBookings = bookings.Count(
                    x => x.StartAt > now &&
                         x.Status != BookingStatus.Cancelled),

                AvailableWorkspaces = workspaces.Count(
                    x => x.IsActive &&
                         x.IsAvailable),

                PendingPaymentBookings = bookings.Count(
                    x => x.PaymentStatus != PaymentStatus.Paid &&
                         x.Status != BookingStatus.Cancelled)
            };
        }
    }
}