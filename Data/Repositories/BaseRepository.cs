
using AutoMapper;
using AutoMapper.QueryableExtensions;
using Core.Extentions;
using Core.Helpers;
using Core.Interfaces;
using Core.Models;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace Data.Repositories;

public class BaseRepository<T> : IBaseRepository<T> where T : class
{
    protected ApplicationDbContext _context;
    private readonly IMapper _mapper;
    public BaseRepository(ApplicationDbContext context, IMapper mapper)
    { 
        _context = context;
        _mapper  = mapper;
    }
    public async Task<T> GetByIdAsync(Guid id)
        => await _context.Set<T>().FindAsync(id);

    public async Task<T> FindAsync(Expression<Func<T, bool>> criteria, string[] includes = null)
    {
        IQueryable<T> query = _context.Set<T>();

        if (includes != null)
            foreach (var item in includes)
                query = query.Include(item);
        return await query.SingleOrDefaultAsync(criteria);
    }
    public async Task<IEnumerable<T>> GetListAsync()
        => await _context.Set<T>().ToListAsync();
    public async Task<IEnumerable<T>> FindAllAsync(Expression<Func<T, bool>> criteria, string[] includes = null)
    {
        IQueryable<T> query = _context.Set<T>();
        if (includes != null)
            foreach (var item in includes)
                query.Include(item);
        return await query.Where(criteria).ToListAsync();
    }
    public async Task<(IEnumerable<T>, PaginationMetadata)> GetPageAsync(int pageNumber, int pageSize)
    {
        var collection = await _context.Set<T>().ToListAsync();
        var totalCount = await CountAsync();
        var paginationMetaData = new PaginationMetadata(totalCount, pageSize, pageNumber);
        return (collection, paginationMetaData);

    }

    public async Task<T> AddAsync(T entity)
    {
        await _context.Set<T>().AddAsync(entity);
        return entity;
    }
    public async Task<IEnumerable<T>> AddRangeAsync(IEnumerable<T> entities)
    {
        await _context.Set<T>().AddRangeAsync(entities);
        return entities;
    }

    public T Update(T entity)
    {
        //_context.Set<T>().Update(entity);
        _context.Entry(entity).State = EntityState.Modified;
        return entity;
    }

    public void Delete(T entity)
        => _context.Set<T>().Remove(entity);
    public void DeleteRange(IEnumerable<T> entities)
        => _context.Set<T>().RemoveRange(entities);

    public void Attach(T entity)
        => _context.Set<T>().Attach(entity);
    public void AttachRange(IEnumerable<T> entities)
        => _context.Set<T>().AttachRange(entities);

    public async Task<int> CountAsync()
        => await _context.Set<T>().CountAsync();
    public async Task<int> CountAsync(Expression<Func<T, bool>> criteria)
        => await _context.Set<T>().CountAsync(criteria);

    public IQueryable<T> Get(Expression<Func<T, bool>> criteria)
    => _context.Set<T>().Where(criteria);

    public PagingVm<TOutput> GetPageAsync<TOutput>(string orderBy = "Id", bool isAscending = false, int pageIndex = 1, int pageSize = 10)
    {
        var query = _context.Set<T>().OrderByPropertyName(orderBy, isAscending)
                   .ProjectTo<TOutput>(_mapper.ConfigurationProvider);

        return PagingHelper.Create(query, pageIndex, pageSize);
    }

}
