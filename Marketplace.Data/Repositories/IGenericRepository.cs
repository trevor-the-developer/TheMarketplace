using System.Linq.Expressions;
using Marketplace.Data.Entities;

namespace Marketplace.Data.Repositories;

/// <summary>
///     Generic repository interface providing common CRUD operations for all entities
/// </summary>
/// <typeparam name="TEntity">Entity type that inherits from BaseEntity</typeparam>
public interface IGenericRepository<TEntity> where TEntity : BaseEntity
{
    // Query operations
    Task<TEntity?> GetByIdAsync(int id);
    Task<TEntity?> GetByIdAsync(int id, params Expression<Func<TEntity, object>>[] includes);
    Task<IEnumerable<TEntity>> GetAllAsync();
    Task<IEnumerable<TEntity>> GetAllAsync(params Expression<Func<TEntity, object>>[] includes);
    Task<TEntity?> GetFirstOrDefaultAsync(Expression<Func<TEntity, bool>> predicate);

    Task<TEntity?> GetFirstOrDefaultAsync(Expression<Func<TEntity, bool>> predicate,
        params Expression<Func<TEntity, object>>[] includes);

    Task<IEnumerable<TEntity>> GetWhereAsync(Expression<Func<TEntity, bool>> predicate);

    Task<IEnumerable<TEntity>> GetWhereAsync(Expression<Func<TEntity, bool>> predicate,
        params Expression<Func<TEntity, object>>[] includes);

    Task<bool> ExistsAsync(Expression<Func<TEntity, bool>> predicate);
    Task<int> CountAsync();
    Task<int> CountAsync(Expression<Func<TEntity, bool>> predicate);

    // Command operations
    Task<TEntity> AddAsync(TEntity entity);
    Task<IEnumerable<TEntity>> AddRangeAsync(IEnumerable<TEntity> entities);
    Task<TEntity> UpdateAsync(TEntity entity);
    Task<IEnumerable<TEntity>> UpdateRangeAsync(IEnumerable<TEntity> entities);
    Task DeleteAsync(int id);
    Task DeleteAsync(TEntity entity);
    Task DeleteRangeAsync(IEnumerable<TEntity> entities);
    Task DeleteRangeAsync(Expression<Func<TEntity, bool>> predicate);

    // Transaction operations
    Task<int> SaveChangesAsync();
}