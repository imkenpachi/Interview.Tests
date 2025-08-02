using Asp.Versioning;
using Microsoft.AspNetCore.Mvc;
using PaymentService.Models.v1.DTOs;
using PaymentService.Services;
using System.Net;

namespace PaymentService.Controllers.Internal.v1
{
    [ApiVersion("1.0")]
    [ApiController]
    [Route("api/internal/v{version:apiVersion}/users/{userId}/[controller]")]
    public class PaymentsController : ControllerBase
    {
        private readonly IPaymentService _paymentService;
        private readonly ILogger<PaymentsController> _logger;

        public PaymentsController(IPaymentService PaymentService, ILogger<PaymentsController> logger)
        {
            _paymentService = PaymentService;
            _logger = logger;
        }

        [HttpPost]
        [MapToApiVersion("1.0")]
        [ProducesResponseType((int)HttpStatusCode.OK)]
        public async Task<IActionResult> RequestPaymentAsync(Guid userId, PaymentRequest request)
        {
            _logger.LogInformation("Requesting payment for user {UserId} order {OrderId} to external provider {Provider}", userId, request.OrderId, request.PaymentProvider);
            await _paymentService.CreatePaymentAsync(userId, request);
            return Ok();
        }
    }
}
