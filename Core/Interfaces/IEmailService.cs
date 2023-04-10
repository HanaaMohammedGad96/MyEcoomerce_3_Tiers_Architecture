using Microsoft.AspNetCore.Http;

namespace Core.Interfaces;

public interface IEmailService
{
    Task SendEmailAsync(string mailTo, string subject, string body, IList<IFormFile> attachments = null);
}
