using FluentValidation;
using Microsoft.AspNetCore.Http;

namespace Core.Models.SubCategory;

public class PostSubCategoryDto : PostPutSubCategoryDto
{
    public class PostSubCategoryValidator : PostPutSubCategoryValidator<PostSubCategoryDto>
    {
        public PostSubCategoryValidator()
        {
            RuleFor(x => x.SubCategoryImage).NotNull().NotEmpty().WithMessage("please enter image");
        }
    }
    public IFormFile? SubCategoryImage { get; set; }
}
