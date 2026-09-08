using WorkspaceHub.Application.DTOs.Workspace;

namespace WorkspaceHub.Application.Interfaces.Services.Woekspace
{

    public interface IWorkspaceService
    {
        Task<WorkspaceResponseDto> CreateAsync(
            CreateWorkspaceRequest request,
            CancellationToken cancellationToken = default);

        Task UpdateAsync(
            int id,
            UpdateWorkspaceRequest request,
            CancellationToken cancellationToken = default);

        Task DeleteAsync(
            int id,
            CancellationToken cancellationToken = default);

        Task<WorkspaceResponseDto> GetByIdAsync(
            int id,
            CancellationToken cancellationToken = default);

        Task<IReadOnlyList<WorkspaceResponseDto>> GetAllAsync(
            CancellationToken cancellationToken = default);
    }
}