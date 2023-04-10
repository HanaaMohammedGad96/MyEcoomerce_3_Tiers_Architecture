using Core.Resources.Common;
using Core.Resources.Product;
using FluentValidation;

namespace Core.Models.Product;

public class PostPutProductDto
{
    public class PostPutProductValidator<T> : AbstractValidator<T> where T : PostPutProductDto
    {
        public PostPutProductValidator()
        {
            RuleFor(p => p.NameArabic).NotEmpty().WithMessage(String.Format(SharedRes.NotEmptyField, ProductRes.NameArabic));
            RuleFor(p => p.NameEnglish).NotEmpty().WithMessage(String.Format(SharedRes.NotEmptyField, ProductRes.NameEnglish));
            RuleFor(p => p.ShortDescriptionArabic).NotEmpty().WithMessage(String.Format(SharedRes.NotEmptyField, ProductRes.ShortDescriptionArabic));
            RuleFor(p => p.ShortDescriptionEnglish).NotEmpty().WithMessage(String.Format(SharedRes.NotEmptyField, ProductRes.ShortDescriptionEnglish));
            RuleFor(p => p.DescriptionArabic).NotEmpty().WithMessage(String.Format(SharedRes.NotEmptyField, ProductRes.DescriptionArabic));
            RuleFor(p => p.DescriptionEnglish).NotEmpty().WithMessage(String.Format(SharedRes.NotEmptyField, ProductRes.DescriptionEnglish));

            RuleFor(p => p.Price).GreaterThan(0).WithMessage("").NotEmpty().WithMessage("");
            RuleFor(p => p.CountInStock).GreaterThanOrEqualTo(0).WithMessage("").NotEmpty().WithMessage("");
            RuleFor(p => p.SubCategoryId).NotEmpty().WithMessage("");
        }
    }

    public string NameArabic { get; set; }
    public string NameEnglish { get; set; }
    public string ShortDescriptionArabic { get; set; }
    public string ShortDescriptionEnglish { get; set; }
    public string DescriptionArabic { get; set; }
    public string DescriptionEnglish { get; set; }
    public decimal Price { get; set; }
    public int CountInStock { get; set; }
    public Guid SubCategoryId { get; set; }
}
