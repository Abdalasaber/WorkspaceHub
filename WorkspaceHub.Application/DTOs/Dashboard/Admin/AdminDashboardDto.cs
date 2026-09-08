namespace WorkspaceHub.Application.DTOs.Dashboard.Admin
{
    public class AdminDashboardDto
    {
        public BookingDashboardDto Bookings { get; set; } = new();

        public RevenueDashboardDto Revenue { get; set; } = new();

        public WorkspaceDashboardDto Workspaces { get; set; } = new();

        public CustomerDashboardDto Customers { get; set; } = new();
    }
}