namespace WorkspaceHub.Application.DTOs.Dashboard.Staff
{
    public class StaffDashboardDto
    {
        public int TodayBookings { get; set; }

        public int UpcomingBookings { get; set; }

        public int AvailableWorkspaces { get; set; }

        public int PendingPaymentBookings { get; set; }
    }
}