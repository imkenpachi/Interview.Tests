using Asp.Versioning;
using Microsoft.AspNetCore.Mvc;
using PaymentService.Models.v1.DTOs;
using PaymentService.Services;
using System.Net;

namespace PaymentService.Controllers.v1;

//TODO: Signature verification
[ApiVersion("1.0")]
[ApiController]
[Route("api/v{version:apiVersion}/[controller]")]
public class PaymentsController : ControllerBase
{
    private readonly IPaymentService _paymentService;
    private readonly ILogger<PaymentsController> _logger;

    public PaymentsController(IPaymentService paymentService, ILogger<PaymentsController> logger)
    {
        _paymentService = paymentService;
        _logger = logger;
    }

    [HttpPost]
    [MapToApiVersion("1.0")]
    [Route("webhook")]
    [ProducesResponseType((int)HttpStatusCode.OK)]
    public async Task<IActionResult> WebhookNotificationAsync(WebhookNotificationMessage request)
    {
        await _paymentService.HandleWebHookNotificationAsync(request);
        return Ok();
    }
}
