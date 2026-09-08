using System.Linq.Expressions;

namespace WorkspaceHub.Application.Interfaces.GenericRepository
{
    public interface IGenericRepository<TEntity> where TEntity : class
    {
        Task<bool> AnyAsync(Expression<Func<TEntity, bool>> predicate, CancellationToken cancellationToken = default);

        Task AddAsync(TEntity entity, CancellationToken cancellationToken = default);

        void Update(TEntity entity);

        void Delete(TEntity entity);

        Task<TEntity?> GetByIdAsync(
          int id,
          Expression<Func<TEntity, bool>>? predicate = null,
          CancellationToken cancellationToken = default);

        Task<IReadOnlyList<TEntity>> GetAllAsync(
            Expression<Func<TEntity, bool>>? predicate = null,
            CancellationToken cancellationToken = default);
    }
}
