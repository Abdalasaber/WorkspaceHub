using WorkspaceHub.Application.DTOs.Dashboard.Staff;

namespace WorkspaceHub.Application.Interfaces.Services
{

    public interface IStaffDashboardService
    {
        Task<StaffDashboardDto> GetAsync(
            CancellationToken cancellationToken = default);
    }
}