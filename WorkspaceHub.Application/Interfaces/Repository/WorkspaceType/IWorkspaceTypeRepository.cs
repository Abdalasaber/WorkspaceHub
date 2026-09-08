using WorkspaceHub.Application.Interfaces.GenericRepository;
using WorkspaceHub.Domain.Entities;

namespace WorkspaceTypeHub.Application.Interfaces.Repository
{
    public interface IWorkspaceTypeRepository : IGenericRepository<WorkspaceType>
    {
        Task<WorkspaceType?> GetByIdWithRelationAsync(
            int id,
            int tenantId,
            CancellationToken cancellationToken = default);

        Task<IReadOnlyList<WorkspaceType>> GetAllWithRelationAsync(
            int tenantId,
            CancellationToken cancellationToken = default);
    }
}