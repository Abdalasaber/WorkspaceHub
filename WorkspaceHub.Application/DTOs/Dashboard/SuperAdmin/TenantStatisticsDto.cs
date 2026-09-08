namespace WorkspaceHub.Application.DTOs.Dashboard.SuperAdmin
{
    public class TenantStatisticsDto
    {
        public int Total { get; set; }
        public int Active { get; set; }
        public int Pending { get; set; }
        public int Inactive { get; set; }
    }
}
