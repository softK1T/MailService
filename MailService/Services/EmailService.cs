using MailKit.Net.Smtp;
using MailKit.Security;
using MailService.Models;
using MailService.Services;
using Microsoft.Extensions.Options;
using MimeKit;
using System.IO;
using System.Threading.Tasks;

public class EmailService : IEmailService
{
    private readonly MailSettings _mailSettings;
    private readonly IWebHostEnvironment _env;

    public EmailService(IOptions<MailSettings> mailSettings, IWebHostEnvironment env)
    {
        _mailSettings = mailSettings.Value;
        _env = env;
    }

    public async Task<EmailResponse> SendEmailAsync(MailRequest request)
    {
        try
        {
            var email = new MimeMessage();
            email.Sender = MailboxAddress.Parse(_mailSettings.Mail);
            email.From.Add(new MailboxAddress(_mailSettings.DisplayName, _mailSettings.Mail));
            email.To.Add(MailboxAddress.Parse(request.ToEmail));
            email.Subject = request.Subject;

            var builder = new BodyBuilder();

            // Handle attachments if any
            if (request.Attachments != null)
            {
                foreach (var file in request.Attachments)
                {
                    if (file.Length > 0)
                    {
                        using (var ms = new MemoryStream())
                        {
                            file.CopyTo(ms);
                            var fileBytes = ms.ToArray();
                            builder.Attachments.Add(file.FileName, fileBytes, ContentType.Parse(file.ContentType));
                        }
                    }
                }
            }

            builder.HtmlBody = request.Body;
            email.Body = builder.ToMessageBody();

            using (var smtp = new SmtpClient())
            {
                smtp.Connect(_mailSettings.Host, _mailSettings.Port, _mailSettings.UseSSL ? SecureSocketOptions.StartTls : SecureSocketOptions.None);
                smtp.Authenticate(_mailSettings.Mail, _mailSettings.Password);
                await smtp.SendAsync(email);
                smtp.Disconnect(true);
            }

            return new EmailResponse
            {
                Success = true,
                Message = "Email sent successfully",
                SentAt = DateTime.UtcNow
            };
        }
        catch (Exception ex)
        {
            return new EmailResponse
            {
                Success = false,
                Message = ex.Message,
                SentAt = DateTime.UtcNow
            };
        }
    }

    public async Task<EmailResponse> SendTemplatedEmailAsync(string toEmail, string subject, string templateName, Dictionary<string, string> templateData)
    {
        try
        {
            string templatePath = Path.Combine(_env.WebRootPath, "Templates", "EmailTemplates", $"{templateName}.html");
            string htmlBody = await File.ReadAllTextAsync(templatePath);

            // Replace placeholders with actual data
            foreach (var item in templateData)
            {
                htmlBody = htmlBody.Replace($"{{{{{item.Key}}}}}", item.Value);
            }

            var mailRequest = new MailRequest
            {
                ToEmail = toEmail,
                Subject = subject,
                Body = htmlBody
            };

            return await SendEmailAsync(mailRequest);
        }
        catch (Exception ex)
        {
            return new EmailResponse
            {
                Success = false,
                Message = ex.Message,
                SentAt = DateTime.UtcNow
            };
        }
    }
}
