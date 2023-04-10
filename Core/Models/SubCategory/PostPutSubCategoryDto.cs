using Core.Resources.Category;
using Core.Resources.Common;
using FluentValidation;

namespace Core.Models.SubCategory;

public class PostPutSubCategoryDto
{
    public class PostPutSubCategoryValidator<T> : AbstractValidator<T> where T : PostPutSubCategoryDto
    {
        public PostPutSubCategoryValidator()
        {
            RuleFor(x => x.CategoryId).NotNull().NotEmpty().WithMessage(CategoryRes.ValidCategoryId);
            RuleFor(x => x.NameArabic).NotEmpty().WithMessage(String.Format(SharedRes.NotEmptyField, CategoryRes.NameArabic));
            RuleFor(x => x.NameEnglish).NotEmpty().WithMessage(String.Format(SharedRes.NotEmptyField, CategoryRes.NameEnglish));
            RuleFor(x => x.DescriptionArabic).NotEmpty().WithMessage(String.Format(SharedRes.NotEmptyField, CategoryRes.DescriptionArabic));
            RuleFor(x => x.DescriptionEnglish).NotEmpty().WithMessage(String.Format(SharedRes.NotEmptyField, CategoryRes.DescriptionEnglish));
        }
    }
    public Guid CategoryId { get; set; }
    public string NameArabic { get; set; }
    public string NameEnglish { get; set; }
    public string DescriptionArabic { get; set; }
    public string DescriptionEnglish { get; set; }
}
