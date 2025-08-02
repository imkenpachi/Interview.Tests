namespace OrderService.Configurations
{
    public class MessageQueueSettings
    {
        public virtual required string RequestProductionQueueName { get; set; }
        public virtual required string GenerateInvoiceQueueName { get; set; }
        public virtual required string SendNotificationQueueName { get; set; }
    }
}
