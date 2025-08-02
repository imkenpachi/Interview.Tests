using Asp.Versioning;
using Microsoft.AspNetCore.Mvc;
using NotificationService.Models.v1.DTOs;
using NotificationService.Services;
using System.Net;

namespace NotificationService.Controllers.Internal.v1
{
    [ApiVersion("1.0")]
    [ApiController]
    [Route("api/internal/v{version:apiVersion}/users/{userId}/[controller]")]
    public class EmailsController : ControllerBase
    {
        private readonly IEmailService _notificationService;
        private readonly ILogger<EmailsController> _logger;

        public EmailsController(IEmailService NotificationService, ILogger<EmailsController> logger)
        {
            _notificationService = NotificationService;
            _logger = logger;
        }

        [HttpPost]
        [MapToApiVersion("1.0")]
        [Route("send")]
        [ProducesResponseType((int)HttpStatusCode.OK)]
        public async Task<IActionResult> SendEmailNotificationAsync([FromRoute] Guid userId, [FromBody] EmailNotificationRequest request)
        {
            _logger.LogInformation("Sending email to user {UserId} with subject {Subject}", userId, request.Subject);
            await _notificationService.SendEmailNotificationAsync(userId, request);
            return Ok();
        }
    }
}
