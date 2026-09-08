using WorkspaceHub.Application.Interfaces.GenericRepository;
using WorkspaceHub.Domain.Entities;

namespace WorkspaceHub.Application.Interfaces.Repository
{
    public interface ICustomerRepository : IGenericRepository<Customer>
    {
        Task<Customer?> GetByIdWithRelationAsync(
            int id,
            int tenantId,
            CancellationToken cancellationToken = default);

        Task<IReadOnlyList<Customer>> GetAllWithRelationAsync(int tenantId, CancellationToken cancellationToken = default);

    }
}
