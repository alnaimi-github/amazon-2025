namespace infrastructure.Data.Repositories;

public class GenericRepository<T> : IGenericRepository<T> where T : BaseEntity
{
    private readonly StoreContext _storeContext;
    private readonly DbSet<T> _dbSet;

    public GenericRepository(StoreContext storeContext)
    {
        _storeContext = storeContext;
        _dbSet = _storeContext.Set<T>();
    }

    public async Task<T> AddAsync(T entity, CancellationToken cancellationToken)
    {
        await _dbSet.AddAsync(entity, cancellationToken);
        await _storeContext.SaveChangesAsync(cancellationToken);
        return entity;
    }

    public async Task<bool> EntityExists(int id, CancellationToken cancellationToken)
    {
        return await _dbSet.AnyAsync(e => e.Id == id, cancellationToken);
    }

    public async Task<T?> GetByIdAsync(int id, CancellationToken cancellationToken)
    {
        return await _dbSet.FindAsync([id], cancellationToken);
    }

    public async Task<IReadOnlyList<T>> ListAllAsync(CancellationToken cancellationToken)
    {
        return await _dbSet.ToListAsync(cancellationToken);
    }

    public async Task<bool> SaveChangesAsync(CancellationToken cancellationToken)
    {
        return await _storeContext.SaveChangesAsync(cancellationToken) > 0;
    }

    public async Task<T> UpdateAsync(T entity, CancellationToken cancellationToken)
    {
        _dbSet.Update(entity);
        await _storeContext.SaveChangesAsync(cancellationToken);
        return entity;
    }

    public async Task<T> DeleteAsync(T entity, CancellationToken cancellationToken)
    {
        _dbSet.Remove(entity);
        await _storeContext.SaveChangesAsync(cancellationToken);
        return entity;
    }

    public async Task<T?> GetEntityWithSpecificationAsync(ISpecification<T> spec ,
     CancellationToken cancellationToken)
    {
        return await ApplySpecification(spec).FirstOrDefaultAsync(cancellationToken);
    }
    public async Task<IReadOnlyList<T>> ListSpecificationAsync(ISpecification<T> spec,
     CancellationToken cancellationToken)
    {
        return await ApplySpecification(spec).ToListAsync(cancellationToken);
    }

    public async Task<TResult?> GetEntityWithSpecAsync<TResult>(ISpecification<T, TResult> spec,
     CancellationToken cancellationToken)
    {
        return await ApplySpecification(spec).FirstOrDefaultAsync();
    }

    public async Task<IReadOnlyList<TResult>> ListSpecAsync<TResult>(ISpecification<T, TResult> spec, 
    CancellationToken cancellationToken)
    {
        return await ApplySpecification(spec).ToListAsync();
    }
  public async Task<int> CountAsync(ISpecification<T> spec, CancellationToken cancellationToken)
    {
        var query = _dbSet.AsQueryable();
            query = spec.ApplyCriteria(query);

            return await query.CountAsync();
    }

   private IQueryable<T> ApplySpecification(ISpecification<T> spec)
    {
        return SpecificationEvaluator<T>.GetQuery(_dbSet.AsQueryable(), spec);
    }

     private IQueryable<TResult> ApplySpecification<TResult>(ISpecification<T,TResult> spec)
    {
        return SpecificationEvaluator<T>.GetQuery<T,TResult>(_dbSet.AsQueryable(), spec);
    }
}

