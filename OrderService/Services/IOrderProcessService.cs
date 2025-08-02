using OrderService.Models.v1.Enums;
using OrderService.Models.v1.DTOs;
using OrderService.Models.v1.DomainModels;

namespace OrderService.Services
{
    public interface IOrderProcessService
    {
        Task<OrderProcess> CreateOrderProcessAsync(Guid userId, Guid orderId, OrderPaymentProcessFlow orderPaymentProcessFlow);
        Task UpdateProcessJobStatusAsync(Guid processId, UpdateProcessJobRequest request);
    }
}
