namespace MailService.Models
{
    public class EmailResponse
    {
        public bool Success { get; set; }
        public string Message { get; set; }
        public DateTime SentAt { get; set; }
    }
}
