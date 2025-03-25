using MailService.Models;

namespace MailService.Services
{
    public interface IEmailService
    {
        Task<EmailResponse> SendEmailAsync(MailRequest mailRequest);
        Task<EmailResponse> SendTemplatedEmailAsync(string toEmail, string subject, string templateName, Dictionary<string, string> templateData);
    }
}
