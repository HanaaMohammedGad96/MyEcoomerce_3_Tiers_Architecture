using Core.Resources.Category;
using Core.Resources.Common;
using FluentValidation;

namespace Core.Models.Category;

public class PostPutCategoryDto
{
    public class CategoryValidator : AbstractValidator<PostPutCategoryDto>
    {
        public CategoryValidator()
        {
            RuleFor(x => x.NameArabic).NotEmpty().WithMessage(String.Format(SharedRes.NotEmptyField, CategoryRes.NameArabic));
            RuleFor(x => x.NameEnglish).NotEmpty().WithMessage(String.Format(SharedRes.NotEmptyField, CategoryRes.NameEnglish));
            RuleFor(x => x.DescriptionArabic).NotEmpty().WithMessage(String.Format(SharedRes.NotEmptyField, CategoryRes.DescriptionArabic));
            RuleFor(x => x.DescriptionEnglish).NotEmpty().WithMessage(String.Format(SharedRes.NotEmptyField, CategoryRes.DescriptionEnglish));
        }
    }

    public string NameArabic { get; set; }
    public string NameEnglish { get; set; }
    public string DescriptionArabic { get; set; }
    public string DescriptionEnglish { get; set; }
    public bool IsActive { get; set; }
}
