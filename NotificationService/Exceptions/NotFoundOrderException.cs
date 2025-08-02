using ECommerce.Common.Exceptions;

namespace NotificationService.Exceptions
{
    public class NotFoundOrderException : CommonApplicationException
    {
        public NotFoundOrderException() : base(System.Net.HttpStatusCode.NotFound, "not_found_order_id", "Order not found")
        {

        }
    }
}
