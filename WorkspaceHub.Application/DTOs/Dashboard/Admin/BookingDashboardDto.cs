namespace WorkspaceHub.Application.DTOs.Dashboard.Admin
{

    public class BookingDashboardDto
    {
        public int Total { get; set; }

        public int Today { get; set; }

        public int Pending { get; set; }

        public int Confirmed { get; set; }
    }
}