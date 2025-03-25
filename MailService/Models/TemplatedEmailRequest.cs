namespace MailService.Models
{
    public class TemplatedEmailRequest
    {
        public string ToEmail { get; set; }
        public string Subject { get; set; }
        public string TemplateName { get; set; }
        public Dictionary<string, string>? TemplateData { get; set; }
    }
}
