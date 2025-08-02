using ECommerce.Common.Exceptions;

namespace PaymentService.Exceptions
{
    public class MissingTransactionException : CommonApplicationException
    {
        public MissingTransactionException() : base(System.Net.HttpStatusCode.BadRequest, "transaction_not_provided", "Transaction not provided")
        {

        }
    }
}
