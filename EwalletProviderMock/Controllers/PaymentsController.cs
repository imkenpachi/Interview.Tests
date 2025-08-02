using EwalletProviderMock.Models;
using Microsoft.AspNetCore.Mvc;
using System.Text;
using Newtonsoft.Json;

namespace EwalletProviderMock.Controllers;

[ApiController]
[Route("api/[controller]")]
public class PaymentsController : ControllerBase
{
    private readonly ILogger<PaymentsController> _logger;

    public PaymentsController(ILogger<PaymentsController> logger)
    {
        _logger = logger;
    }

    [HttpPost]
    [Route("payment-requests")]
    public CreatePaymentResponse Post(CreatePaymentRequest createPaymentRequest)
    {
        var paymentRequestId = Guid.NewGuid();
        
        //Fire and forget
        Task.Run(async () =>
        {
            var randomDelay = new Random().Next(3000, 10000);
            await Task.Delay(randomDelay);

            var payload = new WebhookNotificationMessage
            {
                PaymentRequestId = paymentRequestId,
                StatusCode = SimulateStatusCodeByAmount(createPaymentRequest.Amount),
                TotalAmount = createPaymentRequest.Amount,
                Transaction = SimulateTransactionByAmount(createPaymentRequest),
            };
            var jsonContent = new StringContent(JsonConvert.SerializeObject(payload), Encoding.UTF8, "application/json");
            try
            {
                var response = await new HttpClient().PostAsync(createPaymentRequest.NotificationUri, jsonContent);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Webhook failed");
            }
        });

        var response = new CreatePaymentResponse { PaymentRequestId =  paymentRequestId };
        return response;
    }


    private static string SimulateStatusCodeByAmount(decimal amount)
    {
        decimal factional = amount - Math.Truncate(amount);

        switch (factional)
        {
            case 0.99m:
                return "SUCCESS";
            case 0.11m:
                return "FAILED";
            default:
                return "EXPIRED";
        }
    }
    private static Transaction? SimulateTransactionByAmount(CreatePaymentRequest createPaymentRequest)
    {
        decimal factional = createPaymentRequest.Amount - Math.Truncate(createPaymentRequest.Amount);

        switch (factional)
        {
            case 0.99m:
                return new Transaction { 
                    OrderId = createPaymentRequest.MerchanData.OrderId,
                    TransactionId = Guid.NewGuid(),
                    TransactionTime = DateTime.UtcNow,
                };
            default:
                return default;
        }
    }
}
