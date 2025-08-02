using OrderService.Models.v1.Enums;
using OrderService.Exceptions;
using OrderService.ExternalServices;
using OrderService.Models.v1.DTOs;
using OrderService.Models.v1.DTOs.External;
using OrderService.Repositories;
using ECommerce.Common.Clients;
using OrderService.Models.v1.DomainModels;

namespace OrderService.Services
{
    public class OrderService : IOrderService
    {
        private readonly IOrderRepository _orderRepository;
        private readonly IOrderProcessService _processService;
        private readonly IExternalPaymentService _externalPaymentService;
        private readonly ILogger<OrderService> _logger;
        public OrderService(IOrderRepository orderRepository, IExternalPaymentService externalPaymentService, IOrderProcessService processService, ILogger<OrderService> logger)
        {
            _orderRepository = orderRepository;
            _externalPaymentService = externalPaymentService;
            _processService = processService;
            _logger = logger;
        }

        public async Task<PagedResponseDto<OrderDto>> GetOrdersAsync(Guid userId, string? orderName, int pageNumber, int pageSize)
        {
            var (orders, totalRecords) = await _orderRepository.GetOrdersWithPaginationAsync(userId, orderName, pageNumber, pageSize);

            return new PagedResponseDto<OrderDto>
            {
                Data = orders,
                Pagination = new PaginationMetadata
                {
                    CurrentPage = pageNumber,
                    PageSize = pageSize,
                    TotalRecords = totalRecords
                }
            };
        }

        public async Task<OrderDto> GetOrderAsync(Guid userId, Guid orderId)
        {
            var order = await _orderRepository.GetOrderDtoAsync(userId, orderId);

            return order ?? throw new NotFoundOrderException();
        }

        public async Task<CheckoutOrderResponse> CheckoutOrderAsync(Guid userId, Guid orderId, CheckoutOrderRequest request)
        {
            var order = await _orderRepository.GetAsync(userId, orderId) ?? throw new NotFoundOrderException();
            var allowedStatuses = new OrderStatus[] { OrderStatus.Created, OrderStatus.PaymentFailed };

            if (!allowedStatuses.Contains(order.Status))
            {
                throw new InvalidOrderException();
            }

            var paymentRequest = new PaymentRequest
            {
                OrderId = order.Id,
                UserId = order.UserId,
                Amount = order.TotalAmount,
                PaymentMethod = request.PaymentProvider.ToString(),
            };

            await _externalPaymentService.CreatePaymentAsync(userId, paymentRequest);

            await UpdateOrderStatus(userId, order.Id, OrderStatus.PaymentPending);

            return new CheckoutOrderResponse
            {
                OrderId = userId,
                Status = OrderStatus.PaymentPending,
                UserId = userId,
            };
        }

        public async Task ConfirmPaymentForOrder(Guid orderId, ConfirmPaymentRequest confirmPaymentRequest)
        {
            var order = await _orderRepository.GetAsync(orderId);
            if (order == null)
            {
                _logger.LogError("Could not find order for id {OrderId}", orderId);
                throw new NotFoundOrderException();
            }

            order.Status = confirmPaymentRequest.IsConfirmed ? OrderStatus.PaymentSuccess : OrderStatus.PaymentFailed;
            _orderRepository.Update(order);
            await _orderRepository.SaveChangeAsync();

            await _processService.CreateOrderProcessAsync(order.UserId, orderId, confirmPaymentRequest.IsConfirmed ? OrderPaymentProcessFlow.Success : OrderPaymentProcessFlow.Failure);
        }

        private async Task UpdateOrderStatus(Guid userId, Guid orderId, OrderStatus newStatus)
        {
            var order = await _orderRepository.GetAsync(userId, orderId);
            if (order == null)
            {
                throw new NotFoundOrderException();
            }

            order.Status = newStatus;
            _orderRepository.Update(order);
            await _orderRepository.SaveChangeAsync();
        }

    }
}
