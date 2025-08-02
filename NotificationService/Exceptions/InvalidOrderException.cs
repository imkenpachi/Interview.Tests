using ECommerce.Common.Exceptions;

namespace NotificationService.Exceptions
{
    public class InvalidOrderException : CommonApplicationException
    {
        public InvalidOrderException() : base(System.Net.HttpStatusCode.BadRequest, "invalid_order_id", "Invalid order id provided")
        {

        }
    }
}
