using WorkspaceHub.Application.DTOs.WorkspaceType;

namespace WorkspaceTypeHub.Application.Interfaces.Services
{

    public interface IWorkspaceTypeService
    {
        Task<WorkspaceTypeResponseDto> CreateAsync(CreateWorkspaceTypeRequest request, CancellationToken cancellationToken = default);

        Task UpdateAsync(
            int id,
            UpdateWorkspaceTypeRequest request,
            CancellationToken cancellationToken = default);

        Task DeleteAsync(
            int id,
            CancellationToken cancellationToken = default);

        Task<WorkspaceTypeResponseDto> GetByIdAsync(int id, CancellationToken cancellationToken = default);

        Task<IReadOnlyList<WorkspaceTypeResponseDto>> GetAllAsync(CancellationToken cancellationToken = default);
    }
}