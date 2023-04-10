using Microsoft.AspNetCore.Http;

namespace Core.Models.Product;

public class ProductImageDto
{
    public Guid Id { get; set; }
    public IFormFile Image { get; set; }
}
