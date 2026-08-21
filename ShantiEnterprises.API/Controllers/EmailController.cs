using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ShantiEnterprises.API.DTOs.Email;
using ShantiEnterprises.API.Interfaces;

namespace ShantiEnterprises.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize(Roles = "Admin")]
    public class EmailController : ControllerBase
    {
        private readonly IEmailService _emailService;

        public EmailController(IEmailService emailService)
        {
            _emailService = emailService;
        }

        [HttpPost("test")]
        public async Task<IActionResult> SendTestEmail(
            [FromBody] EmailTestRequestDto request)
        {
            if (string.IsNullOrWhiteSpace(request.ToEmail))
            {
                return BadRequest(new
                {
                    message = "Recipient email is required."
                });
            }

            try
            {
                await _emailService.SendEmailAsync(
                    request.ToEmail,
                    "Shanti Enterprises - Test Email",
                    """
                    <h2>Email Service Working</h2>
                    <p>This is a test email from Shanti Enterprises API.</p>
                    <p>Your SMTP email service has been configured successfully.</p>
                    """,
                    true);

                return Ok(new
                {
                    message = "Test email sent successfully."
                });
            }
            catch (Exception)
            {
                return StatusCode(500, new
                {
                    message = "Unable to send test email."
                });
            }
        }
    }
}