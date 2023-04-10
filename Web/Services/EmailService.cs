using Core.Helpers;
using Core.Interfaces;
using MailKit.Net.Smtp;
using MailKit.Security;
using Microsoft.Extensions.Options;
using MimeKit;

namespace Web.Services;

public class EmailService : IEmailService
{
    private readonly EmailSettings _emailSetting;
    public EmailService(IOptions<EmailSettings> emailSettings)
    {
       _emailSetting= emailSettings.Value;
    }
    public async Task SendEmailAsync(string mailTo, string subject, string body, IList<IFormFile> attachments = null)
    {
        var email = new MimeMessage
        {
            Sender = MailboxAddress.Parse(_emailSetting.Email),
            Subject = subject
        };

        email.To.Add(MailboxAddress.Parse(mailTo));

        var builder = new BodyBuilder();

        if (attachments != null) 
        {
            byte[] fileBytes;
            foreach (var file in attachments)
            {
                if (file.Length > 0)
                {
                   using var memoryStream = new MemoryStream();
                    file.CopyTo(memoryStream);
                    fileBytes = memoryStream.ToArray();

                    builder.Attachments.Add(file.FileName, fileBytes, ContentType.Parse(file.ContentType));
                }
            }
        }

        builder.HtmlBody = body;
        email.Body = builder.ToMessageBody();
        email.From.Add(new MailboxAddress(_emailSetting.DisplayName, _emailSetting.Email));

        using var smtp = new SmtpClient();
        smtp.Connect(_emailSetting.Host, _emailSetting.Port, SecureSocketOptions.StartTls);
        smtp.Authenticate(_emailSetting.Email, _emailSetting.Password);
        await smtp.SendAsync(email);
        smtp.Disconnect(true);
    }
}
