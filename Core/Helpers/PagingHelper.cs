using Core.Models;

namespace Core.Helpers;

public class PagingHelper
{
    public static PagingVm<T> Create<T>(IQueryable<T> query, int pageIndex = 1, int pageSize = 10)
    {
        int totalCount = query.Count();

        if (totalCount <= pageSize || pageIndex <= 0)
            pageIndex = 1;

        int totalPages = (int)Math.Ceiling((double)totalCount / pageSize);

        int excludedRows = (pageIndex - 1) * pageSize;

        var items = query.Skip(excludedRows).Take(pageSize).ToList();

        PageInfo pageInfo = new PageInfo
        {
            PageIndex = pageIndex,
            PageSize = pageSize,
            TotalPages = totalPages,
        };

        return new PagingVm<T>
        {
           PageInfo = pageInfo,
            Items   = items
        };

    }
}
