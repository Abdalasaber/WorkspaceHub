namespace WorkspaceHub.Application.DTOs.Dashboard.SuperAdmin
{
    public class SuperAdminDashboardDto
    {
        public TenantStatisticsDto Tenants { get; set; } = new();
        public UserStatisticsDto Users { get; set; } = new();
        public BookingStatisticsDto Bookings { get; set; } = new();
        public RevenueStatisticsDto Revenue { get; set; } = new();
    }
}
