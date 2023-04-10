namespace Core.Models.Category;

public class PutCategoryDto  : PostPutCategoryDto
{
    public DateTime ModifiedDate { get; set; } = DateTime.Now;
}
