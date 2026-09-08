using WorkspaceHub.Application.Interfaces.GenericRepository;
using WorkspaceHub.Domain.Entities;

namespace TenantHub.Application.Interfaces.Repository
{
    public interface ITenantRepository : IGenericRepository<Tenant>
    {
    }
}
