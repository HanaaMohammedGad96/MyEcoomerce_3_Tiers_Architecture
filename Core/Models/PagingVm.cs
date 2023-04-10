namespace Core.Models;

public class PagingVm<T>
{
    public PageInfo? PageInfo { get; set; }
    
    public IEnumerable<T> Items { get; set; }
}


public class PageInfo
{
    public int PageIndex { get; set; }
    public int PageSize { get; set; }
    public int TotalPages { get; set; }

}