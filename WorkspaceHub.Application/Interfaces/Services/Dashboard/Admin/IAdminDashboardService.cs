using WorkspaceHub.Application.DTOs.Dashboard.Admin;

namespace WorkspaceHub.Application.Interfaces.Services
{
    public interface IAdminDashboardService
    {
        Task<AdminDashboardDto> GetAsync(
            CancellationToken cancellationToken = default);
    }
}