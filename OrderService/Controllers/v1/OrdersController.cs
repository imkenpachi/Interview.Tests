using Asp.Versioning;
using Microsoft.AspNetCore.Mvc;
using OrderService.Models.v1.DTOs;
using OrderService.Services;
using System.Net;

namespace OrderService.Controllers.v1;

//TODO: Auth guard
[ApiVersion("1.0")]
[ApiController]
[Route("api/v{version:apiVersion}/users/{userId}/[controller]")]
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
    [Route("")]
    [ProducesResponseType(typeof(PagedResponseDto<OrderDto>), (int)HttpStatusCode.OK)]
    public async Task<IActionResult> GetOrdersAsync(
        [FromRoute] Guid userId,
        [FromQuery] string? orderName = null,
        [FromQuery] int pageNumber = 1,
        [FromQuery] int pageSize = 10)
    {
        var orders = await _orderService.GetOrdersAsync(userId, orderName, pageNumber, pageSize);
        return Ok(orders);
    }

    [HttpPost]
    [MapToApiVersion("1.0")]
    [Route("{orderId}/checkout")]
    [ProducesResponseType(typeof(CheckoutOrderResponse), (int)HttpStatusCode.OK)]
    public async Task<IActionResult> CheckoutOrderAsync([FromRoute] Guid userId, [FromRoute] Guid orderId, [FromBody] CheckoutOrderRequest request)
    {
        var orders = await _orderService.CheckoutOrderAsync(userId, orderId, request);
        return Ok(orders);
    }
}
