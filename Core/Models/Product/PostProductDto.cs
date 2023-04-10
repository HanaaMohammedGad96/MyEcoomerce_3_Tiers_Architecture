using FluentValidation;
using Microsoft.AspNetCore.Http;

namespace Core.Models.Product;

public class PostProductDto  : PostPutProductDto
{
    public class PostProductValidator : PostPutProductValidator<PostProductDto>
    {
        public PostProductValidator()
        {
            RuleFor(x => x.ImagesOfProduct).NotNull().NotEmpty().WithMessage("please enter image");
        }
    }
    public List<IFormFile> ImagesOfProduct { get; set; }
}
