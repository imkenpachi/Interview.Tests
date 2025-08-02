using ECommerce.Common.Exceptions;

namespace PaymentService.Exceptions
{
    public class InvalidPaymentException : CommonApplicationException
    {
        public InvalidPaymentException() : base(System.Net.HttpStatusCode.BadRequest, "invalid_order_id", "Invalid order id provided")
        {

        }
    }
}
