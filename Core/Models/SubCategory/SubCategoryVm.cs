namespace Core.Models.SubCategory;

public class SubCategoryVm
{
    public Guid Id { get; set; }
    public string? ParentCategoryName { get; set; }
    public string? Name { get; set; }
    public string? Description { get; set; }
    public string? ImagePath { get; set; }
    public bool IsActive { get; set; }
    public DateTime CreatedDate { get; set; }
    public DateTime ModifiedDate { get; set; }
}
