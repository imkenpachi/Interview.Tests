using ECommerce.Common.Exceptions;

namespace PaymentService.Exceptions
{
    public class NotFoundOrderException : CommonApplicationException
    {
        public NotFoundOrderException() : base(System.Net.HttpStatusCode.NotFound, "not_found_order", "Order not found")
        {

        }
    }
}
