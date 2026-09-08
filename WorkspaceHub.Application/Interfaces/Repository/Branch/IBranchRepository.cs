using WorkspaceHub.Application.Interfaces.GenericRepository;
using WorkspaceHub.Domain.Entities;

namespace WorkspaceHub.Application.Interfaces.Repository
{
    public interface IBranchRepository : IGenericRepository<Branch>
    {
        Task<Branch?> GetByIdWithRelationAsync(int id, int tenantId, CancellationToken cancellationToken = default);

        Task<IReadOnlyList<Branch>> GetAllWithRelationAsync(int tenantId, CancellationToken cancellationToken = default);
    }
}
