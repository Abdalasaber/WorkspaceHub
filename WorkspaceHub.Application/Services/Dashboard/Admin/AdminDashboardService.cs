using WorkspaceHub.Application.DTOs.Dashboard.Admin;
using WorkspaceHub.Application.Interfaces;
using WorkspaceHub.Application.Interfaces.GenericRepository;
using WorkspaceHub.Application.Interfaces.Services;
using WorkspaceHub.Domain.Entities;
using WorkspaceHub.Domain.Enums;

namespace WorkspaceHub.Application.Services
{

    public class AdminDashboardService : IAdminDashboardService
    {
        private readonly IGenericRepository<Booking> _bookingRepository;
        private readonly IGenericRepository<Payment> _paymentRepository;
        private readonly IGenericRepository<Workspace> _workspaceRepository;
        private readonly IGenericRepository<Customer> _customerRepository;
        private readonly ICurrentUserService _currentUserService;

        public AdminDashboardService(
            IGenericRepository<Booking> bookingRepository,
            IGenericRepository<Payment> paymentRepository,
            IGenericRepository<Workspace> workspaceRepository,
            IGenericRepository<Customer> customerRepository,
            ICurrentUserService currentUserService)
        {
            _bookingRepository = bookingRepository;
            _paymentRepository = paymentRepository;
            _workspaceRepository = workspaceRepository;
            _customerRepository = customerRepository;
            _currentUserService = currentUserService;
        }

        public async Task<AdminDashboardDto> GetAsync(
            CancellationToken cancellationToken = default)
        {
            var tenantId =
                _currentUserService.GetRequiredTenantId();

            var bookings =
                await _bookingRepository.GetAllAsync(
                    x => x.TenantId == tenantId,
                    cancellationToken);

            var payments =
                await _paymentRepository.GetAllAsync(
                    x => x.TenantId == tenantId,
                    cancellationToken);

            var workspaces =
                await _workspaceRepository.GetAllAsync(
                    x => x.TenantId == tenantId,
                    cancellationToken);

            var customers =
                await _customerRepository.GetAllAsync(
                    x => x.TenantId == tenantId,
                    cancellationToken);

            var today = DateTime.UtcNow.Date;
            var tomorrow = today.AddDays(1);

            var monthStart =
                new DateTime(
                    today.Year,
                    today.Month,
                    1);

            var nextMonth =
                monthStart.AddMonths(1);

            return new AdminDashboardDto
            {
                Bookings = new BookingDashboardDto
                {
                    Total = bookings.Count,

                    Today = bookings.Count(
                        x => x.StartAt >= today &&
                             x.StartAt < tomorrow),

                    Pending = bookings.Count(
                        x => x.Status == BookingStatus.Pending),

                    Confirmed = bookings.Count(
                        x => x.Status == BookingStatus.Confirmed)
                },

                Revenue = new RevenueDashboardDto
                {
                    Total = payments.Sum(
                        x => x.AppliedAmount),

                    Today = payments
                        .Where(x =>
                            x.PaymentDate >= today &&
                            x.PaymentDate < tomorrow)
                        .Sum(x => x.AppliedAmount),

                    ThisMonth = payments
                        .Where(x =>
                            x.PaymentDate >= monthStart &&
                            x.PaymentDate < nextMonth)
                        .Sum(x => x.AppliedAmount)
                },

                Workspaces = new WorkspaceDashboardDto
                {
                    Total = workspaces.Count,

                    Available = workspaces.Count(
                        x => x.IsAvailable && x.IsActive),

                    Unavailable = workspaces.Count(
                        x => !x.IsAvailable || !x.IsActive)
                },

                Customers = new CustomerDashboardDto
                {
                    Total = customers.Count,

                    Active = customers.Count(
                        x => x.IsActive),

                    Inactive = customers.Count(
                        x => !x.IsActive)
                }
            };
        }
    }
}