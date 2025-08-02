using Asp.Versioning;
using Microsoft.AspNetCore.Mvc;
using OrderService.Models.v1.DTOs;
using OrderService.Services;
using System.Net;

namespace OrderService.Controllers.Internal.v1
{
    [ApiVersion("1.0")]
    [ApiController]
    [Route("api/internal/v{version:apiVersion}/users/{userId}/[controller]")]
    public class OrdersController : ControllerBase
    {
        private readonly IOrderService _orderService;
        private readonly ILogger<OrdersController> _logger;

        public OrdersController(IOrderService orderService, ILogger<OrdersController> logger)
        {
            _orderService = orderService;
            _logger = logger;
        }

        [HttpGet]
        [MapToApiVersion("1.0")]
        [Route("{orderId}")]
        [ProducesResponseType(typeof(OrderDto), (int)HttpStatusCode.OK)]
        public async Task<IActionResult> GetOrderInfoAsync([FromRoute] Guid userId, [FromRoute] Guid orderId)
        {
            _logger.LogInformation("Get order info for user {UserId}, order {OrderId}", userId, orderId);
            var order = await _orderService.GetOrderAsync(userId, orderId);
            return Ok(order);
        }

        [HttpPost]
        [MapToApiVersion("1.0")]
        [Route("{orderId}/confirm-payment")]
        [ProducesResponseType((int)HttpStatusCode.OK)]
        public async Task<IActionResult> ConfirmPaymentAsync([FromRoute] Guid orderId, [FromBody] ConfirmPaymentRequest request)
        {
            _logger.LogInformation("Confirming payment for order {OrderId} with isConfirmed {IsConfirmed}", orderId, request.IsConfirmed);
            await _orderService.ConfirmPaymentForOrder(orderId, request);
            return Ok();
        }
    }
}
