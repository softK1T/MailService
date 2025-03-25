using MailService.Models;
using MailService.Services;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

[Route("api/email")]
[ApiController]
public class EmailController : ControllerBase
{
    private readonly IEmailService _emailService;

    public EmailController(IEmailService emailService)
    {
        _emailService = emailService;
    }

    [HttpPost("send")]
    public async Task<IActionResult> SendEmail([FromForm] MailRequest request)
    {
        var result = await _emailService.SendEmailAsync(request);
        if (result.Success)
            return Ok(result);
        return BadRequest(result);
    }

    [HttpPost("send-template")]
    public async Task<IActionResult> SendTemplatedEmail([FromBody] TemplatedEmailRequest request)
    {
        var result = await _emailService.SendTemplatedEmailAsync(
            request.ToEmail,
            request.Subject,
            request.TemplateName,
            request.TemplateData
        );

        if (result.Success)
            return Ok(result);
        return BadRequest(result);
    }
}
