using Core.Models.Category;
using Microsoft.AspNetCore.Http;

namespace Core.Models.Category;

public class PostCategoryDto : PostPutCategoryDto
{  
    public IFormFile? CategoryImage { get; set; }
}
