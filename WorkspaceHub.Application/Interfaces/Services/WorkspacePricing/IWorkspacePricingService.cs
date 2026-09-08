using WorkspaceHub.Application.DTOs.WorkspacePricing;

namespace WorkspaceHub.Application.Interfaces.Services.WorkspacePricing
{
    public interface IWorkspacePricingService
    {
        Task<WorkspacePricingResponseDto> CreateAsync(CreateWorkspacePricingRequest request, CancellationToken cancellationToken = default);
        Task UpdateAsync(int id, UpdateWorkspacePricingRequest request, CancellationToken cancellationToken = default);
        Task DeleteAsync(int id, CancellationToken cancellationToken = default);
        Task<WorkspacePricingResponseDto> GetByIdWithRelationAsync(int id, CancellationToken cancellationToken = default);
        Task<IReadOnlyList<WorkspacePricingResponseDto>> GetAllWithRelationAsync(CancellationToken cancellationToken = default);
    }
}
