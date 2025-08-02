using PaymentService.Configurations;
using PaymentService.Exceptions;
using PaymentService.ExternalServices;
using PaymentService.Models.v1.DomainModels;
using PaymentService.Models.v1.DTOs;
using PaymentService.Models.v1.DTOs.External;
using PaymentService.Models.v1.Enums;
using PaymentService.Repositories;

namespace PaymentService.Services
{
    public class PaymentService : IPaymentService
    {
        private const string WEBHOOK_ENDPOINT = "api/v1/payments/webhook";
        private readonly IPaymentRepository _paymentRepository;
        private readonly IPaymentTransactionRepository _paymentTransactionRepository;
        private readonly IExternalEwalletProviderService _externalEwalletProvider;
        private readonly IExternalOrderService _externalProductionService;
        private readonly string _notificationUrl;

        public PaymentService(IPaymentRepository paymentRepository, IPaymentTransactionRepository paymentTransactionRepository,
            IExternalEwalletProviderService externalEwalletProvider, IExternalOrderService externalProductionService, IServiceConfiguration serviceConfiguration)
        {
            _paymentRepository = paymentRepository;
            _paymentTransactionRepository = paymentTransactionRepository;
            _externalEwalletProvider = externalEwalletProvider;
            _externalProductionService = externalProductionService;
            _notificationUrl = $"{serviceConfiguration.PublicUrl}/{WEBHOOK_ENDPOINT}";
        }

        public async Task<PaymentResponse> CreatePaymentAsync(Guid userId, PaymentRequest paymentRequest)
        {
            var ewalletPaymentRequest = new EwalletPaymentRequest()
            {
                Amount = paymentRequest.Amount,
                NotificationUri = _notificationUrl,
                MerchanData = new MerchanData
                {
                    OrderId = paymentRequest.OrderId,
                }
            };

            var paymentResponse = await _externalEwalletProvider.CreateEwalletPaymentRequestAsync(ewalletPaymentRequest);

            var payment = new Payment()
            {
                Id = Guid.NewGuid(),
                UserId = userId,
                ExternalId = paymentResponse.PaymentRequestId,
                OrderId = paymentRequest.OrderId,
                PaymentProvider = paymentRequest.PaymentProvider,
                Status = PaymentStatus.Requested,
            };

            _paymentRepository.Insert(payment);
            await _paymentRepository.SaveChangeAsync();

            var response = new PaymentResponse() { PaymentId = payment.Id, PaymentStatus = payment.Status };

            return response;
        }

        public async Task HandleWebHookNotificationAsync(WebhookNotificationMessage message)
        {
            switch (message.StatusCode)
            {
                case "SUCCESS":
                    await HandleSuccessPaymentAsync(message);
                    break;
                case "FAILED":
                    await HandleFailedPaymentAsync(message);
                    break;
                case "EXPIRED":
                    await HandleExpiredPaymentAsync(message);
                    break;
                default:
                    await HandleFailedPaymentAsync(message);
                    break;
            }
        }

        private async Task HandleSuccessPaymentAsync(WebhookNotificationMessage message)
        {
            if (message.Transaction is null)
            {
                throw new MissingTransactionException();
            }

            var payment = await _paymentRepository.GetByExternalIdAsync(message.PaymentRequestId);

            if (payment is null)
            {
                throw new NotFoundPaymentException();
            }

            payment.Status = PaymentStatus.Success;

            var paymentTransaction = new PaymentTransaction
            {
                Amount = message.TotalAmount,
                PaymentId = payment.Id,
                TransactionId = message.Transaction.TransactionId,
                TransactionTime = message.Transaction.TransactionTime,
            };

            _paymentRepository.Update(payment);
            _paymentTransactionRepository.Insert(paymentTransaction);
            await _paymentTransactionRepository.SaveChangeAsync();

            await UpdatePaymentConfirmationForOrder(payment.UserId, payment.OrderId, true);
        }

        private async Task HandleFailedPaymentAsync(WebhookNotificationMessage message)
        {
            var payment = await _paymentRepository.GetByExternalIdAsync(message.PaymentRequestId);

            if (payment is null)
            {
                throw new NotFoundPaymentException();
            }

            payment.Status = PaymentStatus.Failed;
            _paymentRepository.Update(payment);
            await _paymentTransactionRepository.SaveChangeAsync();

            await UpdatePaymentConfirmationForOrder(payment.UserId, payment.OrderId, false);
        }

        private async Task HandleExpiredPaymentAsync(WebhookNotificationMessage message)
        {
            var payment = await _paymentRepository.GetByExternalIdAsync(message.PaymentRequestId);

            if (payment is null)
            {
                throw new NotFoundPaymentException();
            }

            payment.Status = PaymentStatus.Expired;
            _paymentRepository.Update(payment);
            await _paymentTransactionRepository.SaveChangeAsync();

            await UpdatePaymentConfirmationForOrder(payment.UserId, payment.OrderId, false);
        }

        private async Task UpdatePaymentConfirmationForOrder(Guid userId, Guid orderId, bool isConfirmed)
        {
            var confirmationRequest = new ConfirmPaymentRequest
            {
                IsConfirmed = isConfirmed,
            };
            await _externalProductionService.SendPaymentConfirmationAsync(userId, orderId, confirmationRequest);
        }
    }
}
