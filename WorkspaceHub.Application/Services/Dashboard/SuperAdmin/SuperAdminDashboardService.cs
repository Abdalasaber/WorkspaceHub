using Microsoft.AspNetCore.Identity;
using WorkspaceHub.Application.DTOs.Dashboard.SuperAdmin;
using WorkspaceHub.Application.Interfaces;
using WorkspaceHub.Application.Interfaces.GenericRepository;
using WorkspaceHub.Application.Interfaces.Services.Dashboard.SuperAdmin;
using WorkspaceHub.Domain.Entities;

namespace WorkspaceHub.Application.Services;

public class SuperAdminDashboardService : ISuperAdminDashboardService
{
    private readonly IGenericRepository<Tenant> _tenantRepository;
    private readonly IGenericRepository<AppUser> _userRepository;
    private readonly IGenericRepository<Booking> _bookingRepository;
    private readonly IGenericRepository<Payment> _paymentRepository;
    private readonly UserManager<AppUser> _userManager;
    private readonly ICurrentUserService _currentUserService;

    public SuperAdminDashboardService(
        IGenericRepository<Tenant> tenantRepository,
        IGenericRepository<AppUser> userRepository,
        IGenericRepository<Booking> bookingRepository,
        IGenericRepository<Payment> paymentRepository,
        UserManager<AppUser> userManager,
        ICurrentUserService currentUserService)
    {
        _tenantRepository = tenantRepository;
        _userRepository = userRepository;
        _bookingRepository = bookingRepository;
        _paymentRepository = paymentRepository;
        _userManager = userManager;
        _currentUserService = currentUserService;
    }

    public async Task<SuperAdminDashboardDto> GetAsync(
        CancellationToken cancellationToken = default)
    {
        if (!_currentUserService.IsSuperAdmin)
        {
            throw new UnauthorizedException(
                "Only SuperAdmin can access the platform dashboard.");
        }

        var tenants = await _tenantRepository.GetAllAsync(
            cancellationToken: cancellationToken);

        var totalTenants = tenants.Count;

        var activeTenants = tenants.Count(
            x => x.IsActive);

        var pendingTenants = tenants.Count(
            x => !x.IsActive);

        var users = await _userRepository.GetAllAsync(
            cancellationToken: cancellationToken);

        var totalUsers = users.Count;

        var admins = await _userManager
            .GetUsersInRoleAsync("Admin");

        var staff = await _userManager
            .GetUsersInRoleAsync("Staff");

        var bookings = await _bookingRepository.GetAllAsync(
            cancellationToken: cancellationToken);

        var today = DateTime.UtcNow.Date;
        var tomorrow = today.AddDays(1);

        var totalBookings = bookings.Count;

        var todayBookings = bookings.Count(
            x => x.StartAt >= today &&
                 x.StartAt < tomorrow);

        var payments = await _paymentRepository.GetAllAsync(
            cancellationToken: cancellationToken);

        var totalRevenue = payments.Sum(
            x => x.AppliedAmount);

        var todayRevenue = payments
            .Where(x =>
                x.PaymentDate >= today &&
                x.PaymentDate < tomorrow)
            .Sum(x => x.AppliedAmount);

        var monthStart = new DateTime(
            today.Year,
            today.Month,
            1);

        var nextMonth = monthStart.AddMonths(1);

        var thisMonthRevenue = payments
            .Where(x =>
                x.PaymentDate >= monthStart &&
                x.PaymentDate < nextMonth)
            .Sum(x => x.AppliedAmount);

        var totalOverpayment =
        payments.Sum(x =>
        Math.Max(x.Amount - x.AppliedAmount, 0));

        var thisMonthOverpayment =
            payments
                .Where(x =>
                    x.PaymentDate >= monthStart &&
                    x.PaymentDate < nextMonth)
                .Sum(x =>
                    Math.Max(
                        x.Amount - x.AppliedAmount,
                        0));


        return new SuperAdminDashboardDto
        {
            Tenants = new TenantStatisticsDto
            {
                Total = totalTenants,
                Active = activeTenants,
                Pending = pendingTenants
            },

            Users = new UserStatisticsDto
            {
                Total = totalUsers,
                Admins = admins.Count,
                Staff = staff.Count
            },

            Bookings = new BookingStatisticsDto
            {
                Total = totalBookings,
                Today = todayBookings
            },

            Revenue = new RevenueStatisticsDto
            {
                Total = totalRevenue,
                Today = todayRevenue,
                ThisMonth = thisMonthRevenue,
                TotalOverpayment = totalOverpayment,
                ThisMonthOverpayment = thisMonthOverpayment
            }
        };
    }
}