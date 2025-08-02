using ECommerce.Common.Exceptions;

namespace PaymentService.Exceptions
{
    public class NotFoundPaymentException : CommonApplicationException
    {
        public NotFoundPaymentException() : base(System.Net.HttpStatusCode.NotFound, "not_found_payment", "Payment not found")
        {

        }
    }
}
