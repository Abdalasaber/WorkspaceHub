using WorkspaceHub.Application.Interfaces.GenericRepository;
using WorkspaceHub.Domain.Entities;

namespace WorkspaceHub.Application.Interfaces.Repository
{
    public interface IWorkspacePricingRepository : IGenericRepository<WorkspacePricing>
    {
        Task<WorkspacePricing?> GetByIdWithRelationAsync(
            int id,
            int tenantId,
            CancellationToken cancellationToken = default);

        Task<IReadOnlyList<WorkspacePricing>> GetAllWithRelationAsync(int tenantId, CancellationToken cancellationToken = default);

    }
}
