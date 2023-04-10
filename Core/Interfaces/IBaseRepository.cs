using Core.Models;
using System.Linq.Expressions;

namespace Core.Interfaces;

public interface IBaseRepository<T> where T : class
{
    Task<T> GetByIdAsync(Guid id);
    IQueryable<T> Get(Expression<Func<T, bool>> criteria);
    Task<IEnumerable<T>> GetListAsync();
    Task<T> FindAsync(Expression<Func<T, bool>> criteria, string[] includes = null);
    Task<IEnumerable<T>> FindAllAsync(Expression<Func<T, bool>> criteria, string[] includes = null);
    Task<(IEnumerable<T>, PaginationMetadata)> GetPageAsync(int pageNumber, int pageSize);
    PagingVm<TOutput> GetPageAsync<TOutput>(string orderBy, bool isAscending = false, int pageIndex = 1, int pageSize = 10);
    Task<T> AddAsync(T entity);
    Task<IEnumerable<T>> AddRangeAsync(IEnumerable<T> entities);
    T Update( T entity);
    void Attach(T entity);
    void AttachRange(IEnumerable<T> entities);
    void Delete(T entity);
    void DeleteRange(IEnumerable<T> entities);
    Task<int> CountAsync();
    Task<int> CountAsync(Expression<Func<T, bool>> criteria);
}
