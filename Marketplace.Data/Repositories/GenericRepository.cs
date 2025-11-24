using System.Linq.Expressions;
using Marketplace.Data.Entities;
using Marketplace.Data.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Marketplace.Data.Repositories;

/// <summary>
/// Generic repository implementation providing common CRUD operations
/// </summary>
/// <typeparam name="TEntity">Entity type that inherits from BaseEntity</typeparam>
public class GenericRepository<TEntity> : IGenericRepository<TEntity> where TEntity : BaseEntity
{
    private readonly MarketplaceDbContext _context;
    private readonly DbSet<TEntity> _dbSet;

    public GenericRepository(MarketplaceDbContext context)
    {
        _context = context;
        _dbSet = _context.Set<TEntity>();
    }

    public async Task<TEntity?> GetByIdAsync(int id, bool trackChanges = false)
    {
        if (trackChanges)
        {
            return await _dbSet.FindAsync(id);
        }
        
        // Use AsNoTracking to avoid change tracker cache issues
        return await _dbSet.AsNoTracking().FirstOrDefaultAsync(e => e.Id == id);
    }

    public async Task<TEntity?> GetByIdAsync(int id, bool trackChanges = false, params Expression<Func<TEntity, object>>[] includes)
    {
        var query = trackChanges ? _dbSet : _dbSet.AsNoTracking();
        
        foreach (var include in includes)
        {
            query = query.Include(include);
        }
        
        return await query.FirstOrDefaultAsync(e => e.Id == id);
    }

    public async Task<IEnumerable<TEntity>> GetAllAsync(bool trackChanges = false)
    {
        var query = trackChanges ? _dbSet : _dbSet.AsNoTracking();
        return await query.ToListAsync();
    }

    public async Task<IEnumerable<TEntity>> GetAllAsync(bool trackChanges = false, params Expression<Func<TEntity, object>>[] includes)
    {
        var query = trackChanges ? _dbSet : _dbSet.AsNoTracking();
        
        foreach (var include in includes)
        {
            query = query.Include(include);
        }
        
        return await query.ToListAsync();
    }

    public async Task<TEntity?> GetFirstOrDefaultAsync(Expression<Func<TEntity, bool>> predicate, bool trackChanges = false)
    {
        var query = trackChanges ? _dbSet : _dbSet.AsNoTracking();
        return await query.FirstOrDefaultAsync(predicate);
    }

    public async Task<TEntity?> GetFirstOrDefaultAsync(Expression<Func<TEntity, bool>> predicate, bool trackChanges = false,
        params Expression<Func<TEntity, object>>[] includes)
    {
        var query = trackChanges ? _dbSet : _dbSet.AsNoTracking();
        
        foreach (var include in includes)
        {
            query = query.Include(include);
        }
        
        return await query.FirstOrDefaultAsync(predicate);
    }

    public async Task<IEnumerable<TEntity>> GetWhereAsync(Expression<Func<TEntity, bool>> predicate, bool trackChanges = false)
    {
        var query = trackChanges ? _dbSet : _dbSet.AsNoTracking();
        return await query.Where(predicate).ToListAsync();
    }

    public async Task<IEnumerable<TEntity>> GetWhereAsync(Expression<Func<TEntity, bool>> predicate, bool trackChanges = false,
        params Expression<Func<TEntity, object>>[] includes)
    {
        var query = trackChanges ? _dbSet : _dbSet.AsNoTracking();
        
        foreach (var include in includes)
        {
            query = query.Include(include);
        }
        
        return await query.Where(predicate).ToListAsync();
    }

    public async Task<bool> ExistsAsync(Expression<Func<TEntity, bool>> predicate)
    {
        return await _dbSet.AnyAsync(predicate);
    }

    public async Task<int> CountAsync()
    {
        return await _dbSet.CountAsync();
    }

    public async Task<int> CountAsync(Expression<Func<TEntity, bool>> predicate)
    {
        return await _dbSet.CountAsync(predicate);
    }

    public async Task<TEntity> AddAsync(TEntity entity)
    {
        await _dbSet.AddAsync(entity);
        return entity;
    }

    public async Task<IEnumerable<TEntity>> AddRangeAsync(IEnumerable<TEntity> entities)
    {
        var addRangeAsync = entities as TEntity[] ?? entities.ToArray();
        await _dbSet.AddRangeAsync(addRangeAsync);
        return addRangeAsync;
    }

    public Task<TEntity> UpdateAsync(TEntity entity)
    {
        _context.Entry(entity).State = EntityState.Modified;
        return Task.FromResult(entity);
    }

    public Task<IEnumerable<TEntity>> UpdateRangeAsync(IEnumerable<TEntity> entities)
    {
        var entitiesArray = entities as TEntity[] ?? entities.ToArray();
        
        foreach (var entity in entitiesArray)
        {
            _context.Entry(entity).State = EntityState.Modified;
        }

        return Task.FromResult<IEnumerable<TEntity>>(entitiesArray);
    }

    public async Task DeleteAsync(int id)
    {
        // Use trackChanges = true here since we're immediately deleting
        var entity = await GetByIdAsync(id, trackChanges: true);
        if (entity != null)
        {
            _dbSet.Remove(entity);
        }
    }

    public Task DeleteAsync(TEntity entity)
    {
        _dbSet.Remove(entity);
        return Task.CompletedTask;
    }

    public Task DeleteRangeAsync(IEnumerable<TEntity> entities)
    {
        _dbSet.RemoveRange(entities);
        return Task.CompletedTask;
    }

    public Task DeleteRangeAsync(Expression<Func<TEntity, bool>> predicate)
    {
        var entities = _dbSet.Where(predicate);
        _dbSet.RemoveRange(entities);
        return Task.CompletedTask;
    }

    public async Task<int> SaveChangesAsync()
    {
        return await _context.SaveChangesAsync();
    }
}