using Core.Entities;

namespace Core.Intarfaces;

public interface IGenericRepository<T> where T : BaseEntity
{
    Task<T?> GetByIdAsync(int id, CancellationToken cancellationToken);
    Task<IReadOnlyList<T>> ListAllAsync(CancellationToken cancellationToken);
    Task<T> AddAsync(T entity, CancellationToken cancellationToken);
    Task<T> UpdateAsync(T entity, CancellationToken cancellationToken);
    Task<T> DeleteAsync(T entity, CancellationToken cancellationToken);
    Task<bool> SaveChangesAsync(CancellationToken cancellationToken);
    Task<bool> EntityExists(int id, CancellationToken cancellationToken);

    Task<T?> GetEntityWithSpecificationAsync(ISpecification<T> spec, CancellationToken cancellationToken);
    Task<IReadOnlyList<T>> ListSpecificationAsync(ISpecification<T>spec, CancellationToken cancellationToken);

    Task<TResult?> GetEntityWithSpecAsync<TResult>(ISpecification<T,TResult> spec, CancellationToken cancellationToken);
    Task<IReadOnlyList<TResult>> ListSpecAsync<TResult>(ISpecification<T,TResult>spec, CancellationToken cancellationToken);



}
