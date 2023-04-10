using Microsoft.AspNetCore.Http;

namespace Core.Models.Product;

public class PutProductDto : PostPutProductDto
{
    public class PutProductValidator : PostPutProductValidator<PutProductDto>
    { }
    public List<IFormFile> ImagesOfProduct { get; set; }
}
