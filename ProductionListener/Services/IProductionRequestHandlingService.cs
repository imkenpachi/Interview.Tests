using ProductionListener.Models.DTOs;
using ProductionListener.Models.Enums;

namespace ProductionListener.Services
{
    public interface IProductionRequestHandlingService
    {
        Task<(ResponseCode, string note)> ProcessMessageAsync(UpdateOrderPaymentStatusMessage message);
        Task ProcessExceptionAsync(Exception exception, UpdateOrderPaymentStatusMessage message);
    }
}
