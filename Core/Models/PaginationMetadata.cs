namespace Core.Models;

public class PaginationMetadata
{
    public PaginationMetadata(int totalItemCount, int pageSize, int currentPage)
    {
        TotalCount = totalItemCount;
        PageSize = pageSize;
        CurrentPage = currentPage;
        TotalPages = (int)Math.Ceiling(totalItemCount / (double)pageSize);
    }
    public int TotalCount { get; set; }
    public int TotalPages { get; set; }
    public int PageSize { get; set; }
    public int CurrentPage { get; set; }

}
