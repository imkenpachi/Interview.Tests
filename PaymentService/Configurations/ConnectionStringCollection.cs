namespace PaymentService.Configurations
{
    public class ConnectionStringCollection
    {
        public virtual required string PaymentService { get; set; }
        public virtual required string AWSRegion { get; set; }
    }
}
