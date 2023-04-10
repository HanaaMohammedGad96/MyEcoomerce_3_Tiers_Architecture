using Microsoft.AspNetCore.Http;

namespace Core.Models.Attachment;

public class AttachmentDto
{
    public IFormFile? Image { get; set; }
}
