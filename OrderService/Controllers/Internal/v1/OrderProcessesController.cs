using Asp.Versioning;
using Microsoft.AspNetCore.Mvc;
using OrderService.Models.v1.DTOs;
using OrderService.Services;
using System.Net;

namespace OrderService.Controllers.Internal.v1
{
    [ApiVersion("1.0")]
    [ApiController]
    [Route("api/internal/v{version:apiVersion}/[controller]")]
    public class OrderProcessesController : ControllerBase
    {
        private readonly IOrderProcessService _orderProcessService;
        private readonly ILogger<OrderProcessesController> _logger;

        public OrderProcessesController(IOrderProcessService orderProcessService, ILogger<OrderProcessesController> logger)
        {
            _orderProcessService = orderProcessService;
            _logger = logger;
        }

        [HttpPost]
        [MapToApiVersion("1.0")]
        [Route("{processId}/status")]
        [ProducesResponseType((int)HttpStatusCode.OK)]
        public async Task<IActionResult> UpdateProcessJobStatusAsync([FromRoute] Guid processId, [FromBody] UpdateProcessJobRequest request)
        {
            _logger.LogInformation("Updating process job status for job {JobId}, type {JobType} with status {JobStatus}", processId, request.JobType, request.JobStatus);
            await _orderProcessService.UpdateProcessJobStatusAsync(processId, request);
            return Ok();
        }
    }
}
