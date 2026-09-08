using WorkspaceHub.Application.DTOs.Branch;

namespace WorkspaceHub.Application.Interfaces.Services
{

    public interface IBranchService
    {
        Task<BranchResponseDto> CreateAsync(
            CreateBranchRequest request,
            CancellationToken cancellationToken = default);

        Task UpdateAsync(
            int id,
            UpdateBranchRequest request,
            CancellationToken cancellationToken = default);

        Task DeleteAsync(
            int id,
            CancellationToken cancellationToken = default);

        Task<BranchResponseDto> GetByIdWithRelationAsync(
            int id,
            CancellationToken cancellationToken = default);

        Task<IReadOnlyList<BranchResponseDto>> GetAllWithRelationAsync(
            CancellationToken cancellationToken = default);
    }
}