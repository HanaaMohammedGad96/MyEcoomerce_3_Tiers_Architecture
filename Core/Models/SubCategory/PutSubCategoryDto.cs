namespace Core.Models.SubCategory;

public class PutSubCategoryDto : PostPutSubCategoryDto
{
    public class PostSubCategoryValidator : PostPutSubCategoryValidator<PutSubCategoryDto>
    { }
    public bool IsActive { get; set; }
}
