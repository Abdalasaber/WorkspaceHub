using WorkspaceHub.Application.DTOs.Tenant;

namespace WorkspaceHub.Application.Interfaces.Services
{
    public interface ITenantService
    {
        // SuperAdmin
        Task ApproveAsync(int tenantId, CancellationToken cancellationToken = default);
        Task<TenantResponseDto> CreateAsync(
            CreateTenantRequest request,
            CancellationToken cancellationToken = default);

        Task<IReadOnlyList<TenantResponseDto>> GetAllAsync(
            CancellationToken cancellationToken = default);

        Task<TenantResponseDto> GetByIdAsync(
            int id,
            CancellationToken cancellationToken = default);

        Task UpdateAsync(
            int id,
            UpdateTenantRequest request,
            CancellationToken cancellationToken = default);

        Task DeleteAsync(
            int id,
            CancellationToken cancellationToken = default);

        // Current Tenant
        Task<TenantResponseDto> GetCurrentAsync(
            CancellationToken cancellationToken = default);

        Task UpdateCurrentAsync(
            UpdateCurrentTenantRequest request,
            CancellationToken cancellationToken = default);
    }
}