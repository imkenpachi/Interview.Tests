using Microsoft.Extensions.Logging;
using ProductionListener.ExternalServices;
using ProductionListener.Models.DTOs;
using ProductionListener.Models.Enums;

namespace ProductionListener.Services
{
    public class ProductionRequestHandlingService : IProductionRequestHandlingService
    {
        private readonly IExternalOrderService _externalOrderService;
        private readonly IExternalProductionService _externalProductionService;
        private readonly ILogger<ProductionRequestHandlingService> _logger;

        public ProductionRequestHandlingService(IExternalOrderService externalOrderService, IExternalProductionService externalProductionService, ILogger<ProductionRequestHandlingService> logger)
        {
            _externalOrderService = externalOrderService;
            _externalProductionService = externalProductionService;
            _logger = logger;
        }

        public Task ProcessExceptionAsync(Exception exception, UpdateOrderPaymentStatusMessage message)
        {
            _logger.LogInformation("Processing production request exception for order {OrderId}, process {ProcessId}, for user {UserId}", message.OrderId, message.ProcessId, message.UserId);

            return Task.FromResult(0);
        }

        public async Task<(ResponseCode, string note)> ProcessMessageAsync(UpdateOrderPaymentStatusMessage message)
        {
            _logger.LogInformation("Processing production request for order {OrderId}, process {ProcessId}, for user {UserId}", message.OrderId, message.ProcessId, message.UserId);

            try
            {
                var orderInfo = await _externalOrderService.GetOrderInfoAsync(message.UserId, message.OrderId);

                var productionRequest = new ProductionRequest
                {
                    OrderId = orderInfo.Id,
                    OrderName = orderInfo.Name,
                    UserId = message.UserId,
                    OrderStatus = orderInfo.Status.ToString(),
                    OrderDetails = orderInfo.OrderDetails,
                };

                await _externalProductionService.SendProductionRequestAsync(message.UserId, productionRequest);

                return (ResponseCode.Success, string.Empty);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occured while processing production request for order {OrderId}, process {ProcessId}, for user {UserId}", message.OrderId, message.ProcessId, message.UserId);
                return (ResponseCode.ExternalServiceCallFailed, ex.Message);
            }
        }
    }
}
