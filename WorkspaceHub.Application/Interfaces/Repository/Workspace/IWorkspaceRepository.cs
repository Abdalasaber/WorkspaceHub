using WorkspaceHub.Application.Interfaces.GenericRepository;
using WorkspaceHub.Domain.Entities;

namespace WorkspaceHub.Application.Interfaces.Repository
{
    public interface IWorkspaceRepository : IGenericRepository<Workspace>
    {
        Task<Workspace?> GetByIdWithRelationAsync(
            int id,
            int tenantId,
            CancellationToken cancellationToken = default);

        Task<IReadOnlyList<Workspace>> GetAllWithRelationAsync(
            int tenantId,
            CancellationToken cancellationToken = default);
    }

}