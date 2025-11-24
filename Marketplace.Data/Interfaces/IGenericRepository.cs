using System.Linq.Expressions;
using Marketplace.Data.Entities;

namespace Marketplace.Data.Interfaces;

/// <summary>
///     Generic repository interface providing common CRUD operations for all entities
/// </summary>
/// <typeparam name="TEntity">Entity type that inherits from BaseEntity</typeparam>
public interface IGenericRepository<TEntity> where TEntity : BaseEntity
{
    // Query operations - Added trackChanges parameter for control over change tracking
    Task<TEntity?> GetByIdAsync(int id, bool trackChanges = false);
    Task<TEntity?> GetByIdAsync(int id, bool trackChanges = false, params Expression<Func<TEntity, object>>[] includes);
    Task<IEnumerable<TEntity>> GetAllAsync(bool trackChanges = false);
    Task<IEnumerable<TEntity>> GetAllAsync(bool trackChanges = false, params Expression<Func<TEntity, object>>[] includes);
    Task<TEntity?> GetFirstOrDefaultAsync(Expression<Func<TEntity, bool>> predicate, bool trackChanges = false);

    Task<TEntity?> GetFirstOrDefaultAsync(Expression<Func<TEntity, bool>> predicate, bool trackChanges = false,
        params Expression<Func<TEntity, object>>[] includes);

    Task<IEnumerable<TEntity>> GetWhereAsync(Expression<Func<TEntity, bool>> predicate, bool trackChanges = false);

    Task<IEnumerable<TEntity>> GetWhereAsync(Expression<Func<TEntity, bool>> predicate, bool trackChanges = false,
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