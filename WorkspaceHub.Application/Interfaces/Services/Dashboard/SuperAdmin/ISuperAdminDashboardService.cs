using WorkspaceHub.Application.DTOs.Dashboard.SuperAdmin;

namespace WorkspaceHub.Application.Interfaces.Services.Dashboard.SuperAdmin
{
    public interface ISuperAdminDashboardService
    {
        Task<SuperAdminDashboardDto> GetAsync(
            CancellationToken cancellationToken = default);
    }
}
